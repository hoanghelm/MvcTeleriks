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
                isReadOnly: false,
                comments: '',
                wshoId: '',
                submitterName: '',
                submitterEmpId: '',
                submitterDesignation: '',
                submissionDate: '',
                additionalCopyToList: [],
                copyToPerson: {},
                validationMessage: '',
                successMessage: '',
                isSubmitting: false,
                wshoOptions: {
                    dataTextField: 'name',
                    dataValueField: 'id',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select WSHO --',
                    valuePrimitive: true
                }
            };

            vm.wshoListPartH = [];
            vm.emailToListPartH = [];
        }

        function loadPartHData(vm, getCurrentDate) {
            if (!canViewPartH(vm)) {
                return $q.resolve();
            }

            determinePartHMode(vm);

            if (vm.partH.isReadOnly) {
                loadPartHWorkflowData(vm);
            }

            return $q.all([
                loadWSHOs(vm),
                loadPartHCopyToList(vm)
            ]).then(function () {
                if (!vm.partH.isReadOnly) {
                    $timeout(function () {
                        refreshKendoDropDowns(vm);
                    }, 100);
                }
            });
        }

        function determinePartHMode(vm) {
            var status = parseInt(vm.incident.status);
            if (status < 7) {
                vm.partH.isReadOnly = true;
                vm.partH.comments = '';
                vm.partH.wshoId = '';
                vm.partH.submitterName = '';
                vm.partH.submitterEmpId = '';
                vm.partH.submitterDesignation = '';
                vm.partH.submissionDate = '';
            } else if (status > 7) {
                vm.partH.isReadOnly = true;
            }
        }

        function canViewPartH(vm) {
            return vm.canUserViewPart('07');
        }

        function canEditPartH(vm) {
            return vm.canUserEditPart('07');
        }

        function loadPartHWorkflowData(vm) {
            var partHWorkflows = [];
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                partHWorkflows = vm.incident.workflows.filter(function (w) {
                    return w.actionCode === '08' || w.actionCode === '06';
                });
            }

            if (partHWorkflows.length > 0) {
                partHWorkflows.sort(function (a, b) {
                    var dateA = new Date(a.date || 0);
                    var dateB = new Date(b.date || 0);
                    return dateB - dateA;
                });

                var latestWorkflow = partHWorkflows[0];
                vm.partH.comments = latestWorkflow.remarks || '';
                vm.partH.submitterName = latestWorkflow.fromName || '';
                vm.partH.submitterEmpId = latestWorkflow.from || '';
                vm.partH.submitterDesignation = latestWorkflow.fromDesignation || '';
                vm.partH.submissionDate = latestWorkflow.date || '';
            }
        }

        function loadWSHOs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
            }

            return IncidentUpdateService.getWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department || '',
                vm.incident.location || ''
            ).then(function (data) {
                vm.wshoListPartH = data;
                vm.partH.wshoOptions.dataSource.data(data);
                if (data.length > 0 && vm.incident.wshoId) {
                    vm.partH.wshoId = vm.incident.wshoId;
                }
            }).catch(function (error) {
                vm.wshoListPartH = [];
                vm.partH.wshoOptions.dataSource.data([]);
            });
        }

        function loadPartHCopyToList(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
            }

            return IncidentUpdateService.getPartECopyToList(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            ).then(function (data) {
                vm.emailToListPartH = data.map(function (person) {
                    return {
                        id: person.id,
                        name: person.name,
                        designation: person.designation || '',
                        selected: true
                    };
                });
            }).catch(function (error) {
            });
        }

        function refreshKendoDropDowns(vm) {
            function refreshDropDown(elementId, dataSource, value) {
                var widget = $('#' + elementId).data('kendoDropDownList');
                if (widget && value) {
                    if (dataSource && dataSource.length > 0) {
                        widget.setDataSource(new kendo.data.DataSource({
                            data: dataSource
                        }));
                        widget.value(value);
                    }
                }
            }

            if (vm.wshoListPartH && vm.wshoListPartH.length > 0) {
                refreshDropDown('partH_wsho', vm.wshoListPartH, vm.partH.wshoId);
            }
        }

        function revertPartHToWSHO(vm) {
            vm.partH.validationMessage = '';
            vm.partH.successMessage = '';

            if (!vm.partH.comments || vm.partH.comments.trim() === '') {
                vm.partH.validationMessage = 'Review & Comment is required (ERR-137)';
                return;
            }

            if (!vm.partH.wshoId) {
                vm.partH.validationMessage = 'Name of WSHO is required (ERR-133)';
                return;
            }

            if (!confirm('Are you sure you want to revert to WSHO? This action cannot be undone.')) {
                return;
            }

            vm.partH.isSubmitting = true;

            var selectedEmailTo = vm.emailToListPartH
                .filter(function (person) { return person.selected; })
                .map(function (person) { return person.id; });

            var revertData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partH.comments,
                wshoId: vm.partH.wshoId,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partH.additionalCopyToList
            };

            IncidentUpdateService.revertPartHToWSHO(revertData)
                .then(function (response) {
                    if (response.success) {
                        vm.partH.successMessage = response.message || 'Part H reverted successfully to WSHO';
                        setTimeout(function () {
                            $window.location.href = (window.WIRS_CONFIG ? window.WIRS_CONFIG.basePath : '') + '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partH.validationMessage = response.message || 'Failed to revert Part H';
                    }
                })
                .catch(function (error) {
                    vm.partH.validationMessage = error.message || 'An error occurred while reverting Part H';
                })
                .finally(function () {
                    vm.partH.isSubmitting = false;
                });
        }

        function closeReport(vm) {
            vm.partH.validationMessage = '';
            vm.partH.successMessage = '';

            if (!vm.partH.comments || vm.partH.comments.trim() === '') {
                vm.partH.validationMessage = 'Review & Comment is required (ERR-137)';
                return;
            }

            if (!confirm('Are you sure you want to close the report? This action cannot be undone.')) {
                return;
            }

            vm.partH.isSubmitting = true;

            var selectedEmailTo = vm.emailToListPartH
                .filter(function (person) { return person.selected; })
                .map(function (person) { return person.id; });

            var submitData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partH.comments,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partH.additionalCopyToList
            };

            IncidentUpdateService.closeReport(submitData)
                .then(function (response) {
                    if (response.success) {
                        vm.partH.successMessage = response.message || 'Report closed successfully';
                        setTimeout(function () {
                            $window.location.href = (window.WIRS_CONFIG ? window.WIRS_CONFIG.basePath : '') + '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partH.validationMessage = response.message || 'Failed to close report';
                    }
                })
                .catch(function (error) {
                    vm.partH.validationMessage = error.message || 'An error occurred while closing report';
                })
                .finally(function () {
                    vm.partH.isSubmitting = false;
                });
        }

        function openEmployeeSearch(vm, context) {
            if (typeof window.openEmployeeSearch === 'function') {
                window.openEmployeeSearch('partH', function(employee) {
                    if (context === 'copyTo') {
                        $timeout(function() {
                            vm.partH.copyToPerson = {
                                name: employee.name,
                                employeeNo: employee.empId,
                                designation: employee.designation
                            };
                        });
                    }
                });
            }
        }

        function addPartHCopyTo(vm) {
            if (!vm.partH.copyToPerson) {
                vm.partH.copyToPerson = {};
            }

            if (!vm.partH.copyToPerson.name || !vm.partH.copyToPerson.employeeNo) {
                alert('Please enter name and employee number');
                return;
            }

            if (!vm.partH.additionalCopyToList) {
                vm.partH.additionalCopyToList = [];
            }

            var exists = vm.partH.additionalCopyToList.some(function(p) {
                return p.employeeNo === vm.partH.copyToPerson.employeeNo;
            });

            if (exists) {
                alert('This person is already in the copy to list');
                return;
            }

            vm.partH.additionalCopyToList.push({
                name: vm.partH.copyToPerson.name,
                employeeNo: vm.partH.copyToPerson.employeeNo,
                designation: vm.partH.copyToPerson.designation || ''
            });

            vm.partH.copyToPerson = {};
        }

        function removePartHCopyToPerson(vm, index) {
            if (vm.partH.additionalCopyToList && index >= 0 && index < vm.partH.additionalCopyToList.length) {
                vm.partH.additionalCopyToList.splice(index, 1);
            }
        }
    }
})();
