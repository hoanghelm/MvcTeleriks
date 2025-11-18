(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartEService', PartEService);

    PartEService.$inject = ['$window', '$timeout', '$q', 'IncidentUpdateService'];

    function PartEService($window, $timeout, $q, IncidentUpdateService) {
        var service = {
            initializePartE: initializePartE,
            loadPartEData: loadPartEData,
            canViewPartE: canViewPartE,
            canEditPartE: canEditPartE,
            revertPartEToWSHO: revertPartEToWSHO,
            approveAndSubmitPartE: approveAndSubmitPartE,
            openEmployeeSearch: openEmployeeSearch,
            addPartECopyTo: addPartECopyTo,
            removePartECopyToPerson: removePartECopyToPerson
        };

        return service;

        function initializePartE(vm) {
            vm.partE = {
                isReadOnly: false,
                comments: '',
                wshoId: '',
                hodId: '',
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
                },
                hodOptions: {
                    dataTextField: 'name',
                    dataValueField: 'id',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select HOD --',
                    valuePrimitive: true
                }
            };

            vm.wshoListPartE = [];
            vm.hodListPartE = [];
            vm.emailToListPartE = [];
        }

        function loadPartEData(vm, getCurrentDate) {
            if (!canViewPartE(vm)) {
                return $q.resolve();
            }

            determinePartEMode(vm);

            if (vm.partE.isReadOnly) {
                loadPartEWorkflowData(vm);
            }

            return $q.all([
                loadWSHOs(vm),
                loadHODs(vm),
                loadEmailToList(vm)
            ]).then(function () {
                if (!vm.partE.isReadOnly) {
                    $timeout(function () {
                        refreshKendoDropDowns(vm);
                    }, 0);
                }
            });
        }

        function determinePartEMode(vm) {
            if (vm.incident.status === '04') {
                vm.partE.isReadOnly = false;
                vm.partE.submitterName = '';
                vm.partE.submitterEmpId = '';
                vm.partE.submitterDesignation = '';
                vm.partE.submissionDate = '';
            } else if (parseInt(vm.incident.status) > 4) {
                vm.partE.isReadOnly = true;
            }
        }

        function loadPartEWorkflowData(vm) {
            var partEWorkflows = [];
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                partEWorkflows = vm.incident.workflows.filter(function (w) {
                    return w.actionCode === '05';
                });
            }

            if (partEWorkflows.length > 0) {
                partEWorkflows.sort(function (a, b) {
                    var dateA = new Date(a.date || 0);
                    var dateB = new Date(b.date || 0);
                    return dateB - dateA;
                });

                var latestWorkflow = partEWorkflows[0];
                vm.partE.comments = latestWorkflow.remarks || '';
                vm.partE.submitterName = latestWorkflow.fromName || '';
                vm.partE.submitterEmpId = latestWorkflow.from || '';
                vm.partE.submitterDesignation = latestWorkflow.fromDesignation || '';
                vm.partE.submissionDate = latestWorkflow.date || '';
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
                vm.wshoListPartE = data;
                vm.partE.wshoOptions.dataSource.data(data);
                if (data.length > 0 && vm.incident.wshoId) {
                    vm.partE.wshoId = vm.incident.wshoId;
                }
            }).catch(function (error) {
                vm.wshoListPartE = [];
                vm.partE.wshoOptions.dataSource.data([]);
            });
        }

        function loadHODs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
            }

            return IncidentUpdateService.getHODs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department || '',
                vm.incident.location || ''
            ).then(function (data) {
                vm.hodListPartE = data;
                vm.partE.hodOptions.dataSource.data(data);
                if (data.length > 0 && vm.incident.hodId) {
                    vm.partE.hodId = vm.incident.hodId;
                }
            }).catch(function (error) {
                vm.hodListPartE = [];
                vm.partE.hodOptions.dataSource.data([]);
            });
        }

        function loadEmailToList(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
            }

            return IncidentUpdateService.getPartECopyToList(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            ).then(function (data) {
                vm.emailToListPartE = data.map(function (person) {
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
                    }
                    widget.value(value);
                    widget.trigger('change');
                }
            }

            refreshDropDown('partE_wsho', vm.partE.wshoOptions.dataSource, vm.partE.wshoId);
            refreshDropDown('partE_hod', vm.partE.hodOptions.dataSource, vm.partE.hodId);
        }

        function canViewPartE(vm) {
            return vm.canUserViewPart('04');
        }

        function canEditPartE(vm) {
            return vm.canUserEditPart('04');
        }

        function revertPartEToWSHO(vm) {
            vm.partE.validationMessage = '';
            vm.partE.successMessage = '';

            if (!vm.partE.comments || vm.partE.comments.trim() === '') {
                vm.partE.validationMessage = 'Review & Comment is required (ERR-137)';
                return;
            }

            if (!vm.partE.wshoId) {
                vm.partE.validationMessage = 'Name of WSHO is required (ERR-135)';
                return;
            }

            if (!confirm('Are you sure you want to revert to WSHO? This action cannot be undone.')) {
                return;
            }

            vm.partE.isSubmitting = true;

            var selectedEmailTo = vm.emailToListPartE
                .filter(function (person) { return person.selected; })
                .map(function (person) { return person.id; });

            var revertData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partE.comments,
                wshoId: vm.partE.wshoId,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partE.additionalCopyToList
            };

            IncidentUpdateService.revertPartEToWSHO(revertData)
                .then(function (response) {
                    if (response.success) {
                        vm.partE.successMessage = response.message || 'Part E reverted successfully to WSHO';
                        setTimeout(function () {
                            $window.location.href = (window.WIRS_CONFIG ? window.WIRS_CONFIG.basePath : '') + '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partE.validationMessage = response.message || 'Failed to revert Part E';
                    }
                })
                .catch(function (error) {
                    vm.partE.validationMessage = error.message || 'An error occurred while reverting Part E';
                })
                .finally(function () {
                    vm.partE.isSubmitting = false;
                });
        }

        function approveAndSubmitPartE(vm) {
            vm.partE.validationMessage = '';
            vm.partE.successMessage = '';

            if (!vm.partE.comments || vm.partE.comments.trim() === '') {
                vm.partE.validationMessage = 'Review & Comment is required (ERR-134)';
                return;
            }

            if (!vm.partE.hodId) {
                vm.partE.validationMessage = 'Name of HOD is required (ERR-133)';
                return;
            }

            if (!confirm('Are you sure you want to approve and submit to HOD? This action cannot be undone.')) {
                return;
            }

            vm.partE.isSubmitting = true;

            var selectedEmailTo = vm.emailToListPartE
                .filter(function (person) { return person.selected; })
                .map(function (person) { return person.id; });

            var submitData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partE.comments,
                hodId: vm.partE.hodId,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partE.additionalCopyToList
            };

            IncidentUpdateService.submitPartE(submitData)
                .then(function (response) {
                    if (response.success) {
                        vm.partE.successMessage = response.message || 'Part E submitted successfully to HOD';
                        setTimeout(function () {
                            $window.location.href = (window.WIRS_CONFIG ? window.WIRS_CONFIG.basePath : '') + '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partE.validationMessage = response.message || 'Failed to submit Part E';
                    }
                })
                .catch(function (error) {
                    vm.partE.validationMessage = error.message || 'An error occurred while submitting Part E';
                })
                .finally(function () {
                    vm.partE.isSubmitting = false;
                });
        }

        function openEmployeeSearch(vm, context) {
            if (typeof window.openEmployeeSearch === 'function') {
                window.openEmployeeSearch('partE', function(employee) {
                    if (context === 'copyTo') {
                        $timeout(function() {
                            vm.partE.copyToPerson = {
                                name: employee.name,
                                employeeNo: employee.empId,
                                designation: employee.designation
                            };
                        });
                    }
                });
            }
        }

        function addPartECopyTo(vm) {
            if (!vm.partE.copyToPerson) {
                vm.partE.copyToPerson = {};
            }

            if (!vm.partE.copyToPerson.name || !vm.partE.copyToPerson.employeeNo) {
                alert('Please enter name and employee number');
                return;
            }

            if (!vm.partE.additionalCopyToList) {
                vm.partE.additionalCopyToList = [];
            }

            var exists = vm.partE.additionalCopyToList.some(function(p) {
                return p.employeeNo === vm.partE.copyToPerson.employeeNo;
            });

            if (exists) {
                alert('This person has already been added');
                return;
            }

            vm.partE.additionalCopyToList.push({
                name: vm.partE.copyToPerson.name,
                employeeNo: vm.partE.copyToPerson.employeeNo,
                designation: vm.partE.copyToPerson.designation || ''
            });

            vm.partE.copyToPerson = {};
        }

        function removePartECopyToPerson(vm, index) {
            vm.partE.additionalCopyToList.splice(index, 1);
        }
    }
})();
