// LOB Maintenance View Model
var lobMaintenanceViewModel = {
    // Current state
    currentSectorCode: null,
    isEditMode: false,
    currentLOBData: null,

    // Initialize the view model
    init: function() {
        this.bindEvents();
        this.initializeEmptyGrid();
    },

    // Bind event handlers
    bindEvents: function() {
        // Form validation
        $('#lobForm').on('submit', function(e) {
            e.preventDefault();
            return false;
        });
    },

    // Initialize empty grid
    initializeEmptyGrid: function() {
        var grid = $('#lobGrid').data('kendoGrid');
        if (grid) {
            grid.dataSource.data([]);
        }
        $('#pnlLOBUpdate').hide();
    },

    // Hide all panels
    hideAllPanels: function() {
        $('#pnlLOBUpdate').hide();
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

    // Validate LOB form
    validateLOBForm: function() {
        this.clearValidationErrors();
        var isValid = true;

        var lobName = $('#txtLOBName').data('kendoTextBox').value();
        var lobCode = $('#txtLOBCode').data('kendoTextBox').value();

        if (!lobCode || lobCode.trim() === '') {
            this.showValidationError('lobCode', 'LOB Code is required');
            isValid = false;
        }

        if (!lobName || lobName.trim() === '') {
            this.showValidationError('lobName', 'LOB Name is required');
            isValid = false;
        } else if (lobName.length > 100) {
            this.showValidationError('lobName', 'LOB Name cannot exceed 100 characters');
            isValid = false;
        }

        return isValid;
    },

    // Load LOB list for selected sector
    loadLOBList: function(sectorCode) {
        var self = this;
        self.showLoading();

        $.ajax({
            url: '/Maintenance/GetLOBList',
            type: 'POST',
            data: { sbaCode: sectorCode },
            success: function(response) {
                self.hideLoading();
                if (response && Array.isArray(response)) {
                    var grid = $('#lobGrid').data('kendoGrid');
                    grid.dataSource.data(response);
                    
                    $('#pnlLOBUpdate').hide();
                } else {
                    self.showNotification('Failed to load LOB list', 'error');
                }
            },
            error: function(xhr, status, error) {
                self.hideLoading();
                self.showNotification('Error loading LOB list: ' + error, 'error');
            }
        });
    },

    // Load LOB record for editing
    loadLOBRecord: function(sbaCode, sbuCode) {
        var self = this;
        self.showLoading();

        $.ajax({
            url: '/Maintenance/GetLOBByUid',
            type: 'POST',
            data: { 
                sbaCode: sbaCode,
                sbuCode: sbuCode 
            },
            success: function(response) {
                self.hideLoading();
                if (response) {
                    self.populateLOBForm(response);
                    self.currentLOBData = response;
                    self.isEditMode = true;
                    $('#pnlLOBUpdate').show();
                } else {
                    self.showNotification('LOB record not found', 'error');
                }
            },
            error: function(xhr, status, error) {
                self.hideLoading();
                self.showNotification('Error loading LOB record: ' + error, 'error');
            }
        });
    },

    // Populate LOB form with data
    populateLOBForm: function(data) {
        $('#txtLOBCode').data('kendoTextBox').value(data.SbuCode || '');
        $('#txtLOBName').data('kendoTextBox').value(data.SbuName || '');
        
        if (data.InactiveDate) {
            var datePicker = $('#txtInactiveDate').data('kendoDatePicker');
            if (datePicker) {
                datePicker.value(new Date(data.InactiveDate));
            }
        }
    },

    // Clear LOB form
    clearLOBForm: function() {
        $('#txtLOBCode').data('kendoTextBox').value('');
        $('#txtLOBName').data('kendoTextBox').value('');
        
        var datePicker = $('#txtInactiveDate').data('kendoDatePicker');
        if (datePicker) {
            datePicker.value(null);
        }
        
        this.clearValidationErrors();
    },

    // Generate new LOB code
    generateLOBCode: function() {
        var self = this;
        
        if (!self.currentSectorCode) {
            self.showNotification('Please select a sector first', 'error');
            return;
        }

        $.ajax({
            url: '/Maintenance/GenerateLOBCode',
            type: 'POST',
            data: { sbaCode: self.currentSectorCode },
            success: function(response) {
                if (response.success) {
                    $('#txtLOBCode').data('kendoTextBox').value(response.code);
                } else {
                    self.showNotification('Failed to generate LOB code: ' + response.message, 'error');
                }
            },
            error: function(xhr, status, error) {
                self.showNotification('Error generating LOB code: ' + error, 'error');
            }
        });
    },

    // Save LOB record
    saveLOBRecord: function() {
        var self = this;

        if (!self.validateLOBForm()) {
            return;
        }

        var formData = {
            SbaCode: self.currentSectorCode,
            SbuCode: $('#txtLOBCode').data('kendoTextBox').value(),
            SbuName: $('#txtLOBName').data('kendoTextBox').value(),
            InactiveDate: ''
        };

        var datePicker = $('#txtInactiveDate').data('kendoDatePicker');
        if (datePicker && datePicker.value()) {
            formData.InactiveDate = kendo.toString(datePicker.value(), 'dd/MM/yyyy');
        }

        self.showLoading();

        $.ajax({
            url: '/Maintenance/SaveLOB',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                self.hideLoading();
                if (response.success) {
                    self.showNotification(response.message, 'success');
                    self.loadLOBList(self.currentSectorCode);
                    self.currentLOBData = null;
                    self.isEditMode = false;
                } else {
                    self.showNotification(response.message, 'error');
                }
            },
            error: function(xhr, status, error) {
                self.hideLoading();
                self.showNotification('Error saving LOB record: ' + error, 'error');
            }
        });
    }
};

// Global event handlers
function onSectorChange() {
    var sectorDropDown = $('#ddlSector').data('kendoDropDownList');
    var selectedValue = sectorDropDown.value();
    
    if (selectedValue && selectedValue !== '') {
        lobMaintenanceViewModel.currentSectorCode = selectedValue;
        lobMaintenanceViewModel.loadLOBList(selectedValue);
    } else {
        lobMaintenanceViewModel.currentSectorCode = null;
        var grid = $('#lobGrid').data('kendoGrid');
        if (grid) {
            grid.dataSource.data([]);
        }
    }
}

function newLOBRecord() {
    if (!lobMaintenanceViewModel.currentSectorCode) {
        lobMaintenanceViewModel.showNotification('Please select a sector first', 'error');
        return;
    }

    lobMaintenanceViewModel.clearLOBForm();
    lobMaintenanceViewModel.isEditMode = false;
    lobMaintenanceViewModel.currentLOBData = null;
    lobMaintenanceViewModel.generateLOBCode();
    
    $('#pnlLOBUpdate').show();
}

function editLOBRecord(sbaCode, sbuCode) {
    lobMaintenanceViewModel.loadLOBRecord(sbaCode, sbuCode);
}

function cancelLOBEdit() {
    lobMaintenanceViewModel.clearLOBForm();
    lobMaintenanceViewModel.currentLOBData = null;
    lobMaintenanceViewModel.isEditMode = false;
    
    $('#pnlLOBUpdate').hide();
}

function saveLOBRecord() {
    lobMaintenanceViewModel.saveLOBRecord();
}