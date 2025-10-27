var userUpdateViewModel = (function () {

    var _userAccessData = [];
    var _currentUserAccessId = 0;
    var _masterData = {
        userRoles: [],
        sectors: [],
        lobs: [],
        departments: [],
        locations: [],
        accountStatuses: []
    };

    var _elements = {
        txtUserId: null,
        txtUserName: null,
        txtEmail: null,
        ddlUserRole: null,
        ddlAccountStatus: null,
        dtpInactiveDate: null,
        userAccessGrid: null
    };

    var _userData = null;

    function init(userData) {
        _userData = userData;
        initializeElements();
        loadMasterData();
        setupRealTimeValidation();
        initializeUserAccessGrid();
    }

    function initializeElements() {
        _elements.txtUserId = $("#txtUserId").data("kendoTextBox");
        _elements.txtUserName = $("#txtUserName").data("kendoTextBox");
        _elements.txtEmail = $("#txtEmail").data("kendoTextBox");
        _elements.ddlUserRole = $("#ddlUserRole").data("kendoDropDownList");
        _elements.ddlAccountStatus = $("#ddlAccountStatus").data("kendoDropDownList");
        _elements.dtpInactiveDate = $("#dtpInactiveDate").data("kendoDatePicker");
        _elements.userAccessGrid = $("#userAccessGrid").data("kendoGrid");
    }

    function setupRealTimeValidation() {
        if (_elements.txtEmail) {
            _elements.txtEmail.bind("change", function () {
                var value = this.value();
                var emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/i;
                if (value && !emailRegex.test(value)) {
                    showValidationError('emailError', 'Please enter a valid email address');
                } else {
                    clearValidationError('emailError');
                }
            });
        }
    }

    function loadMasterData() {
        TelerikSkeleton.showDropdownSkeleton("#ddlUserRole");
        TelerikSkeleton.showDropdownSkeleton("#ddlAccountStatus");

        var requests = [
            $.get('/MasterData/GetUserRoles'),
            $.get('/MasterData/GetSectors'),
            $.get('/MasterData/GetAccountStatuses')
        ];

        $.when.apply($, requests).done(function (userRoles, sectors, accountStatuses) {
            _masterData.userRoles = userRoles[0] ? userRoles[0] : [];
            _masterData.sectors = sectors[0] ? sectors[0] : [];
            _masterData.accountStatuses = accountStatuses[0] ? accountStatuses[0] : [];

            _masterData.lobs = [];
            _masterData.departments = [];
            _masterData.locations = [];

            if (_elements.ddlUserRole) {
                _elements.ddlUserRole.setDataSource(_masterData.userRoles);
                if (_userData && _userData.userRole) {
                    _elements.ddlUserRole.value(_userData.userRole);
                }
            }

            if (_elements.ddlAccountStatus) {
                _elements.ddlAccountStatus.setDataSource(_masterData.accountStatuses);
                if (_userData && _userData.accountStatus) {
                    _elements.ddlAccountStatus.value(_userData.accountStatus);
                }
            }

            TelerikSkeleton.hideDropdownSkeleton("#ddlUserRole");
            TelerikSkeleton.hideDropdownSkeleton("#ddlAccountStatus");

            if (_userData && _userData.userAccess && _userData.userAccess.length > 0) {
                loadExistingUserAccess(_userData.userAccess);
            }
        }).fail(function () {
            TelerikSkeleton.hideDropdownSkeleton("#ddlUserRole");
            TelerikSkeleton.hideDropdownSkeleton("#ddlAccountStatus");
            TelerikNotification.error('Error loading master data');
        });
    }

    function loadExistingUserAccess(userAccessList) {
        _userAccessData = [];
        _currentUserAccessId = 0;

        if (userAccessList) {
            userAccessList.forEach(function (access) {
                _userAccessData.push({
                    Id: ++_currentUserAccessId,
                    UserRoleCode: access.userRoleCode || '',
                    UserRoleName: access.userRoleName || '',
                    SectorCode: access.sectorCode || '',
                    SectorValue: access.sectorValue || '',
                    LOBCode: access.lobCode || '',
                    LOBValue: access.lobValue || '',
                    DepartmentCode: access.departmentCode || '',
                    DepartmentValue: access.departmentValue || '',
                    LocationCode: access.locationCode || '',
                    LocationValue: access.locationValue || ''
                });
            });
        }

        refreshUserAccessGrid();
    }

    function initializeUserAccessGrid() {
        if (_elements.userAccessGrid) {
            _elements.userAccessGrid.setDataSource(new kendo.data.DataSource({
                data: [],
                schema: {
                    model: {
                        id: "Id",
                        fields: {
                            Id: { type: "number", editable: false },
                            UserRoleCode: { type: "string" },
                            UserRoleName: { type: "string" },
                            SectorCode: { type: "string" },
                            SectorValue: { type: "string" },
                            LOBCode: { type: "string" },
                            LOBValue: { type: "string" },
                            DepartmentCode: { type: "string" },
                            DepartmentValue: { type: "string" },
                            LocationCode: { type: "string" },
                            LocationValue: { type: "string" }
                        }
                    }
                }
            }));

            _elements.userAccessGrid.bind("edit", onGridEdit);
            _elements.userAccessGrid.bind("remove", onGridRemove);
        }
    }

    function addNewAccess() {
        if (!_elements.userAccessGrid) return;

        var newAccess = {
            Id: ++_currentUserAccessId,
            UserRoleCode: '',
            UserRoleName: '--Select--',
            SectorCode: '',
            SectorValue: '--Select--',
            LOBCode: '',
            LOBValue: '--Select--',
            DepartmentCode: '',
            DepartmentValue: '--Select--',
            LocationCode: '',
            LocationValue: '--Select--'
        };

        _userAccessData.push(newAccess);
        refreshUserAccessGrid();

        setTimeout(function () {
            var grid = _elements.userAccessGrid;
            var lastRow = grid.tbody.find("tr:last");
            grid.editRow(lastRow);
        }, 100);
    }

    function refreshUserAccessGrid() {
        if (!_elements.userAccessGrid) return;

        var dataSource = new kendo.data.DataSource({
            data: _userAccessData,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { type: "number", editable: false },
                        UserRoleCode: { type: "string" },
                        UserRoleName: { type: "string" },
                        SectorCode: { type: "string" },
                        SectorValue: { type: "string" },
                        LOBCode: { type: "string" },
                        LOBValue: { type: "string" },
                        DepartmentCode: { type: "string" },
                        DepartmentValue: { type: "string" },
                        LocationCode: { type: "string" },
                        LocationValue: { type: "string" }
                    }
                }
            }
        });

        _elements.userAccessGrid.setDataSource(dataSource);
    }

    function onGridRemove(e) {
        var dataItem = e.model;
        var index = _userAccessData.findIndex(function (item) {
            return item.Id === dataItem.Id;
        });

        if (index !== -1) {
            _userAccessData.splice(index, 1);
        }
    }

    function updateUser() {
        if (!validateForm()) {
            return;
        }

        var userData = {
            UserId: $("#hiddenUserId").val(),
            Email: _elements.txtEmail.value(),
            UserRole: _elements.ddlUserRole.value(),
            AccountStatus: _elements.ddlAccountStatus.value(),
            InactiveDate: _elements.dtpInactiveDate && _elements.dtpInactiveDate.value() ?
                kendo.toString(_elements.dtpInactiveDate.value(), "yyyy-MM-dd") : null,
            UserAccess: _userAccessData.map(function (item) {
                return {
                    UserRoleCode: item.UserRoleCode || _elements.ddlUserRole.value(),
                    SectorCode: item.SectorCode,
                    LobCode: item.LOBCode,
                    DepartmentCode: item.DepartmentCode,
                    LocationCode: item.LocationCode
                };
            })
        };

        $("#btnUpdateUser").data("kendoButton").enable(false);

        $.ajax({
            url: '/User/UpdateUser',
            type: 'POST',
            data: JSON.stringify(userData),
            contentType: 'application/json',
            success: function (response) {
                $("#btnUpdateUser").data("kendoButton").enable(true);

                if (response.success) {
                    TelerikNotification.success("User updated successfully!");
                    setTimeout(function () {
                        window.location.href = '/User';
                    }, 2000);
                } else {
                    TelerikNotification.error(response.message);
                }
            },
            error: function () {
                $("#btnUpdateUser").data("kendoButton").enable(true);
                TelerikNotification.error('Error updating user. Please try again.');
            }
        });
    }

    function validateForm() {
        var isValid = true;

        $('.validation-error').empty();

        var email = _elements.txtEmail.value();
        var emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/i;
        if (!email) {
            showValidationError('emailError', 'Email Address is required');
            isValid = false;
        } else if (!emailRegex.test(email)) {
            showValidationError('emailError', 'Please enter a valid email address (e.g., user@company.com)');
            isValid = false;
        }

        if (!_elements.ddlUserRole.value()) {
            showValidationError('userRoleError', 'User Role is required');
            isValid = false;
        }

        if (_userAccessData.length === 0) {
            showValidationError('accessError', 'At least one user access permission is required');
            isValid = false;
        } else {
            for (var i = 0; i < _userAccessData.length; i++) {
                var access = _userAccessData[i];
                if (!access.UserRoleCode || !access.SectorCode || !access.LOBCode) {
                    showValidationError('accessError', 'User Role, Sector and LOB are required for all access rows');
                    isValid = false;
                    break;
                }
            }
        }

        return isValid;
    }

    function cancelForm() {
        if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
            window.location.href = '/User';
        }
    }

    function showValidationError(elementId, message) {
        $("#" + elementId).html(`<span class="validation-error">${message}</span>`);
    }

    function clearValidationError(elementId) {
        $("#" + elementId).empty();
    }

    function onGridError(e) {
        console.error('Grid error:', e);
    }

    function editRow(e) {
        e.preventDefault();
        var grid = $("#userAccessGrid").data("kendoGrid");
        var tr = $(e.currentTarget).closest("tr");
        grid.editRow(tr);
    }

    function deleteRow(e) {
        e.preventDefault();
        var grid = $("#userAccessGrid").data("kendoGrid");
        var tr = $(e.currentTarget).closest("tr");
        var dataItem = grid.dataItem(tr);

        if (confirm("Are you sure you want to delete this access?")) {
            var index = _userAccessData.findIndex(function (item) {
                return item.Id === dataItem.Id;
            });

            if (index !== -1) {
                _userAccessData.splice(index, 1);
                refreshUserAccessGrid();
            }
        }
    }

    function onGridEdit(e) {
        var dataItem = e.model;
        var row = $(e.container).closest("tr");

        var userRoleDropDown = row.find('[name*="UserRoleName"]').data('kendoDropDownList');
        if (userRoleDropDown) {
            userRoleDropDown.setDataSource(_masterData.userRoles);
            if (!e.model.isNew() && dataItem.UserRoleCode) {
                setTimeout(function () {
                    userRoleDropDown.value(dataItem.UserRoleCode);
                }, 50);
            }
        }

        var sectorDropDown = row.find('[name*="SectorValue"]').data('kendoDropDownList');
        if (sectorDropDown) {
            sectorDropDown.setDataSource(_masterData.sectors);
            if (!e.model.isNew() && dataItem.SectorCode) {
                setTimeout(function () {
                    sectorDropDown.value(dataItem.SectorCode);
                }, 50);
            }
        }

        if (e.model.isNew()) {
            dataItem.set('UserRoleName', '--Select--');
            dataItem.set('SectorValue', '--Select--');
            dataItem.set('LOBValue', '--Select--');
            dataItem.set('DepartmentValue', '--Select--');
            dataItem.set('LocationValue', '--Select--');
        } else {
            if (dataItem.SectorCode) {
                loadLOBsBySector(dataItem.SectorCode, function (lobs) {
                    var lobDropDown = row.find('[name*="LOBValue"]').data('kendoDropDownList');
                    if (lobDropDown) {
                        lobDropDown.setDataSource(lobs);
                        if (dataItem.LOBCode) {
                            setTimeout(function () {
                                lobDropDown.value(dataItem.LOBCode);
                            }, 50);
                        }
                    }
                });
            }

            if (dataItem.SectorCode && dataItem.LOBCode) {
                loadDepartmentsByLOB(dataItem.SectorCode, dataItem.LOBCode, function (departments) {
                    var deptDropDown = row.find('[name*="DepartmentValue"]').data('kendoDropDownList');
                    if (deptDropDown) {
                        deptDropDown.setDataSource(departments);
                        if (dataItem.DepartmentCode) {
                            setTimeout(function () {
                                deptDropDown.value(dataItem.DepartmentCode);
                            }, 100);
                        }
                    }
                });
            }

            if (dataItem.SectorCode && dataItem.LOBCode && dataItem.DepartmentCode) {
                loadLocationsByDepartment(dataItem.SectorCode, dataItem.LOBCode, dataItem.DepartmentCode, function (locations) {
                    var locationDropDown = row.find('[name*="LocationValue"]').data('kendoDropDownList');
                    if (locationDropDown) {
                        locationDropDown.setDataSource(locations);
                        if (dataItem.LocationCode) {
                            setTimeout(function () {
                                locationDropDown.value(dataItem.LocationCode);
                            }, 150);
                        }
                    }
                });
            }
        }
    }

    function loadLOBsBySector(sectorCode, callback) {
        if (!sectorCode) {
            _masterData.lobs = [];
            callback && callback([]);
            return;
        }

        $.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function (response) {
                _masterData.lobs = response;
                callback && callback(response);
            })
            .fail(function () {
                _masterData.lobs = [];
                callback && callback([]);
            });
    }

    function loadDepartmentsByLOB(sectorCode, lobCode, callback) {
        if (!sectorCode || !lobCode) {
            _masterData.departments = [];
            callback && callback([]);
            return;
        }

        $.get('/MasterData/GetDepartments', { sectorCode: sectorCode, lobCode: lobCode })
            .done(function (response) {
                _masterData.departments = response;
                callback && callback(response);
            })
            .fail(function () {
                _masterData.departments = [];
                callback && callback([]);
            });
    }

    function loadLocationsByDepartment(sectorCode, lobCode, deptCode, callback) {
        if (!sectorCode || !lobCode || !deptCode) {
            _masterData.locations = [];
            callback && callback([]);
            return;
        }

        $.get('/MasterData/GetLocations', { sectorCode: sectorCode, lobCode: lobCode, deptCode: deptCode })
            .done(function (response) {
                _masterData.locations = response;
                callback && callback(response);
            })
            .fail(function () {
                _masterData.locations = [];
                callback && callback([]);
            });
    }

    function updateDataItemInGrid(dataItem) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;

        var index = _userAccessData.findIndex(function (item) {
            return item.Id === dataItem.Id;
        });

        if (index !== -1) {
            _userAccessData[index] = {
                Id: dataItem.Id,
                UserRoleCode: dataItem.UserRoleCode,
                UserRoleName: dataItem.UserRoleName,
                SectorCode: dataItem.SectorCode,
                SectorValue: dataItem.SectorValue,
                LOBCode: dataItem.LOBCode,
                LOBValue: dataItem.LOBValue,
                DepartmentCode: dataItem.DepartmentCode,
                DepartmentValue: dataItem.DepartmentValue,
                LocationCode: dataItem.LocationCode,
                LocationValue: dataItem.LocationValue
            };
        }
    }

    function onUserRoleChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;

        var dataItem = grid.dataItem($(e.sender.element).closest("tr"));
        if (!dataItem) return;

        var roleCode = e.sender.value();
        var roleText = e.sender.text();

        dataItem.set('UserRoleCode', roleCode);
        dataItem.set('UserRoleName', roleText);

        updateDataItemInGrid(dataItem);
    }

    function onSectorChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;

        var row = $(e.sender.element).closest("tr");
        var dataItem = grid.dataItem(row);
        if (!dataItem) return;

        var sectorCode = e.sender.value();
        var sectorText = e.sender.text();

        dataItem.SectorCode = sectorCode;
        dataItem.SectorValue = sectorText;

        dataItem.LOBCode = '';
        dataItem.LOBValue = '--Select--';
        dataItem.DepartmentCode = '';
        dataItem.DepartmentValue = '--Select--';
        dataItem.LocationCode = '';
        dataItem.LocationValue = '--Select--';

        var lobDropDown = row.find('[name*="LOBValue"]').data('kendoDropDownList');
        var deptDropDown = row.find('[name*="DepartmentValue"]').data('kendoDropDownList');
        var locationDropDown = row.find('[name*="LocationValue"]').data('kendoDropDownList');

        if (lobDropDown) {
            lobDropDown.setDataSource([]);
            lobDropDown.value('');
        }
        if (deptDropDown) {
            deptDropDown.setDataSource([]);
            deptDropDown.value('');
        }
        if (locationDropDown) {
            locationDropDown.setDataSource([]);
            locationDropDown.value('');
        }

        if (sectorCode) {
            loadLOBsBySector(sectorCode, function (lobs) {
                if (lobDropDown) {
                    lobDropDown.setDataSource(lobs);
                }
            });
        }

        updateDataItemInGrid(dataItem);
    }

    function onLOBChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;

        var row = $(e.sender.element).closest("tr");
        var dataItem = grid.dataItem(row);
        if (!dataItem) return;

        var lobCode = e.sender.value();
        var lobText = e.sender.text();
        var sectorCode = dataItem.SectorCode;

        dataItem.LOBCode = lobCode;
        dataItem.LOBValue = lobText;

        dataItem.DepartmentCode = '';
        dataItem.DepartmentValue = '--Select--';
        dataItem.LocationCode = '';
        dataItem.LocationValue = '--Select--';

        var deptDropDown = row.find('[name*="DepartmentValue"]').data('kendoDropDownList');
        var locationDropDown = row.find('[name*="LocationValue"]').data('kendoDropDownList');

        if (deptDropDown) {
            deptDropDown.setDataSource([]);
            deptDropDown.value('');
        }
        if (locationDropDown) {
            locationDropDown.setDataSource([]);
            locationDropDown.value('');
        }

        if (sectorCode && lobCode) {
            loadDepartmentsByLOB(sectorCode, lobCode, function (departments) {
                if (deptDropDown) {
                    deptDropDown.setDataSource(departments);
                }
            });
        }

        updateDataItemInGrid(dataItem);
    }

    function onDepartmentChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;

        var row = $(e.sender.element).closest("tr");
        var dataItem = grid.dataItem(row);
        if (!dataItem) return;

        var deptCode = e.sender.value();
        var deptText = e.sender.text();
        var sectorCode = dataItem.SectorCode;
        var lobCode = dataItem.LOBCode;

        dataItem.DepartmentCode = deptCode;
        dataItem.DepartmentValue = deptText;

        dataItem.LocationCode = '';
        dataItem.LocationValue = '--Select--';

        var locationDropDown = row.find('[name*="LocationValue"]').data('kendoDropDownList');

        if (locationDropDown) {
            locationDropDown.setDataSource([]);
            locationDropDown.value('');
        }

        if (sectorCode && lobCode && deptCode) {
            loadLocationsByDepartment(sectorCode, lobCode, deptCode, function (locations) {
                if (locationDropDown) {
                    locationDropDown.setDataSource(locations);
                }
            });
        }

        updateDataItemInGrid(dataItem);
    }

    function onLocationChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;

        var row = $(e.sender.element).closest("tr");
        var dataItem = grid.dataItem(row);
        if (!dataItem) return;

        var locationCode = e.sender.value();
        var locationText = e.sender.text();

        dataItem.LocationCode = locationCode;
        dataItem.LocationValue = locationText;

        updateDataItemInGrid(dataItem);
    }

    return {
        init: init,
        addNewAccess: addNewAccess,
        updateUser: updateUser,
        cancelForm: cancelForm,
        onGridError: onGridError,
        editRow: editRow,
        deleteRow: deleteRow,
        loadLOBsBySector: loadLOBsBySector,
        loadDepartmentsByLOB: loadDepartmentsByLOB,
        loadLocationsByDepartment: loadLocationsByDepartment,
        onUserRoleChange: onUserRoleChange,
        onSectorChange: onSectorChange,
        onLOBChange: onLOBChange,
        onDepartmentChange: onDepartmentChange,
        onLocationChange: onLocationChange
    };

})();

window.addNewAccess = userUpdateViewModel.addNewAccess;
window.updateUser = userUpdateViewModel.updateUser;
window.cancelForm = userUpdateViewModel.cancelForm;
window.onGridError = userUpdateViewModel.onGridError;
window.editRow = userUpdateViewModel.editRow;
window.deleteRow = userUpdateViewModel.deleteRow;
window.onUserRoleChange = userUpdateViewModel.onUserRoleChange;
window.onSectorChange = userUpdateViewModel.onSectorChange;
window.onLOBChange = userUpdateViewModel.onLOBChange;
window.onDepartmentChange = userUpdateViewModel.onDepartmentChange;
window.onLocationChange = userUpdateViewModel.onLocationChange;