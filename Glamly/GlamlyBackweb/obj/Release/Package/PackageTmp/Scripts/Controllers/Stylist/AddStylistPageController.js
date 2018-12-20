app.controller('AddStylistPageController', ['$scope', '$http', '$location', 'authData', '$timeout', '$window', 'ModelDialogService', 'fileReader', 'Upload', function ($scope, $http, $location, authData, $timeout, $window, modelDialogService, fileReader, Upload) {

    $scope.StylistData = {};

    $scope.StylistDataList = [];

    $scope.photo = "";


    $scope.removeimg = function () {
        $scope.photo = "";
    }
    $scope.reset = function () {
        $scope.StylistData = {};
        $scope.photo = "";      
    }
   
    $scope.AddStylist = function () {
        $scope.StylistData.FirstName = "";
        $scope.StylistData.LastName = "";
        $scope.StylistData.RowOne = "";
        $scope.StylistData.RowTwo = "";
        $scope.StylistData.RowThree = "";
    };
    $scope.openPopup = function (v) {
        if (v == 'add') {
            $scope.reset();
            $('#stylistedit_popup').modal('show');
        } else {
            $('#Editstylistedit_popup').modal('show');
        }

    }


    $scope.AddStylistPage = function (guidimg) {
        $('#processing').show();
        //$scope.reset();
        $scope.StylistData.profileimageguid = guidimg;
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/AddStylistPage',
            data: JSON.stringify($scope.StylistData),
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'userData': JSON.stringify(authData.authenticationData.userData),
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {
                modelDialogService.info("Stylist has been added successfully");               
                var myEl = angular.element(document.querySelector('#stylistedit_popup'));               
                myEl.removeClass('in');             
                getAllStylistPage();
                $scope.StylistData = null;
            }
            else {
                modelDialogService.error("error");
                $scope.UserData = null;
            }
        })
    }

    $scope.UpdateStylistPage = function (guidimg) {
        $('#processing').show();
        if (guidimg != '') {
            $scope.StylistData.profileimageguid = guidimg;
        } else {
            $scope.StylistData.profileimageguid = '';
        }
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/UpdateStylistPage',
            data: JSON.stringify($scope.StylistData),
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
                   modelDialogService.info("Stylist has been updated successfully");
                   var myEl = angular.element(document.querySelector('#Editstylistedit_popup'));
                   myEl.removeClass('in');
                   getAllStylistPage();
               }
           })
            .error(function (data) {
                // $.unblockUI();
            });
    };

    $scope.uploadPictures = function (value) {
        $scope.messageKey = "";
        $scope.messageClass = "";
        $scope.progress = "";
        var photo = $scope.photo;
        $scope.errorMsg = null;
        $('#stylistedit_popup').modal('hide');
        $('#Editstylistedit_popup').modal('hide');
        if (parseInt($scope.photo.size / (1024 * 1024)) > 4) {
            modelDialogService.warning("Please select valid file less than 5MB.");
            return;
        }

        if (!$scope.photo.size) {
            if (value == "add") {
                $scope.AddStylistPage('');
                return;
            }

            if (value == "edit") {
                $scope.UpdateStylistPage('');
                return;
            }
        }

        Upload.upload({
            data: { file: photo },
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'UserId': authData.authenticationData.Id
            },
            url: serviceBase + 'GlamlyApi/User/SaveFiles',
        }).then(function (response) {
            var guidimg = response.data;
           
            if (value == "add") {
                $scope.AddStylistPage(guidimg);
            }
            if (value == "edit") {
                $scope.UpdateStylistPage(guidimg);
            }


        }, function (response) {
            if (response.status > 0) {
                $scope.messageKey = response.status + ': ' + response.data;
                $scope.messageClass = "danger";
            }
        }, function (evt) {
            // Math.min is to fix IE which reports 200% sometimes
            $scope.progress = Math.min(95, parseInt(100.0 * evt.loaded / evt.total));
        });
    }


    function getAllStylistPage() {
        $('#processing').show();
        // $scope.CurrentPage = pageNumber;
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetStylistPage',
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
                    $scope.StylistDataList = data.responseData;
                    // $scope.TypeList = data.responseData.service_type;
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };

    getAllStylistPage();

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


    $scope.GetStylistPagebyId = function (value) {
        $scope.openPopup();
        $scope.StylistData = value;
        $scope.StylistData.skillone = value.skill1;
        $scope.StylistData.skillsecond = value.skill2;
        $scope.StylistData.skillthird = value.skill3;
        $scope.photo = value.profileimageguid;       
    };

    $scope.removeStylitPage = function (StylistData) {
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/RemoveStylistPage',
            data: JSON.stringify($scope.StylistData),
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'userData': JSON.stringify(authData.authenticationData.userData),
            }
        })
         .success(function (data) {
             $('#processing').hide();
             if (data.responseCode == 1) {
                 $('#Editstylistedit_popup').modal('hide');
                 modelDialogService.info("StylistPage has been remove successfully");                
                 getAllStylistPage();
                 $scope.StylistData = null;
             }
             else {
                 modelDialogService.error("error");
                 $scope.UserData = null;
             }
         })
    }

    

    $scope.imageSrc = "";

    app.directive("ngFileSelect", function (fileReader, $timeout) {

        return {
            scope: {
                ngModel: '='
            },
            link: function ($scope, el) {
                function getFile(file) {
                    fileReader.readAsDataUrl(file, $scope)
                      .then(function (result) {
                          $timeout(function () {
                              $scope.ngModel = result;
                          });
                      });
                }

                el.bind("change", function (e) {
                    var file = (e.srcElement || e.target).files[0];
                    getFile(file);
                });
            }
        };
    });


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


    $scope.toggleServices = function (id) {
        $("#h2_" + id).toggleClass('fa fa-chevron-down').toggleClass('fa fa-chevron-up');
        $("#collapse_" + id).slideToggle();
    }

}]);