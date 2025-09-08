const DashboardViewModel = {
    // Data (Model)
    data: {
        incidents: [],
        pendingIncidents: [],
        filteredIncidents: [],
        currentFilter: 'all',
        searchTerm: '',
        loading: false
    },

    // DOM Elements (cached for performance)
    elements: {
        searchInput: null,
        clearSearchBtn: null,
        incidentCount: null,
        pendingCount: null,
        listTitle: null,
        incidentGrid: null,
        incidentSkeleton: null,
        dashboardCards: null
    },

    // Initialize the view model
    init: function() {
        this.cacheElements();
        this.bindEvents();
        this.loadData();
    },

    // Cache DOM elements
    cacheElements: function() {
        this.elements.searchInput = $('#incidentSearchInput');
        this.elements.clearSearchBtn = $('#clearSearchBtn');
        this.elements.incidentCount = $('.incident-count');
        this.elements.pendingCount = $('.pending-count');
        this.elements.listTitle = $('#listTitle');
        this.elements.incidentGrid = $('#incidentGrid');
        this.elements.incidentSkeleton = $('#incidentSkeleton');
        this.elements.dashboardCards = $('.dashboard-card');
    },

    // Bind event handlers
    bindEvents: function() {
        const self = this;

        // Search input change
        this.elements.searchInput.on('input', function() {
            self.handleSearchInput($(this).val());
        });

        // Clear search button
        this.elements.clearSearchBtn.on('click', function() {
            self.handleClearSearch();
        });
    },

    // Event Handlers
    handleSearchInput: function(searchTerm) {
        this.data.searchTerm = searchTerm;
        this.applyCurrentFilter();
    },

    handleClearSearch: function() {
        this.elements.searchInput.val('');
        this.data.searchTerm = '';
        this.applyCurrentFilter();
    },

    // Data Loading
    loadData: function() {
        const self = this;
        self.setLoadingState(true);
        
        $.ajax({
            url: '/Home/GetIncidentData',
            type: 'POST',
            success: function(response) {
                self.handleDataLoadSuccess(response);
            },
            error: function(xhr, status, error) {
                self.handleDataLoadError(error);
            },
            complete: function() {
                self.setLoadingState(false);
            }
        });
    },

    handleDataLoadSuccess: function(response) {
        if (response.success) {
            this.data.incidents = response.incidents || [];
            this.data.pendingIncidents = response.pendingIncidents || [];
            this.updateCounters();
            this.applyCurrentFilter();
            this.showSuccessMessage('Data loaded successfully');
        } else {
            this.showErrorMessage(response.message || 'Failed to load data');
        }
    },

    handleDataLoadError: function(error) {
        this.showErrorMessage('Failed to load incident data');
        console.error('Dashboard data load error:', error);
    },

    // View State Management
    setLoadingState: function(isLoading) {
        this.data.loading = isLoading;
        
        if (isLoading) {
            this.elements.incidentGrid.fadeOut(300);
            this.elements.incidentSkeleton.fadeIn(300);
        }
    },

    updateCounters: function() {
        this.elements.incidentCount.text(this.data.incidents.length);
        this.elements.pendingCount.text(this.data.pendingIncidents.length);
    },

    setFilter: function(filterType) {
        this.data.currentFilter = filterType;
        
        // Update title based on filter
        const titles = {
            'all': 'All Incident Reports',
            'pending': 'Pending Reports'
        };
        this.elements.listTitle.text(titles[filterType] || 'All Incident Reports');
        
        this.applyCurrentFilter();
    },

    applyCurrentFilter: function() {
        let dataToShow = [];
        
        switch(this.data.currentFilter) {
            case 'pending':
                dataToShow = this.data.pendingIncidents;
                break;
            case 'all':
            default:
                dataToShow = [...this.data.incidents, ...this.data.pendingIncidents];
        }
        
        if (this.data.searchTerm) {
            dataToShow = this.filterDataBySearch(dataToShow);
        }
        
        this.data.filteredIncidents = dataToShow;
        this.updateGridData(dataToShow);
    },

    filterDataBySearch: function(data) {
        const searchTerm = this.data.searchTerm.toLowerCase();
        return data.filter(incident => 
            (incident.incidentDesc && incident.incidentDesc.toLowerCase().includes(searchTerm)) ||
            (incident.incident_id && incident.incident_id.toLowerCase().includes(searchTerm)) ||
            (incident.creatorName && incident.creatorName.toLowerCase().includes(searchTerm))
        );
    },


    // Grid Management
    updateGridData: function(data) {
        const grid = this.elements.incidentGrid.data("kendoGrid");
        
        if (!grid) {
            setTimeout(() => { 
                this.updateGridData(data); 
            }, 100);
            return;
        }

        this.elements.incidentSkeleton.fadeOut(300, () => {
            grid.setDataSource(new kendo.data.DataSource({
                data: data,
                pageSize: 15,
                schema: {
                    model: {
                        fields: {
                            incident_id: { type: "string" },
                            incidentDateTime: { type: "date" },
                            sbuName: { type: "string" },
                            departmentName: { type: "string" },
                            incidentDesc: { type: "string" },
                            creatorName: { type: "string" },
                            submittedOn: { type: "date" },
                            statusDesc: { type: "string" }
                        }
                    }
                }
            }));
            this.elements.incidentGrid.fadeIn(300);
        });
    },

    showSuccessMessage: function(message) {
        if (kendo.ui && kendo.ui.Notification) {
            const notification = $("<span></span>").kendoNotification({
                position: { pinned: true, top: 30, right: 30 },
                autoHideAfter: 3000,
                stacking: "up"
            }).data("kendoNotification");
            notification.success(message);
        }
    },

    showErrorMessage: function(message) {
        if (kendo.ui && kendo.ui.Notification) {
            const notification = $("<span></span>").kendoNotification({
                position: { pinned: true, top: 30, right: 30 },
                autoHideAfter: 5000,
                stacking: "up"
            }).data("kendoNotification");
            notification.error(message);
        }
    },

    // Public API for external interaction
    api: {
        setFilter: function(filterType) {
            DashboardViewModel.setFilter(filterType);
        },
        
        refreshData: function() {
            DashboardViewModel.loadData();
        },
        
        clearSearch: function() {
            DashboardViewModel.handleClearSearch();
        },
        
        getCurrentFilter: function() {
            return DashboardViewModel.data.currentFilter;
        },
        
        getFilteredData: function() {
            return DashboardViewModel.data.filteredIncidents;
        },
        
        isLoading: function() {
            return DashboardViewModel.data.loading;
        }
    }
};