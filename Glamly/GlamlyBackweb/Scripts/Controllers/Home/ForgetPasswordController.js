
app.controller('ForgetPasswordController', ['$scope', '$http', '$location', 'authData', 'LoginService', '$translate', '$filter', 'ModelDialogService', function ($scope, $http, $location, authData, loginService, $translate, $filter, modelDialogService) {

    $scope.EmailId = ''; 

    $scope.SendForgetPasswordEmail = function () {
        if (!$scope.ForgetPassowrdForm.$valid) {
            $scope.errorMessage = $filter("translate")("COMMON.ERROR.INVALIDFORM");
            $timeout(function () { alert($scope.errorMessage) }, 1);
            $(".form-control.ng-invalid").focus();
            $(".form-control.ng-invalid").eq(0).focus();
            return;
        }

        $http({
            method: 'POST',
            url: serviceBase + 'EBApi/User/SendForgetPasswordEmail',
            params: { emailId: $scope.EmailId },
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'userData': JSON.stringify(authData.authenticationData.userData),
            }
        })
            .success(function (data) {
                if (data.ResponseCode == 1) {
                    modelDialogService.info("We have sent you a link to reset your password.");
                }
                else {
                    modelDialogService.error("An error has occurred due to network issue");
                }
            })
            .error(function (data) {
                modelDialogService.error("An error has occurred due to network issue");
            });
    }

}]);
