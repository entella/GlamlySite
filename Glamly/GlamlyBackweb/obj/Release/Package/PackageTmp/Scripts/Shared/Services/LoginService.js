
app.service('LoginService', ['$http', '$q', 'AuthenticationService', 'authData', '$window', 'localStorageService', 
    function ($http, $q, authenticationService, authData, $window, localStorageService) {
        var userInfo;
        var loginServiceURL = serviceBase + 'AuthenticationToken';
        var deviceInfo = [];
        var deferred;
       
        this.login = function (userName, password) {
            $('#processing').show();
           // blockUI();
            deferred = $q.defer();
            var data = "grant_type=password&username=" + userName + "&password=" + password;
            $http.post(loginServiceURL, data, {
                headers:
                { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).success(function (response) {
                
                var o = response;              
                userInfo = {
                    accessToken: response.access_token,
                    Id: response.Id,
                    userName: response.userName,
                    userEmail: response.userEmail,
                    userType: response.userType,                  
                    userData: { Id: response.Id, EmailId: response.userEmail}
                };
                authenticationService.setTokenInfo(userInfo);
                authData.authenticationData.IsAuthenticated = true;
                authData.authenticationData.Id = response.Id;
                authData.authenticationData.userName = response.userName;
                authData.authenticationData.userEmail = response.userEmail;
                authData.authenticationData.userType = response.userType;
                authData.authenticationData.activeItem = 1;
             //   authData.authenticationData.userMenu = JSON.parse(response.userMenu).Data;
                authData.authenticationData.accessToken = 'Bearer ' + userInfo.accessToken;
                authData.authenticationData.userData.Id = response.Id;
                authData.authenticationData.userData.EmailId = response.userEmail;
                deferred.resolve(response);
                $('#processing').hide();
            })
                .error(function (err, status) {
                    authData.authenticationData.IsAuthenticated = false;
                    authData.authenticationData.userName = "";
                    authData.authenticationData.userEmail = "";
                    deferred.resolve(undefined);
                    $('#processing').hide();
                });

            return deferred.promise;
        }

        this.logOut = function () {
            authenticationService.removeToken();
            authData.authenticationData.IsAuthenticated = false;
            authData.authenticationData.Id = "";
            authData.authenticationData.userName = "";
            authData.authenticationData.userEmail = "";
          

            //authData.authenticationData.userMenu = [
            //    { Id: 1, Name: "MENU.HOME.HOME", Url: "home", ParentId: 0 },
            //    { Id: 2, Name: "MENU.HOME.COMPANY", Url: "company", ParentId: 0 },
            //    { Id: 3, Name: "MENU.HOME.PERSON", Url: "person", ParentId: 0 },
            //    { Id: 4, Name: "MENU.HOME.ABOUT_US", Url: "aboutus", ParentId: 0 },
            //    { Id: 5, Name: "MENU.HOME.REGISTER", Url: "register", ParentId: 0 },
            //    { Id: 6, Name: "MENU.HOME.LOG_IN", Url: "Login", ParentId: 0 }
            //];
        }

    }
]);
