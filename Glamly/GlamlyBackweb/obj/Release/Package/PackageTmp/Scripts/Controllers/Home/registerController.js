
app.controller('registerController', ['$scope', '$http', '$location', 'authData', 'LoginService', '$translate',  '$window','ModelDialogService', function ($scope, $http, $location, authData, loginService, $translate, $window, modelDialogService) {
   
    var _registeration = {
        selectcontent: false,    
        accessToken: ""
        
    };  

   // $scope.message = "register page message";
    $scope.UserData = {};
    $scope.selectcontent = "private";

   // _registeration.selectcontent = true;
    //Register new user  
    $scope.saveUpdateUser = function () {        
        $http({
            method: 'POST',
            url: serviceBase + 'EBApi/User/RegisterUser',
            data: JSON.stringify($scope.UserData),
        })
        .success(function (data) {
            if (data.ResponseCode == 1) {
                modelDialogService.info($scope.TranslateText('COMMON.MESSAGES.REGISTER_SUCCESS'));
               // $scope.UserData = {};
           //     $scope.UserData = null;
                //$scope.message = $translate.use() == "en_US" ? 'User Register Successfully' : 'Användarregistrering framgångsrikt';
                // $scope.reset = ".has-error";   
                // $scope.UserData = null;
                $window.location.href = serviceBase + '#/login';
                $scope.resetRegister();
               // $scope.UserPrivateForm.$setPristine();

            } else if (data.ResponseCode == 5) {
                modelDialogService.warning($scope.TranslateText('COMMON.MESSAGES.REGISTER_WARNING'));
            }
            else {
                modelDialogService.error($scope.TranslateText('COMMON.MESSAGES.COMMON_ERROR'));
                $scope.UserData = null;
               
            }
        })
        .error(function (data) {           
            modelDialogService.error($scope.TranslateText('COMMON.MESSAGES.COMMON_ERROR'));
        });
    }

    $scope.radioselection = function () {
        $scope.UserData = null;
    }

    $scope.resetRegister = function () {       
      //  $scope.UserData = angular.copy($scope.UserData);
        $scope.UserData.Address = "";
        $scope.UserData.City = "";
        $scope.UserData.ConfirmPassword = "";
        $scope.UserData.Email = "";
        $scope.UserData.FirstName = "";
        $scope.UserData.LastName = "";
        $scope.UserData.Mobile = "";
        $scope.UserData.Password = "";
        $scope.UserData.PersonalNumber = "";
        $scope.UserData.PostalNumber = "";
    };

    $scope.TermsAndConditionLinkClick = function () {      
        var url = serviceBase + 'policies/RegistrationPolicies.html';
        $window.open(url);
    }

}]);
