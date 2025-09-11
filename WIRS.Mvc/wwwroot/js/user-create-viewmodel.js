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
        // Wait for DOM to be ready before initializing the window
        setTimeout(function() {
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
        
        // Add real-time validation
        setupRealTimeValidation();
    }
    
    function setupRealTimeValidation() {
        // User Name validation
        if (_elements.txtUserName) {
            _elements.txtUserName.bind("change", function() {
                var value = this.value();
                if (value && (value.length < 2 || value.length > 100)) {
                    showValidationError('userNameError', 'User name must be between 2 and 100 characters');
                } else {
                    clearValidationError('userNameError');
                }
            });
        }
        
        // Email validation
        if (_elements.txtEmail) {
            _elements.txtEmail.bind("change", function() {
                var value = this.value();
                var emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/i;
                if (value && !emailRegex.test(value)) {
                    showValidationError('emailError', 'Please enter a valid email address');
                } else {
                    clearValidationError('emailError');
                }
            });
        }
        
        // User ID validation
        if (_elements.txtUserId) {
            _elements.txtUserId.bind("change", function() {
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
        // Show skeleton loading only for dropdowns that need data
        TelerikSkeleton.showDropdownSkeleton("#ddlUserRole");
        
        // Load all master data in parallel
        var requests = [
            $.get('/MasterData/GetUserRoles'),
            $.get('/MasterData/GetSectors'),
            $.get('/MasterData/GetLocations')
        ];
        
        $.when.apply($, requests).done(function(userRoles, sectors, locations) {
            _masterData.userRoles = userRoles[0].success ? userRoles[0].data : [];
            _masterData.sectors = sectors[0].success ? sectors[0].data : [];
            _masterData.locations = locations[0].success ? locations[0].data : [];
            
            // Cache empty arrays for LOBs and Departments - they'll be loaded on demand
            _masterData.lobs = [];
            _masterData.departments = [];
            
            // Hide skeleton loading for dropdowns
            TelerikSkeleton.hideDropdownSkeleton("#ddlUserRole");
        }).fail(function() {
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
            
            // Bind to edit events for cascading dropdowns
            _elements.userAccessGrid.bind("edit", onGridEdit);
        }
    }
    
    function initializeEmployeeSearchWindow() {
        // Employee search is now handled by the EmployeeSearchComponent
        _employeeSearchWindow = $("#employeeSearchWindow").data("kendoWindow");
    }
    
    
    // Removed old showNotification - now using TelerikNotification
    
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
        
        $.ajax({
            url: '/User/ValidateUserExists',
            type: 'POST',
            data: JSON.stringify(userId),
            contentType: 'application/json',
            success: function(response) {
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
            error: function() {
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
    
    // Employee search functions are now handled by EmployeeSearchComponent
    
    function addNewAccess() {
        if (!_elements.userAccessGrid) return;
        
        var newAccess = {
            Id: ++_currentUserAccessId,
            UserRoleCode: '',
            UserRoleName: 'Select Role',
            SectorCode: '',
            SectorValue: 'Select Sector',
            LOBCode: '',
            LOBValue: 'Select LOB',
            DepartmentCode: '',
            DepartmentValue: 'Select Department',
            LocationCode: '',
            LocationValue: 'Select Location'
        };
        
        _userAccessData.push(newAccess);
        refreshUserAccessGrid();
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
    
    function saveUser() {
        if (!validateForm()) {
            return;
        }
        
        var userData = {
            UserId: _elements.txtUserId.value(),
            UserName: _elements.txtUserName.value(),
            Email: _elements.txtEmail.value(),
            UserRole: _elements.ddlUserRole.value(),
            UserAccess: _userAccessData.map(function(item) {
                return {
                    UserRoleCode: item.UserRoleCode || _elements.ddlUserRole.value(),
                    SectorCode: item.SectorCode,
                    LobCode: item.LOBCode,
                    DepartmentCode: item.DepartmentCode,
                    LocationCode: item.LocationCode
                };
            })
        };
        
        // Disable the save button during submission
        $("#btnCreateUser").data("kendoButton").enable(false);
        
        $.ajax({
            url: '/User/CreateUser',
            type: 'POST',
            data: JSON.stringify(userData),
            contentType: 'application/json',
            success: function(response) {
                $("#btnCreateUser").data("kendoButton").enable(true);
                
                if (response.success) {
                    TelerikNotification.success("User created successfully!");
                    setTimeout(function() {
                        window.location.href = '/Home';
                    }, 2000);
                } else {
                    TelerikNotification.error(response.message);
                }
            },
            error: function() {
                $("#btnCreateUser").data("kendoButton").enable(true);
                TelerikNotification.error('Error creating user. Please try again.');
            }
        });
    }
    
    function validateForm() {
        var isValid = true;
        
        // Clear previous errors
        $('.validation-error').empty();
        
        // Validate User ID
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
        
        // Validate User Name
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
        
        // Validate Email
        var email = _elements.txtEmail.value();
        var emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/i;
        if (!email) {
            showValidationError('emailError', 'Email Address is required');
            isValid = false;
        } else if (!emailRegex.test(email)) {
            showValidationError('emailError', 'Please enter a valid email address (e.g., user@company.com)');
            isValid = false;
        }
        
        // Validate User Role
        if (!_elements.ddlUserRole.value()) {
            showValidationError('userRoleError', 'User Role is required');
            isValid = false;
        }
        
        // Validate User Access
        if (_userAccessData.length === 0) {
            showValidationError('accessError', 'At least one user access permission is required');
            isValid = false;
        } else {
            // Validate each access entry
            for (var i = 0; i < _userAccessData.length; i++) {
                var access = _userAccessData[i];
                if (!access.UserRoleCode || !access.SectorCode || !access.LocationCode) {
                    showValidationError('accessError', 'All user access fields must be completed');
                    isValid = false;
                    break;
                }
            }
        }
        
        return isValid;
    }
    
    function cancelForm() {
        if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
            window.location.href = '/Home';
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
    
    function onGridEdit(e) {
        if (e.model.isNew()) {
            // For new rows, set default values
            e.model.set('UserRoleName', 'Select Role');
            e.model.set('SectorValue', 'Select Sector');
            e.model.set('LOBValue', 'Select LOB');
            e.model.set('DepartmentValue', 'Select Department');
            e.model.set('LocationValue', 'Select Location');
        }
    }
    
    // Load LOBs when sector changes
    function loadLOBsBySector(sectorCode, callback) {
        if (!sectorCode) {
            _masterData.lobs = [];
            callback && callback([]);
            return;
        }
        
        $.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function(response) {
                if (response.success) {
                    _masterData.lobs = response.data;
                    callback && callback(response.data);
                } else {
                    _masterData.lobs = [];
                    callback && callback([]);
                }
            })
            .fail(function() {
                _masterData.lobs = [];
                callback && callback([]);
            });
    }
    
    // Load departments when LOB changes
    function loadDepartmentsByLOB(sectorCode, lobCode, callback) {
        if (!sectorCode || !lobCode) {
            _masterData.departments = [];
            callback && callback([]);
            return;
        }
        
        $.get('/MasterData/GetDepartments', { sectorCode: sectorCode, lobCode: lobCode })
            .done(function(response) {
                if (response.success) {
                    _masterData.departments = response.data;
                    callback && callback(response.data);
                } else {
                    _masterData.departments = [];
                    callback && callback([]);
                }
            })
            .fail(function() {
                _masterData.departments = [];
                callback && callback([]);
            });
    }
    
    // Handle sector dropdown change in grid
    function onSectorChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;
        
        var dataItem = grid.dataItem($(e.sender.element).closest("tr"));
        if (!dataItem) return;
        
        var sectorCode = e.sender.value();
        var sectorText = e.sender.text();
        
        // Update the current row's sector info
        dataItem.set('SectorCode', sectorCode);
        dataItem.set('SectorValue', sectorText);
        
        // Reset dependent dropdowns
        dataItem.set('LOBCode', '');
        dataItem.set('LOBValue', 'Select LOB');
        dataItem.set('DepartmentCode', '');
        dataItem.set('DepartmentValue', 'Select Department');
        
        // Load LOBs for this sector
        if (sectorCode) {
            loadLOBsBySector(sectorCode, function(lobs) {
                // Find LOB dropdown in the current row and update its data source
                var row = $(e.sender.element).closest("tr");
                var lobDropDown = row.find('[data-role="dropdownlist"]').filter(function() {
                    return $(this).attr('name').indexOf('LOBValue') >= 0;
                }).data('kendoDropDownList');
                
                if (lobDropDown) {
                    lobDropDown.setDataSource(lobs);
                }
            });
        }
    }
    
    // Handle LOB dropdown change in grid
    function onLOBChange(e) {
        var grid = $("#userAccessGrid").data("kendoGrid");
        if (!grid) return;
        
        var dataItem = grid.dataItem($(e.sender.element).closest("tr"));
        if (!dataItem) return;
        
        var lobCode = e.sender.value();
        var lobText = e.sender.text();
        var sectorCode = dataItem.SectorCode;
        
        // Update the current row's LOB info
        dataItem.set('LOBCode', lobCode);
        dataItem.set('LOBValue', lobText);
        
        // Reset dependent dropdowns
        dataItem.set('DepartmentCode', '');
        dataItem.set('DepartmentValue', 'Select Department');
        
        // Load departments for this sector/LOB combination
        if (sectorCode && lobCode) {
            loadDepartmentsByLOB(sectorCode, lobCode, function(departments) {
                // Find Department dropdown in the current row and update its data source
                var row = $(e.sender.element).closest("tr");
                var deptDropDown = row.find('[data-role="dropdownlist"]').filter(function() {
                    return $(this).attr('name').indexOf('DepartmentValue') >= 0;
                }).data('kendoDropDownList');
                
                if (deptDropDown) {
                    deptDropDown.setDataSource(departments);
                }
            });
        }
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
        onSectorChange: onSectorChange,
        onLOBChange: onLOBChange
    };
    
})();

// Global functions for event handlers
window.onUserIdChange = userCreateViewModel.onUserIdChange;
window.addNewAccess = userCreateViewModel.addNewAccess;
window.saveUser = userCreateViewModel.saveUser;
window.cancelForm = userCreateViewModel.cancelForm;
window.onGridError = userCreateViewModel.onGridError;
window.onSectorChange = userCreateViewModel.onSectorChange;
window.onLOBChange = userCreateViewModel.onLOBChange;