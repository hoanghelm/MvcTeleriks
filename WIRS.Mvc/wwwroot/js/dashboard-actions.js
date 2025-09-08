function viewIncident(incidentId) {
    $.ajax({
        url: '/Home/GetIncidentRedirectUrl',
        type: 'POST',
        data: { incidentId: incidentId, action: 'view' },
        success: function(response) {
            if (response.success) {
                window.location.href = response.url;
            } else {
                if (DashboardViewModel && DashboardViewModel.showErrorMessage) {
                    DashboardViewModel.showErrorMessage(response.message);
                }
            }
        },
        error: function() {
            if (DashboardViewModel && DashboardViewModel.showErrorMessage) {
                DashboardViewModel.showErrorMessage('Failed to load incident details');
            }
        }
    });
}

function printIncident(incidentId) {
    $.ajax({
        url: '/Home/GetPrintUrl',
        type: 'POST',
        data: { incidentId: incidentId },
        success: function(response) {
            if (response.success) {
                const printWindow = window.open(response.url, 'PrintWindow',
                    'height=700,width=960,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no');
                if (printWindow) {
                    printWindow.focus();
                } else {
                    if (DashboardViewModel && DashboardViewModel.showErrorMessage) {
                        DashboardViewModel.showErrorMessage('Please allow popups for this site');
                    }
                }
            } else {
                if (DashboardViewModel && DashboardViewModel.showErrorMessage) {
                    DashboardViewModel.showErrorMessage(response.message);
                }
            }
        },
        error: function() {
            if (DashboardViewModel && DashboardViewModel.showErrorMessage) {
                DashboardViewModel.showErrorMessage('Failed to generate print URL');
            }
        }
    });
}

function refreshIncidentData() {
    if (DashboardViewModel && DashboardViewModel.api) {
        DashboardViewModel.api.refreshData();
    }
}

function onLogout() {
    if (confirm('Are you sure you want to logout?')) {
        $.post('/Login/Logout', function() {
            window.location.href = '/Login';
        }).fail(function() {
            window.location.href = '/Login';
        });
    }
}

$(document).ready(function() {
    if (typeof DashboardViewModel !== 'undefined') {
        DashboardViewModel.init();
    }
});