// Usage Examples for the new Telerik Dropdown Framework

// Example 1: Basic ViewModel with dropdowns
class ProductViewModel extends BaseViewModel {
    constructor() {
        super();
        this.data.selectedCategory = null;
        this.data.selectedSubCategory = null;
        this.data.productName = '';
        this.dropdowns = {};

        this.initializeDropdowns();
    }

    initializeDropdowns() {
        const factory = window.DropdownFactory;

        // Create category dropdown
        this.dropdowns.category = factory.createApiDropdown('#categoryDropDown', '/api/categories', this, {
            bindTo: 'selectedCategory',
            onChange: (value) => this.onCategoryChanged(value)
        });

        // Create subcategory dropdown that cascades from category
        this.dropdowns.subCategory = TelerikDropdownComponent.createCascadingDropdown(
            '#subCategoryDropDown',
            '#categoryDropDown',
            '/api/subcategories',
            'categoryCode',
            this,
            {
                bindTo: 'selectedSubCategory'
            }
        );
    }

    onCategoryChanged(value) {
        console.log('Category changed to:', value);
        // Any additional logic when category changes
    }
}

// Example 2: Converting existing ViewBag dropdown to framework
$(document).ready(function() {
    // OLD WAY (ViewBag-based):
    // var locations = @Html.Raw(Json.Serialize(ViewBag.Locations));
    // $("#locationDropdown").kendoDropDownList({
    //     dataSource: locations,
    //     dataTextField: "Value",
    //     dataValueField: "Code"
    // });

    // NEW WAY (Framework-based):
    $(document).on('framework:ready', function() {
        const locationDropdown = window.DropdownFactory.createLocationDropdown('#locationDropdown');
    });
});

// Example 3: Creating a custom dropdown type
class CustomDropdownFactory extends DropdownFactory {
    // Extend the factory to add custom dropdown types
    createEmploymentTypeDropdown(selector, viewModel = null, options = {}) {
        const employmentTypes = [
            { Code: "FT", Value: "Full Time" },
            { Code: "PT", Value: "Part Time" },
            { Code: "CT", Value: "Contract" },
            { Code: "TMP", Value: "Temporary" }
        ];

        const defaultOptions = {
            staticData: employmentTypes,
            bindTo: 'selectedEmploymentType',
            optionLabel: 'Select Employment Type',
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }

    createPriorityDropdown(selector, viewModel = null, options = {}) {
        const defaultOptions = {
            apiEndpoint: '/MasterData/GetPriorities',
            bindTo: 'selectedPriority',
            optionLabel: 'Select Priority',
            placeholder: 'Choose priority level...',
            ...options
        };

        return new TelerikDropdownComponent(selector, viewModel, defaultOptions);
    }
}

// Example 4: Complete form with all dropdown types
class IncidentFormViewModel extends BaseViewModel {
    constructor() {
        super();
        this.setupData();
        this.setupValidation();
        this.setupDropdowns();
    }

    setupData() {
        // All form data properties
        this.data.selectedSba = null;
        this.data.selectedSbu = null;
        this.data.selectedDivision = null;
        this.data.selectedDepartment = null;
        this.data.selectedLocation = null;
        this.data.selectedPriority = null;
        this.data.isUrgent = null;
    }

    setupValidation() {
        this.addValidationRule('selectedSba', { required: true, message: 'SBA is required' });
        this.addValidationRule('selectedSbu', { required: true, message: 'SBU is required' });
        this.addValidationRule('selectedLocation', { required: true, message: 'Location is required' });
    }

    setupDropdowns() {
        const factory = window.DropdownFactory;

        // Create cascading organizational dropdowns
        const orgDropdowns = factory.createStandardIncidentDropdowns(
            '#ddlSba',
            '#ddlSbu',
            '#ddlDivision',
            '#ddlDepartment',
            '#ddlLocation',
            this
        );

        // Create other dropdowns
        this.dropdowns = {
            ...orgDropdowns,
            priority: factory.createApiDropdown('#ddlPriority', '/api/priorities', this, {
                bindTo: 'selectedPriority'
            }),
            isUrgent: factory.createYesNoDropdown('#ddlIsUrgent', this, {
                bindTo: 'isUrgent'
            })
        };

        // Subscribe to changes for business logic
        this.subscribe('selectedPriority', (value) => {
            if (value === 'HIGH') {
                this.data.isUrgent = '1';
            }
        });
    }
}

// Example 5: Bulk operations
class BulkDropdownManager {
    static initializeUserManagementPage() {
        $(document).on('framework:ready', function() {
            const factory = window.DropdownFactory;

            // Create all user management dropdowns at once
            const dropdowns = factory.createUserManagementDropdowns(
                '#ddlUserRole',
                '#ddlSector',
                '#ddlLocation'
            );

            // Store for later use
            window.userManagementDropdowns = dropdowns;
        });
    }

