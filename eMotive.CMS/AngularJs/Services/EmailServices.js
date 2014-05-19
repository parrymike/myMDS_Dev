angular.module('emailServices', []).factory('$emailServices', function ($http) {
    return {//https://github.com/mgonto/restangular <== look into this?
        getAllEmails: function () {
            return $http.get("/api/Emails/").then(function (result) {
                return result.data;
            });
        },
        getEmails: function (ids) {
            return $http.get("/api/Emails/", { params: { Ids: ids } }).then(function (result) {
                return result.data;
            });
        },
        newEmail: function () {
            return $http.get("/api/Emails/New").then(function (result) {
                return result.data;
            });
        },
        createEmail: function (data) {
            return $http.post("/api/Emails/", { Email: data }).then(function (result) {
                return result.data;
            });
        },
        updateEmail: function (data) {
            return $http.put("/api/Emails/", { Email: data }).then(function (result) {
                return result.data;
            });
        },
        deleteEmail: function (id) {

            return $http.delete("/api/Emails/", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        getAuditTrail: function (id) {
            return $http.get("/api/Emails/Audit", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        doRollBack: function (record) {
            return $http.post("/api/Emails/Audit/Rollback", { Record: record }).then(function (result) {
                return result.data;
            });
        },
        doSearch: function (search) {
            return $http.post("/api/Emails/Search", { EmailSearch: search }).then(function (result) {
                return result.data;
            });
        }
    };
});