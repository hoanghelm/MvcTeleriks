(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartDService', PartDService);

    PartDService.$inject = ['$window', '$timeout', 'IncidentUpdateService'];

    function PartDService($window, $timeout, IncidentUpdateService) {
        var service = {
            initializePartD: initializePartD,
            loadPartDData: loadPartDData,
            canViewPartD: canViewPartD,
            canEditPartD: canEditPartD,
            revertToWSHO: revertToWSHO,
            submitToHeadLOB: submitToHeadLOB
        };

        return service;

        function initializePartD(vm) {
            vm.partD = {
                isReadOnly: false,
                comments: '',
                wshoId: '',
                headLobId: '',
                submitterName: '',
                submitterEmpId: '',
                submitterDesignation: '',
                submissionDate: '',
                validationMessage: '',
                successMessage: '',
                isSubmitting: false,
                wshoOptions: {
                    dataTextField: 'userName',
                    dataValueField: 'userId',
                    dataSource: [],
                    optionLabel: '-- Select WSHO --'
                },
                headLobOptions: {
                    dataTextField: 'userName',
                    dataValueField: 'userId',
                    dataSource: [],
                    optionLabel: '-- Select Head LOB --'
                }
            };

            vm.wshoList = [];
            vm.headLobList = [];
        }

        function loadPartDData(vm, getCurrentDate) {
            if (!canViewPartD(vm)) {
                return Promise.resolve();
            }

            determinePartDMode(vm);

            if (vm.partD.isReadOnly) {
                loadPartDWorkflowData(vm);
                loadPartDReadOnlyData(vm);
            }

            return Promise.all([
                loadWSHOs(vm),
                loadHeadLOBs(vm)
            ]).then(function () {
                if (!vm.partD.isReadOnly) {
                    $timeout(function () {
                        refreshKendoDropDowns(vm);
                    }, 0);
                }
            });
        }

        function determinePartDMode(vm) {
            if (vm.incident.status === '03') {
                vm.partD.isReadOnly = false;
                vm.partD.submitterName = '';
                vm.partD.submitterEmpId = '';
                vm.partD.submitterDesignation = '';
                vm.partD.submissionDate = '';
            } else if (parseInt(vm.incident.status) > 3) {
                vm.partD.isReadOnly = true;
            }
        }

        function loadPartDWorkflowData(vm) {
            if (!vm.incident.workflows || vm.incident.workflows.length === 0) {
                return;
            }

            var partDWorkflows = vm.incident.workflows.filter(function (w) {
                return w.actionCode === '04';
            });

            if (partDWorkflows.length === 0) {
                return;
            }

            partDWorkflows.sort(function (a, b) {
                var dateA = new Date(a.submittedDate);
                var dateB = new Date(b.submittedDate);
                return dateB - dateA;
            });

            var latestWorkflow = partDWorkflows[0];
            vm.partD.submitterName = latestWorkflow.fromName || '';
            vm.partD.submitterEmpId = latestWorkflow.from || '';
            vm.partD.submitterDesignation = latestWorkflow.fromDesignation || '';
            vm.partD.submissionDate = latestWorkflow.submittedDate || '';
        }

        function loadPartDReadOnlyData(vm) {
            vm.partD.comments = vm.incident.partDComments || '';
        }

        function loadWSHOs(vm) {
            if (!vm.incident.sectorCode || !vm.incident.lobCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getWSHOs(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode || '',
                vm.incident.locationCode || ''
            ).then(function (data) {
                vm.wshoList = data.map(function (item) {
                    return {
                        userId: item.userId || item.employeeNo,
                        userName: item.userName || item.name,
                        designation: item.designation || ''
                    };
                });
                vm.partD.wshoOptions.dataSource = vm.wshoList;
            }).catch(function (error) {
                vm.wshoList = [];
                vm.partD.wshoOptions.dataSource = [];
            });
        }

        function loadHeadLOBs(vm) {
            if (!vm.incident.sectorCode || !vm.incident.lobCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getHODs(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode || '',
                vm.incident.locationCode || ''
            ).then(function (data) {
                vm.headLobList = data.map(function (item) {
                    return {
                        userId: item.userId || item.employeeNo,
                        userName: item.userName || item.name,
                        designation: item.designation || ''
                    };
                });
                vm.partD.headLobOptions.dataSource = vm.headLobList;
            }).catch(function (error) {
                vm.headLobList = [];
                vm.partD.headLobOptions.dataSource = [];
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

            refreshDropDown('partD_wsho', vm.partD.wshoOptions.dataSource, vm.partD.wshoId);
            refreshDropDown('partD_headlob', vm.partD.headLobOptions.dataSource, vm.partD.headLobId);
        }

        function canViewPartD(vm) {
            if (!vm.incident || !vm.incident.status || parseInt(vm.incident.status) < 3) {
                return false;
            }

            if (vm.isWorkflowClosed && vm.isWorkflowClosed()) {
                var highestCode = vm.getHighestCompletedActionCode();
                var hasPartDAction = vm.incident.workflows && vm.incident.workflows.some(function (w) {
                    return w.actionCode === '04';
                });

                if (!hasPartDAction && parseInt(highestCode) < 4) {
                    return false;
                }
            }

            return true;
        }

        function canEditPartD(vm) {
            if (!vm.incident || vm.incident.status !== '03') return false;
            if (!vm.currentUser || !vm.currentUser.userId) return false;

            if (vm.incident.cwshoId === vm.currentUser.userId) return true;

            return false;
        }

        function revertToWSHO(vm) {
            vm.partD.validationMessage = '';
            vm.partD.successMessage = '';

            if (!vm.partD.comments || vm.partD.comments.trim() === '') {
                vm.partD.validationMessage = 'Review & Comment is required (ERR-137)';
                return;
            }

            if (!vm.partD.wshoId) {
                vm.partD.validationMessage = 'Name of WSHO is required (ERR-135)';
                return;
            }

            if (!confirm('Are you sure you want to revert to WSHO? This action cannot be undone.')) {
                return;
            }

            vm.partD.isSubmitting = true;

            var revertData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partD.comments,
                wshoId: vm.partD.wshoId,
                actionType: 'revert'
            };

            IncidentUpdateService.revertPartDToWSHO(revertData)
                .then(function (response) {
                    if (response.success) {
                        vm.partD.successMessage = response.message || 'Part D reverted successfully to WSHO';
                        setTimeout(function () {
                            $window.location.href = '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partD.validationMessage = response.message || 'Failed to revert Part D';
                    }
                })
                .catch(function (error) {
                    vm.partD.validationMessage = error.message || 'An error occurred while reverting Part D';
                })
                .finally(function () {
                    vm.partD.isSubmitting = false;
                });
        }

        function submitToHeadLOB(vm) {
            vm.partD.validationMessage = '';
            vm.partD.successMessage = '';

            if (!vm.partD.comments || vm.partD.comments.trim() === '') {
                vm.partD.validationMessage = 'Review & Comment is required (ERR-137)';
                return;
            }

            if (!vm.partD.headLobId) {
                vm.partD.validationMessage = 'Name of Head LOB is required (ERR-133)';
                return;
            }

            if (!confirm('Are you sure you want to submit to Head LOB? This action cannot be undone.')) {
                return;
            }

            vm.partD.isSubmitting = true;

            var submitData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partD.comments,
                headLobId: vm.partD.headLobId,
                actionType: 'submit'
            };

            IncidentUpdateService.submitPartDToHeadLOB(submitData)
                .then(function (response) {
                    if (response.success) {
                        vm.partD.successMessage = response.message || 'Part D submitted successfully to Head LOB';
                        setTimeout(function () {
                            $window.location.href = '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partD.validationMessage = response.message || 'Failed to submit Part D';
                    }
                })
                .catch(function (error) {
                    vm.partD.validationMessage = error.message || 'An error occurred while submitting Part D';
                })
                .finally(function () {
                    vm.partD.isSubmitting = false;
                });
        }
    }
})();