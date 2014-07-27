app.directive('autoFocus', function() {
	return{
		restrict: 'A',

		link: function(scope, element, attrs){
			scope.$watch(function(){
				return scope.$eval(attrs.autoFocus);
			},function (newValue){
				if (newValue == true){
					element[0].focus();
				}
			});
		}
	};
});