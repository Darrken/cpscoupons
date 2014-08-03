app.controller('adminMenuCtrl', function ($scope, programsApiService, alertService, adminService) {
	adminService.adminCheck('/adminmenu');
	
	$scope.alerter = alertService;

	$scope.getProgramList = function () {
		programsApiService.getByCommand('getProgramList')
			.then(function (data) {
				$scope.programs = data;
			})
			.catch(function () {
				$scope.alerter.addAlert('danger', 'Unable to get Program data.');
			});
	};

	$scope.getProgramList();
});

app.controller('programAdminCtrl', function ($scope, $location, $anchorScroll, $timeout, programsApiService, alertService, adminService) {
	adminService.adminCheck('/adminprogram');
	
	$scope.alerter = alertService;
	$scope.Emails = [];
	$scope.ParticipatingMalls = [];
	$scope.previewMode = false;
	$scope.previewMalls = [];

	$scope.program = {
		ParticipatingMalls: [],
		CouponWordCount: null,
		Description: null,
		Disclaimer: null,
		DeadlineCoupon: null,
		DeadlineInMall: null,
		Retailers: [],
		Fields: [],
		Name: null,
	};

	$scope.getMallList = function () {
		programsApiService.getByCommand('getMallList')
			.then(function (data) {
				$scope.malls = data;
			})
			.catch(function () {
				$scope.alerter.addAlert('danger', 'Unable to get Center data.');
			});
	};

	$scope.getMallList();

	$scope.toggleCenter = function (id) {
		var idx = $scope.ParticipatingMalls.indexOf(id);

		if (idx > -1) {
			$scope.ParticipatingMalls.splice(idx, 1);
		}
		else {
			$scope.ParticipatingMalls.push(id);
		}

		$scope.previewMalls = [];
		_.forEach($scope.ParticipatingMalls, function (participatingMall) {
			var mall = _.find($scope.malls, { 'Id': participatingMall });
			$scope.previewMalls.push(mall);
		});
	};

	$scope.addField = function () {
		$scope.program.Fields.push({ Name: null });
	};

	$scope.removeEmptyField = function (fieldName, index) {
		if (!fieldName || fieldName.length === 0) {
			$scope.program.Fields.splice(index, 1);
		}
	};

	$scope.togglePreviewMode = function () {
		$scope.previewMode = !$scope.previewMode;
		$location.hash('top');
		$anchorScroll();
	};

	$scope.$on('$locationChangeStart', function (ev) {
		ev.preventDefault();
	});

	$scope.saveProgram = function () {
		var emails = $scope.program.Emails.split('\n');
		$scope.program.Retailers = [];
		_.forEach(emails, function (email) {
			$scope.program.Retailers.push({ Email: email });
		});

		$scope.program.ParticipatingMalls = [];
		_.forEach($scope.ParticipatingMalls, function (id) {
			$scope.program.ParticipatingMalls.push({ Id: id });
		});

		programsApiService.saveByCommand('createProgram', $scope.program)
			.then(function (data) {
				if (data.status === 200) {
					//TODO: figure out why this doesn't always work
					$scope.alerter.addAlert('success', 'Program was successfully saved.');
				}
			})
			.catch(function (data) {
				$scope.alerter.addAlert('danger', 'Unable to save Program data.');
			})
			.finally(function () {
				$location.hash('top');
				$anchorScroll();
			});
	};
});

app.controller('programSignupCtrl', function ($scope, $routeParams, programsApiService, alertService, adminService) {
	adminService.adminCheck('/signup/' + $routeParams.urlguid);

	$scope.alerter = alertService;
	$scope.agreed = false;

	$scope.getProgramByGuid = function () {
		programsApiService.getProgramByRetailer($routeParams.urlguid)
			.then(function (data) {
				$scope.program = data;
				$scope.retailer = _.find(data.Retailers, { 'UrlGuid': $routeParams.urlguid.toLowerCase() });
				$scope.retailer.FieldValues = [];
				$scope.retailer.SelectedMalls = [];
				_.forEach($scope.program.Fields, function (field) {
					var fieldValue = { Id: field.Id, Name: field.Name, Value: null, RetailerId: $scope.retailer.Id };
					$scope.retailer.FieldValues.push(fieldValue);
				});
			})
			.catch(function () {
				$scope.alerter.addAlert('danger', 'Unable to get Program data. Please contact your MMM rep.');
			});
	};
	$scope.getProgramByGuid();

	$scope.toggleCenter = function (id) {
		var idx = $scope.retailer.SelectedMalls.indexOf(id);

		if (idx > -1) {
			$scope.retailer.SelectedMalls.splice(idx, 1);
		}
		else {
			$scope.retailer.SelectedMalls.push(id);
		}
	};

	$scope.submitForm = function () {
		if ($scope.retailer.SelectedMalls.length < 1) {
			$scope.alerter.addAlert('danger', 'Please select at least one shopping center.');
			return;
		}

		programsApiService.saveByCommand('signUp', $scope.retailer)
			.then(function (data) {
				if (data.status === 200) {
					//TODO: figure out why this doesn't always work
					$scope.alerter.addAlert('success', 'Program was successfully saved.');
				}
			})
			.catch(function (data) {
				$scope.alerter.addAlert('danger', 'Unable to save Program data.');
			});
	};
});

