/**
 * ProductCare — The Ownership Continuity Experience
 * Application Interaction Controller
 * Specification: docs/DESIGN.md
 */

document.addEventListener('DOMContentLoaded', function () {
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

    // 2. Mobile Sidebar Drawer Toggler
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

    // 3. Auto-dismissing Flash Alerts (after 7 seconds)
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 250);
        }, 7000);
    });
});
