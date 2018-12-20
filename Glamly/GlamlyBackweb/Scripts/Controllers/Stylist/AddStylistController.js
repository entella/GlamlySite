app.controller('AddStylistController', ['$scope', '$http', '$location', 'authData', '$timeout', '$window', 'ModelDialogService', function ($scope, $http, $location, authData, $timeout, $window, modelDialogService) {

    $scope.UserData = {};

    $scope.ServicesList = [];
   
    $scope.saveUpdateUser = function () {
        $('#processing').show();
        var serviceNames = [];
        var selectedservice = '';
        var selectedserviceId = '';
        angular.forEach($scope.ServicesList, function (service) {
            if (service.chkvalue) {
                serviceNames.push(service.servicename);
                selectedservice += service.servicename + ',';
                selectedserviceId += service.id + ',';
            }
        });
        $scope.UserData.services = selectedservice.replace(/,\s*$/, "");
        $scope.UserData.serviceId = selectedserviceId.replace(/,\s*$/, "");
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/RegisterProUser',
            data: JSON.stringify($scope.UserData),
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'userData': JSON.stringify(authData.authenticationData.userData),
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {            
                modelDialogService.info("Stylist har lagts till");
                $window.location.href = serviceBase + '#/StylistHome';
              //  $scope.resetRegister();              
            } else if (data.responseCode == 6) {
                modelDialogService.warning("Stylist har redan existerat");
            }
            else {
                modelDialogService.error("error");
                $scope.UserData = null;

            }
        })
        .error(function (data) {
            $('#processing').hide();
            modelDialogService.error("error");
        });
    }


    function GetServices() {
        $('#processing').show();
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetServicesList',
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
                   $scope.ServicesList = data.responseData;
               }
           })
           .error(function (data) {
               $('#processing').hide();
               return null;
           });
    };

    GetServices();


    $scope.AddStylist = function () {        
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

    //$scope.GetServices = function () {     
    //    $http({
    //        method: 'GET',
    //        url: serviceBase + 'GlamlyApi/User/GetServicesList',
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
    //                $scope.ServicesList = data.responseData;
    //            }
    //        })
    //        .error(function (data) {
    //            $.unblockUI();
    //            return null;
    //        });
    //}




    //$scope.GetServices();   

    $scope.showpassword = function () {     
            $('#password').hide();
            $('.input-field').show();
            $('.glyphicon-eye-open').hide();
            $('.glyphicon-eye-close').show();
    }


    $scope.hidepassword = function () {
        $('#password').show();
        $('.input-field').hide();
        $('.glyphicon-eye-open').show();
        $('.glyphicon-eye-close').hide();
    }


}]);