(function () {
    'use strict';

    angular
        .module('incidentUpdateApp')
        .factory('PartAService', PartAService);

    PartAService.$inject = ['$timeout', '$sce', '$q', 'IncidentUpdateService'];

    function PartAService($timeout, $sce, $q, IncidentUpdateService) {
        var service = {
            initializePartA: initializePartA,
            loadPartAData: loadPartAData,
            onIncidentTypeChange: onIncidentTypeChange,
            addInjuredPerson: addInjuredPerson,
            deleteInjuredPerson: deleteInjuredPerson,
            searchInjuredPerson: searchInjuredPerson,
            addEyeWitness: addEyeWitness,
            deleteEyeWitness: deleteEyeWitness,
            searchEyeWitness: searchEyeWitness,
            getIncidentTypeName: getIncidentTypeName,
            getSectorName: getSectorName,
            getLOBName: getLOBName,
            getDepartmentName: getDepartmentName,
            getLocationName: getLocationName
        };

        return service;

        function initializePartA(vm) {
            vm.injuredPerson = {};
            vm.eyeWitness = {};
            vm.injuredPersonType = 'E';
            vm.showOtherIncidentType = false;
            vm.maxDate = new Date();

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
                damageDescription: '',
                hasEyewitness: 'N',
                hasDamage: 'N',
                workingOvertime: 'N',
                isJobRelated: 'N',
                hospitalClinicName: '',
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

            vm.injuredGridOptions = {
                columns: [
                    { field: 'name', title: 'Name', width: 200 },
                    { field: 'employeeNo', title: 'Employee No/NRIC', width: 150 },
                    { field: 'race', title: 'Race', width: 100 },
                    { field: 'gender', title: 'Gender', width: 100 },
                    { field: 'age', title: 'Age', width: 80 },
                    { field: 'company', title: 'Company', width: 120 },
                    { field: 'contactNo', title: 'Contact No', width: 120 },
                    { field: 'nationality', title: 'Nationality', width: 120 },
                    { field: 'designation', title: 'Designation', width: 150 },
                    { field: 'employmentType', title: 'Employment Type', width: 150 },
                    { field: 'dateOfEmployment', title: 'Date of Employment', width: 150 },
                    {
                        command: {
                            text: 'Delete',
                            click: function (e) {
                                deleteInjuredPerson(vm, e);
                            }
                        },
                        title: ' ',
                        width: 100
                    }
                ],
                selectable: false,
                scrollable: true,
                sortable: true,
                pageable: false
            };

            vm.witnessGridOptions = {
                columns: [
                    { field: 'name', title: 'Name', width: 250 },
                    { field: 'employeeNo', title: 'Employee No', width: 150 },
                    { field: 'designation', title: 'Designation', width: 200 },
                    { field: 'contactNo', title: 'Contact Number', width: 150 },
                    {
                        command: {
                            text: 'Delete',
                            click: function (e) {
                                deleteEyeWitness(vm, e);
                            }
                        },
                        title: ' ',
                        width: 100
                    }
                ],
                selectable: false,
                scrollable: true,
                sortable: true,
                pageable: false
            };
        }

        function loadPartAData(vm) {
            return $q.all([
                loadPartALookups(vm),
                loadPartAWorkflowUsers(vm)
            ]).then(function () {

                mapIncidentToPartA(vm);

                $timeout(function () {
                    refreshKendoDropDowns(vm);
                }, 0);
            });
        }

        function loadPartALookups(vm) {
            return $q.all([
                IncidentUpdateService.getIncidentTypes().then(function (data) {
                    vm.partA.incidentTypeOptions.dataSource = data;
                }),
                IncidentUpdateService.getSectors().then(function (data) {
                    vm.partA.sectorOptions.dataSource = data;
                }),
                IncidentUpdateService.getLOBs(vm.incident.sectorCode || vm.incident.sbaCode).then(function (data) {
                    vm.partA.lobOptions.dataSource = data;
                }).catch(function () {
                    vm.partA.lobOptions.dataSource = [];
                }),
                IncidentUpdateService.getDepartments(
                    vm.incident.sectorCode || vm.incident.sbaCode,
                    vm.incident.lobCode || vm.incident.sbuCode
                ).then(function (data) {
                    vm.partA.departmentOptions.dataSource = data;
                }).catch(function () {
                    vm.partA.departmentOptions.dataSource = [];
                }),
                IncidentUpdateService.getLocations(
                    vm.incident.sectorCode || vm.incident.sbaCode,
                    vm.incident.lobCode || vm.incident.sbuCode,
                    vm.incident.departmentCode || vm.incident.department
                ).then(function (data) {
                    vm.partA.locationOptions.dataSource = data;
                }).catch(function () {
                    vm.partA.locationOptions.dataSource = [];
                })
            ]);
        }

        function loadPartAWorkflowUsers(vm) {
            return $q.all([
                IncidentUpdateService.getHODs(
                    vm.incident.sectorCode || vm.incident.sbaCode,
                    vm.incident.lobCode || vm.incident.sbuCode,
                    vm.incident.departmentCode || vm.incident.department,
                    vm.incident.locationCode || vm.incident.location
                ).then(function (data) {
                    vm.partA.hodOptions.dataSource = data;
                }).catch(function () {
                    vm.partA.hodOptions.dataSource = [];
                }),
                IncidentUpdateService.getAHODs(
                    vm.incident.sectorCode || vm.incident.sbaCode,
                    vm.incident.lobCode || vm.incident.sbuCode,
                    vm.incident.departmentCode || vm.incident.department,
                    vm.incident.locationCode || vm.incident.location
                ).then(function (data) {
                    vm.partA.ahodOptions.dataSource = data;
                }).catch(function () {
                    vm.partA.ahodOptions.dataSource = [];
                })
            ]);
        }

        function mapIncidentToPartA(vm) {
            if (!vm.incident) return;

            if (vm.incident.incidentTypes && vm.incident.incidentTypes.length > 0) {
                var incType = vm.incident.incidentTypes[0];
                vm.partA.incidentType = incType.type || incType.code || '';
            } else if (vm.incident.incidentType) {
                var incType = vm.incident.incidentType;
                vm.partA.incidentType = (typeof incType === 'object' && incType.code) ? incType.code : incType;
            } else {
                vm.partA.incidentType = '';
            }
            vm.partA.incidentOther = vm.incident.incidentOther || '';

            if (vm.incident.incidentDate) {
                vm.partA.incidentDate = new Date(vm.incident.incidentDate);
            }
            if (vm.incident.incidentTime) {
                var timeStr = vm.incident.incidentTime.toString();
                var timeParts;

                if (timeStr.indexOf(':') > -1) {
                    timeParts = timeStr.split(':');
                } else if (timeStr.length === 4) {
                    timeParts = [timeStr.substring(0, 2), timeStr.substring(2, 4)];
                } else {
                    timeParts = [];
                }

                if (timeParts.length >= 2) {
                    var timeDate = new Date();
                    timeDate.setHours(parseInt(timeParts[0], 10));
                    timeDate.setMinutes(parseInt(timeParts[1], 10));
                    vm.partA.incidentTime = timeDate;
                }
            }

            var sectorCode = vm.incident.sectorCode || vm.incident.sbaCode || '';
            vm.partA.sectorCode = (typeof sectorCode === 'object' && sectorCode.code) ? sectorCode.code : sectorCode;

            var lobCode = vm.incident.lobCode || vm.incident.sbuCode || '';
            vm.partA.lobCode = (typeof lobCode === 'object' && lobCode.code) ? lobCode.code : lobCode;

            var departmentCode = vm.incident.departmentCode || vm.incident.department || '';
            vm.partA.departmentCode = (typeof departmentCode === 'object' && departmentCode.code) ? departmentCode.code : departmentCode;

            var locationCode = vm.incident.locationCode || vm.incident.location || '';
            vm.partA.locationCode = (typeof locationCode === 'object' && locationCode.code) ? locationCode.code : locationCode;

            vm.partA.exactLocation = vm.incident.exactLocation || '';

            vm.partA.incidentDescription = vm.incident.incidentDesc || vm.incident.incidentDescription || '';
            vm.partA.damageDescription = vm.incident.damageDescription || '';

            vm.partA.hasEyewitness = vm.incident.hasEyewitness || vm.incident.anyEyewitness || 'N';
            vm.partA.hasDamage = vm.incident.hasDamage || 'N';
            vm.partA.workingOvertime = vm.incident.isWorkingOvertime || vm.incident.workingOvertime || 'N';
            vm.partA.isJobRelated = vm.incident.isJobrelated || vm.incident.isJobRelated || 'N';

            vm.partA.hospitalClinicName = vm.incident.examinedHospitalClinicName || vm.incident.hospitalClinicName || '';
            vm.partA.officialWorkingHours = vm.incident.officialWorkingHrs || vm.incident.officialWorkingHours || '';

            vm.partA.injuredPersons = vm.incident.injuredPersons || [];

            vm.partA.eyewitnesses = vm.incident.eyewitnesses || [];

            vm.partA.superiorName = vm.incident.superiorName || '';
            vm.partA.superiorEmpNo = vm.incident.superiorEmpNo || '';
            vm.partA.superiorDesignation = vm.incident.superiorDesignation || '';

            var partAWorkflows = [];
            if (vm.incident.workflows && vm.incident.workflows.length > 0) {
                partAWorkflows = vm.incident.workflows.filter(function (w) {
                    return w.actionCode === '01';
                });
            }

            if (partAWorkflows.length > 0) {
                partAWorkflows.sort(function (a, b) {
                    var dateA = new Date(a.date || 0);
                    var dateB = new Date(b.date || 0);
                    return dateB - dateA;
                });
                vm.partA.submittedDate = partAWorkflows[0].date ? new Date(partAWorkflows[0].date) : null;
            } else if (vm.incident.createdDate || vm.incident.submittedDate) {
                vm.partA.submittedDate = new Date(vm.incident.createdDate || vm.incident.submittedDate);
            }

            vm.partA.hodId = vm.incident.hodId || '';
            vm.partA.wshoId = vm.incident.wshoId || '';
            vm.partA.ahodId = vm.incident.ahodId || '';
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

            refreshDropDown('partA_incidentType', vm.partA.incidentTypeOptions.dataSource, vm.partA.incidentType);
            refreshDropDown('partA_sector', vm.partA.sectorOptions.dataSource, vm.partA.sectorCode);
            refreshDropDown('partA_lob', vm.partA.lobOptions.dataSource, vm.partA.lobCode);
            refreshDropDown('partA_department', vm.partA.departmentOptions.dataSource, vm.partA.departmentCode);
            refreshDropDown('partA_location', vm.partA.locationOptions.dataSource, vm.partA.locationCode);
            refreshDropDown('partA_hod', vm.partA.hodOptions.dataSource, vm.partA.hodId);
            refreshDropDown('partA_wsho', vm.partA.wshoOptions.dataSource, vm.partA.wshoId);
            refreshDropDown('partA_ahod', vm.partA.ahodOptions.dataSource, vm.partA.ahodId);
        }

        function onIncidentTypeChange(vm) {
            vm.showOtherIncidentType = vm.partA.incidentType === 'other' || vm.partA.incidentType === 'OTH';
        }

        function addInjuredPerson(vm) {
            if (!vm.injuredPerson.name || !vm.injuredPerson.employeeNo) {
                alert('Please fill in Name and Employee Number');
                return;
            }

            var person = angular.copy(vm.injuredPerson);
            person.type = vm.injuredPersonType;
            vm.partA.injuredPersons.push(person);

            vm.injuredPerson = {};

            if (vm.injuredGrid) {
                vm.injuredGrid.dataSource.read();
            }
        }

        function deleteInjuredPerson(vm, e) {
            var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
            var index = vm.partA.injuredPersons.indexOf(dataItem);
            if (index >= 0) {
                vm.partA.injuredPersons.splice(index, 1);
                if (vm.injuredGrid) {
                    vm.injuredGrid.dataSource.read();
                }
            }
        }

        function searchInjuredPerson(vm) {
            if (window.openEmployeeSearch) {
                window.openEmployeeSearch('injured', function (employee) {
                    $timeout(function () {
                        vm.injuredPerson.name = employee.name;
                        vm.injuredPerson.employeeNo = employee.empId;
                        vm.injuredPerson.contactNo = employee.contactNo;
                        vm.injuredPerson.age = employee.age;
                        vm.injuredPerson.race = employee.race;
                        vm.injuredPerson.nationality = employee.nationality;
                        vm.injuredPerson.gender = employee.gender;
                        vm.injuredPerson.employmentType = employee.employmentType;
                        vm.injuredPerson.dateOfEmployment = employee.dateOfEmployment;
                        vm.injuredPerson.designation = employee.designation;
                    });
                });
            }
        }

        function addEyeWitness(vm) {
            if (!vm.eyeWitness.name) {
                alert('Please fill in Name');
                return;
            }

            var witness = angular.copy(vm.eyeWitness);
            vm.partA.eyewitnesses.push(witness);

            vm.eyeWitness = {};

            if (vm.witnessGrid) {
                vm.witnessGrid.dataSource.read();
            }
        }

        function deleteEyeWitness(vm, e) {
            var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
            var index = vm.partA.eyewitnesses.indexOf(dataItem);
            if (index >= 0) {
                vm.partA.eyewitnesses.splice(index, 1);
                if (vm.witnessGrid) {
                    vm.witnessGrid.dataSource.read();
                }
            }
        }

        function searchEyeWitness(vm) {
            if (window.openEmployeeSearch) {
                window.openEmployeeSearch('witness', function (employee) {
                    $timeout(function () {
                        vm.eyeWitness.name = employee.name;
                        vm.eyeWitness.employeeNo = employee.empId;
                        vm.eyeWitness.contactNo = employee.contactNo;
                        vm.eyeWitness.designation = employee.designation;
                    });
                });
            }
        }

        function getIncidentTypeName(vm, code) {
            if (!code || !vm.partA.incidentTypeOptions.dataSource) return code || '';
            var item = vm.partA.incidentTypeOptions.dataSource.find(function (i) {
                return i.code === code || i.type === code;
            });
            return item ? (item.value || item.description) : code;
        }

        function getSectorName(vm, code) {
            if (!code || !vm.partA.sectorOptions.dataSource) return code || '';
            var item = vm.partA.sectorOptions.dataSource.find(function (i) { return i.code === code; });
            return item ? item.value : code;
        }

        function getLOBName(vm, code) {
            if (!code || !vm.partA.lobOptions.dataSource) return code || '';
            var item = vm.partA.lobOptions.dataSource.find(function (i) { return i.code === code; });
            return item ? item.value : code;
        }

        function getDepartmentName(vm, code) {
            if (!code || !vm.partA.departmentOptions.dataSource) return code || '';
            var item = vm.partA.departmentOptions.dataSource.find(function (i) { return i.code === code; });
            return item ? item.value : code;
        }

        function getLocationName(vm, code) {
            if (!code || !vm.partA.locationOptions.dataSource) return code || '';
            var item = vm.partA.locationOptions.dataSource.find(function (i) { return i.code === code; });
            return item ? item.value : code;
        }
    }
})();