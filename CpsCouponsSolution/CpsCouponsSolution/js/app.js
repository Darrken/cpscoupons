var app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'ngSanitize'])
	.config(function($routeProvider, $locationProvider) {
		$routeProvider.
			when('/adminmenu', {
				templateUrl: '/views/admin.html',
				controller: 'adminMenuCtrl'
			}).
			when('/adminprogram/:programId', {
				templateUrl: '/views/programedit.html',
				controller: 'programCtrl'
			}).
			when('/adminprogram', {
				templateUrl: '/views/programcreate.html',
				controller: 'programAdminCtrl'
			}).
			when('/signup/:urlguid', {
				templateUrl: '/views/programsignup.html',
				controller: 'programSignupCtrl'
			}).
			otherwise({
				redirectTo: '/'
			});
		$locationProvider.html5Mode(true);
	});