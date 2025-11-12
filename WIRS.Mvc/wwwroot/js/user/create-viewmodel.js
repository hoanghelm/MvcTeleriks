var userCreateViewModel = (function () {

    var _employeeSearchWindow;
    var _userAccessData = [];
    var _currentUserAccessId = 0;
    var _masterData = {
        userRoles: [],
        sectors: [],
        lobs: [],
        departments: [],
        locations: []
    };

    var _elements = {
        txtUserId: null,
        txtUserName: null,
        txtEmail: null,
        ddlUserRole: null,
        userAccessGrid: null
    };

    function init() {
        initializeElements();
        loadMasterData();

        setTimeout(function () {
            initializeEmployeeSearchWindow();
        }, 100);
        initializeUserAccessGrid();
    }

    function initializeElements() {
        _elements.txtUserId = $("#txtUserId").data("kendoTextBox");
        _elements.txtUserName = $("#txtUserName").data("kendoTextBox");
        _elements.txtEmail = $("#txtEmail").data("kendoTextBox");
        _elements.ddlUserRole = $("#ddlUserRole").data("kendoDropDownList");
        _elements.userAccessGrid = $("#userAccessGrid").data("kendoGrid");

        setupRealTimeValidation();
    }

    function setupRealTimeValidation() {

        if (_elements.txtUserName) {
            _elements.txtUserName.bind("change", function () {
                var value = this.value();
                if (value && (value.length < 2 || value.length > 100)) {
                    showValidationError('userNameError', 'User name must be between 2 and 100 characters');
                } else {
                    clearValidationError('userNameError');
                }
            });
        }

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

        if (_elements.txtUserId) {
            _elements.txtUserId.bind("change", function () {
                var value = this.value();
                if (value && (value.length !== 8 || !/^\d{8}$/.test(value))) {
                    showValidationError('userIdError', 'User ID must be exactly 8 digits');
                } else {
                    clearValidationError('userIdError');
                }
            });
        }
    }

    function loadMasterData() {

        TelerikSkeleton.showDropdownSkeleton("#ddlUserRole");

        var requests = [
            ApiConfig.get('/MasterData/GetUserRoles'),
            ApiConfig.get('/MasterData/GetSectors')
        ];

        $.when.apply($, requests).done(function (userRoles, sectors) {
            _masterData.userRoles = userRoles[0] ? userRoles[0] : [];
            _masterData.sectors = sectors[0] ? sectors[0] : [];

            _masterData.lobs = [];
            _masterData.departments = [];
            _masterData.locations = [];

            TelerikSkeleton.hideDropdownSkeleton("#ddlUserRole");
        }).fail(function () {
            TelerikSkeleton.hideDropdownSkeleton("#ddlUserRole");
            TelerikNotification.error('Error loading master data');
        });
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

    function initializeEmployeeSearchWindow() {

        _employeeSearchWindow = $("#employeeSearchWindow").data("kendoWindow");
    }

    function onUserIdChange() {
        var userId = _elements.txtUserId.value();

        if (userId && userId.length === 8 && /^\d+$/.test(userId)) {
            validateUserExists(userId);
        } else {
            clearUserInfo();
        }
    }

    function validateUserExists(userId) {
        TelerikSkeleton.showTextboxSkeleton("#txtUserName");
        TelerikSkeleton.showTextboxSkeleton("#txtEmail");

        ApiConfig.ajax({
            url: '/User/ValidateUserExists',
            type: 'POST',
            data: JSON.stringify(userId),
            contentType: 'application/json',
            success: function (response) {
                TelerikSkeleton.hideTextboxSkeleton("#txtUserName");
                TelerikSkeleton.hideTextboxSkeleton("#txtEmail");

                if (response.success) {
                    _elements.txtUserName.value(response.user.userName);
                    _elements.txtEmail.value(response.user.email);
                    clearValidationError('userIdError');
                    TelerikNotification.success('Employee information loaded successfully');
                } else {
                    showValidationError('userIdError', response.message);
                    clearUserInfo();
                }
            },
            error: function () {
                TelerikSkeleton.hideTextboxSkeleton("#txtUserName");
                TelerikSkeleton.hideTextboxSkeleton("#txtEmail");
                TelerikNotification.error('Error validating user. Please try again.');
            }
        });
    }

    function clearUserInfo() {
        _elements.txtUserName.value('');
        _elements.txtEmail.value('');
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

    function saveUser() {
        if (!validateForm()) {
            return;
        }

        var userData = {
            UserId: _elements.txtUserId.value(),
            UserName: _elements.txtUserName.value(),
            Email: _elements.txtEmail.value(),
            UserRole: _elements.ddlUserRole.value(),
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

        $("#btnCreateUser").data("kendoButton").enable(false);

        ApiConfig.ajax({
            url: '/User/CreateUser',
            type: 'POST',
            data: JSON.stringify(userData),
            contentType: 'application/json',
            success: function (response) {
                $("#btnCreateUser").data("kendoButton").enable(true);

                if (response.success) {
                    TelerikNotification.success("User created successfully!");
                    setTimeout(function () {
                        window.location.href = ApiConfig.buildUrl('/User');
                    }, 2000);
                } else {
                    TelerikNotification.error(response.message);
                }
            },
            error: function () {
                $("#btnCreateUser").data("kendoButton").enable(true);
                TelerikNotification.error('Error creating user. Please try again.');
            }
        });
    }

    function validateForm() {
        var isValid = true;

        $('.validation-error').empty();

        var userId = _elements.txtUserId.value();
        if (!userId) {
            showValidationError('userIdError', 'User ID is required');
            isValid = false;
        } else if (userId.length !== 8) {
            showValidationError('userIdError', 'User ID must be exactly 8 digits');
            isValid = false;
        } else if (!/^\d{8}$/.test(userId)) {
            showValidationError('userIdError', 'User ID must contain only numbers');
            isValid = false;
        }

        var userName = _elements.txtUserName.value();
        if (!userName) {
            showValidationError('userNameError', 'User Name is required');
            isValid = false;
        } else if (userName.length < 2) {
            showValidationError('userNameError', 'User Name must be at least 2 characters');
            isValid = false;
        } else if (userName.length > 100) {
            showValidationError('userNameError', 'User Name cannot exceed 100 characters');
            isValid = false;
        }

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
            window.location.href = ApiConfig.buildUrl('/User');
        }
    }

    function showValidationError(elementId, message) {
        $("#" + elementId).html(`<span class="validation-error">${message}</span>`);
    }

    function clearValidationError(elementId) {
        $("#" + elementId).empty();
    }

    function onGridError(e) {
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
                        setTimeout(function () {
                            lobDropDown.value(dataItem.LOBCode);
                        }, 50);
                    }
                });
            }

            if (dataItem.SectorCode && dataItem.LOBCode) {
                loadDepartmentsByLOB(dataItem.SectorCode, dataItem.LOBCode, function (departments) {
                    var deptDropDown = row.find('[name*="DepartmentValue"]').data('kendoDropDownList');
                    if (deptDropDown) {
                        deptDropDown.setDataSource(departments);
                        setTimeout(function () {
                            deptDropDown.value(dataItem.DepartmentCode);
                        }, 50);
                    }
                });
            }

            if (dataItem.SectorCode && dataItem.LOBCode && dataItem.DepartmentCode) {
                loadLocationsByDepartment(dataItem.SectorCode, dataItem.LOBCode, dataItem.DepartmentCode, function (locations) {
                    var locationDropDown = row.find('[name*="LocationValue"]').data('kendoDropDownList');
                    if (locationDropDown) {
                        locationDropDown.setDataSource(locations);
                        setTimeout(function () {
                            locationDropDown.value(dataItem.LocationCode);
                        }, 50);
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

        ApiConfig.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
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

        ApiConfig.get('/MasterData/GetDepartments', { sectorCode: sectorCode, lobCode: lobCode })
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

        ApiConfig.get('/MasterData/GetLocations', { sectorCode: sectorCode, lobCode: lobCode, deptCode: deptCode })
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
    }

    function onUserRoleChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;

        var row = $(e.sender.element).closest("tr");
        var dataItem = grid.dataItem(row);
        if (!dataItem) return;

        var roleCode = e.sender.value();
        var roleText = e.sender.text();

        dataItem.UserRoleCode = roleCode;
        dataItem.UserRoleName = roleText;
    }

    return {
        init: init,
        onUserIdChange: onUserIdChange,
        addNewAccess: addNewAccess,
        saveUser: saveUser,
        cancelForm: cancelForm,
        onGridError: onGridError,
        loadLOBsBySector: loadLOBsBySector,
        loadDepartmentsByLOB: loadDepartmentsByLOB,
        loadLocationsByDepartment: loadLocationsByDepartment,
        onUserRoleChange: onUserRoleChange,
        onSectorChange: onSectorChange,
        onLOBChange: onLOBChange,
        onDepartmentChange: onDepartmentChange,
        onLocationChange: onLocationChange,
        showNotification: function (message, type) {
            if (type === 'success') {
                TelerikNotification.success(message);
            } else if (type === 'error') {
                TelerikNotification.error(message);
            }
        }
    };

})();

window.onUserIdChange = userCreateViewModel.onUserIdChange;
window.addNewAccess = userCreateViewModel.addNewAccess;
window.saveUser = userCreateViewModel.saveUser;
window.cancelForm = userCreateViewModel.cancelForm;
window.onGridError = userCreateViewModel.onGridError;
window.onUserRoleChange = userCreateViewModel.onUserRoleChange;
window.onSectorChange = userCreateViewModel.onSectorChange;
window.onLOBChange = userCreateViewModel.onLOBChange;
window.onDepartmentChange = userCreateViewModel.onDepartmentChange;
window.onLocationChange = userCreateViewModel.onLocationChange;