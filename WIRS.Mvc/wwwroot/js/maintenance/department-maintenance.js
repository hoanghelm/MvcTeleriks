var departmentMaintenanceViewModel = {
    isLoading: false,
    
    init: function() {
        this.initializeEmptyGrid();
        this.bindEvents();
    },

    initializeEmptyGrid: function() {
        // Initialize empty department grid
        var grid = $("#departmentGrid").data("kendoGrid");
        if (grid) {
            grid.setDataSource(new kendo.data.DataSource({
                data: [],
                pageSize: 15,
                schema: {
                    model: {
                        id: "Uid",
                        fields: {
                            SbaCode: { type: "string" },
                            SbaName: { type: "string" },
                            SbuCode: { type: "string" },
                            SbuName: { type: "string" },
                            DepartmentCode: { type: "string" },
                            DepartmentName: { type: "string" },
                            InactiveDate: { type: "string" },
                            Uid: { type: "string" },
                            CodeType: { type: "string" }
                        }
                    }
                }
            }));
        }
    },

    bindEvents: function() {
        // Bind dropdown change events
        $("#ddlSector").change(function() {
            departmentMaintenanceViewModel.onSectorChange();
        });

        $("#ddlLOB").change(function() {
            departmentMaintenanceViewModel.onLOBChange();
        });
    },

    showLoading: function() {
        this.isLoading = true;
        $("#loadingOverlay").show();
    },

    hideLoading: function() {
        this.isLoading = false;
        $("#loadingOverlay").hide();
    },

    showNotification: function(message, type) {
        type = type || 'info';
        var notification = $("#notification").kendoNotification({
            position: {
                pinned: true,
                top: 100,
                right: 30
            },
            autoHideAfter: 5000,
            stacking: "down"
        }).data("kendoNotification");

        notification.show(message, type);
    },

    clearValidationErrors: function() {
        $(".validation-error").text("");
    },

    validateForm: function() {
        this.clearValidationErrors();
        var isValid = true;

        var departmentName = $("#txtDepartmentName").val().trim();
        if (!departmentName) {
            $("#departmentNameError").text("Department Name is required");
            isValid = false;
        }

        return isValid;
    }
};

function onSectorChange() {
    departmentMaintenanceViewModel.onSectorChange();
}

departmentMaintenanceViewModel.onSectorChange = function() {
    var sectorCode = $("#ddlSector").val();
    
    // Clear LOB dropdown
    var lobDropDown = $("#ddlLOB").data("kendoDropDownList");
    if (lobDropDown) {
        lobDropDown.setDataSource(new kendo.data.DataSource({
            data: []
        }));
        lobDropDown.value("");
    }
    
    // Clear grid
    this.initializeEmptyGrid();
    
    if (sectorCode && sectorCode !== "") {
        // Load LOB data for selected sector
        lobDropDown.setDataSource(new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/MasterData/GetLOBs",
                    type: "POST",
                    data: function() {
                        return { sectorCode: sectorCode };
                    }
                }
            },
            schema: {
                data: "data"
            }
        }));
    }
};

function onLOBChange() {
    departmentMaintenanceViewModel.onLOBChange();
}

departmentMaintenanceViewModel.onLOBChange = function() {
    // Clear grid when LOB changes
    this.initializeEmptyGrid();
};

function searchDepartments() {
    var sectorCode = $("#ddlSector").val();
    var lobCode = $("#ddlLOB").val();
    var departmentName = $("#txtDepartmentSearch").val().trim();

    if (!sectorCode || sectorCode === "") {
        departmentMaintenanceViewModel.showNotification("Please select a Sector", "error");
        return;
    }

    if (!lobCode || lobCode === "") {
        departmentMaintenanceViewModel.showNotification("Please select a LOB", "error");
        return;
    }

    departmentMaintenanceViewModel.showLoading();

    $.ajax({
        url: "/Maintenance/GetDepartmentList",
        type: "POST",
        data: {
            sbaCode: sectorCode,
            sbuCode: lobCode,
            departmentName: departmentName
        },
        success: function(response) {
            departmentMaintenanceViewModel.hideLoading();
            
            if (response && Array.isArray(response)) {
                var grid = $("#departmentGrid").data("kendoGrid");
                if (grid) {
                    grid.setDataSource(new kendo.data.DataSource({
                        data: response,
                        pageSize: 15,
                        schema: {
                            model: {
                                id: "Uid",
                                fields: {
                                    SbaCode: { type: "string" },
                                    SbaName: { type: "string" },
                                    SbuCode: { type: "string" },
                                    SbuName: { type: "string" },
                                    DepartmentCode: { type: "string" },
                                    DepartmentName: { type: "string" },
                                    InactiveDate: { type: "string" },
                                    Uid: { type: "string" },
                                    CodeType: { type: "string" }
                                }
                            }
                        }
                    }));
                }
                
                if (response.length === 0) {
                    departmentMaintenanceViewModel.showNotification("No department records found for the selected criteria", "info");
                }
            } else {
                departmentMaintenanceViewModel.showNotification("No data received from server", "error");
            }
        },
        error: function(xhr, status, error) {
            departmentMaintenanceViewModel.hideLoading();
            departmentMaintenanceViewModel.showNotification("Error loading department records: " + error, "error");
        }
    });
}

