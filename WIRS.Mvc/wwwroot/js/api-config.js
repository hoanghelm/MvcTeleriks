const ApiConfig = (function() {
    function getBasePath() {
        if (window.WIRS_CONFIG && window.WIRS_CONFIG.basePath !== undefined) {
            return window.WIRS_CONFIG.basePath;
        }

        const hostname = window.location.hostname.toLowerCase();
        const isLocalhost = hostname === 'localhost' || hostname === '127.0.0.1' || hostname.startsWith('192.168.');

        if (isLocalhost) {
            return '';
        }

        const pathArray = window.location.pathname.split('/');
        const basePath = pathArray[1] && pathArray[1].toLowerCase() === 'wirs' ? '/WIRS' : '';
        return basePath;
    }

    const BASE_PATH = getBasePath();

    function buildUrl(endpoint) {
        if (!endpoint) return '';

        const cleanEndpoint = endpoint.startsWith('/') ? endpoint : '/' + endpoint;
        return BASE_PATH + cleanEndpoint;
    }

    function ajax(options) {
        if (options.url) {
            options.url = buildUrl(options.url);
        }
        return $.ajax(options);
    }

    function get(url, data, success, dataType) {
        return $.get(buildUrl(url), data, success, dataType);
    }

    function post(url, data, success, dataType) {
        return $.post(buildUrl(url), data, success, dataType);
    }

    function getJson(url, data, success) {
        return $.getJSON(buildUrl(url), data, success);
    }

    return {
        BASE_PATH: BASE_PATH,
        buildUrl: buildUrl,
        ajax: ajax,
        get: get,
        post: post,
        getJSON: getJson
    };
})();

window.WIRSAngularSetup = window.WIRSAngularSetup || {};
window.WIRSAngularSetup.basePath = ApiConfig.BASE_PATH;
window.WIRSAngularSetup.initialized = false;

window.WIRSAngularSetup.setupInterceptor = function(app) {
    if (!app) return;

    app.constant('API_BASE_PATH', window.WIRSAngularSetup.basePath);

    if (!window.WIRSAngularSetup.initialized) {
        app.factory('httpInterceptor', ['API_BASE_PATH', function(API_BASE_PATH) {
            return {
                request: function(config) {
                    if (config.url && config.url.startsWith('/') && !config.url.startsWith('//')) {
                        if (API_BASE_PATH && !config.url.startsWith(API_BASE_PATH)) {
                            config.url = API_BASE_PATH + config.url;
                        }
                    }
                    return config;
                }
            };
        }]);
        window.WIRSAngularSetup.initialized = true;
    }

    app.config(['$httpProvider', function($httpProvider) {
        $httpProvider.interceptors.push('httpInterceptor');
    }]);
};
