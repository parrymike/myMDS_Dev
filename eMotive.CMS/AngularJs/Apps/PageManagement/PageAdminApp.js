var pageAdminApp = angular.module('pageAdminApp', ['ngRoute', 'pageServices', 'ngSanitize', 'mgcrea.ngStrap', 'ngPrettyJson', 'ui.tinymce']);

pageAdminApp.config(function ($routeProvider) {

    var dir = "/AngularJs/Apps/PageManagement/Templates/";

    $routeProvider.when('/Home', {
        controller: 'HomeController',
        templateUrl: dir + 'Home.html'
    })
        .when('/Pages', {
        controller: 'PagesController',
        templateUrl: dir + 'Pages.html'
    })
        .when('/CreateSection', {
        controller: 'CreateSectionController',
        templateUrl: dir + 'CreateSection.html'
    }).
        when('/CreatePage', {
            controller: 'CreatePageController',
            templateUrl: dir +'CreatePage.html'
        }).when('/EditPage', {
            controller: 'EditPageController',
            templateUrl: dir + 'EditPage.html'
        }).
        when('/Edit', {
            controller: 'EditController',
            templateUrl: dir + 'Edit.html'
        }).when('/EditSection', {
            controller: 'EditSectionController',
            templateUrl: dir + 'EditSection.html'
        }).
        when('/Audit', {
            controller: 'AuditController',
            templateUrl: dir + 'Audit.html'
        }).
        otherwise({
            redirectTo: '/Home'
        });
});


pageAdminApp.controller("HomeController", function ($scope, $pageServices) {

    $pageServices.getAllSections().then(function (data) {
        $scope.sections = data.Result;
    });
    
    $scope.delete = function (index) {
        
        var item = $scope.sections[index];
        //
        $pageServices.deleteSection(item.ID).then(function (data) {
            if (data.Success) {
                $scope.sections.splice(index, 1);
            } else {
                alert("error!");
            }
        });
    }
});


pageAdminApp.controller("CreateSectionController", function ($scope, $pageServices) {
    $pageServices.newSection().then(function (data) {
        $scope.section = data.Result;
    });

    $scope.SaveSection = function() {
        $pageServices.createSection($scope.section).then(function (data) {
            if (data.Success) {
                $.gritter.add({
                    title: '<i class="fa fa-check"></i> Success',
                    text: "The section '" + $scope.section.Name + "' was successfully created",
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-success'
                });
            } else {
                alert("error!");

            }
        });
    };
});

pageAdminApp.controller("EditSectionController", function ($scope, $location, $pageServices) {

    $pageServices.getSections($location.search()["id"]).then(function (data) {
        $scope.section = data.Result[0];
    });

    $scope.SaveSection = function () {
        $pageServices.updateSection($scope.section).then(function (data) {
            if (data.Success) {
                $.gritter.add({
                    title: '<i class="fa fa-check"></i> Success',
                    text: "The section '" + $scope.section.Name + "' was successfully updated",
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-success'
                });
            } else {
                alert("error!");

            }
        });
    };
});

pageAdminApp.controller("PagesController", function ($scope, $location, $pageServices) {

    $pageServices.getSections($location.search()["id"]).then(function (data) {
        $scope.section = data.Result[0];
    });
});


pageAdminApp.controller("CreatePageController", function ($scope, $location, $pageServices) {
    var sectionID = $location.search()["section"];
    $pageServices.newPage().then(function (data) {
        $scope.page = data.Result;
        $scope.page.PageSection = sectionID;
    });

    $pageServices.getSections(sectionID).then(function (data) {
        $scope.section = data.Result[0];
    });

    $scope.savePage = function() {
        $pageServices.createPage($scope.page).then(function (data) {
            if (data.Success) {
                $scope.page = data.Result;

                $.gritter.add({
                    title: '<i class="fa fa-check"></i> Success',
                    text: 'The page was created successfully',
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-success'
                });
            } else {
                alert(data.Errors[0]);
            }
        });
    };

});

pageAdminApp.controller("EditPageController", function ($scope, $location, $pageServices) {
    var sectionID = $location.search()["section"];
    var pageID = $location.search()["page"];
    $pageServices.getPages(pageID).then(function (data) {
        $scope.page = data.Result[0];
    });

    $pageServices.getSections(sectionID).then(function (data) {
        $scope.section = data.Result[0];
    });

    $scope.savePage = function () {
        $pageServices.updatePage($scope.page).then(function (data) {
            if (data.Success) {
                $scope.page = data.Result;

                $.gritter.add({
                    title: '<i class="fa fa-check"></i> Success',
                    text: 'The page was updated successfully',
                    sticky: false,
                    time: 1500,
                    class_name: 'gritter-success'
                });
            } else {
                alert(data.Errors[0]);
            }
        });
    };

});

pageAdminApp.controller("AuditController", function ($scope, $pageServices, $location, $route) {

    $scope.showHide = {};

    var sectionID = $location.search()["section"];

    $pageServices.getAuditTrail($location.search()["id"]).then(function (data) {
        $scope.auditList = data.Result;
    });


    $pageServices.getSections(sectionID).then(function (data) {
        $scope.section = data.Result[0];
    });

    $scope.rollBack = function (id) {
        $pageServices.doRollBack($scope.auditList[id]).then(function (rollBackData) {

            if (rollBackData.Success) {
                $route.reload();
                $location.path('/Audit').replace();
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