var locationMaintenanceViewModel = (function () {

    var _currentMode = 'search';
    var _isEditMode = false;

    var _elements = {
        ddlSector: null,
        ddlLOB: null,
        ddlDepartment: null,
        ddlFormSector: null,
        ddlFormLOB: null,
        ddlFormDepartment: null,
        locationGrid: null,
        txtLocationCode: null,
        txtLocationName: null,
        dtpInactiveDate: null,
        searchSection: null,
        resultsSection: null,
        entryFormSection: null,
        formTitle: null
    };

    var _currentSectorCode = null;
    var _currentLOBCode = null;
    var _currentDepartmentCode = null;

    function init() {
        initializeElements();
        showSearchSection();
    }

    function initializeElements() {
        _elements.ddlSector = $("#ddlSector").data("kendoDropDownList");
        _elements.ddlLOB = $("#ddlLOB").data("kendoDropDownList");
        _elements.ddlDepartment = $("#ddlDepartment").data("kendoDropDownList");
        _elements.ddlFormSector = $("#ddlFormSector").data("kendoDropDownList");
        _elements.ddlFormLOB = $("#ddlFormLOB").data("kendoDropDownList");
        _elements.ddlFormDepartment = $("#ddlFormDepartment").data("kendoDropDownList");
        _elements.locationGrid = $("#locationGrid").data("kendoGrid");
        _elements.txtLocationCode = $("#txtLocationCode").data("kendoTextBox");
        _elements.txtLocationName = $("#txtLocationName").data("kendoTextBox");
        _elements.dtpInactiveDate = $("#txtInactiveDate").data("kendoDatePicker");
        _elements.searchSection = $("#searchSection");
        _elements.resultsSection = $("#resultsSection");
        _elements.entryFormSection = $("#entryFormSection");
        _elements.formTitle = $("#formTitle");

        $('#locationForm').on('submit', function (e) {
            e.preventDefault();
            return false;
        });
    }

    function showLoading() {
        kendo.ui.progress($("#locationGrid"), true);
    }

    function hideLoading() {
        kendo.ui.progress($("#locationGrid"), false);
    }

    function showNotification(message, type) {
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
    }

    function clearValidationErrors() {
        $('.validation-error').removeClass('show').text('');
    }

    function showValidationError(fieldId, message) {
        $('#' + fieldId + 'Error').text(message).addClass('show');
    }

    function validateLocationForm() {
        clearValidationErrors();
        var isValid = true;

        var locationName = _elements.txtLocationName ? _elements.txtLocationName.value() : '';
        var locationCode = _elements.txtLocationCode ? _elements.txtLocationCode.value() : '';

        if (!locationCode || locationCode.trim() === '') {
            showValidationError('locationCode', 'Location Code is required');
            isValid = false;
        }

        if (!locationName || locationName.trim() === '') {
            showValidationError('locationName', 'Location Name is required');
            isValid = false;
        } else if (locationName.length > 100) {
            showValidationError('locationName', 'Location Name cannot exceed 100 characters');
            isValid = false;
        }

        return isValid;
    }

    function onSectorChange() {
        if (!_elements.ddlSector || !_elements.ddlLOB) return;

        var sectorCode = _elements.ddlSector.value();
        _currentSectorCode = sectorCode;

        _elements.ddlLOB.setDataSource([]);
        _elements.ddlLOB.value('');
        _elements.ddlDepartment.setDataSource([]);
        _elements.ddlDepartment.value('');

        if (_elements.locationGrid) {
            _elements.locationGrid.dataSource.data([]);
        }

        if (sectorCode) {
            loadLOBList(sectorCode, _elements.ddlLOB);
        }
    }

    function onLOBChange() {
        if (!_elements.ddlSector || !_elements.ddlLOB || !_elements.ddlDepartment) return;

        var sectorCode = _elements.ddlSector.value();
        var lobCode = _elements.ddlLOB.value();
        _currentLOBCode = lobCode;

        _elements.ddlDepartment.setDataSource([]);
        _elements.ddlDepartment.value('');

        if (_elements.locationGrid) {
            _elements.locationGrid.dataSource.data([]);
        }

        if (sectorCode && lobCode) {
            loadDepartmentList(sectorCode, lobCode, _elements.ddlDepartment);
        }
    }

    function onDepartmentChange() {
        var deptCode = _elements.ddlDepartment ? _elements.ddlDepartment.value() : '';
        _currentDepartmentCode = deptCode;

        if (_elements.locationGrid) {
            _elements.locationGrid.dataSource.data([]);
        }
    }

    function onFormSectorChange() {
        if (!_elements.ddlFormSector || !_elements.ddlFormLOB) return;

        var sectorCode = _elements.ddlFormSector.value();

        _elements.ddlFormLOB.setDataSource([]);
        _elements.ddlFormLOB.value('');
        _elements.ddlFormDepartment.setDataSource([]);
        _elements.ddlFormDepartment.value('');

        if (sectorCode) {
            loadLOBList(sectorCode, _elements.ddlFormLOB);
        }
    }

    function onFormLOBChange() {
        if (!_elements.ddlFormSector || !_elements.ddlFormLOB || !_elements.ddlFormDepartment) return;

        var sectorCode = _elements.ddlFormSector.value();
        var lobCode = _elements.ddlFormLOB.value();

        _elements.ddlFormDepartment.setDataSource([]);
        _elements.ddlFormDepartment.value('');

        if (sectorCode && lobCode) {
            loadDepartmentList(sectorCode, lobCode, _elements.ddlFormDepartment);
        }
    }

    function loadLOBList(sectorCode, targetDropdown) {
        ApiConfig.ajax({
            url: '/MasterData/GetLOBs',
            type: 'GET',
            data: { sectorCode: sectorCode },
            success: function (response) {
                if (response && Array.isArray(response) && targetDropdown) {
                    targetDropdown.setDataSource(response);
                    targetDropdown.value('');
                } else if (targetDropdown) {
                    targetDropdown.setDataSource([]);
                }
            },
            error: function (xhr, status, error) {
                showNotification('Error loading LOB list: ' + error, 'error');
                if (targetDropdown) {
                    targetDropdown.setDataSource([]);
                }
            }
        });
    }

    function loadDepartmentList(sectorCode, lobCode, targetDropdown) {
        ApiConfig.ajax({
            url: '/MasterData/GetDepartments',
            type: 'GET',
            data: {
                sectorCode: sectorCode,
                lobCode: lobCode
            },
            success: function (response) {
                if (response && Array.isArray(response) && targetDropdown) {
                    targetDropdown.setDataSource(response);
                    targetDropdown.value('');
                } else if (targetDropdown) {
                    targetDropdown.setDataSource([]);
                }
            },
            error: function (xhr, status, error) {
                showNotification('Error loading department list: ' + error, 'error');
                if (targetDropdown) {
                    targetDropdown.setDataSource([]);
                }
            }
        });
    }

    function searchLocations() {
        if (!_currentSectorCode || !_currentLOBCode || !_currentDepartmentCode) {
            showNotification('Please select all filters first', 'error');
            return;
        }

        loadLocationList(_currentSectorCode, _currentLOBCode, _currentDepartmentCode);
    }

    function loadLocationList(sectorCode, lobCode, departmentCode) {
        showLoading();

        ApiConfig.ajax({
            url: '/Maintenance/GetLocationList',
            type: 'POST',
            data: {
                sbaCode: sectorCode,
                sbuCode: lobCode,
                departmentCode: departmentCode
            },
            success: function (response) {
                hideLoading();

                if (response && Array.isArray(response)) {
                    var mappedData = response.map(function (item) {
                        return {
                            SbaCode: item.sbaCode || item.SbaCode || '',
                            SbaName: item.sbaName || item.SbaName || '',
                            SbuCode: item.sbuCode || item.SbuCode || '',
                            SbuName: item.sbuName || item.SbuName || '',
                            DepartmentCode: item.departmentCode || item.DepartmentCode || '',
                            DepartmentName: item.departmentName || item.DepartmentName || '',
                            LocationCode: item.locationCode || item.LocationCode || '',
                            LocationName: item.locationName || item.LocationName || '',
                            InactiveDate: item.inactiveDate || item.InactiveDate || '',
                            Uid: item.uid || item.Uid || ''
                        };
                    });

                    if (_elements.locationGrid) {
                        var dataSource = new kendo.data.DataSource({
                            data: mappedData,
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
                                        LocationCode: { type: "string" },
                                        LocationName: { type: "string" },
                                        InactiveDate: { type: "string" },
                                        Uid: { type: "string" }
                                    }
                                }
                            }
                        });

                        _elements.locationGrid.setDataSource(dataSource);
                    }
                } else {
                    showNotification('Failed to load location list', 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error loading location list: ' + error, 'error');
            }
        });
    }

    function showNewForm() {
        if (!_currentSectorCode || !_currentLOBCode || !_currentDepartmentCode) {
            showNotification('Please select all filters first', 'error');
            return;
        }

        _isEditMode = false;
        _currentMode = 'form';

        clearLocationForm();

        if (_elements.ddlFormSector) {
            _elements.ddlFormSector.value(_currentSectorCode);
            loadLOBList(_currentSectorCode, _elements.ddlFormLOB);

            setTimeout(function () {
                if (_elements.ddlFormLOB) {
                    _elements.ddlFormLOB.value(_currentLOBCode);
                    loadDepartmentList(_currentSectorCode, _currentLOBCode, _elements.ddlFormDepartment);

                    setTimeout(function () {
                        if (_elements.ddlFormDepartment) {
                            _elements.ddlFormDepartment.value(_currentDepartmentCode);
                        }
                    }, 300);
                }
            }, 300);
        }

        if (_elements.formTitle) {
            _elements.formTitle.text('Create Location');
        }

        showFormSection();

        setTimeout(function () {
            generateLocationCode();
        }, 800);
    }

    function editLocationRecord(sbaCode, sbuCode, departmentCode, locationCode) {
        _isEditMode = true;
        _currentMode = 'form';

        if (_elements.formTitle) {
            _elements.formTitle.text('Edit Location');
        }
        loadLocationRecord(sbaCode, sbuCode, departmentCode, locationCode);
    }

    function loadLocationRecord(sbaCode, sbuCode, departmentCode, locationCode) {
        showLoading();

        ApiConfig.ajax({
            url: '/Maintenance/GetLocationByUid',
            type: 'POST',
            data: {
                sbaCode: sbaCode,
                sbuCode: sbuCode,
                departmentCode: departmentCode,
                locationCode: locationCode
            },
            success: function (response) {
                hideLoading();
                if (response) {
                    var mappedData = {
                        SbaCode: response.sbaCode || response.SbaCode || sbaCode,
                        SbuCode: response.sbuCode || response.SbuCode || sbuCode,
                        DepartmentCode: response.departmentCode || response.DepartmentCode || departmentCode,
                        LocationCode: response.locationCode || response.LocationCode,
                        LocationName: response.locationName || response.LocationName,
                        InactiveDate: response.inactiveDate || response.InactiveDate
                    };

                    populateLocationForm(mappedData);
                    showFormSection();
                } else {
                    showNotification('Location record not found', 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error loading location record: ' + error, 'error');
            }
        });
    }

    function populateLocationForm(data) {
        if (_elements.ddlFormSector && data.SbaCode) {
            _elements.ddlFormSector.value(data.SbaCode);
            loadLOBList(data.SbaCode, _elements.ddlFormLOB);
        }

        setTimeout(function () {
            if (_elements.ddlFormLOB && data.SbuCode) {
                _elements.ddlFormLOB.value(data.SbuCode);
                loadDepartmentList(data.SbaCode, data.SbuCode, _elements.ddlFormDepartment);
            }
        }, 300);

        setTimeout(function () {
            if (_elements.ddlFormDepartment && data.DepartmentCode) {
                _elements.ddlFormDepartment.value(data.DepartmentCode);
            }
        }, 600);

        if (_elements.txtLocationCode) {
            _elements.txtLocationCode.value(data.LocationCode || '');
        }
        if (_elements.txtLocationName) {
            _elements.txtLocationName.value(data.LocationName || '');
        }

        if (data.InactiveDate && _elements.dtpInactiveDate) {
            _elements.dtpInactiveDate.value(new Date(data.InactiveDate));
        }
    }

    function clearLocationForm() {
        if (_elements.txtLocationCode) {
            _elements.txtLocationCode.value('');
        }
        if (_elements.txtLocationName) {
            _elements.txtLocationName.value('');
        }
        if (_elements.dtpInactiveDate) {
            _elements.dtpInactiveDate.value(null);
        }

        clearValidationErrors();
    }

    function generateLocationCode() {
        var formLobCode = _elements.ddlFormLOB ? _elements.ddlFormLOB.value() : '';

        if (!formLobCode) {
            showNotification('Please select LOB first', 'error');
            return;
        }

        ApiConfig.ajax({
            url: '/Maintenance/GenerateLocationCode',
            type: 'POST',
            data: { sbuCode: formLobCode },
            success: function (response) {
                if (response.success && _elements.txtLocationCode) {
                    _elements.txtLocationCode.value(response.code);
                } else {
                    showNotification('Failed to generate location code: ' + response.message, 'error');
                }
            },
            error: function (xhr, status, error) {
                showNotification('Error generating location code: ' + error, 'error');
            }
        });
    }

    function saveLocationRecord() {
        if (!validateLocationForm()) {
            return;
        }

        var formSectorCode = _elements.ddlFormSector ? _elements.ddlFormSector.value() : '';
        var formLobCode = _elements.ddlFormLOB ? _elements.ddlFormLOB.value() : '';
        var formDeptCode = _elements.ddlFormDepartment ? _elements.ddlFormDepartment.value() : '';

        var formData = {
            SbaCode: formSectorCode,
            SbuCode: formLobCode,
            DepartmentCode: formDeptCode,
            LocationCode: _elements.txtLocationCode ? _elements.txtLocationCode.value() : '',
            LocationName: _elements.txtLocationName ? _elements.txtLocationName.value() : '',
            InactiveDate: ''
        };

        if (_elements.dtpInactiveDate && _elements.dtpInactiveDate.value()) {
            formData.InactiveDate = kendo.toString(_elements.dtpInactiveDate.value(), 'dd/MM/yyyy');
        }

        showLoading();

        ApiConfig.ajax({
            url: '/Maintenance/SaveLocation',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                hideLoading();
                if (response.success) {
                    showNotification(response.message, 'success');
                    loadLocationList(formSectorCode, formLobCode, formDeptCode);
                    showSearchSection();
                } else {
                    showNotification(response.message, 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error saving location record: ' + error, 'error');
            }
        });
    }

    function cancelLocationEdit() {
        if (_isEditMode || hasFormChanges()) {
            if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
                clearLocationForm();
                showSearchSection();
            }
        } else {
            clearLocationForm();
            showSearchSection();
        }
    }

    function hasFormChanges() {
        return !!(_elements.txtLocationCode && _elements.txtLocationCode.value()) ||
            !!(_elements.txtLocationName && _elements.txtLocationName.value());
    }

    function showSearchSection() {
        _currentMode = 'search';
        _isEditMode = false;
        if (_elements.entryFormSection) {
            _elements.entryFormSection.hide();
        }
        if (_elements.searchSection) {
            _elements.searchSection.show();
        }
        if (_elements.resultsSection) {
            _elements.resultsSection.show();
        }
    }

    function showFormSection() {
        _currentMode = 'form';
        if (_elements.searchSection) {
            _elements.searchSection.hide();
        }
        if (_elements.resultsSection) {
            _elements.resultsSection.hide();
        }
        if (_elements.entryFormSection) {
            _elements.entryFormSection.show();
        }
    }

    return {
        init: init,
        onSectorChange: onSectorChange,
        onLOBChange: onLOBChange,
        onDepartmentChange: onDepartmentChange,
        onFormSectorChange: onFormSectorChange,
        onFormLOBChange: onFormLOBChange,
        searchLocations: searchLocations,
        showNewForm: showNewForm,
        editLocationRecord: editLocationRecord,
        saveLocationRecord: saveLocationRecord,
        cancelLocationEdit: cancelLocationEdit
    };

})();

function onSectorChange() {
    locationMaintenanceViewModel.onSectorChange();
}

function onLOBChange() {
    locationMaintenanceViewModel.onLOBChange();
}

function onDepartmentChange() {
    locationMaintenanceViewModel.onDepartmentChange();
}

function onFormSectorChange() {
    locationMaintenanceViewModel.onFormSectorChange();
}

function onFormLOBChange() {
    locationMaintenanceViewModel.onFormLOBChange();
}

function searchLocations() {
    locationMaintenanceViewModel.searchLocations();
}

function newLocationRecord() {
    locationMaintenanceViewModel.showNewForm();
}

function editLocationRecord(sbaCode, sbuCode, departmentCode, locationCode) {
    locationMaintenanceViewModel.editLocationRecord(sbaCode, sbuCode, departmentCode, locationCode);
}

function saveLocationRecord() {
    locationMaintenanceViewModel.saveLocationRecord();
}

function cancelLocationEdit() {
    locationMaintenanceViewModel.cancelLocationEdit();
}