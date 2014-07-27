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


app.controller('programCtrl', function($scope, $routeParams, $location, $anchorScroll, programsApiService) {
	$scope.alerts = [];
	//$scope.program.ProgramId = $routeParams.programId;
	$scope.program = {
		ParticipatingMalls: [],
		CouponWordCount: null,
		Description: null,
		Disclaimer: null,
		Emails: [],
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
		var idx = $scope.program.ParticipatingMalls.indexOf(id);

		// is currently selected
		if (idx > -1) {
			$scope.program.ParticipatingMalls.splice(idx, 1);
		}
// is newly selected
		else {
			$scope.program.ParticipatingMalls.push(id);
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
		$scope.program.Retailers = [];

		_.forEach(emails, function(email) {
			$scope.program.Retailers.push({ Email: email });
		});

		programsApiService.saveByCommand('createProgram', $scope.program)
			.then(function(data) {
				if (data.status === 200) {
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
