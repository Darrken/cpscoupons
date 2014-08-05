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

		getRetailersByMall: function (mallId) {
			var deferred = $q.defer();

			var apiUrl = this.baseQueriesApiUrl + 'getRetailersByMall?mallId=' + encodeURIComponent(mallId);

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

		getRetailersByProgram: function (programId) {
			var deferred = $q.defer();

			var apiUrl = this.baseQueriesApiUrl + 'getRetailersByProgram?programId=' + encodeURIComponent(programId);

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
		
		getProgramById: function (programId) {
			var deferred = $q.defer();

			var apiUrl = this.baseQueriesApiUrl + 'getProgramById?programId=' + encodeURIComponent(programId);

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

		getProgramByRetailer: function (id) {
			var deferred = $q.defer();

			var apiUrl = this.baseQueriesApiUrl + 'getProgramByRetailer?guid=' + encodeURIComponent(id);

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

app.factory('alertService', function() {
	var root = {};
	root.alerts = [];

	root.clearAlerts = function () {
		root.alerts = [];
	};
	
	root.addAlert = function (errorType, message) {
		root.alerts.push({ type: errorType, msg: message });
	};

	root.closeAlert = function (index) {
		root.alerts.splice(index, 1);
	};

	return root;
});

app.factory('fileService', function ($filter) {

	function createColumns(columns) {

		var fileColumns = '';

		for (var j = 0; j < columns.length; j++) {
			var innerValue = columns[j].toString();
			var result = innerValue.replace(/"/g, '""');

			if (result.search(/("|,|\n)/g) >= 0) {
				result = '"' + result + '"';
			}

			if (j > 0) {
				fileColumns += ',';
			}

			fileColumns += result;
		}

		fileColumns += '\n';

		return fileColumns;
	}

	function createFileResult(columns, rows) {
		var fileResult = createColumns(columns);

		for (var i = 0; i < rows.length; i++) {
			var row = rows[i];

			for (var j = 0; j < columns.length; j++) {
				var result = '';

				result = angular.isDefined(row[columns[j]]) ? row[columns[j]] : result;
				result = result.toString().replace(/"/g, '""');

				if (result.search(/("|,|\n)/g) >= 0) {
					result = '"' + result + '"';
				}

				if (j > 0) {
					fileResult += ',';
				}

				fileResult += result;
			}

			fileResult += '\n';
		}
		return fileResult;
	}

	return {
		getFileResult: function (columns, rows) {

			var fileResult = createFileResult(columns, rows);

			return fileResult;
		},

		createCsvFile: function (columns, rows, fileName) {

			var nowDate = new Date;
			var utcDateString = (
				[
					nowDate.getUTCFullYear(),
					nowDate.getUTCMonth(),
					nowDate.getUTCDate() + 'T' + nowDate.getUTCHours(),
					nowDate.getUTCMinutes(),
					nowDate.getUTCSeconds()
				].join('_'));

			fileName = fileName || 'NoNameProvided';
			fileName = fileName + '_' + utcDateString;

			var fileResult = createFileResult(columns, rows);

			var downloadAnchor = document.createElement('a');
			downloadAnchor.setAttribute('type', 'text/csv');
			var byteOrderMark = decodeURIComponent('%EF%BB%BF');
			var b = new Blob([byteOrderMark + fileResult], { type: 'text/csv' });
			downloadAnchor.href = URL.createObjectURL(b);
			downloadAnchor.setAttribute('download', fileName + '.csv');

			downloadAnchor.click();
		}
	};
});

app.factory('adminService', function ($location) {
	var adminPwd = 'CpsAdmin1';
	var isAdmin = false;
	var returnPath = '/adminmenu';
	
	return {
		loginAdmin: function (pwd) {
			if (isAdmin = pwd === adminPwd) {
				$location.path(returnPath);
				return true;
			}

			return false;
		},
		
		adminCheck: function (path) {
			returnPath = path;
			
			if (!isAdmin) {
				$location.path('adminlogin');
			}
		},
		
		isAdmin: function() {
			return isAdmin;
		}
	};
});