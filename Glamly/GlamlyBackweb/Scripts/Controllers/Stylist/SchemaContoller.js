app.controller("SchemaController", ['$scope', '$http', 'authData', '$filter', '$timeout', 'uiCalendarConfig', '$window', 'ModelDialogService', function ($scope, $http, authData, $filter, $timeout, uiCalendarConfig, $window, modelDialogService) {
    $scope.calendarEvents = [];
    $scope.eventSources = [$scope.calendarEvents];
    $scope.availEvents = [];
    var events = [];
    $scope.calendarEventsForPage = [];
    $scope.CalendarData = {};
    $scope.seleteddates = {
        availEvents: [],
        selectstylistID: "",
        UserName: ""
    };
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

                    for (let i = 0; i < $scope.events.length; i++) {
                        $scope.events[i].datalist = [];
                        for (let y = 0; y < $scope.events[i].date.length; y++) {
                            let item = createCalendarEvent($scope.events[i].stylistId, $scope.events[i].date[y].date,  $scope.events[i].date[y].status);
                            $scope.calendarEvents.push(item);
                        }
                        $scope.events[i].datalist.push($scope.calendarEvents);
                        $scope.calendarEvents = [];
                    }
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };
    getAvailDate();


    function createCalendarEvent(id,  startDate,  status) {
        let local = convertUtcToLocal(startDate)
        return {
            id: id,          
            date: $filter('date')(local, "yyyy-MM-ddTHH:mm:ss"),          
            status: status
        };
    }

    function createavaildates(stylistid, startDate, status) {
        let local = convertUtcToLocal(startDate)
        return {
            stylistid: stylistid,
            date: startDate,
            status: status
        };
    }

    function convertUtcToLocal(date) {
        var stillUtc = moment.utc(date).toDate();
        return moment(stillUtc).local().format('YYYY-MM-DDTHH:mm');
    };

    $scope.uiConfig = {

        calendar: {
            height: 100,
            editable: true,
            header: {
                left: '',
                center: 'prev title next',
                right: ''
            },
            firstDay:1,
            eventRender: function (event, element) {
                var dataToFind = moment(event.date).format('YYYY-MM-DD');
                $("#" + event.id + " td[data-date='" + dataToFind + "']").addClass('fc-content1');
            },
            dayClick: function (date, allDay, jsEvent, view) {
                debugger;
                // alert($(this).closest('.whitebgDiv').parent().find('input[name=idNote]').val());
                var stylistid = $(this).closest('.whitebgDiv').parent().find('input[name=stylistId]').val();
                var dataToFind = moment(date).format('YYYY-MM-DD');
                var IsMatched = false;
                var UserName = $(this).closest('.whitebgDiv').parent().find('input[name=userName]').val();
                for (let i = 0; i < $scope.events.length; i++) {
                    if ($scope.events[i].stylistId == stylistid) {
                        var index = -1;
                        var val = moment(date).format('YYYY-MM-DD');
                        var filteredObj = $scope.events[i].datalist[0].find(function (item, k) {
                            if (moment(item.date).format('YYYY-MM-DD') === val) {                               
                                index = k;
                                return k;
                            }
                        });
                        var Count = ($.grep($scope.events[i].datalist[0], function (e) { return moment(e.date).format('YYYY-MM-DD') == moment(date).format('YYYY-MM-DD') }));
                        if (Count == 0) {
                            $scope.events[i].datalist[0].push({ 'id': parseInt(stylistid), 'date': moment(date).format('YYYY-MM-DD') + 'T05:30:00', 'status': "add" })                          
                        } else {
                            if (index != -1) {
                                $scope.events[i].datalist[0].splice(index, 1);                                
                            }
                        }
                    }
                }
                //var dataToFind = moment(date).format('YYYY-MM-DD');
                if ($("#" + stylistid + " td[data-date='" + dataToFind + "']").hasClass('fc-content1')) {
                    $("#" + stylistid + " td[data-date='" + dataToFind + "']").removeClass('fc-content1');

                    let item = createavaildates(stylistid, dataToFind, "delete");
                    if ($scope.seleteddates.availEvents == undefined) {
                        $scope.seleteddates.availEvents = [];
                        $scope.seleteddates.availEvents.push(item);
                        $scope.seleteddates.selectstylistID = stylistid;
                        $scope.seleteddates.UserName = UserName;
                    }
                    else {
                        $scope.seleteddates.availEvents.push(item);
                        $scope.seleteddates.selectstylistID = stylistid;
                        $scope.seleteddates.UserName = UserName;
                    }
                    // saveCalendarDates1(stylistid, dataToFind, UserName, "delete");
                }
                else {
                    $("#" + stylistid + " td[data-date='" + dataToFind + "']").addClass('fc-content1');
                    let item = createavaildates(stylistid, dataToFind, "add");
                    if ($scope.seleteddates.availEvents == undefined) {
                        $scope.seleteddates.availEvents = [];
                        $scope.seleteddates.availEvents.push(item);
                        $scope.seleteddates.selectstylistID = stylistid;
                        $scope.seleteddates.UserName = UserName;
                    }
                    else {
                        $scope.seleteddates.availEvents.push(item);
                        $scope.seleteddates.selectstylistID = stylistid;
                        $scope.seleteddates.UserName = UserName;
                    }
                    //saveCalendarDates1(stylistid, dataToFind, UserName, "add");
                }
            }
        }
    };


    $scope.saveCalendarDates = function (stylistid,username) {       
        $('#processing').show();      
        $scope.CalendarData.StylistId = stylistid;
        var stylists;       
        var stylists = ($.grep($scope.seleteddates.availEvents, function (e) { return e.stylistid == stylistid }));
        $scope.CalendarData.date = stylists;
        $scope.CalendarData.UserName = username;
        $http({
            method: 'POST',
            url: serviceBase + 'GlamlyApi/User/SaveAvailDatesbyStylist',
            data: JSON.stringify($scope.CalendarData),
            headers: {
                'Authorization': authData.authenticationData.accessToken,
                'UserId': authData.authenticationData.Id

            }
        })
            .success(function (data) {
                $('#processing').hide();
                if (data.responseCode == 1) {
                    $scope.seleteddates.availEvents = "";
                    $scope.seleteddates = [];
                    $scope.CalendarData = {};
                    getAvailDate();
                    modelDialogService.info("Schemat har uppdaterats framgångsrikt");
                }
                else {
                    modelDialogService.error("error");
                }
            })
            .error(function (data) {
                $('#processing').hide();
                return null;
            });
    };


}]);





