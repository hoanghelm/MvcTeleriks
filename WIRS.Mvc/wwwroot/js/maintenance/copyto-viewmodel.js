var maintenanceCopyToViewModel = (function () {

    var _currentMode = 'search';
    var _isEditMode = false;

    var _elements = {
        ddlSearchSector: null,
        ddlSearchLOB: null,
        ddlSearchDepartment: null,
        ddlSearchLocation: null,
        copyToGrid: null,
        ddlSector: null,
        ddlLOB: null,
        ddlDepartment: null,
        ddlLocation: null,
        txtUserId: null,
        txtUserName: null,
        dtpInactiveDate: null,
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
        _elements.ddlSearchSector = $("#ddlSearchSector").data("kendoDropDownList");
        _elements.ddlSearchLOB = $("#ddlSearchLOB").data("kendoDropDownList");
        _elements.ddlSearchDepartment = $("#ddlSearchDepartment").data("kendoDropDownList");
        _elements.ddlSearchLocation = $("#ddlSearchLocation").data("kendoDropDownList");
        _elements.copyToGrid = $("#copyToGrid").data("kendoGrid");
        _elements.ddlSector = $("#ddlSector").data("kendoDropDownList");
        _elements.ddlLOB = $("#ddlLOB").data("kendoDropDownList");
        _elements.ddlDepartment = $("#ddlDepartment").data("kendoDropDownList");
        _elements.ddlLocation = $("#ddlLocation").data("kendoDropDownList");
        _elements.txtUserId = $("#txtUserId").data("kendoTextBox");
        _elements.txtUserName = $("#txtUserName").data("kendoTextBox");
        _elements.dtpInactiveDate = $("#dtpInactiveDate").data("kendoDatePicker");
        _elements.searchSection = $("#searchSection");
        _elements.entryFormSection = $("#entryFormSection");
        _elements.formTitle = $("#formTitle");
    }

    function loadMasterData() {
        loadSectors();
    }

    function loadSectors() {
        TelerikSkeleton.showDropdownSkeleton("#ddlSector");

        $.get('/MasterData/GetSectors')
            .done(function (response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlSector");
                if (response.success && _elements.ddlSector) {
                    _elements.ddlSector.setDataSource(response.data);
                }
            })
            .fail(function () {
                TelerikSkeleton.hideDropdownSkeleton("#ddlSector");
                TelerikNotification.error('Error loading sectors');
            });
    }

    function onSearchSectorChange() {
        if (!_elements.ddlSearchSector || !_elements.ddlSearchLOB) return;

        var sectorCode = _elements.ddlSearchSector.value();

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

        _elements.ddlSearchLocation.setDataSource([]);
        _elements.ddlSearchLocation.value('');

        if (sectorCode && lobCode && departmentCode) {
            loadLocationsBySearchDepartment(sectorCode, lobCode, departmentCode);
        }
    }

    function loadLOBsBySearchSector(sectorCode) {
        $.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function (response) {
                if (_elements.ddlSearchLOB) {
                    _elements.ddlSearchLOB.setDataSource(response);
                }
            })
            .fail(function () {
                TelerikNotification.error('Error loading LOBs');
            });
    }

    function loadDepartmentsBySearchSectorLOB(sectorCode, lobCode) {
        $.get('/MasterData/GetDepartments', { sectorCode: sectorCode, lobCode: lobCode })
            .done(function (response) {
                if (_elements.ddlSearchDepartment) {
                    _elements.ddlSearchDepartment.setDataSource(response);
                }
            })
            .fail(function () {
                TelerikNotification.error('Error loading departments');
            });
    }

    function loadLocationsBySearchDepartment(sectorCode, lobCode, departmentCode) {
        $.get('/MasterData/GetLocations', { sectorCode: sectorCode, lobCode: lobCode, deptCode: departmentCode })
            .done(function (response) {
                if (_elements.ddlSearchLocation) {
                    _elements.ddlSearchLocation.setDataSource(response);
                }
            })
            .fail(function () {
                TelerikNotification.error('Error loading locations');
            });
    }

    function onSectorChange() {
        if (!_elements.ddlSector || !_elements.ddlLOB) return;

        var sectorCode = _elements.ddlSector.value();

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

        _elements.ddlLocation.setDataSource([]);
        _elements.ddlLocation.value('');

        if (sectorCode && lobCode && departmentCode) {
            loadLocationsByDepartment(sectorCode, lobCode, departmentCode);
        }
    }

    function loadLOBsBySector(sectorCode, callback) {
        TelerikSkeleton.showDropdownSkeleton("#ddlLOB");

        $.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function (response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLOB");
                if (_elements.ddlLOB) {
                    _elements.ddlLOB.setDataSource(response);
                    if (callback && typeof callback === 'function') {
                        callback();
                    }
                }
            })
            .fail(function () {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLOB");
                TelerikNotification.error('Error loading LOBs');
            });
    }

    function loadDepartmentsBySectorLOB(sectorCode, lobCode, callback) {
        TelerikSkeleton.showDropdownSkeleton("#ddlDepartment");

        $.get('/MasterData/GetDepartments', { sectorCode: sectorCode, lobCode: lobCode })
            .done(function (response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlDepartment");
                if (_elements.ddlDepartment) {
                    _elements.ddlDepartment.setDataSource(response);
                    if (callback && typeof callback === 'function') {
                        callback();
                    }
                }
            })
            .fail(function () {
                TelerikSkeleton.hideDropdownSkeleton("#ddlDepartment");
                TelerikNotification.error('Error loading departments');
            });
    }

    function loadLocationsByDepartment(sectorCode, lobCode, departmentCode, callback) {
        TelerikSkeleton.showDropdownSkeleton("#ddlLocation");

        $.get('/MasterData/GetLocations', { sectorCode: sectorCode, lobCode: lobCode, deptCode: departmentCode })
            .done(function (response) {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLocation");
                if (_elements.ddlLocation) {
                    _elements.ddlLocation.setDataSource(response);
                    if (callback && typeof callback === 'function') {
                        callback();
                    }
                }
            })
            .fail(function () {
                TelerikSkeleton.hideDropdownSkeleton("#ddlLocation");
                TelerikNotification.error('Error loading locations');
            });
    }

    function showLoading() {
        kendo.ui.progress($("#copyToGrid"), true);
    }

    function hideLoading() {
        kendo.ui.progress($("#copyToGrid"), false);
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

        showLoading();

        $.ajax({
            url: '/Maintenance/GetCopyToList',
            type: 'POST',
            data: searchCriteria,
            success: function (response) {
                hideLoading();

                if (response.success) {
                    var dataSource = new kendo.data.DataSource({
                        data: response.data || [],
                        pageSize: 20,
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
                                    locationCode: { type: "string" },
                                    locationName: { type: "string" },
                                    userId: { type: "string" },
                                    userName: { type: "string" },
                                    inactiveDate: { type: "date" },
                                    uid: { type: "string" }
                                }
                            },
                            parse: function (data) {
                                var parsedData = [];
                                if (data && Array.isArray(data)) {
                                    parsedData = data.map(function (item) {
                                        return {
                                            sbaCode: item.sbaCode || '',
                                            sbaName: item.sbaName || '',
                                            sbuCode: item.sbuCode || '',
                                            sbuName: item.sbuName || '',
                                            departmentCode: item.departmentCode || '',
                                            departmentName: item.departmentName || '',
                                            locationCode: item.locationCode || '',
                                            locationName: item.locationName || '',
                                            userId: item.userId || '',
                                            userName: item.userName || '',
                                            inactiveDate: item.inactiveDate ? new Date(item.inactiveDate) : null,
                                            uid: item.uid || ''
                                        };
                                    });
                                }
                                return parsedData;
                            }
                        }
                    });

                    _elements.copyToGrid.setDataSource(dataSource);

                    var totalCount = response.totalCount || 0;
                    var hasSearchCriteria = !!(searchCriteria.sbaCode || searchCriteria.sbuCode ||
                        searchCriteria.departmentCode || searchCriteria.locationCode);

                    if (hasSearchCriteria) {
                        if (totalCount === 0) {
                            TelerikNotification.warning('No copy-to entries found matching your search criteria.');
                        } else if (totalCount === 1) {
                            TelerikNotification.success('Found 1 copy-to entry matching your search.');
                        }
                    }
                } else {
                    TelerikNotification.error(response.message || 'Error searching copy-to list');
                    _elements.copyToGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
                }
            },
            error: function () {
                hideLoading();
                TelerikNotification.error('Error searching copy-to list. Please try again.');
                _elements.copyToGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
            }
        });
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

        populateForm(dataItem);

        showFormSection();
    }

    function populateForm(data) {
        if (!data) return;

        $("#hiddenUid").val(data.uid || data.Uid);

        var sbaCode = data.sbaCode || data.SbaCode;
        var sbuCode = data.sbuCode || data.SbuCode;
        var departmentCode = data.departmentCode || data.DepartmentCode;
        var locationCode = data.locationCode || data.LocationCode;

        if (_elements.ddlSector && sbaCode) {
            _elements.ddlSector.value(sbaCode);

            if (sbuCode) {
                loadLOBsBySector(sbaCode, function () {
                    if (_elements.ddlLOB) {
                        setTimeout(function () {
                            _elements.ddlLOB.value(sbuCode);

                            if (departmentCode) {
                                loadDepartmentsBySectorLOB(sbaCode, sbuCode, function () {
                                    if (_elements.ddlDepartment) {
                                        setTimeout(function () {
                                            _elements.ddlDepartment.value(departmentCode);

                                            if (locationCode) {
                                                loadLocationsByDepartment(sbaCode, sbuCode, departmentCode, function () {
                                                    if (_elements.ddlLocation) {
                                                        setTimeout(function () {
                                                            _elements.ddlLocation.value(locationCode);
                                                        }, 100);
                                                    }
                                                });
                                            }
                                        }, 100);
                                    }
                                });
                            }
                        }, 100);
                    }
                });
            }
        }

        if (_elements.txtUserId) _elements.txtUserId.value(data.userId || data.UserId || '');
        if (_elements.txtUserName) _elements.txtUserName.value(data.userName || data.UserName || '');

        var inactiveDate = data.inactiveDate || data.InactiveDate;
        if (_elements.dtpInactiveDate && inactiveDate) {
            _elements.dtpInactiveDate.value(new Date(inactiveDate));
        }
    }

    function clearForm() {
        $("#hiddenUid").val('');

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

        if (_elements.txtUserId) _elements.txtUserId.value('');
        if (_elements.txtUserName) _elements.txtUserName.value('');
        if (_elements.dtpInactiveDate) _elements.dtpInactiveDate.value(null);

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

        $("#btnSave").data("kendoButton").enable(false);

        $.ajax({
            url: '/Maintenance/SaveCopyToList',
            type: 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            success: function (response) {
                $("#btnSave").data("kendoButton").enable(true);

                if (response.success) {
                    TelerikNotification.success("Copy-to entry saved successfully!");

                    searchCopyToList();
                    showSearchSection();
                } else {
                    TelerikNotification.error(response.message || 'Error saving copy-to entry');
                }
            },
            error: function () {
                $("#btnSave").data("kendoButton").enable(true);
                TelerikNotification.error('Error saving copy-to entry. Please try again.');
            }
        });
    }

    function validateForm() {
        var isValid = true;

        $('.validation-error').empty();

        if (!_elements.ddlSector || !_elements.ddlSector.value()) {
            showValidationError('sectorError', 'Sector is required');
            isValid = false;
        }

        if (!_elements.ddlLOB || !_elements.ddlLOB.value()) {
            showValidationError('lobError', 'LOB is required');
            isValid = false;
        }

        if (!_elements.ddlDepartment || !_elements.ddlDepartment.value()) {
            showValidationError('departmentError', 'Department is required');
            isValid = false;
        }

        var userId = _elements.txtUserId ? _elements.txtUserId.value() : '';
        if (!userId) {
            showValidationError('userIdError', 'User ID is required');
            isValid = false;
        } else if (userId.length !== 8 || !/^\d{8}$/.test(userId)) {
            showValidationError('userIdError', 'User ID must be exactly 8 digits');
            isValid = false;
        }

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
        $("#" + elementId).html('<span class="validation-error">' + message + '</span>');
    }

    function cancelForm() {
        if (_isEditMode || hasFormChanges()) {
            if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
                clearForm();
                showSearchSection();
            }
        } else {
            clearForm();
            showSearchSection();
        }
    }

    function hasFormChanges() {
        return !!(_elements.ddlSector && _elements.ddlSector.value()) ||
            !!(_elements.txtUserId && _elements.txtUserId.value()) ||
            !!(_elements.txtUserName && _elements.txtUserName.value());
    }

    function showSearchSection() {
        _currentMode = 'search';
        _isEditMode = false;
        _elements.entryFormSection.hide();
        _elements.searchSection.show();
        $("#resultsSection").show();
    }

    function showFormSection() {
        _currentMode = 'form';
        _elements.searchSection.hide();
        $("#resultsSection").hide();
        _elements.entryFormSection.show();
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