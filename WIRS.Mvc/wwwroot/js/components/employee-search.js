var EmployeeSearchComponent = (function () {
    var instances = {};

    function EmployeeSearchInstance(config) {
        this.modalId = config.modalId;
        this.gridId = config.gridId;
        this.onSelectCallback = config.onSelectCallback;
        this.modal = null;
        this.grid = null;
        this.selectedEmployee = null;

        this.init();
    }

    EmployeeSearchInstance.prototype.init = function () {
        var self = this;

        this.modal = $("#" + this.modalId).data("kendoWindow");
        this.grid = $("#" + this.gridId).data("kendoGrid");

        if (this.modal) {
            this.modal.bind("open", function () {
                self.loadAllEmployees();
            });
        }
    };

    EmployeeSearchInstance.prototype.loadAllEmployees = function () {
        var self = this;

        if (!this.grid) return;

        kendo.ui.progress(this.grid.element, true);

        $.ajax({
            url: '/User/SearchEmployees',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                employeeId: '',
                employeeName: '',
                pageNo: 1,
                pageSize: 100
            }),
            success: function (response) {
                kendo.ui.progress(self.grid.element, false);

                if (response.success && response.data && response.data.employees) {
                    var employees = response.data.employees.map(function (emp) {
                        return {
                            EmployeeId: emp.employeeId,
                            EmployeeName: emp.employeeName,
                            CostCentreName: emp.costCentreName,
                            Designation: emp.designation,
                            Email: emp.email,
                            ContactNo: emp.contactNo
                        };
                    });

                    self.grid.setDataSource(new kendo.data.DataSource({
                        data: employees,
                        pageSize: 10,
                        schema: {
                            model: {
                                id: "EmployeeId",
                                fields: {
                                    EmployeeId: { type: "string" },
                                    EmployeeName: { type: "string" },
                                    CostCentreName: { type: "string" },
                                    Designation: { type: "string" },
                                    Email: { type: "string" },
                                    ContactNo: { type: "string" }
                                }
                            }
                        }
                    }));
                } else {
                    self.grid.setDataSource(new kendo.data.DataSource({ data: [] }));
                    TelerikNotification.error(response.message || 'Failed to load employees');
                }
            },
            error: function (xhr, status, error) {
                kendo.ui.progress(self.grid.element, false);
                console.error('Error loading employees:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
                TelerikNotification.error('Error loading employees. Please check console for details.');
            }
        });
    };

    EmployeeSearchInstance.prototype.open = function () {
        if (this.modal) {
            var searchBox = $("#filterEmployeeName").data("kendoTextBox");
            if (searchBox) searchBox.value('');

            this.selectedEmployee = null;

            this.modal.center().open();
        }
    };

    EmployeeSearchInstance.prototype.close = function () {
        if (this.modal) {
            this.modal.close();
        }
    };

    EmployeeSearchInstance.prototype.getSelectedEmployee = function () {
        return this.selectedEmployee;
    };

    EmployeeSearchInstance.prototype.selectEmployee = function (employeeId) {
        if (!this.grid) return;

        var dataSource = this.grid.dataSource;
        var employee = null;

        for (var i = 0; i < dataSource.data().length; i++) {
            var item = dataSource.data()[i];
            if (item.EmployeeId === employeeId) {
                employee = item;
                break;
            }
        }

        if (employee) {
            this.selectedEmployee = employee;

            if (window[this.onSelectCallback]) {
                window[this.onSelectCallback](employee);
            }

            this.close();
        }
    };

    return {
        create: function (config) {
            var instance = new EmployeeSearchInstance(config);
            instances[config.modalId] = instance;
            return instance;
        },

        getInstance: function (modalId) {
            return instances[modalId];
        }
    };
})();

function onEmployeeSearchModalOpen() {
}

function onEmployeeSearchModalClose() {
    var modalId = this.element.attr('id');
    var instance = EmployeeSearchComponent.getInstance(modalId);

    if (instance) {
        instance.selectedEmployee = null;
    }
}

function performAdvancedSearch() {
    var employeeNameBox = $("#filterEmployeeName").data("kendoTextBox");
    var employeeName = employeeNameBox ? employeeNameBox.value() : '';
    var grid = $("#employeeSearchGrid").data("kendoGrid");

    if (!grid) return;

    kendo.ui.progress(grid.element, true);

    $.ajax({
        url: '/User/SearchEmployees',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            employeeId: '',
            employeeName: employeeName,
            pageNo: 1,
            pageSize: 100
        }),
        success: function (response) {
            kendo.ui.progress(grid.element, false);

            if (response.success && response.data && response.data.employees) {
                var employees = response.data.employees.map(function (emp) {
                    return {
                        EmployeeId: emp.employeeId,
                        EmployeeName: emp.employeeName,
                        CostCentreName: emp.costCentreName,
                        Designation: emp.designation,
                        Email: emp.email,
                        ContactNo: emp.contactNo
                    };
                });

                grid.setDataSource(new kendo.data.DataSource({
                    data: employees,
                    pageSize: 10,
                    schema: {
                        model: {
                            id: "EmployeeId",
                            fields: {
                                EmployeeId: { type: "string" },
                                EmployeeName: { type: "string" },
                                CostCentreName: { type: "string" },
                                Designation: { type: "string" },
                                Email: { type: "string" },
                                ContactNo: { type: "string" }
                            }
                        }
                    }
                }));

                if (employees.length === 0) {
                    TelerikNotification.info('No employees found matching your search');
                }
            } else {
                grid.setDataSource(new kendo.data.DataSource({ data: [] }));
                TelerikNotification.error(response.message || 'Search failed');
            }
        },
        error: function (xhr, status, error) {
            kendo.ui.progress(grid.element, false);
            console.error('Search error:', error);
            console.error('Status:', status);
            console.error('Response:', xhr.responseText);
            TelerikNotification.error('Error searching employees. Please check console for details.');
        }
    });
}

function selectEmployeeById(employeeId) {
    var instance = EmployeeSearchComponent.getInstance('employeeSearchWindow');
    if (instance) {
        instance.selectEmployee(employeeId);
    }
}