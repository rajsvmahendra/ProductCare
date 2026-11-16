/**
 * ProductCare — Application Interaction & Motion Controller
 * Specification: docs/DESIGN.md
 */

document.addEventListener('DOMContentLoaded', function () {
    // 1. Password Visibility Toggles
    const toggleButtons = document.querySelectorAll('.btn-password-toggle');
    toggleButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const targetId = this.getAttribute('data-target');
            const input = document.getElementById(targetId);
            if (!input) return;

            const icon = this.querySelector('i');
            if (input.type === 'password') {
                input.type = 'text';
                if (icon) {
                    icon.classList.remove('bi-eye');
                    icon.classList.add('bi-eye-slash');
                }
                this.setAttribute('aria-label', 'Hide password');
            } else {
                input.type = 'password';
                if (icon) {
                    icon.classList.remove('bi-eye-slash');
                    icon.classList.add('bi-eye');
                }
                this.setAttribute('aria-label', 'Show password');
            }
        });
    });

    // 2. Mobile Sidebar Drawer Toggler
    const sidebarToggle = document.getElementById('pcSidebarToggle');
    const sidebar = document.querySelector('.pc-portal-sidebar');
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            sidebar.classList.toggle('is-open');
        });

        document.addEventListener('click', function (e) {
            if (sidebar.classList.contains('is-open') && !sidebar.contains(e.target) && e.target !== sidebarToggle) {
                sidebar.classList.remove('is-open');
            }
        });
    }

    // 3. Subtle Hero Parallax Tilt (when reduced-motion is not requested)
    const isReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const mockupStage = document.querySelector('.pc-mockup-stage');
    const mockupWindow = document.querySelector('.pc-mockup-window');

    if (!isReducedMotion && mockupStage && mockupWindow) {
        mockupStage.addEventListener('mousemove', function (e) {
            const rect = mockupStage.getBoundingClientRect();
            const x = e.clientX - rect.left - rect.width / 2;
            const y = e.clientY - rect.top - rect.height / 2;
            
            const rotateX = -(y / rect.height) * 10;
            const rotateY = (x / rect.width) * 14;

            mockupWindow.style.transform = `rotateX(${rotateX}deg) rotateY(${rotateY}deg)`;
        });

        mockupStage.addEventListener('mouseleave', function () {
            mockupWindow.style.transform = '';
        });
    }

    // 4. Auto-dismiss Flash Alerts
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert && alert.classList.contains('show')) {
                alert.classList.remove('show');
                alert.classList.add('fade');
                setTimeout(() => alert.remove(), 300);
            }
        }, 6000);
    });
});
