(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartBService', PartBService);

    PartBService.$inject = ['$window', '$timeout', '$q', 'IncidentUpdateService'];

    function PartBService($window, $timeout, $q, IncidentUpdateService) {
        var service = {
            initializePartB: initializePartB,
            loadPartBData: loadPartBData,
            canViewPartB: canViewPartB,
            canEditPartB: canEditPartB,
            submitPartB: submitPartB,
            closePartB: closePartB,
            openEmployeeSearch: openEmployeeSearch,
            addPartBCopyTo: addPartBCopyTo,
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
                copyToPerson: {},
                validationMessage: '',
                submitting: false,
                wshoOptions: {
                    dataTextField: 'name',
                    dataValueField: 'id',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select WSHO --',
                    valuePrimitive: true
                },
                alternateWshoOptions: {
                    dataTextField: 'name',
                    dataValueField: 'id',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select Alternate WSHO --',
                    valuePrimitive: true
                }
            };

            vm.injuredCaseTypes = [];
            vm.wshoList = [];
            vm.alternateWshoList = [];
            vm.emailToList = [];
        }

        function loadPartBData(vm) {
            if (!canViewPartB(vm)) {
                return $q.resolve();
            }

            return $q.all([
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
                });
        }

        function loadWSHOs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
            }

            return IncidentUpdateService.getWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function (data) {
                    vm.wshoList = data;
                    vm.partB.wshoOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    vm.partB.wshoOptions.dataSource.data([]);
                });
        }

        function loadAlternateWSHOs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
            }

            return IncidentUpdateService.getAlternateWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function (data) {
                    vm.alternateWshoList = data;
                    vm.partB.alternateWshoOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    vm.partB.alternateWshoOptions.dataSource.data([]);
                });
        }

        function loadEmailToList(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
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
                });
        }

        function determinePartBMode(vm) {
            if (vm.incident.status === '01') {
                vm.partB.isReadOnly = false;
                // In edit mode, reset to use current user data
                vm.partB.reviewComment = '';
                vm.partB.submitterName = '';
                vm.partB.submitterEmpId = '';
                vm.partB.submitterDesignation = '';
                vm.partB.submissionDate = '';
            } else if (parseInt(vm.incident.status) > 1) {
                vm.partB.isReadOnly = true;
                loadPartBReadOnlyData(vm);
            }
        }

        function loadPartBReadOnlyData(vm) {
            // Get workflows with actionCode "02" (Part B)
            var partBWorkflows = [];
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                partBWorkflows = vm.incident.workflows.filter(function (w) {
                    return w.actionCode === '02';
                });
            }

            // Sort by date descending and get the latest one
            if (partBWorkflows.length > 0) {
                partBWorkflows.sort(function (a, b) {
                    var dateA = new Date(a.date || 0);
                    var dateB = new Date(b.date || 0);
                    return dateB - dateA; // Descending order
                });

                var latestWorkflow = partBWorkflows[0];
                vm.partB.reviewComment = latestWorkflow.remarks || '';
                vm.partB.submitterName = latestWorkflow.fromName || '';
                vm.partB.submitterEmpId = latestWorkflow.from || '';
                vm.partB.submitterDesignation = latestWorkflow.fromDesignation || '';
                vm.partB.submissionDate = latestWorkflow.date || '';
            }

            vm.partB.injuredCaseType = vm.incident.injuredCaseType || '';
        }

        function canViewPartB(vm) {
            return vm.canUserViewPart('01');
        }

        function canEditPartB(vm) {
            return vm.canUserEditPart('01');
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
            if (typeof window.openEmployeeSearch === 'function') {
                window.openEmployeeSearch('partB', function(employee) {
                    if (context === 'copyTo') {
                        $timeout(function() {
                            vm.partB.copyToPerson = {
                                name: employee.name,
                                employeeNo: employee.empId,
                                designation: employee.designation
                            };
                        });
                    }
                });
            }
        }

        function removeCopyToPerson(vm, index) {
            vm.partB.additionalCopyToList.splice(index, 1);
        }

        function addPartBCopyTo(vm) {
            if (!vm.partB.copyToPerson) {
                vm.partB.copyToPerson = {};
            }

            if (!vm.partB.copyToPerson.name || !vm.partB.copyToPerson.employeeNo) {
                alert('Please enter name and employee number');
                return;
            }

            if (!vm.partB.additionalCopyToList) {
                vm.partB.additionalCopyToList = [];
            }

            var exists = vm.partB.additionalCopyToList.some(function(p) {
                return p.employeeNo === vm.partB.copyToPerson.employeeNo;
            });

            if (exists) {
                alert('This person has already been added');
                return;
            }

            vm.partB.additionalCopyToList.push({
                name: vm.partB.copyToPerson.name,
                employeeNo: vm.partB.copyToPerson.employeeNo,
                designation: vm.partB.copyToPerson.designation || ''
            });

            vm.partB.copyToPerson = {};
        }

        function closePartB(vm) {
            if (!confirm('Are you sure you want to reject this incident report?')) {
                return;
            }

            vm.partB.validationMessage = '';

            // Validate Review and Comment
            if (!vm.partB.reviewComment || !vm.partB.reviewComment.trim()) {
                vm.partB.validationMessage = 'Review and Comment is required (ERR-134)';
                return;
            }

            // Validate WSHO selection
            if (!vm.partB.wshoId) {
                vm.partB.validationMessage = 'WSHO selection is required (ERR-135)';
                return;
            }

            vm.partB.submitting = true;

            var selectedEmailTo = vm.emailToList
                .filter(function (person) { return person.selected; })
                .map(function (person) { return person.id; });

            var closeData = {
                incidentId: vm.incident.incidentId,
                injuredCaseType: vm.partB.injuredCaseType,
                reviewComment: vm.partB.reviewComment,
                wshoId: vm.partB.wshoId,
                alternateWshoId: vm.partB.alternateWshoId || '',
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partB.additionalCopyToList.map(function (person) {
                    return person.employeeNo;
                })
            };

            IncidentUpdateService.closePartB(closeData)
                .then(function (response) {
                    if (response.success) {
                        alert('Part B rejected successfully. Incident report has been closed.');
                        $window.location.href = '/Home/Index';
                    } else {
                        vm.partB.validationMessage = response.message || 'Failed to close Part B';
                    }
                })
                .catch(function (error) {
                    vm.partB.validationMessage = error.message || 'An error occurred while closing Part B';
                })
                .finally(function () {
                    vm.partB.submitting = false;
                });
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