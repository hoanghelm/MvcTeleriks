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
            submitPartD: submitPartD,
            openEmployeeSearchForPartD: openEmployeeSearchForPartD,
            removeAdditionalCopyToFromPartD: removeAdditionalCopyToFromPartD,
            cancelPartD: cancelPartD,
            getHsbuName: getHsbuName
        };

        return service;

        function initializePartD(vm) {
            vm.partD = {
                isReadOnly: false,
                comments: '',
                hsbuId: '',
                emailToList: [],
                additionalCopyToList: [],
                submitterName: '',
                submitterEmpId: '',
                submitterDesignation: '',
                submittedDate: '',
                currentDate: '',
                validationMessage: '',
                successMessage: '',
                isSubmitting: false,
                hsbuOptions: {
                    dataTextField: 'userName',
                    dataValueField: 'userId',
                    dataSource: [],
                    optionLabel: '-- Select HSBU --'
                }
            };

            vm.hsbuList = [];
        }

        function loadPartDData(vm, getCurrentDate) {
            if (!canViewPartD(vm)) {
                return Promise.resolve();
            }

            vm.partD.currentDate = getCurrentDate();
            determinePartDMode(vm);

            if (vm.partD.isReadOnly) {
                return loadPartDReadOnlyData(vm);
            }

            return Promise.all([
                loadHSBUs(vm),
                loadPartDEmailToList(vm)
            ]).then(function () {
                if (!vm.partD.isReadOnly) {
                    $timeout(function () {
                        refreshKendoDropDowns(vm);
                    }, 0);
                }
            });
        }

        function determinePartDMode(vm) {
            vm.partD.isReadOnly = vm.incident.status !== '03' || !canEditPartD(vm);
        }

        function loadPartDReadOnlyData(vm) {
            vm.partD.comments = vm.incident.partDComments || '';
            vm.partD.hsbuId = vm.incident.hsbuId || '';
            vm.partD.submitterName = vm.incident.partDSubmitterName || '';
            vm.partD.submitterEmpId = vm.incident.partDSubmitterEmpId || '';
            vm.partD.submitterDesignation = vm.incident.partDSubmitterDesignation || '';
            vm.partD.submittedDate = vm.incident.partDSubmittedDate || '';

            return Promise.all([
                loadHSBUs(vm),
                loadPartDEmailToList(vm)
            ]);
        }

        function loadHSBUs(vm) {
            if (!vm.incident.sectorCode || !vm.incident.lobCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getHSBUs(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode || '',
                vm.incident.locationCode || ''
            ).then(function (data) {
                vm.hsbuList = data.map(function (item) {
                    return {
                        userId: item.userId || item.employeeNo,
                        userName: item.userName || item.name,
                        designation: item.designation || ''
                    };
                });
                vm.partD.hsbuOptions.dataSource = vm.hsbuList;
            }).catch(function (error) {
                vm.hsbuList = [];
                vm.partD.hsbuOptions.dataSource = [];
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

            refreshDropDown('partD_hsbu', vm.partD.hsbuOptions.dataSource, vm.partD.hsbuId);
        }

        function loadPartDEmailToList(vm) {
            if (!vm.incident.sectorCode || !vm.incident.lobCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getPartDCopyToList(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode || '',
                vm.incident.locationCode || ''
            ).then(function (data) {
                vm.partD.emailToList = data.map(function (item) {
                    return {
                        userId: item.userId || item.employeeNo,
                        userName: item.userName || item.name,
                        designation: item.designation || '',
                        selected: true
                    };
                });
            }).catch(function (error) {
                console.error('Error loading email to list:', error);
                vm.partD.emailToList = [];
            });
        }

        function canViewPartD(vm) {
            return vm.incident && vm.incident.status && parseInt(vm.incident.status) >= 3;
        }

        function canEditPartD(vm) {
            if (!vm.incident || vm.incident.status !== '03') return false;
            if (!vm.currentUser || !vm.currentUser.userId) return false;

            if (vm.incident.hodId === vm.currentUser.userId) return true;
            if (vm.incident.ahodId === vm.currentUser.userId) return true;

            return false;
        }

        function submitPartD(vm) {
            vm.partD.validationMessage = '';
            vm.partD.successMessage = '';

            if (!vm.partD.comments || vm.partD.comments.trim() === '') {
                vm.partD.validationMessage = 'Comments are required (ERR-137)';
                return;
            }

            if (!vm.partD.hsbuId) {
                vm.partD.validationMessage = 'HSBU selection is required (ERR-133)';
                return;
            }

            if (!confirm('Are you sure you want to submit Part D to HSBU? This action cannot be undone.')) {
                return;
            }

            vm.partD.isSubmitting = true;

            var selectedEmailTo = vm.partD.emailToList
                .filter(function (person) { return person.selected; })
                .map(function (person) { return person.userId; });

            var submitData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partD.comments,
                hsbuId: vm.partD.hsbuId,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partD.additionalCopyToList.map(function (person) {
                    return {
                        employeeNo: person.employeeId,
                        name: person.name,
                        designation: person.designation
                    };
                })
            };

            IncidentUpdateService.submitPartD(submitData)
                .then(function (response) {
                    if (response.success) {
                        vm.partD.successMessage = response.message || 'Part D submitted successfully to HSBU';
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

        function openEmployeeSearchForPartD(vm) {
            alert('Employee search functionality will be implemented');
        }

        function removeAdditionalCopyToFromPartD(vm, index) {
            vm.partD.additionalCopyToList.splice(index, 1);
        }

        function cancelPartD() {
            if (confirm('Are you sure you want to cancel? Any unsaved changes will be lost.')) {
                $window.location.href = '/Home/Index';
            }
        }

        function getHsbuName(vm, hsbuId) {
            if (!hsbuId || !vm.hsbuList || vm.hsbuList.length === 0) {
                return 'N/A';
            }
            var hsbu = vm.hsbuList.find(function (h) {
                return h.userId === hsbuId;
            });
            return hsbu ? hsbu.userName + ' (' + hsbu.userId + ')' : 'N/A';
        }
    }
})();