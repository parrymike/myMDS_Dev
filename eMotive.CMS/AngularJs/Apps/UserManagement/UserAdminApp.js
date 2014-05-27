var userAdminApp = angular.module('userAdminApp', ['ngRoute', 'userServices', 'ngSanitize', 'mgcrea.ngStrap', 'ngAnimate', 'ngPrettyJson', 'datatables']);

userAdminApp.config(function ($routeProvider) {

    var dir = "/AngularJs/Apps/UserManagement/Templates/";

    $routeProvider.when('/Home', {
        controller: 'HomeController',
        templateUrl: dir + 'Home.html'
        }).
        otherwise({
            redirectTo: '/Home'
        });
});

userAdminApp.controller("HomeController", function ($scope, $userServices) {

    $userServices.getAllUsers().then(function (data) {
        $scope.users = data.Result;
        
        });
});

