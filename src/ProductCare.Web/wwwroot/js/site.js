/**
 * ProductCare — Digital Product Identity & Interaction Controller
 * Course: INT402 Modern Web Programming Tools and Techniques
 */

document.addEventListener('DOMContentLoaded', function () {
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // 1. Password Visibility Toggles (Bidirectional, Accessible, Safe, Delegated)
    // Ensures clicking the eye toggle NEVER submits any form, and controls ONLY its corresponding password input.
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.pc-toggle-password, .btn-password-toggle, .auth-pwd-toggle, .pwd-toggle-btn');
        if (!btn) return;

        // Ensure clicking eye icon never submits the form or bubbles to form handlers
        e.preventDefault();
        e.stopPropagation();

        const targetId = btn.getAttribute('data-target');
        let input = null;

        // 1. First search closest parent container (ensures toggle controls only its immediate sibling field)
        const container = btn.closest('.auth-field-box, .input-group, .position-relative, div');
        if (container) {
            input = container.querySelector('input[type="password"], input[type="text"]');
        }

        // 2. If not found in sibling container, fallback to document ID
        if (!input && targetId) {
            input = document.getElementById(targetId);
        }

        if (!input) return;

        const icon = btn.querySelector('i');
        const eyeOpen = btn.querySelector('.pc-eye-open');
        const eyeSlash = btn.querySelector('.pc-eye-slash');

        if (input.type === 'password') {
            input.type = 'text';
            if (icon) {
                icon.classList.remove('bi-eye');
                icon.classList.add('bi-eye-slash');
            }
            if (eyeOpen) eyeOpen.style.display = 'none';
            if (eyeSlash) eyeSlash.style.display = 'inline-block';
            btn.setAttribute('aria-label', 'Hide password');
            btn.setAttribute('title', 'Hide password');
        } else {
            input.type = 'password';
            if (icon) {
                icon.classList.remove('bi-eye-slash');
                icon.classList.add('bi-eye');
            }
            if (eyeOpen) eyeOpen.style.display = 'inline-block';
            if (eyeSlash) eyeSlash.style.display = 'none';
            btn.setAttribute('aria-label', 'Show password');
            btn.setAttribute('title', 'Show password');
        }
    });

    // 2. Copy to Clipboard Utility
    document.querySelectorAll('.pc-copy-btn').forEach(btn => {
        btn.addEventListener('click', async function (e) {
            e.preventDefault();
            const text = this.getAttribute('data-copy');
            if (!text) return;

            try {
                await navigator.clipboard.writeText(text);
                const originalHtml = this.innerHTML;
                this.innerHTML = '<i class="bi bi-check2 text-success"></i> Copied';
                setTimeout(() => {
                    this.innerHTML = originalHtml;
                }, 2000);
            } catch (err) {
                console.warn('Clipboard write failed:', err);
            }
        });
    });

    // 3. Public Mobile Navigation Hamburger Toggler
    const mobileNavBtn = document.getElementById('pcMobileNavToggle');
    const mobileNavDrawer = document.getElementById('pcMobileNavDrawer');
    const mobileNavClose = document.getElementById('pcMobileNavClose');

    if (mobileNavBtn && mobileNavDrawer) {
        mobileNavBtn.addEventListener('click', function () {
            mobileNavDrawer.style.transform = 'translateY(0)';
            document.body.style.overflow = 'hidden';
        });

        if (mobileNavClose) {
            mobileNavClose.addEventListener('click', function () {
                mobileNavDrawer.style.transform = 'translateY(-100%)';
                document.body.style.overflow = '';
            });
        }

        const drawerLinks = mobileNavDrawer.querySelectorAll('a');
        drawerLinks.forEach(link => {
            link.addEventListener('click', function () {
                mobileNavDrawer.style.transform = 'translateY(-100%)';
                document.body.style.overflow = '';
            });
        });
    }

    // 3b. Sliding Liquid-Glass Auth Portal Controller
    const authCard = document.getElementById('authCard');
    if (authCard) {
        const signUpTrigger = document.getElementById('signUpTrigger');
        const signInTrigger = document.getElementById('signInTrigger');
        const mobileRegisterTriggers = document.querySelectorAll('.pc-switch-register, .pc-mobile-switch-to-register');
        const mobileLoginTriggers = document.querySelectorAll('.pc-switch-login, .pc-mobile-switch-to-login');

        function switchToRegister() {
            authCard.classList.add('active');
            if (window.history && window.history.pushState) {
                window.history.pushState(null, '', '/Account/Register');
            }
            document.title = "Create Account — ProductCare";
        }

        function switchToLogin() {
            authCard.classList.remove('active');
            if (window.history && window.history.pushState) {
                window.history.pushState(null, '', '/Account/Login');
            }
            document.title = "Sign In — ProductCare";
        }

        if (signUpTrigger) {
            signUpTrigger.addEventListener('click', function (e) {
                e.preventDefault();
                switchToRegister();
            });
        }

        if (signInTrigger) {
            signInTrigger.addEventListener('click', function (e) {
                e.preventDefault();
                switchToLogin();
            });
        }

        mobileRegisterTriggers.forEach(btn => {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                switchToRegister();
            });
        });

        mobileLoginTriggers.forEach(btn => {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                switchToLogin();
            });
        });

        // Query param or hash check on page load
        const urlParams = new URLSearchParams(window.location.search);
        if (urlParams.get('mode') === 'register' || window.location.hash === '#register') {
            authCard.classList.add('active');
        } else if (urlParams.get('mode') === 'login' || window.location.hash === '#login') {
            authCard.classList.remove('active');
        }

        window.addEventListener('popstate', function () {
            if (window.location.pathname.toLowerCase().includes('register')) {
                authCard.classList.add('active');
                document.title = "Create Account — ProductCare";
            } else {
                authCard.classList.remove('active');
                document.title = "Sign In — ProductCare";
            }
        });
    }

    // 4. Portal Customer Workspace Navigation Controller (Desktop Collapse & Mobile Drawer)
    const portalLayout = document.getElementById('pcPortalLayout');
    const collapseToggle = document.getElementById('pcSidebarCollapseToggle');
    const sidebarToggle = document.getElementById('pcSidebarToggle');
    const sidebar = document.getElementById('pcPortalSidebar') || document.querySelector('.pc-portal-sidebar');
    const sidebarClose = document.getElementById('pcSidebarMobileClose');
    const backdrop = document.getElementById('pcPortalBackdrop');

    // Desktop: Restore Expanded State only on explicit URL parameter (Default is compact 60px rail; flyout remains closed on page load and reload)
    const flyout = document.getElementById('pcSidebarFlyout');
    if (portalLayout && window.innerWidth >= 992) {
        try {
            const urlParams = new URLSearchParams(window.location.search);
            const isExplicitlyExpanded = urlParams.get('expanded') === 'true';

            if (isExplicitlyExpanded) {
                if (flyout) flyout.classList.add('show');
            } else {
                if (flyout) flyout.classList.remove('show');
                localStorage.setItem('pc_sidebar_expanded', 'false');
            }
        } catch (e) {}
    }

    // QA / Screenshot scrollBottom handler
    try {
        const scrollBottomParam = new URLSearchParams(window.location.search).get('scrollBottom');
        if (scrollBottomParam === 'true') {
            const scrollToBottom = () => {
                const mainContainer = document.querySelector('.pc-portal-main') || document.querySelector('.pc-portal-content main');
                if (mainContainer) {
                    mainContainer.scrollTop = mainContainer.scrollHeight;
                }
                window.scrollTo(0, document.body.scrollHeight);
            };
            scrollToBottom();
            window.addEventListener('load', scrollToBottom);
            setTimeout(scrollToBottom, 250);
        }
    } catch (e) {}

    const railToggle = document.getElementById('pcRailExpandBtn');
    const flyoutClose = document.getElementById('pcFlyoutCloseBtn');

    function toggleDesktopFlyout() {
        if (!flyout) return;
        const willShow = !flyout.classList.contains('show');
        flyout.classList.toggle('show');
        try {
            localStorage.setItem('pc_sidebar_expanded', willShow ? 'true' : 'false');
        } catch (e) {}
    }

    function closeDesktopFlyout() {
        if (flyout) {
            flyout.classList.remove('show');
            try {
                localStorage.setItem('pc_sidebar_expanded', 'false');
            } catch (e) {}
        }
    }

    if (collapseToggle) {
        collapseToggle.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            toggleDesktopFlyout();
        });
    }

    if (railToggle) {
        railToggle.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            toggleDesktopFlyout();
        });
    }

    if (flyoutClose) {
        flyoutClose.addEventListener('click', function (e) {
            e.preventDefault();
            closeDesktopFlyout();
        });
    }

    // Automatically close expanded navigation flyout immediately on selecting any navigation link
    if (flyout) {
        flyout.addEventListener('click', function (e) {
            const navLink = e.target.closest('a');
            if (!navLink) return;

            // Do not intercept or close on non-navigation controls (like close button)
            if (navLink.id === 'pcFlyoutCloseBtn' || navLink.classList.contains('btn-close')) {
                return;
            }

            // Immediately collapse the expanded flyout on link navigation
            if (navLink.getAttribute('href')) {
                closeDesktopFlyout();
            }
        });
    }

    // Close flyout when clicking outside
    document.addEventListener('click', function (e) {
        if (flyout && flyout.classList.contains('show')) {
            if (!flyout.contains(e.target) && !e.target.closest('#pcRailExpandBtn') && !e.target.closest('#pcSidebarCollapseToggle')) {
                closeDesktopFlyout();
            }
        }
    });

    // 4b. ProductCare Digital Pass 3D Flip Card Interaction
    const passCard = document.getElementById('pcDigitalPassCard') || document.querySelector('.pc-pass-card-scene');
    if (passCard) {
        passCard.addEventListener('click', function (e) {
            // Do not flip if user is clicking an actionable anchor link or button inside the card
            if (e.target.closest('a') || e.target.closest('button')) {
                return;
            }
            passCard.classList.toggle('is-flipped');
        });
    }

    // Mobile: Slide-Out Drawer & Backdrop Controller
    function openMobileDrawer() {
        if (sidebar) sidebar.classList.add('show');
        if (backdrop) backdrop.classList.add('show');
        document.body.style.overflow = 'hidden';
    }

    function closeMobileDrawer() {
        if (sidebar) sidebar.classList.remove('show');
        if (backdrop) backdrop.classList.remove('show');
        document.body.style.overflow = '';
    }

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            openMobileDrawer();
        });
    }

    if (sidebarClose) {
        sidebarClose.addEventListener('click', function (e) {
            e.stopPropagation();
            closeMobileDrawer();
        });
    }

    if (backdrop) {
        backdrop.addEventListener('click', function () {
            closeMobileDrawer();
        });
    }

    if (sidebar) {
        sidebar.querySelectorAll('.pc-sidebar-link').forEach(link => {
            link.addEventListener('click', function () {
                closeDesktopFlyout();
                if (window.innerWidth < 992) {
                    closeMobileDrawer();
                }
            });
        });
    }

    // 5. Scroll-Driven Reveal Observer
    const revealElements = document.querySelectorAll('.reveal-on-scroll');
    if (revealElements.length > 0) {
        if (!prefersReducedMotion && 'IntersectionObserver' in window) {
            const revealObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('is-revealed');
                        observer.unobserve(entry.target);
                    }
                });
            }, {
                root: null,
                threshold: 0.12,
                rootMargin: '0px 0px -40px 0px'
            });

            revealElements.forEach(el => revealObserver.observe(el));
        } else {
            revealElements.forEach(el => el.classList.add('is-revealed'));
        }
    }

    // 6. Auto-dismissing Flash Alerts (after 7 seconds)
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 250);
        }, 7000);
    });

    // 7. Interactive Capability Showcase Tabs Switcher
    const showcaseTabs = document.querySelectorAll('.pc-showcase-tab');
    const showcasePanels = document.querySelectorAll('.pc-showcase-panel');

    if (showcaseTabs.length > 0 && showcasePanels.length > 0) {
        showcaseTabs.forEach(tab => {
            tab.addEventListener('click', function (e) {
                e.preventDefault();
                const targetId = this.getAttribute('data-target');

                showcaseTabs.forEach(t => {
                    t.classList.remove('active');
                    t.setAttribute('aria-selected', 'false');
                });
                this.classList.add('active');
                this.setAttribute('aria-selected', 'true');

                showcasePanels.forEach(panel => {
                    if (panel.id === targetId) {
                        panel.classList.add('active');
                    } else {
                        panel.classList.remove('active');
                    }
                });
            });
        });
    }

    // 8. Public Floating Navbar Elevation & Smooth Scroll Spy
    const publicNavbar = document.getElementById('pcPublicNavbar');
    const desktopNavPills = document.querySelectorAll('#pcMainDesktopNav .pc-nav-pill');

    if (publicNavbar) {
        const handleScroll = () => {
            if (window.scrollY > 24) {
                publicNavbar.classList.add('scrolled');
            } else {
                publicNavbar.classList.remove('scrolled');
            }
        };

        window.addEventListener('scroll', handleScroll, { passive: true });
        handleScroll();

        // Smooth Section Scroll with 84px Navbar Offset
        document.querySelectorAll('a[href^="#"], a[href^="/#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                const targetId = href.replace(/^\/?#/, '');
                if (!targetId) return;

                const targetEl = document.getElementById(targetId);
                if (targetEl) {
                    e.preventDefault();
                    const navbarOffset = 84;
                    const elementPosition = targetEl.getBoundingClientRect().top;
                    const offsetPosition = elementPosition + window.pageYOffset - navbarOffset;

                    window.scrollTo({
                        top: offsetPosition,
                        behavior: 'smooth'
                    });

                    // Update desktop pill active state
                    desktopNavPills.forEach(pill => {
                        if (pill.getAttribute('data-target') === targetId) {
                            pill.classList.add('active');
                        } else {
                            pill.classList.remove('active');
                        }
                    });
                }
            });
        });

        // IntersectionObserver for ScrollSpy across sections
        if ('IntersectionObserver' in window) {
            const sections = document.querySelectorAll('section[id]');
            const sectionObserver = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const currentId = entry.target.getAttribute('id');
                        desktopNavPills.forEach(pill => {
                            if (pill.getAttribute('data-target') === currentId) {
                                pill.classList.add('active');
                            } else {
                                pill.classList.remove('active');
                            }
                        });
                    }
                });
            }, {
                root: null,
                threshold: 0.35,
                rootMargin: '-80px 0px -40% 0px'
            });

            sections.forEach(sec => sectionObserver.observe(sec));
        }
    }
});
