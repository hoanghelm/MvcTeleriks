(function () {
    var app = angular.module('incidentApp', ['kendo.directives']);
    if (window.WIRSAngularSetup && window.WIRSAngularSetup.setupInterceptor) {
        window.WIRSAngularSetup.setupInterceptor(app);
    }
})();
