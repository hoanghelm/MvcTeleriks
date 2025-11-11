(function () {
    angular
        .module('incidentUpdateApp')
        .factory('IncidentUpdateService', IncidentUpdateService);

    IncidentUpdateService.$inject = ['$http', '$q'];

    function IncidentUpdateService($http, $q) {
        var service = {
            getIncidentById: getIncidentById,
            getCurrentUser: getCurrentUser,
            getStatusName: getStatusName,
            getIncidentTypes: getIncidentTypes,
            getSectors: getSectors,
            getLOBs: getLOBs,
            getDepartments: getDepartments,
            getLocations: getLocations,
            getHODs: getHODs,
            getAHODs: getAHODs,
            getInjuredCaseTypes: getInjuredCaseTypes,
            getWSHOs: getWSHOs,
            getAlternateWSHOs: getAlternateWSHOs,
            getPartBCopyToList: getPartBCopyToList,
            submitPartB: submitPartB,
            closePartB: closePartB,
            searchEmployee: searchEmployee,
            getCWSHOs: getCWSHOs,
            getNatureOfInjury: getNatureOfInjury,
            getHeadNeckTorso: getHeadNeckTorso,
            getUpperLimbs: getUpperLimbs,
            getLowerLimbs: getLowerLimbs,
            getIncidentClass: getIncidentClass,
            getIncidentAgent: getIncidentAgent,
            getUnsafeConditions: getUnsafeConditions,
            getUnsafeActs: getUnsafeActs,
            getContributingFactors: getContributingFactors,
            getNegligentOptions: getNegligentOptions,
            savePartC: savePartC,
            submitPartC: submitPartC,
            closePartC: closePartC,
            getHSBUs: getHSBUs,
            getPartDCopyToList: getPartDCopyToList,
            submitPartD: submitPartD,
            revertPartDToWSHO: revertPartDToWSHO,
            getHeadLOBs: getHeadLOBs,
            getPartECopyToList: getPartECopyToList,
            submitPartE: submitPartE,
            revertPartEToWSHO: revertPartEToWSHO,
            submitPartF: submitPartF,
            submitPartG: submitPartG,
            revertPartGToHOD: revertPartGToHOD
        };

        return service;

        function getIncidentById(incidentId) {
            return $http.get('/Incident/GetIncidentById?id=' + incidentId)
                .then(function (response) {
                    if (response.data.success) {
                        return response.data.incident;
                    }
                    return $q.reject(response.data.message || 'Failed to load incident');
                })
                .catch(handleError);
        }

        function getCurrentUser() {
            return $http.get('/User/GetCurrentUser')
                .then(function (response) {
                    if (response.data.success) {
                        return response.data.user;
                    }
                    return $q.reject(response.data.message || 'Failed to load user');
                })
                .catch(handleError);
        }

        function getStatusName(statusCode) {
            return $http.get('/MasterData/GetLookupByType?type=Actions')
                .then(function(response) {
                    if (response.data && Array.isArray(response.data)) {
                        var statusItem = response.data.find(function(item) {
                            return item.code === statusCode;
                        });
                        return statusItem ? statusItem.value : statusCode;
                    }
                    return statusCode;
                })
                .catch(function(error) {
                    console.error('Error loading status name:', error);
                    return statusCode;
                });
        }

        function getIncidentTypes() {
            return $http.get('/MasterData/GetLookupByType?type=Incident Type')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getSectors() {
            return $http.get('/MasterData/GetSectors')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getLOBs(sectorCode) {
            return $http.get('/MasterData/GetLOBs?sectorCode=' + sectorCode)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getDepartments(sectorCode, lobCode) {
            var url = '/MasterData/GetDepartments?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getLocations(sectorCode, lobCode, departmentCode) {
            var url = '/MasterData/GetLocations?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getHODs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetHODs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getAHODs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetAHODs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getInjuredCaseTypes() {
            return $http.get('/MasterData/GetLookupByType?type=InjuredCaseType')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getWSHOs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetWSHOs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getAlternateWSHOs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetAWSHOs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getPartBCopyToList(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetPartACopyTo?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function submitPartB(partBData) {
            return $http.post('/Incident/SubmitPartB', partBData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function closePartB(partBData) {
            return $http.post('/Incident/ClosePartB', partBData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function searchEmployee(searchTerm, searchType) {
            return $http.get('/api/Employee/Search?name=' + searchTerm + '&searchType=' + searchType)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getCWSHOs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetCWSHOs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getNatureOfInjury() {
            return $http.get('/MasterData/GetLookupByType?type=Nature Of Injury')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getHeadNeckTorso() {
            return $http.get('/MasterData/GetLookupByType?type=Head Neck Torso')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getUpperLimbs() {
            return $http.get('/MasterData/GetLookupByType?type=Upper Limbs')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getLowerLimbs() {
            return $http.get('/MasterData/GetLookupByType?type=Lower Limbs')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getIncidentClass() {
            return $http.get('/MasterData/GetLookupByType?type=Incident Class')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getIncidentAgent() {
            return $http.get('/MasterData/GetLookupByType?type=Incident Agent')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getUnsafeConditions() {
            return $http.get('/MasterData/GetLookupByType?type=Unsafe Condition')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getUnsafeActs() {
            return $http.get('/MasterData/GetLookupByType?type=Unsafe Act')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getContributingFactors() {
            return $http.get('/MasterData/GetLookupByType?type=Factors')
                .then(handleSuccess)
                .catch(handleError);
        }

        function getNegligentOptions() {
            return $http.get('/MasterData/GetLookupByType?type=Negligent')
                .then(handleSuccess)
                .catch(handleError);
        }

        function savePartC(partCData) {
            return $http.post('/Incident/SavePartC', partCData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function submitPartC(partCData) {
            return $http.post('/Incident/SubmitPartC', partCData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function closePartC(closeData) {
            return $http.post('/Incident/ClosePartC', closeData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getHSBUs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetHSBUs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getPartDCopyToList(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetPartACopyTo?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function submitPartD(partDData) {
            return $http.post('/Incident/SubmitPartD', partDData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function revertPartDToWSHO(revertData) {
            return $http.post('/Incident/RevertPartDToWSHO', revertData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getHeadLOBs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetHeadLOBs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getPartECopyToList(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetPartACopyTo?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function submitPartE(partEData) {
            return $http.post('/Incident/SubmitPartE', partEData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function revertPartEToWSHO(revertData) {
            return $http.post('/Incident/RevertPartEToWSHO', revertData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function submitPartF(formData) {
            return $http.post('/Incident/SubmitPartF', formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
                .then(handleSuccess)
                .catch(handleError);
        }

        function submitPartG(formData) {
            return $http.post('/Incident/SubmitPartG', formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
                .then(handleSuccess)
                .catch(handleError);
        }

        function revertPartGToHOD(formData) {
            return $http.post('/Incident/RevertPartGToHOD', formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
                .then(handleSuccess)
                .catch(handleError);
        }

        function handleSuccess(response) {
            return response.data;
        }

        function handleError(error) {
            return $q.reject(error.data || error.statusText || 'An error occurred');
        }
    }
})();