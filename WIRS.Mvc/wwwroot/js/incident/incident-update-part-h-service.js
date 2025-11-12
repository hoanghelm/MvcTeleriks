(function () {
    'use strict';

    angular.module('incidentUpdateApp').factory('PartHService', PartHService);

    PartHService.$inject = ['$window', '$timeout', '$q', 'IncidentUpdateService'];

    function PartHService($window, $timeout, $q, IncidentUpdateService) {
        var service = {
            initializePartH: initializePartH,
            loadPartHData: loadPartHData,
            canViewPartH: canViewPartH,
            canEditPartH: canEditPartH,
            revertPartHToWSHO: revertPartHToWSHO,
            closeReport: closeReport,
            openEmployeeSearch: openEmployeeSearch,
            addPartHCopyTo: addPartHCopyTo,
            removePartHCopyToPerson: removePartHCopyToPerson
        };

        return service;

        function initializePartH(vm) {
            vm.partH = {
                comments: '',
                submitterName: '',
                submitterEmpId: '',
                submittedDate: '',
                submitterDesignation: '',
                wshoId: '',
                wshoList: [],
                isReadOnly: false,
                isSubmitting: false,
                workflowHistory: [],
                copyTo: {
                    name: '',
                    employeeNo: '',
                    designation: ''
                },
                additionalCopyToList: []
            };

            vm.emailToListPartH = [];
        }

        function loadPartHData(vm, getCurrentDate) {
            var deferred = $q.defer();

            determinePartHMode(vm);

            if (vm.partH.isReadOnly) {
                loadPartHWorkflowData(vm);
            }

            loadWSHOs(vm).then(function () {
                return loadPartHCopyToList(vm);
            }).then(function () {
                deferred.resolve();
            }).catch(function (error) {
                deferred.reject(error);
            });

            return deferred.promise;
        }

        function determinePartHMode(vm) {
            var status = vm.incident.status;
            vm.partH.isReadOnly = (status !== '07');
        }

        function canViewPartH(vm) {
            var status = vm.incident.status;
            return status === '07' || status === '08';
        }

        function canEditPartH(vm) {
            var status = vm.incident.status;
            return status === '07';
        }

        function loadPartHWorkflowData(vm) {
            var partHWorkflows = vm.incident.workflows.filter(function (w) {
                return w.actionCode === '08' || w.actionCode === '06';
            });

            partHWorkflows.sort(function (a, b) {
                return new Date(b.date) - new Date(a.date);
            });

            if (partHWorkflows.length > 0) {
                var latestWorkflow = partHWorkflows[0];
                vm.partH.comments = latestWorkflow.remarks || '';
                vm.partH.submitterName = latestWorkflow.fromName || '';
                vm.partH.submitterEmpId = latestWorkflow.from || '';
                vm.partH.submittedDate = latestWorkflow.date || '';
                vm.partH.submitterDesignation = latestWorkflow.fromDesignation || '';
            }

            vm.partH.workflowHistory = partHWorkflows.filter(function (w) {
                return w.actionCode === '08' || w.actionCode === '06';
            });
        }

        function loadWSHOs(vm) {
            var deferred = $q.defer();

            IncidentUpdateService.getWSHOs(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode,
                vm.incident.locationCode
            ).then(function (data) {
                vm.partH.wshoList = data || [];
                deferred.resolve();
            }).catch(function (error) {
                deferred.reject('Failed to load WSHOs: ' + error);
            });

            return deferred.promise;
        }

        function loadPartHCopyToList(vm) {
            var deferred = $q.defer();

            IncidentUpdateService.getPartECopyToList(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode,
                vm.incident.locationCode
            ).then(function (data) {
                vm.emailToListPartH = (data || []).map(function (person) {
                    return {
                        userId: person.userId,
                        name: person.name,
                        selected: false
                    };
                });
                deferred.resolve();
            }).catch(function (error) {
                deferred.reject('Failed to load copy to list: ' + error);
            });

            return deferred.promise;
        }

        function revertPartHToWSHO(vm) {
            if (vm.partH.isSubmitting) {
                return;
            }

            if (!vm.partH.comments || vm.partH.comments.trim() === '') {
                $window.alert('Review & Comment is required (ERR-137)');
                return;
            }

            if (!vm.partH.wshoId) {
                $window.alert('Name of WSHO is required (ERR-133)');
                return;
            }

            vm.partH.isSubmitting = true;

            var selectedEmailTo = vm.emailToListPartH.filter(function (p) {
                return p.selected;
            }).map(function (p) {
                return p.userId;
            });

            var submitData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partH.comments,
                wshoId: vm.partH.wshoId,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partH.additionalCopyToList.map(function (p) {
                    return {
                        employeeNo: p.employeeNo,
                        name: p.name,
                        designation: p.designation
                    };
                })
            };

            IncidentUpdateService.revertPartHToWSHO(submitData).then(function (response) {
                if (response.success) {
                    $window.alert(response.message || 'Part H reverted successfully to WSHO');
                    $window.location.href = '/Incident/Update?id=' + vm.incident.incidentId;
                } else {
                    $window.alert(response.message || 'Failed to revert Part H');
                    vm.partH.isSubmitting = false;
                }
            }).catch(function (error) {
                $window.alert('Error reverting Part H: ' + (error.message || 'Unknown error'));
                vm.partH.isSubmitting = false;
            });
        }

        function closeReport(vm) {
            if (vm.partH.isSubmitting) {
                return;
            }

            if (!vm.partH.comments || vm.partH.comments.trim() === '') {
                $window.alert('Review & Comment is required (ERR-137)');
                return;
            }

            vm.partH.isSubmitting = true;

            var selectedEmailTo = vm.emailToListPartH.filter(function (p) {
                return p.selected;
            }).map(function (p) {
                return p.userId;
            });

            var submitData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partH.comments,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partH.additionalCopyToList.map(function (p) {
                    return {
                        employeeNo: p.employeeNo,
                        name: p.name,
                        designation: p.designation
                    };
                })
            };

            IncidentUpdateService.closeReport(submitData).then(function (response) {
                if (response.success) {
                    $window.alert(response.message || 'Report closed successfully');
                    $window.location.href = '/Incident/Update?id=' + vm.incident.incidentId;
                } else {
                    $window.alert(response.message || 'Failed to close report');
                    vm.partH.isSubmitting = false;
                }
            }).catch(function (error) {
                $window.alert('Error closing report: ' + (error.message || 'Unknown error'));
                vm.partH.isSubmitting = false;
            });
        }

        function openEmployeeSearch(vm, context) {
            vm.partH.searchContext = context;
            if (window.employeeSearchInstance) {
                window.employeeSearchInstance.open();
            }
        }

        function addPartHCopyTo(vm) {
            if (!vm.partH.copyTo.name || !vm.partH.copyTo.employeeNo) {
                $window.alert('Please search and select an employee first');
                return;
            }

            var exists = vm.partH.additionalCopyToList.some(function (p) {
                return p.employeeNo === vm.partH.copyTo.employeeNo;
            });

            if (exists) {
                $window.alert('This employee is already in the copy to list');
                return;
            }

            vm.partH.additionalCopyToList.push({
                name: vm.partH.copyTo.name,
                employeeNo: vm.partH.copyTo.employeeNo,
                designation: vm.partH.copyTo.designation
            });

            vm.partH.copyTo = {
                name: '',
                employeeNo: '',
                designation: ''
            };
        }

        function removePartHCopyToPerson(vm, index) {
            vm.partH.additionalCopyToList.splice(index, 1);
        }
    }
})();
