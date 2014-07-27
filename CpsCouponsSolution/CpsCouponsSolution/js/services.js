app.factory('programsApiService', function ($http, $q) {

	return {
		baseQueriesApiUrl: '/api/programs/',

		getByCommand: function (command) {
			var deferred = $q.defer();

			var apiUrl = this.baseQueriesApiUrl + command;

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

		saveByCommand: function (command, model) {
			var deferred = $q.defer();
			var apiUrl = this.baseQueriesApiUrl + command;
			
			$http({
				url: apiUrl,
				method: 'POST',
				dataType: 'json',
				data: model
			})
				.success(function (data, status) {
					data = data || {};
					data.status = status;

					deferred.resolve(data);
				})
				.error(function (data, status) {
					data = data || {};
					data.status = status;

					deferred.reject(data);
				});

			return deferred.promise;
		}

	};
});