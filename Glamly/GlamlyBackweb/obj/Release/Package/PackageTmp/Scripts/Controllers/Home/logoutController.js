app.controller('logOutController', ['$scope', '$http', '$location', 'authData', 'LoginService', '$window', function ($scope, $http, $location, authData, loginService, $window) {

    $scope.logOut = function () {
        loginService.logOut();
       
        $location.path('/login');
    }
    $scope.logOut();
}]);
