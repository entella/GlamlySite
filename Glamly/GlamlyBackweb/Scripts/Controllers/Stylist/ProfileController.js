app.controller('ProfileController', ['$scope', '$http', 'authData', '$location', '$filter',  '$routeParams','$timeout', '$window','$rootScope', 'ModelDialogService', function ($scope, $http, authData, $location, $filter, $routeParams, $timeout,$window,$rootScope, modelDialogService) {

    $scope.UserData = {};
    $scope.userId = $routeParams.id;
    $scope.IsEdit = false;
    $scope.btnText = 'Redigera konto';
    $scope.editmode = false;
    function getstylistDetail() {
        $('#processing').show();
        // $scope.CurrentPage = pageNumber;
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/GetStylistsById?userId=' + $scope.userId,
            //  params: { userId: $scope.userId },
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
                    $scope.UserData = data.responseData;
                    $scope.servicesData = $scope.UserData.services.split(",") // Delimiter is a string
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };
    getstylistDetail();




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



    function UpdateStylist() {
        $('#processing').show();
        var serviceNames = [];
        var selectedservice = '';
        angular.forEach($scope.ServicesList, function (service) {
            if (service.chkvalue) {
                serviceNames.push(service.servicename);
                selectedservice += service.servicename + ',';
            }
        });
        $scope.UserData.services = selectedservice.replace(/,\s*$/, "");;
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/UpdateStylist',
            data: JSON.stringify($scope.UserData),
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'UserId': authData.authenticationData.Id
            }
        })
           .success(function (data) {
               $('#processing').hide();
               if (data.responseCode != 1) {                
                   $scope.errorMessage = data.ResponseMessage;
                   $timeout(function () { alert($scope.errorMessage) }, 0);
                   if (data.ResponseCode === 5) $window.location.href = serviceBase + '#/login';
               }
               else {
                   getstylistDetail();
                   modelDialogService.info("Stylist har uppdaterats framgångsrikt");
               }
           })
            .error(function (data) {
               // $.unblockUI();
            });
    };





    $scope.EditAction = function () {
        if ($scope.btnText == "Redigera konto") {
            $scope.editmode = true;
            $scope.IsEdit = true;
            $scope.btnText = 'Save';
            //Split the  string with comma
            var a = $scope.UserData.services.split(",") // Delimiter is a string
            for (var x = 0; x < $scope.ServicesList.length; x++) {
                for (var i = 0; i < a.length; i++) {
                    if ($scope.ServicesList[x].servicename == a[i]) {
                        $scope.ServicesList[x].chkvalue = true;
                    }
                }
            }
        } else {

            UpdateStylist();
            $scope.IsEdit = false;
            $scope.btnText = 'Redigera konto';
            getstylistDetail();
        }

        //Loop 
    }


 
        $rootScope.goBack = function(){
            $window.history.back();
        }


    $scope.deleteStylist = function (UserData) {       
        if (confirm('Är du säker att du vill radera?')) {
            DeleteStylist(UserData);
        } else {
            return;
        }
    };

    function DeleteStylist(UserData) {       
        $('#processing').show();      
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/DeleteStylist',
            params: {
                stylistid: $scope.userId
            },
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'userData': JSON.stringify(authData.authenticationData.userData),
                'UserId': authData.authenticationData.Id
            }
        })
        .success(function (data) {           
            $('#processing').hide();

            if (data.responseCode == 21) {
                modelDialogService.warning("stylist kan inte raderas eftersom han har bokning");                                     
            }
            else {
                modelDialogService.info("Stylist har tagits bort");
                $window.location.href = serviceBase + '#/StylistHome';
                getstylistDetail();
            }          
        })
        .error(function (data) {
            $('#processing').hide();           
        });
    }  

}]);