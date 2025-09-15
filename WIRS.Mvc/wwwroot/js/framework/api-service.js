// API Service for handling HTTP requests
class ApiService {
    constructor(baseUrl = '') {
        this.baseUrl = baseUrl;
        this.defaultOptions = {
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        };
    }

    async request(url, options = {}) {
        const config = {
            ...this.defaultOptions,
            ...options,
            headers: {
                ...this.defaultOptions.headers,
                ...options.headers
            }
        };

        try {
            const response = await fetch(this.baseUrl + url, config);

            if (!response.ok) {
                throw new Error(`HTTP Error: ${response.status} ${response.statusText}`);
            }

            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return await response.json();
            }

            return await response.text();
        } catch (error) {
            console.error('API Request failed:', error);
            throw error;
        }
    }

    async get(url, params = {}) {
        const queryString = Object.keys(params).length > 0
            ? '?' + new URLSearchParams(params).toString()
            : '';

        return this.request(url + queryString, {
            method: 'GET'
        });
    }

    async post(url, data = {}) {
        return this.request(url, {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async put(url, data = {}) {
        return this.request(url, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async delete(url) {
        return this.request(url, {
            method: 'DELETE'
        });
    }

    // Special method for form data
    async postForm(url, formData) {
        const options = {
            method: 'POST',
            body: formData
        };

        // Remove Content-Type header to let browser set it with boundary for FormData
        const headers = { ...this.defaultOptions.headers };
        delete headers['Content-Type'];
        options.headers = headers;

        return this.request(url, options);
    }

    // Method for jQuery AJAX compatibility (for existing code)
    jqueryAjax(options) {
        return new Promise((resolve, reject) => {
            const defaultOptions = {
                type: 'GET',
                dataType: 'json',
                success: resolve,
                error: (xhr, status, error) => {
                    console.error('jQuery AJAX Error:', error);
                    reject(new Error(error || 'Request failed'));
                }
            };

            $.ajax({ ...defaultOptions, ...options });
        });
    }
}

// Cache service for dropdown data
class CacheService {
    constructor() {
        this.cache = new Map();
        this.expiryTimes = new Map();
    }

    set(key, value, ttlMinutes = 30) {
        this.cache.set(key, value);
        this.expiryTimes.set(key, Date.now() + (ttlMinutes * 60 * 1000));
    }

    get(key) {
        const expiry = this.expiryTimes.get(key);
        if (expiry && Date.now() > expiry) {
            this.cache.delete(key);
            this.expiryTimes.delete(key);
            return null;
        }
        return this.cache.get(key) || null;
    }

    has(key) {
        return this.get(key) !== null;
    }

    clear(keyPattern = null) {
        if (keyPattern) {
            const regex = new RegExp(keyPattern);
            for (const key of this.cache.keys()) {
                if (regex.test(key)) {
                    this.cache.delete(key);
                    this.expiryTimes.delete(key);
                }
            }
        } else {
            this.cache.clear();
            this.expiryTimes.clear();
        }
    }
}

// Master data service for dropdown data management
class MasterDataService {
    constructor(apiService, cacheService) {
        this.apiService = apiService;
        this.cache = cacheService;
    }

    async getUserRoles() {
        const cacheKey = 'userRoles';
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get('/MasterData/GetUserRoles');
            console.log('GetUserRoles API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed UserRoles data:', data);
            this.cache.set(cacheKey, data);
        }

        return data;
    }

    async getLocations() {
        const cacheKey = 'locations';
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get('/MasterData/GetLocations');
            console.log('GetLocations API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed Locations data:', data);
            this.cache.set(cacheKey, data);
        }

        return data;
    }

    async getSectors() {
        const cacheKey = 'sectors';
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get('/MasterData/GetSectors');
            console.log('GetSectors API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed Sectors data:', data);
            this.cache.set(cacheKey, data);
        }

        return data;
    }

    async getSbaList() {
        const cacheKey = 'sbaList';
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get('/MasterData/GetSbaList');
            console.log('GetSbaList API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed SbaList data:', data);
            this.cache.set(cacheKey, data);
        }

        return data;
    }

    async getSbusBySba(sbaCode) {
        const cacheKey = `sbus_${sbaCode}`;
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get(`/MasterData/GetSbusBySba?sbaCode=${sbaCode}`);
            console.log('GetSbusBySba API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed SBUs data:', data);
            this.cache.set(cacheKey, data, 15); // Shorter cache for dependent data
        }

        return data;
    }

    async getDivisionsBySbu(sbuCode) {
        const cacheKey = `divisions_${sbuCode}`;
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get(`/MasterData/GetDivisionsBySbu?sbuCode=${sbuCode}`);
            console.log('GetDivisionsBySbu API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed Divisions data:', data);
            this.cache.set(cacheKey, data, 15);
        }

        return data;
    }

    async getDepartmentsBySbu(sbuCode) {
        const cacheKey = `departments_sbu_${sbuCode}`;
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get(`/MasterData/GetDepartmentsBySbu?sbuCode=${sbuCode}`);
            console.log('GetDepartmentsBySbu API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed Departments (SBU) data:', data);
            this.cache.set(cacheKey, data, 15);
        }

        return data;
    }

    async getDepartmentsByDivision(divisionCode) {
        const cacheKey = `departments_division_${divisionCode}`;
        let data = this.cache.get(cacheKey);

        if (!data) {
            const response = await this.apiService.get(`/MasterData/GetDepartmentsByDivision?divisionCode=${divisionCode}`);
            console.log('GetDepartmentsByDivision API response:', response);

            if (response.success && response.data) {
                data = response.data;
                // Ensure uppercase properties
                data = data.map(item => ({
                    Code: item.Code || item.code,
                    Value: item.Value || item.value || item.text || item.name
                }));
            } else {
                data = [];
            }

            console.log('Processed Departments (Division) data:', data);
            this.cache.set(cacheKey, data, 15);
        }

        return data;
    }

    // Clear cache for dependent dropdowns
    clearDependentCache(parentKey) {
        switch (parentKey) {
            case 'sba':
                this.cache.clear('sbus_');
                this.cache.clear('divisions_');
                this.cache.clear('departments_');
                break;
            case 'sbu':
                this.cache.clear('divisions_');
                this.cache.clear('departments_sbu_');
                break;
            case 'division':
                this.cache.clear('departments_division_');
                break;
        }
    }
}