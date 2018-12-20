app.controller('DashboardController', ['$scope', '$http', '$location', 'authData', '$timeout', function ($scope, $http, $location, authData, $timeout) {

    $scope.CurrentPage = 1;
    $scope.RecordsPerPage = 10;
    $scope.RecordsTotalCount = 0;

    $scope.RecordsPerPageOptions = [10, 15, 20, 25, 30, 35, 40]

    $scope.StylistList = [];

    $scope.orderByField = 'Stylist';
    $scope.reverseSort = false;
 
    function getUserDetail() {
        // $scope.CurrentPage = pageNumber;
        $('#processing').show();
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetAllStylists',
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
                    $scope.StylistList = data.responseData;
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };
      getUserDetail();

      $scope.NavigateToStylistDetail = function (stylist) {    
          $location.path('/Profile/' + stylist.id);
      }


    //$scope.GetUsersByUserType = function (pageNumber) {    
    //    $scope.CurrentPage = pageNumber;
    //    $http({
    //        method: 'GET',
    //        url: serviceBase + 'GlamlyApi/User/GetAllStylists_WithPaging?recordsPerPage=' + $scope.RecordsPerPage + '&pageNumber=' + $scope.CurrentPage,
    //        headers: {
    //            'Authorization': authData.authenticationData.accessToken,
    //            'UserId': authData.authenticationData.Id
    //        }
    //    })
    //        .success(function (data) {
    //            if (data.responseCode !== 1) {
    //                $scope.errorMessage = data.ResponseMessage;
    //                $timeout(function () { alert($scope.errorMessage) }, 0);
    //                if (data.ResponseCode === 5) $window.location.href = serviceBase + '#/login';
    //            } else {
    //                $scope.StylistList = data.responseData;
    //            }
    //        })
    //        .error(function (data) {
    //            $.unblockUI();
    //            return null;
    //        });
    //}


  //  $scope.GetUsersByUserType($scope.CurrentPage);
 
    $scope.RecordsPerPageChangeEvent = function () {
        $scope.CurrentPage = 1;
        $scope.GetUsersByUserType($scope.CurrentPage);
    }

}]);