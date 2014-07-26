var app = angular.module('app', ['ngRoute', 'ui.bootstrap'])

.config(['$routeProvider',
  function($routeProvider, $locationProvider) {
  	$routeProvider.
	  when('/admin', {
	  	templateUrl: 'views/admin.html',
	  	controller: 'adminMenuCtrl'
	  }).
	  when('/adminprogram/:programId', {
	  	templateUrl: 'views/program.html',
	  	controller: 'programCtrl'
	  }).
	  otherwise({
	  	redirectTo: '/'
	  });
	  $locationProvider.html5Mode(true);
  }]);