angular.module('courseServices', []).factory('$courseServices', function ($http) {
    return {//https://github.com/mgonto/restangular <== look into this?
        getAllCourses: function () {
            return $http.get("/api/Courses/").then(function (result) {
                return result.data;
            });
        },
        getCourses: function (ids) {
            return $http.get("/api/Courses/", { params: { Ids: ids } }).then(function (result) {
                return result.data;
            });
        },
        newCourse: function () {
            return $http.get("/api/Courses/New").then(function (result) {
                return result.data;
            });
        },
        createCourse: function (data) {
            return $http.post("/api/Courses/", { Course: data }).then(function (result) {
                return result.data;
            });
        },
        updateCourse: function (data) {
            return $http.put("/api/Courses/", { Course: data }).then(function (result) {
                return result.data;
            });
        },
        deleteCourse: function (id) {

            return $http.delete("/api/Courses/", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        getAuditTrail: function (id) {
            return $http.get("/api/Courses/Audit", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        doRollBack: function (record) {
            return $http.post("/api/Courses/Audit/Rollback", { Record: record }).then(function (result) {
                return result.data;
            });
        },
        doSearch: function (search) {
            return $http.post("/api/Courses/Search", { CourseSearch: search }).then(function (result) {
                return result.data;
            });
        }
    };
});