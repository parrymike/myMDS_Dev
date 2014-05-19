angular.module('eventServices', []).factory('$eventServices', function ($http) {
    return {//https://github.com/mgonto/restangular <== look into this?
        getAllEvents: function () {
            return $http.get("/api/Events/").then(function (result) {
                return result.data;
            });
        },
        getApplicationEvents: function (applicationID) {
            return $http.get("/api/Events/Application", { params: { ApplicationID: applicationID } }).then(function (result) {
                return result.data;
            });
        },
        getEvents: function (Ids) {
            return $http.get("/api/Events/", { params: { Ids: Ids } }).then(function (result) {
                return result.data;
            });
        },
        getEventTypeIds: function (type, eventId) {
            return $http.get("/api/Events/ForType", { params: { Type: type, EventId: eventId } }).then(function (result) {
                return result.data;
            });
        },
        newEvent: function () {
            return $http.get("/api/Events/New").then(function (result) {
                return result.data;
            });
        },
        createEvent: function (data) {
            return $http.post("/api/Events/", { Event: data }).then(function (result) {
                return result.data;
            });
        },
        updateEvent: function (data) {
            return $http.put("/api/Events/", { Event: data }).then(function (result) {
                return result.data;
            });
        },
        deleteEvent: function (id) {

            return $http.delete("/api/Events/", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        saveApplicationEvents: function (events, applicationId) {

            return $http.put("/api/Events/Application", { Events: events, ApplicationId: applicationId }).then(function (result) {
                return result.data;
            });
        },
        getAuditTrail: function (id) {
            return $http.get("/api/Events/Audit", { params: { Id: id } }).then(function (result) {
                return result.data;
            });
        },
        assignToEvent: function (data) {
            return $http.post("/api/Events/AssignToEvent", { eventObject: data }).then(function (result) {
                return result.data;
            });
        },
        doRollBack: function (record) {
            return $http.post("/api/Events/Audit/Rollback", { Record: record }).then(function (result) {
                return result.data;
            });
        },
        doSearch: function (search) {
            return $http.post("/api/Events/Search", { EventSearch: search }).then(function (result) {
                return result.data;
            });
        }
    };
});