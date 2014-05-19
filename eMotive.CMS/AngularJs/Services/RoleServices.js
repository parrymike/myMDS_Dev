angular.module('roleServices', []).factory('$roleServices', function ($http) {
    return {//https://github.com/mgonto/restangular <== look into this?
        getAllRoles: function () {
            return $http.get("/api/Roles/").then(function (result) {
                return result.data;
            });
        },
        getRoles: function (ids) {
            return $http.get("/api/Roles/", { params: { Ids: ids }}).then(function (result) {
                return result.data;
            });
        },
        newRole: function () {
            return $http.get("/api/Roles/New").then(function (result) {
                return result.data;
            });
        },
        createRole: function (data) {
            return $http.post("/api/Roles/", { Role: data }).then(function (result) {
                return result.data;
            });
        },
        updateRole: function (data) {
            return $http.put("/api/Roles/", { Role: data }).then(function (result) {
                return result.data;
            });
        },
        deleteRole: function (data) {
            return $http.delete("/api/Roles/", {params: { Role: data }}).then(function (result) {
                return result.data;
            });
        },
        getAuditTrail: function (id) {
            return $http.get("/api/Roles/Audit", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        doRollBack: function (record) {
            return $http.post("/api/Roles/Audit/Rollback", { Record: record }).then(function (result) {
                return result.data;
            });
        },
        doSearch: function (search) {
            return $http.post("/api/Roles/Search", { RoleSearch: search }).then(function (result) {
                return result.data;
            });
        }
    };
});