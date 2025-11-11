(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartFService', PartFService);

    PartFService.$inject = ['$window', '$timeout', '$q', 'IncidentUpdateService'];

    function PartFService($window, $timeout, $q, IncidentUpdateService) {
        var service = {
            initializePartF: initializePartF,
            loadPartFData: loadPartFData,
            canViewPartF: canViewPartF,
            canEditPartF: canEditPartF,
            submitPartF: submitPartF,
            removePartFAttachment: removePartFAttachment,
            removePartFRiskAttachment: removePartFRiskAttachment
        };

        return service;

        function initializePartF(vm) {
            vm.partF = {
                isReadOnly: false,
                comments: '',
                partCComments: '',
                partDComments: '',
                partEComments: '',
                riskAssessmentReview: '',
                wshoId: '',
                submitterName: '',
                submitterEmpId: '',
                submitterDesignation: '',
                submissionDate: '',
                attachments: [],
                riskAttachments: [],
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

            vm.wshoListPartF = [];
        }

        function loadPartFData(vm, getCurrentDate) {
            if (!canViewPartF(vm)) {
                return $q.resolve();
            }

            determinePartFMode(vm);

            if (vm.partF.isReadOnly) {
                loadPartFWorkflowData(vm);
            }

            loadPreviousPartsComments(vm);

            return loadWSHOs(vm).then(function () {
                if (!vm.partF.isReadOnly) {
                    $timeout(function () {
                        refreshKendoDropDowns(vm);
                        setupFileUploadHandlers(vm);
                    }, 0);
                }
            });
        }

        function determinePartFMode(vm) {
            if (vm.incident.status === '05') {
                vm.partF.isReadOnly = false;
                vm.partF.submitterName = '';
                vm.partF.submitterEmpId = '';
                vm.partF.submitterDesignation = '';
                vm.partF.submissionDate = '';
            } else if (parseInt(vm.incident.status) > 5) {
                vm.partF.isReadOnly = true;
            }
        }

        function loadPartFWorkflowData(vm) {
            var partFWorkflows = [];
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                partFWorkflows = vm.incident.workflows.filter(function (w) {
                    return w.actionCode === '06';
                });
            }

            if (partFWorkflows.length > 0) {
                partFWorkflows.sort(function (a, b) {
                    var dateA = new Date(a.date || 0);
                    var dateB = new Date(b.date || 0);
                    return dateB - dateA;
                });

                var latestWorkflow = partFWorkflows[0];
                vm.partF.comments = latestWorkflow.remarks || '';
                vm.partF.submitterName = latestWorkflow.fromName || '';
                vm.partF.submitterEmpId = latestWorkflow.from || '';
                vm.partF.submitterDesignation = latestWorkflow.fromDesignation || '';
                vm.partF.submissionDate = latestWorkflow.date || '';
            }

            if (vm.incident.riskAssessmentReview) {
                vm.partF.riskAssessmentReview = vm.incident.riskAssessmentReview;
            }
        }

        function loadPreviousPartsComments(vm) {
            var partCWorkflows = vm.incident.workflows ? vm.incident.workflows.filter(function (w) {
                return w.actionCode === '03';
            }) : [];
            if (partCWorkflows.length > 0) {
                partCWorkflows.sort(function (a, b) {
                    return new Date(b.date || 0) - new Date(a.date || 0);
                });
                vm.partF.partCComments = vm.incident.recommendActionDesc || partCWorkflows[0].remarks || '';
            }

            var partDWorkflows = vm.incident.workflows ? vm.incident.workflows.filter(function (w) {
                return w.actionCode === '04';
            }) : [];
            if (partDWorkflows.length > 0) {
                partDWorkflows.sort(function (a, b) {
                    return new Date(b.date || 0) - new Date(a.date || 0);
                });
                vm.partF.partDComments = partDWorkflows[0].remarks || '';
            }

            var partEWorkflows = vm.incident.workflows ? vm.incident.workflows.filter(function (w) {
                return w.actionCode === '05';
            }) : [];
            if (partEWorkflows.length > 0) {
                partEWorkflows.sort(function (a, b) {
                    return new Date(b.date || 0) - new Date(a.date || 0);
                });
                vm.partF.partEComments = partEWorkflows[0].remarks || '';
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
                vm.wshoListPartF = data;
                vm.partF.wshoOptions.dataSource.data(data);
                if (data.length > 0 && vm.incident.wshoId) {
                    vm.partF.wshoId = vm.incident.wshoId;
                }
            }).catch(function (error) {
                vm.wshoListPartF = [];
                vm.partF.wshoOptions.dataSource.data([]);
            });
        }

        function refreshKendoDropDowns(vm) {
            var widget = $('#partF_wsho').data('kendoDropDownList');
            if (widget && vm.partF.wshoId) {
                if (vm.wshoListPartF && vm.wshoListPartF.length > 0) {
                    widget.setDataSource(new kendo.data.DataSource({
                        data: vm.wshoListPartF
                    }));
                }
                widget.value(vm.partF.wshoId);
                widget.trigger('change');
            }
        }

        function setupFileUploadHandlers(vm) {
            var fileInput = document.getElementById('partF_fileUpload');
            var riskFileInput = document.getElementById('partF_riskFileUpload');

            if (fileInput) {
                fileInput.addEventListener('change', function (e) {
                    handleFileSelection(vm, e.target.files, false);
                });
            }

            if (riskFileInput) {
                riskFileInput.addEventListener('change', function (e) {
                    handleFileSelection(vm, e.target.files, true);
                });
            }
        }

        function handleFileSelection(vm, files, isRiskAttachment) {
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

                if (isRiskAttachment) {
                    vm.partF.riskAttachments.push(fileObj);
                } else {
                    vm.partF.attachments.push(fileObj);
                }
            }

            if (!isRiskAttachment) {
                document.getElementById('partF_fileUpload').value = '';
            } else {
                document.getElementById('partF_riskFileUpload').value = '';
            }
        }

        function canViewPartF(vm) {
            if (!vm.incident || !vm.incident.status || parseInt(vm.incident.status) < 5) {
                return false;
            }

            if (vm.isWorkflowClosed && vm.isWorkflowClosed()) {
                var highestCode = vm.getHighestCompletedActionCode();
                var hasPartFAction = vm.incident.workflows && vm.incident.workflows.some(function (w) {
                    return w.actionCode === '06';
                });

                if (!hasPartFAction && parseInt(highestCode) < 6) {
                    return false;
                }
            }

            return true;
        }

        function canEditPartF(vm) {
            if (!vm.incident || vm.incident.status !== '05') return false;
            if (!vm.currentUser || !vm.currentUser.userId) return false;

            if (vm.incident.hodId === vm.currentUser.userId) return true;
            if (vm.incident.wshoId === vm.currentUser.userId) return true;

            return false;
        }

        function submitPartF(vm) {
            vm.partF.validationMessage = '';
            vm.partF.successMessage = '';

            if (!vm.partF.comments || vm.partF.comments.trim() === '') {
                vm.partF.validationMessage = 'Provide Objective Evidence of Actions Taken is required (ERR-137)';
                return;
            }

            if (!vm.partF.riskAssessmentReview) {
                vm.partF.validationMessage = 'Risk Assessment Review selection is required (ERR-116)';
                return;
            }

            if (!vm.partF.wshoId) {
                vm.partF.validationMessage = 'Name of WSHO is required (ERR-133)';
                return;
            }

            if (!confirm('Are you sure you want to submit to WSHO? This action cannot be undone.')) {
                return;
            }

            vm.partF.isSubmitting = true;

            var formData = new FormData();
            formData.append('incidentId', vm.incident.incidentId);
            formData.append('comments', vm.partF.comments);
            formData.append('riskAssessmentReview', vm.partF.riskAssessmentReview);
            formData.append('wshoId', vm.partF.wshoId);

            for (var i = 0; i < vm.partF.attachments.length; i++) {
                if (vm.partF.attachments[i].file) {
                    formData.append('attachments', vm.partF.attachments[i].file);
                }
            }

            for (var j = 0; j < vm.partF.riskAttachments.length; j++) {
                if (vm.partF.riskAttachments[j].file) {
                    formData.append('riskAttachments', vm.partF.riskAttachments[j].file);
                }
            }

            IncidentUpdateService.submitPartF(formData)
                .then(function (response) {
                    if (response.success) {
                        vm.partF.successMessage = response.message || 'Part F submitted successfully to WSHO';
                        setTimeout(function () {
                            $window.location.href = '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partF.validationMessage = response.message || 'Failed to submit Part F';
                    }
                })
                .catch(function (error) {
                    vm.partF.validationMessage = error.message || 'An error occurred while submitting Part F';
                })
                .finally(function () {
                    vm.partF.isSubmitting = false;
                });
        }

        function removePartFAttachment(vm, index) {
            vm.partF.attachments.splice(index, 1);
        }

        function removePartFRiskAttachment(vm, index) {
            vm.partF.riskAttachments.splice(index, 1);
        }
    }
})();
