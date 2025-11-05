(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartBService', PartBService);

    PartBService.$inject = ['$window', 'IncidentUpdateService'];

    function PartBService($window, IncidentUpdateService) {
        var service = {
            initializePartB: initializePartB,
            loadPartBData: loadPartBData,
            canViewPartB: canViewPartB,
            canEditPartB: canEditPartB,
            submitPartB: submitPartB,
            openEmployeeSearch: openEmployeeSearch,
            removeCopyToPerson: removeCopyToPerson,
            getInjuredCaseTypeText: getInjuredCaseTypeText
        };

        return service;

        function initializePartB(vm) {
            vm.partB = {
                isReadOnly: false,
                injuredCaseType: '',
                reviewComment: '',
                wshoId: '',
                alternateWshoId: '',
                additionalCopyToList: [],
                validationMessage: '',
                submitting: false
            };

            vm.injuredCaseTypes = [];
            vm.wshoList = [];
            vm.alternateWshoList = [];
            vm.emailToList = [];
        }

        function loadPartBData(vm) {
            if (!canViewPartB(vm)) {
                return Promise.resolve();
            }

            return Promise.all([
                loadInjuredCaseTypes(vm),
                loadWSHOs(vm),
                loadAlternateWSHOs(vm),
                loadEmailToList(vm)
            ]).then(function () {
                determinePartBMode(vm);
            });
        }

        function loadInjuredCaseTypes(vm) {
            return IncidentUpdateService.getInjuredCaseTypes()
                .then(function (data) {
                    vm.injuredCaseTypes = data;
                    if (vm.injuredCaseTypes.length > 0 && !vm.partB.injuredCaseType) {
                        vm.partB.injuredCaseType = vm.injuredCaseTypes[0].code;
                    }
                })
                .catch(function (error) {
                    console.error('Failed to load injured case types:', error);
                });
        }

        function loadWSHOs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function (data) {
                    vm.wshoList = data;
                })
                .catch(function (error) {
                    console.error('Failed to load WSHOs:', error);
                });
        }

        function loadAlternateWSHOs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getAlternateWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function (data) {
                    vm.alternateWshoList = data;
                })
                .catch(function (error) {
                    console.error('Failed to load alternate WSHOs:', error);
                });
        }

        function loadEmailToList(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getPartBCopyToList(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function (data) {
                    vm.emailToList = data.map(function (person) {
                        return {
                            id: person.id,
                            name: person.name,
                            designation: person.designation || '',
                            selected: true
                        };
                    });
                })
                .catch(function (error) {
                    console.error('Failed to load email to list:', error);
                });
        }

        function determinePartBMode(vm) {
            if (vm.incident.status === '01') {
                vm.partB.isReadOnly = false;
            } else if (parseInt(vm.incident.status) > 1) {
                vm.partB.isReadOnly = true;
                loadPartBReadOnlyData(vm);
            }
        }

        function loadPartBReadOnlyData(vm) {
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                var partBWorkflow = vm.incident.workflows.find(function (w) {
                    return w.status === '02' && w.role === 'HOD';
                });

                if (partBWorkflow) {
                    vm.partB.reviewComment = partBWorkflow.comments || '';
                    vm.partB.submitterName = partBWorkflow.submitterName || '';
                    vm.partB.submitterEmpId = partBWorkflow.submitterEmpId || '';
                    vm.partB.submitterDesignation = partBWorkflow.submitterDesignation || '';
                    vm.partB.submissionDate = partBWorkflow.submissionDate || '';
                }
            }

            vm.partB.injuredCaseType = vm.incident.injuredCaseType || '';
        }

        function canViewPartB(vm) {
            return vm.incident.status && parseInt(vm.incident.status) >= 1;
        }

        function canEditPartB(vm) {
            if (!vm.incident || !vm.currentUser) {
                return false;
            }

            if (vm.incident.status !== '01') {
                return false;
            }

            if (vm.incident.hodId === vm.currentUser.userId) {
                return true;
            }

            if (vm.incident.ahodId === vm.currentUser.userId) {
                return true;
            }

            return false;
        }

        function submitPartB(vm) {
            vm.partB.validationMessage = '';

            if (!vm.partB.reviewComment) {
                vm.partB.validationMessage = 'Review and Comment is required (ERR-134)';
                return;
            }

            if (!vm.partB.wshoId) {
                vm.partB.validationMessage = 'WSHO selection is required (ERR-135)';
                return;
            }

            vm.partB.submitting = true;

            var selectedEmailTo = vm.emailToList
                .filter(function (person) { return person.selected; })
                .map(function (person) { return person.id; });

            var submitData = {
                incidentId: vm.incident.incidentId,
                injuredCaseType: vm.partB.injuredCaseType,
                reviewComment: vm.partB.reviewComment,
                wshoId: vm.partB.wshoId,
                alternateWshoId: vm.partB.alternateWshoId || '',
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partB.additionalCopyToList
            };

            IncidentUpdateService.submitPartB(submitData)
                .then(function (response) {
                    if (response.success) {
                        alert('Part B submitted successfully!');
                        $window.location.href = '/Home/Index';
                    } else {
                        vm.partB.validationMessage = response.message || 'Failed to submit Part B';
                    }
                })
                .catch(function (error) {
                    vm.partB.validationMessage = error.message || 'An error occurred while submitting Part B';
                })
                .finally(function () {
                    vm.partB.submitting = false;
                });
        }

        function openEmployeeSearch(vm, context) {
            console.log('Employee search not yet implemented');
        }

        function removeCopyToPerson(vm, index) {
            vm.partB.additionalCopyToList.splice(index, 1);
        }

        function getInjuredCaseTypeText(vm) {
            if (!vm.partB.injuredCaseType || !vm.injuredCaseTypes.length) {
                return 'N/A';
            }

            var caseType = vm.injuredCaseTypes.find(function (ct) {
                return ct.code === vm.partB.injuredCaseType;
            });

            return caseType ? caseType.value : vm.partB.injuredCaseType;
        }
    }
})();