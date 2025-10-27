(function () {
    angular
        .module('incidentApp')
        .factory('IncidentService', IncidentService);

    IncidentService.$inject = ['$http', '$q'];

    function IncidentService($http, $q) {
        var service = {
            getIncidentTypes: getIncidentTypes,
            getSectors: getSectors,
            getLOBs: getLOBs,
            getDepartments: getDepartments,
            getLocations: getLocations,
            getHODs: getHODs,
            getWSHOs: getWSHOs,
            getAHODs: getAHODs,
            getHRCopyToList: getHRCopyToList,
            createIncident: createIncident,
            searchEmployee: searchEmployee,
            getCurrentUser: getCurrentUser
        };

        return service;

        function getIncidentTypes() {
            return $http.get('/MasterData/GetLookupByType?type=IncidentType')
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
            return $http.get('/MasterData/GetDepartments?sectorCode=' + sectorCode + '&lobCode=' + lobCode)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getLocations(sectorCode, lobCode, departmentCode) {
            var url = '/MasterData/GetLocations?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) {
                url += '&deptCode=' + departmentCode;
            }
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

        function getWSHOs(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetWSHOs?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
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

        function getHRCopyToList(sectorCode, lobCode, departmentCode, locationCode) {
            var url = '/User/GetPartACopyTo?sectorCode=' + sectorCode + '&lobCode=' + lobCode;
            if (departmentCode) url += '&departmentCode=' + departmentCode;
            if (locationCode) url += '&locationCode=' + locationCode;
            return $http.get(url)
                .then(handleSuccess)
                .catch(handleError);
        }

        function createIncident(incidentData) {
            return $http.post('/Incident/CreateApi', incidentData)
                .then(handleSuccess)
                .catch(handleError);
        }

        function searchEmployee(searchTerm, searchType) {
            return $http.get('/api/Employee/Search?name=' + searchTerm + '&searchType=' + searchType)
                .then(handleSuccess)
                .catch(handleError);
        }

        function getCurrentUser() {
            return $http.get('/User/GetCurrentUser')
                .then(function(response) {
                    if (response.data.success) {
                        return response.data.user;
                    }
                    return handleError(response);
                })
                .catch(handleError);
        }

        function handleSuccess(response) {
            return response.data;
        }

        function handleError(error) {
            console.error('API Error:', error);
            return $q.reject(error.data || 'An error occurred');
        }
    }
})();
