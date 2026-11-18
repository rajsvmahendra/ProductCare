/**
 * ProductCare — Midnight Precision Visual Experience
 * Application Interaction Controller
 * Specification: docs/DESIGN.md v3.0
 */

document.addEventListener('DOMContentLoaded', function () {
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // 1. Password Visibility Toggles
    const toggleButtons = document.querySelectorAll('.pc-toggle-password, .btn-password-toggle');
    toggleButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const targetId = this.getAttribute('data-target');
            const input = document.getElementById(targetId);
            if (!input) return;

            const eyeOpen = this.querySelector('.pc-eye-open');
            const eyeSlash = this.querySelector('.pc-eye-slash');

            if (input.type === 'password') {
                input.type = 'text';
                if (eyeOpen) eyeOpen.style.display = 'none';
                if (eyeSlash) eyeSlash.style.display = 'inline-block';
                this.setAttribute('aria-label', 'Hide password');
            } else {
                input.type = 'password';
                if (eyeOpen) eyeOpen.style.display = 'inline-block';
                if (eyeSlash) eyeSlash.style.display = 'none';
                this.setAttribute('aria-label', 'Show password');
            }
        });
    });

    // 2. Public Mobile Navigation Hamburger Toggler
    const mobileNavBtn = document.getElementById('pcMobileNavToggle');
    const mobileNavDrawer = document.getElementById('pcMobileNavDrawer');
    const mobileNavClose = document.getElementById('pcMobileNavClose');

    if (mobileNavBtn && mobileNavDrawer) {
        mobileNavBtn.addEventListener('click', function () {
            mobileNavDrawer.classList.add('show');
            document.body.style.overflow = 'hidden';
        });

        if (mobileNavClose) {
            mobileNavClose.addEventListener('click', function () {
                mobileNavDrawer.classList.remove('show');
                document.body.style.overflow = '';
            });
        }

        // Close on clicking any drawer link
        const drawerLinks = mobileNavDrawer.querySelectorAll('a');
        drawerLinks.forEach(link => {
            link.addEventListener('click', function () {
                mobileNavDrawer.classList.remove('show');
                document.body.style.overflow = '';
            });
        });
    }

    // 3. Portal Mobile Sidebar Drawer Toggler
    const sidebarToggle = document.getElementById('pcSidebarToggle');
    const sidebar = document.querySelector('.pc-portal-sidebar');
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            sidebar.classList.toggle('show');
        });

        document.addEventListener('click', function (e) {
            if (sidebar.classList.contains('show') && !sidebar.contains(e.target) && e.target !== sidebarToggle) {
                sidebar.classList.remove('show');
            }
        });
    }

    // 4. Scroll-Driven Reveal Observer
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

    // 5. Hero Product Demonstration Mouse Parallax / Dynamic Lighting (Desktop)
    const demoContainer = document.querySelector('.pc-demo-container');
    if (demoContainer && !prefersReducedMotion && window.innerWidth >= 992) {
        demoContainer.addEventListener('mousemove', function (e) {
            const rect = demoContainer.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;

            const rotateX = ((y - centerY) / centerY) * -3;
            const rotateY = ((x - centerX) / centerX) * 4;

            demoContainer.style.transform = `perspective(1000px) rotateX(${rotateX.toFixed(2)}deg) rotateY(${rotateY.toFixed(2)}deg) translateY(-2px)`;
        });

        demoContainer.addEventListener('mouseleave', function () {
            demoContainer.style.transform = '';
        });
    }

    // 6. Auto-dismissing Flash Alerts (after 7 seconds)
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 250);
        }, 7000);
    });
});
