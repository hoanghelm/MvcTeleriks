// Telerik Dropdown Component - Unified dropdown management
class TelerikDropdownComponent {
    constructor(selector, viewModel, options = {}) {
        this.selector = selector;
        this.element = $(selector);
        this.viewModel = viewModel;
        this.options = this.mergeOptions(options);
        this.dropdown = null;
        this.isInitialized = false;
        this.dependencies = [];

        this.init();
    }

    mergeOptions(userOptions) {
        const defaultOptions = {
            dataTextField: "Value",
            dataValueField: "Code", // Keep uppercase Code as requested
            optionLabel: "Please select...",
            filter: "contains",
            minLength: 0,
            delay: 300,
            autoBind: true,
            cascadeFrom: null,
            cascadeFromField: null,
            dataSource: null,
            bindTo: null,
            bindToText: null,
            onChange: null,
            onDataBound: null,
            onCascade: null,
            loadOnInit: true,
            cacheEnabled: true,
            apiEndpoint: null,
            apiParams: null,
            staticData: null,
            placeholder: null,
            htmlAttributes: { class: "form-control", style: "width: 100%" },
            template: null,
            valueTemplate: null,
            headerTemplate: null,
            footerTemplate: null,
            enabled: true,
            suggest: false,
            clearButton: false,
            height: 200,
            animation: {
                open: { effects: "slideDown", duration: 200 },
                close: { effects: "slideUp", duration: 200 }
            },
            virtual: false,
            enforceMinLength: false,
            ignoreCase: true,
            popup: {
                appendTo: "body"
            }
        };

        return $.extend(true, {}, defaultOptions, userOptions);
    }

    init() {
        if (this.element.length === 0) {
            console.warn(`TelerikDropdownComponent: Element not found for selector '${this.selector}'`);
            return;
        }

        this.setupDataSource();
        this.createDropdown();
        this.bindToViewModel();
        this.setupCascading();

        if (this.options.loadOnInit && this.options.dataSource && !this.options.staticData) {
            this.refresh();
        }

        this.isInitialized = true;
    }

    setupDataSource() {
        if (this.options.staticData) {
            // Use static data
            this.options.dataSource = {
                data: this.options.staticData
            };
        } else if (this.options.apiEndpoint) {
            // Setup API-based data source
            this.options.dataSource = {
                transport: {
                    read: {
                        url: this.options.apiEndpoint,
                        dataType: "json",
                        data: () => {
                            let params = { ...this.options.apiParams };

                            // Add cascade parameters if applicable
                            if (this.options.cascadeFrom && this.options.cascadeFromField) {
                                const parentValue = this.getCascadeParentValue();
                                if (parentValue) {
                                    params[this.options.cascadeFromField] = parentValue;
                                }
                            }

                            return params;
                        }
                    }
                },
                schema: {
                    data: (response) => {
                        // Handle different response formats
                        if (response.success && response.data) {
                            return response.data;
                        } else if (Array.isArray(response)) {
                            return response;
                        }
                        return [];
                    }
                },
                serverFiltering: this.options.serverFiltering || false,
                serverSorting: this.options.serverSorting || false
            };
        } else if (!this.options.dataSource) {
            // Empty data source
            this.options.dataSource = {
                data: []
            };
        }
    }

    createDropdown() {
        const kendoOptions = {
            dataTextField: this.options.dataTextField,
            dataValueField: this.options.dataValueField,
            optionLabel: this.options.optionLabel,
            filter: this.options.filter,
            minLength: this.options.minLength,
            delay: this.options.delay,
            autoBind: this.options.autoBind,
            dataSource: this.options.dataSource,
            htmlAttributes: this.options.htmlAttributes,
            enabled: this.options.enabled,
            suggest: this.options.suggest,
            clearButton: this.options.clearButton,
            height: this.options.height,
            animation: this.options.animation,
            virtual: this.options.virtual,
            enforceMinLength: this.options.enforceMinLength,
            ignoreCase: this.options.ignoreCase,
            popup: this.options.popup,
            change: (e) => this.handleChange(e),
            dataBound: (e) => this.handleDataBound(e),
            cascade: (e) => this.handleCascade(e),
            open: (e) => this.handleOpen(e),
            close: (e) => this.handleClose(e)
        };

        // Add templates if provided
        if (this.options.template) kendoOptions.template = this.options.template;
        if (this.options.valueTemplate) kendoOptions.valueTemplate = this.options.valueTemplate;
        if (this.options.headerTemplate) kendoOptions.headerTemplate = this.options.headerTemplate;
        if (this.options.footerTemplate) kendoOptions.footerTemplate = this.options.footerTemplate;
        if (this.options.placeholder) kendoOptions.placeholder = this.options.placeholder;

        // Setup cascading
        if (this.options.cascadeFrom) {
            kendoOptions.cascadeFrom = this.options.cascadeFrom;
        }

        this.dropdown = this.element.kendoDropDownList(kendoOptions).data("kendoDropDownList");

        if (!this.dropdown) {
            console.error(`Failed to initialize Kendo DropDownList for selector '${this.selector}'`);
        }
    }

