var emailAdminApp = angular.module('emailAdminApp', ['ngRoute', 'emailServices', 'applicationServices', 'eventServices', 'ngSanitize', 'ngPrettyJson', 'mgcrea.ngStrap', 'ui.tinymce']);

emailAdminApp.config(function ($routeProvider, $datepickerProvider) {

    var dir = "/AngularJs/Apps/EmailManagement/Templates/";

    $routeProvider.when('/Home', {
        controller: 'HomeController',
        templateUrl: dir + 'Home.html'
    }).
        when('/Create', {
            controller: 'CreateController',
            templateUrl: dir +'Create.html'
        }).
        when('/Edit', {
            controller: 'EditController',
            templateUrl: dir + 'Edit.html'
        }).
        when('/Audit', {
            controller: 'AuditController',
            templateUrl: dir + 'Audit.html'
        }).
        otherwise({
            redirectTo: '/Home'
        });


    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd/MM/yyyy',
        startWeek: 1
    });
});

emailAdminApp.controller("HomeController", function ($scope, $applicationServices, $eventServices, $emailServices, $location) {

    $scope.selectedApplication = $location.search()["applicationId"];//TODO: can these just be vars rather than scope?????###########################
    $scope.selectedEvent = $location.search()["eventId"];

    $applicationServices.getAllApplications().then(function(data) {
        $scope.applications = data.Result;
    });

    if ($scope.selectedApplication != null) {



        $eventServices.getApplicationEvents($scope.selectedApplication).then(function (data) {
            $scope.applicationEvents = data.Result;

            $eventServices.getEventTypeIds("Email", $scope.selectedEvent).then(function (data) {
                if (data.Result != "") {
                    $emailServices.getEmails(data.Result).then(function (emailData) {
                        $scope.emails = emailData.Result;
                    });
                }
            });
        });
    }

    $scope.fetchApplicationEvents = function () {
        if ($scope.selectedApplication != null) {
            $eventServices.getApplicationEvents($scope.selectedApplication).then(function(data) {
                $scope.applicationEvents = data.Result;

            });
        } else {
            $scope.selectedEvent = null;
        }
    };
    
    $scope.fetchEventEmails = function () {
        
        if ($scope.selectedEvent != null) {
            $scope.emails = null;
            $eventServices.getEventTypeIds("Email", $scope.selectedEvent.ID).then(function (data) {
                if (data.Result != "") {
                    $emailServices.getEmails(data.Result).then(function(emailData) {
                        $scope.emails = emailData.Result;
                    });
                }
            });
        }
    };


});

emailAdminApp.controller("CreateController", function ($scope, $emailServices, $eventServices, $applicationServices, $location) {
    $scope.applicationId = $location.search()["applicationId"];//TODO: can these just be vars rather than scope?????###########################
    $scope.eventId = $location.search()["eventId"];

    $scope.mailPriority = [{ ID: 0, Name: "Normal" }, { ID: 1, Name: "Low" }, { ID: 2, Name: "High" }];

    $emailServices.newEmail().then(function(data) {
        $scope.email = data.Result;
    });

    $applicationServices.getApplications($scope.applicationId).then(function (data) {
        $scope.application = data.Result[0];
    }); 

    $eventServices.getEvents($scope.eventId).then(function (data) {
        $scope.event = data.Result[0];
    });

    $scope.saveEmail = function() {
        $emailServices.createEmail($scope.email).then(function(data) {
            if (data.Success) {
                $scope.email = data.Result;

                $eventServices.assignToEvent({ Type: "Email", EventId: $scope.event.ID, ObjectId: $scope.email.ID }).then(function(eventData) {
                    if (eventData.Success) {
                        //todo: email has still been created, how do we handle a success email creation but failed event assignation?
                        $location.path('/Home').replace();
                    } else {
                        alert("error");
                    }
                });

             //   $route.reload();
                
            } else {
                alert("error!");
            }
        });
    };
});

emailAdminApp.controller("EditController", function ($scope, $emailServices, $eventServices, $applicationServices, $location) {
    var applicationId = $location.search()["applicationId"];//TODO: can these just be vars rather than scope?????###########################
    var eventId = $location.search()["eventId"];
    var emailId = $location.search()["Id"];

    $scope.mailPriority = [{ ID: 0, Name: "Normal" }, { ID: 1, Name: "Low" }, { ID: 2, Name: "High" }];

    $applicationServices.getApplications(applicationId).then(function (data) {
        $scope.application = data.Result[0];
    });

    $eventServices.getEvents(eventId).then(function (data) {
        $scope.event = data.Result[0];
    });

    $emailServices.getEmails(emailId).then(function(data) {
        if (data.Success) {

            $scope.email = data.Result[0];

        } else {
            {
                alert("error");
            }
        }
    });




    $scope.saveEmail = function () {
        $emailServices.updateEmail($scope.email).then(function (data) {
            if (data.Success) {
                $scope.email = data.Result;

                //   $route.reload();
                $location.path('/Home').replace();
            } else {
                alert("error!");
            }
        });
    };
});

emailAdminApp.controller("AuditController", function ($scope, $emailServices, $location, $route) {

    $scope.showHide = {};

    $emailServices.getAuditTrail($location.search()["id"]).then(function (data) {
        $scope.auditList = data.Result;
    });

    $scope.rollBack = function (id) {
        $emailServices.doRollBack($scope.auditList[id]).then(function (rollBackData) {

            if (rollBackData.Success) {
               // $route.reload();
                $location.path('/Audit').replace();
            } else {
                alert("error!");
            }
        });
    }
});
