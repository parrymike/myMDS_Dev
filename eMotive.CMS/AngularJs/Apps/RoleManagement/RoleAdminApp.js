var coursesAdminApp = angular.module('courseAdminApp', ['ngRoute', 'roleServices', 'ngSanitize']);

roleAdminApp.config(function ($routeProvider) {

    var dir = "/AngularJs/Apps/RoleManagement/Templates/";

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

roleAdminApp.controller("HomeController", function ($scope, $roleServices) {

    $roleServices.getAllRoles().then(function (data) {
        $scope.roles = data.Result;
    });
    
   // $roleServices.doSearch({Query:"Test3 +Rol*"}).then(function (data) {
   //     $scope.roles = data.Result.Roles;
   // });
    $scope.delete = function (id) {
        var item = $scope.roles[id];
        //
        $roleServices.deleteRole(item).then(function (data) {
            if (data.Success) {
                $scope.invoice.items.splice(id, 1);
            } else {
                alert("error!");
            }
        });

    }
});

roleAdminApp.controller("CreateController", function ($scope, $roleServices, $location) {
    $roleServices.newRole().then(function (data) {
        $scope.role = data.Result;
    });

    $scope.SaveRole = function() {
        $roleServices.createRole($scope.role).then(function (data) {

            if (data.Success) {
                $scope.role = data.Result;
                $location.path('/Home').replace();
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
});

roleAdminApp.controller("EditController", function ($scope, $roleServices, $location) {
    $roleServices.getRoles($location.search()["id"]).then(function (data) {

        if (data.Success) {
           
            $scope.role = data.Result[0];
            
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


    $scope.SaveRole = function() {
        $roleServices.updateRole($scope.role).then(function(data) {
            if (data.Success) {
                $location.path('/Home').replace();
            } else {
                alert("error!");
            }
        });
    };

});

roleAdminApp.controller("AuditController", function ($scope, $roleServices, $location, $route) {
    $roleServices.getAuditTrail($location.search()["id"]).then(function (data) {
        $scope.auditList = data.Result;
    });

    $scope.rollBack = function(id) {
        $roleServices.doRollBack($scope.auditList[id]).then(function (rollBackData) {
            
            if (rollBackData.Success) {
                $route.reload();
                $location.path('/Audit').replace();
            } else {
                alert("error!");
            }
        });
    }

    $scope.syntaxHighlight = function (json) {
        if (typeof json != 'string') {
            json = JSON.stringify(json, undefined, 2);
        }
        json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
        return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
            var cls = 'number';
            if (/^"/.test(match)) {
                if (/:$/.test(match)) {
                    cls = 'key';
                } else {
                    cls = 'string';
                }
            } else if (/true|false/.test(match)) {
                cls = 'boolean';
            } else if (/null/.test(match)) {
                cls = 'null';
            }
            return '<span class="' + cls + '">' + match + '</span>';
        });
    }
});