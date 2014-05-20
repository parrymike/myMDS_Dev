var applicationAdminApp = angular.module('applicationAdminApp', ['ngRoute', 'courseServices', 'applicationServices', 'eventServices', 'ngSanitize', 'mgcrea.ngStrap', 'ngAnimate', 'ngPrettyJson']);

applicationAdminApp.config(function ($routeProvider, $datepickerProvider) {

    var dir = "/AngularJs/Apps/ApplicationManagement/Templates/";

    $routeProvider.when('/Home', {
        controller: 'HomeController',
        templateUrl: dir + 'Home.html'
    }).
        when('/Create', {
            controller: 'CreateController',
            templateUrl: dir + 'Create.html'
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



applicationAdminApp.controller("HomeController", function ($scope, $applicationServices) {

    $applicationServices.getAllApplications().then(function (data) {
        $scope.applications = data.Result;
    });

    $scope.delete = function (id) {

        var item = $scope.applications[id];

        $applicationServices.deleteApplication(item.ID).then(function (data) {
            if (data.Success) {
                $scope.applications.splice(id, 1);
            } else {
                alert("error!");
            }
        });

    }
});

applicationAdminApp.controller("CreateController", function ($scope, $applicationServices, $courseServices, $eventServices, $location) {
    $applicationServices.newApplication().then(function (data) {
        $scope.application = data.Result;

        $courseServices.getAllCourses().then(function (courseData) {
            $scope.courses = courseData.Result;
        });
    });

    $scope.SaveApplication = function () {
        $scope.application.CourseAccess = [];
        angular.forEach($scope.courses, function (item, value) {
            angular.forEach(item.CourseYears, function (index1, value1) {
                if (index1.checked == "true") {
                    $scope.application.CourseAccess.push({ ID: 0, ApplicationID: 0, CourseYearID: index1.ID });
                }
            });
        });

        $applicationServices.createApplication($scope.application).then(function (data) {

            if (data.Success) {
                $scope.application = data.Result;

                $eventServices.saveApplicationEvents($scope.events, $scope.application.ID);

                $location.path('/Home').replace();
            } else {
                alert(data.Errors[0]);
            }
        });
    };

    $scope.addRole = function () {
        $scope.application.Roles.push({
            ID: 0,
            Name: ''
        });
    };

    $scope.removeRole = function (index) {
        $scope.application.Roles.splice(index, 1);
    };

    $scope.tabs = [
          {
              "title": "Home",
              "template": "/AngularJs/Apps/ApplicationManagement/Templates/tabs/home.html"
          },
          {
              "title": "Course Access",
              "template": "/AngularJs/Apps/ApplicationManagement/Templates/tabs/courses.html"
          },
          {
              "title": "Role Access",
              "template": "/AngularJs/Apps/ApplicationManagement/Templates/tabs/roles.html"
          },
          {
              "title": "Events",
              "template": "/AngularJs/Apps/EventManagement/Templates/tabs/list.html"
          }
    ];

    $scope.tabs.activeTab = 0;
    $scope.events = [];

    $scope.addEvent = function () {
        $scope.events.push({
            ID: 0,
            Name: '',
            NiceName: '',
            Description: '',
            Enabled: true,
            System: false
    });
    };

    $scope.removeEvent = function (index) {
        $scope.events.splice(index, 1);
    };
});

applicationAdminApp.controller("EditController", function ($scope, $applicationServices, $courseServices, $eventServices, $location, $modal) {
  
    $applicationServices.getApplications($location.search()["id"]).then(function (data) {

        if (data.Success) {

            $scope.application = data.Result[0];

            $courseServices.getAllCourses().then(function (courseData) {
                $scope.courses = courseData.Result;

                angular.forEach($scope.courses, function (item, value) {
                    angular.forEach(item.CourseYears, function (index1, value1) {
                        angular.forEach($scope.application.CourseAccess, function (index2, value2) {

                            if (index2.CourseYearID == index1.ID) {
                                $scope.courses[value].CourseYears[value1].checked = "true";

                            }
                        });
                    });
                });
            });



        } else {
            alert(data.Errors[0]);
        }
    });

    $scope.SaveApplication = function () {
        $scope.application.CourseAccess = [];
        angular.forEach($scope.courses, function (item, value) {
            angular.forEach(item.CourseYears, function (index1, value1) {
                if (index1.checked == "true") {
                    $scope.application.CourseAccess.push({ ID: 0, ApplicationID: $scope.application.ID, CourseYearID: index1.ID });
                }
            });
        });

        $applicationServices.updateApplication($scope.application).then(function (data) {

            if (data.Success) {
                $scope.application = data.Result;

                //todo: DO SOMETHING HERE I>E IF WE DONT MOVE PAGE ON SAVE, NEED TO REFRESH EVENT LIST SO NEW IDs EXIST??
                $eventServices.saveApplicationEvents($scope.events, $scope.application.ID);

                $location.path('/Home').replace();
            } else {
                alert(data.Errors[0]);
            }
        });
    };

    $scope.addRole = function () {
        $scope.application.Roles.push({
            ID: 0,
            Name: ''
        });
    };

    $scope.removeRole = function (index) {
        $scope.application.Roles.splice(index, 1);
    };


    $scope.tabs = [
          {
              "title": "Home",
              "template": "/AngularJs/Apps/ApplicationManagement/Templates/tabs/home.html"
          },
          {
              "title": "Course Access",
              "template": "/AngularJs/Apps/ApplicationManagement/Templates/tabs/courses.html"
          },
          {
              "title": "Role Access",
              "template": "/AngularJs/Apps/ApplicationManagement/Templates/tabs/roles.html"
          },
          {
              "title": "Events",
              "template": "/AngularJs/Apps/EventManagement/Templates/tabs/list.html"
          }
    ];

    $scope.tabs.activeTab = 0;

    $scope.populateEvents = function(tab) {
        if (tab == "Events") {
            if ($scope.events === undefined) {
                $eventServices.getApplicationEvents($scope.application.ID).then(function (eventData) {
                    $scope.events = eventData.Result;
                });
            }
        }
    };



    $scope.addEvent = function () {
        $scope.events.push({
            ID: 0,
            Name: '',
            NiceName: '',
            Description: '',
            Enabled: true,
            System: false
        });
    };

    $scope.removeEvent = function (index) {
        $scope.events.splice(index, 1);
    };
    
    var auditModal = $modal({ template: "/AngularJs/Apps/AuditManagement/Templates/Modals/Audit.html", show: false, scope: $scope, backdrop: 'static' });

    $scope.showAuditModal = function (index) {
        $scope.selectedEvent = $scope.events[index];;
        auditModal.$promise.then(auditModal.show);
    };

    $scope.hideAuditModal = function () {
        auditModal.$promise.then(auditModal.hide);
        $eventServices.getApplicationEvents($scope.application.ID).then(function (eventData) {
            $scope.events = eventData.Result;
        });
        $scope.selectedEvent = null;
    };



    var tagModal = $modal({ template: "/AngularJs/Apps/EventManagement/Templates/Modals/Tags.html", show: false, scope: $scope, backdrop: 'static' });

    $scope.showTagModal = function (index) {
        $scope.selectedEvent = $scope.events[index];
        tagModal.$promise.then(tagModal.show);
    };

    $scope.hideTagModal = function () {
        tagModal.$promise.then(tagModal.hide);
        $scope.selectedEvent = null;
    };


/*    $scope.addTag = function (eventID) {
        $scope.events.push({
            ID: 0,
            EventID: eventID,
            Tag: '',
            Description: ''
        });
    };

    $scope.removeTag = function (index) {
        $scope.events.splice(index, 1);
    };*/
});

applicationAdminApp.controller("AuditController", function ($scope, $applicationServices) {

    $scope.showHide = {};

    $applicationServices.getAuditTrail($location.search()["id"]).then(function (data) {
        $scope.auditList = data.Result;
    });

    $scope.rollBack = function (id) {
        $applicationServices.doRollBack($scope.auditList[id]).then(function (rollBackData) {

            if (rollBackData.Success) {
                $route.reload();
                $location.path('/Audit').replace();
            } else {
                alert(data.Errors[0]);
            }
        });
    }
});

applicationAdminApp.controller("AuditModalController", function ($scope, $eventServices) {
    $scope.showHide = {};

    $eventServices.getAuditTrail($scope.selectedEvent.ID).then(function (data) {
        $scope.auditList = data.Result;
    });

    $scope.rollBack = function (id) {
        $eventServices.doRollBack($scope.auditList[id]).then(function (rollBackData) {

            if (rollBackData.Success) {
                $eventServices.getAuditTrail($scope.selectedEvent.ID).then(function (data) {
                    $scope.auditList = data.Result;
                });
            } else {
                alert("error!");
            }
        });
    }

});

applicationAdminApp.controller("TagModalController", function ($scope) {

    $scope.addTag = function (eventID) {

        if ($scope.selectedEvent.Tags == undefined)
            $scope.selectedEvent.Tags = [];

        $scope.selectedEvent.Tags.push({
            ID: 0,
            EventID: eventID,
            Tag: '',
            Description: ''
        });
    };

    $scope.removeTag = function (index) {
        $scope.selectedEvent.Tags.splice(index, 1);
    };


});