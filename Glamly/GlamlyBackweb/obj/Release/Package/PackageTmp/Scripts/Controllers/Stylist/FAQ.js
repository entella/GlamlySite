app.controller('FAQController', ['$scope', '$http', 'authData', '$location', '$filter', '$timeout', '$window', '$rootScope', 'ModelDialogService', function ($scope, $http, authData, $location, $filter, $timeout, $window, $rootScope, modelDialogService) {

    $scope.FAQData = {};
    $scope.FAQList = []; 
    $scope.question = "";
    $scope.answer = "";

    $scope.toggleServices = function (id) {
        $("#h2_" + id).find('a').toggleClass('fa fa-chevron-down').toggleClass('fa fa-chevron-up');
        $("#collapse_" + id).slideToggle();
    }

    $scope.reset = function () {
        $scope.FAQData = {};       
    }

    $scope.openPopup = function (v) {
        if (v == 'add') {
            $scope.reset();
            $('#FAQAdd_popup').modal('show');
        } else {
            $('#FAQEdit_popup').modal('show');
        }

    }

    $scope.GetFAQById = function (value) {
        $scope.openPopup();
        $scope.FAQData = value;
        $scope.FAQData.question = value.question;
        $scope.FAQData.answer = value.answer;
    };

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

    function getAllFAQ() {
        $('#processing').show();
        // $scope.CurrentPage = pageNumber;
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetFAQList',
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
                    $scope.FAQList = data.responseData;
                    // $scope.TypeList = data.responseData.service_type;
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };
    getAllFAQ();

     

    $scope.AddNewFAQ = function () {
        $('#processing').show();
        $('#FAQAdd_popup').modal('hide');
        $('#FAQEdit_popup').modal('hide');
        
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/AddFAQ',
            data: JSON.stringify($scope.FAQData),
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'userData': JSON.stringify(authData.authenticationData.userData),
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {
                modelDialogService.info("FAQ has been added successfully");

                var myEl = angular.element(document.querySelector('#FAQAdd_popup'));              
                myEl.removeClass('in');
             
                getAllFAQ();
                $scope.FAQData = null;
            }
            else {
                modelDialogService.error("error");
                $scope.FAQData = null;
            }
        })
    }

    $scope.UpdateFAQData = function (guidimg) {
        $('#processing').show();
        $('#FAQAdd_popup').modal('hide');
        $('#FAQEdit_popup').modal('hide');
        
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/UpdateFAQData',
            data: JSON.stringify($scope.FAQData),
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
                   modelDialogService.info("FAQ has been updated successfully");
                   var myEl = angular.element(document.querySelector('#FAQEdit_popup'));
                   myEl.removeClass('in');
                   getAllFAQ();
                   $scope.FAQData = null;
               }
           })
            .error(function (data) {
                // $.unblockUI();
            });
    };

    $scope.removeFAQ = function (FAQData) {
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/RemoveFAQ',
            data: JSON.stringify($scope.FAQData),
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'userData': JSON.stringify(authData.authenticationData.userData),
            }
        })
         .success(function (data) {
             $('#processing').hide();
             if (data.responseCode == 1) {
                 $('#FAQEdit_popup').modal('hide');
                 modelDialogService.info("FAQ has been remove successfully");
                 getAllFAQ();
                 $scope.StylistData = null;
             }
             else {
                 modelDialogService.error("error");
                 $scope.UserData = null;
             }
         })
    }
    

    $rootScope.goBack = function () {
        $window.history.back();
    }

}]);