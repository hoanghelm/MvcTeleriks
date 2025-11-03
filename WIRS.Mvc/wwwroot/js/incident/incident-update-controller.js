(function () {
    angular
        .module('incidentUpdateApp')
        .controller('IncidentUpdateController', IncidentUpdateController);

    IncidentUpdateController.$inject = ['$window', '$location', '$scope', '$timeout', 'IncidentUpdateService'];

    function IncidentUpdateController($window, $location, $scope, $timeout, IncidentUpdateService) {
        var vm = this;

        vm.loading = true;
        vm.error = null;
        vm.incident = {};
        vm.currentUser = {};
        vm.injuredCaseTypes = [];
        vm.wshoList = [];
        vm.alternateWshoList = [];
        vm.emailToList = [];
        vm.activeTab = 'A';

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
            showCloseOptions: false
        };

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
            isSubmitting: false
        };

        vm.partA = {
            incidentType: '',
            incidentOther: '',
            incidentDate: null,
            incidentTime: null,
            sectorCode: '',
            lobCode: '',
            departmentCode: '',
            locationCode: '',
            exactLocation: '',
            incidentDescription: '',
            hasEyewitness: '',
            hasDamage: '',
            workingOvertime: '',
            isJobRelated: '',
            hospitalClinicName: '',
            damageDescription: '',
            officialWorkingHours: '',
            injuredPersons: [],
            eyewitnesses: [],
            superiorName: '',
            superiorEmpNo: '',
            superiorDesignation: '',
            submittedDate: null,
            hodId: '',
            wshoId: '',
            ahodId: '',
            incidentTypeOptions: {
                dataTextField: 'value',
                dataValueField: 'code',
                dataSource: [],
                optionLabel: '-- Select Incident Type --'
            },
            sectorOptions: {
                dataTextField: 'value',
                dataValueField: 'code',
                dataSource: [],
                optionLabel: '-- Select Sector --'
            },
            lobOptions: {
                dataTextField: 'value',
                dataValueField: 'code',
                dataSource: [],
                optionLabel: '-- Select LOB --'
            },
            departmentOptions: {
                dataTextField: 'value',
                dataValueField: 'code',
                dataSource: [],
                optionLabel: '-- Select Department --'
            },
            locationOptions: {
                dataTextField: 'value',
                dataValueField: 'code',
                dataSource: [],
                optionLabel: '-- Select Location --'
            },
            hodOptions: {
                dataTextField: 'userName',
                dataValueField: 'userId',
                dataSource: [],
                optionLabel: '-- Select HOD --'
            },
            wshoOptions: {
                dataTextField: 'userName',
                dataValueField: 'userId',
                dataSource: [],
                optionLabel: '-- Select WSHO --'
            },
            ahodOptions: {
                dataTextField: 'userName',
                dataValueField: 'userId',
                dataSource: [],
                optionLabel: '-- Select Alternate HOD --'
            }
        };

        vm.cwshoList = [];
        vm.hsbuList = [];
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

        vm.canViewPartB = canViewPartB;
        vm.canViewPartC = canViewPartC;
        vm.canViewPartD = canViewPartD;
        vm.canEditPartB = canEditPartB;
        vm.canEditPartC = canEditPartC;
        vm.canEditPartD = canEditPartD;
        vm.isInjuryIncident = isInjuryIncident;
        vm.getStatusClass = getStatusClass;
        vm.getIncidentTypeText = getIncidentTypeText;
        vm.getInjuredCaseTypeText = getInjuredCaseTypeText;
        vm.getHsbuName = getHsbuName;
        vm.getCurrentDate = getCurrentDate;
        vm.submitPartB = submitPartB;
        vm.submitPartD = submitPartD;
        vm.openEmployeeSearch = openEmployeeSearch;
        vm.openEmployeeSearchForPartD = openEmployeeSearchForPartD;
        vm.removeCopyToPerson = removeCopyToPerson;
        vm.removeAdditionalCopyToFromPartD = removeAdditionalCopyToFromPartD;
        vm.addPersonInterviewed = addPersonInterviewed;
        vm.removePersonInterviewed = removePersonInterviewed;
        vm.addInjuryDetail = addInjuryDetail;
        vm.removeInjuryDetail = removeInjuryDetail;
        vm.addMedicalCertificate = addMedicalCertificate;
        vm.removeMedicalCertificate = removeMedicalCertificate;
        vm.savePartC = savePartC;
        vm.submitPartC = submitPartC;
        vm.closePartC = closePartC;
        vm.cancelPartD = cancelPartD;
        vm.cancel = cancel;

        init();

        function init() {
            var incidentId = getIncidentIdFromUrl();
            if (!incidentId) {
                vm.error = 'No incident ID provided';
                vm.loading = false;
                return;
            }

            loadCurrentUser()
                .then(function() {
                    return loadIncident(incidentId);
                })
                .then(function() {
                    return loadPartAData();
                })
                .then(function() {
                    return loadPartBData();
                })
                .then(function() {
                    return loadPartCData();
                })
                .then(function() {
                    return loadPartDData();
                })
                .catch(function(error) {
                    vm.error = error || 'Failed to load incident data';
                })
                .finally(function() {
                    vm.loading = false;
                });
        }

        function getIncidentIdFromUrl() {
            var url = $window.location.href;
            var params = new URLSearchParams($window.location.search);
            return params.get('id');
        }

        function loadCurrentUser() {
            return IncidentUpdateService.getCurrentUser()
                .then(function(user) {
                    vm.currentUser = {
                        userId: user.userId,
                        name: user.userName || user.displayName || 'User',
                        designation: user.designation || '',
                        sectorCode: user.sectorCode || ''
                    };
                });
        }

        function loadIncident(incidentId) {
            return IncidentUpdateService.getIncidentById(incidentId)
                .then(function(incident) {
                    console.log('Loaded incident data:', incident);
                    console.log('IncidentTypes array:', incident.incidentTypes);
                    console.log('IncidentTypes length:', incident.incidentTypes ? incident.incidentTypes.length : 'undefined');
                    vm.incident = incident;
                    return loadStatusName();
                })
                .then(function() {
                    determinePartBMode();
                });
        }

        function loadStatusName() {
            if (!vm.incident.status) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getStatusName(vm.incident.status)
                .then(function(statusName) {
                    vm.incident.statusName = statusName;
                    console.log('Status name loaded:', statusName);
                })
                .catch(function(error) {
                    console.error('Error loading status name:', error);
                    vm.incident.statusName = vm.incident.status;
                });
        }

        function loadPartAData() {
            // Part A is always visible (read-only)
            return Promise.all([
                loadPartALookups(),
                loadPartAWorkflowUsers()
            ]).then(function() {
                // Use $timeout to ensure mapping happens in Angular digest cycle
                $timeout(function() {
                    mapIncidentToPartA();
                }, 0);
            });
        }

        function loadPartALookups() {
            return Promise.all([
                IncidentUpdateService.getIncidentTypes().then(function(data) {
                    vm.partA.incidentTypeOptions.dataSource = data;
                }),
                IncidentUpdateService.getSectors().then(function(data) {
                    vm.partA.sectorOptions.dataSource = data;
                }),
                IncidentUpdateService.getLOBs(vm.incident.sectorCode || vm.incident.sbaCode).then(function(data) {
                    vm.partA.lobOptions.dataSource = data;
                }).catch(function() {
                    vm.partA.lobOptions.dataSource = [];
                }),
                IncidentUpdateService.getDepartments(
                    vm.incident.sectorCode || vm.incident.sbaCode,
                    vm.incident.lobCode || vm.incident.sbuCode
                ).then(function(data) {
                    vm.partA.departmentOptions.dataSource = data;
                }).catch(function() {
                    vm.partA.departmentOptions.dataSource = [];
                }),
                IncidentUpdateService.getLocations(
                    vm.incident.sectorCode || vm.incident.sbaCode,
                    vm.incident.lobCode || vm.incident.sbuCode,
                    vm.incident.departmentCode || vm.incident.department
                ).then(function(data) {
                    vm.partA.locationOptions.dataSource = data;
                }).catch(function() {
                    vm.partA.locationOptions.dataSource = [];
                })
            ]);
        }

        function loadPartAWorkflowUsers() {
            if (!vm.incident.sectorCode && !vm.incident.sbaCode) {
                return Promise.resolve();
            }

            var sectorCode = vm.incident.sectorCode || vm.incident.sbaCode;
            var lobCode = vm.incident.lobCode || vm.incident.sbuCode;

            return Promise.all([
                IncidentUpdateService.getHODs(sectorCode, lobCode).then(function(data) {
                    vm.partA.hodOptions.dataSource = data;
                }).catch(function() {
                    vm.partA.hodOptions.dataSource = [];
                }),
                IncidentUpdateService.getWSHOs(sectorCode, lobCode).then(function(data) {
                    vm.partA.wshoOptions.dataSource = data;
                }).catch(function() {
                    vm.partA.wshoOptions.dataSource = [];
                }),
                IncidentUpdateService.getAHODs(sectorCode, lobCode).then(function(data) {
                    vm.partA.ahodOptions.dataSource = data;
                }).catch(function() {
                    vm.partA.ahodOptions.dataSource = [];
                })
            ]);
        }

        function mapIncidentToPartA() {
            if (!vm.incident) return;

            console.log('Mapping incident to Part A', vm.incident);

            // Basic incident details - extract from incidentTypes array
            if (vm.incident.incidentTypes && vm.incident.incidentTypes.length > 0) {
                // Get the first incident type code
                // API returns {type: "...", description: "..."} OR {code: "...", value: "..."}
                var incType = vm.incident.incidentTypes[0];
                vm.partA.incidentType = incType.type || incType.code || '';
                console.log('Setting incident type to:', vm.partA.incidentType, 'from:', incType);
            } else if (vm.incident.incidentType) {
                // Fallback: if incidentType is directly on incident object
                var incType = vm.incident.incidentType;
                vm.partA.incidentType = (typeof incType === 'object' && incType.code) ? incType.code : incType;
                console.log('Setting incident type from incident.incidentType:', vm.partA.incidentType);
            } else {
                vm.partA.incidentType = '';
                console.log('No incident types found in incident data');
            }
            vm.partA.incidentOther = vm.incident.incidentOther || '';

            // Parse dates and times
            if (vm.incident.incidentDate) {
                vm.partA.incidentDate = new Date(vm.incident.incidentDate);
            }
            if (vm.incident.incidentTime) {
                var timeParts = vm.incident.incidentTime.split(':');
                if (timeParts.length >= 2) {
                    var timeDate = new Date();
                    timeDate.setHours(parseInt(timeParts[0], 10));
                    timeDate.setMinutes(parseInt(timeParts[1], 10));
                    vm.partA.incidentTime = timeDate;
                }
            }

            // Organization - extract code if it's an object
            var sectorCode = vm.incident.sectorCode || vm.incident.sbaCode || '';
            vm.partA.sectorCode = (typeof sectorCode === 'object' && sectorCode.code) ? sectorCode.code : sectorCode;

            var lobCode = vm.incident.lobCode || vm.incident.sbuCode || '';
            vm.partA.lobCode = (typeof lobCode === 'object' && lobCode.code) ? lobCode.code : lobCode;

            var departmentCode = vm.incident.departmentCode || vm.incident.department || '';
            vm.partA.departmentCode = (typeof departmentCode === 'object' && departmentCode.code) ? departmentCode.code : departmentCode;

            var locationCode = vm.incident.locationCode || vm.incident.location || '';
            vm.partA.locationCode = (typeof locationCode === 'object' && locationCode.code) ? locationCode.code : locationCode;

            vm.partA.exactLocation = vm.incident.exactLocation || '';

            // Description
            vm.partA.incidentDescription = vm.incident.incidentDesc || vm.incident.incidentDescription || '';
            vm.partA.damageDescription = vm.incident.damageDescription || '';

            // Boolean flags
            vm.partA.hasEyewitness = vm.incident.hasEyewitness || vm.incident.anyEyewitness || 'N';
            vm.partA.hasDamage = vm.incident.hasDamage || 'N';
            vm.partA.workingOvertime = vm.incident.isWorkingOvertime || vm.incident.workingOvertime || 'N';
            vm.partA.isJobRelated = vm.incident.isJobrelated || vm.incident.isJobRelated || 'N';

            // Additional info
            vm.partA.hospitalClinicName = vm.incident.examinedHospitalClinicName || vm.incident.hospitalClinicName || '';
            vm.partA.officialWorkingHours = vm.incident.officialWorkingHrs || vm.incident.officialWorkingHours || '';

            // Injured persons
            vm.partA.injuredPersons = vm.incident.injuredPersons || [];

            // Eyewitnesses
            vm.partA.eyewitnesses = vm.incident.eyewitnesses || [];

            // Submitter info
            vm.partA.superiorName = vm.incident.superiorName || '';
            vm.partA.superiorEmpNo = vm.incident.superiorEmpNo || '';
            vm.partA.superiorDesignation = vm.incident.superiorDesignation || '';

            if (vm.incident.createdDate || vm.incident.submittedDate) {
                vm.partA.submittedDate = new Date(vm.incident.createdDate || vm.incident.submittedDate);
            }

            // Workflow
            vm.partA.hodId = vm.incident.hodId || '';
            vm.partA.wshoId = vm.incident.wshoId || '';
            vm.partA.ahodId = vm.incident.ahodId || '';

            console.log('Part A data after mapping:', vm.partA);

            // Force Angular digest cycle to update Kendo UI controls
            if ($scope.$root && $scope.$root.$$phase !== '$apply' && $scope.$root.$$phase !== '$digest') {
                $scope.$applyAsync();
            }

            // Additional delay to ensure Kendo widgets are fully initialized
            $timeout(function() {
                console.log('Attempting to set Kendo widget values...');
                console.log('vm.partA at widget set time:', vm.partA);

                // Try to get Kendo widgets and set values manually
                var incidentTypeWidget = $('#partA_incidentType').data('kendoDropDownList');
                if (incidentTypeWidget) {
                    console.log('Found incidentType widget');
                    console.log('  - Current value:', incidentTypeWidget.value());
                    console.log('  - DataSource items:', incidentTypeWidget.dataSource.data().length);
                    console.log('  - Setting value to:', vm.partA.incidentType);

                    // Set dataSource and value
                    if (vm.partA.incidentTypeOptions.dataSource.length > 0) {
                        incidentTypeWidget.setDataSource(new kendo.data.DataSource({
                            data: vm.partA.incidentTypeOptions.dataSource
                        }));
                    }
                    incidentTypeWidget.value(vm.partA.incidentType);
                    incidentTypeWidget.trigger('change');
                    console.log('  - Value after set:', incidentTypeWidget.value());
                } else {
                    console.log('incidentType widget not found - checking element exists:', $('#partA_incidentType').length > 0);
                }

                var sectorWidget = $('#partA_sector').data('kendoDropDownList');
                if (sectorWidget) {
                    console.log('Found sector widget, setting value:', vm.partA.sectorCode);
                    if (vm.partA.sectorOptions.dataSource.length > 0) {
                        sectorWidget.setDataSource(new kendo.data.DataSource({
                            data: vm.partA.sectorOptions.dataSource
                        }));
                    }
                    sectorWidget.value(vm.partA.sectorCode);
                    sectorWidget.trigger('change');
                }

                var lobWidget = $('#partA_lob').data('kendoDropDownList');
                if (lobWidget) {
                    console.log('Found LOB widget, setting value:', vm.partA.lobCode);
                    if (vm.partA.lobOptions.dataSource.length > 0) {
                        lobWidget.setDataSource(new kendo.data.DataSource({
                            data: vm.partA.lobOptions.dataSource
                        }));
                    }
                    lobWidget.value(vm.partA.lobCode);
                    lobWidget.trigger('change');
                }

                // Date and time pickers
                var dateWidget = $('#partA_incidentDate').data('kendoDatePicker');
                if (dateWidget && vm.partA.incidentDate) {
                    console.log('Found date widget, setting value:', vm.partA.incidentDate);
                    dateWidget.value(vm.partA.incidentDate);
                }

                var timeWidget = $('#partA_incidentTime').data('kendoTimePicker');
                if (timeWidget && vm.partA.incidentTime) {
                    console.log('Found time widget, setting value:', vm.partA.incidentTime);
                    timeWidget.value(vm.partA.incidentTime);
                }
            }, 1000);
        }

        function loadPartBData() {
            if (!canViewPartB()) {
                return Promise.resolve();
            }

            return Promise.all([
                loadInjuredCaseTypes(),
                loadWSHOs(),
                loadAlternateWSHOs(),
                loadEmailToList()
            ]);
        }

        function loadInjuredCaseTypes() {
            return IncidentUpdateService.getInjuredCaseTypes()
                .then(function(data) {
                    vm.injuredCaseTypes = data;
                    if (vm.injuredCaseTypes.length > 0 && !vm.partB.injuredCaseType) {
                        vm.partB.injuredCaseType = vm.injuredCaseTypes[0].code;
                    }
                })
                .catch(function(error) {
                    console.error('Failed to load injured case types:', error);
                });
        }

        function loadWSHOs() {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function(data) {
                    vm.wshoList = data;
                })
                .catch(function(error) {
                    console.error('Failed to load WSHOs:', error);
                });
        }

        function loadAlternateWSHOs() {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getAlternateWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function(data) {
                    vm.alternateWshoList = data;
                })
                .catch(function(error) {
                    console.error('Failed to load alternate WSHOs:', error);
                });
        }

        function loadEmailToList() {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getPartBCopyToList(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function(data) {
                    vm.emailToList = data.map(function(person) {
                        return {
                            id: person.id,
                            name: person.name,
                            designation: person.designation || '',
                            selected: true
                        };
                    });
                })
                .catch(function(error) {
                    console.error('Failed to load email to list:', error);
                });
        }

        function determinePartBMode() {
            if (vm.incident.status === '01') {
                vm.partB.isReadOnly = false;
            } else if (parseInt(vm.incident.status) > 1) {
                vm.partB.isReadOnly = true;
                loadPartBReadOnlyData();
            }
        }

        function loadPartBReadOnlyData() {
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                var partBWorkflow = vm.incident.workflows.find(function(w) {
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

        function canViewPartB() {
            return vm.incident.status && parseInt(vm.incident.status) >= 1;
        }

        function canViewPartC() {
            return vm.incident.status && parseInt(vm.incident.status) >= 2;
        }

        function canEditPartB() {
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

        function getStatusClass() {
            var status = parseInt(vm.incident.status);
            if (status === 0) return 'bg-gray-200 text-gray-800';
            if (status >= 1 && status <= 3) return 'bg-yellow-200 text-yellow-800';
            if (status >= 4 && status <= 7) return 'bg-blue-200 text-blue-800';
            if (status === 8) return 'bg-green-200 text-green-800';
            return 'bg-gray-200 text-gray-800';
        }

        function getIncidentTypeText() {
            if (!vm.incident.incidentTypes || vm.incident.incidentTypes.length === 0) {
                return 'N/A';
            }
            return vm.incident.incidentTypes.join(', ');
        }

        function getInjuredCaseTypeText() {
            if (!vm.partB.injuredCaseType || !vm.injuredCaseTypes.length) {
                return 'N/A';
            }

            var caseType = vm.injuredCaseTypes.find(function(ct) {
                return ct.code === vm.partB.injuredCaseType;
            });

            return caseType ? caseType.value : vm.partB.injuredCaseType;
        }

        function getCurrentDate() {
            var today = new Date();
            var day = String(today.getDate()).padStart(2, '0');
            var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var month = monthNames[today.getMonth()];
            var year = today.getFullYear();
            return day + '-' + month + '-' + year;
        }

        function submitPartB() {
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
                .filter(function(person) { return person.selected; })
                .map(function(person) { return person.id; });

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
                .then(function(response) {
                    if (response.success) {
                        alert('Part B submitted successfully!');
                        $window.location.href = '/Home/Index';
                    } else {
                        vm.partB.validationMessage = response.message || 'Failed to submit Part B';
                    }
                })
                .catch(function(error) {
                    vm.partB.validationMessage = error.message || 'An error occurred while submitting Part B';
                })
                .finally(function() {
                    vm.partB.submitting = false;
                });
        }

        function openEmployeeSearch(context) {
            console.log('Employee search not yet implemented');
        }

        function removeCopyToPerson(index) {
            vm.partB.additionalCopyToList.splice(index, 1);
        }

        function loadPartCData() {
            if (!canViewPartC()) {
                return Promise.resolve();
            }

            determinePartCMode();

            if (vm.partC.isReadOnly) {
                return loadPartCReadOnlyData();
            }

            return Promise.all([
                loadPartCLookups(),
                loadCWSHOs()
            ]);
        }

        function loadPartCLookups() {
            var promises = [
                IncidentUpdateService.getNatureOfInjury().then(function(data) {
                    vm.natureOfInjury = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getHeadNeckTorso().then(function(data) {
                    vm.headNeckTorso = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getUpperLimbs().then(function(data) {
                    vm.upperLimbs = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getLowerLimbs().then(function(data) {
                    vm.lowerLimbs = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getIncidentClass().then(function(data) {
                    vm.incidentClass = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getIncidentAgent().then(function(data) {
                    vm.incidentAgent = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getUnsafeConditions().then(function(data) {
                    vm.unsafeConditions = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getUnsafeActs().then(function(data) {
                    vm.unsafeActs = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getContributingFactors().then(function(data) {
                    vm.contributingFactors = data.map(function(item) { return { code: item.code, value: item.value, selected: false }; });
                }),
                IncidentUpdateService.getNegligentOptions().then(function(data) {
                    vm.negligentOptions = data;
                })
            ];

            return Promise.all(promises).catch(function(error) {
                console.error('Failed to load Part C lookups:', error);
            });
        }

        function loadCWSHOs() {
            if (!vm.incident.sbaCode || !vm.incident.sbuCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getCWSHOs(
                vm.incident.sbaCode,
                vm.incident.sbuCode,
                vm.incident.department,
                vm.incident.location
            )
                .then(function(data) {
                    vm.cwshoList = data;
                })
                .catch(function(error) {
                    console.error('Failed to load CWSHOs:', error);
                });
        }

        function determinePartCMode() {
            if (vm.incident.status === '02') {
                vm.partC.isReadOnly = false;
            } else if (parseInt(vm.incident.status) > 2) {
                vm.partC.isReadOnly = true;
            }
        }

        function loadPartCReadOnlyData() {
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                var partCWorkflow = vm.incident.workflows.find(function(w) {
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

        function canEditPartC() {
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

        function isInjuryIncident() {
            if (!vm.incident || !vm.incident.incidentTypes) {
                return false;
            }
            return vm.incident.incidentTypes.indexOf('1') !== -1;
        }

        function addPersonInterviewed() {
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

        function removePersonInterviewed(index) {
            vm.partC.personsInterviewed.splice(index, 1);
        }

        function addInjuryDetail() {
            if (!vm.partC.injuryDetail.injuredPersonId) {
                alert('Please select injured person');
                return;
            }

            var selectedNature = vm.natureOfInjury.filter(function(n) { return n.selected; }).map(function(n) { return n.value; }).join(', ');
            var selectedHead = vm.headNeckTorso.filter(function(h) { return h.selected; }).map(function(h) { return h.value; }).join(', ');
            var selectedUpper = vm.upperLimbs.filter(function(u) { return u.selected; }).map(function(u) { return u.value; }).join(', ');
            var selectedLower = vm.lowerLimbs.filter(function(l) { return l.selected; }).map(function(l) { return l.value; }).join(', ');

            var bodyParts = [selectedHead, selectedUpper, selectedLower].filter(function(p) { return p; }).join('; ');

            var injuredPerson = vm.incident.injuredPersons.find(function(p) { return p.employeeNo === vm.partC.injuryDetail.injuredPersonId; });

            vm.partC.injuryDetails.push({
                injuredPersonId: vm.partC.injuryDetail.injuredPersonId,
                injuredPersonName: injuredPerson ? injuredPerson.name : '',
                natureOfInjury: selectedNature,
                bodyParts: bodyParts,
                description: vm.partC.injuryDetail.description || '',
                natureOfInjuryList: vm.natureOfInjury.filter(function(n) { return n.selected; }).map(function(n) { return n.code; }),
                headNeckTorsoList: vm.headNeckTorso.filter(function(h) { return h.selected; }).map(function(h) { return h.code; }),
                upperLimbsList: vm.upperLimbs.filter(function(u) { return u.selected; }).map(function(u) { return u.code; }),
                lowerLimbsList: vm.lowerLimbs.filter(function(l) { return l.selected; }).map(function(l) { return l.code; })
            });

            vm.partC.injuryDetail = {};
            vm.natureOfInjury.forEach(function(n) { n.selected = false; });
            vm.headNeckTorso.forEach(function(h) { h.selected = false; });
            vm.upperLimbs.forEach(function(u) { u.selected = false; });
            vm.lowerLimbs.forEach(function(l) { l.selected = false; });
        }

        function removeInjuryDetail(index) {
            vm.partC.injuryDetails.splice(index, 1);
        }

        function addMedicalCertificate() {
            if (!vm.partC.medicalCert.injuredPersonId) {
                alert('Please select injured person');
                return;
            }

            if (!vm.partC.medicalCert.numberOfDays || vm.partC.medicalCert.numberOfDays <= 0) {
                alert('Please enter number of days');
                return;
            }

            var injuredPerson = vm.incident.injuredPersons.find(function(p) { return p.employeeNo === vm.partC.medicalCert.injuredPersonId; });

            vm.partC.medicalCertificates.push({
                injuredPersonId: vm.partC.medicalCert.injuredPersonId,
                injuredPersonName: injuredPerson ? injuredPerson.name : '',
                fromDate: vm.partC.medicalCert.fromDate || '',
                toDate: vm.partC.medicalCert.toDate || '',
                numberOfDays: vm.partC.medicalCert.numberOfDays,
                hasAttachment: false
            });

            vm.partC.medicalCert = {};
        }

        function removeMedicalCertificate(index) {
            vm.partC.medicalCertificates.splice(index, 1);
        }

        function savePartC() {
            vm.partC.validationMessage = '';
            vm.partC.saving = true;

            var partCData = buildPartCData();

            IncidentUpdateService.savePartC(partCData)
                .then(function(response) {
                    if (response.success) {
                        alert('Part C saved successfully!');
                    } else {
                        vm.partC.validationMessage = response.message || 'Failed to save Part C';
                    }
                })
                .catch(function(error) {
                    vm.partC.validationMessage = error.message || 'An error occurred while saving Part C';
                })
                .finally(function() {
                    vm.partC.saving = false;
                });
        }

        function submitPartC() {
            vm.partC.validationMessage = '';

            if (!validatePartC()) {
                return;
            }

            vm.partC.submitting = true;

            var partCData = buildPartCData();

            IncidentUpdateService.submitPartC(partCData)
                .then(function(response) {
                    if (response.success) {
                        alert('Part C submitted successfully to HOD!');
                        $window.location.href = '/Home/Index';
                    } else {
                        vm.partC.validationMessage = response.message || 'Failed to submit Part C';
                    }
                })
                .catch(function(error) {
                    vm.partC.validationMessage = error.message || 'An error occurred while submitting Part C';
                })
                .finally(function() {
                    vm.partC.submitting = false;
                });
        }

        function closePartC() {
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
                partCData: buildPartCData()
            };

            IncidentUpdateService.closePartC(closeData)
                .then(function(response) {
                    if (response.success) {
                        alert('Incident closed successfully!');
                        $window.location.href = '/Home/Index';
                    } else {
                        vm.partC.validationMessage = response.message || 'Failed to close incident';
                    }
                })
                .catch(function(error) {
                    vm.partC.validationMessage = error.message || 'An error occurred while closing incident';
                })
                .finally(function() {
                    vm.partC.closing = false;
                });
        }

        function validatePartC() {
            if (!vm.partC.isNegligent) {
                vm.partC.validationMessage = 'Negligent field is required (ERR-136)';
                return false;
            }

            if (vm.partC.isNegligent === 'Y' && !vm.partC.negligentComments) {
                vm.partC.validationMessage = 'Negligent comments required when employee is negligent (ERR-137)';
                return false;
            }

            if (!vm.partC.whatHappenedAndWhy) {
                vm.partC.validationMessage = 'What happened and why is required (ERR-138)';
                return false;
            }

            if (!vm.partC.recommendedActions) {
                vm.partC.validationMessage = 'Recommended actions is required (ERR-139)';
                return false;
            }

            var hasIncidentClass = vm.incidentClass.some(function(c) { return c.selected; });
            if (!hasIncidentClass) {
                vm.partC.validationMessage = 'At least one incident classification is required';
                return false;
            }

            var hasUnsafeCondition = vm.unsafeConditions.some(function(c) { return c.selected; });
            var hasUnsafeAct = vm.unsafeActs.some(function(a) { return a.selected; });
            if (!hasUnsafeCondition && !hasUnsafeAct) {
                vm.partC.validationMessage = 'At least one unsafe condition OR unsafe act is required';
                return false;
            }

            return true;
        }

        function buildPartCData() {
            return {
                incidentId: vm.incident.incidentId,
                isNegligent: vm.partC.isNegligent,
                negligentComments: vm.partC.negligentComments || '',
                needsRiskAssessmentReview: vm.partC.needsRiskAssessmentReview,
                riskAssessmentComments: vm.partC.riskAssessmentComments || '',
                whatHappenedAndWhy: vm.partC.whatHappenedAndWhy,
                recommendedActions: vm.partC.recommendedActions,
                additionalComments: vm.partC.additionalComments || '',
                personsInterviewed: vm.partC.personsInterviewed,
                injuryDetails: vm.partC.injuryDetails.map(function(injury) {
                    return {
                        injuredPersonId: injury.injuredPersonId,
                        injuredPersonName: injury.injuredPersonName,
                        natureOfInjury: injury.natureOfInjuryList,
                        headNeckTorso: injury.headNeckTorsoList,
                        upperLimbs: injury.upperLimbsList,
                        lowerLimbs: injury.lowerLimbsList,
                        description: injury.description
                    };
                }),
                medicalCertificates: vm.partC.medicalCertificates,
                incidentClassList: vm.incidentClass.filter(function(c) { return c.selected; }).map(function(c) { return c.code; }),
                incidentAgentList: vm.incidentAgent.filter(function(a) { return a.selected; }).map(function(a) { return a.code; }),
                unsafeConditionsList: vm.unsafeConditions.filter(function(c) { return c.selected; }).map(function(c) { return c.code; }),
                unsafeActsList: vm.unsafeActs.filter(function(a) { return a.selected; }).map(function(a) { return a.code; }),
                contributingFactorsList: vm.contributingFactors.filter(function(f) { return f.selected; }).map(function(f) { return f.code; })
            };
        }

        // ========== Part D Functions ==========

        function canViewPartD() {
            return vm.incident && vm.incident.status && parseInt(vm.incident.status) >= 3;
        }

        function canEditPartD() {
            if (!vm.incident || vm.incident.status !== '03') return false;
            if (!vm.currentUser || !vm.currentUser.userId) return false;

            // HOD or Alternate HOD can edit Part D
            if (vm.incident.hodId === vm.currentUser.userId) return true;
            if (vm.incident.ahodId === vm.currentUser.userId) return true;

            return false;
        }

        function loadPartDData() {
            if (!canViewPartD()) {
                return Promise.resolve();
            }

            vm.partD.currentDate = getCurrentDate();
            determinePartDMode();

            if (vm.partD.isReadOnly) {
                return loadPartDReadOnlyData();
            }

            return Promise.all([
                loadHSBUs(),
                loadPartDEmailToList()
            ]);
        }

        function determinePartDMode() {
            vm.partD.isReadOnly = vm.incident.status !== '03' || !canEditPartD();
        }

        function loadPartDReadOnlyData() {
            // Load read-only data from incident
            vm.partD.comments = vm.incident.partDComments || '';
            vm.partD.hsbuId = vm.incident.hsbuId || '';
            vm.partD.submitterName = vm.incident.partDSubmitterName || '';
            vm.partD.submitterEmpId = vm.incident.partDSubmitterEmpId || '';
            vm.partD.submitterDesignation = vm.incident.partDSubmitterDesignation || '';
            vm.partD.submittedDate = vm.incident.partDSubmittedDate || '';

            // Load HSBU list and email to list for display purposes
            return Promise.all([
                loadHSBUs(),
                loadPartDEmailToList()
            ]);
        }

        function loadHSBUs() {
            if (!vm.incident.sectorCode || !vm.incident.lobCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getHSBUs(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode || '',
                vm.incident.locationCode || ''
            ).then(function(data) {
                vm.hsbuList = data.map(function(item) {
                    return {
                        userId: item.userId || item.employeeNo,
                        userName: item.userName || item.name,
                        designation: item.designation || ''
                    };
                });
            }).catch(function(error) {
                console.error('Error loading HSBUs:', error);
                vm.hsbuList = [];
            });
        }

        function loadPartDEmailToList() {
            if (!vm.incident.sectorCode || !vm.incident.lobCode) {
                return Promise.resolve();
            }

            return IncidentUpdateService.getPartDCopyToList(
                vm.incident.sectorCode,
                vm.incident.lobCode,
                vm.incident.departmentCode || '',
                vm.incident.locationCode || ''
            ).then(function(data) {
                vm.partD.emailToList = data.map(function(item) {
                    return {
                        userId: item.userId || item.employeeNo,
                        userName: item.userName || item.name,
                        designation: item.designation || '',
                        selected: true  // Default all selected
                    };
                });
            }).catch(function(error) {
                console.error('Error loading email to list:', error);
                vm.partD.emailToList = [];
            });
        }

        function getHsbuName(hsbuId) {
            if (!hsbuId || !vm.hsbuList || vm.hsbuList.length === 0) {
                return 'N/A';
            }
            var hsbu = vm.hsbuList.find(function(h) {
                return h.userId === hsbuId;
            });
            return hsbu ? hsbu.userName + ' (' + hsbu.userId + ')' : 'N/A';
        }

        function openEmployeeSearchForPartD() {
            // TODO: Implement employee search modal
            // For now, just show an alert
            alert('Employee search functionality will be implemented');
        }

        function removeAdditionalCopyToFromPartD(index) {
            vm.partD.additionalCopyToList.splice(index, 1);
        }

        function submitPartD() {
            vm.partD.validationMessage = '';
            vm.partD.successMessage = '';

            // Validation
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
                .filter(function(person) { return person.selected; })
                .map(function(person) { return person.userId; });

            var submitData = {
                incidentId: vm.incident.incidentId,
                comments: vm.partD.comments,
                hsbuId: vm.partD.hsbuId,
                emailToList: selectedEmailTo,
                additionalCopyToList: vm.partD.additionalCopyToList.map(function(person) {
                    return {
                        employeeNo: person.employeeId,
                        name: person.name,
                        designation: person.designation
                    };
                })
            };

            IncidentUpdateService.submitPartD(submitData)
                .then(function(response) {
                    if (response.success) {
                        vm.partD.successMessage = response.message || 'Part D submitted successfully to HSBU';
                        setTimeout(function() {
                            $window.location.href = '/Home/Index';
                        }, 2000);
                    } else {
                        vm.partD.validationMessage = response.message || 'Failed to submit Part D';
                    }
                })
                .catch(function(error) {
                    vm.partD.validationMessage = error.message || 'An error occurred while submitting Part D';
                })
                .finally(function() {
                    vm.partD.isSubmitting = false;
                });
        }

        function cancelPartD() {
            if (confirm('Are you sure you want to cancel? Any unsaved changes will be lost.')) {
                $window.location.href = '/Home/Index';
            }
        }

        function cancel() {
            if (confirm('Are you sure you want to cancel? Any unsaved changes will be lost.')) {
                $window.location.href = '/Home/Index';
            }
        }
    }
})();
