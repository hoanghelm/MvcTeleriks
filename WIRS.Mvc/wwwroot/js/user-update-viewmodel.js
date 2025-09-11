var userUpdateViewModel = (function () {
    
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
        ddlAccountStatus: null,
        dtpInactiveDate: null,
        userAccessGrid: null,
        loadingOverlay: null
    };
    
    function init(existingUserAccess) {
        initializeElements();
        loadMasterData();
        setupValidation();
        initializeUserAccessGrid();
        
        // Load existing user access data
        if (existingUserAccess && existingUserAccess.length > 0) {
            loadExistingUserAccess(existingUserAccess);
        }
    }
    
    function initializeElements() {
        _elements.txtUserId = $("#txtUserId").data("kendoTextBox");
        _elements.txtUserName = $("#txtUserName").data("kendoTextBox");
        _elements.txtEmail = $("#txtEmail").data("kendoTextBox");
        _elements.ddlUserRole = $("#ddlUserRole").data("kendoDropDownList");
        _elements.ddlAccountStatus = $("#ddlAccountStatus").data("kendoDropDownList");
        _elements.dtpInactiveDate = $("#dtpInactiveDate").data("kendoDatePicker");
        _elements.userAccessGrid = $("#userAccessGrid").data("kendoGrid");
        _elements.loadingOverlay = $("#loadingOverlay");
    }
    
    function setupValidation() {
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
    }
    
    function loadMasterData() {
        // Show skeleton loading only for dropdowns that need data
        TelerikSkeleton.showDropdownSkeleton("#ddlUserRole");
        TelerikSkeleton.showDropdownSkeleton("#ddlAccountStatus");
        
        var requests = [
            $.get('/MasterData/GetUserRoles'),
            $.get('/MasterData/GetSectors'),
            $.get('/MasterData/GetLocations'),
            $.get('/MasterData/GetAccountStatuses')
        ];
        
        $.when.apply($, requests).done(function(userRoles, sectors, locations, accountStatuses) {
            _masterData.userRoles = userRoles[0].success ? userRoles[0].data : [];
            _masterData.sectors = sectors[0].success ? sectors[0].data : [];
            _masterData.locations = locations[0].success ? locations[0].data : [];
            
            _masterData.lobs = [];
            _masterData.departments = [];
            
            // Hide skeleton loading for dropdowns
            TelerikSkeleton.hideDropdownSkeleton("#ddlUserRole");
            TelerikSkeleton.hideDropdownSkeleton("#ddlAccountStatus");
        }).fail(function() {
            TelerikSkeleton.hideDropdownSkeleton("#ddlUserRole");
            TelerikSkeleton.hideDropdownSkeleton("#ddlAccountStatus");
            TelerikNotification.error('Error loading master data');
        });
    }
    
    function loadExistingUserAccess(userAccessList) {
        _userAccessData = [];
        _currentUserAccessId = 0;
        
        if (userAccessList) {
            userAccessList.forEach(function(access, index) {
                _userAccessData.push({
                    Id: ++_currentUserAccessId,
                    UserRoleCode: access.UserRoleCode || '',
                    UserRoleName: access.UserRoleName || '',
                    SectorCode: access.SectorCode || '',
                    SectorValue: access.SectorValue || '',
                    LOBCode: access.LOBCode || '',
                    LOBValue: access.LOBValue || '',
                    DepartmentCode: access.DepartmentCode || '',
                    DepartmentValue: access.DepartmentValue || '',
                    LocationCode: access.LocationCode || '',
                    LocationValue: access.LocationValue || ''
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
        }
    }
    
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
    
    function updateUser() {
        if (!validateForm()) {
            return;
        }
        
        var updateData = {
            UserId: $("#hiddenUserId").val(),
            Email: _elements.txtEmail ? _elements.txtEmail.value() : '',
            UserRole: _elements.ddlUserRole ? _elements.ddlUserRole.value() : '',
            AccountStatus: _elements.ddlAccountStatus ? _elements.ddlAccountStatus.value() : '',
            InactiveDate: _elements.dtpInactiveDate && _elements.dtpInactiveDate.value() ? 
                         kendo.toString(_elements.dtpInactiveDate.value(), "yyyy-MM-dd") : '',
            UserAccess: _userAccessData.map(function(item) {
                return {
                    UserRoleCode: item.UserRoleCode || (_elements.ddlUserRole ? _elements.ddlUserRole.value() : ''),
                    SectorCode: item.SectorCode,
                    LobCode: item.LOBCode,
                    DepartmentCode: item.DepartmentCode,
                    LocationCode: item.LocationCode
                };
            })
        };
        
        showLoading();
        
        $.ajax({
            url: '/User/UpdateUser',
            type: 'POST',
            data: JSON.stringify(updateData),
            contentType: 'application/json',
            success: function(response) {
                hideLoading();
                
                if (response.success) {
                    TelerikNotification.success("User updated successfully!");
                    setTimeout(function() {
                        window.location.href = '/User';
                    }, 2000);
                } else {
                    TelerikNotification.error(response.message);
                }
            },
            error: function() {
                hideLoading();
                TelerikNotification.error('Error updating user. Please try again.');
            }
        });
    }
    
    function resetPassword() {
        if (!confirm("Are you sure you want to reset this user's password? A new password will be generated and sent to their email.")) {
            return;
        }
        
        var userId = $("#hiddenUserId").val();
        
        showLoading();
        
        $.ajax({
            url: '/User/ResetUserPassword',
            type: 'POST',
            data: JSON.stringify(userId),
            contentType: 'application/json',
            success: function(response) {
                hideLoading();
                
                if (response.success) {
                    TelerikNotification.success("Password reset successfully! New password sent to user's email.");
                } else {
                    TelerikNotification.error(response.message);
                }
            },
            error: function() {
                hideLoading();
                TelerikNotification.error('Error resetting password. Please try again.');
            }
        });
    }
    
    function inactiveUser() {
        if (!confirm("Are you sure you want to inactivate this user? This action cannot be undone.")) {
            return;
        }
        
        var userId = $("#hiddenUserId").val();
        
        showLoading();
        
        $.ajax({
            url: '/User/InactiveUser',
            type: 'POST',
            data: JSON.stringify(userId),
            contentType: 'application/json',
            success: function(response) {
                hideLoading();
                
                if (response.success) {
                    TelerikNotification.success("User inactivated successfully!");
                    setTimeout(function() {
                        window.location.href = '/User';
                    }, 2000);
                } else {
                    TelerikNotification.error(response.message);
                }
            },
            error: function() {
                hideLoading();
                TelerikNotification.error('Error inactivating user. Please try again.');
            }
        });
    }
    
    function validateForm() {
        var isValid = true;
        
        // Clear previous errors
        $('.validation-error').empty();
        
        // Validate Email
        var email = _elements.txtEmail ? _elements.txtEmail.value() : '';
        var emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/i;
        if (!email) {
            showValidationError('emailError', 'Email Address is required');
            isValid = false;
        } else if (!emailRegex.test(email)) {
            showValidationError('emailError', 'Please enter a valid email address');
            isValid = false;
        }
        
        // Validate User Role
        if (!_elements.ddlUserRole || !_elements.ddlUserRole.value()) {
            showValidationError('userRoleError', 'User Role is required');
            isValid = false;
        }
        
        // Validate User Access
        if (_userAccessData.length === 0) {
            showValidationError('accessError', 'At least one user access permission is required');
            isValid = false;
        } else {
            for (var i = 0; i < _userAccessData.length; i++) {
                var access = _userAccessData[i];
                if (!access.UserRoleCode || !access.SectorCode) {
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
            window.location.href = '/User';
        }
    }
    
    function showValidationError(elementId, message) {
        $("#" + elementId).html(`<span class="validation-error text-sm text-red-600">${message}</span>`);
    }
    
    function clearValidationError(elementId) {
        $("#" + elementId).empty();
    }
    
    function onGridEdit(e) {
        if (e.model.isNew()) {
            e.model.set('UserRoleName', 'Select Role');
            e.model.set('SectorValue', 'Select Sector');
            e.model.set('LOBValue', 'Select LOB');
            e.model.set('DepartmentValue', 'Select Department');
            e.model.set('LocationValue', 'Select Location');
        }
    }
    
    function showLoading() {
        if (_elements.loadingOverlay) {
            _elements.loadingOverlay.show();
        }
    }
    
    function hideLoading() {
        if (_elements.loadingOverlay) {
            _elements.loadingOverlay.hide();
        }
    }
    
    
    return {
        init: init,
        addNewAccess: addNewAccess,
        updateUser: updateUser,
        resetPassword: resetPassword,
        inactiveUser: inactiveUser,
        cancelForm: cancelForm
    };
    
})();

// Global functions for event handlers
window.addNewAccess = userUpdateViewModel.addNewAccess;
window.updateUser = userUpdateViewModel.updateUser;
window.resetPassword = userUpdateViewModel.resetPassword;
window.inactiveUser = userUpdateViewModel.inactiveUser;
window.cancelForm = userUpdateViewModel.cancelForm;