
app.controller('loginController', ['$scope', 'LoginService', '$location', '$translate', 'ModelDialogService', function ($scope,  loginService, $location, $translate, modelDialogService) {

    loginService.logOut();

    $scope.loginData = {
        userName: "",
        password: ""
    };

    $scope.showTypesSelectionDiv = false;

    $scope.message = '';

    $scope.userTypeSelection = function (userTypeSelected) {
        $scope.userType = userTypeSelected;
        $scope.showTypesSelectionDiv = false;
    }
  
    $scope.login = function () {
       
        loginService.login($scope.loginData.userName, $scope.loginData.password).then(function (response) {
          //  debugger;         
            $scope.message = '';
            if (response == undefined || response == null) {
                modelDialogService.error("Du har angivit fel inloggningsuppgifter. Försök igen");
            //    $scope.message = "invalid credentials";
            }
            else {               
                $location.path('/bookings');
            }
        });
    }

    $scope.forgetPassword = function () {
       
        $location.path('/ForgetPassword');
    }

    $scope.registerUser = function () {
        $location.path('/register');
    }

}]);
