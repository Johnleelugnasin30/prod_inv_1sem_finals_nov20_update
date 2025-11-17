/* ============================================
   CWTP Asset Inventory - Login Page Scripts
   ============================================ */

class LoginManager {
    constructor() {
        this.currentTab = 'user';
        this.init();
    }

    init() {
        this.setupTabSwitching();
        this.setupModals();
        this.setupFormHandlers();
        this.setupValidation();
    }

    // Tab Switching
    setupTabSwitching() {
        const tabButtons = document.querySelectorAll('.tab-btn');
        const tabContents = document.querySelectorAll('.tab-content');

        tabButtons.forEach(button => {
            button.addEventListener('click', () => {
                const targetTab = button.dataset.tab;
                
                tabButtons.forEach(btn => btn.classList.remove('active'));
                tabContents.forEach(content => content.classList.add('hidden'));
                
                button.classList.add('active');
                document.getElementById(`${targetTab}-login`).classList.remove('hidden');
                
                this.currentTab = targetTab;
            });
        });
    }

    // Modal Management
    setupModals() {
        document.querySelectorAll('[data-modal]').forEach(trigger => {
            trigger.addEventListener('click', (e) => {
                e.preventDefault();
                const modalId = trigger.dataset.modal;
                this.openModal(modalId);
            });
        });

        document.querySelectorAll('.modal-close').forEach(btn => {
            btn.addEventListener('click', () => {
                this.closeAllModals();
            });
        });

        document.querySelectorAll('.modal-overlay').forEach(overlay => {
            overlay.addEventListener('click', (e) => {
                if (e.target === overlay) {
                    this.closeAllModals();
                }
            });
        });

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.closeAllModals();
            }
        });
    }

    openModal(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.add('active');
            document.body.style.overflow = 'hidden';
        }
    }

    closeAllModals() {
        document.querySelectorAll('.modal-overlay').forEach(modal => {
            modal.classList.remove('active');
        });
        document.body.style.overflow = '';
    }

    // Form Handlers
    setupFormHandlers() {
        const userLoginForm = document.getElementById('user-login-form');
        if (userLoginForm) {
            userLoginForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleUserLogin(new FormData(userLoginForm));
            });
        }

        const adminLoginForm = document.getElementById('admin-login-form');
        if (adminLoginForm) {
            adminLoginForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleAdminLogin(new FormData(adminLoginForm));
            });
        }

        const createAccountForm = document.getElementById('create-account-form');
        if (createAccountForm) {
            createAccountForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleCreateAccount(new FormData(createAccountForm));
            });
        }

        const emailVerificationForm = document.getElementById('email-verification-form');
        if (emailVerificationForm) {
            emailVerificationForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleEmailVerification(new FormData(emailVerificationForm));
            });
        }

        const idVerificationForm = document.getElementById('id-verification-form');
        if (idVerificationForm) {
            idVerificationForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleIdVerification(new FormData(idVerificationForm));
            });
        }

        const forgotPasswordForm = document.getElementById('forgot-password-form');
        if (forgotPasswordForm) {
            forgotPasswordForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleForgotPassword(new FormData(forgotPasswordForm));
            });
        }

        const createAdminKeyForm = document.getElementById('create-admin-key-form');
        if (createAdminKeyForm) {
            createAdminKeyForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleCreateAdminKey(new FormData(createAdminKeyForm));
            });
        }
    }

    // Form Submission Handlers - Now with real backend integration
    async handleUserLogin(formData) {
        const email = formData.get('email');
        const password = formData.get('password');

        if (!this.validateEmail(email)) {
            this.showAlert('error', 'Please enter a valid email address');
            return;
        }

        const submitBtn = document.querySelector('#user-login-form button[type="submit"]');
        this.setLoading(submitBtn, true);

        try {
            const response = await fetch('/Login?handler=UserLogin', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'Verification code sent!');
                setTimeout(() => {
                    this.openModal('email-verification-modal');
                }, 1000);
            } else {
                this.showAlert('error', data.message || 'Login failed');
            }
        } catch (error) {
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('Login error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    async handleAdminLogin(formData) {
        const username = formData.get('username');
        const password = formData.get('password');
        const adminKey = formData.get('adminKey');

        if (!username || !password || !adminKey) {
            this.showAlert('error', 'All fields are required');
            return;
        }

        const submitBtn = document.querySelector('#admin-login-form button[type="submit"]');
        this.setLoading(submitBtn, true);

        try {
            const response = await fetch('/Login?handler=AdminLogin', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', 'Login successful! Redirecting...');
                setTimeout(() => {
                    window.location.href = data.redirect || '/ManageProduct';
                }, 1000);
            } else {
                this.showAlert('error', data.message || 'Admin login failed');
            }
        } catch (error) {
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('Admin login error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    async handleCreateAccount(formData) {
        const email = formData.get('email');
        const password = formData.get('password');
        const confirmPassword = formData.get('confirmPassword');
        const termsAccepted = formData.get('terms');

        if (!termsAccepted) {
            this.showAlert('error', 'Please accept the terms and conditions');
            return;
        }

        if (password !== confirmPassword) {
            this.showAlert('error', 'Passwords do not match');
            return;
        }

        const submitBtn = document.querySelector('#create-account-form button[type="submit"]');
        this.setLoading(submitBtn, true);

        try {
            const response = await fetch('/Login?handler=CreateAccount', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'Account created successfully!');
                this.closeAllModals();
                if (data.showIdVerification) {
                    setTimeout(() => {
                        this.openModal('id-verification-modal');
                    }, 1500);
                }
            } else {
                this.showAlert('error', data.message || 'Account creation failed');
            }
        } catch (error) {
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('Create account error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    async handleEmailVerification(formData) {
        const code = formData.get('verificationCode');

        if (code.length !== 6) {
            this.showAlert('error', 'Please enter a valid 6-digit code');
            return;
        }

        const submitBtn = document.querySelector('#email-verification-form button[type="submit"]');
        this.setLoading(submitBtn, true);

        try {
            const response = await fetch('/Login?handler=VerifyEmail', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'Email verified successfully!');
                this.closeAllModals();
                setTimeout(() => {
                    window.location.href = data.redirect || '/Index';
                }, 1500);
            } else {
                this.showAlert('error', data.message || 'Verification failed');
            }
        } catch (error) {
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('Email verification error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    async handleIdVerification(formData) {
        const idNumber = formData.get('idNumber');
        const email = formData.get('email');

        if (!idNumber || !email) {
            this.showAlert('error', 'Both fields are required');
            return;
        }

        const submitBtn = document.querySelector('#id-verification-form button[type="submit"]');
        this.setLoading(submitBtn, true);

        try {
            const response = await fetch('/Login?handler=VerifyId', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'ID verified successfully!');
                this.closeAllModals();
            } else {
                this.showAlert('error', data.message || 'ID verification failed');
            }
        } catch (error) {
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('ID verification error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    async handleForgotPassword(formData) {
        const email = formData.get('email');

        if (!this.validateEmail(email)) {
            this.showAlert('error', 'Please enter a valid email address');
            return;
        }

        const submitBtn = document.querySelector('#forgot-password-form button[type="submit"]');
        this.setLoading(submitBtn, true);

        try {
            const response = await fetch('/Login?handler=ForgotPassword', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'Password reset link sent!');
                this.closeAllModals();
            } else {
                this.showAlert('error', data.message || 'Failed to send reset link');
            }
        } catch (error) {
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('Forgot password error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    async handleCreateAdminKey(formData) {
        const masterKey = formData.get('masterKey');
        const newAdminKey = formData.get('newAdminKey');

        if (!masterKey || !newAdminKey) {
            this.showAlert('error', 'Both fields are required');
            return;
        }

        const submitBtn = document.querySelector('#create-admin-key-form button[type="submit"]');
        this.setLoading(submitBtn, true);

        try {
            const response = await fetch('/Login?handler=CreateAdminKey', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'Admin key created successfully!');
                this.closeAllModals();
            } else {
                this.showAlert('error', data.message || 'Failed to create admin key');
            }
        } catch (error) {
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('Create admin key error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    // Validation
    setupValidation() {
        document.querySelectorAll('input[type="email"]').forEach(input => {
            input.addEventListener('blur', () => {
                if (input.value && !this.validateEmail(input.value)) {
                    input.style.borderColor = '#dc3545';
                } else {
                    input.style.borderColor = '';
                }
            });
        });

        const verificationInput = document.getElementById('verificationCode');
        if (verificationInput) {
            verificationInput.addEventListener('input', (e) => {
                e.target.value = e.target.value.replace(/[^0-9]/g, '').slice(0, 6);
            });
        }
    }

    validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }

    // Helper Methods
    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    setLoading(button, isLoading) {
        if (isLoading) {
            button.classList.add('loading');
            button.disabled = true;
        } else {
            button.classList.remove('loading');
            button.disabled = false;
        }
    }

    showAlert(type, message) {
        document.querySelectorAll('.alert').forEach(alert => alert.remove());

        const alert = document.createElement('div');
        alert.className = `alert alert-${type}`;
        alert.innerHTML = `
            <span>${type === 'error' ? '⚠️' : type === 'success' ? '✓' : 'ℹ️'}</span>
            <span>${message}</span>
        `;

        const activeForm = document.querySelector('.tab-content:not(.hidden) form') ||
                          document.querySelector('.modal-overlay.active .modal-body form');
        
        if (activeForm) {
            activeForm.insertBefore(alert, activeForm.firstChild);
            setTimeout(() => alert.remove(), 5000);
        }
    }
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    new LoginManager();
});

// Prevent form resubmission on refresh
if (window.history.replaceState) {
    window.history.replaceState(null, null, window.location.href);
}