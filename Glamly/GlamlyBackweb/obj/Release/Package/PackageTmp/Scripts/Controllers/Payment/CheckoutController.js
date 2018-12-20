app.controller('CheckoutController', ['$scope', '$http', '$location', 'authData', '$routeParams', 'LoginService', '$window', '$timeout', function ($scope, $http, $location, authData, $routeParams, loginService, $window, $timeout) {
    $scope.paymentData = [];
    
    $scope.getPaymentData = function () {
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/GetPaymentData?paymentid=' + $routeParams.paymentid
        }).success(function (data) {
            if (data.responseCode !== 1) {
                $scope.errorMessage = data.ResponseMessage;
                $timeout(function () { alert($scope.errorMessage) }, 0);
                if (data.ResponseCode === 5) $window.location.href = serviceBase + '#/login';
            } else {
                $scope.paymentData = data.responseData;
                $timeout(function () { $("#dibsform").submit(); }, 500);
            }
        }).error(function (data) {
            return null;
        });
    };

    $scope.getPaymentData();
}]);