    bindToViewModel() {
        if (!this.viewModel || !this.options.bindTo) return;

        // Subscribe to ViewModel changes
        this.viewModel.subscribe(this.options.bindTo, (value) => {
            if (this.dropdown && this.dropdown.value() !== value) {
                this.dropdown.value(value);
            }
        });

        // Bind text field if specified
        if (this.options.bindToText) {
            this.viewModel.subscribe(this.options.bindToText, (text) => {
                if (this.dropdown) {
                    const item = this.dropdown.dataItem();
                    if (item && item[this.options.dataTextField] !== text) {
                        // Find item by text and set value
                        const dataSource = this.dropdown.dataSource;
                        const foundItem = dataSource.data().find(item =>
                            item[this.options.dataTextField] === text
                        );
                        if (foundItem) {
                            this.dropdown.value(foundItem[this.options.dataValueField]);
                        }
                    }
                }
            });
        }

        // Set initial value from ViewModel
        const initialValue = this.viewModel.data[this.options.bindTo];
        if (initialValue !== undefined && this.dropdown) {
            this.dropdown.value(initialValue);
        }
    }

    setupCascading() {
        if (!this.options.cascadeFrom) return;

        const parentDropdown = $(this.options.cascadeFrom).data("kendoDropDownList");
        if (parentDropdown) {
            this.dependencies.push({
                element: this.options.cascadeFrom,
                dropdown: parentDropdown
            });
        }
    }

    handleChange(e) {
        const value = e.sender.value();
        const dataItem = e.sender.dataItem();

        // Update ViewModel if bound
        if (this.viewModel && this.options.bindTo) {
            this.viewModel.data[this.options.bindTo] = value;
        }

        if (this.viewModel && this.options.bindToText && dataItem) {
            this.viewModel.data[this.options.bindToText] = dataItem[this.options.dataTextField];
        }

        // Clear dependent dropdowns when this value changes
        this.clearDependents();

        // Call custom onChange handler
        if (this.options.onChange && typeof this.options.onChange === 'function') {
            this.options.onChange(value, dataItem, e);
        }

        // Trigger validation if available
        if (this.viewModel && this.viewModel.validateProperty && this.options.bindTo) {
            this.viewModel.validateProperty(this.options.bindTo);
        }
    }

    handleDataBound(e) {
        // Call custom onDataBound handler
        if (this.options.onDataBound && typeof this.options.onDataBound === 'function') {
            this.options.onDataBound(e);
        }

        // Show skeleton loading effect end
        if (typeof TelerikSkeleton !== 'undefined') {
            TelerikSkeleton.hideDropdownSkeleton(this.selector);
        }
    }

    handleCascade(e) {
        // Call custom onCascade handler
        if (this.options.onCascade && typeof this.options.onCascade === 'function') {
            this.options.onCascade(e);
        }
    }

    handleOpen(e) {
        // Show skeleton loading if needed
        if (typeof TelerikSkeleton !== 'undefined' && this.dropdown.dataSource.total() === 0) {
            TelerikSkeleton.showDropdownSkeleton(this.selector);
        }
    }

    handleClose(e) {
        // Hide skeleton loading
        if (typeof TelerikSkeleton !== 'undefined') {
            TelerikSkeleton.hideDropdownSkeleton(this.selector);
        }
    }

    getCascadeParentValue() {
        if (!this.options.cascadeFrom) return null;

        const parentDropdown = $(this.options.cascadeFrom).data("kendoDropDownList");
        return parentDropdown ? parentDropdown.value() : null;
    }

    clearDependents() {
        // This would be implemented by specific dropdowns that have dependents
        // Override in specific implementations
    }

    // Public API methods
    refresh() {
        if (this.dropdown && this.dropdown.dataSource) {
            if (typeof TelerikSkeleton !== 'undefined') {
                TelerikSkeleton.showDropdownSkeleton(this.selector);
            }
            this.dropdown.dataSource.read();
        }
    }

    clear() {
        if (this.dropdown) {
            this.dropdown.value("");
            this.clearDependents();
        }
    }

    value(newValue = undefined) {
        if (!this.dropdown) return undefined;

        if (newValue !== undefined) {
            this.dropdown.value(newValue);
            return this;
        }
        return this.dropdown.value();
    }

    text() {
        if (!this.dropdown) return "";
        return this.dropdown.text();
    }

    dataItem() {
        if (!this.dropdown) return null;
        return this.dropdown.dataItem();
    }

    enable(enabled = true) {
        if (this.dropdown) {
            this.dropdown.enable(enabled);
        }
        return this;
    }

    disable() {
        return this.enable(false);
    }

    setDataSource(data) {
        if (this.dropdown) {
            this.dropdown.setDataSource(data);
        }
        return this;
    }

    destroy() {
        if (this.dropdown) {
            this.dropdown.destroy();
        }
        this.dependencies = [];
        this.isInitialized = false;
    }

    isEnabled() {
        return this.dropdown ? this.dropdown.element.prop("disabled") !== true : false;
    }

    isEmpty() {
        const value = this.value();
        return !value || value === "";
    }

    hasData() {
        return this.dropdown && this.dropdown.dataSource.total() > 0;
    }

    // Static helper methods
    static createFromViewBag(selector, viewBagData, options = {}) {
        const mergedOptions = {
            staticData: viewBagData,
            ...options
        };
        return new TelerikDropdownComponent(selector, null, mergedOptions);
    }

    static createApiDropdown(selector, apiEndpoint, viewModel = null, options = {}) {
        const mergedOptions = {
            apiEndpoint: apiEndpoint,
            ...options
        };
        return new TelerikDropdownComponent(selector, viewModel, mergedOptions);
    }

    static createCascadingDropdown(selector, parentSelector, apiEndpoint, cascadeField, viewModel = null, options = {}) {
        const mergedOptions = {
            apiEndpoint: apiEndpoint,
            cascadeFrom: parentSelector,
            cascadeFromField: cascadeField,
            autoBind: false,
            ...options
        };
        return new TelerikDropdownComponent(selector, viewModel, mergedOptions);
    }
}