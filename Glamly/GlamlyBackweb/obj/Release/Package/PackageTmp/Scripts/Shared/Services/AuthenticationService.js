app.service('AuthenticationService', ['$http', '$q', '$window', 'localStorageService', 'authData',
    function ($http, $q, $window, localStorageService, authData) {
        var tokenInfo;

        this.setTokenInfo = function (data) {
            tokenInfo = data;
            $window.sessionStorage["TokenInfo"] = JSON.stringify(tokenInfo);
        }

        this.getTokenInfo = function () {
            return tokenInfo;
        }

        this.removeToken = function () {
            tokenInfo = null;
            $window.sessionStorage["TokenInfo"] = null;
            localStorageService.remove('AppPreferences');
            authData.authenticationData.IsAuthenticated = false;
            authData.authenticationData.userName = "";
            authData.authenticationData.userEmail = "";
            authData.authenticationData.userType = "";
            authData.authenticationData.userMenu = "";
            authData.authenticationData.accessToken = "";
            authData.authenticationData.activeItem = "";
            authData.authenticationData.userData = {};
            //$window.location.reload();
        }

        this.init = function () {
            if ($window.sessionStorage["TokenInfo"]) {
                tokenInfo = JSON.parse($window.sessionStorage["TokenInfo"]);
            }
        }

        this.setHeader = function (http) {
            delete http.defaults.headers.common['X-Requested-With'];
            if ((tokenInfo != undefined) && (tokenInfo.accessToken != undefined) && (tokenInfo.accessToken != null) && (tokenInfo.accessToken != "")) {
                http.defaults.headers.common['Authorization'] = 'Bearer ' + tokenInfo.accessToken;
                http.defaults.headers.common['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
            }
        }

        this.init();
    }
]);
