app.controller('adminMenuCtrl', ['$scope', '$http', function($scope, $http) {
		$scope.programs = [{name: 'program1', id: 1 }, {name: 'program2', id: 2}];
	}
]);

app.controller('programCtrl', ['$scope', '$routeParams', function($scope, $routeParams) {
		$scope.programId = $routeParams.programId;
	}
]);