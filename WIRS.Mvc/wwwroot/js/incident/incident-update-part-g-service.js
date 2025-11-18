(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartGService', PartGService);

    PartGService.$inject = ['$window', '$timeout', '$q', 'IncidentUpdateService'];

    function PartGService($window, $timeout, $q, IncidentUpdateService) {
        var service = {
            initializePartG: initializePartG,
            loadPartGData: loadPartGData,
            canViewPartG: canViewPartG,
            canEditPartG: canEditPartG,
            submitPartG: submitPartG,
            revertPartGToHOD: revertPartGToHOD,
            removePartGAttachment: removePartGAttachment
        };

        return service;

        function initializePartG(vm) {
            vm.partG = {
                isReadOnly: false,
                comments: '',
                hodId: '',
                cwshoId: '',
                submitterName: '',
                submitterEmpId: '',
                submitterDesignation: '',
                submissionDate: '',
                attachments: [],
                validationMessage: '',
                successMessage: '',
                isSubmitting: false,
                hodOptions: {
                    dataTextField: 'name',
                    dataValueField: 'id',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select HOD --',
                    valuePrimitive: true
                },
                cwshoOptions: {
                    dataTextField: 'name',
                    dataValueField: 'id',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select Chairman WSH --',
                    valuePrimitive: true
                }
            };

            vm.hodListPartG = [];
            vm.cwshoListPartG = [];
        }

        function loadPartGData(vm, getCurrentDate) {
            if (!canViewPartG(vm)) {
                return $q.resolve();
            }

            determinePartGMode(vm);

            if (vm.partG.isReadOnly) {
                loadPartGWorkflowData(vm);
            }

            return $q.all([
                loadHODs(vm),
                loadCWSHOs(vm)
            ]).then(function () {
                if (!vm.partG.isReadOnly) {
                    $timeout(function () {
                        refreshKendoDropDowns(vm);
                        setupFileUploadHandlers(vm);
                    }, 0);
                }
            });
        }

        function determinePartGMode(vm) {
            if (vm.incident.status === '06') {
                vm.partG.isReadOnly = false;
                vm.partG.submitterName = '';
                vm.partG.submitterEmpId = '';
                vm.partG.submitterDesignation = '';
                vm.partG.submissionDate = '';
            } else if (parseInt(vm.incident.status) > 6) {
                vm.partG.isReadOnly = true;
            }
        }

        function loadPartGWorkflowData(vm) {
            var partGWorkflows = [];
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                partGWorkflows = vm.incident.workflows.filter(function (w) {
                    return w.actionCode === '07';
                });
            }

            if (partGWorkflows.length > 0) {
                partGWorkflows.sort(function (a, b) {
                    var dateA = new Date(a.date || 0);
                    var dateB = new Date(b.date || 0);
                    return dateB - dateA;
                });

                var latestWorkflow = partGWorkflows[0];
                vm.partG.comments = latestWorkflow.remarks || '';
                vm.partG.submitterName = latestWorkflow.fromName || '';
                vm.partG.submitterEmpId = latestWorkflow.from || '';
                vm.partG.submitterDesignation = latestWorkflow.fromDesignation || '';
                vm.partG.submissionDate = latestWorkflow.date || '';
            }
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
                vm.hodListPartG = data;
                vm.partG.hodOptions.dataSource.data(data);
                if (data.length > 0 && vm.incident.hodId) {
                    vm.partG.hodId = vm.incident.hodId;
                }
            }).catch(function (error) {
                vm.hodListPartG = [];
                vm.partG.hodOptions.dataSource.data([]);
            });
        }

        function loadCWSHOs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return $q.resolve();
            }

            return IncidentUpdateService.getCWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department || '',
                vm.incident.location || ''
            ).then(function (data) {
                vm.cwshoListPartG = data;
                vm.partG.cwshoOptions.dataSource.data(data);
                if (data.length > 0 && vm.incident.cwshoId) {
                    vm.partG.cwshoId = vm.incident.cwshoId;
                }
            }).catch(function (error) {
                vm.cwshoListPartG = [];
                vm.partG.cwshoOptions.dataSource.data([]);
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

            refreshDropDown('partG_hod', vm.partG.hodOptions.dataSource, vm.partG.hodId);
            refreshDropDown('partG_cwsho', vm.partG.cwshoOptions.dataSource, vm.partG.cwshoId);
        }

        function setupFileUploadHandlers(vm) {
            var fileInput = document.getElementById('partG_fileUpload');

            if (fileInput) {
                fileInput.addEventListener('change', function (e) {
                    handleFileSelection(vm, e.target.files);
                });
            }
        }

        function handleFileSelection(vm, files) {
            if (!files || files.length === 0) return;

            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var fileObj = {
                    fileName: file.name,
                    fileSize: file.size,
                    fileType: file.type,
                    file: file,
                    filePath: null
                };

                vm.partG.attachments.push(fileObj);
            }

            document.getElementById('partG_fileUpload').value = '';
        }

        function canViewPartG(vm) {
            return vm.canUserViewPart('06');
        }

        function canEditPartG(vm) {
            return vm.canUserEditPart('06');
        }

        function submitPartG(vm) {
            vm.partG.validationMessage = '';
            vm.partG.successMessage = '';

            if (!vm.partG.comments || vm.partG.comments.trim() === '') {
                vm.partG.validationMessage = 'Review & Comment is required (ERR-134)';
                return;
            }

            if (!vm.partG.cwshoId) {
                vm.partG.validationMessage = 'Name of Chairman WSH is required (ERR-133)';
                return;
            }

            if (!confirm('Are you sure you want to submit to Chairman WSH? This action cannot be undone.')) {
                return;
            }

            vm.partG.isSubmitting = true;

            var formData = new FormData();
            formData.append('incidentId', vm.incident.incidentId);
            formData.append('comments', vm.partG.comments);
            formData.append('cwshoId', vm.partG.cwshoId);

            for (var i = 0; i < vm.partG.attachments.length; i++) {
                if (vm.partG.attachments[i].file) {
                    formData.append('attachments', vm.partG.attachments[i].file);
                }
            }

            IncidentUpdateService.submitPartG(formData)
                .then(function (response) {
                    if (response.success) {
                        vm.partG.successMessage = response.message || 'Part G submitted successfully to Chairman WSH';
                        setTimeout(function () {
                            $window.location.href = (window.WIRS_CONFIG ? window.WIRS_CONFIG.basePath : '') + '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partG.validationMessage = response.message || 'Failed to submit Part G';
                    }
                })
                .catch(function (error) {
                    vm.partG.validationMessage = error.message || 'An error occurred while submitting Part G';
                })
                .finally(function () {
                    vm.partG.isSubmitting = false;
                });
        }

        function revertPartGToHOD(vm) {
            vm.partG.validationMessage = '';
            vm.partG.successMessage = '';

            if (!vm.partG.comments || vm.partG.comments.trim() === '') {
                vm.partG.validationMessage = 'Review & Comment is required (ERR-134)';
                return;
            }

            if (!vm.partG.hodId) {
                vm.partG.validationMessage = 'Name of HOD is required (ERR-133)';
                return;
            }

            if (!confirm('Are you sure you want to revert to HOD? This action cannot be undone.')) {
                return;
            }

            vm.partG.isSubmitting = true;

            var formData = new FormData();
            formData.append('incidentId', vm.incident.incidentId);
            formData.append('comments', vm.partG.comments);
            formData.append('hodId', vm.partG.hodId);

            for (var i = 0; i < vm.partG.attachments.length; i++) {
                if (vm.partG.attachments[i].file) {
                    formData.append('attachments', vm.partG.attachments[i].file);
                }
            }

            IncidentUpdateService.revertPartGToHOD(formData)
                .then(function (response) {
                    if (response.success) {
                        vm.partG.successMessage = response.message || 'Part G reverted successfully to HOD';
                        setTimeout(function () {
                            $window.location.href = (window.WIRS_CONFIG ? window.WIRS_CONFIG.basePath : '') + '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partG.validationMessage = response.message || 'Failed to revert Part G';
                    }
                })
                .catch(function (error) {
                    vm.partG.validationMessage = error.message || 'An error occurred while reverting Part G';
                })
                .finally(function () {
                    vm.partG.isSubmitting = false;
                });
        }

        function removePartGAttachment(vm, index) {
            vm.partG.attachments.splice(index, 1);
        }
    }
})();
