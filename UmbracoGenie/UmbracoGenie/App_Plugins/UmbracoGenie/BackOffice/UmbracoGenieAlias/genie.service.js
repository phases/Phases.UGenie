angular.module('umbraco')
    .factory('genieService', function ($http) {
        var service = {
            saveConfiguration: function (config) {
                // Deep copy to avoid modifying original data
                var cleanConfig = angular.copy(config);

                // Function to remove Angular properties recursively
                var removeAngularProps = function (obj) {
                    if (obj && typeof obj === 'object') {
                        delete obj.$$hashKey;
                        Object.keys(obj).forEach(key => {
                            if (obj[key] && typeof obj[key] === 'object') {
                                removeAngularProps(obj[key]);
                            }
                        });
                    }
                };

                // Clean all objects
                removeAngularProps(cleanConfig.selectedTextModel);
                removeAngularProps(cleanConfig.selectedImageModel);
                cleanConfig.textModels?.forEach(removeAngularProps);
                cleanConfig.imageModels?.forEach(removeAngularProps);

                return $http.post('/umbraco/backoffice/api/Genie/SaveConfiguration', cleanConfig);
            },
            getConfiguration: function () {
                return $http.get('/umbraco/backoffice/api/Genie/GetConfiguration');
            }
        };
        return service;
    });
