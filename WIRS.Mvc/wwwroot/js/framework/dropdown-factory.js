// Dropdown Factory for creating common dropdown types
class DropdownFactory {
    constructor(serviceContainer) {
        this.serviceContainer = serviceContainer;
        this.masterDataService = serviceContainer.get('masterDataService');
    }

    // Location dropdown
    createLocationDropdown(selector, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: '/MasterData/GetLocations',
            bindTo: 'selectedLocation',
            optionLabel: 'Select Location',
            placeholder: 'Select a location...',
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // User Role dropdown
    createUserRoleDropdown(selector, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: '/MasterData/GetUserRoles',
            bindTo: 'selectedUserRole',
            optionLabel: 'Select Role',
            placeholder: 'Select a user role...',
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // Sector dropdown
    createSectorDropdown(selector, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: '/MasterData/GetSectors',
            bindTo: 'selectedSector',
            optionLabel: 'Select Sector',
            placeholder: 'Select a sector...',
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // SBA dropdown
    createSbaDropdown(selector, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: '/MasterData/GetSbaList',
            bindTo: 'selectedSba',
            optionLabel: 'Select SBA',
            placeholder: 'Select an SBA...',
            onChange: (value, dataItem, e) => {
                // Clear dependent dropdowns
                this.clearSbaDependents(selector);
                if (options.onChange) options.onChange(value, dataItem, e);
            },
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // SBU dropdown (cascades from SBA)
    createSbuDropdown(selector, sbaSelector, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: '/MasterData/GetSbusBySba',
            cascadeFrom: sbaSelector,
            cascadeFromField: 'sbaCode',
            bindTo: 'selectedSbu',
            optionLabel: 'Select SBU',
            placeholder: 'Select an SBU...',
            autoBind: false,
            onChange: (value, dataItem, e) => {
                // Clear dependent dropdowns
                this.clearSbuDependents(selector);
                if (options.onChange) options.onChange(value, dataItem, e);
            },
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // Division dropdown (cascades from SBU)
    createDivisionDropdown(selector, sbuSelector, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: '/MasterData/GetDivisionsBySbu',
            cascadeFrom: sbuSelector,
            cascadeFromField: 'sbuCode',
            bindTo: 'selectedDivision',
            optionLabel: 'Select Division',
            placeholder: 'Select a division...',
            autoBind: false,
            onChange: (value, dataItem, e) => {
                // Clear dependent dropdowns
                this.clearDivisionDependents(selector);
                if (options.onChange) options.onChange(value, dataItem, e);
            },
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // Department dropdown (can cascade from either SBU or Division)
    createDepartmentDropdown(selector, parentSelector, parentType = 'sbu', viewModel = null, options = {}) {
        const apiEndpoint = parentType === 'division'
            ? '/MasterData/GetDepartmentsByDivision'
            : '/MasterData/GetDepartmentsBySbu';

        const cascadeField = parentType === 'division' ? 'divisionCode' : 'sbuCode';

        const defaultOptions = {
            apiEndpoint: apiEndpoint,
            cascadeFrom: parentSelector,
            cascadeFromField: cascadeField,
            bindTo: 'selectedDepartment',
            optionLabel: 'Select Department',
            placeholder: 'Select a department...',
            autoBind: false,
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // Yes/No dropdown
    createYesNoDropdown(selector, viewModel = null, options = {}) {
        const yesNoData = [
            { Code: "1", Value: "Yes" },
            { Code: "0", Value: "No" }
        ];

        const defaultOptions = {
            staticData: yesNoData,
            bindTo: 'selectedYesNo',
            optionLabel: 'Please select...',
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // Generic API dropdown
    createApiDropdown(selector, apiEndpoint, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: apiEndpoint,
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    // Method to create dropdowns from ViewBag data (for backward compatibility)
    createFromViewBag(selector, viewBagData, options = {}) {
        return TelerikDropdownComponent.createFromViewBag(selector, viewBagData, options);
    }

    // Helper methods to clear dependent dropdowns
    clearSbaDependents(sbaSelector) {
        const sbuSelector = sbaSelector.replace('Sba', 'Sbu');
        const divisionSelector = sbaSelector.replace('Sba', 'Division');
        const departmentSelector = sbaSelector.replace('Sba', 'Department');

        this.clearDropdown(sbuSelector);
        this.clearDropdown(divisionSelector);
        this.clearDropdown(departmentSelector);
    }

    clearSbuDependents(sbuSelector) {
        const divisionSelector = sbuSelector.replace('Sbu', 'Division');
        const departmentSelector = sbuSelector.replace('Sbu', 'Department');

        this.clearDropdown(divisionSelector);
        this.clearDropdown(departmentSelector);
    }

    clearDivisionDependents(divisionSelector) {
        const departmentSelector = divisionSelector.replace('Division', 'Department');
        this.clearDropdown(departmentSelector);
    }

    clearDropdown(selector) {
        const dropdown = $(selector).data("kendoDropDownList");
        if (dropdown) {
            dropdown.setDataSource([]);
            dropdown.value("");
        }
    }

    // Bulk dropdown creation
    createStandardIncidentDropdowns(sbaSelector, sbuSelector, divisionSelector, departmentSelector, locationSelector, viewModel = null) {
        const dropdowns = {};

        dropdowns.sba = this.createSbaDropdown(sbaSelector, viewModel);
        dropdowns.sbu = this.createSbuDropdown(sbuSelector, sbaSelector, viewModel);
        dropdowns.division = this.createDivisionDropdown(divisionSelector, sbuSelector, viewModel);
        dropdowns.department = this.createDepartmentDropdown(departmentSelector, sbuSelector, 'sbu', viewModel);
        dropdowns.location = this.createLocationDropdown(locationSelector, viewModel);

        return dropdowns;
    }

    createUserManagementDropdowns(userRoleSelector, sectorSelector, locationSelector, viewModel = null) {
        const dropdowns = {};

        dropdowns.userRole = this.createUserRoleDropdown(userRoleSelector, viewModel);
        dropdowns.sector = this.createSectorDropdown(sectorSelector, viewModel);
        dropdowns.location = this.createLocationDropdown(locationSelector, viewModel);

        return dropdowns;
    }
}

// Factory service registration helper
class DropdownService {
    constructor() {
        this.factories = new Map();
        this.instances = new Map();
    }

    registerFactory(name, factory) {
        this.factories.set(name, factory);
    }

    getFactory(name) {
        return this.factories.get(name);
    }

    createDropdown(type, selector, ...args) {
        const factory = this.factories.get('default');
        if (!factory) {
            throw new Error('Default dropdown factory not registered');
        }

        const methodName = `create${type.charAt(0).toUpperCase() + type.slice(1)}Dropdown`;
        if (typeof factory[methodName] === 'function') {
            const instance = factory[methodName](selector, ...args);
            this.instances.set(selector, instance);
            return instance;
        }

        throw new Error(`Dropdown type '${type}' not supported`);
    }

    getInstance(selector) {
        return this.instances.get(selector);
    }

    destroyInstance(selector) {
        const instance = this.instances.get(selector);
        if (instance && typeof instance.destroy === 'function') {
            instance.destroy();
            this.instances.delete(selector);
        }
    }

    destroyAll() {
        for (const [selector, instance] of this.instances) {
            if (typeof instance.destroy === 'function') {
                instance.destroy();
            }
        }
        this.instances.clear();
    }
}