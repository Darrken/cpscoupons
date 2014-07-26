var adminControllers = angular.module('adminControllers', []);

adminControllers.controller('menuCtrl', ['$scope', '$http',
  function ($scope, $http) {
  	
  }]);

adminControllers.controller('programCtrl', ['$scope', '$routeParams',
  function ($scope, $routeParams) {
  	$scope.programId = $routeParams.programId;
  }]);