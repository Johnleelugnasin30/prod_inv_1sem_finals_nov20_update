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
<<<<<<< HEAD
            
            // Re-attach form handlers when modal opens (in case form wasn't in DOM during init)
            this.attachFormHandlers();
        }
    }
    
    attachFormHandlers() {
        // Re-attach create admin key form handler when modal opens
        const createAdminKeyForm = document.getElementById('create-admin-key-form');
        if (createAdminKeyForm && !createAdminKeyForm.hasAttribute('data-handler-attached')) {
            createAdminKeyForm.addEventListener('submit', (e) => {
                e.preventDefault();
                e.stopPropagation();
                console.log('Form submit event triggered');
                this.handleCreateAdminKey(new FormData(createAdminKeyForm));
            });
            createAdminKeyForm.setAttribute('data-handler-attached', 'true');
=======
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
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
<<<<<<< HEAD
        const email = formData.get('Email') || formData.get('email');
        const password = formData.get('Password') || formData.get('password');
=======
        const email = formData.get('email');
        const password = formData.get('password');
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180

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
<<<<<<< HEAD
                    'Content-Type': 'application/x-www-form-urlencoded'
=======
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
                },
                body: new URLSearchParams(formData)
            });

<<<<<<< HEAD
            if (!response.ok) {
                // Try to get error message from response
                let errorMessage = `HTTP error! status: ${response.status}`;
                try {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorMessage;
                } catch (e) {
                    const errorText = await response.text();
                    errorMessage = errorText || errorMessage;
                }
                throw new Error(errorMessage);
            }

=======
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
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
<<<<<<< HEAD
            const errorMsg = error.message || 'Connection error. Please try again.';
            this.showAlert('error', errorMsg);
=======
            this.showAlert('error', 'Connection error. Please try again.');
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
            console.error('Login error:', error);
        } finally {
            this.setLoading(submitBtn, false);
        }
    }

    async handleAdminLogin(formData) {
<<<<<<< HEAD
        const username = formData.get('Username') || formData.get('username');
        const password = formData.get('Password') || formData.get('password');
        const adminKey = formData.get('AdminKey') || formData.get('adminKey');
=======
        const username = formData.get('username');
        const password = formData.get('password');
        const adminKey = formData.get('adminKey');
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180

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
<<<<<<< HEAD
                    'Content-Type': 'application/x-www-form-urlencoded'
=======
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
                },
                body: new URLSearchParams(formData)
            });

<<<<<<< HEAD
            if (!response.ok) {
                // Try to get error message from response
                let errorMessage = `HTTP error! status: ${response.status}`;
                try {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorMessage;
                } catch (e) {
                    const errorText = await response.text();
                    errorMessage = errorText || errorMessage;
                }
                throw new Error(errorMessage);
            }

=======
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
            const data = await response.json();

            if (data.success) {
                this.showAlert('success', 'Login successful! Redirecting...');
<<<<<<< HEAD
                // Redirect immediately or use provided redirect URL
                const redirectUrl = data.redirect || '/AdminDashboard';
                setTimeout(() => {
                    window.location.href = redirectUrl;
                }, 500);
=======
                setTimeout(() => {
                    window.location.href = data.redirect || '/ManageProduct';
                }, 1000);
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
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
<<<<<<< HEAD
            // Ensure VerificationCode is sent with correct name
            const formDataToSend = new FormData();
            formDataToSend.append('VerificationCode', code.trim());

            const response = await fetch('/Login?handler=VerifyEmail', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: new URLSearchParams(formDataToSend)
            });

            if (!response.ok) {
                // Try to get error message from response
                let errorMessage = `HTTP error! status: ${response.status}`;
                try {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorMessage;
                } catch (e) {
                    const errorText = await response.text();
                    errorMessage = errorText || errorMessage;
                }
                throw new Error(errorMessage);
            }

=======
            const response = await fetch('/Login?handler=VerifyEmail', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: new URLSearchParams(formData)
            });

>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'Email verified successfully!');
                this.closeAllModals();
<<<<<<< HEAD
                // Redirect to appropriate dashboard based on user role
                const redirectUrl = data.redirect || '/UserView';
                console.log('Redirecting to:', redirectUrl);
                // Redirect immediately
                window.location.href = redirectUrl;
