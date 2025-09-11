var maintenanceCopyToViewModel = (function () {
    
    var _currentMode = 'search'; // 'search' or 'form'
    var _isEditMode = false;
    
    var _elements = {
        // Search elements
        ddlSearchSector: null,
        ddlSearchLOB: null,
        ddlSearchDepartment: null,
        ddlSearchLocation: null,
        copyToGrid: null,
        
        // Form elements
        ddlSector: null,
        ddlLOB: null,
        ddlDepartment: null,
        ddlLocation: null,
        txtUserId: null,
        txtUserName: null,
        dtpInactiveDate: null,
        
        // Sections
        searchSection: null,
        entryFormSection: null,
        formTitle: null
    };
    
    function init() {
        initializeElements();
        loadMasterData();
        showSearchSection();
    }
    
    function initializeElements() {
        // Search elements
        _elements.ddlSearchSector = $("#ddlSearchSector").data("kendoDropDownList");
        _elements.ddlSearchLOB = $("#ddlSearchLOB").data("kendoDropDownList");
        _elements.ddlSearchDepartment = $("#ddlSearchDepartment").data("kendoDropDownList");
        _elements.ddlSearchLocation = $("#ddlSearchLocation").data("kendoDropDownList");
        _elements.copyToGrid = $("#copyToGrid").data("kendoGrid");
        
        // Form elements
        _elements.ddlSector = $("#ddlSector").data("kendoDropDownList");
        _elements.ddlLOB = $("#ddlLOB").data("kendoDropDownList");
        _elements.ddlDepartment = $("#ddlDepartment").data("kendoDropDownList");
        _elements.ddlLocation = $("#ddlLocation").data("kendoDropDownList");
        _elements.txtUserId = $("#txtUserId").data("kendoTextBox");
        _elements.txtUserName = $("#txtUserName").data("kendoTextBox");
        _elements.dtpInactiveDate = $("#dtpInactiveDate").data("kendoDatePicker");
        
        // Sections
        _elements.searchSection = $("#searchSection");
        _elements.entryFormSection = $("#entryFormSection");
        _elements.formTitle = $("#formTitle");
    }
    
    function loadMasterData() {
        // Search dropdowns will load automatically from their DataSource
        // Form dropdowns need to be populated
        loadSectors();
    }
    
    function loadSectors() {
        // Load sectors for the form dropdown (search dropdown loads automatically)
        TelerikSkeleton.showDropdownSkeleton("#ddlSector");
        
        $.get('/MasterData/GetSectors')
            .done(function(response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlSector");
                if (response.success && _elements.ddlSector) {
                    _elements.ddlSector.setDataSource(response.data);
                }
            })
            .fail(function() {
                TelerikSkeleton.hideDropdownSkeleton("#ddlSector");
                TelerikNotification.error('Error loading sectors');
            });
    }
    
    // Search section functions
    function onSearchSectorChange() {
        if (!_elements.ddlSearchSector || !_elements.ddlSearchLOB) return;
        
        var sectorCode = _elements.ddlSearchSector.value();
        
        // Reset dependent dropdowns
        _elements.ddlSearchLOB.setDataSource([]);
        _elements.ddlSearchLOB.value('');
        _elements.ddlSearchDepartment.setDataSource([]);
        _elements.ddlSearchDepartment.value('');
        _elements.ddlSearchLocation.setDataSource([]);
        _elements.ddlSearchLocation.value('');
        
        if (sectorCode) {
            loadLOBsBySearchSector(sectorCode);
        }
    }
    
    function onSearchLOBChange() {
        if (!_elements.ddlSearchSector || !_elements.ddlSearchLOB || !_elements.ddlSearchDepartment) return;
        
        var sectorCode = _elements.ddlSearchSector.value();
        var lobCode = _elements.ddlSearchLOB.value();
        
        // Reset dependent dropdowns
        _elements.ddlSearchDepartment.setDataSource([]);
        _elements.ddlSearchDepartment.value('');
        _elements.ddlSearchLocation.setDataSource([]);
        _elements.ddlSearchLocation.value('');
        
        if (sectorCode && lobCode) {
            loadDepartmentsBySearchSectorLOB(sectorCode, lobCode);
        }
    }
    
    function onSearchDepartmentChange() {
        if (!_elements.ddlSearchSector || !_elements.ddlSearchLOB || !_elements.ddlSearchDepartment || !_elements.ddlSearchLocation) return;
        
        var sectorCode = _elements.ddlSearchSector.value();
        var lobCode = _elements.ddlSearchLOB.value();
        var departmentCode = _elements.ddlSearchDepartment.value();
        
        // Reset location dropdown
        _elements.ddlSearchLocation.setDataSource([]);
        _elements.ddlSearchLocation.value('');
        
        if (sectorCode && lobCode && departmentCode) {
            loadLocationsBySearchDepartment(sectorCode, lobCode, departmentCode);
        }
    }
    
    function loadLOBsBySearchSector(sectorCode) {
        $.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function(response) {
                if (response.success && _elements.ddlSearchLOB) {
                    _elements.ddlSearchLOB.setDataSource(response.data);
                }
            })
            .fail(function() {
                TelerikNotification.error('Error loading LOBs');
            });
    }
    
    function loadDepartmentsBySearchSectorLOB(sectorCode, lobCode) {
        $.get('/MasterData/GetDepartments', { sectorCode: sectorCode, lobCode: lobCode })
            .done(function(response) {
                if (response.success && _elements.ddlSearchDepartment) {
                    _elements.ddlSearchDepartment.setDataSource(response.data);
                }
            })
            .fail(function() {
                TelerikNotification.error('Error loading departments');
            });
    }
    
    function loadLocationsBySearchDepartment(sectorCode, lobCode, departmentCode) {
        $.get('/MasterData/GetLocations', { sectorCode: sectorCode, lobCode: lobCode, departmentCode: departmentCode })
            .done(function(response) {
                if (response.success && _elements.ddlSearchLocation) {
                    _elements.ddlSearchLocation.setDataSource(response.data);
                }
            })
            .fail(function() {
                TelerikNotification.error('Error loading locations');
            });
    }
    
    // Form section functions
    function onSectorChange() {
        if (!_elements.ddlSector || !_elements.ddlLOB) return;
        
        var sectorCode = _elements.ddlSector.value();
        
        // Reset dependent dropdowns
        _elements.ddlLOB.setDataSource([]);
        _elements.ddlLOB.value('');
        _elements.ddlDepartment.setDataSource([]);
        _elements.ddlDepartment.value('');
        _elements.ddlLocation.setDataSource([]);
        _elements.ddlLocation.value('');
        
        if (sectorCode) {
            loadLOBsBySector(sectorCode);
        }
    }
    
    function onLOBChange() {
        if (!_elements.ddlSector || !_elements.ddlLOB || !_elements.ddlDepartment) return;
        
        var sectorCode = _elements.ddlSector.value();
        var lobCode = _elements.ddlLOB.value();
        
        // Reset dependent dropdowns
        _elements.ddlDepartment.setDataSource([]);
        _elements.ddlDepartment.value('');
        _elements.ddlLocation.setDataSource([]);
        _elements.ddlLocation.value('');
        
        if (sectorCode && lobCode) {
            loadDepartmentsBySectorLOB(sectorCode, lobCode);
        }
    }
    
    function onDepartmentChange() {
        if (!_elements.ddlSector || !_elements.ddlLOB || !_elements.ddlDepartment || !_elements.ddlLocation) return;
        
        var sectorCode = _elements.ddlSector.value();
        var lobCode = _elements.ddlLOB.value();
        var departmentCode = _elements.ddlDepartment.value();
        
        // Reset location dropdown
        _elements.ddlLocation.setDataSource([]);
        _elements.ddlLocation.value('');
        
        if (sectorCode && lobCode && departmentCode) {
            loadLocationsByDepartment(sectorCode, lobCode, departmentCode);
        }
    }
    
    function loadLOBsBySector(sectorCode) {
        TelerikSkeleton.showDropdownSkeleton("#ddlLOB");
        
        $.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function(response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLOB");
                if (response.success && _elements.ddlLOB) {
                    _elements.ddlLOB.setDataSource(response.data);
                }
            })
            .fail(function() {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLOB");
                TelerikNotification.error('Error loading LOBs');
            });
    }
    
    function loadDepartmentsBySectorLOB(sectorCode, lobCode) {
        TelerikSkeleton.showDropdownSkeleton("#ddlDepartment");
        
        $.get('/MasterData/GetDepartments', { sectorCode: sectorCode, lobCode: lobCode })
            .done(function(response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlDepartment");
                if (response.success && _elements.ddlDepartment) {
                    _elements.ddlDepartment.setDataSource(response.data);
                }
            })
            .fail(function() {
                TelerikSkeleton.hideDropdownSkeleton("#ddlDepartment");
                TelerikNotification.error('Error loading departments');
            });
    }
    
    function loadLocationsByDepartment(sectorCode, lobCode, departmentCode) {
        TelerikSkeleton.showDropdownSkeleton("#ddlLocation");
        
        $.get('/MasterData/GetLocations', { sectorCode: sectorCode, lobCode: lobCode, departmentCode: departmentCode })
            .done(function(response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLocation");
                if (response.success && _elements.ddlLocation) {
                    _elements.ddlLocation.setDataSource(response.data);
                }
            })
            .fail(function() {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLocation");
                TelerikNotification.error('Error loading locations');
            });
    }
    
    function searchCopyToList() {
        if (!_elements.copyToGrid) {
            TelerikNotification.error('Grid not initialized');
            return;
        }
        
        var searchCriteria = {
            sbaCode: _elements.ddlSearchSector ? _elements.ddlSearchSector.value() : '',
            sbuCode: _elements.ddlSearchLOB ? _elements.ddlSearchLOB.value() : '',
            departmentCode: _elements.ddlSearchDepartment ? _elements.ddlSearchDepartment.value() : '',
            locationCode: _elements.ddlSearchLocation ? _elements.ddlSearchLocation.value() : ''
        };
        
        // Show skeleton loading for grid
        showGridSkeleton();
        
        $.ajax({
            url: '/Maintenance/GetCopyToList',
            type: 'POST',
            data: searchCriteria,
            success: function(response) {
                hideGridSkeleton();
                
                if (response.success) {
                    var dataSource = new kendo.data.DataSource({
                        data: response.data || [],
                        pageSize: 20,
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
                                    LocationCode: { type: "string" },
                                    LocationName: { type: "string" },
                                    UserId: { type: "string" },
                                    UserName: { type: "string" },
                                    InactiveDate: { type: "date" },
                                    Uid: { type: "string" }
                                }
                            }
                        }
                    });
                    
                    _elements.copyToGrid.setDataSource(dataSource);
                    
                    // Show appropriate notifications
                    var totalCount = response.totalCount || 0;
                    var hasSearchCriteria = !!(searchCriteria.sbaCode || searchCriteria.sbuCode || 
                                             searchCriteria.departmentCode || searchCriteria.locationCode);
                    
                    if (hasSearchCriteria) {
                        if (totalCount === 0) {
                            TelerikNotification.warning('No copy-to entries found matching your search criteria.');
                        } else if (totalCount === 1) {
                            TelerikNotification.success('Found 1 copy-to entry matching your search.');
                        }
                        // Don't show notification for normal result counts to avoid spam
                    }
                } else {
                    TelerikNotification.error(response.message || 'Error searching copy-to list');
                    _elements.copyToGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
                }
            },
            error: function() {
                hideGridSkeleton();
                TelerikNotification.error('Error searching copy-to list. Please try again.');
                _elements.copyToGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
            }
        });
    }
    
    function showGridSkeleton() {
        // Simple skeleton loading implementation
        if (_elements.copyToGrid) {
            var skeletonData = [];
            for (var i = 0; i < 5; i++) {
                skeletonData.push({
                    Uid: 'skeleton-' + i,
                    SbaName: '...',
                    SbuName: '...',
                    DepartmentName: '...',
                    LocationName: '...',
                    UserId: '...',
                    UserName: '...',
                    InactiveDate: null
                });
            }
            _elements.copyToGrid.setDataSource(new kendo.data.DataSource({ data: skeletonData }));
        }
    }
    
    function hideGridSkeleton() {
        // Grid will be updated with actual data
    }
    
    function showNewForm() {
        _isEditMode = false;
        _currentMode = 'form';
        
        clearForm();
        _elements.formTitle.text('Create Copy To Entry');
        
        showFormSection();
    }
    
    function editCopyTo(dataItem) {
        _isEditMode = true;
        _currentMode = 'form';
        
        _elements.formTitle.text('Edit Copy To Entry');
        
        // Populate form with existing data
        populateForm(dataItem);
        
        showFormSection();
    }
    
    function populateForm(data) {
        if (!data) return;
        
        // Set hidden ID
        $("#hiddenUid").val(data.Uid);
        
        // Set sector first, then cascade load other dropdowns
        if (_elements.ddlSector && data.SbaCode) {
            _elements.ddlSector.value(data.SbaCode);
            
            // Load and set LOB
            if (data.SbuCode) {
                loadLOBsBySector(data.SbaCode, function() {
                    _elements.ddlLOB.value(data.SbuCode);
                    
                    // Load and set Department
                    if (data.DepartmentCode) {
                        loadDepartmentsBySectorLOB(data.SbaCode, data.SbuCode, function() {
                            _elements.ddlDepartment.value(data.DepartmentCode);
                            
                            // Load and set Location
                            if (data.LocationCode) {
                                loadLocationsByDepartment(data.SbaCode, data.SbuCode, data.DepartmentCode, function() {
                                    _elements.ddlLocation.value(data.LocationCode);
                                });
                            }
                        });
                    }
                });
            }
        }
        
        // Set other fields
        if (_elements.txtUserId) _elements.txtUserId.value(data.UserId || '');
        if (_elements.txtUserName) _elements.txtUserName.value(data.UserName || '');
        if (_elements.dtpInactiveDate && data.InactiveDate) {
            _elements.dtpInactiveDate.value(new Date(data.InactiveDate));
        }
    }
    
    function clearForm() {
        $("#hiddenUid").val('');
        
        // Clear dropdowns
        if (_elements.ddlSector) _elements.ddlSector.value('');
        if (_elements.ddlLOB) {
            _elements.ddlLOB.setDataSource([]);
            _elements.ddlLOB.value('');
        }
        if (_elements.ddlDepartment) {
            _elements.ddlDepartment.setDataSource([]);
            _elements.ddlDepartment.value('');
        }
        if (_elements.ddlLocation) {
            _elements.ddlLocation.setDataSource([]);
            _elements.ddlLocation.value('');
        }
        
        // Clear text fields
        if (_elements.txtUserId) _elements.txtUserId.value('');
        if (_elements.txtUserName) _elements.txtUserName.value('');
        if (_elements.dtpInactiveDate) _elements.dtpInactiveDate.value(null);
        
        // Clear validation errors
        $('.validation-error').empty();
    }
    
    function saveCopyTo() {
        if (!validateForm()) {
            return;
        }
        
        var formData = {
            Uid: $("#hiddenUid").val(),
            SbaCode: _elements.ddlSector ? _elements.ddlSector.value() : '',
            SbuCode: _elements.ddlLOB ? _elements.ddlLOB.value() : '',
            DepartmentCode: _elements.ddlDepartment ? _elements.ddlDepartment.value() : '',
            LocationCode: _elements.ddlLocation ? _elements.ddlLocation.value() : '',
            UserId: _elements.txtUserId ? _elements.txtUserId.value() : '',
            UserName: _elements.txtUserName ? _elements.txtUserName.value() : '',
            InactiveDate: _elements.dtpInactiveDate && _elements.dtpInactiveDate.value() ? 
                         kendo.toString(_elements.dtpInactiveDate.value(), "yyyy-MM-dd") : null
        };
        
        // Disable save button during submission
        $("#btnSave").data("kendoButton").enable(false);
        
        $.ajax({
            url: '/Maintenance/SaveCopyToList',
            type: 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            success: function(response) {
                $("#btnSave").data("kendoButton").enable(true);
                
                if (response.success) {
                    TelerikNotification.success("Copy-to entry saved successfully!");
                    
                    // Refresh the grid and return to search
                    searchCopyToList();
                    showSearchSection();
                } else {
                    TelerikNotification.error(response.message || 'Error saving copy-to entry');
                }
            },
            error: function() {
                $("#btnSave").data("kendoButton").enable(true);
                TelerikNotification.error('Error saving copy-to entry. Please try again.');
            }
        });
    }
    
    function validateForm() {
        var isValid = true;
        
        // Clear previous errors
        $('.validation-error').empty();
        
        // Validate Sector
        if (!_elements.ddlSector || !_elements.ddlSector.value()) {
            showValidationError('sectorError', 'Sector is required');
            isValid = false;
        }
        
        // Validate LOB
        if (!_elements.ddlLOB || !_elements.ddlLOB.value()) {
            showValidationError('lobError', 'LOB is required');
            isValid = false;
        }
        
        // Validate Department
        if (!_elements.ddlDepartment || !_elements.ddlDepartment.value()) {
            showValidationError('departmentError', 'Department is required');
            isValid = false;
        }
        
        // Validate Location
        if (!_elements.ddlLocation || !_elements.ddlLocation.value()) {
            showValidationError('locationError', 'Location is required');
            isValid = false;
        }
        
        // Validate User ID
        var userId = _elements.txtUserId ? _elements.txtUserId.value() : '';
        if (!userId) {
            showValidationError('userIdError', 'User ID is required');
            isValid = false;
        } else if (userId.length !== 8 || !/^\d{8}$/.test(userId)) {
            showValidationError('userIdError', 'User ID must be exactly 8 digits');
            isValid = false;
        }
        
        // Validate User Name
        var userName = _elements.txtUserName ? _elements.txtUserName.value() : '';
        if (!userName) {
            showValidationError('userNameError', 'User Name is required');
            isValid = false;
        } else if (userName.length > 100) {
            showValidationError('userNameError', 'User Name cannot exceed 100 characters');
            isValid = false;
        }
        
        return isValid;
    }
    
    function showValidationError(elementId, message) {
        $("#" + elementId).html(`<span class="validation-error">${message}</span>`);
    }
    
    function cancelForm() {
        if (_isEditMode || hasFormChanges()) {
            if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
                showSearchSection();
            }
        } else {
            showSearchSection();
        }
    }
    
    function hasFormChanges() {
        // Simple check if form has any values
        return !!(_elements.ddlSector && _elements.ddlSector.value()) ||
               !!(_elements.txtUserId && _elements.txtUserId.value()) ||
               !!(_elements.txtUserName && _elements.txtUserName.value());
    }
    
    function showSearchSection() {
        _currentMode = 'search';
        _elements.entryFormSection.hide();
        _elements.searchSection.show();
        $("#resultsSection").show();
    }
    
    function showFormSection() {
        _currentMode = 'form';
        _elements.searchSection.hide();
        $("#resultsSection").hide();
        _elements.entryFormSection.addClass('show').show();
    }
    
    return {
        init: init,
        searchCopyToList: searchCopyToList,
        showNewForm: showNewForm,
        editCopyTo: editCopyTo,
        saveCopyTo: saveCopyTo,
        cancelForm: cancelForm,
        onSearchSectorChange: onSearchSectorChange,
        onSearchLOBChange: onSearchLOBChange,
        onSearchDepartmentChange: onSearchDepartmentChange,
        onSectorChange: onSectorChange,
        onLOBChange: onLOBChange,
        onDepartmentChange: onDepartmentChange
    };
    
})();

// Global functions for event handlers
window.searchCopyToList = maintenanceCopyToViewModel.searchCopyToList;
window.showNewForm = maintenanceCopyToViewModel.showNewForm;
window.saveCopyTo = maintenanceCopyToViewModel.saveCopyTo;
window.cancelForm = maintenanceCopyToViewModel.cancelForm;
window.onSearchSectorChange = maintenanceCopyToViewModel.onSearchSectorChange;
window.onSearchLOBChange = maintenanceCopyToViewModel.onSearchLOBChange;
window.onSearchDepartmentChange = maintenanceCopyToViewModel.onSearchDepartmentChange;
window.onSectorChange = maintenanceCopyToViewModel.onSectorChange;
window.onLOBChange = maintenanceCopyToViewModel.onLOBChange;
window.onDepartmentChange = maintenanceCopyToViewModel.onDepartmentChange;