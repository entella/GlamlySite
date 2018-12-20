
app.controller('indexController', ['$scope', '$http', '$location', 'authData', 'LoginService', '$window', function ($scope, $http, $location, authData, loginService, $window) {

    $scope.authentication = authData.authenticationData;
    // $scope.states = {};
    // $scope.states.activeItem = 1;
    $scope.authentication.activeItem = 1;

    //  if ($scope.authentication == undefined || $scope.authentication.userMenu == undefined || $scope.authentication.userMenu == '')
    $scope.userMenu = [

            { Id: 1, Name: "Bokningar", Url: "bookings", ParentId: 0 },
            { Id: 2, Name: "Lägg till", Url: "AddStylist", ParentId: 0 },
            { Id: 3, Name: "Stylister", Url: "StylistHome", ParentId: 0 },
            { Id: 4, Name: "Kunder", Url: "Customer", ParentId: 0 },
            { Id: 5, Name: "Tjänster", Url: "services", ParentId: 0 },
            { Id: 6, Name: "Schema", Url: "Schema", ParentId: 0 },
           { Id: 7, Name: "FAQ", Url: "FAQ", ParentId: 0 },
            { Id: 8, Name: "StylisterPage", Url: "AddStylistPage", ParentId: 0 },
            { Id:9, Name: "Logga ut", Url: "logout", ParentId: 0 }
    ];


}]);
