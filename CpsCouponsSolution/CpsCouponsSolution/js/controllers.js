app.controller('adminMenuCtrl', function ($scope, programsApiService) {
	$scope.alerts = [];

	$scope.getProgramList = function () {
		programsApiService.getByCommand('getProgramList')
			.then(function (data) {
				$scope.programs = data;
			})
			.catch(function () {
				addAlert('danger', 'Unable to get Program data.');
			});
	};

	function addAlert(errorType, message) {
		$scope.alerts.push({ type: errorType, msg: message });
	};

	$scope.closeAlert = function (index) {
		$scope.alerts.splice(index, 1);
	};

	$scope.getProgramList();
});


app.controller('programAdminCtrl', function ($scope, $location, $anchorScroll, programsApiService) {
	$scope.alerts = [];
	$scope.Emails = [];
	$scope.ParticipatingMalls = [];

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

	$scope.getMallList = function() {
		programsApiService.getByCommand('getMallList')
			.then(function(data) {
				$scope.malls = data;
			})
			.catch(function() {
				addAlert('danger', 'Unable to get Center data.');
			});
	};

	$scope.getMallList();

	function addAlert(errorType, message) {
		$scope.alerts.push({ type: errorType, msg: message });
	};

	$scope.closeAlert = function(index) {
		$scope.alerts.splice(index, 1);
	};

	$scope.toggleCenter = function(id) {
		var idx = $scope.ParticipatingMalls.indexOf(id);

		if (idx > -1) {
			$scope.ParticipatingMalls.splice(idx, 1);
		}
		else {
			$scope.ParticipatingMalls.push(id);
		}
	};

	$scope.addField = function() {
		$scope.program.Fields.push({ Name: null });
	};

	$scope.removeEmptyField = function(fieldName, index) {
		if (!fieldName || fieldName.length === 0) {
			$scope.program.Fields.splice(index, 1);
		}
	}

	$scope.saveProgram = function() {
		var emails = $scope.program.Emails.split('\n');

		_.forEach(emails, function(email) {
			$scope.program.Retailers.push({ Email: email });
		});

		_.forEach($scope.ParticipatingMalls, function (id) {
			$scope.program.ParticipatingMalls.push({ Id: id });
		});

		programsApiService.saveByCommand('createProgram', $scope.program)
			.then(function(data) {
				if (data.status === 200) {
					//TODO: figure out why this doesn't always work
					addAlert('success', 'Program was successfully saved.');
				}
			})
			.catch(function(data) {
				addAlert('danger', 'Unable to save Program data.');
			})
			.finally(function() {
				$location.hash('top');
				$anchorScroll();
			});
	};
});


app.controller('programSignupCtrl', function ($scope, $routeParams, programsApiService) {
	$scope.alerts = [];
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
				addAlert('danger', 'Unable to get Program data. Please contact your MMM rep.');
			});
	};
	$scope.getProgramByGuid();
	
	function addAlert(errorType, message) {
		$scope.alerts.push({ type: errorType, msg: message });
	};

	$scope.closeAlert = function (index) {
		$scope.alerts.splice(index, 1);
	};

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
			addAlert('danger', 'Please select at least one shopping center.');
			return;
		}

		programsApiService.saveByCommand('signUp', $scope.retailer)
			.then(function(data) {
				if (data.status === 200) {
					//TODO: figure out why this doesn't always work
					addAlert('success', 'Program was successfully saved.');
				}
			})
			.catch(function(data) {
				addAlert('danger', 'Unable to save Program data.');
			});
	};
});
