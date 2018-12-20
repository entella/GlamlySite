
app.service('ModelDialogService', ['$uibModal', '$q',
    function ($uibModal, $q) {
        var deferred;

        this.error = function (msg) {
            //$scope.temp = type;
            deferred = $q.defer();
            genericModalDialog = $uibModal.open({
                animation: true,
                size: 'sm',
                templateUrl: 'Views\\Shared\\Dialogs\\ErrorDialog.html',
                controller: genericModalDialogCtrl,
                //size: size,
                resolve: {
                    msg: function () {
                        return msg;
                    }
                },
            });

            genericModalDialog.result.then(function (response) {
                deferred.resolve(response);
            });

            return deferred.promise;
        };

        this.info = function (msg) {
            //$scope.temp = type;
            deferred = $q.defer();
            genericModalDialog = $uibModal.open({
                animation: true,
                size: 'sm',
                templateUrl: 'Views\\Shared\\Dialogs\\SuccessDialog.html',
                controller: genericModalDialogCtrl,
                //size: size,
                resolve: {
                    msg: function () {
                        return msg;
                    }
                },
            });

            genericModalDialog.result.then(function (response) {
                deferred.resolve(response);
            });

            return deferred.promise;
        };

        this.warning = function (msg) {
            //$scope.temp = type;
            deferred = $q.defer();
            genericModalDialog = $uibModal.open({
                animation: true,
                size: 'sm',
                templateUrl: 'Views\\Shared\\Dialogs\\WarningDialog.html',
                controller: genericModalDialogCtrl,
                //size: size,
                resolve: {
                    msg: function () {
                        return msg;
                    }
                },
            });

            genericModalDialog.result.then(function (response) {
                deferred.resolve(response);
            });

            return deferred.promise;
        };

        this.confirm = function (msg) {
            //$scope.temp = type;
            deferred = $q.defer();
            genericModalDialog = $uibModal.open({
                animation: true,
                size: 'sm',
                templateUrl: 'Views\\Shared\\Dialogs\\ConfirmDialog.html',
                controller: genericModalDialogCtrl,
                //size: size,
                resolve: {
                    msg: function () {
                        return msg;
                    }
                },
            });

            genericModalDialog.result.then(function (response) {
                deferred.resolve(response);
            });

            return deferred.promise;
        };

        function genericModalDialogCtrl($scope, msg) {
            $scope.msg = msg;
            $scope.dismissModal = function () {
                genericModalDialog.close(false);
                genericModalDialog.dismiss();
            };
            $scope.yes = function () {
                genericModalDialog.close(true);
            };
        }

        this.employmentConfirmDialog = function () {
            deferred = $q.defer();
            employmentAcceptRejectDialog = $uibModal.open({
                animation: true,
                size: 'sm',
                templateUrl: 'Views\\Shared\\Dialogs\\EmploymentConfirmDialog.html',
                controller: employmentAcceptRejectDialogCtrl,
                //size: size,
                //resolve: {
                //    msg: function () {
                //        return msg;
                //    }
                //},
            });

            employmentAcceptRejectDialog.result.then(function (response) {
                deferred.resolve(response);
            });

            return deferred.promise;
        };

        function employmentAcceptRejectDialogCtrl($scope, $window) {
            $scope.dismissModal = function () {
                employmentAcceptRejectDialog.close(false);
                employmentAcceptRejectDialog.dismiss();
            };

            $scope.Accept = function () {
                employmentAcceptRejectDialog.close(true);
                employmentAcceptRejectDialog.dismiss();
            };

            $scope.TermsAndConditionLinkClick = function () {
                var url = serviceBase + 'policies/employmentpolicy.html';
                $window.open(url);
            }
        }
    }
]);
