var coursesAdminApp = angular.module('courseAdminApp', ['ngRoute', 'courseServices', 'ngSanitize', 'mgcrea.ngStrap', 'ngAnimate', 'ngPrettyJson']);

coursesAdminApp.config(function ($routeProvider, $datepickerProvider) {

    var dir = "/AngularJs/Apps/CourseManagement/Templates/";

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

coursesAdminApp.directive('validNumber', function () {
                    return {
                        require: '?ngModel',
                        link: function (scope, element, attrs, ngModelCtrl) {
                            if (!ngModelCtrl) {
                                return;
                            }

                            ngModelCtrl.$parsers.push(function (val) {
                                var clean = val.replace(/[^0-9]+/g, '');
                                if (val !== clean) {
                                    ngModelCtrl.$setViewValue(clean);
                                    ngModelCtrl.$render();
                                }
                                return clean;
                            });

                            element.bind('keypress', function (event) {
                                if (event.keyCode === 32) {
                                    event.preventDefault();
                                }
                            });
                        }
                    };
                });
coursesAdminApp.controller("HomeController", function ($scope, $courseServices) {

    $courseServices.getAllCourses().then(function (data) {
        $scope.courses = data.Result;
    });
    
   // $roleServices.doSearch({Query:"Test3 +Rol*"}).then(function (data) {
   //     $scope.roles = data.Result.Roles;
   // });
    $scope.delete = function (id) {
        
        var item = $scope.courses[id];
        //
        $courseServices.deleteCourse(item.ID).then(function (data) {
            if (data.Success) {
                $scope.courses.splice(id, 1);
                $.gritter.add({
                    title: '<i class="fa fa-check"></i> Success',
                    text: 'Course was deleted successfully',
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-success'
                });
            } else {
                $.gritter.add({
                    title: '<i class="fa fa-times"></i> Error',
                    text: data.message,
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-error'
                });
            }
        });

    }
});

coursesAdminApp.controller("CreateController", function ($scope, $courseServices, $location) {
    $courseServices.newCourse().then(function (data) {
        $scope.course = data.Result;
    });

    $scope.SaveCourse = function() {
        $courseServices.createCourse($scope.course).then(function (data) {

            if (data.Success) {
                $scope.course = data.Result;
                $.gritter.add({
                    title: '<i class="fa fa-check"></i> Success',
                    text: 'Course was created successfully',
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-success'
                });
            } else {
                alert("error!");
              /*  $scope.message.text = data.message;
                $scope.message.show = true;
                $scope.message.class = "alert alert-error";
                $scope.QuotaList.show = false;
                $scope.QuotaList.quotas = {};*/
             //   alert(data.Errors[0]);
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

coursesAdminApp.controller("EditController", function ($scope, $courseServices, $location) {
    $courseServices.getCourses($location.search()["id"]).then(function (data) {

        if (data.Success) {
           
            $scope.course = data.Result[0];
            
        } else {
            $.gritter.add({
                title: '<i class="fa fa-times"></i> Error',
                text: data.message,
                sticky: false,
                time: 1500,
                class_name: 'gritter-error'
            });
            alert("error!");
            /*  $scope.message.text = data.message;
              $scope.message.show = true;
              $scope.message.class = "alert alert-error";
              $scope.QuotaList.show = false;
              $scope.QuotaList.quotas = {};*/
            //   alert(data.Errors[0]);
        }
    });


    $scope.SaveCourse = function () {
        $courseServices.updateCourse($scope.course).then(function (data) {

            if (data.Success) {
                $scope.course = data.Result;
                $.gritter.add({
                    title: '<i class="fa fa-check"></i> Success',
                    text: 'Course was edited successfully',
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-success'
                });
            } else {
                alert("error!");
                /*  $scope.message.text = data.message;
                  $scope.message.show = true;
                  $scope.message.class = "alert alert-error";
                  $scope.QuotaList.show = false;
                  $scope.QuotaList.quotas = {};*/
                //   alert(data.Errors[0]);
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

coursesAdminApp.controller("AuditController", function ($scope, $courseServices, $location, $route) {

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