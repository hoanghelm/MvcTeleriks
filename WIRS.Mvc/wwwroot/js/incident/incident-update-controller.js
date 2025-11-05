(function () {
    angular
        .module('incidentUpdateApp')
        .controller('IncidentUpdateController', IncidentUpdateController);

    IncidentUpdateController.$inject = ['$window', '$location', '$scope', '$timeout', '$sce', 'IncidentUpdateService', 'PartAService', 'PartBService', 'PartCService', 'PartDService'];

    function IncidentUpdateController($window, $location, $scope, $timeout, $sce, IncidentUpdateService, PartAService, PartBService, PartCService, PartDService) {
        var vm = this;

        vm.loading = true;
        vm.error = null;
        vm.incident = {};
        vm.currentUser = {};
        vm.activeTab = 'A';

        PartBService.initializePartB(vm);

        PartCService.initializePartC(vm);

        PartDService.initializePartD(vm);

        PartAService.initializePartA(vm);

        vm.getStatusClass = getStatusClass;
        vm.getIncidentTypeText = getIncidentTypeText;
        vm.getCurrentDate = getCurrentDate;
        vm.getWorkflowsByAction = getWorkflowsByAction;
        vm.cancel = cancel;

        vm.onIncidentTypeChange = function () { PartAService.onIncidentTypeChange(vm); };
        vm.addInjuredPerson = function () { PartAService.addInjuredPerson(vm); };
        vm.deleteInjuredPerson = function (index) { PartAService.deleteInjuredPerson(vm, index); };
        vm.searchInjuredPerson = function () { PartAService.searchInjuredPerson(vm); };
        vm.addEyeWitness = function () { PartAService.addEyeWitness(vm); };
        vm.deleteEyeWitness = function (index) { PartAService.deleteEyeWitness(vm, index); };
        vm.searchEyeWitness = function () { PartAService.searchEyeWitness(vm); };
        vm.getIncidentTypeName = function (code) { return PartAService.getIncidentTypeName(vm, code); };
        vm.getSectorName = function (code) { return PartAService.getSectorName(vm, code); };
        vm.getLOBName = function (code) { return PartAService.getLOBName(vm, code); };
        vm.getDepartmentName = function (code) { return PartAService.getDepartmentName(vm, code); };
        vm.getLocationName = function (code) { return PartAService.getLocationName(vm, code); };

        vm.canViewPartB = function () { return PartBService.canViewPartB(vm); };
        vm.canEditPartB = function () { return PartBService.canEditPartB(vm); };
        vm.submitPartB = function () { PartBService.submitPartB(vm); };
        vm.rejectPartB = function () { PartBService.rejectPartB(vm); };
        vm.searchPartBCopyTo = function () { PartBService.openEmployeeSearch(vm, 'copyTo'); };
        vm.addPartBCopyTo = function () { PartBService.addPartBCopyTo(vm); };
        vm.openEmployeeSearch = function (context) { PartBService.openEmployeeSearch(vm, context); };
        vm.removeCopyToPerson = function (index) { PartBService.removeCopyToPerson(vm, index); };
        vm.getInjuredCaseTypeText = function () { return PartBService.getInjuredCaseTypeText(vm); };

        vm.canViewPartC = function () { return PartCService.canViewPartC(vm); };
        vm.canEditPartC = function () { return PartCService.canEditPartC(vm); };
        vm.isInjuryIncident = function () { return PartCService.isInjuryIncident(vm); };
        vm.addPersonInterviewed = function () { PartCService.addPersonInterviewed(vm); };
        vm.removePersonInterviewed = function (index) { PartCService.removePersonInterviewed(vm, index); };
        vm.addInjuryDetail = function () { PartCService.addInjuryDetail(vm); };
        vm.removeInjuryDetail = function (index) { PartCService.removeInjuryDetail(vm, index); };
        vm.addMedicalCertificate = function () { PartCService.addMedicalCertificate(vm); };
        vm.removeMedicalCertificate = function (index) { PartCService.removeMedicalCertificate(vm, index); };
        vm.savePartC = function () { PartCService.savePartC(vm); };
        vm.submitPartC = function () { PartCService.submitPartC(vm); };
        vm.closePartC = function () { PartCService.closePartC(vm); };

        vm.canViewPartD = function () { return PartDService.canViewPartD(vm); };
        vm.canEditPartD = function () { return PartDService.canEditPartD(vm); };
        vm.submitPartD = function () { PartDService.submitPartD(vm); };
        vm.openEmployeeSearchForPartD = function () { PartDService.openEmployeeSearchForPartD(vm); };
        vm.removeAdditionalCopyToFromPartD = function (index) { PartDService.removeAdditionalCopyToFromPartD(vm, index); };
        vm.cancelPartD = function () { PartDService.cancelPartD(); };
        vm.getHsbuName = function (hsbuId) { return PartDService.getHsbuName(vm, hsbuId); };

        init();

        function init() {
            var incidentId = getIncidentIdFromUrl();
            if (!incidentId) {
                vm.error = 'No incident ID provided';
                vm.loading = false;
                return;
            }

            loadCurrentUser()
                .then(function () {
                    return loadIncident(incidentId);
                })
                .then(function () {
                    return PartAService.loadPartAData(vm);
                })
                .then(function () {
                    return PartBService.loadPartBData(vm);
                })
                .then(function () {
                    return PartCService.loadPartCData(vm);
                })
                .then(function () {
                    return PartDService.loadPartDData(vm, getCurrentDate);
                })
                .catch(function (error) {
                    vm.error = error || 'Failed to load incident data';
                })
                .finally(function () {
                    vm.loading = false;
                });
        }

        function getIncidentIdFromUrl() {
            var url = $window.location.href;
            var params = new URLSearchParams($window.location.search);
            return params.get('id');
        }

        function loadCurrentUser() {
            return IncidentUpdateService.getCurrentUser()
                .then(function (user) {
                    vm.currentUser = {
                        userId: user.userId,
                        name: user.userName || user.displayName || 'User',
                        designation: user.designation || '',
                        sectorCode: user.sectorCode || ''
                    };
                });
        }

        function loadIncident(incidentId) {
            return IncidentUpdateService.getIncidentById(incidentId)
                .then(function (incident) {
                    console.log('Loaded incident data:', incident);
                    console.log('IncidentTypes array:', incident.incidentTypes);
                    console.log('IncidentTypes length:', incident.incidentTypes ? incident.incidentTypes.length : 'undefined');
                    vm.incident = incident;
                    return loadStatusName();
                })
                .then(function () {

                });
        }

        function loadStatusName() {
            if (!vm.incident.status) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getStatusName(vm.incident.status)
                .then(function (statusName) {
                    vm.incident.statusName = statusName;
                    console.log('Status name loaded:', statusName);
                })
                .catch(function (error) {
                    console.error('Error loading status name:', error);
                    vm.incident.statusName = vm.incident.status;
                });
        }

        function getStatusClass() {
            var status = parseInt(vm.incident.status);
            if (status === 0) return 'bg-gray-200 text-gray-800';
            if (status >= 1 && status <= 3) return 'bg-yellow-200 text-yellow-800';
            if (status >= 4 && status <= 7) return 'bg-blue-200 text-blue-800';
            if (status === 8) return 'bg-green-200 text-green-800';
            return 'bg-gray-200 text-gray-800';
        }

        function getIncidentTypeText() {
            if (!vm.incident.incidentTypes || vm.incident.incidentTypes.length === 0) {
                return 'N/A';
            }
            return vm.incident.incidentTypes.join(', ');
        }

        function getCurrentDate() {
            var today = new Date();
            var day = String(today.getDate()).padStart(2, '0');
            var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var month = monthNames[today.getMonth()];
            var year = today.getFullYear();
            return day + '-' + month + '-' + year;
        }

        function cancel() {
            if (confirm('Are you sure you want to cancel? Any unsaved changes will be lost.')) {
                $window.location.href = '/Home/Index';
            }
        }

        function getWorkflowsByAction(actionCode) {
            if (!vm.incident || !vm.incident.workflows) {
                return [];
            }
            return vm.incident.workflows.filter(function (workflow) {

                if (workflow.toDesignation && typeof workflow.toDesignation === 'string') {
                    workflow.toDesignation = $sce.trustAsHtml(workflow.toDesignation);
                }
                if (workflow.toName && typeof workflow.toName === 'string') {
                    workflow.toName = $sce.trustAsHtml(workflow.toName);
                }

                return workflow.actionCode === actionCode;
            });
        }
    }
})();(function () {
    angular
        .module('incidentUpdateApp')
        .controller('IncidentUpdateController', IncidentUpdateController);

    IncidentUpdateController.$inject = ['$window', '$location', '$scope', '$timeout', '$sce', 'IncidentUpdateService', 'PartAService', 'PartBService', 'PartCService', 'PartDService'];

    function IncidentUpdateController($window, $location, $scope, $timeout, $sce, IncidentUpdateService, PartAService, PartBService, PartCService, PartDService) {
        var vm = this;

        vm.loading = true;
        vm.error = null;
        vm.incident = {};
        vm.currentUser = {};
        vm.activeTab = 'A';

        PartBService.initializePartB(vm);

        PartCService.initializePartC(vm);

        PartDService.initializePartD(vm);

        PartAService.initializePartA(vm);

        vm.getStatusClass = getStatusClass;
        vm.getIncidentTypeText = getIncidentTypeText;
        vm.getCurrentDate = getCurrentDate;
        vm.getWorkflowsByAction = getWorkflowsByAction;
        vm.cancel = cancel;

        vm.onIncidentTypeChange = function () { PartAService.onIncidentTypeChange(vm); };
        vm.addInjuredPerson = function () { PartAService.addInjuredPerson(vm); };
        vm.deleteInjuredPerson = function (index) { PartAService.deleteInjuredPerson(vm, index); };
        vm.searchInjuredPerson = function () { PartAService.searchInjuredPerson(vm); };
        vm.addEyeWitness = function () { PartAService.addEyeWitness(vm); };
        vm.deleteEyeWitness = function (index) { PartAService.deleteEyeWitness(vm, index); };
        vm.searchEyeWitness = function () { PartAService.searchEyeWitness(vm); };
        vm.getIncidentTypeName = function (code) { return PartAService.getIncidentTypeName(vm, code); };
        vm.getSectorName = function (code) { return PartAService.getSectorName(vm, code); };
        vm.getLOBName = function (code) { return PartAService.getLOBName(vm, code); };
        vm.getDepartmentName = function (code) { return PartAService.getDepartmentName(vm, code); };
        vm.getLocationName = function (code) { return PartAService.getLocationName(vm, code); };

        vm.canViewPartB = function () { return PartBService.canViewPartB(vm); };
        vm.canEditPartB = function () { return PartBService.canEditPartB(vm); };
        vm.submitPartB = function () { PartBService.submitPartB(vm); };
        vm.rejectPartB = function () { PartBService.rejectPartB(vm); };
        vm.searchPartBCopyTo = function () { PartBService.openEmployeeSearch(vm, 'copyTo'); };
        vm.addPartBCopyTo = function () { PartBService.addPartBCopyTo(vm); };
        vm.openEmployeeSearch = function (context) { PartBService.openEmployeeSearch(vm, context); };
        vm.removeCopyToPerson = function (index) { PartBService.removeCopyToPerson(vm, index); };
        vm.getInjuredCaseTypeText = function () { return PartBService.getInjuredCaseTypeText(vm); };

        vm.canViewPartC = function () { return PartCService.canViewPartC(vm); };
        vm.canEditPartC = function () { return PartCService.canEditPartC(vm); };
        vm.isInjuryIncident = function () { return PartCService.isInjuryIncident(vm); };
        vm.addPersonInterviewed = function () { PartCService.addPersonInterviewed(vm); };
        vm.removePersonInterviewed = function (index) { PartCService.removePersonInterviewed(vm, index); };
        vm.addInjuryDetail = function () { PartCService.addInjuryDetail(vm); };
        vm.removeInjuryDetail = function (index) { PartCService.removeInjuryDetail(vm, index); };
        vm.addMedicalCertificate = function () { PartCService.addMedicalCertificate(vm); };
        vm.removeMedicalCertificate = function (index) { PartCService.removeMedicalCertificate(vm, index); };
        vm.savePartC = function () { PartCService.savePartC(vm); };
        vm.submitPartC = function () { PartCService.submitPartC(vm); };
        vm.closePartC = function () { PartCService.closePartC(vm); };

        vm.canViewPartD = function () { return PartDService.canViewPartD(vm); };
        vm.canEditPartD = function () { return PartDService.canEditPartD(vm); };
        vm.submitPartD = function () { PartDService.submitPartD(vm); };
        vm.openEmployeeSearchForPartD = function () { PartDService.openEmployeeSearchForPartD(vm); };
        vm.removeAdditionalCopyToFromPartD = function (index) { PartDService.removeAdditionalCopyToFromPartD(vm, index); };
        vm.cancelPartD = function () { PartDService.cancelPartD(); };
        vm.getHsbuName = function (hsbuId) { return PartDService.getHsbuName(vm, hsbuId); };

        init();

        function init() {
            var incidentId = getIncidentIdFromUrl();
            if (!incidentId) {
                vm.error = 'No incident ID provided';
                vm.loading = false;
                return;
            }

            loadCurrentUser()
                .then(function () {
                    return loadIncident(incidentId);
                })
                .then(function () {
                    return PartAService.loadPartAData(vm);
                })
                .then(function () {
                    return PartBService.loadPartBData(vm);
                })
                .then(function () {
                    return PartCService.loadPartCData(vm);
                })
                .then(function () {
                    return PartDService.loadPartDData(vm, getCurrentDate);
                })
                .catch(function (error) {
                    vm.error = error || 'Failed to load incident data';
                })
                .finally(function () {
                    vm.loading = false;
                });
        }

        function getIncidentIdFromUrl() {
            var url = $window.location.href;
            var params = new URLSearchParams($window.location.search);
            return params.get('id');
        }

        function loadCurrentUser() {
            return IncidentUpdateService.getCurrentUser()
                .then(function (user) {
                    vm.currentUser = {
                        userId: user.userId,
                        name: user.userName || user.displayName || 'User',
                        designation: user.designation || '',
                        sectorCode: user.sectorCode || ''
                    };
                });
        }

        function loadIncident(incidentId) {
            return IncidentUpdateService.getIncidentById(incidentId)
                .then(function (incident) {
                    console.log('Loaded incident data:', incident);
                    console.log('IncidentTypes array:', incident.incidentTypes);
                    console.log('IncidentTypes length:', incident.incidentTypes ? incident.incidentTypes.length : 'undefined');
                    vm.incident = incident;
                    return loadStatusName();
                })
                .then(function () {

                });
        }

        function loadStatusName() {
            if (!vm.incident.status) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getStatusName(vm.incident.status)
                .then(function (statusName) {
                    vm.incident.statusName = statusName;
                    console.log('Status name loaded:', statusName);
                })
                .catch(function (error) {
                    console.error('Error loading status name:', error);
                    vm.incident.statusName = vm.incident.status;
                });
        }

        function getStatusClass() {
            var status = parseInt(vm.incident.status);
            if (status === 0) return 'bg-gray-200 text-gray-800';
            if (status >= 1 && status <= 3) return 'bg-yellow-200 text-yellow-800';
            if (status >= 4 && status <= 7) return 'bg-blue-200 text-blue-800';
            if (status === 8) return 'bg-green-200 text-green-800';
            return 'bg-gray-200 text-gray-800';
        }

        function getIncidentTypeText() {
            if (!vm.incident.incidentTypes || vm.incident.incidentTypes.length === 0) {
                return 'N/A';
            }
            return vm.incident.incidentTypes.join(', ');
        }

        function getCurrentDate() {
            var today = new Date();
            var day = String(today.getDate()).padStart(2, '0');
            var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var month = monthNames[today.getMonth()];
            var year = today.getFullYear();
            return day + '-' + month + '-' + year;
        }

        function cancel() {
            if (confirm('Are you sure you want to cancel? Any unsaved changes will be lost.')) {
                $window.location.href = '/Home/Index';
            }
        }

        function getWorkflowsByAction(actionCode) {
            if (!vm.incident || !vm.incident.workflows) {
                return [];
            }
            return vm.incident.workflows.filter(function (workflow) {

                if (workflow.toDesignation && typeof workflow.toDesignation === 'string') {
                    workflow.toDesignation = $sce.trustAsHtml(workflow.toDesignation);
                }
                if (workflow.toName && typeof workflow.toName === 'string') {
                    workflow.toName = $sce.trustAsHtml(workflow.toName);
                }

                return workflow.actionCode === actionCode;
            });
        }
    }
})();