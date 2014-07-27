app.controller('adminMenuCtrl', function ($scope) {
	$scope.programs = [{ name: 'program1', id: 1 }, { name: 'program2', id: 2 }];
});

app.controller('programCtrl', function ($scope, $routeParams, programsApiService) {
	$scope.program = {};
	$scope.alerts = [];
	$scope.program.ProgramId = $routeParams.programId;
	$scope.program.Centers = [];

	$scope.getMallList = function () {
		programsApiService.getMallList()
			.then(function (data) {
				$scope.malls = data;
			})
			.catch(function () {
				addAlert('Unable to get Center data');
			});
	};

	$scope.getMallList();

	function addAlert(message) {
		$scope.alerts.push({ type: 'danger', msg: message });
	};

	$scope.closeAlert = function (index) {
		$scope.alerts.splice(index, 1);
	};

	$scope.toggleCenter = function (id) {
		var idx = $scope.program.Centers.indexOf(id);

		// is currently selected
		if (idx > -1) {
			$scope.program.Centers.splice(idx, 1);
		}

			// is newly selected
		else {
			$scope.program.Centers.push(id);
		}
	};

	$scope.saveProgram = function () {
		var toSubmit = {};


	};
});