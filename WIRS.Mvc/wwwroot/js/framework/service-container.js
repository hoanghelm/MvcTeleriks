// Service container for dependency injection
class ServiceContainer {
    constructor() {
        this.services = new Map();
        this.singletons = new Map();
    }

    register(name, service, isSingleton = false) {
        this.services.set(name, { service, isSingleton });
    }

    get(name) {
        const serviceInfo = this.services.get(name);
        if (!serviceInfo) {
            throw new Error(`Service '${name}' not found`);
        }

        if (serviceInfo.isSingleton) {
            if (!this.singletons.has(name)) {
                const instance = typeof serviceInfo.service === 'function'
                    ? new serviceInfo.service()
                    : serviceInfo.service;
                this.singletons.set(name, instance);
            }
            return this.singletons.get(name);
        }

        return typeof serviceInfo.service === 'function'
            ? new serviceInfo.service()
            : serviceInfo.service;
    }

    has(name) {
        return this.services.has(name);
    }

    remove(name) {
        this.services.delete(name);
        this.singletons.delete(name);
    }

    clear() {
        this.services.clear();
        this.singletons.clear();
    }
}

// Global service container instance
window.ServiceContainer = window.ServiceContainer || new ServiceContainer();