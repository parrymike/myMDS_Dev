var eventAdminApp = angular.module('eventAdminApp', ['ngRoute', 'eventServices', 'ngSanitize', 'mgcrea.ngStrap']);

eventAdminApp.config(function ($routeProvider, $datepickerProvider) {

    var dir = "/AngularJs/Apps/EventManagement/Templates/";

    $routeProvider.when('/Home', {
        controller: 'HomeController',
        templateUrl: dir + 'Home.html'
    }).
        otherwise({
            redirectTo: '/Home'
        });
});

eventAdminApp.controller("HomeController", function ($scope, $eventServices) {

    $eventServices.getAllEvents().then(function (data) {
        $scope.events = data.Result;
    });
    
   // $roleServices.doSearch({Query:"Test3 +Rol*"}).then(function (data) {
   //     $scope.roles = data.Result.Roles;
   // });
});