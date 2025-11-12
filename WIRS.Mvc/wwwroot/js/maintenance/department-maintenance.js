var departmentMaintenanceViewModel = (function () {

    var _currentMode = 'search';
    var _isEditMode = false;

    var _elements = {
        ddlSector: null,
        ddlLOB: null,
        ddlFormSector: null,
        ddlFormLOB: null,
        txtDepartmentSearch: null,
        departmentGrid: null,
        txtDepartmentCode: null,
        txtDepartmentName: null,
        dtpInactiveDate: null,
        searchSection: null,
        resultsSection: null,
        entryFormSection: null,
        formTitle: null
    };

    var _currentSectorCode = null;
    var _currentLOBCode = null;

    function init() {
        initializeElements();
        showSearchSection();
    }

    function initializeElements() {
        _elements.ddlSector = $("#ddlSector").data("kendoDropDownList");
        _elements.ddlLOB = $("#ddlLOB").data("kendoDropDownList");
        _elements.ddlFormSector = $("#ddlFormSector").data("kendoDropDownList");
        _elements.ddlFormLOB = $("#ddlFormLOB").data("kendoDropDownList");
        _elements.txtDepartmentSearch = $("#txtDepartmentSearch").data("kendoTextBox");
        _elements.departmentGrid = $("#departmentGrid").data("kendoGrid");
        _elements.txtDepartmentCode = $("#txtDepartmentCode").data("kendoTextBox");
        _elements.txtDepartmentName = $("#txtDepartmentName").data("kendoTextBox");
        _elements.dtpInactiveDate = $("#txtInactiveDate").data("kendoDatePicker");
        _elements.searchSection = $("#searchSection");
        _elements.resultsSection = $("#resultsSection");
        _elements.entryFormSection = $("#entryFormSection");
        _elements.formTitle = $("#formTitle");

        $('#departmentForm').on('submit', function (e) {
            e.preventDefault();
            return false;
        });
    }

    function showLoading() {
        kendo.ui.progress($("#departmentGrid"), true);
    }

    function hideLoading() {
        kendo.ui.progress($("#departmentGrid"), false);
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
        $('.validation-error').text('');
    }

    function showValidationError(fieldId, message) {
        $('#' + fieldId).text(message);
    }

    function validateForm() {
        clearValidationErrors();
        var isValid = true;

        var departmentName = _elements.txtDepartmentName ? _elements.txtDepartmentName.value().trim() : '';
        if (!departmentName) {
            showValidationError('departmentNameError', 'Department Name is required');
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

        if (_elements.departmentGrid) {
            _elements.departmentGrid.dataSource.data([]);
        }

        if (sectorCode) {
            loadLOBList(sectorCode, _elements.ddlLOB);
        }
    }

    function onLOBChange() {
        var lobCode = _elements.ddlLOB ? _elements.ddlLOB.value() : '';
        _currentLOBCode = lobCode;

        if (_elements.departmentGrid) {
            _elements.departmentGrid.dataSource.data([]);
        }
    }

    function onFormSectorChange() {
        if (!_elements.ddlFormSector || !_elements.ddlFormLOB) return;

        var sectorCode = _elements.ddlFormSector.value();

        _elements.ddlFormLOB.setDataSource([]);
        _elements.ddlFormLOB.value('');

        if (sectorCode) {
            loadLOBList(sectorCode, _elements.ddlFormLOB);
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

    function searchDepartments() {
        if (!_currentSectorCode) {
            showNotification('Please select a Sector', 'error');
            return;
        }

        if (!_currentLOBCode) {
            showNotification('Please select a LOB', 'error');
            return;
        }

        var departmentName = _elements.txtDepartmentSearch ? _elements.txtDepartmentSearch.value().trim() : '';

        showLoading();

        ApiConfig.ajax({
            url: '/Maintenance/GetDepartmentList',
            type: 'POST',
            data: {
                sbaCode: _currentSectorCode,
                sbuCode: _currentLOBCode,
                departmentName: departmentName
            },
            success: function (response) {
                hideLoading();

                if (response && Array.isArray(response)) {
                    if (_elements.departmentGrid) {
                        var dataSource = new kendo.data.DataSource({
                            data: response,
                            pageSize: 15,
                            schema: {
                                model: {
                                    id: "uid",
                                    fields: {
                                        sbaCode: { type: "string" },
                                        sbaName: { type: "string" },
                                        sbuCode: { type: "string" },
                                        sbuName: { type: "string" },
                                        departmentCode: { type: "string" },
                                        departmentName: { type: "string" },
                                        inactiveDate: { type: "string" },
                                        uid: { type: "string" },
                                        codeType: { type: "string" }
                                    }
                                }
                            }
                        });

                        _elements.departmentGrid.setDataSource(dataSource);
                    }

                    if (response.length === 0) {
                        showNotification('No department records found for the selected criteria', 'info');
                    }
                } else {
                    showNotification('No data received from server', 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error loading department records: ' + error, 'error');
            }
        });
    }

    function showNewForm() {
        if (!_currentSectorCode) {
            showNotification('Please select a Sector first', 'error');
            return;
        }

        if (!_currentLOBCode) {
            showNotification('Please select a LOB first', 'error');
            return;
        }

        _isEditMode = false;
        _currentMode = 'form';

        clearDepartmentForm();

        if (_elements.ddlFormSector) {
            _elements.ddlFormSector.value(_currentSectorCode);
            loadLOBList(_currentSectorCode, _elements.ddlFormLOB);

            setTimeout(function () {
                if (_elements.ddlFormLOB) {
                    _elements.ddlFormLOB.value(_currentLOBCode);
                }
            }, 300);
        }

        if (_elements.formTitle) {
            _elements.formTitle.text('Create Department');
        }

        setTimeout(function () {
            generateDepartmentCode();
        }, 500);
    }

    function generateDepartmentCode() {
        var formLobCode = _elements.ddlFormLOB ? _elements.ddlFormLOB.value() : '';

        if (!formLobCode) {
            showNotification('Please select LOB first', 'error');
            return;
        }

        showLoading();

        ApiConfig.ajax({
            url: '/Maintenance/GenerateDepartmentCode',
            type: 'POST',
            data: { sbuCode: formLobCode },
            success: function (response) {
                hideLoading();

                if (response && response.success) {
                    if (_elements.txtDepartmentCode) {
                        _elements.txtDepartmentCode.value(response.code);
                    }
                    showFormSection();
                    if (_elements.txtDepartmentName) {
                        var txtDepartmentNameElement = _elements.txtDepartmentName.element;
                        if (txtDepartmentNameElement) {
                            txtDepartmentNameElement.focus();
                        }
                    }
                } else {
                    showNotification('Error generating department code: ' + (response.message || 'Unknown error'), 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error generating department code: ' + error, 'error');
            }
        });
    }

    function editDepartmentRecord(codeType, sbaCode, sbuCode, departmentCode) {
        _isEditMode = true;
        _currentMode = 'form';

        if (_elements.formTitle) {
            _elements.formTitle.text('Edit Department');
        }
        loadDepartmentRecord(codeType, sbaCode, sbuCode, departmentCode);
    }

    function loadDepartmentRecord(codeType, sbaCode, sbuCode, departmentCode) {
        showLoading();

        ApiConfig.ajax({
            url: '/Maintenance/GetDepartmentByUid',
            type: 'POST',
            data: {
                codeType: codeType,
                sbaCode: sbaCode,
                sbuCode: sbuCode,
                departmentCode: departmentCode
            },
            success: function (response) {
                hideLoading();

                if (response) {
                    var mappedData = {
                        SbaCode: response.sbaCode || response.SbaCode || sbaCode,
                        SbuCode: response.sbuCode || response.SbuCode || sbuCode,
                        DepartmentCode: response.departmentCode || response.DepartmentCode || departmentCode,
                        DepartmentName: response.departmentName || response.DepartmentName || '',
                        InactiveDate: response.inactiveDate || response.InactiveDate || ''
                    };

                    populateDepartmentForm(mappedData);
                    showFormSection();
                    if (_elements.txtDepartmentName) {
                        var txtDepartmentNameElement = _elements.txtDepartmentName.element;
                        if (txtDepartmentNameElement) {
                            txtDepartmentNameElement.focus();
                        }
                    }
                } else {
                    showNotification('Department record not found', 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error loading department record: ' + error, 'error');
            }
        });
    }

    function populateDepartmentForm(data) {
        if (_elements.ddlFormSector && data.SbaCode) {
            _elements.ddlFormSector.value(data.SbaCode);
            loadLOBList(data.SbaCode, _elements.ddlFormLOB);
        }

        setTimeout(function () {
            if (_elements.ddlFormLOB && data.SbuCode) {
                _elements.ddlFormLOB.value(data.SbuCode);
            }
        }, 300);

        if (_elements.txtDepartmentCode) {
            _elements.txtDepartmentCode.value(data.DepartmentCode || '');
        }
        if (_elements.txtDepartmentName) {
            _elements.txtDepartmentName.value(data.DepartmentName || '');
        }

        var inactiveDate = data.InactiveDate;
        if (inactiveDate && _elements.dtpInactiveDate) {
            _elements.dtpInactiveDate.value(new Date(inactiveDate));
        }
    }

    function clearDepartmentForm() {
        if (_elements.txtDepartmentCode) {
            _elements.txtDepartmentCode.value('');
        }
        if (_elements.txtDepartmentName) {
            _elements.txtDepartmentName.value('');
        }
        if (_elements.dtpInactiveDate) {
            _elements.dtpInactiveDate.value(null);
        }

        clearValidationErrors();
    }

    function saveDepartmentRecord() {
        if (!validateForm()) {
            return;
        }

        var formSectorCode = _elements.ddlFormSector ? _elements.ddlFormSector.value() : '';
        var formLobCode = _elements.ddlFormLOB ? _elements.ddlFormLOB.value() : '';

        var departmentData = {
            SbaCode: formSectorCode,
            SbuCode: formLobCode,
            DepartmentCode: _elements.txtDepartmentCode ? _elements.txtDepartmentCode.value() : '',
            DepartmentName: _elements.txtDepartmentName ? _elements.txtDepartmentName.value().trim() : '',
            InactiveDate: '',
            CodeType: 'DEPT'
        };

        if (_elements.dtpInactiveDate && _elements.dtpInactiveDate.value()) {
            departmentData.InactiveDate = kendo.toString(_elements.dtpInactiveDate.value(), 'dd/MM/yyyy');
        }

        showLoading();

        ApiConfig.ajax({
            url: '/Maintenance/SaveDepartment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(departmentData),
            success: function (response) {
                hideLoading();

                if (response && response.success) {
                    showNotification(response.message || 'Department record saved successfully', 'success');
                    searchDepartments();
                    showSearchSection();
                } else {
                    showNotification('Save failed: ' + (response.message || 'Unknown error'), 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error saving department record: ' + error, 'error');
            }
        });
    }

    function cancelDepartmentEdit() {
        if (_isEditMode || hasFormChanges()) {
            if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
                clearDepartmentForm();
                showSearchSection();
            }
        } else {
            clearDepartmentForm();
            showSearchSection();
        }
    }

    function hasFormChanges() {
        return !!(_elements.txtDepartmentCode && _elements.txtDepartmentCode.value()) ||
            !!(_elements.txtDepartmentName && _elements.txtDepartmentName.value());
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
        onFormSectorChange: onFormSectorChange,
        searchDepartments: searchDepartments,
        showNewForm: showNewForm,
        editDepartmentRecord: editDepartmentRecord,
        saveDepartmentRecord: saveDepartmentRecord,
        cancelDepartmentEdit: cancelDepartmentEdit
    };

})();

function onSectorChange() {
    departmentMaintenanceViewModel.onSectorChange();
}

function onLOBChange() {
    departmentMaintenanceViewModel.onLOBChange();
}

function onFormSectorChange() {
    departmentMaintenanceViewModel.onFormSectorChange();
}

function searchDepartments() {
    departmentMaintenanceViewModel.searchDepartments();
}

function newDepartmentRecord() {
    departmentMaintenanceViewModel.showNewForm();
}

function editDepartmentRecord(codeType, sbaCode, sbuCode, departmentCode) {
    departmentMaintenanceViewModel.editDepartmentRecord(codeType, sbaCode, sbuCode, departmentCode);
}

function saveDepartmentRecord() {
    departmentMaintenanceViewModel.saveDepartmentRecord();
}

function cancelDepartmentEdit() {
    departmentMaintenanceViewModel.cancelDepartmentEdit();
}