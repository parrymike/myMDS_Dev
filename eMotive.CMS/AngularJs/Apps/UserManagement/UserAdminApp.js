var userAdminApp = angular.module('userAdminApp', ['ngRoute', 'userServices', 'ngSanitize', 'mgcrea.ngStrap', 'ngAnimate', 'ngPrettyJson', 'ngTable'])

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

userAdminApp.controller("HomeController", function ($scope, $userServices, $filter, ngTableParams) {

    $userServices.getAllUsers().then(function (data_) {
        $scope.users = data_.Result;

        var data = $scope.users;

        $scope.tableParams = new ngTableParams({
            page: 1,                // show first page
            count: 10,              // show 10 per page
            sorting: {name: 'asc'}  // initial sorting
        }, {
            total: data.length,     // length of data
            getData: function ($defer, params) {
                // use build-in angular filter
                var filteredData = params.filter() ?
                        $filter('filter')(data, params.filter()) :
                        data;
                var orderedData = params.sorting() ?
                        $filter('orderBy')(filteredData, params.orderBy()) :
                        data;

                params.total(orderedData.length); // set total for recalc pagination
                $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
            }
        });
    });
});



