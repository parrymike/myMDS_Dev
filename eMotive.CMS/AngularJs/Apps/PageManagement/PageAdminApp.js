var coursesAdminApp = angular.module('pageAdminApp', ['ngRoute', 'pageServices', 'ngSanitize', 'mgcrea.ngStrap', 'ngPrettyJson']);

pageAdminApp.config(function ($routeProvider) {

    var dir = "/AngularJs/Apps/PageManagement/Templates/";

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
});


pageAdminApp.controller("HomeController", function ($scope, $courseServices) {

    $courseServices.getAllSections().then(function (data) {
        $scope.sections = data.Result;
    });
    
    $scope.delete = function (index) {
        
        var item = $scope.sections[index];
        //
        $courseServices.deleteSection(item.ID).then(function (data) {
            if (data.Success) {
                $scope.sections.splice(index, 1);
            } else {
                alert("error!");
            }
        });
    }
});
/*
pageAdminApp.controller("CreateController", function ($scope, $courseServices, $location) {
    $courseServices.newCourse().then(function (data) {
        $scope.course = data.Result;
    });

    $scope.SaveCourse = function() {
        $courseServices.createCourse($scope.course).then(function (data) {

            if (data.Success) {
                $scope.course = data.Result;
                $location.path('/Home').replace();
            } else {
                alert("error!");

            }       
        });
    };

    $scope.addCourseYear = function () {
        $scope.course.CourseYears.push({
            ID: 0,
            Name: '',
            Abbreviation: '',
            CourseID: $scope.course.ID,
            YearStart: '',
            CourseYear: ''
        });
    };

    $scope.removeCourseYear = function (index) {
        $scope.course.CourseYears.splice(index, 1);
    };
});

pageAdminApp.controller("EditController", function ($scope, $courseServices, $location) {
    $courseServices.getCourses($location.search()["id"]).then(function (data) {

        if (data.Success) {
           
            $scope.course = data.Result[0];
            
        } else {
            alert("error!");
        }
    });


    $scope.SaveCourse = function () {
        $courseServices.updateCourse($scope.course).then(function (data) {

            if (data.Success) {
                $scope.course = data.Result;
                $location.path('/Home').replace();
            } else {
                alert("error!");
            }
        });
    };

    $scope.addCourseYear = function () {
        $scope.course.CourseYears.push({
            ID: 0,
            Name: '',
            Abbreviation: '',
            CourseID: $scope.course.ID,
            YearStart: '',
            CourseYear: ''
        });
    };

    $scope.removeCourseYear = function (index) {
        $scope.course.CourseYears.splice(index, 1);
    };

});

pageAdminApp.controller("AuditController", function ($scope, $courseServices, $location, $route) {

    $scope.showHide = {};

    $courseServices.getAuditTrail($location.search()["id"]).then(function (data) {
        $scope.auditList = data.Result;
    });

    $scope.rollBack = function(id) {
        $courseServices.doRollBack($scope.auditList[id]).then(function (rollBackData) {
            
            if (rollBackData.Success) {
                $route.reload();
                $location.path('/Audit').replace();
            } else {
                alert("error!");
            }
        });
    }
});
*/