var userListViewModel = (function () {

    var _elements = {
        ddlSearchSector: null,
        ddlSearchLOB: null,
        ddlSearchUserRole: null,
        txtSearchUserId: null,
        txtSearchUserName: null,
        userGrid: null
    };

    function init() {
        initializeElements();
        setupEventHandlers();
        searchUsers();
    }

    function initializeElements() {
        _elements.ddlSearchSector = $("#ddlSearchSector").data("kendoDropDownList");
        _elements.ddlSearchLOB = $("#ddlSearchLOB").data("kendoDropDownList");
        _elements.ddlSearchUserRole = $("#ddlSearchUserRole").data("kendoDropDownList");
        _elements.txtSearchUserId = $("#txtSearchUserId").data("kendoTextBox");
        _elements.txtSearchUserName = $("#txtSearchUserName").data("kendoTextBox");
        _elements.userGrid = $("#userGrid").data("kendoGrid");
    }

    function setupEventHandlers() {
        if (_elements.txtSearchUserId) {
            $("#txtSearchUserId").keypress(function (e) {
                if (e.which === 13) {
                    searchUsers();
                }
            });
        }

        if (_elements.txtSearchUserName) {
            $("#txtSearchUserName").keypress(function (e) {
                if (e.which === 13) {
                    searchUsers();
                }
            });
        }
    }

    function showLoading() {
        kendo.ui.progress($("#userGrid"), true);
    }

    function hideLoading() {
        kendo.ui.progress($("#userGrid"), false);
    }

    function searchUsers() {
        if (!_elements.userGrid) {
            return;
        }

        var searchCriteria = {
            Sector: _elements.ddlSearchSector ? _elements.ddlSearchSector.value() : '',
            LOB: _elements.ddlSearchLOB ? _elements.ddlSearchLOB.value() : '',
            UserRole: _elements.ddlSearchUserRole ? _elements.ddlSearchUserRole.value() : '',
            UserId: _elements.txtSearchUserId ? _elements.txtSearchUserId.value() : '',
            UserName: _elements.txtSearchUserName ? _elements.txtSearchUserName.value() : ''
        };

        showLoading();

        ApiConfig.ajax({
            url: '/User/SearchUsers',
            type: 'POST',
            data: JSON.stringify(searchCriteria),
            contentType: 'application/json',
            success: function (response) {
                hideLoading();

                if (response.success && response.data) {
                    var users = response.data.users || response.data.Users || [];

                    var mappedUsers = users.map(function (user) {
                        return {
                            UserId: user.userId || user.UserId,
                            UserName: user.userName || user.UserName,
                            UserRoleName: user.userRoleName || user.UserRoleName,
                            Email: user.email || user.Email,
                            AccountStatusName: user.accountStatusName || user.AccountStatusName
                        };
                    });

                    var dataSource = new kendo.data.DataSource({
                        data: mappedUsers,
                        pageSize: 20,
                        schema: {
                            model: {
                                id: "UserId",
                                fields: {
                                    UserId: { type: "string" },
                                    UserName: { type: "string" },
                                    UserRoleName: { type: "string" },
                                    Email: { type: "string" },
                                    AccountStatusName: { type: "string" }
                                }
                            }
                        }
                    });

                    _elements.userGrid.setDataSource(dataSource);

                    var totalCount = response.data.totalCount || response.data.TotalCount || 0;
                    var hasSearchCriteria = hasActiveSearchCriteria();

                    if (hasSearchCriteria) {
                        if (totalCount === 0) {
                            TelerikNotification.warning('No users found matching your search criteria. Try adjusting your filters.');
                        } else if (totalCount === 1) {
                            TelerikNotification.success('Found 1 user matching your search.');
                        } else if (totalCount > 50) {
                            TelerikNotification.info(`Found ${totalCount} users. Consider refining your search for better results.`);
                        }
                    }
                } else {
                    TelerikNotification.error(response.message || 'Error searching users');
                    _elements.userGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                TelerikNotification.error('Error searching users. Please try again.');
                _elements.userGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
            }
        });
    }

    function onSectorChange() {
        if (!_elements.ddlSearchSector || !_elements.ddlSearchLOB) return;

        var sectorCode = _elements.ddlSearchSector.value();

        _elements.ddlSearchLOB.setDataSource([]);
        _elements.ddlSearchLOB.value('');

        if (sectorCode) {
            loadLOBsBySector(sectorCode, function (lobs) {
                _elements.ddlSearchLOB.setDataSource(lobs);
            });
        }
    }

    function loadLOBsBySector(sectorCode, callback) {
        ApiConfig.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function (response) {
                callback(response);
            })
            .fail(function () {
                callback([]);
            });
    }

    function hasActiveSearchCriteria() {
        var sector = _elements.ddlSearchSector ? _elements.ddlSearchSector.value() : '';
        var lob = _elements.ddlSearchLOB ? _elements.ddlSearchLOB.value() : '';
        var userRole = _elements.ddlSearchUserRole ? _elements.ddlSearchUserRole.value() : '';
        var userId = _elements.txtSearchUserId ? _elements.txtSearchUserId.value() : '';
        var userName = _elements.txtSearchUserName ? _elements.txtSearchUserName.value() : '';

        return !!(sector || lob || userRole || userId || userName);
    }

    function refreshGrid() {
        searchUsers();
    }

    return {
        init: init,
        searchUsers: searchUsers,
        onSectorChange: onSectorChange,
        refreshGrid: refreshGrid
    };

})();

window.searchUsers = userListViewModel.searchUsers;
window.onSectorChange = userListViewModel.onSectorChange;