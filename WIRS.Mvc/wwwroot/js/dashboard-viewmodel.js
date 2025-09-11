const DashboardViewModel = {
    data: {
        currentFilter: 'pending',
        searchTerm: '',
        allIncidents: [],
        pendingIncidents: [],
        originalAllIncidents: [],
        originalPendingIncidents: [],
        isLoading: false,
        gridFilters: null
    },
    elements: {
        searchInput: null,
        clearSearchBtn: null,
        listTitle: null,
        incidentGrid: null,
        incidentSkeleton: null,
        incidentCountElement: null,
        pendingCountElement: null
    },
    searchTimeout: null,

    init: function () {
        this.cacheElements();
        this.bindEvents();
        this.loadIncidentData();
        this.initializeDefaultSelection();
    },

    cacheElements: function () {
        this.elements.searchInput = $('#incidentSearchInput');
        this.elements.clearSearchBtn = $('#clearSearchBtn');
        this.elements.listTitle = $('#listTitle');
        this.elements.incidentGrid = $('#incidentGrid');
        this.elements.incidentSkeleton = $('#incidentSkeleton');
        this.elements.incidentCountElement = $('.incident-count');
        this.elements.pendingCountElement = $('.pending-count');
    },

    bindEvents: function () {
        const self = this;

        this.elements.searchInput.on('input', function () {
            self.handleSearchInput($(this).val());
        });

        this.elements.clearSearchBtn.on('click', function () {
            self.handleClearSearch();
        });

        $('.dashboard-card').on('click', function () {
            const filterType = $(this).data('filter');
            self.selectCard(this, filterType);
        });

        $('#refreshDataBtn').on('click', function () {
            self.refreshIncidentData();
        });
    },

    initializeDefaultSelection: function () {
        const self = this;
        setTimeout(function () {
            const pendingCard = $('#pendingCard')[0];
            if (pendingCard) {
                self.selectCard(pendingCard, 'pending');
            }
        }, 500);
    },

    loadIncidentData: function () {
        if (this.data.isLoading) return;

        this.data.isLoading = true;
        this.elements.incidentSkeleton.show();

        $.ajax({
            url: '/Home/GetIncidentData',
            type: 'GET',
            success: (response) => {
                this.data.isLoading = false;
                this.elements.incidentSkeleton.hide();

                if (response.success) {
                    this.data.originalAllIncidents = response.allIncidents || [];
                    this.data.originalPendingIncidents = response.pendingIncidents || [];

                    this.data.allIncidents = [...this.data.originalAllIncidents];
                    this.data.pendingIncidents = [...this.data.originalPendingIncidents];

                    console.log("Loaded data - All:", this.data.allIncidents.length, "Pending:", this.data.pendingIncidents.length);
                    if (this.data.pendingIncidents.length > 0) {
                        console.log("Sample pending item:", this.data.pendingIncidents[0]);
                        console.log("Sample fields:", Object.keys(this.data.pendingIncidents[0]));
                    }

                    this.updateCounts(response.totalCount || 0, response.pendingCount || 0);
                    this.updateGridData('pending');
                } else {
                    this.handleError(response.message || 'Failed to load incident data');
                }
            },
            error: (xhr, status, error) => {
                this.data.isLoading = false;
                this.elements.incidentSkeleton.hide();
                this.handleError('An error occurred while loading incident data');
                console.error('AJAX Error:', xhr, status, error);
            }
        });
    },

    updateCounts: function (totalCount, pendingCount) {
        this.elements.incidentCountElement.text(totalCount);
        this.elements.pendingCountElement.text(pendingCount);
    },

    handleSearchInput: function (searchTerm) {
        console.log("Search input:", searchTerm);
        this.data.searchTerm = searchTerm || '';

        if (this.searchTimeout) {
            clearTimeout(this.searchTimeout);
        }

        this.searchTimeout = setTimeout(() => {
            this.applyClientSideSearch();
        }, 300);
    },

    handleClearSearch: function () {
        console.log("Clearing search");
        this.elements.searchInput.val('');
        this.data.searchTerm = '';

        const grid = this.elements.incidentGrid.data("kendoGrid");
        if (grid) {
            grid.dataSource.filter({});
            this.data.gridFilters = null;
        }

        this.applyClientSideSearch();
    },

    applyClientSideSearch: function () {
        console.log("Applying client-side search with term:", this.data.searchTerm);

        const searchTerm = (this.data.searchTerm || '').toLowerCase().trim();
        let dataToFilter;

        if (this.data.currentFilter === 'all') {
            dataToFilter = [...this.data.originalAllIncidents];
        } else {
            dataToFilter = [...this.data.originalPendingIncidents];
        }

        if (searchTerm !== '') {
            console.log("Filtering data with search term:", searchTerm);
            dataToFilter = this.filterDataBySearchTerm(dataToFilter, searchTerm);
            console.log("Search results:", dataToFilter.length);
        }

        if (this.data.currentFilter === 'all') {
            this.data.allIncidents = dataToFilter;
        } else {
            this.data.pendingIncidents = dataToFilter;
        }

        this.updateGridDataWithFilters(this.data.currentFilter);
    },

    filterDataBySearchTerm: function (data, searchTerm) {
        if (!data || !Array.isArray(data)) {
            console.log("Invalid data for filtering:", data);
            return [];
        }

        console.log("Filtering", data.length, "items with term:", searchTerm);

        const filtered = data.filter(item => {
            if (!item) return false;

            const searchableFields = [
                this.getFieldValue(item, 'incidentId'),
                this.getFieldValue(item, 'incidentDesc'),
                this.getFieldValue(item, 'creatorName'),
                this.getFieldValue(item, 'sbuName'),
                this.getFieldValue(item, 'departmentName'),
                this.getFieldValue(item, 'statusDesc')
            ];

            const matches = searchableFields.some(field => {
                const fieldStr = (field || '').toString().toLowerCase();
                const match = fieldStr.includes(searchTerm);
                return match;
            });

            return matches;
        });

        console.log("Filtered result count:", filtered.length);
        return filtered;
    },

    getFieldValue: function (item, fieldName) {
        if (!item) return '';

        const variations = [
            fieldName,
            this.capitalizeFirst(fieldName),
            fieldName.toLowerCase(),
            fieldName.toUpperCase(),
            this.camelToSnake(fieldName),
            this.camelToSnake(fieldName).toUpperCase()
        ];

        for (let variation of variations) {
            if (item.hasOwnProperty(variation) && item[variation] !== null && item[variation] !== undefined) {
                return item[variation];
            }
        }

        return '';
    },

    capitalizeFirst: function (str) {
        if (!str) return '';
        return str.charAt(0).toUpperCase() + str.slice(1);
    },

    camelToSnake: function (str) {
        if (!str) return '';
        return str.replace(/([A-Z])/g, '_$1').toLowerCase();
    },

    updateGridData: function (filterType) {
        this.updateGridDataWithFilters(filterType);
    },

    updateGridDataWithFilters: function (filterType) {
        const grid = this.elements.incidentGrid.data("kendoGrid");
        if (!grid) {
            console.log("Grid not found");
            return;
        }

        const dataToShow = filterType === 'all' ? this.data.allIncidents : this.data.pendingIncidents;
        console.log(`Setting grid data for ${filterType}:`, dataToShow.length, "items");

        if (dataToShow.length === 0) {
            console.log("No data to show");
            grid.dataSource.data([]);
            grid.refresh();
            return;
        }

        const processedData = dataToShow.map(item => {
            const processed = {
                incidentId: this.getFieldValue(item, 'incidentId'),
                incidentDateTime: this.getFieldValue(item, 'incidentDateTime') || this.getFieldValue(item, 'incident_date_time'),
                sbuName: this.getFieldValue(item, 'sbuName') || this.getFieldValue(item, 'sbu_name'),
                departmentName: this.getFieldValue(item, 'departmentName') || this.getFieldValue(item, 'department_name'),
                incidentDesc: this.getFieldValue(item, 'incidentDesc') || this.getFieldValue(item, 'incident_desc'),
                creatorName: this.getFieldValue(item, 'creatorName') || this.getFieldValue(item, 'creator_name'),
                submittedOn: this.getFieldValue(item, 'submittedOn') || this.getFieldValue(item, 'submitted_on'),
                statusDesc: this.getFieldValue(item, 'statusDesc') || this.getFieldValue(item, 'status_desc'),
                status: this.getFieldValue(item, 'status')
            };

            if (processed.incidentDateTime && typeof processed.incidentDateTime === 'string') {
                processed.incidentDateTime = new Date(processed.incidentDateTime);
            }
            if (processed.submittedOn && typeof processed.submittedOn === 'string') {
                processed.submittedOn = new Date(processed.submittedOn);
            }

            return processed;
        });

        grid.dataSource.data(processedData);

        if (this.data.gridFilters) {
            console.log("Reapplying grid filters:", this.data.gridFilters);
            grid.dataSource.filter(this.data.gridFilters);
        }

        grid.refresh();
        console.log("Grid refreshed with", processedData.length, "items");
    },

    onGridFilter: function (e) {
        console.log("Grid filter event:", e);
        this.data.gridFilters = e.filter;
        this.applyClientSideSearch();
    },

    setFilter: function (filterType) {
        console.log("Setting filter to:", filterType);
        this.data.currentFilter = filterType;

        const titles = {
            'all': 'All Incident Reports',
            'pending': 'Pending Reports'
        };

        this.elements.listTitle.text(titles[filterType] || 'Pending Reports');

        const grid = this.elements.incidentGrid.data("kendoGrid");
        if (grid) {
            grid.dataSource.filter({});
            this.data.gridFilters = null;
        }

        this.applyClientSideSearch();
    },

    refreshData: function () {
        this.elements.searchInput.val('');
        this.data.searchTerm = '';
        this.data.gridFilters = null;
        this.loadIncidentData();
    },

    selectCard: function (cardElement, filterType) {
        $('.dashboard-card').removeClass('border-blue-500 border-yellow-500 shadow-xl ring-2 ring-blue-200 ring-yellow-200');
        $('.dashboard-card').addClass('border-transparent');

        $(cardElement).removeClass('border-transparent');
        if (filterType === 'all') {
            $(cardElement).addClass('border-blue-500 shadow-xl ring-2 ring-blue-200');
        } else if (filterType === 'pending') {
            $(cardElement).addClass('border-yellow-500 shadow-xl ring-2 ring-yellow-200');
        }

        this.setFilter(filterType);
    },

    refreshIncidentData: function () {
        const pendingCard = $('#pendingCard')[0];
        if (pendingCard) {
            this.selectCard(pendingCard, 'pending');
        }
        this.refreshData();
    },

    viewIncident: function (incidentId) {
        if (incidentId) {
            window.location.href = '/Incident/Details?id=' + encodeURIComponent(incidentId);
        }
    },

    printIncident: function (incidentId) {
        if (incidentId) {
            window.open('/Incident/Print?id=' + encodeURIComponent(incidentId), '_blank');
        }
    },

    onGridDataBound: function (e) {
        if (DashboardViewModel.data.isUpdatingGrid) {
            console.log("Grid data bound - skipping during update");
            return;
        }

        console.log("Grid data bound event");
        $('#incidentSkeleton').hide();

        const grid = e.sender;
        const gridElement = grid.element;

        gridElement.find('tbody tr').each(function () {
            $(this).hover(
                function () { $(this).addClass('bg-gray-50'); },
                function () { $(this).removeClass('bg-gray-50'); }
            );
        });

        DashboardViewModel.updateGridInfo(grid);
        DashboardViewModel.applyCustomGridStyling(gridElement);
    },

    onGridError: function (e) {
        console.error('Grid error:', e);
        $('#incidentSkeleton').hide();

        let errorMessage = 'An error occurred while loading data. Please try again.';

        if (e.errors) {
            if (typeof e.errors === 'string') {
                errorMessage = e.errors;
            } else if (Array.isArray(e.errors) && e.errors.length > 0) {
                errorMessage = e.errors.join(', ');
            }
        }

        this.handleError(errorMessage);
    },

    updateGridInfo: function (grid) {
        if (!grid || !grid.dataSource) return;

        const total = grid.dataSource.total();
        const currentPage = grid.dataSource.page();
        const pageSize = grid.dataSource.pageSize();
        const startRecord = total > 0 ? ((currentPage - 1) * pageSize) + 1 : 0;
        const endRecord = Math.min(currentPage * pageSize, total);

        const gridInfo = $('.grid-info');
        if (gridInfo.length > 0) {
            if (total > 0) {
                gridInfo.text(`Showing ${startRecord} to ${endRecord} of ${total} entries`);
            } else {
                gridInfo.text('No entries found');
            }
        }
    },

    applyCustomGridStyling: function (gridElement) {
        gridElement.find('tbody tr').each(function () {
            const $row = $(this);
            const statusCell = $row.find('td').eq(7);

            if (statusCell.length > 0) {
                const statusSpan = statusCell.find('span');
                if (statusSpan.length > 0) {
                    const statusText = statusSpan.text().toLowerCase();

                    if (statusText.includes('pending')) {
                        statusSpan.addClass('status-pending');
                    } else if (statusText.includes('completed')) {
                        statusSpan.addClass('status-completed');
                    } else if (statusText.includes('progress')) {
                        statusSpan.addClass('status-progress');
                    }
                }
            }
        });

        gridElement.find('.k-button').each(function () {
            $(this).addClass('action-button');
        });

        gridElement.find('a.text-blue-600').each(function () {
            $(this).addClass('incident-link');
        });
    },

    handleError: function (message) {
        console.error('Dashboard error:', message);
        TelerikNotification.error(message);
    },

    api: {
        setFilter: function (filterType) {
            DashboardViewModel.setFilter(filterType);
        },
        refreshData: function () {
            DashboardViewModel.refreshData();
        },
        clearSearch: function () {
            DashboardViewModel.handleClearSearch();
        },
        getCurrentFilter: function () {
            return DashboardViewModel.data.currentFilter;
        },
        getCurrentData: function () {
            return {
                currentFilter: DashboardViewModel.data.currentFilter,
                allIncidents: DashboardViewModel.data.allIncidents,
                pendingIncidents: DashboardViewModel.data.pendingIncidents
            };
        },
        viewIncident: function (incidentId) {
            DashboardViewModel.viewIncident(incidentId);
        },
        printIncident: function (incidentId) {
            DashboardViewModel.printIncident(incidentId);
        },
        onGridDataBound: function (e) {
            DashboardViewModel.onGridDataBound(e);
        },
        onGridError: function (e) {
            DashboardViewModel.onGridError(e);
        },
        onGridFilter: function (e) {
            DashboardViewModel.onGridFilter(e);
        }
    }
};