    static clearAllDropdowns() {
        if (window.userManagementDropdowns) {
            Object.values(window.userManagementDropdowns).forEach(dropdown => {
                dropdown.clear();
            });
        }
    }

    static refreshAllDropdowns() {
        if (window.userManagementDropdowns) {
            Object.values(window.userManagementDropdowns).forEach(dropdown => {
                dropdown.refresh();
            });
        }
    }
}

// Example 6: Advanced usage with custom templates
$(document).ready(function() {
    $(document).on('framework:ready', function() {
        // Dropdown with custom template
        const customTemplateDropdown = new TelerikDropdownComponent('#customDropdown', null, {
            apiEndpoint: '/api/employees',
            dataTextField: 'name',
            dataValueField: 'empNo',
            template: '<div class="employee-item">' +
                     '<img src="/images/employees/#: empNo #.jpg" alt="photo" />' +
                     '<span class="employee-name">#: name #</span>' +
                     '<span class="employee-dept">#: department #</span>' +
                     '</div>',
            valueTemplate: '<span class="selected-employee">#: name # (#: empNo #)</span>',
            height: 300
        });

        // Dropdown with server filtering
        const serverFilterDropdown = new TelerikDropdownComponent('#serverFilterDropdown', null, {
            apiEndpoint: '/api/products/search',
            serverFiltering: true,
            filter: 'contains',
            minLength: 3,
            delay: 500
        });
    });
});

// Example 7: Error handling and loading states
class RobustDropdownViewModel extends BaseViewModel {
    constructor() {
        super();
        this.setupDropdownsWithErrorHandling();
    }

    setupDropdownsWithErrorHandling() {
        $(document).on('framework:ready', function() {
            try {
                const factory = window.DropdownFactory;

                const dropdown = factory.createLocationDropdown('#locationDropdown', this, {
                    onDataBound: (e) => {
                        // Hide loading indicator
                        this.hideLoadingIndicator('#locationDropdown');
                    },
                    onChange: (value, dataItem, e) => {
                        // Log change for audit
                        console.log('Location changed:', { value, dataItem });
                    }
                });

                // Show loading indicator
                this.showLoadingIndicator('#locationDropdown');

            } catch (error) {
                console.error('Error initializing dropdowns:', error);
                this.showErrorMessage('Failed to initialize dropdowns');
            }
        }.bind(this));
    }

    showLoadingIndicator(selector) {
        $(selector).closest('.form-group').addClass('loading');
    }

    hideLoadingIndicator(selector) {
        $(selector).closest('.form-group').removeClass('loading');
    }

    showErrorMessage(message) {
        // Implement your error display logic
        console.error(message);
    }
}

// Example 8: Migration helper for existing code
class MigrationHelper {
    // Helper to convert old jQuery-based dropdown initialization
    static migrateDropdown(selector, oldConfig) {
        const newConfig = {
            apiEndpoint: oldConfig.dataSource?.transport?.read?.url,
            dataTextField: oldConfig.dataTextField || 'Value',
            dataValueField: oldConfig.dataValueField || 'Code',
            optionLabel: oldConfig.optionLabel,
            onChange: oldConfig.change
        };

        return new TelerikDropdownComponent(selector, null, newConfig);
    }

    // Batch migration
    static migrateExistingDropdowns(dropdownConfigs) {
        const migratedDropdowns = {};

        for (const [selector, config] of Object.entries(dropdownConfigs)) {
            migratedDropdowns[selector] = this.migrateDropdown(selector, config);
        }

        return migratedDropdowns;
    }
}