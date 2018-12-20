
app.controller('ResetPasswordController', ['$scope', '$http', '$location', 'authData', 'LoginService', '$translate', 'ModelDialogService', function ($scope, $http, $location, authData, loginService, $translate, modelDialogService) {
    
    $scope.UserKey = $location.url().split('=')[1];

    $scope.ChangePassword = {
        Id: "",
        UserEmail: "",
        OldPassword: "",
        NewPassword: "",
        ConfirmPassword: ""
    };

    $scope.ValidateUserURL = function () {
       
      //  blockUI();
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/ValidateUserByKey?userKey=' + $scope.UserKey
        })
            .success(function (data) {
                if (data.responseCode !== 1) {
                    //$.unblockUI();
                    //$scope.errorMessage = data.ResponseMessage;
                    //$timeout(function () { alert($scope.errorMessage) }, 0);
                    if (data.responseCode === 2) $window.location.href = serviceBase + '#/login';
                } else {
                  //  $.unblockUI();
                    $scope.UserData = data.responseData;
                }
            })
            .error(function (data) {
               // $.unblockUI();
                $scope.errorMessage = 'An error has occurred due to network issue';
                return null;
            });
    }
    $scope.ValidateUserURL();

    $scope.ResetPassword = function () {
        ResetUserPassword();
    }

    function ResetUserPassword(ChangePassword) {
       
        if ($scope.ChangePassword == undefined || $scope.ChangePassword == null)
        {
            modelDialogService.warning("Your reset key is expire.Try again");
            return
        }
           
        $scope.ChangePassword.Id = $scope.UserData.id;
        $scope.ChangePassword.UserEmail = $scope.UserData.user_email;
        var userDataString = JSON.stringify($scope.ChangePassword);
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/ResetPassword',
            data: userDataString,
        })
    .success(function (data) {
       
        if (data.responseCode == 1) {
            modelDialogService.info("Your password has been reset successfully");
           // $scope.reset();
        }
        else if (data.responseCode == 4)
            modelDialogService.error("Email does not exist.Please register first");
        else
            modelDialogService.error("An error has occurred due to network issue");
    })
    .error(function (data) {
        modelDialogService.error("An error has occurred due to network issue");
    });
    }

}]);
