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
        // Perform initial search to load all users
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
        // Enter key to search
        if (_elements.txtSearchUserId) {
            $("#txtSearchUserId").keypress(function(e) {
                if (e.which === 13) {
                    searchUsers();
                }
            });
        }
        
        if (_elements.txtSearchUserName) {
            $("#txtSearchUserName").keypress(function(e) {
                if (e.which === 13) {
                    searchUsers();
                }
            });
        }
    }
    
    function searchUsers() {
        if (!_elements.userGrid) {
            console.error('User grid not initialized');
            return;
        }
        
        var searchCriteria = {
            Sector: _elements.ddlSearchSector ? _elements.ddlSearchSector.value() : '',
            LOB: _elements.ddlSearchLOB ? _elements.ddlSearchLOB.value() : '',
            UserRole: _elements.ddlSearchUserRole ? _elements.ddlSearchUserRole.value() : '',
            UserId: _elements.txtSearchUserId ? _elements.txtSearchUserId.value() : '',
            UserName: _elements.txtSearchUserName ? _elements.txtSearchUserName.value() : ''
        };
        
        showSkeletonLoading();
        
        $.ajax({
            url: '/User/SearchUsers',
            type: 'POST',
            data: JSON.stringify(searchCriteria),
            contentType: 'application/json',
            success: function(response) {
                hideSkeletonLoading();
                
                if (response.success) {
                    var dataSource = new kendo.data.DataSource({
                        data: response.data.Users || [],
                        pageSize: 20,
                        schema: {
                            model: {
                                id: "UserId",
                                fields: {
                                    UserId: { type: "string" },
                                    UserName: { type: "string" },
                                    UserRoleName: { type: "string" },
                                    Email: { type: "string" },
                                    AccountStatusName: { type: "string" },
                                    SectorName: { type: "string" },
                                    LOBName: { type: "string" },
                                    LastLoginDate: { type: "date" }
                                }
                            }
                        }
                    });
                    
                    _elements.userGrid.setDataSource(dataSource);
                    
                    // Show search results summary only when appropriate
                    var totalCount = response.data.TotalCount || 0;
                    var hasSearchCriteria = hasActiveSearchCriteria();
                    
                    if (hasSearchCriteria) {
                        if (totalCount === 0) {
                            TelerikNotification.warning('No users found matching your search criteria. Try adjusting your filters.');
                        } else if (totalCount === 1) {
                            TelerikNotification.success('Found 1 user matching your search.');
                        } else if (totalCount > 50) {
                            TelerikNotification.info(`Found ${totalCount} users. Consider refining your search for better results.`);
                        }
                        // Don't show notification for normal result counts (2-50) to avoid spam
                    }
                } else {
                    TelerikNotification.error(response.message || 'Error searching users');
                    _elements.userGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
                }
            },
            error: function(xhr, status, error) {
                hideSkeletonLoading();
                console.error('Search error:', error);
                TelerikNotification.error('Error searching users. Please try again.');
                _elements.userGrid.setDataSource(new kendo.data.DataSource({ data: [] }));
            }
        });
    }
    
    function onSectorChange() {
        if (!_elements.ddlSearchSector || !_elements.ddlSearchLOB) return;
        
        var sectorCode = _elements.ddlSearchSector.value();
        
        // Reset LOB dropdown
        _elements.ddlSearchLOB.setDataSource([]);
        _elements.ddlSearchLOB.value('');
        
        if (sectorCode) {
            // Load LOBs for selected sector
            loadLOBsBySector(sectorCode, function(lobs) {
                _elements.ddlSearchLOB.setDataSource(lobs);
            });
        }
    }
    
    function loadLOBsBySector(sectorCode, callback) {
        $.get('/MasterData/GetLOBs', { sectorCode: sectorCode })
            .done(function(response) {
                if (response.success) {
                    callback(response.data);
                } else {
                    callback([]);
                }
            })
            .fail(function() {
                callback([]);
            });
    }
    
    function showSkeletonLoading() {
        if (!_elements.userGrid) return;
        
        var skeletonTemplate = kendo.template($("#skeleton-template").html());
        var skeletonData = [];
        
        // Create 5 skeleton rows
        for (var i = 0; i < 5; i++) {
            skeletonData.push({});
        }
        
        var skeletonDataSource = new kendo.data.DataSource({
            data: skeletonData
        });
        
        _elements.userGrid.setDataSource(skeletonDataSource);
    }
    
    function hideSkeletonLoading() {
        // Skeleton loading will be replaced when actual data loads
        // This function exists for consistency but actual hiding happens when data loads
    }
    
    function hasActiveSearchCriteria() {
        var sector = _elements.ddlSearchSector ? _elements.ddlSearchSector.value() : '';
        var lob = _elements.ddlSearchLOB ? _elements.ddlSearchLOB.value() : '';
        var userRole = _elements.ddlSearchUserRole ? _elements.ddlSearchUserRole.value() : '';
        var userId = _elements.txtSearchUserId ? _elements.txtSearchUserId.value() : '';
        var userName = _elements.txtSearchUserName ? _elements.txtSearchUserName.value() : '';
        
        return !!(sector || lob || userRole || userId || userName);
    }
    
    // Removed old showNotification - now using TelerikNotification
    
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

// Global functions for event handlers
window.searchUsers = userListViewModel.searchUsers;
window.onSectorChange = userListViewModel.onSectorChange;