app.factory('programsApiService', function ($http, $q) {

	return {
		baseQueriesApiUrl: '/api/programs/',

		getMallList: function () {
			var deferred = $q.defer();

			var apiUrl = this.baseQueriesApiUrl + 'getMallList';

			$http({ method: 'GET', url: apiUrl }).
				success(function (data) {
					deferred.resolve(data);
				}).
				error(function (data, status) {
					data = data || {};

					data.status = status;

					deferred.reject(data);
				});

			return deferred.promise;
		},

	};
});