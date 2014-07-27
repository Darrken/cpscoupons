app.controller('adminMenuCtrl', function ($scope, programsApiService) {
	$scope.programs = [{ name: 'program1', id: 1 }, { name: 'program2', id: 2 }];

	$scope.getMallList = function () {
		programsApiService.getMallList()
			.then(function(data) {
				$scope.malls = data;
			})
			.catch(function() {
				addAlert('Unable to get Center data');
			});
	};

	$scope.getMallList();
		console.log($scope);

	$scope.addAlert = function (message) {
		$scope.alerts.push({ msg: message });
	};

	$scope.closeAlert = function (index) {
		$scope.alerts.splice(index, 1);
	};
}

);

app.controller('programCtrl', ['$scope', '$routeParams', function($scope, $routeParams) {
		$scope.programId = $routeParams.programId;
	}
]);