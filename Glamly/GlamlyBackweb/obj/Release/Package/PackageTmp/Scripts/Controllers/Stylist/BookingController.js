app.controller('BookingController', ['$scope', '$http', 'authData', '$location', '$filter', '$timeout', 'ModelDialogService', function ($scope, $http, authData, $location, $filter, $timeout, modelDialogService) {

    $scope.BookingList = [];

    $scope.servicetypeList = [];

    $scope.StylistNameList = [];

    $scope.ServiceMakeup = [];
    $scope.ServiceHarvard = [];
    $scope.ServiceLaglar = [];
    $scope.ServiceFransar = [];
    $scope.selectedTestAccount = null;

    $scope.ServiceModel = {
        BookingList: [],
        DraftBookingList: [],
        servicetypeList: [],
        OngoingBookingList: [],
        CancelBookingList: [],
        PendingBookingList: [],
        stylistID: "",
        rejectedbystylist:""
    };

    $scope.StylistName = "";
    $scope.closemode = true;
    $scope.closemode1 = true;
    $scope.closemode2 = true;
    $scope.closemode3 = true;


    function getbookings() {
        
        $('#processing').show();
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetBookings',
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
                    $scope.ServiceModel.BookingList = data.responseData.sort(function (a, b) { return (a.datetime > b.datetime) ? 1 : ((b.datetime > a.datetime) ? -1 : 0); });
                    $scope.ServiceModel.DraftBookingList = data.responseData.filter(function (o) {
                        return o.workflowstatus === 0 || o.workflowstatus === 41;
                    });
                    $scope.ServiceModel.OngoingBookingList = data.responseData.filter(function (o) {
                        return o.workflowstatus === 21 
                    });
                    $scope.ServiceModel.PendingBookingList = data.responseData.filter(function (o) {
                        return o.workflowstatus === 11
                    });
                    // $scope.ServiceModel.OngoingBookingList.sort(function (a, b) { return (a.datetime > b.datetime) ? 1 : ((b.datetime > a.datetime) ? -1 : 0); });
                    $scope.ServiceModel.CancelBookingList = data.responseData.filter(function (o) {
                        return o.workflowstatus === 51 || o.workflowstatus === 71;
                    });
                    getstylistByService();
                    angular.forEach($scope.ServiceModel.BookingList, function (servicename) {
                        $scope.StylistName = data.responseData[0].servicewithtypes[0].servicename;
                    });
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };

    getbookings();

    function getstylistByService() {

        $('#processing').show();
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetstylistByService',
            data: JSON.stringify($scope.StylistName),
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
                    $scope.StylistNameList = data.responseData;
                    $scope.allDataList = data.responseData;

                    angular.forEach($scope.ServiceModel.BookingList, function (booking) {
                        booking.stylists = $scope.GetstylistByServiceId(booking.servicewithtypes[0].servicename, booking.datetime);
                    });
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };

    $scope.GetstylistByServiceId = function (name, datetime) {
        //Add the datetime check also for the stylist calendar date matched with booking date
        $scope.StylistByCurrentdate = [];
        $scope.eventsListNew = [];
        $scope.eventsListNew1 = [];
        var IsExistStylist = false;
        $scope.tempStyleId = [];
        var dataToFind = moment(datetime).format('YYYY-MM-DD');
        angular.forEach($scope.events, function (evt) {
            angular.forEach(evt.date, function (evtdt) {
                evtdt.id = evt.stylistId;
                $scope.eventsListNew.push(evtdt);
            })
        });

        $.each($scope.eventsListNew, function (index, evt) {
            if (moment(evt.date).format('YYYY-MM-DD') == dataToFind) {
                $scope.tempStyleId.push(evt);
            }
        });
        
        $.each($scope.tempStyleId, function (index, event) {
            var events = $.grep($scope.eventsListNew1, function (e) {
                return event.stylistID === e.id
            });
            if (events.length === 0) {
                $scope.eventsListNew1.push(event);
            }
        });

        $scope.stylists = ($.grep($scope.allDataList, function (e) { return e.name == name }))[0].stylists;

        for (let i = 0; i < $scope.eventsListNew1.length; i++) {
            var stylists = ($.grep($scope.stylists, function (e) { return e.stylistID == $scope.eventsListNew1[i].id }))[0];
            if (stylists) {
                $scope.StylistByCurrentdate.push(stylists);
            }
        }      
        return $scope.StylistByCurrentdate;
    };


    $scope.IsPending = function (workflow) {
        if (workflow == 11)
            return true;
        return false;
    };

    $scope.IsRejected = function (workflow, stylistid) {
       
        //$http({
        //    method: 'GET',
        //    url: serviceBase + 'GlamlyApi/User/GetstylistById',
        //    data: JSON.stringify(stylistid),
        //    headers: {
        //        'Authorization': authData.authenticationData.accessToken,
        //        'UserId': authData.authenticationData.Id
        //    }
        //})
        //   .success(function (data) {
        //       $('#processing').hide();
        //       if (data.responseCode !== 1) {
        //           $scope.errorMessage = data.ResponseMessage;
        //           $timeout(function () { alert($scope.errorMessage) }, 0);
        //           if (data.ResponseCode === 5) $window.location.href = serviceBase + '#/login';
        //       } else {
        //           $scope.ServiceModel.rejectedbystylist = data.responseData;
        //       }
        //   })
        //   .error(function (data) {
        //       $('#processing').hide();
        //       return null;
        //   });


        if (workflow == 41)
            return true;
        return false;
    };

    $scope.deleteBooking = function (booking) {

        if (confirm('Are you sure you want to cancel?')) {
            DeleteBooking(booking);
        } else {
            return;
        }
    };

    function DeleteBooking(booking) {
        $('#processing').show();
        var Bookingid = booking.bookingid;
        $http({
            method: 'DELETE',
            url: serviceBase + 'GlamlyApi/User/DeleteBooking',
            params: {
                id: Bookingid
            },
            headers: {
                'Authorization': authData.authenticationData.accessToken
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {
                modelDialogService.info("Booking has been deleted successfully");
                getbookings();
                //$scope.reset();
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

    $scope.SendDeleteBookingMail = function (booking) {

        DeleteBookingMail(booking);
    };

    function DeleteBookingMail(booking) {

        $('#processing').show();
        var Bookingid = booking.bookingid;
        $http({
            method: 'DELETE',
            url: serviceBase + 'GlamlyApi/User/SendDeleteBookingMail',
            params: {
                id: Bookingid
            },
            headers: {
                'Authorization': authData.authenticationData.accessToken
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {
                modelDialogService.info("Booking has been deleted successfully");
                getbookings();
                //$scope.reset();
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
  
    $scope.ApproveBooking = function (booking) {

        $('#processing').show();
        var Bookingid = booking.bookingid;

        var selectedstylist

        var range;
        for (var i = 0; i < booking.servicewithtypes.length; i++) {
            if (booking.servicewithtypes[i].stylistID != undefined)
                var selectedstylist = booking.servicewithtypes[i].stylistID;
        }

        // var selectedstylist = booking.servicewithtypes[0].stylistID == undefined ?booking.servicewithtypes[1].stylistID:undefined;
        if (selectedstylist == undefined) {
            modelDialogService.info("Please select the stylist before button click!");
            $('#processing').hide();
            return;
        }
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/ApprovedBooking',
            params: {
                bookingid: Bookingid,
                stylistid: selectedstylist
            },
            headers: {
                'Authorization': authData.authenticationData.accessToken
            }
        })
        .success(function (data) {
            $('#processing').hide();
            if (data.responseCode == 1) {
                modelDialogService.info("Booking is approve by admin successfully");
                //  $scope.message = $translate.use() == "en_US" ? 'Successfully Deleted.' : 'Raderats Framgångsrikt.';
                //  $scope.class = "alert alert-success";
                getbookings();
                // $scope.reset();
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


    $scope.togglebookings = function (e) {

        // e.preventDefault();

        var $this = $(this);


        if ($this.parent().next().hasClass('toggle-show')) {
            $this.parent().next().removeClass('toggle-show');
            $this.parent().next().slideUp(350);
            $(this).toggleClass('collapsed');
        } else {
            //$this.parent().parent().find('.toggle-content').removeClass('toggle-show');
            //$this.parent().parent().find('.toggle-content').slideUp(350);
            $this.parent().next().toggleClass('toggle-show');
            $this.parent().next().slideToggle(350);
            $(this).toggleClass('collapsed');
        }
    };

    $scope.closediv = function (closemode) {

        if (closemode == true) {
            $scope.closemode = false;
        }
        else {
            $scope.closemode = true;
        }

    }
    $scope.closediv1 = function (closemode1) {

        if (closemode1 == true) {
            $scope.closemode1 = false;
        }
        else {
            $scope.closemode1 = true;
        }

    }
    $scope.closediv2 = function (closemode2) {

        if (closemode2 == true) {
            $scope.closemode2 = false;
        }
        else {
            $scope.closemode2 = true;
        }

    }
    $scope.closediv3 = function (closemode3) {

        if (closemode3 == true) {
            $scope.closemode3 = false;
        }
        else {
            $scope.closemode3 = true;
        }

    }


    function getAvailDate() {
        $('#processing').show();
        // $scope.CurrentPage = pageNumber;
        $http({
            method: 'GET',
            url: serviceBase + 'GlamlyApi/User/GetAvailDatesbyStylist',
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
                    $scope.events = data.responseData;                  
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };
    getAvailDate();
    function createCalendarEvent(id, startDate, status) {
        let local = convertUtcToLocal(startDate)
        return {
            id: id,
            date: $filter('date')(local, "yyyy-MM-ddTHH:mm:ss"),
            status: status
        };
    }

    function convertUtcToLocal(date) {
        var stillUtc = moment.utc(date).toDate();
        return moment(stillUtc).local().format('YYYY-MM-DDTHH:mm');
    };

}]);