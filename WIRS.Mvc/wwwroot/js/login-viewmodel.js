// Login Page View Model following MVVM pattern
const LoginViewModel = {
    // Data (Model)
    data: {
        isLoading: false,
        formValid: true,
        userId: '',
        password: '',
        errorMessage: ''
    },

    // DOM Elements (cached for performance)
    elements: {
        form: null,
        loginBtn: null,
        loginBtnText: null,
        userIdInput: null,
        passwordInput: null,
        errorAlert: null
    },

    // Initialize the view model
    init: function() {
        this.cacheElements();
        this.bindEvents();
        this.initializeView();
    },

    // Cache DOM elements
    cacheElements: function() {
        this.elements.form = $('#loginForm');
        this.elements.loginBtn = $('#loginBtn');
        this.elements.loginBtnText = $('#loginBtnText');
        this.elements.userIdInput = $('#UserId');
        this.elements.passwordInput = $('#Password');
        this.elements.errorAlert = $('#errorAlert');
    },

    // Bind event handlers
    bindEvents: function() {
        const self = this;

        // Form submission
        this.elements.form.on('submit', function(e) {
            self.handleFormSubmit(e);
        });

        // Input validation
        this.elements.userIdInput.add(this.elements.passwordInput).on('input', function() {
            self.handleInputChange($(this));
        });

        // Error alert close
        if (this.elements.errorAlert.length) {
            this.elements.errorAlert.find('button').on('click', function() {
                self.hideErrorMessage();
            });
        }
    },

    // Initialize view state
    initializeView: function() {
        // Focus first input
        this.elements.userIdInput.focus();

        // Auto-hide error message if exists
        if (this.elements.errorAlert.length) {
            setTimeout(() => {
                this.hideErrorMessage();
            }, 5000);
        }
    },

    // Event Handlers
    handleFormSubmit: function(e) {
        const form = e.target;

        // Validate form
        if (!this.validateForm(form)) {
            e.preventDefault();
            e.stopPropagation();
            return false;
        }

        // Set loading state
        this.setLoadingState(true);

        // Reset loading state after timeout (fallback)
        setTimeout(() => {
            this.setLoadingState(false);
        }, 10000);
    },

    handleInputChange: function($input) {
        // Remove error styling
        this.clearInputError($input);
        
        // Update data model
        const fieldName = $input.attr('id');
        if (fieldName === 'UserId') {
            this.data.userId = $input.val();
        } else if (fieldName === 'Password') {
            this.data.password = $input.val();
        }
    },

    // View State Management
    setLoadingState: function(isLoading) {
        this.data.isLoading = isLoading;

        if (isLoading) {
            this.elements.loginBtn.prop('disabled', true);
            this.elements.loginBtn.removeClass('hover:scale-105 login-button').addClass('opacity-75 cursor-not-allowed');
            this.elements.loginBtnText.html(this.getLoadingSpinner() + 'Signing in...');
        } else {
            this.elements.loginBtn.prop('disabled', false);
            this.elements.loginBtn.removeClass('opacity-75 cursor-not-allowed').addClass('hover:scale-105 login-button');
            this.elements.loginBtnText.html('Sign In');
        }
    },

    clearInputError: function($input) {
        $input.removeClass('border-red-400 bg-red-50/20');
        $input.siblings('.text-red-300').hide();
    },

    showInputError: function($input, message) {
        $input.addClass('border-red-400 bg-red-50/20');
        $input.siblings('.text-red-300').text(message).show();
    },

    hideErrorMessage: function() {
        if (this.elements.errorAlert.length) {
            this.elements.errorAlert.fadeOut(500);
        }
    },

    // Validation
    validateForm: function(form) {
        let isValid = true;

        // HTML5 validation
        if (!form.checkValidity()) {
            isValid = false;
        }

        // Custom validation
        if (!this.data.userId || this.data.userId.trim().length === 0) {
            this.showInputError(this.elements.userIdInput, 'User ID is required');
            isValid = false;
        }

        if (!this.data.password || this.data.password.trim().length === 0) {
            this.showInputError(this.elements.passwordInput, 'Password is required');
            isValid = false;
        }

        this.data.formValid = isValid;
        return isValid;
    },

    // Helper Methods
    getLoadingSpinner: function() {
        return '<svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white inline-block" fill="none" viewBox="0 0 24 24">' +
               '<circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>' +
               '<path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>' +
               '</svg>';
    },

    // Public API for external interaction
    api: {
        setLoading: function(isLoading) {
            LoginViewModel.setLoadingState(isLoading);
        },
        
        showError: function(message) {
            LoginViewModel.data.errorMessage = message;
            // Could implement dynamic error display here
        },
        
        clearErrors: function() {
            LoginViewModel.hideErrorMessage();
        }
    }
};

// Auto-initialize when DOM is ready
$(document).ready(function() {
    LoginViewModel.init();
});