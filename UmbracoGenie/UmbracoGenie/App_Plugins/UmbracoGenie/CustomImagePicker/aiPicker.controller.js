function apiPickerController($scope, $http, $element) {
    $scope.submit = function (result) {
        if ($scope.model.submit) {
            $scope.model.submit(result);
        }
    };

    $scope.close = function () {
        if ($scope.model.close) {
            $scope.model.close();
        }
    };

    $scope.generate = function () {
        $scope.model.loading = true;
        fetch('/umbraco/api/AIGeneration/GenerateImage', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ prompt: $scope.model.prompt })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(imagePath => {
                $scope.model.imageUrl = imagePath;
            })
            .catch(error => {
                console.error('Error generating image:', error);
            })
            .finally(() => {
                $scope.model.loading = false;
            });
        
    };
}
angular.module('umbraco').controller("apiPicker.controller", apiPickerController);