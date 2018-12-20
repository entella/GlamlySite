app.controller('ServicesController', ['$scope', '$http', 'authData', '$location', '$filter', '$timeout', '$window','$rootScope','ModelDialogService', function ($scope, $http, authData, $location, $filter, $timeout,$window,$rootScope, modelDialogService) {

    $scope.ServiceTypeList = [];
    $scope.TypeList = [];
    $scope.IsEdit = false;
    $scope.typename = "";
    $scope.price = "";
   
    $scope.toggleServices = function (id) {
        $("#h2_" + id).find('a').toggleClass('fa fa-chevron-down').toggleClass('fa fa-chevron-up');
        $("#collapse_" + id).slideToggle();
    }

    $scope.openDefaultList = function () {
        if ($scope.CustomOpenTabId > 0) {
            $scope.CustomOpenTabId = 0;
            $scope.toggleServices($scope.CustomOpenTabId);
        }
        else {
            $scope.toggleServices($scope.DefaultOpenTabId);
        }
    }

    $scope.DefaultOpenTabId = 0;
    $scope.CustomOpenTabId = 0;

    $scope.ShowDetailsOrNot = function (id) {
        if ($scope.CustomOpenTabId == id) {
            $scope.CustomOpenTabId = 0;
            return 'display:block';
        }
        else if ($scope.DefaultOpenTabId == id) {
            return 'display: block';
        }
        else {            
            return 'display: none';
        }

    }

    $scope.ShowAnchorIcon = function (id) {
        if ($scope.CustomOpenTabId == id) {
            return 'fa fa-chevron-up';
        }
        else if ($scope.DefaultOpenTabId == id) {
            return 'fa fa-chevron-up';
        }
        else {
            return 'fa fa-chevron-down';
        }

    }

    function getAllServices() {
        $('#processing').show();
        // $scope.CurrentPage = pageNumber;
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetServices',
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
                    $scope.ServiceModel.Services = data.responseData;
                   // $scope.TypeList = data.responseData.service_type;
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };

    getAllServices();
  
  
    $scope.ServiceModel = {
        Services: [],
        selected: {},
        ServiceType: '',
        Price:''
    };

    $scope.reset = function () {       
        //$scope.ServiceModel.selected = {};
        //$scope.ServiceModel.ServiceType = '';
        //$scope.ServiceModel.Price = '';
        $('input[type=text]').val('');
    };
    // gets the template to ng-include for a table row / item
    $scope.getTemplate = function (Service) {
        if (Service.id === $scope.ServiceModel.selected.id)
            return 'edit';
        else
            return 'display';
    };

    $scope.editService = function (Service) {
        $scope.ServiceModel.selected = angular.copy(Service);
    };
    $scope.IsButtonDisabled = function () {
        if ($scope.ServiceModel.ServiceType) { return false; } else { return true; }
    }


    $scope.saveType = function (sname,id) {
        
        $('#processing').show();
        if ($('#txtName_' + id).val() == '' || $('#txtPrice_' + id).val() == '') {
            modelDialogService.info("Service type should not be empty");
            $('#processing').hide();
            return;
        }
        var typeName = $('#txtName_' + id).val(); //$scope.ServiceModel.ServiceType;
        var typePrice = $('#txtPrice_' + id).val();// $scope.ServiceModel.Price;
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/AddServiceType',
            params: {
                ServiceType: typeName,
                Price: typePrice,
                ServiceName: sname
            },
            headers: {
                'Authorization': authData.authenticationData.accessToken
            }
        })
        .success(function (data) {
            $('#processing').hide();
            
            if (data.responseCode == 1) {
                modelDialogService.info("Service type has been saved successfully");            
              //  $scope.ServicesForm.AddNameText.$setUntouched();
                getAllServices();
                $scope.reset();
            } else if (data.responseCode == 6) {
                modelDialogService.info("Service type already exist");            
            }
            else {
                modelDialogService.info("Internal server error");            
            }
        })
        .error(function () {
            modelDialogService.info("Internal server error");           
        });
    };

    $scope.addService = function (sname) {
        
        $('#processing').show();            
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/AddService',
            params: {              
                ServiceName: sname
            },
            headers: {
                'Authorization': authData.authenticationData.accessToken
            }
        })
        .success(function (data) {
            $('#processing').hide();
            
            if (data.responseCode == 1) {
                modelDialogService.info("Service has been saved successfully");
                //  $scope.ServicesForm.AddNameText.$setUntouched();
                getAllServices();
                $scope.reset();
            } else if (data.responseCode == 6) {
                modelDialogService.info("Service already exist");
            }
            else {
                modelDialogService.info("Internal server error");
            }
        })
        .error(function () {
            modelDialogService.info("Internal server error");
        });
    };

    $scope.updateService = function (idx) {
        
        $('#processing').show();
        var selectedService = JSON.stringify($scope.ServiceModel.selected);
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/UpdateService',
            data: selectedService,
            headers: {
                'Authorization': authData.authenticationData.accessToken
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {
                modelDialogService.info("Service type has been updated successfully");              
                getAllServices();
                $scope.reset();
            } else if (data == 2) {
                modelDialogService.info("Service type already exist");              
            }
            else {
                modelDialogService.info("Internal server error");             
            }
        })
        .error(function (data) {
            //Redirect to error page
            modelDialogService.info("Internal server error");        
        });
    };

    $scope.deleteService = function (Service) {     
        if (confirm('Are you sure you want to delete?')) {
            deleteService(Service);
        } else {
            return;
        }
    };

    function deleteService(Service) {
        $('#processing').show();
        var Serviceid = Service.id;
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/DeleteService',
            params: {
                id: Serviceid
            },
            headers: {
                'Authorization': authData.authenticationData.accessToken
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {
                modelDialogService.info("Service type has been deleted successfully");           
                getAllServices();
                $scope.reset();
            }
            else {
                modelDialogService.info("Internal server error");
            }
        })
        .error(function (data) {
            //Redirect to error page
            modelDialogService.info("Internal server error");
        });
    }  
    $rootScope.goBack = function () {
        $window.history.back();
    }  

}]);