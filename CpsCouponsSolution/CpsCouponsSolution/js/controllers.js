app.controller('adminMenuCtrl', function($scope) {
		$scope.programs = [{ name: 'program1', id: 1 }, { name: 'program2', id: 2 }];
	}
);

app.controller('programCtrl', function ($scope, $routeParams, programsApiService) {
		$scope.programId = $routeParams.programId;

		$scope.getMallList = function() {
			programsApiService.getMallList()
				.then(function(data) {
					$scope.malls = data;
				})
				.catch(function() {
					addAlert('Unable to get Center data');
				});
		};

		$scope.getMallList();

		$scope.addAlert = function(message) {
			$scope.alerts.push({ msg: message });
		};

		$scope.closeAlert = function(index) {
			$scope.alerts.splice(index, 1);
		};
	}
);