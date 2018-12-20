app.controller('CustomerProfileController', ['$scope', '$http', 'authData', '$location', '$filter', '$routeParams', '$timeout', '$window', '$rootScope', 'ModelDialogService', function ($scope, $http, authData, $location, $filter, $routeParams, $timeout, $window, $rootScope, modelDialogService) {

    $scope.UserData = {};
    $scope.userId = $routeParams.id;
    $scope.IsEdit = false;
    $scope.btnText = 'Redigera konto';
    $scope.editmode = false;


    function getcustomerDetail() {
        $('#processing').show();
        // $scope.CurrentPage = pageNumber;
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/GetCustomerById?userId=' + $scope.userId,
            //  params: { userId: $scope.userId },
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'UserId': authData.authenticationData.Id
            }
        })
            .success(function (data) {
                $('#processing').hide();
               if (data.responseCode == 5) {
                    modelDialogService.warning("Yet there is no any booking!");
                } else {
                    $scope.UserData = data.responseData;
                 //   $scope.servicesData = $scope.UserData.services.split(",") // Delimiter is a string
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };
    getcustomerDetail();
   

}]);