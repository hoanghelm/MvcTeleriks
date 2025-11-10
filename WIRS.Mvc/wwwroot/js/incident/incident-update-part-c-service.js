(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartCService', PartCService);

    PartCService.$inject = ['$window', '$timeout', 'IncidentUpdateService'];

    function PartCService($window, $timeout, IncidentUpdateService) {
        var service = {
            initializePartC: initializePartC,
            loadPartCData: loadPartCData,
            canViewPartC: canViewPartC,
            canEditPartC: canEditPartC,
            isInjuryIncident: isInjuryIncident,
            addPersonInterviewed: addPersonInterviewed,
            removePersonInterviewed: removePersonInterviewed,
            addInjuryDetail: addInjuryDetail,
            removeInjuryDetail: removeInjuryDetail,
            addMedicalCertificate: addMedicalCertificate,
            removeMedicalCertificate: removeMedicalCertificate,
            savePartC: savePartC,
            submitPartC: submitPartC,
            closePartC: closePartC,
            openEmployeeSearch: openEmployeeSearch
        };

        return service;

        function initializePartC(vm) {
            vm.partC = {
                isReadOnly: false,
                isNegligent: '',
                negligentComments: '',
                needsRiskAssessmentReview: 'N',
                riskAssessmentComments: '',
                whatHappenedAndWhy: '',
                recommendedActions: '',
                additionalComments: '',
                cwshoId: '',
                personsInterviewed: [],
                injuryDetails: [],
                medicalCertificates: [],
                personInterviewed: {},
                injuryDetail: {},
                medicalCert: {},
                validationMessage: '',
                saving: false,
                submitting: false,
                closing: false,
                showCloseOptions: false,
                injuredPersonOptions: {
                    dataTextField: 'name',
                    dataValueField: 'empNo',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select Injured Person --',
                    valuePrimitive: true
                },
                cwshoOptions: {
                    dataTextField: 'name',
                    dataValueField: 'id',
                    dataSource: new kendo.data.DataSource({ data: [] }),
                    optionLabel: '-- Select Corporate WSHO --',
                    valuePrimitive: true
                }
            };

            vm.cwshoList = [];
            vm.natureOfInjury = [];
            vm.headNeckTorso = [];
            vm.upperLimbs = [];
            vm.lowerLimbs = [];
            vm.incidentClass = [];
            vm.incidentAgent = [];
            vm.unsafeConditions = [];
            vm.unsafeActs = [];
            vm.contributingFactors = [];
            vm.negligentOptions = [];
        }

        function loadPartCData(vm) {
            if (!canViewPartC(vm)) {
                return Promise.resolve();
            }

            determinePartCMode(vm);

            if (vm.partC.isReadOnly) {
                return loadPartCReadOnlyData(vm);
            }

            if (vm.incident.injuredPersons && vm.incident.injuredPersons.length > 0) {
                vm.partC.injuredPersonOptions.dataSource.data(vm.incident.injuredPersons);
            }

            return Promise.all([
                loadPartCLookups(vm),
                loadCWSHOs(vm)
            ]).then(function () {
                if (!vm.partC.isReadOnly) {
                    $timeout(function () {
                        refreshKendoDropDowns(vm);
                    }, 0);
                }
            });
        }

        function loadPartCLookups(vm) {
            var promises = [
                IncidentUpdateService.getNatureOfInjury().then(function (data) {
                    vm.natureOfInjury = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getHeadNeckTorso().then(function (data) {
                    vm.headNeckTorso = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getUpperLimbs().then(function (data) {
                    vm.upperLimbs = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getLowerLimbs().then(function (data) {
                    vm.lowerLimbs = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getIncidentClass().then(function (data) {
                    vm.incidentClass = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getIncidentAgent().then(function (data) {
                    vm.incidentAgent = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getUnsafeConditions().then(function (data) {
                    vm.unsafeConditions = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getUnsafeActs().then(function (data) {
                    vm.unsafeActs = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getContributingFactors().then(function (data) {
                    vm.contributingFactors = data.map(function (item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getNegligentOptions().then(function (data) {
                    vm.negligentOptions = data;
                })
            ];

            return Promise.all(promises).catch(function (error) {
                return Promise.resolve();
            });
        }

        function loadCWSHOs(vm) {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getCWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function (data) {
                    vm.cwshoList = data;
                    vm.partC.cwshoOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    vm.partC.cwshoOptions.dataSource.data([]);
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

            refreshDropDown('partC_injuredPerson', vm.partC.injuredPersonOptions.dataSource, vm.partC.injuryDetail.injuredPersonId);
            refreshDropDown('partC_mcInjuredPerson', vm.partC.injuredPersonOptions.dataSource, vm.partC.medicalCert.injuredPersonId);
            refreshDropDown('partC_cwsho', vm.partC.cwshoOptions.dataSource, vm.partC.cwshoId);
        }

        function determinePartCMode(vm) {
            if (vm.incident.status === '02') {
                vm.partC.isReadOnly = false;
            } else if (parseInt(vm.incident.status) > 2) {
                vm.partC.isReadOnly = true;
            }
        }

        function loadPartCReadOnlyData(vm) {
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                var partCWorkflow = vm.incident.workflows.find(function (w) {
                    return w.status === '03' && w.role === 'WSHO';
                });

                if (partCWorkflow) {
                    vm.partC.isNegligent = vm.incident.negligent || '';
                    vm.partC.negligentComments = vm.incident.negligentComments || '';
                    vm.partC.whatHappenedAndWhy = vm.incident.whatHappenedAndWhyComments || '';
                    vm.partC.recommendedActions = vm.incident.recommendActionDesc || '';
                    vm.partC.submitterName = partCWorkflow.submitterName || '';
                    vm.partC.submitterEmpId = partCWorkflow.submitterEmpId || '';
                    vm.partC.submitterDesignation = partCWorkflow.submitterDesignation || '';
                    vm.partC.submissionDate = partCWorkflow.submissionDate || '';
                }
            }
            return Promise.resolve();
        }

        function canViewPartC(vm) {
            if (!vm.incident.status || parseInt(vm.incident.status) < 2) {
                return false;
            }

            if (vm.isWorkflowClosed && vm.isWorkflowClosed()) {
                var highestCode = vm.getHighestCompletedActionCode();
                var hasPartCAction = vm.incident.workflows && vm.incident.workflows.some(function (w) {
                    return w.actionCode === '03';
                });

                if (!hasPartCAction && parseInt(highestCode) < 3) {
                    return false;
                }
            }

            return true;
        }

        function canEditPartC(vm) {
            if (!vm.incident || !vm.currentUser) {
                return false;
            }

            if (vm.incident.status !== '02') {
                return false;
            }

            if (vm.incident.wshoId === vm.currentUser.userId) {
                return true;
            }

            if (vm.incident.alternateWshoId === vm.currentUser.userId) {
                return true;
            }

            return false;
        }

        function isInjuryIncident(vm) {
            if (!vm.incident || !vm.incident.incidentTypes || !vm.incident.incidentTypes.length) {
                return false;
            }

            for (var i = 0; i < vm.incident.incidentTypes.length; i++) {
                var type = vm.incident.incidentTypes[i];

                if (typeof type === 'object' && type !== null) {
                    var typeValue = type.code || type.incidentTypeCode || type.typeCode || type.type || type.incidentType;
                    if (typeValue === '1' || typeValue === 1) {
                        return true;
                    }
                } else {
                    if (type === '1' || type === 1) {
                        return true;
                    }
                }
            }

            return false;
        }

        function addPersonInterviewed(vm) {
            if (!vm.partC.personInterviewed.name) {
                alert('Please enter person name');
                return;
            }

            vm.partC.personsInterviewed.push({
                name: vm.partC.personInterviewed.name,
                employeeNo: vm.partC.personInterviewed.employeeNo || '',
                designation: vm.partC.personInterviewed.designation || '',
                contactNo: vm.partC.personInterviewed.contactNo || ''
            });

            vm.partC.personInterviewed = {};
        }

        function removePersonInterviewed(vm, index) {
            vm.partC.personsInterviewed.splice(index, 1);
        }

        function addInjuryDetail(vm) {
            if (!vm.partC.injuryDetail.injuredPersonId) {
                alert('Please select injured person');
                return;
            }

            var selectedNatureList = vm.natureOfInjury.filter(function (n) { return n.selected; }).map(function (n) { return n.value; });
            var selectedHeadList = vm.headNeckTorso.filter(function (h) { return h.selected; }).map(function (h) { return h.value; });
            var selectedUpperList = vm.upperLimbs.filter(function (u) { return u.selected; }).map(function (u) { return u.value; });
            var selectedLowerList = vm.lowerLimbs.filter(function (l) { return l.selected; }).map(function (l) { return l.value; });

            var bodyPartsList = selectedHeadList.concat(selectedUpperList).concat(selectedLowerList);

            var selectedNature = selectedNatureList.join(', ');
            var bodyParts = bodyPartsList.join(', ');

            var injuredPerson = vm.incident.injuredPersons.find(function (p) { return p.empNo === vm.partC.injuryDetail.injuredPersonId; });

            vm.partC.injuryDetails.push({
                injuredPersonId: vm.partC.injuryDetail.injuredPersonId,
                injuredPersonName: injuredPerson ? injuredPerson.name : '',
                natureOfInjury: selectedNature,
                bodyParts: bodyParts,
                natureOfInjuryList: selectedNatureList,
                bodyPartsList: bodyPartsList,
                description: vm.partC.injuryDetail.description || '',
                natureOfInjuryCodeList: vm.natureOfInjury.filter(function (n) { return n.selected; }).map(function (n) { return n.code; }),
                headNeckTorsoList: vm.headNeckTorso.filter(function (h) { return h.selected; }).map(function (h) { return h.code; }),
                upperLimbsList: vm.upperLimbs.filter(function (u) { return u.selected; }).map(function (u) { return u.code; }),
                lowerLimbsList: vm.lowerLimbs.filter(function (l) { return l.selected; }).map(function (l) { return l.code; })
            });

            vm.partC.injuryDetail = {};
            vm.natureOfInjury.forEach(function (n) { n.selected = false; });
            vm.headNeckTorso.forEach(function (h) { h.selected = false; });
            vm.upperLimbs.forEach(function (u) { u.selected = false; });
            vm.lowerLimbs.forEach(function (l) { l.selected = false; });
        }

        function removeInjuryDetail(vm, index) {
            vm.partC.injuryDetails.splice(index, 1);
        }

        function addMedicalCertificate(vm) {
            if (!vm.partC.medicalCert.injuredPersonId) {
                alert('Please select injured person');
                return;
            }

            if (!vm.partC.medicalCert.numberOfDays || vm.partC.medicalCert.numberOfDays <= 0) {
                alert('Please enter number of days');
                return;
            }

            var injuredPerson = vm.incident.injuredPersons.find(function (p) { return p.empNo === vm.partC.medicalCert.injuredPersonId; });

            var fromDate = vm.partC.medicalCert.fromDate;
            var toDate = vm.partC.medicalCert.toDate;

            if (typeof fromDate === 'string' && fromDate.indexOf('T') > -1) {
                fromDate = new Date(fromDate);
            }
            if (typeof toDate === 'string' && toDate.indexOf('T') > -1) {
                toDate = new Date(toDate);
            }

            vm.partC.medicalCertificates.push({
                injuredPersonId: vm.partC.medicalCert.injuredPersonId,
                injuredPersonName: injuredPerson ? injuredPerson.name : '',
                fromDate: fromDate,
                toDate: toDate,
                numberOfDays: vm.partC.medicalCert.numberOfDays,
                hasAttachment: false
            });

            vm.partC.medicalCert = {};
        }

        function removeMedicalCertificate(vm, index) {
            vm.partC.medicalCertificates.splice(index, 1);
        }

        function openEmployeeSearch(vm, context) {
            if (typeof window.openEmployeeSearch === 'function') {
                window.openEmployeeSearch('partC', function(employee) {
                    $timeout(function() {
                        if (context === 'personInterviewed') {
                            vm.partC.personInterviewed = {
                                name: employee.name,
                                employeeNo: employee.empId,
                                designation: employee.designation,
                                contactNo: employee.contactNo || ''
                            };
                        }
                    });
                });
            }
        }

        function savePartC(vm) {
            vm.partC.validationMessage = '';
            vm.partC.saving = true;

            var partCData = buildPartCData(vm);

            IncidentUpdateService.savePartC(partCData)
                .then(function (response) {
                    if (response.success) {
                        alert('Part C saved successfully!');
                    } else {
                        vm.partC.validationMessage = response.message || 'Failed to save Part C';
                    }
                })
                .catch(function (error) {
                    vm.partC.validationMessage = error.message || 'An error occurred while saving Part C';
                })
                .finally(function () {
                    vm.partC.saving = false;
                });
        }

        function submitPartC(vm) {
            vm.partC.validationMessage = '';

            if (!validatePartC(vm)) {
                return;
            }

            vm.partC.submitting = true;

            var partCData = buildPartCData(vm);

            IncidentUpdateService.submitPartC(partCData)
                .then(function (response) {
                    if (response.success) {
                        alert('Part C submitted successfully to Chairman WSH!');
                        $window.location.href = '/Home/Index';
                    } else {
                        vm.partC.validationMessage = response.message || 'Failed to submit Part C';
                    }
                })
                .catch(function (error) {
                    vm.partC.validationMessage = error.message || 'An error occurred while submitting Part C';
                })
                .finally(function () {
                    vm.partC.submitting = false;
                });
        }

        function closePartC(vm) {
            vm.partC.validationMessage = '';

            if (!vm.partC.additionalComments) {
                vm.partC.validationMessage = 'Additional comments required for closure (ERR-134)';
                return;
            }

            if (!vm.partC.cwshoId) {
                vm.partC.validationMessage = 'Corporate WSHO selection required (ERR-135)';
                return;
            }

            if (!confirm('Are you sure you want to close this incident? This will bypass Parts D, E, F, and G.')) {
                return;
            }

            vm.partC.closing = true;

            var closeData = {
                incidentId: vm.incident.incidentId,
                additionalComments: vm.partC.additionalComments,
                cwshoId: vm.partC.cwshoId,
                partCData: buildPartCData(vm)
            };

            IncidentUpdateService.closePartC(closeData)
                .then(function (response) {
                    if (response.success) {
                        alert('Incident closed successfully!');
                        $window.location.href = '/Home/Index';
                    } else {
                        vm.partC.validationMessage = response.message || 'Failed to close incident';
                    }
                })
                .catch(function (error) {
                    vm.partC.validationMessage = error.message || 'An error occurred while closing incident';
                })
                .finally(function () {
                    vm.partC.closing = false;
                });
        }

        function validatePartC(vm) {
            if (!vm.partC.isNegligent) {
                vm.partC.validationMessage = 'Negligent/ Non Negligent is required (ERR-136)';
                return false;
            }

            if (!vm.partC.additionalComments) {
                vm.partC.validationMessage = 'Comment is required (ERR-137)';
                return false;
            }

            if (!vm.partC.cwshoId) {
                vm.partC.validationMessage = 'Name of Chairman WSH is required (ERR-138)';
                return false;
            }

            if (!vm.partC.recommendedActions) {
                vm.partC.validationMessage = 'Corrective and Preventive Action(s) is required (ERR-139)';
                return false;
            }

            var hasIncidentClass = vm.incidentClass.some(function (c) { return c.selected; });
            if (!hasIncidentClass) {
                vm.partC.validationMessage = 'At least one incident classification is required';
                return false;
            }

            var hasUnsafeCondition = vm.unsafeConditions.some(function (c) { return c.selected; });
            var hasUnsafeAct = vm.unsafeActs.some(function (a) { return a.selected; });
            if (!hasUnsafeCondition && !hasUnsafeAct) {
                vm.partC.validationMessage = 'At least one unsafe condition OR unsafe act is required';
                return false;
            }

            return true;
        }

        function buildPartCData(vm) {
            return {
                incidentId: vm.incident.incidentId,
                isNegligent: vm.partC.isNegligent,
                negligentComments: vm.partC.negligentComments || '',
                needsRiskAssessmentReview: vm.partC.needsRiskAssessmentReview,
                riskAssessmentComments: vm.partC.riskAssessmentComments || '',
                whatHappenedAndWhy: vm.partC.whatHappenedAndWhy || '',
                recommendedActions: vm.partC.recommendedActions,
                additionalComments: vm.partC.additionalComments || '',
                cwshoId: vm.partC.cwshoId || '',
                personsInterviewed: vm.partC.personsInterviewed,
                injuryDetails: vm.partC.injuryDetails.map(function (injury) {
                    return {
                        injuredPersonId: injury.injuredPersonId,
                        injuredPersonName: injury.injuredPersonName,
                        natureOfInjury: injury.natureOfInjuryCodeList,
                        headNeckTorso: injury.headNeckTorsoList,
                        upperLimbs: injury.upperLimbsList,
                        lowerLimbs: injury.lowerLimbsList,
                        description: injury.description
                    };
                }),
                medicalCertificates: vm.partC.medicalCertificates,
                incidentClassList: vm.incidentClass.filter(function (c) { return c.selected; }).map(function (c) { return c.code; }),
                incidentAgentList: vm.incidentAgent.filter(function (a) { return a.selected; }).map(function (a) { return a.code; }),
                unsafeConditionsList: vm.unsafeConditions.filter(function (c) { return c.selected; }).map(function (c) { return c.code; }),
                unsafeActsList: vm.unsafeActs.filter(function (a) { return a.selected; }).map(function (a) { return a.code; }),
                contributingFactorsList: vm.contributingFactors.filter(function (f) { return f.selected; }).map(function (f) { return f.code; })
            };
        }
    }
})();