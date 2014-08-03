var app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'ngSanitize'])
	.config(function($routeProvider, $locationProvider) {
		$routeProvider.
			when('/adminmenu', {
				templateUrl: '/views/admin.html',
				controller: 'adminMenuCtrl'
			}).
			when('/adminlogin', {
				templateUrl: '/views/adminlogin.html',
				controller: 'adminLoginCtrl'
			}).
			when('/adminprogram/:programId', {
				templateUrl: '/views/programedit.html',
				controller: 'programAdminCtrl'
			}).
			when('/adminprogram', {
				templateUrl: '/views/programcreate.html',
				controller: 'programAdminCtrl'
			}).
			when('/signup/:urlguid', {
				templateUrl: '/views/programsignup.html',
				controller: 'programSignupCtrl'
			}).
			when('/retailersbycenter', {
				templateUrl: '/views/retailersbycenter.html',
				controller: 'retailersByCenterCtrl'
			}).
			when('/retailersbyprogram', {
				templateUrl: '/views/retailersbyprogram.html',
				controller: 'retailersByProgramCtrl'
			}).
			otherwise({
				redirectTo: '/'
			});
		$locationProvider.html5Mode(true);
	});