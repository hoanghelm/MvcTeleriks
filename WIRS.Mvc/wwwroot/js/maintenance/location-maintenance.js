// Location Maintenance View Model
var locationMaintenanceViewModel = {
    // Current state
    currentSectorCode: null,
    currentLOBCode: null,
    currentDepartmentCode: null,
    isEditMode: false,
    currentLocationData: null,

    // Initialize the view model
    init: function() {
        this.bindEvents();
        this.initializeEmptyGrid();
    },

    // Bind event handlers
    bindEvents: function() {
        // Form validation
        $('#locationForm').on('submit', function(e) {
            e.preventDefault();
            return false;
        });
    },

    // Initialize empty grid
    initializeEmptyGrid: function() {
        var grid = $('#locationGrid').data('kendoGrid');
        if (grid) {
            grid.dataSource.data([]);
        }
        $('#pnlLocationUpdate').hide();
    },

    // Hide all panels
    hideAllPanels: function() {
        $('#pnlLocationUpdate').hide();
    },

    // Show loading overlay
    showLoading: function() {
        $('#loadingOverlay').show();
    },

    // Hide loading overlay
    hideLoading: function() {
        $('#loadingOverlay').hide();
    },

    // Show notification
    showNotification: function(message, type) {
        var notification = $("#notification").kendoNotification({
            position: {
                pinned: true,
                top: 30,
                right: 30
            },
            autoHideAfter: 3000,
            stacking: "down"
        }).data("kendoNotification");

        if (type === 'success') {
            notification.success(message);
        } else if (type === 'error') {
            notification.error(message);
        } else {
            notification.info(message);
        }
    },

    // Clear validation errors
    clearValidationErrors: function() {
        $('.validation-error').removeClass('show').text('');
    },

    // Show validation error
    showValidationError: function(fieldId, message) {
        $('#' + fieldId + 'Error').text(message).addClass('show');
    },

    // Validate location form
    validateLocationForm: function() {
        this.clearValidationErrors();
        var isValid = true;

        var locationName = $('#txtLocationName').data('kendoTextBox').value();
        var locationCode = $('#txtLocationCode').data('kendoTextBox').value();

        if (!locationCode || locationCode.trim() === '') {
            this.showValidationError('locationCode', 'Location Code is required');
            isValid = false;
        }

        if (!locationName || locationName.trim() === '') {
            this.showValidationError('locationName', 'Location Name is required');
            isValid = false;
        } else if (locationName.length > 100) {
            this.showValidationError('locationName', 'Location Name cannot exceed 100 characters');
            isValid = false;
        }

        return isValid;
    },

    // Load LOB list for selected sector
    loadLOBList: function(sectorCode) {
        var self = this;
        var lobDropDown = $('#ddlLOB').data('kendoDropDownList');

        $.ajax({
            url: '/MasterData/GetLOBs',
            type: 'POST',
            data: { sectorCode: sectorCode },
            success: function(response) {
                if (response && Array.isArray(response)) {
                    lobDropDown.setDataSource(response);
                    lobDropDown.value('');
                } else {
                    lobDropDown.setDataSource([]);
                }
                
                // Clear department dropdown
                var deptDropDown = $('#ddlDepartment').data('kendoDropDownList');
                deptDropDown.setDataSource([]);
                deptDropDown.value('');
            },
            error: function(xhr, status, error) {
                self.showNotification('Error loading LOB list: ' + error, 'error');
                lobDropDown.setDataSource([]);
            }
        });
    },

    // Load department list for selected LOB
    loadDepartmentList: function(sectorCode, lobCode) {
        var self = this;
        var deptDropDown = $('#ddlDepartment').data('kendoDropDownList');

        $.ajax({
            url: '/MasterData/GetDepartments',
            type: 'POST',
            data: { 
                sectorCode: sectorCode,
                lobCode: lobCode 
            },
            success: function(response) {
                if (response && Array.isArray(response)) {
                    deptDropDown.setDataSource(response);
                    deptDropDown.value('');
                } else {
                    deptDropDown.setDataSource([]);
                }
            },
            error: function(xhr, status, error) {
                self.showNotification('Error loading department list: ' + error, 'error');
                deptDropDown.setDataSource([]);
            }
        });
    },

    // Load location list for selected filters
    loadLocationList: function(sectorCode, lobCode, departmentCode) {
        var self = this;
        self.showLoading();

        $.ajax({
            url: '/Maintenance/GetLocationList',
            type: 'POST',
            data: { 
                sbaCode: sectorCode,
                sbuCode: lobCode,
                departmentCode: departmentCode
            },
            success: function(response) {
                self.hideLoading();
                if (response && Array.isArray(response)) {
                    var grid = $('#locationGrid').data('kendoGrid');
                    grid.dataSource.data(response);
                    
                    $('#pnlLocationUpdate').hide();
                } else {
                    self.showNotification('Failed to load location list', 'error');
                }
            },
            error: function(xhr, status, error) {
                self.hideLoading();
                self.showNotification('Error loading location list: ' + error, 'error');
            }
        });
    },

    // Load location record for editing
    loadLocationRecord: function(sbaCode, sbuCode, departmentCode, locationCode) {
        var self = this;
        self.showLoading();

        $.ajax({
            url: '/Maintenance/GetLocationByUid',
            type: 'POST',
            data: { 
                sbaCode: sbaCode,
                sbuCode: sbuCode,
                departmentCode: departmentCode,
                locationCode: locationCode
            },
            success: function(response) {
                self.hideLoading();
                if (response) {
                    self.populateLocationForm(response);
                    self.currentLocationData = response;
                    self.isEditMode = true;
                    $('#pnlLocationUpdate').show();
                } else {
                    self.showNotification('Location record not found', 'error');
                }
            },
            error: function(xhr, status, error) {
                self.hideLoading();
                self.showNotification('Error loading location record: ' + error, 'error');
            }
        });
    },

    // Populate location form with data
    populateLocationForm: function(data) {
        $('#txtLocationCode').data('kendoTextBox').value(data.LocationCode || '');
        $('#txtLocationName').data('kendoTextBox').value(data.LocationName || '');
        
        if (data.InactiveDate) {
            var datePicker = $('#txtInactiveDate').data('kendoDatePicker');
            if (datePicker) {
                datePicker.value(new Date(data.InactiveDate));
            }
        }
    },

    // Clear location form
    clearLocationForm: function() {
        $('#txtLocationCode').data('kendoTextBox').value('');
        $('#txtLocationName').data('kendoTextBox').value('');
        
        var datePicker = $('#txtInactiveDate').data('kendoDatePicker');
        if (datePicker) {
            datePicker.value(null);
        }
        
        this.clearValidationErrors();
    },

    // Generate new location code
    generateLocationCode: function() {
        var self = this;
        
        if (!self.currentLOBCode) {
            self.showNotification('Please select filters first', 'error');
            return;
        }

        $.ajax({
            url: '/Maintenance/GenerateLocationCode',
            type: 'POST',
            data: { sbuCode: self.currentLOBCode },
            success: function(response) {
                if (response.success) {
                    $('#txtLocationCode').data('kendoTextBox').value(response.code);
                } else {
                    self.showNotification('Failed to generate location code: ' + response.message, 'error');
                }
            },
            error: function(xhr, status, error) {
                self.showNotification('Error generating location code: ' + error, 'error');
            }
        });
    },

    // Save location record
    saveLocationRecord: function() {
        var self = this;

        if (!self.validateLocationForm()) {
            return;
        }

        var formData = {
            SbaCode: self.currentSectorCode,
            SbuCode: self.currentLOBCode,
            DepartmentCode: self.currentDepartmentCode,
            LocationCode: $('#txtLocationCode').data('kendoTextBox').value(),
            LocationName: $('#txtLocationName').data('kendoTextBox').value(),
            InactiveDate: ''
        };

        var datePicker = $('#txtInactiveDate').data('kendoDatePicker');
        if (datePicker && datePicker.value()) {
            formData.InactiveDate = kendo.toString(datePicker.value(), 'dd/MM/yyyy');
        }

        self.showLoading();

        $.ajax({
            url: '/Maintenance/SaveLocation',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                self.hideLoading();
                if (response.success) {
                    self.showNotification(response.message, 'success');
                    self.loadLocationList(self.currentSectorCode, self.currentLOBCode, self.currentDepartmentCode);
                    self.currentLocationData = null;
                    self.isEditMode = false;
                } else {
                    self.showNotification(response.message, 'error');
                }
            },
            error: function(xhr, status, error) {
                self.hideLoading();
                self.showNotification('Error saving location record: ' + error, 'error');
            }
        });
    }
};

