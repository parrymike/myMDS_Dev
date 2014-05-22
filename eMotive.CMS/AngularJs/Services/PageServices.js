angular.module('pageServices', []).factory('$pageServices', function ($http) {
    return {//https://github.com/mgonto/restangular <== look into this?
        getAllSections: function () {
            return $http.get("/api/Pages/Sections").then(function (result) {
                return result.data;
            });
        },
        getSections: function (ids) {
            return $http.get("/api/Pages/Sections", { params: { Ids: ids } }).then(function (result) {
                return result.data;
            });
        },
        newSection: function () {
            return $http.get("/api/Pages/Sections/New").then(function (result) {
                return result.data;
            });
        },
        createSection: function (data) {
            return $http.post("/api/Pages/Sections", { Section: data }).then(function (result) {
                return result.data;
            });
        },
        updateSection: function (data) {
            return $http.put("/api/Pages/Sections", { Section: data }).then(function (result) {
                return result.data;
            });
        },
        deleteSection: function (id) {

            return $http.delete("/api/Pages/Sections", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },

        getPages: function (ids) {
            return $http.get("/api/Pages", { params: { Ids: ids } }).then(function (result) {
                return result.data;
            });
        },
        newPage: function () {
            return $http.get("/api/Pages/New").then(function (result) {
                return result.data;
            });
        },

        createPage: function (data) {
            return $http.post("/api/Pages", { Page: data }).then(function (result) {
                return result.data;
            });
        },
        updatePage: function (data) {
            return $http.put("/api/Pages", { Page: data }).then(function (result) {
                return result.data;
            });
        },
        deletePage: function (id) {

            return $http.delete("/api/Pages", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        getAuditTrail: function (id) {
            return $http.get("/api/Pages/Audit", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        doRollBack: function (record) {
            return $http.post("/api/Pages/Audit/Rollback", { Record: record }).then(function (result) {
                return result.data;
            });
        },
        doSearch: function (search) {
            return $http.post("/api/Pages/Search", { PageSearch: search }).then(function (result) {
                return result.data;
            });
        }
    };
});