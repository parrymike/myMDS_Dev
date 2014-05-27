    angular.module('userServices', []).factory('$userServices', function ($http) {
        return {//https://github.com/mgonto/restangular <== look into this?
            getAllUsers: function () {
                
                return $http.get("/api/Users/").then(function (result) {
                    return result.data;
                });
            }
        };
    });