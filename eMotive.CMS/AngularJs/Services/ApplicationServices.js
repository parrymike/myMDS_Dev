angular.module('applicationServices', []).factory('$applicationServices', function ($http) {
    return {//https://github.com/mgonto/restangular <== look into this?
        getAllApplications: function () {
            return $http.get("/api/Applications/").then(function (result) {
                return result.data;
            });
        },
        getApplications: function (ids) {
            return $http.get("/api/Applications/", { params: { Ids: ids } }).then(function (result) {
                return result.data;
            });
        },
        newApplication: function () {
            return $http.get("/api/Applications/New").then(function (result) {
                return result.data;
            });
        },
        createApplication: function (data) {
            return $http.post("/api/Applications/", { Application: data }).then(function (result) {
                return result.data;
            });
        },
        updateApplication: function (data) {
            return $http.put("/api/Applications/", { Application: data }).then(function (result) {
                return result.data;
            });
        },
        deleteApplication: function (id) {

            return $http.delete("/api/Applications/", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        getAuditTrail: function (id) {
            return $http.get("/api/Applications/Audit", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        doRollBack: function (record) {
            return $http.post("/api/Applications/Audit/Rollback", { Record: record }).then(function (result) {
                return result.data;
            });
        },
        doSearch: function (search) {
            return $http.post("/api/Applications/Search", { ApplicationSearch: search }).then(function (result) {
                return result.data;
            });
        }
    };
});