// Global event handlers
function onSectorChange() {
    var sectorDropDown = $('#ddlSector').data('kendoDropDownList');
    var selectedValue = sectorDropDown.value();
    
    if (selectedValue && selectedValue !== '') {
        locationMaintenanceViewModel.currentSectorCode = selectedValue;
        locationMaintenanceViewModel.loadLOBList(selectedValue);
    } else {
        locationMaintenanceViewModel.currentSectorCode = null;
        $('#ddlLOB').data('kendoDropDownList').setDataSource([]);
        $('#ddlDepartment').data('kendoDropDownList').setDataSource([]);
        var grid = $('#locationGrid').data('kendoGrid');
        if (grid) {
            grid.dataSource.data([]);
        }
    }
}

function onLOBChange() {
    var lobDropDown = $('#ddlLOB').data('kendoDropDownList');
    var selectedValue = lobDropDown.value();
    
    if (selectedValue && selectedValue !== '' && locationMaintenanceViewModel.currentSectorCode) {
        locationMaintenanceViewModel.currentLOBCode = selectedValue;
        locationMaintenanceViewModel.loadDepartmentList(locationMaintenanceViewModel.currentSectorCode, selectedValue);
    } else {
        locationMaintenanceViewModel.currentLOBCode = null;
        $('#ddlDepartment').data('kendoDropDownList').setDataSource([]);
        var grid = $('#locationGrid').data('kendoGrid');
        if (grid) {
            grid.dataSource.data([]);
        }
    }
}

