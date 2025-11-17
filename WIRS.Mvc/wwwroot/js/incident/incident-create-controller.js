(function () {
    angular
        .module('incidentApp')
        .controller('IncidentCreateController', IncidentCreateController);

    IncidentCreateController.$inject = ['$scope', '$window', 'IncidentService'];

    function IncidentCreateController($scope, $window, IncidentService) {
        var vm = this;

        vm.incident = {
            incidentType: null,
            incidentOther: '',
            incidentDate: new Date(),
            incidentTime: new Date(),
            sectorCode: '',
            lobCode: '',
            departmentCode: '',
            locationCode: '',
            exactLocation: '',
            incidentDescription: '',
            damageDescription: '',
            isJobRelated: '',
            hospitalClinicName: '',
            workingOvertime: '',
            officialWorkingHours: '',
            hasEyeWitness: false,
            hodId: '',
            wshoId: '',
            ahodId: ''
        };

        vm.injuredPerson = {};
        vm.eyeWitness = {};
        vm.injuredPersons = [];
        vm.eyeWitnesses = [];
        vm.hrCopyToList = [];
        vm.currentUser = {};
        vm.currentDate = new Date();
        vm.maxDate = new Date();

        vm.showOtherIncidentType = false;
        vm.injuredPersonType = 'E';
        vm.validationMessage = '';
        vm.successMessage = '';
        vm.isSubmitting = false;

        vm.incidentTypeOptions = {
            dataTextField: 'value',
            dataValueField: 'code',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
        };

        vm.sectorOptions = {
            dataTextField: 'value',
            dataValueField: 'code',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
        };

        vm.lobOptions = {
            dataTextField: 'value',
            dataValueField: 'code',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
        };

        vm.departmentOptions = {
            dataTextField: 'value',
            dataValueField: 'code',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
        };

        vm.locationOptions = {
            dataTextField: 'value',
            dataValueField: 'code',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
        };

        vm.hodOptions = {
            dataTextField: 'name',
            dataValueField: 'id',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
        };

        vm.wshoOptions = {
            dataTextField: 'name',
            dataValueField: 'id',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
        };

        vm.ahodOptions = {
            dataTextField: 'name',
            dataValueField: 'id',
            optionLabel: '--Select--',
            dataSource: new kendo.data.DataSource({ data: [] }),
            valuePrimitive: true
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
                    title: 'Action',
                    width: 100,
                    template: function(dataItem) {
                        return '<button type="button" class="text-red-600 hover:text-red-800" data-uid="' + dataItem.uid + '">Remove</button>';
                    }
                }
            ],
            selectable: false,
            scrollable: true,
            sortable: true,
            pageable: false,
            dataBound: onInjuredGridDataBound
        };

        vm.witnessGridOptions = {
            columns: [
                { field: 'name', title: 'Name', width: 250 },
                { field: 'employeeNo', title: 'Employee No', width: 150 },
                { field: 'designation', title: 'Designation', width: 200 },
                { field: 'contactNo', title: 'Contact Number', width: 150 },
                {
                    title: 'Action',
                    width: 100,
                    template: function(dataItem) {
                        return '<button type="button" class="text-red-600 hover:text-red-800" data-uid="' + dataItem.uid + '">Remove</button>';
                    }
                }
            ],
            selectable: false,
            scrollable: true,
            sortable: true,
            pageable: false,
            dataBound: onWitnessGridDataBound
        };

        vm.onIncidentTypeChange = onIncidentTypeChange;
        vm.onSectorChange = onSectorChange;
        vm.onLobChange = onLobChange;
        vm.onDepartmentChange = onDepartmentChange;
        vm.addInjuredPerson = addInjuredPerson;
        vm.addEyeWitness = addEyeWitness;
        vm.removeInjuredPerson = removeInjuredPerson;
        vm.removeEyeWitness = removeEyeWitness;
        vm.searchInjuredPerson = searchInjuredPerson;
        vm.searchEyeWitness = searchEyeWitness;
        vm.submitIncident = submitIncident;
        vm.cancel = cancel;

        init();

        function onInjuredGridDataBound(e) {
            var grid = e.sender;
            grid.tbody.find('button[data-uid]').on('click', function() {
                var uid = $(this).data('uid');
                vm.removeInjuredPerson(uid);
            });
        }

        function onWitnessGridDataBound(e) {
            var grid = e.sender;
            grid.tbody.find('button[data-uid]').on('click', function() {
                var uid = $(this).data('uid');
                vm.removeEyeWitness(uid);
            });
        }

        function init() {
            loadIncidentTypes();
            loadSectors().then(function () {
                loadCurrentUser();
            })
        }

        function loadCurrentUser() {
            IncidentService.getCurrentUser()
                .then(function (data) {
                    vm.currentUser = {
                        userId: data.userId,
                        name: data.userName || data.displayName || 'User',
                        designation: data.designation || '',
                        sectorCode: data.sbaName || ''
                    };

                    if (vm.currentUser.sectorCode) {
                        vm.incident.sectorCode = vm.currentUser.sectorCode;
                        loadLOBs(vm.currentUser.sectorCode);
                    }
                })
                .catch(function (error) {
                    console.error('Failed to load current user:', error);
                    vm.validationMessage = 'Failed to load user information';
                });
        }

        function loadIncidentTypes() {
            IncidentService.getIncidentTypes()
                .then(function (data) {
                    vm.incidentTypeOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load incident types:', error);
                });
        }

        function loadSectors() {
            return IncidentService.getSectors()
                .then(function (data) {
                    vm.sectorOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load sectors:', error);
                });
        }

        function loadLOBs(sectorCode) {
            if (!sectorCode) return;

            IncidentService.getLOBs(sectorCode)
                .then(function (data) {
                    vm.lobOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load LOBs:', error);
                });
        }

        function loadDepartments(sectorCode, lobCode) {
            if (!sectorCode || !lobCode) return;

            IncidentService.getDepartments(sectorCode, lobCode)
                .then(function (data) {
                    vm.departmentOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load departments:', error);
                });
        }

        function loadLocations(sectorCode, lobCode, departmentCode) {
            if (!sectorCode || !lobCode) return;

            IncidentService.getLocations(sectorCode, lobCode, departmentCode)
                .then(function (data) {
                    vm.locationOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load locations:', error);
                });
        }

        function loadWorkflowUsers(sectorCode, lobCode, departmentCode, locationCode) {
            if (!sectorCode || !lobCode) return;

            IncidentService.getHODs(sectorCode, lobCode, departmentCode, locationCode)
                .then(function (data) {
                    vm.hodOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load HODs:', error);
                });

            IncidentService.getWSHOs(sectorCode, lobCode, departmentCode, locationCode)
                .then(function (data) {
                    vm.wshoOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load WSHOs:', error);
                });

            IncidentService.getAHODs(sectorCode, lobCode, departmentCode, locationCode)
                .then(function (data) {
                    vm.ahodOptions.dataSource.data(data);
                })
                .catch(function (error) {
                    console.error('Failed to load AHODs:', error);
                });

            IncidentService.getHRCopyToList(sectorCode, lobCode, departmentCode, locationCode)
                .then(function (data) {
                    vm.hrCopyToList = data.map(function (hr) {
                        return {
                            id: hr.id,
                            name: hr.name,
                            selected: true
                        };
                    });
                })
                .catch(function (error) {
                    console.error('Failed to load HR Copy To list:', error);
                });
        }

        function onIncidentTypeChange() {
            vm.showOtherIncidentType = vm.incident.incidentType === 'OTH' || vm.incident.incidentType === 'Others';
        }

        function onSectorChange() {
            vm.incident.lobCode = '';
            vm.incident.departmentCode = '';
            vm.incident.locationCode = '';
            vm.lobOptions.dataSource.data([]);
            vm.departmentOptions.dataSource.data([]);
            vm.locationOptions.dataSource.data([]);

            if (vm.incident.sectorCode) {
                loadLOBs(vm.incident.sectorCode);
            }
        }

        function onLobChange() {
            vm.incident.departmentCode = '';
            vm.incident.locationCode = '';
            vm.departmentOptions.dataSource.data([]);
            vm.locationOptions.dataSource.data([]);

            if (vm.incident.sectorCode && vm.incident.lobCode) {
                loadDepartments(vm.incident.sectorCode, vm.incident.lobCode);
                loadWorkflowUsers(vm.incident.sectorCode, vm.incident.lobCode, '', '');
            }
        }

        function onDepartmentChange() {
            vm.incident.locationCode = '';
            vm.locationOptions.dataSource.data([]);

            if (vm.incident.sectorCode && vm.incident.lobCode) {
                loadLocations(vm.incident.sectorCode, vm.incident.lobCode, vm.incident.departmentCode);
                loadWorkflowUsers(vm.incident.sectorCode, vm.incident.lobCode, vm.incident.departmentCode, '');
            }
        }

        function addInjuredPerson() {
            if (!vm.injuredPerson.name || !vm.injuredPerson.employeeNo) {
                vm.validationMessage = 'Please enter Name and Employee Number for injured person';
                return;
            }

            var person = angular.copy(vm.injuredPerson);
            person.type = vm.injuredPersonType;
            vm.injuredPersons.push(person);

            vm.injuredPerson = {};
            vm.validationMessage = '';

            if (vm.injuredGrid) {
                vm.injuredGrid.dataSource.read();
            }
        }

        function deleteInjuredPerson(e) {
            e.preventDefault();
            var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
            var index = vm.injuredPersons.indexOf(dataItem);
            if (index > -1) {
                vm.injuredPersons.splice(index, 1);
                $scope.$apply();
            }
        }

        function removeInjuredPerson(uid) {
            $scope.$apply(function() {
                var grid = vm.injuredGrid;
                if (!grid) return;

                var dataItem = grid.dataSource.getByUid(uid);
                var index = vm.injuredPersons.indexOf(dataItem);
                if (index > -1) {
                    vm.injuredPersons.splice(index, 1);
                    grid.dataSource.read();
                }
            });
        }

        function addEyeWitness() {
            if (!vm.eyeWitness.name || !vm.eyeWitness.employeeNo) {
                vm.validationMessage = 'Please enter Name and Employee Number for eye witness';
                return;
            }

            vm.eyeWitnesses.push(angular.copy(vm.eyeWitness));
            vm.eyeWitness = {};
            vm.validationMessage = '';

            if (vm.witnessGrid) {
                vm.witnessGrid.dataSource.read();
            }
        }

        function deleteEyeWitness(e) {
            e.preventDefault();
            var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
            var index = vm.eyeWitnesses.indexOf(dataItem);
            if (index > -1) {
                vm.eyeWitnesses.splice(index, 1);
                $scope.$apply();
            }
        }

        function removeEyeWitness(uid) {
            $scope.$apply(function() {
                var grid = vm.witnessGrid;
                if (!grid) return;

                var dataItem = grid.dataSource.getByUid(uid);
                var index = vm.eyeWitnesses.indexOf(dataItem);
                if (index > -1) {
                    vm.eyeWitnesses.splice(index, 1);
                    grid.dataSource.read();
                }
            });
        }

        function searchInjuredPerson() {
            if (typeof window.openEmployeeSearch === 'function') {
                window.openEmployeeSearch('InjuredSearch', function (employee) {
                    $scope.$apply(function () {
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

        function searchEyeWitness() {
            if (typeof window.openEmployeeSearch === 'function') {
                window.openEmployeeSearch('EyeWitnessSearch', function (employee) {
                    $scope.$apply(function () {
                        vm.eyeWitness.name = employee.name;
                        vm.eyeWitness.employeeNo = employee.empId;
                        vm.eyeWitness.designation = employee.designation;
                        vm.eyeWitness.contactNo = employee.contactNo;
                    });
                });
            }
        }

        function submitIncident() {
            if (!validateForm()) {
                return;
            }

            vm.isSubmitting = true;
            vm.validationMessage = '';

            var incidentData = prepareIncidentData();

            IncidentService.createIncident(incidentData)
                .then(function (response) {
                    vm.isSubmitting = false;
                    if (response.success) {
                        vm.successMessage = 'Incident report submitted successfully';
                        setTimeout(function () {
                            $window.location.href = '/Home/Index';
                        }, 2000);
                    } else {
                        vm.validationMessage = response.message || 'Failed to submit incident report';
                    }
                })
                .catch(function (error) {
                    vm.isSubmitting = false;
                    vm.validationMessage = 'An error occurred while submitting the incident report';
                    console.error('Submit error:', error);
                });
        }

        function validateForm() {
            vm.validationMessage = '';

            if (!vm.incident.incidentType) {
                vm.validationMessage = 'Please select Incident Type';
                return false;
            }

            if (vm.showOtherIncidentType && !vm.incident.incidentOther) {
                vm.validationMessage = 'Please specify Other Incident Type';
                return false;
            }

            if (!vm.incident.incidentDate) {
                vm.validationMessage = 'Please select Incident Date';
                return false;
            }

            if (!vm.incident.incidentTime) {
                vm.validationMessage = 'Please select Incident Time';
                return false;
            }

            if (!vm.incident.lobCode) {
                vm.validationMessage = 'Please select LOB';
                return false;
            }

            if (!vm.incident.incidentDescription) {
                vm.validationMessage = 'Please enter Incident Description';
                return false;
            }

            if (vm.incident.incidentDescription.length > 2000) {
                vm.validationMessage = 'Incident Description cannot exceed 2000 characters';
                return false;
            }

            if (vm.incident.incidentType === '1' && vm.injuredPersons.length === 0) {
                vm.validationMessage = 'Please add at least one injured person';
                return false;
            }

            if (vm.incident.incidentType !== '1' && !vm.incident.damageDescription) {
                vm.validationMessage = 'Please enter Damage Description';
                return false;
            }

            if (vm.incident.hasEyeWitness && vm.eyeWitnesses.length === 0) {
                vm.validationMessage = 'Please add at least one eye witness';
                return false;
            }

            if (!vm.incident.hodId) {
                vm.validationMessage = 'Please select HOD';
                return false;
            }

            return true;
        }

        function prepareIncidentData() {
            var selectedCopyTo = vm.hrCopyToList
                .filter(function (hr) { return hr.selected; })
                .map(function (hr) { return hr.id; });

            var incidentDate = kendo.toString(vm.incident.incidentDate, 'dd-MMM-yyyy');
            var incidentTime = kendo.toString(vm.incident.incidentTime, 'HH:mm');

            return {
                incidentType: vm.incident.incidentType,
                incidentOther: vm.incident.incidentOther,
                incidentDate: incidentDate,
                incidentTime: incidentTime,
                incidentDateTime: incidentDate + ' ' + incidentTime,
                sectorCode: vm.incident.sectorCode,
                lobCode: vm.incident.lobCode,
                departmentCode: vm.incident.departmentCode || '',
                locationCode: vm.incident.locationCode || '',
                exactLocation: vm.incident.exactLocation,
                incidentDescription: vm.incident.incidentDescription,
                damageDescription: vm.incident.damageDescription,
                isJobRelated: vm.incident.isJobRelated,
                hospitalClinicName: vm.incident.hospitalClinicName,
                workingOvertime: vm.incident.workingOvertime,
                officialWorkingHours: vm.incident.officialWorkingHours,
                hasEyeWitness: vm.incident.hasEyeWitness,
                hodId: vm.incident.hodId,
                wshoId: vm.incident.wshoId || '',
                ahodId: vm.incident.ahodId || '',
                injuredPersons: vm.injuredPersons,
                eyeWitnesses: vm.eyeWitnesses,
                copyToList: selectedCopyTo,
                superiorEmpNo: vm.currentUser.userId,
                superiorName: vm.currentUser.name,
                superiorDesignation: vm.currentUser.designation
            };
        }

        function cancel() {
            $window.location.href = '/Home/Index';
        }
    }
})();