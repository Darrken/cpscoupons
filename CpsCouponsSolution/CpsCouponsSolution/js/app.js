var app = angular.module('app', ['ngRoute', 'ui.bootstrap'])
	.config(function($routeProvider, $locationProvider) {
		$routeProvider.
			when('/admin', {
				templateUrl: '/views/admin.html',
				controller: 'adminMenuCtrl'
			}).
			when('/adminprogram/:programId', {
				templateUrl: '/views/programedit.html',
				controller: 'programCtrl'
			}).
			when('/adminprogram', {
				templateUrl: '/views/programcreate.html',
				controller: 'programCtrl'
			}).
			when('/signup/:guid', {
				templateUrl: '/views/program.html',
				controller: 'programCtrl'
			}).
			otherwise({
				redirectTo: '/'
			});
		$locationProvider.html5Mode(true);
	});