
var serviceBase = '';

var app = angular.module('AngularApp', ['ngRoute', 'LocalStorageModule', 'pascalprecht.translate', 'angularUtils.directives.dirPagination', 'ui.bootstrap', 'moment-picker', 'rzModule', 'ui.calendar', 'ngFileUpload' ]);

app.config(['$routeProvider', '$translateProvider', '$translatePartialLoaderProvider', function ($routeProvider, $translateProvider, $translatePartialLoaderProvider) {

    $routeProvider.caseInsensitiveMatch = true;

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "Views/Home/Login.html"
    });

    $routeProvider.when("/Customer", {
        controller: "CustomerController",
        templateUrl: "Views/Stylist/Customer.html"
    });

    $routeProvider.when("/CustomerProfile", {
        controller: "CustomerProfileController",
        templateUrl: "Views/Stylist/CustomerProfile.html"
    });

    $routeProvider.when("/CustomerProfile/:id", {
        controller: "CustomerProfileController",
        templateUrl: "Views/Stylist/CustomerProfile.html"
    });

    $routeProvider.when("/StylistHome", {
        controller: "DashboardController",
        templateUrl: "Views/Stylist/Dashboard.html"
    });

    $routeProvider.when("/addStylist", {
        controller: "AddStylistController",
        templateUrl: "Views/Stylist/AddStylist.html"
    });
    
    $routeProvider.when("/bookings", {
        controller: "BookingController",
        templateUrl: "Views/Stylist/Bookings.html"
    });

    $routeProvider.when("/services", {
        controller: "ServicesController",
        templateUrl: "Views/Stylist/Services.html"
    });

    $routeProvider.when("/profile", {
        controller: "ProfileController",
        templateUrl: "Views/Stylist/Profile.html"
    });

    $routeProvider.when("/profile/:id", {
        controller: "ProfileController",
        templateUrl: "Views/Stylist/Profile.html"
    });

    $routeProvider.when("/Schema", {
        controller: "SchemaController",
        templateUrl: "Views/Stylist/Schema.html"
    });

    $routeProvider.when("/FAQ", {
        controller: "FAQController",
        templateUrl: "Views/Stylist/FAQ.html"
    });

    $routeProvider.when("/AddStylistPage", {
        controller: "AddStylistPageController",
        templateUrl: "Views/Stylist/AddStylistPage.html"
    });

    $routeProvider.when("/ForgetPassword", {
        controller: "ForgetPasswordController",
        templateUrl: "Views/Home/ForgetPassword.html"
    });
    $routeProvider.when("/ResetPassword", {
        controller: "ResetPasswordController",
        templateUrl: "Views/Home/ResetPassword.html"
    });
    $routeProvider.when("/ChangePassword", {
        controller: "CompanyChangePasswordController",
        templateUrl: "Views/Company/ChangePassword.html"
    });

    $routeProvider.when("/logout", {
        controller: "logOutController",
        templateUrl: "Views/Home/LogOut.html"
    });

    $routeProvider.when("/PaymentMethods", {
        controller: "PaymentMethodsController",
        templateUrl: "Views/Payment/PaymentMethods.html"
    });

    $routeProvider.when("/Checkout", {
        controller: "CheckoutController",
        templateUrl: "Views/Payment/Checkout.html"
    });

    $routeProvider.when("/Success", {
        controller: "SuccessController",
        templateUrl: "Views/Payment/Success.html"
    });

    $routeProvider.when("/Cancel", {
        controller: "CancelController",
        templateUrl: "Views/Payment/Cancel.html"
    });

   

    $routeProvider.otherwise({ redirectTo: "/login" });

    //Translation Code
    //http://www.c-sharpcorner.com/UploadFile/e5276f/multilingual-concept-for-angularjs-application/
    $translatePartialLoaderProvider.addPart('Resources');
    $translateProvider.useLoader('$translatePartialLoader',
        {
            urlTemplate: "Scripts/{part}/{lang}.json"
        });
    $translateProvider.preferredLanguage('en_US');

}])
    .config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push(function ($q, $rootScope, $window, $location, localStorageService) {
            return {
                request: function (config) {
                    return config;
                },
                requestError: function (rejection) {
                    return $q.reject(rejection);
                },
                response: function (response) {
                    if (response.status == "401") {
                        $window.sessionStorage["TokenInfo"] = null;
                        localStorageService.remove('AppPreferences');
                        $location.path('/login');
                        $window.location.reload();
                    }
                    //the same response/modified/or a new one need to be returned.
                    return response;
                },
                responseError: function (rejection) {
                    if (rejection.status == "401") {
                        $window.sessionStorage["TokenInfo"] = null;
                        localStorageService.remove('AppPreferences');
                        $location.path('/login');
                        $window.location.reload();
                    }
                    return $q.reject(rejection);
                }
            };
        });
    }]);

//Translation Code
app.run(function ($rootScope, $translate) {
    $rootScope.$on('$translatePartialLoaderStructureChanged', function () {
        $translate.refresh();
    });
});

app.controller('AppController', ['$scope', '$http', '$translate', 'localStorageService', '$location', 'authData', '$filter', '$window',
    function ($scope, $http, $translate, localStorageService, $location, authData, $filter, $window) {
        $scope.Languages = [{ code: 'en_US', name: 'English' }, { code: 'sv_SE', name: 'Swedish' }];

        $scope.AppPreferences = {};
        $scope.AppPreferences.Selectedlanguage = $scope.Languages[0];
        

        $scope.$on('$routeChangeSuccess', function () {
            $scope.activePath = $location.path();
        });

        $scope.onLanguageChange = function (key) {
            $translate.use(key);
            $translate.refresh();
        };

        $scope.IsAdminUser = function () {
            if (authData.authenticationData.userType == 1 ||
                authData.authenticationData.userType == 2)
                return true;
            return false;
        };

        $scope.IsCompanyUser = function () {
            if (authData.authenticationData.userType == 11 ||
                authData.authenticationData.userType == 12)
                return true;
            return false;
        };
        $scope.IsPersonUser = function () {
            if (authData.authenticationData.userType == 21)
                return true;
            return false;
        };

        $scope.IsReportInOpenStatus = function (reportStatus) {
            if (reportStatus == 0)
                return true;
            return false;
        }

        $scope.IsReportInWorkFlowWithPerson = function (reportStatus) {
            if (reportStatus == 11)
                return true;
            return false;
        }

        $scope.IsReportInWorkFlowWithCompany = function (reportStatus) {
            if (reportStatus == 12)
                return true;
            return false;
        }

        $scope.IsReportInWorkFlowWithAdmin = function (reportStatus) {
            if (reportStatus == 13)
                return true;
            return false;
        }

        $scope.GetUserId = function () {
                return authData.authenticationData.Id;
        };

        $scope.TranslateText = function(text) {
            var $translate = $filter('translate');
            return $translate(text != null && text != undefined && text.length > 0 ? text : '');
        }

    }]);

var compareTo = function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
};

app.directive("compareTo", compareTo);

var ngEnter = function () {
    
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter, { 'event': event });
                });

                event.preventDefault();
            }
        });
    };
};

app.directive("ngFileSelect", function (fileReader, $timeout) {
    alert("sdfsd44");
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

app.directive("ngEnter", ngEnter);