function onDepartmentChange() {
    var deptDropDown = $('#ddlDepartment').data('kendoDropDownList');
    var selectedValue = deptDropDown.value();
    
    locationMaintenanceViewModel.currentDepartmentCode = selectedValue;
    var grid = $('#locationGrid').data('kendoGrid');
    if (grid) {
        grid.dataSource.data([]);
    }
}

function searchLocations() {
    if (!locationMaintenanceViewModel.currentSectorCode || 
        !locationMaintenanceViewModel.currentLOBCode || 
        !locationMaintenanceViewModel.currentDepartmentCode) {
        locationMaintenanceViewModel.showNotification('Please select all filters first', 'error');
        return;
    }

    locationMaintenanceViewModel.loadLocationList(
        locationMaintenanceViewModel.currentSectorCode,
        locationMaintenanceViewModel.currentLOBCode,
        locationMaintenanceViewModel.currentDepartmentCode
    );
}

function newLocationRecord() {
    if (!locationMaintenanceViewModel.currentSectorCode || 
        !locationMaintenanceViewModel.currentLOBCode || 
        !locationMaintenanceViewModel.currentDepartmentCode) {
        locationMaintenanceViewModel.showNotification('Please select all filters first', 'error');
        return;
    }

    locationMaintenanceViewModel.clearLocationForm();
    locationMaintenanceViewModel.isEditMode = false;
    locationMaintenanceViewModel.currentLocationData = null;
    locationMaintenanceViewModel.generateLocationCode();
    
    $('#pnlLocationUpdate').show();
}

function editLocationRecord(sbaCode, sbuCode, departmentCode, locationCode) {
    locationMaintenanceViewModel.loadLocationRecord(sbaCode, sbuCode, departmentCode, locationCode);
}

function cancelLocationEdit() {
    locationMaintenanceViewModel.clearLocationForm();
    locationMaintenanceViewModel.currentLocationData = null;
    locationMaintenanceViewModel.isEditMode = false;
    
    $('#pnlLocationUpdate').hide();
}

function saveLocationRecord() {
    locationMaintenanceViewModel.saveLocationRecord();
}