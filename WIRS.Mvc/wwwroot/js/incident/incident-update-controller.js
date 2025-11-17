(function () {
    angular
        .module('incidentUpdateApp')
        .controller('IncidentUpdateController', IncidentUpdateController);

    IncidentUpdateController.$inject = ['$window', '$location', '$scope', '$timeout', '$sce', '$q', 'IncidentUpdateService', 'PartAService', 'PartBService', 'PartCService', 'PartDService', 'PartEService', 'PartFService', 'PartGService', 'PartHService'];

    function IncidentUpdateController($window, $location, $scope, $timeout, $sce, $q, IncidentUpdateService, PartAService, PartBService, PartCService, PartDService, PartEService, PartFService, PartGService, PartHService) {
        var vm = this;

        vm.loading = true;
        vm.error = null;
        vm.incident = {};
        vm.currentUser = {};
        vm.activeTab = 'A';

        PartBService.initializePartB(vm);

        PartCService.initializePartC(vm);

        PartDService.initializePartD(vm);

        PartEService.initializePartE(vm);

        PartFService.initializePartF(vm);

        PartGService.initializePartG(vm);

        PartHService.initializePartH(vm);

        PartAService.initializePartA(vm);

        vm.getStatusClass = getStatusClass;
        vm.getIncidentTypeText = getIncidentTypeText;
        vm.getCurrentDate = getCurrentDate;
        vm.getWorkflowsByAction = getWorkflowsByAction;
        vm.isWorkflowClosed = isWorkflowClosed;
        vm.getHighestCompletedActionCode = getHighestCompletedActionCode;
        vm.cancel = cancel;
        vm.canUserViewPart = canUserViewPart;
        vm.canUserEditPart = canUserEditPart;

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
        vm.closePartB = function () { PartBService.closePartB(vm); };
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
        vm.openPartCEmployeeSearch = function (context) { PartCService.openEmployeeSearch(vm, context); };

        vm.canViewPartD = function () { return PartDService.canViewPartD(vm); };
        vm.canEditPartD = function () { return PartDService.canEditPartD(vm); };
        vm.revertToWSHO = function () { PartDService.revertToWSHO(vm); };
        vm.submitToHeadLOB = function () { PartDService.submitToHeadLOB(vm); };

        vm.canViewPartE = function () { return PartEService.canViewPartE(vm); };
        vm.canEditPartE = function () { return PartEService.canEditPartE(vm); };
        vm.revertPartEToWSHO = function () { PartEService.revertPartEToWSHO(vm); };
        vm.approveAndSubmitPartE = function () { PartEService.approveAndSubmitPartE(vm); };
        vm.searchPartECopyTo = function () { PartEService.openEmployeeSearch(vm, 'copyTo'); };
        vm.addPartECopyTo = function () { PartEService.addPartECopyTo(vm); };
        vm.removePartECopyToPerson = function (index) { PartEService.removePartECopyToPerson(vm, index); };

        vm.canViewPartF = function () { return PartFService.canViewPartF(vm); };
        vm.canEditPartF = function () { return PartFService.canEditPartF(vm); };
        vm.submitPartF = function () { PartFService.submitPartF(vm); };
        vm.removePartFAttachment = function (index) { PartFService.removePartFAttachment(vm, index); };
        vm.removePartFRiskAttachment = function (index) { PartFService.removePartFRiskAttachment(vm, index); };

        vm.canViewPartG = function () { return PartGService.canViewPartG(vm); };
        vm.canEditPartG = function () { return PartGService.canEditPartG(vm); };
        vm.submitPartG = function () { PartGService.submitPartG(vm); };
        vm.revertPartGToHOD = function () { PartGService.revertPartGToHOD(vm); };
        vm.removePartGAttachment = function (index) { PartGService.removePartGAttachment(vm, index); };

        vm.canViewPartH = function () { return PartHService.canViewPartH(vm); };
        vm.canEditPartH = function () { return PartHService.canEditPartH(vm); };
        vm.revertPartHToWSHO = function () { PartHService.revertPartHToWSHO(vm); };
        vm.closeReport = function () { PartHService.closeReport(vm); };
        vm.searchPartHCopyTo = function () { PartHService.openEmployeeSearch(vm, 'copyTo'); };
        vm.addPartHCopyTo = function () { PartHService.addPartHCopyTo(vm); };
        vm.removePartHCopyToPerson = function (index) { PartHService.removePartHCopyToPerson(vm, index); };

        init();

        function init() {
            var incidentId = getIncidentIdFromUrl();
            if (!incidentId) {
                vm.error = 'No incident ID provided';
                vm.loading = false;
                return;
            }

            $q.all([
                loadCurrentUser(),
                loadIncident(incidentId)
            ])
                .then(function () {
                    return $q.all([
                        PartAService.loadPartAData(vm),
                        PartBService.loadPartBData(vm),
                        PartCService.loadPartCData(vm),
                        PartDService.loadPartDData(vm, getCurrentDate),
                        PartEService.loadPartEData(vm, getCurrentDate),
                        PartFService.loadPartFData(vm, getCurrentDate),
                        PartGService.loadPartGData(vm, getCurrentDate),
                        PartHService.loadPartHData(vm, getCurrentDate)
                    ]);
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
                })
                .catch(function (error) {
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

        function isWorkflowClosed() {
            if (!vm.incident || !vm.incident.workflows) {
                return false;
            }
            return vm.incident.workflows.some(function (workflow) {
                return workflow.actionCode === '08';
            });
        }

        function getHighestCompletedActionCode() {
            if (!vm.incident || !vm.incident.workflows) {
                return '00';
            }

            var hasClosure = vm.incident.workflows.some(function (w) { return w.actionCode === '08'; });

            if (!hasClosure) {
                var actionCodes = vm.incident.workflows
                    .map(function (w) { return w.actionCode; })
                    .filter(function (code) { return code && code !== '08'; });

                if (actionCodes.length === 0) return '00';

                return actionCodes.reduce(function (max, code) {
                    return (parseInt(code) || 0) > (parseInt(max) || 0) ? code : max;
                }, '00');
            }

            var completedCodes = vm.incident.workflows
                .map(function (w) { return w.actionCode; })
                .filter(function (code) { return code && code !== '08'; });

            if (completedCodes.length === 0) return '00';

            return completedCodes.reduce(function (max, code) {
                return (parseInt(code) || 0) > (parseInt(max) || 0) ? code : max;
            }, '00');
        }

        function canUserViewPart(minStatus) {
            if (!vm.incident || !vm.currentUser) {
                return false;
            }

            if (!vm.incident.status || parseInt(vm.incident.status) < parseInt(minStatus)) {
                return false;
            }

            if (vm.incident.createdBy === vm.currentUser.userId) {
                return true;
            }

            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                var userInWorkflow = vm.incident.workflows.some(function (workflow) {
                    var toUsers = workflow.to || '';
                    return toUsers.indexOf(vm.currentUser.userId) !== -1;
                });
                if (userInWorkflow) {
                    return true;
                }
            }

            return false;
        }

        function canUserEditPart(requiredStatus) {
            if (!vm.incident || !vm.currentUser) {
                return false;
            }

            if (vm.incident.status !== requiredStatus) {
                return false;
            }

            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                var canEdit = vm.incident.workflows.some(function (workflow) {
                    var toUsers = workflow.to || '';
                    var actionCode = workflow.actionCode || '';
                    return actionCode === vm.incident.status && toUsers.indexOf(vm.currentUser.userId) !== -1;
                });
                return canEdit;
            }

            return false;
        }
    }
})();