app.controller('retailersByCenterCtrl', function ($scope, $routeParams, programsApiService, alertService, fileService, adminService) {
	adminService.adminCheck('/retailersbycenter');
	
	$scope.alerter = alertService;
	$scope.malls = [];
	$scope.selectedMall = {};
	$scope.retailers = [];
	//[{ email: 'test@test.com', hasSignedUp: false, storeName: 'test store', contactName: 'john smith', repName: 'john rep', phone: '123-456-7890' },
	//{ email: 'anothertest@test.com', hasSignedUp: true, storeName: 'kiosk?', contactName: 'jane smith', repName: 'jane rep', phone: '789-456-1230' }];

	$scope.getMallList = function () {
		programsApiService.getByCommand('getMallsWithSignups')
			.then(function (data) {
				$scope.malls = data;
			})
			.catch(function () {
				$scope.alerter.addAlert('danger', 'Unable to get Center data.');
			});
	};

	$scope.getRetailersByMall = function () {
		programsApiService.getRetailersByMall($scope.selectedMall.Id)
			.then(function (data) {
				$scope.retailers = data;
			})
			.catch(function () {
				$scope.alerter.addAlert('danger', 'Unable to get report data.');
			});
	};

	$scope.exportReport = function () {
		var exportColumns = ['email', 'hasSignedUp', 'storeName', 'contactName', 'repName', 'phone'];
		fileService.createCsvFile(exportColumns, $scope.retailers, 'retailers_by_center');
	};

	$scope.malls = $scope.getMallList();
});

app.controller('retailersByProgramCtrl', function ($scope, $routeParams, programsApiService, alertService, fileService, adminService) {
	adminService.adminCheck('/retailersbyprogram');

	$scope.alerter = alertService;
	$scope.programs = [];
	$scope.selectedProgram = {};
	$scope.retailers = [];
	//[{ email: 'test@test.com', hasSignedUp: false, storeName: 'test store', contactName: 'john smith', repName: 'john rep', phone: '123-456-7890' },
	//{ email: 'anothertest@test.com', hasSignedUp: true, storeName: 'kiosk?', contactName: 'jane smith', repName: 'jane rep', phone: '789-456-1230' }];

	$scope.getProgramList = function () {
		programsApiService.getByCommand('getProgramList')
			.then(function (data) {
				$scope.programs = data;
			})
			.catch(function () {
				$scope.alerter.addAlert('danger', 'Unable to get Program data.');
			});
	};

	$scope.getRetailersByProgram = function () {
		programsApiService.getRetailersByProgram($scope.selectedProgram.Id)
			.then(function (data) {
				$scope.retailers = data;
			})
			.catch(function () {
				$scope.alerter.addAlert('danger', 'Unable to get report data.');
			});
	};

	$scope.exportReport = function () {
		var exportColumns = ['email', 'hasSignedUp', 'storeName', 'contactName', 'repName', 'phone'];
		fileService.createCsvFile(exportColumns, $scope.retailers, 'retailers_by_center');
	};

	$scope.getProgramList();
});

app.controller('adminLoginCtrl', function ($scope, $location, alertService, adminService) {
	$scope.alerter = alertService;
	$scope.pwd = '';

	$scope.login = function () {
		var wasSuccessful = adminService.loginAdmin($scope.pwd);

		if (wasSuccessful) {
			$scope.alerter.clearAlerts();
		} else {
			$scope.alerter.addAlert('danger', 'Invalid password.');
		}
	};
});