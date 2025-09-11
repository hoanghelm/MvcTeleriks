// Standardized Telerik Notification System
var TelerikNotification = (function() {
    
    var _notification = null;
    
    function init() {
        // Create notification widget if it doesn't exist
        if (!_notification) {
            // Create a temporary element for the notification
            if ($("#globalNotification").length === 0) {
                $("body").append('<span id="globalNotification"></span>');
            }
            
            _notification = $("#globalNotification").kendoNotification({
                position: {
                    pinned: true,
                    top: 20,
                    right: 20
                },
                autoHideAfter: 5000,
                stacking: "up",
                templates: [{
                    type: "success",
                    template: "<div class='k-notification-success'><span class='k-icon k-i-check-circle'></span>#= message #</div>"
                }, {
                    type: "error", 
                    template: "<div class='k-notification-error'><span class='k-icon k-i-exclamation-circle'></span>#= message #</div>"
                }, {
                    type: "warning",
                    template: "<div class='k-notification-warning'><span class='k-icon k-i-warning'></span>#= message #</div>"
                }, {
                    type: "info",
                    template: "<div class='k-notification-info'><span class='k-icon k-i-info'></span>#= message #</div>"
                }]
            }).data("kendoNotification");
        }
    }
    
    function show(message, type) {
        type = type || 'info';
        init(); // Ensure notification is initialized
        
        if (_notification) {
            _notification.show({
                message: message
            }, type);
        } else {
            // Fallback to console if notification fails
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }
    
    function success(message) {
        show(message, 'success');
    }
    
    function error(message) {
        show(message, 'error');
    }
    
    function warning(message) {
        show(message, 'warning');
    }
    
    function info(message) {
        show(message, 'info');
    }
    
    function hide() {
        if (_notification) {
            _notification.hide();
        }
    }
    
    return {
        init: init,
        show: show,
        success: success,
        error: error,
        warning: warning,
        info: info,
        hide: hide
    };
    
})();

// Global functions for easy access
window.showNotification = TelerikNotification.show;
window.showSuccessNotification = TelerikNotification.success;
window.showErrorNotification = TelerikNotification.error;
window.showWarningNotification = TelerikNotification.warning;
window.showInfoNotification = TelerikNotification.info;