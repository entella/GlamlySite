app.controller('CustomerController', ['$scope', '$http', '$location', 'authData', '$timeout', function ($scope, $http, $location, authData, $timeout) {

    $scope.CurrentPage = 1;
    $scope.RecordsPerPage = 10;
    $scope.RecordsTotalCount = 0;

    $scope.RecordsPerPageOptions = [10, 15, 20, 25, 30, 35, 40]

    $scope.CustomerList = [];

    $scope.orderByField = 'Stylist';
    $scope.reverseSort = false;

    function getCustomers() {
        // $scope.CurrentPage = pageNumber;
        $('#processing').show();
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetCustomers',
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'UserId': authData.authenticationData.Id
            }
        })
            .success(function (data) {
                $('#processing').hide();
                if (data.responseCode !== 1) {
                    $scope.errorMessage = data.ResponseMessage;
                    $timeout(function () { alert($scope.errorMessage) }, 0);
                    if (data.ResponseCode === 5) $window.location.href = serviceBase + '#/login';
                } else {
                    $scope.CustomerList = data.responseData;
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };
    getCustomers();

    $scope.NavigateToCustomerDetail = function (stylist) {
        $location.path('/CustomerProfile/' + stylist.id);
    }

    $scope.RecordsPerPageChangeEvent = function () {
        $scope.CurrentPage = 1;
        $scope.GetUsersByUserType($scope.CurrentPage);
    }



}]);