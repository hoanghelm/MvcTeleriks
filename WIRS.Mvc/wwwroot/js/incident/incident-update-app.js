(function () {
    var app = angular.module('incidentUpdateApp', ['kendo.directives']);
    if (window.WIRSAngularSetup && window.WIRSAngularSetup.setupInterceptor) {
        window.WIRSAngularSetup.setupInterceptor(app);
    }
})();