=======
                setTimeout(() => {
                    window.location.href = data.redirect || '/Index';
                }, 1500);
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
            } else {
                this.showAlert('error', data.message || 'Verification failed');
            }
        } catch (error) {
<<<<<<< HEAD
            const errorMsg = error.message || 'Connection error. Please try again.';
            this.showAlert('error', errorMsg);
=======
            this.showAlert('error', 'Connection error. Please try again.');
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
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
<<<<<<< HEAD
        console.log('handleCreateAdminKey called', formData);
        
        const masterKey = formData.get('MasterKey') || formData.get('masterKey');
        const newAdminKey = formData.get('NewAdminKey') || formData.get('newAdminKey');

        console.log('MasterKey:', masterKey, 'NewAdminKey:', newAdminKey);
=======
        const masterKey = formData.get('masterKey');
        const newAdminKey = formData.get('newAdminKey');
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180

        if (!masterKey || !newAdminKey) {
            this.showAlert('error', 'Both fields are required');
            return;
        }

        const submitBtn = document.querySelector('#create-admin-key-form button[type="submit"]');
<<<<<<< HEAD
        if (submitBtn) {
            this.setLoading(submitBtn, true);
        }

        try {
            // Create FormData with correct field names
            const formDataToSend = new FormData();
            formDataToSend.append('MasterKey', masterKey.trim());
            formDataToSend.append('NewAdminKey', newAdminKey.trim());

            console.log('Sending request to /Login?handler=CreateAdminKey');

=======
        this.setLoading(submitBtn, true);

        try {
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
            const response = await fetch('/Login?handler=CreateAdminKey', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
<<<<<<< HEAD
                body: new URLSearchParams(formDataToSend)
            });

            console.log('Response status:', response.status);
            console.log('Response content-type:', response.headers.get('content-type'));

            // Check if response is JSON
            const contentType = response.headers.get('content-type');
            let data;
            
            if (contentType && contentType.includes('application/json')) {
                data = await response.json();
            } else {
                // Response is not JSON (probably HTML error page)
                const text = await response.text();
                console.error('Non-JSON response received:', text.substring(0, 200));
                
                if (!response.ok) {
                    throw new Error(`Server error (${response.status}): Received HTML instead of JSON. Please check the server logs.`);
                } else {
                    throw new Error('Server returned non-JSON response. Please check the server configuration.');
                }
            }

            console.log('Response data:', data);

            if (data.success) {
                this.showAlert('success', data.message || 'Admin key created successfully!');
                // Clear form
                const masterKeyInput = document.getElementById('masterKey');
                const newAdminKeyInput = document.getElementById('newAdminKey');
                if (masterKeyInput) masterKeyInput.value = '';
                if (newAdminKeyInput) newAdminKeyInput.value = '';
                // Close modal after a short delay
                setTimeout(() => {
                    this.closeAllModals();
                }, 1500);
=======
                body: new URLSearchParams(formData)
            });

            const data = await response.json();

            if (data.success) {
                this.showAlert('success', data.message || 'Admin key created successfully!');
                this.closeAllModals();
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
            } else {
                this.showAlert('error', data.message || 'Failed to create admin key');
            }
        } catch (error) {
<<<<<<< HEAD
            const errorMsg = error.message || 'Connection error. Please try again.';
            this.showAlert('error', errorMsg);
            console.error('Create admin key error:', error);
        } finally {
            if (submitBtn) {
                this.setLoading(submitBtn, false);
            }
=======
            this.showAlert('error', 'Connection error. Please try again.');
            console.error('Create admin key error:', error);
        } finally {
            this.setLoading(submitBtn, false);
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
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
<<<<<<< HEAD
        // Try multiple ways to get the anti-forgery token
        let token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (!token) {
            token = document.querySelector('input[name="__RequestVerificationToken"]')?.getAttribute('value');
        }
        if (!token) {
            // Try to get from form
            const form = document.querySelector('form[method="post"]');
            if (form) {
                const formToken = form.querySelector('input[name="__RequestVerificationToken"]');
                token = formToken ? formToken.value : '';
            }
        }
        return token || '';
=======
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
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

<<<<<<< HEAD
// Initialize Login Manager
document.addEventListener('DOMContentLoaded', () => {
    if (typeof LoginManager !== 'undefined') {
        window.loginManager = new LoginManager();
        console.log('Login Manager initialized successfully');
    } else {
        console.error('LoginManager class not found! Check if login.js is loaded.');
    }
=======
// Initialize
document.addEventListener('DOMContentLoaded', () => {
    new LoginManager();
>>>>>>> 30da2921ccd7506cfc34fa4ebe511e442f353180
});

// Prevent form resubmission on refresh
if (window.history.replaceState) {
    window.history.replaceState(null, null, window.location.href);
}