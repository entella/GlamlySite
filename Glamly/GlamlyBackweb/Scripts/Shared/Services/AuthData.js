app.factory('authData', ['$window', '$location', function ($window, $location) {
    var authDataFactory = {};
    var _authentication = {
        IsAuthenticated: false,
        userName: "",
        userEmail:"",
        userMenu: "",
        userType: "",
        accessToken: "",
        activeItem :1,
        userData: {}
    };
    if ($window.sessionStorage["TokenInfo"]) {
        var tokenInfo = JSON.parse($window.sessionStorage["TokenInfo"]);
        if (tokenInfo != null) {
            _authentication.IsAuthenticated = true;
            _authentication.userName = tokenInfo.userName;
            _authentication.userEmail = tokenInfo.userEmail;
        //    _authentication.userMenu = tokenInfo.userMenu.Data;
            _authentication.userType = tokenInfo.userType;
            _authentication.Id = tokenInfo.Id;
            _authentication.activeItem = 1;
            _authentication.accessToken = 'Bearer ' + tokenInfo.accessToken;
            _authentication.userData = { Id: _authentication.Id, EmailId: _authentication.userEmail };
        }
    }

    authDataFactory.authenticationData = _authentication;

    return authDataFactory;
}]);