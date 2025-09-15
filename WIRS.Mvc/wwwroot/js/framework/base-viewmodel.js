// Base ViewModel class with observable data and subscriber pattern
class BaseViewModel {
    constructor() {
        this.subscribers = {};
        this.data = this.createObservableData();
        this.validationErrors = {};
    }

    createObservableData() {
        return new Proxy({}, {
            set: (target, property, value) => {
                const oldValue = target[property];
                target[property] = value;

                // Only notify if value actually changed
                if (oldValue !== value) {
                    this.notifySubscribers(property, value, oldValue);
                }
                return true;
            }
        });
    }

    subscribe(property, callback) {
        if (!this.subscribers[property]) {
            this.subscribers[property] = [];
        }
        this.subscribers[property].push(callback);
    }

    unsubscribe(property, callback) {
        if (this.subscribers[property]) {
            this.subscribers[property] = this.subscribers[property].filter(cb => cb !== callback);
        }
    }

    notifySubscribers(property, value, oldValue) {
        if (this.subscribers[property]) {
            this.subscribers[property].forEach(callback => {
                try {
                    callback(value, oldValue, property);
                } catch (error) {
                    console.error(`Error in subscriber for property ${property}:`, error);
                }
            });
        }
    }

    // Validation support
    addValidationRule(property, rule) {
        if (!this.validationRules) {
            this.validationRules = {};
        }
        if (!this.validationRules[property]) {
            this.validationRules[property] = [];
        }
        this.validationRules[property].push(rule);
    }

    validateProperty(property) {
        if (!this.validationRules || !this.validationRules[property]) {
            return true;
        }

        const value = this.data[property];
        const errors = [];

        for (const rule of this.validationRules[property]) {
            if (rule.required && (!value || value === '')) {
                errors.push(rule.message || `${property} is required`);
            }
            if (rule.validate && typeof rule.validate === 'function') {
                const result = rule.validate(value);
                if (result !== true) {
                    errors.push(result);
                }
            }
        }

        if (errors.length > 0) {
            this.validationErrors[property] = errors;
            return false;
        } else {
            delete this.validationErrors[property];
            return true;
        }
    }

    validateAll() {
        if (!this.validationRules) return true;

        let isValid = true;
        for (const property in this.validationRules) {
            if (!this.validateProperty(property)) {
                isValid = false;
            }
        }
        return isValid;
    }

    getValidationErrors() {
        return this.validationErrors;
    }

    clearValidationErrors() {
        this.validationErrors = {};
    }
}