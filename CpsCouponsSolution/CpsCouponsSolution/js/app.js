var app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'ngSanitize'])
	.config(function($routeProvider, $locationProvider) {
		$routeProvider.
			when('/adminmenu', {
				templateUrl: '/views/adminmenu.html',
				controller: 'adminMenuCtrl'
			}).
			when('/adminlogin', {
				templateUrl: '/views/adminlogin.html',
				controller: 'adminLoginCtrl'
			}).
			when('/programedit/:programId', {
				templateUrl: '/views/adminprogram.html',
				controller: 'programCtrl'
			}).
			when('/programcreate', {
				templateUrl: '/views/adminprogram.html',
				controller: 'programCtrl'
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