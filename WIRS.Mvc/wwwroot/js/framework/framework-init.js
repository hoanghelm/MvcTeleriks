// Framework Initialization - Sets up services and global instances
$(document).ready(function() {
    // Initialize service container
    if (!window.ServiceContainer) {
        window.ServiceContainer = new ServiceContainer();
    }

    // Initialize services
    const apiService = new ApiService();
    const cacheService = new CacheService();
    const masterDataService = new MasterDataService(apiService, cacheService);

    // Register services
    window.ServiceContainer.register('apiService', apiService, true);
    window.ServiceContainer.register('cacheService', cacheService, true);
    window.ServiceContainer.register('masterDataService', masterDataService, true);

    // Initialize dropdown factory
    const dropdownFactory = new DropdownFactory(window.ServiceContainer);
    window.ServiceContainer.register('dropdownFactory', dropdownFactory, true);

    // Initialize dropdown service
    const dropdownService = new DropdownService();
    dropdownService.registerFactory('default', dropdownFactory);
    window.ServiceContainer.register('dropdownService', dropdownService, true);

    // Global access points for backward compatibility
    window.DropdownFactory = dropdownFactory;
    window.DropdownService = dropdownService;
    window.ApiService = apiService;
    window.MasterDataService = masterDataService;

    // Framework ready event
    $(document).trigger('framework:ready');
});

// Global helper functions for easy access
window.createDropdown = function(type, selector, ...args) {
    return window.DropdownService.createDropdown(type, selector, ...args);
};

window.getDropdown = function(selector) {
    return window.DropdownService.getInstance(selector);
};

window.destroyDropdown = function(selector) {
    return window.DropdownService.destroyInstance(selector);
};

// Framework utilities
window.FrameworkUtils = {
    // Convert ViewBag data format to standard format
    normalizeViewBagData: function(viewBagData) {
        if (!Array.isArray(viewBagData)) return [];

        return viewBagData.map(item => {
            // Handle different formats
            if (typeof item === 'string') {
                return { Code: item, Value: item };
            } else if (item.hasOwnProperty('Code') && item.hasOwnProperty('Value')) {
                return item;
            } else if (item.hasOwnProperty('id') && item.hasOwnProperty('name')) {
                return { Code: item.id, Value: item.name };
            } else if (item.hasOwnProperty('value') && item.hasOwnProperty('text')) {
                return { Code: item.value, Value: item.text };
            }
            return item;
        });
    },

    // Get API service instance
    getApiService: function() {
        return window.ServiceContainer.get('apiService');
    },

    // Get master data service instance
    getMasterDataService: function() {
        return window.ServiceContainer.get('masterDataService');
    },

    // Clear all dropdown caches
    clearAllCaches: function() {
        const cacheService = window.ServiceContainer.get('cacheService');
        cacheService.clear();
    },

    // Setup common validation rules
    setupValidationRules: function(viewModel) {
        // Add common validation rules
        viewModel.addValidationRule('email', {
            validate: (value) => {
                if (!value) return true; // Optional field
                const emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/i;
                return emailRegex.test(value) || 'Please enter a valid email address';
            }
        });

        viewModel.addValidationRule('empNo', {
            validate: (value) => {
                if (!value) return true; // Optional field
                return /^\d{8}$/.test(value) || 'Employee number must be 8 digits';
            }
        });

        return viewModel;
    },

    // Debounce function for performance optimization
    debounce: function(func, wait, immediate) {
        var timeout;
        return function() {
            var context = this, args = arguments;
            var later = function() {
                timeout = null;
                if (!immediate) func.apply(context, args);
            };
            var callNow = immediate && !timeout;
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
            if (callNow) func.apply(context, args);
        };
    },

    // Throttle function for performance optimization
    throttle: function(func, limit) {
        var inThrottle;
        return function() {
            var args = arguments;
            var context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }
};

// Global event handlers for framework
$(document).on('framework:ready', function() {
    console.log('WIRS Telerik Framework initialized successfully');
});

// Cleanup on page unload
$(window).on('beforeunload', function() {
    if (window.DropdownService) {
        window.DropdownService.destroyAll();
    }
});