function newDepartmentRecord() {
    var sectorCode = $("#ddlSector").val();
    var lobCode = $("#ddlLOB").val();

    if (!sectorCode || sectorCode === "") {
        departmentMaintenanceViewModel.showNotification("Please select a Sector first", "error");
        return;
    }

    if (!lobCode || lobCode === "") {
        departmentMaintenanceViewModel.showNotification("Please select a LOB first", "error");
        return;
    }

    departmentMaintenanceViewModel.showLoading();

    // Generate new department code
    $.ajax({
        url: "/Maintenance/GenerateDepartmentCode",
        type: "POST",
        data: { sbuCode: lobCode },
        success: function(response) {
            departmentMaintenanceViewModel.hideLoading();
            
            if (response && response.success) {
                // Clear form
                $("#txtDepartmentCode").val(response.code);
                $("#txtDepartmentName").val("");
                $("#txtInactiveDate").val("");
                
                departmentMaintenanceViewModel.clearValidationErrors();
                
                // Show edit panel
                $("#pnlDepartmentUpdate").show();
                $("#txtDepartmentName").focus();
            } else {
                departmentMaintenanceViewModel.showNotification("Error generating department code: " + (response.message || "Unknown error"), "error");
            }
        },
        error: function(xhr, status, error) {
            departmentMaintenanceViewModel.hideLoading();
            departmentMaintenanceViewModel.showNotification("Error generating department code: " + error, "error");
        }
    });
}

function editDepartmentRecord(codeType, sbaCode, sbuCode, departmentCode) {
    departmentMaintenanceViewModel.showLoading();

    $.ajax({
        url: "/Maintenance/GetDepartmentByUid",
        type: "POST",
        data: {
            codeType: codeType,
            sbaCode: sbaCode,
            sbuCode: sbuCode,
            departmentCode: departmentCode
        },
        success: function(response) {
            departmentMaintenanceViewModel.hideLoading();
            
            if (response) {
                // Populate form
                $("#txtDepartmentCode").val(response.DepartmentCode);
                $("#txtDepartmentName").val(response.DepartmentName);
                $("#txtInactiveDate").val(response.InactiveDate);
                
                departmentMaintenanceViewModel.clearValidationErrors();
                
                // Show edit panel
                $("#pnlDepartmentUpdate").show();
                $("#txtDepartmentName").focus();
            } else {
                departmentMaintenanceViewModel.showNotification("Department record not found", "error");
            }
        },
        error: function(xhr, status, error) {
            departmentMaintenanceViewModel.hideLoading();
            departmentMaintenanceViewModel.showNotification("Error loading department record: " + error, "error");
        }
    });
}

function saveDepartmentRecord() {
    if (!departmentMaintenanceViewModel.validateForm()) {
        return;
    }

    var sectorCode = $("#ddlSector").val();
    var lobCode = $("#ddlLOB").val();
    
    var departmentData = {
        SbaCode: sectorCode,
        SbuCode: lobCode,
        DepartmentCode: $("#txtDepartmentCode").val(),
        DepartmentName: $("#txtDepartmentName").val().trim(),
        InactiveDate: $("#txtInactiveDate").val(),
        CodeType: "DEPT" // Default code type for departments
    };

    departmentMaintenanceViewModel.showLoading();

    $.ajax({
        url: "/Maintenance/SaveDepartment",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(departmentData),
        success: function(response) {
            departmentMaintenanceViewModel.hideLoading();
            
            if (response && response.success) {
                departmentMaintenanceViewModel.showNotification(response.message || "Department record saved successfully", "success");
                
                // Hide edit panel
                $("#pnlDepartmentUpdate").hide();
                
                // Refresh grid
                searchDepartments();
            } else {
                departmentMaintenanceViewModel.showNotification("Save failed: " + (response.message || "Unknown error"), "error");
            }
        },
        error: function(xhr, status, error) {
            departmentMaintenanceViewModel.hideLoading();
            departmentMaintenanceViewModel.showNotification("Error saving department record: " + error, "error");
        }
    });
}

function cancelDepartmentEdit() {
    $("#pnlDepartmentUpdate").hide();
    departmentMaintenanceViewModel.clearValidationErrors();
}