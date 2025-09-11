/**
 * Employee Search Component
 * Reusable JavaScript module for employee search functionality
 */

var EmployeeSearchComponent = (function() {
    'use strict';

    // Private variables
    var _instances = {};
    
    /**
     * Constructor for EmployeeSearch instance
     */
    function EmployeeSearch(options) {
        this.options = $.extend({
            modalId: 'employeeSearchModal',
            gridId: 'employeeSearchGrid',
            onSelectCallback: null,
            apiEndpoint: '/User/SearchEmployees',
            enableAdvancedFilters: true,
            enableExport: true,
            autoSearch: false,
            pageSize: 25
        }, options);
        
        this.elements = {};
        this.selectedEmployee = null;
        
        this.init();
    }

    EmployeeSearch.prototype = {
        /**
         * Initialize the component
         */
        init: function() {
            this.initializeElements();
            this.bindEvents();
            this.setupGrid();
            
            // Store instance for global access
            _instances[this.options.modalId] = this;
        },

        /**
         * Initialize DOM elements
         */
        initializeElements: function() {
            this.elements.modal = $("#" + this.options.modalId).data("kendoWindow");
            this.elements.grid = $("#" + this.options.gridId).data("kendoGrid");
            this.elements.splitter = $("#employeeSearchSplitter").data("kendoSplitter");
            
            // Filter elements
            this.elements.filterEmployeeId = $("#filterEmployeeId").data("kendoTextBox");
            this.elements.filterEmployeeName = $("#filterEmployeeName").data("kendoTextBox");
            this.elements.filterEmployeeEmail = $("#filterEmployeeEmail").data("kendoTextBox");
            this.elements.filterDepartment = $("#filterDepartment").data("kendoDropDownList");
            
            // Action buttons
            this.elements.btnSelect = $("#btnSelectEmployee").data("kendoButton");
            this.elements.btnCancel = $("#btnCancelSearch").data("kendoButton");
            this.elements.btnSearch = $("#btnAdvancedSearch").data("kendoButton");
            this.elements.btnClear = $("#btnClearFilters").data("kendoButton");
            this.elements.btnExport = $("#btnExportResults").data("kendoButton");
            
            // Info elements
            this.elements.resultsCount = $("#resultsCount");
            this.elements.selectedEmployeeInfo = $("#selectedEmployeeInfo");
            this.elements.selectedEmployeeName = $("#selectedEmployeeName");
            this.elements.selectedEmployeeId = $("#selectedEmployeeId");
            this.elements.selectedEmployeeEmail = $("#selectedEmployeeEmail");
        },

        /**
         * Bind event handlers
         */
        bindEvents: function() {
            var self = this;
            
            // Grid selection change
            if (this.elements.grid) {
                this.elements.grid.bind("change", function() {
                    self.onGridSelectionChange();
                });
            }
            
            // Enter key search
            var filterInputs = [
                this.elements.filterEmployeeId,
                this.elements.filterEmployeeName,
                this.elements.filterEmployeeEmail
            ];
            
            filterInputs.forEach(function(input) {
                if (input) {
                    input.bind("keypress", function(e) {
                        if (e.which === 13) {
                            self.performSearch();
                        }
                    });
                }
            });
        },

        /**
         * Setup grid configuration
         */
        setupGrid: function() {
            if (!this.elements.grid) return;
            
            var self = this;
            
            // Configure data source
            var dataSource = new kendo.data.DataSource({
                pageSize: this.options.pageSize,
                schema: {
                    model: {
                        id: "EmployeeId",
                        fields: {
                            EmployeeId: { type: "string" },
                            EmployeeName: { type: "string" },
                            Email: { type: "string" },
                            Department: { type: "string" },
                            Designation: { type: "string" },
                            Status: { type: "string" }
                        }
                    },
                    total: function(response) {
                        return response.totalCount || response.length || 0;
                    },
                    data: function(response) {
                        return response.data || response;
                    }
                },
                requestEnd: function(e) {
                    self.updateResultsCount(e.response);
                }
            });
            
            this.elements.grid.setDataSource(dataSource);
        },

        /**
         * Open the modal
         */
        open: function() {
            if (this.elements.modal) {
                this.clearSelection();
                this.clearFilters();
                this.elements.modal.center().open();
            }
        },

        /**
         * Close the modal
         */
        close: function() {
            if (this.elements.modal) {
                this.elements.modal.close();
            }
        },

        /**
         * Perform employee search
         */
        performSearch: function() {
            var filters = this.getFilterValues();
            
            if (!this.hasValidFilters(filters)) {
                this.showNotification("Please enter at least one search criteria", "warning");
                return;
            }
            
            this.searchEmployees(filters);
        },

        /**
         * Get current filter values
         */
        getFilterValues: function() {
            return {
                EmployeeId: this.elements.filterEmployeeId ? this.elements.filterEmployeeId.value() : "",
                EmployeeName: this.elements.filterEmployeeName ? this.elements.filterEmployeeName.value() : "",
                Email: this.elements.filterEmployeeEmail ? this.elements.filterEmployeeEmail.value() : "",
                Department: this.elements.filterDepartment ? this.elements.filterDepartment.value() : "",
                PageNo: 1,
                PageSize: this.options.pageSize
            };
        },

        /**
         * Validate if at least one filter has value
         */
        hasValidFilters: function(filters) {
            return filters.EmployeeId || filters.EmployeeName || filters.Email || filters.Department;
        },

        /**
         * Search employees via API
         */
        searchEmployees: function(filters) {
            var self = this;
            
            this.showLoading(true);
            
            $.ajax({
                url: this.options.apiEndpoint,
                type: 'POST',
                data: JSON.stringify(filters),
                contentType: 'application/json',
                success: function(response) {
                    self.showLoading(false);
                    
                    if (response.success) {
                        self.populateGrid(response.data);
                    } else {
                        self.showNotification(response.message || "Search failed", "error");
                    }
                },
                error: function() {
                    self.showLoading(false);
                    self.showNotification("Error searching employees. Please try again.", "error");
                }
            });
        },

        /**
         * Populate grid with search results
         */
        populateGrid: function(data) {
            if (this.elements.grid) {
                var employees = data.Employees || data;
                this.elements.grid.dataSource.data(employees);
                this.updateResultsCount({ data: employees, totalCount: data.TotalCount });
            }
        },

        /**
         * Update results count display
         */
        updateResultsCount: function(response) {
            var count = response.totalCount || (response.data && response.data.length) || 0;
            var text = count === 0 ? "No results" : `${count} employee${count !== 1 ? 's' : ''} found`;
            this.elements.resultsCount.text(text);
        },

        /**
         * Handle grid selection change
         */
        onGridSelectionChange: function() {
            var selected = this.elements.grid.select();
            
            if (selected.length > 0) {
                this.selectedEmployee = this.elements.grid.dataItem(selected);
                this.showSelectedEmployee(this.selectedEmployee);
                this.enableSelectButton(true);
            } else {
                this.clearSelection();
            }
        },

        /**
         * Show selected employee information
         */
        showSelectedEmployee: function(employee) {
            this.elements.selectedEmployeeName.text(employee.EmployeeName);
            this.elements.selectedEmployeeId.text(`ID: ${employee.EmployeeId}`);
            this.elements.selectedEmployeeEmail.text(employee.Email);
            this.elements.selectedEmployeeInfo.show();
        },

        /**
         * Clear current selection
         */
        clearSelection: function() {
            this.selectedEmployee = null;
            this.elements.selectedEmployeeInfo.hide();
            this.enableSelectButton(false);
            
            if (this.elements.grid) {
                this.elements.grid.clearSelection();
            }
        },

        /**
         * Enable/disable select button
         */
        enableSelectButton: function(enable) {
            if (this.elements.btnSelect) {
                this.elements.btnSelect.enable(enable);
            }
        },

        /**
         * Clear all filters
         */
        clearFilters: function() {
            if (this.elements.filterEmployeeId) this.elements.filterEmployeeId.value("");
            if (this.elements.filterEmployeeName) this.elements.filterEmployeeName.value("");
            if (this.elements.filterEmployeeEmail) this.elements.filterEmployeeEmail.value("");
            if (this.elements.filterDepartment) this.elements.filterDepartment.value("");
            
            // Clear grid
            if (this.elements.grid) {
                this.elements.grid.dataSource.data([]);
            }
            
            this.updateResultsCount({ totalCount: 0 });
        },

        /**
         * Select current employee and trigger callback
         */
        selectEmployee: function() {
            if (!this.selectedEmployee) {
                this.showNotification("Please select an employee", "warning");
                return;
            }
            
            // Execute callback if provided
            if (this.options.onSelectCallback && typeof window[this.options.onSelectCallback] === 'function') {
                window[this.options.onSelectCallback](this.selectedEmployee);
            }
            
            this.close();
        },

        /**
         * Export search results
         */
        exportResults: function() {
            if (this.elements.grid) {
                var dataSource = this.elements.grid.dataSource;
                if (dataSource.total() === 0) {
                    this.showNotification("No data to export", "warning");
                    return;
                }
                
                // Trigger Kendo Grid export
                this.elements.grid.saveAsExcel();
            }
        },

        /**
         * Show loading state
         */
        showLoading: function(show) {
            // You can implement a loading overlay here
            if (show) {
                this.elements.modal.element.find('.employee-search-container').addClass('loading');
            } else {
                this.elements.modal.element.find('.employee-search-container').removeClass('loading');
            }
        },

        /**
         * Show notification message
         */
        showNotification: function(message, type) {
            // Simple notification - you can enhance this with a toast library
            var className = type === 'error' ? 'alert-danger' : type === 'warning' ? 'alert-warning' : 'alert-info';
            console.log(`[${type.toUpperCase()}] ${message}`);
            
            // You can integrate with your notification system here
        },

        /**
         * Destroy the instance
         */
        destroy: function() {
            if (this.elements.modal) {
                this.elements.modal.destroy();
            }
            delete _instances[this.options.modalId];
        }
    };

    // Public API
    return {
        /**
         * Create a new EmployeeSearch instance
         */
        create: function(options) {
            return new EmployeeSearch(options);
        },

        /**
         * Get existing instance by modal ID
         */
        getInstance: function(modalId) {
            return _instances[modalId];
        },

        /**
         * Global event handlers for use in HTML attributes
         */
        globalHandlers: {
            performAdvancedSearch: function() {
                var instance = _instances['employeeSearchModal'];
                if (instance) instance.performSearch();
            },

            clearAllFilters: function() {
                var instance = _instances['employeeSearchModal'];
                if (instance) instance.clearFilters();
            },

            onEmployeeSelected: function() {
                var instance = _instances['employeeSearchModal'];
                if (instance) instance.selectEmployee();
            },

            closeEmployeeSearchModal: function() {
                var instance = _instances['employeeSearchModal'];
                if (instance) instance.close();
            },

            exportSearchResults: function() {
                var instance = _instances['employeeSearchModal'];
                if (instance) instance.exportResults();
            },

            onEmployeeSearchModalClose: function() {
                var instance = _instances['employeeSearchModal'];
                if (instance) instance.clearSelection();
            },

            onEmployeeGridSelectionChange: function() {
                var instance = _instances['employeeSearchModal'];
                if (instance) instance.onGridSelectionChange();
            }
        }
    };
})();

// Expose global handlers for HTML event binding
window.performAdvancedSearch = EmployeeSearchComponent.globalHandlers.performAdvancedSearch;
window.clearAllFilters = EmployeeSearchComponent.globalHandlers.clearAllFilters;
window.onEmployeeSelected = EmployeeSearchComponent.globalHandlers.onEmployeeSelected;
window.closeEmployeeSearchModal = EmployeeSearchComponent.globalHandlers.closeEmployeeSearchModal;
window.exportSearchResults = EmployeeSearchComponent.globalHandlers.exportSearchResults;
window.onEmployeeSearchModalClose = EmployeeSearchComponent.globalHandlers.onEmployeeSearchModalClose;
window.onEmployeeGridSelectionChange = EmployeeSearchComponent.globalHandlers.onEmployeeGridSelectionChange;