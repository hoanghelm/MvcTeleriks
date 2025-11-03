const DashboardViewModel = {
    data: {
        currentFilter: 'pending',
        allIncidents: [],
        pendingIncidents: [],
        originalAllIncidents: [],
        originalPendingIncidents: [],
        isLoading: false
    },
    elements: {
        listTitle: null,
        incidentGrid: null,
        incidentCountElement: null,
        pendingCountElement: null
    },

    init: function () {
        this.cacheElements();
        this.bindEvents();
        this.loadIncidentData();
        this.initializeDefaultSelection();
    },

    cacheElements: function () {
        this.elements.listTitle = $('#listTitle');
        this.elements.incidentGrid = $('#incidentGrid');
        this.elements.incidentCountElement = $('.incident-count');
        this.elements.pendingCountElement = $('.pending-count');
    },

    bindEvents: function () {
        const self = this;

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

    showLoading: function () {
        kendo.ui.progress(this.elements.incidentGrid, true);
    },

    hideLoading: function () {
        kendo.ui.progress(this.elements.incidentGrid, false);
    },

    loadIncidentData: function () {
        if (this.data.isLoading) return;

        this.data.isLoading = true;
        this.showLoading();

        $.ajax({
            url: '/Home/GetIncidentData',
            type: 'GET',
            success: (response) => {
                this.data.isLoading = false;
                this.hideLoading();

                if (response.success) {
                    this.data.originalAllIncidents = response.allIncidents || [];
                    this.data.originalPendingIncidents = response.pendingIncidents || [];

                    this.data.allIncidents = [...this.data.originalAllIncidents];
                    this.data.pendingIncidents = [...this.data.originalPendingIncidents];

                    this.updateCounts(response.totalCount || 0, response.pendingCount || 0);
                    this.updateGridData('pending');
                } else {
                    this.handleError(response.message || 'Failed to load incident data');
                }
            },
            error: (xhr, status, error) => {
                this.data.isLoading = false;
                this.hideLoading();
                this.handleError('An error occurred while loading incident data');
            }
        });
    },

    updateCounts: function (totalCount, pendingCount) {
        this.elements.incidentCountElement.text(totalCount);
        this.elements.pendingCountElement.text(pendingCount);
    },

    updateGridData: function (filterType) {
        const grid = this.elements.incidentGrid.data("kendoGrid");
        if (!grid) return;

        this.showLoading();

        const dataToShow = filterType === 'all' ? this.data.allIncidents : this.data.pendingIncidents;

        const processedData = dataToShow.map(item => {
            const incidentDateTime = item.incidentDateTime;
            const submittedOn = item.submittedOn;

            return {
                incidentId: item.incidentId || '',
                incidentDateTime: incidentDateTime && typeof incidentDateTime === 'string' ? new Date(incidentDateTime) : incidentDateTime,
                sbuName: item.sbuName || '',
                departmentName: item.departmentName || '',
                incidentDesc: item.incidentDesc || '',
                creatorName: item.creatorName || '',
                submittedOn: submittedOn && typeof submittedOn === 'string' ? new Date(submittedOn) : submittedOn,
                statusDesc: item.statusDesc || '',
                status: item.status || ''
            };
        });

        grid.dataSource.data(processedData);

        setTimeout(() => {
            this.hideLoading();
        }, 300);
    },

    setFilter: function (filterType) {
        this.data.currentFilter = filterType;

        const titles = {
            'all': 'All Incident Reports',
            'pending': 'Pending Reports'
        };

        this.elements.listTitle.text(titles[filterType] || 'Pending Reports');

        this.updateGridData(filterType);
    },

    refreshData: function () {
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
            window.location.href = '/Incident/Update?id=' + encodeURIComponent(incidentId);
        }
    },

    printIncident: function (incidentId) {
        if (incidentId) {
            window.open('/Incident/Print?id=' + encodeURIComponent(incidentId), '_blank');
        }
    },

    onGridDataBound: function (e) {
        if (DashboardViewModel.data.isUpdatingGrid) return;

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
        DashboardViewModel.hideLoading();

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
        TelerikNotification.error(message);
    },

    api: {
        setFilter: function (filterType) {
            DashboardViewModel.setFilter(filterType);
        },
        refreshData: function () {
            DashboardViewModel.refreshData();
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
        }
    }
};