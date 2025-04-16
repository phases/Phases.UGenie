(function () {
    'use strict';

    function GenieController($scope, notificationsService, genieService) {
        var vm = this;

        // Make vm available to child scopes
        $scope.vm = vm;

        vm.content = {
            name: "Genie",
            navigation: [
                {
                    "alias": "default",
                    "name": "Genie",
                    "icon": "icon-settings",
                    "view": "/App_Plugins/umbracoGenie/BackOffice/UmbracoGenieAlias/views/default.html",
                    "active": true
                },
                {
                    "alias": "textModels",
                    "name": "Text Models",
                    "icon": "icon-document-dashed-line",
                    "view": "/App_Plugins/umbracoGenie/BackOffice/UmbracoGenieAlias/views/textmodels.html"
                },
                {
                    "alias": "imageModels",
                    "name": "Image Models",
                    "icon": "icon-picture",
                    "view": "/App_Plugins/umbracoGenie/BackOffice/UmbracoGenieAlias/views/imagemodels.html"
                }
            ],
            textModels: [
                {
                    name: 'OpenAI',
                    modelId: 'gpt-3.5-turbo',
                    apiKey: '',
                    endpoint: ''
                },
                {
                    name: 'Azure OpenAI',
                    modelId: 'gpt-4',
                    apiKey: ''
                },
                {
                    name: 'Gemini',
                    modelId: 'gemini-pro',
                    apiKey: ''
                },
                {
                    name: 'Ollama',
                    modelId: 'llama2',
                    endpoint: 'http://localhost:11434'
                },
                //{
                //    name: 'HuggingFace',
                //    model: 'bigscience/bloom',
                //    endpoint: 'https://api-inference.huggingface.co/models',
                //    apiKey: ''
                //}
            ],
            imageModels: [
                {
                    name: 'Azure OpenAI',
                    provider: 'Azure',
                    apiKey: '',
                    model: 'dall-e-3',
                    endpoint: ''
                },
                {
                    name: 'OpenAI',
                    provider: 'OpenAI',
                    apiKey: '',
                    model: 'dall-e-3'
                },
                {
                    name: 'In-House',
                    provider: 'InHouse',
                    endpoint: 'http://localhost:5000/api/image-generation',
                    apiKey: ''
                }
            ],
            selectedTextModel: {
                name: 'OpenAI',
                modelId: 'gpt-3.5-turbo',
                apiKey: '',
                endpoint: ''
            },
            selectedImageModel: null,
            // New toggle options with default values
            enableForTextBox: false,
            enableForTextArea: false
        };

        // Track original selections
        vm.content.originalSelectedTextModel = angular.copy(vm.content.selectedTextModel);
        vm.content.originalSelectedImageModel = vm.content.selectedImageModel;
        // Track original toggle values
        vm.content.originalEnableForTextBox = vm.content.enableForTextBox;
        vm.content.originalEnableForTextArea = vm.content.enableForTextArea;

        vm.save = function () {
            var config = {
                selectedTextModel: vm.content.selectedTextModel || vm.content.textModels[0], // Default to OpenAI if none selected
                selectedImageModel: vm.content.selectedImageModel,
                textModels: vm.content.textModels,
                imageModels: vm.content.imageModels,
                // Explicitly convert toggle values to booleans (defaulting to false)
                enableForTextBox: vm.content.enableForTextBox === true,
                enableForTextArea: vm.content.enableForTextArea === true
            };

            genieService.saveConfiguration(config)
                .then(function (response) {
                    notificationsService.success("Success", "Configuration saved successfully");
                    vm.hasChanges = false;
                    // Update original values
                    vm.content.originalSelectedTextModel = angular.copy(vm.content.selectedTextModel || vm.content.textModels[0]);
                    vm.content.originalSelectedImageModel = angular.copy(vm.content.selectedImageModel);
                    vm.content.originalEnableForTextBox = vm.content.enableForTextBox;
                    vm.content.originalEnableForTextArea = vm.content.enableForTextArea;
                    if ($scope.$$childHead) {
                        $scope.$$childHead.hasChanges = false;
                    }
                })
                .catch(function (error) {
                    notificationsService.error("Error", "Failed to save configuration");
                    console.error('Save error:', error);
                });
        };

        // Load initial configuration
        genieService.getConfiguration()
            .then(function (response) {
                if (response.data) {
                    // Map PascalCase to camelCase
                    function mapModel(model) {
                        if (!model) return vm.content.textModels[0]; // Default to OpenAI if no model
                        return {
                            name: model.Name,
                            modelId: model.ModelId,
                            apiKey: model.ApiKey,
                            endpoint: model.Endpoint,
                            provider: model.Provider,
                            model: model.Model
                        };
                    }

                    // Map arrays
                    vm.content.textModels = (response.data.TextModels || []).map(mapModel);
                    vm.content.imageModels = (response.data.ImageModels || []).map(mapModel);

                    // Map selected models
                    vm.content.selectedTextModel = mapModel(response.data.SelectedTextModel);
                    vm.content.selectedImageModel = mapModel(response.data.SelectedImageModel);

                    // Map the toggle options
                    vm.content.enableForTextBox = response.data.enableForTextBox || false;
                    vm.content.enableForTextArea = response.data.enableForTextArea || false;

                    // Update original values
                    vm.content.originalSelectedTextModel = angular.copy(vm.content.selectedTextModel);
                    vm.content.originalSelectedImageModel = angular.copy(vm.content.selectedImageModel);
                    vm.content.originalEnableForTextBox = angular.copy(vm.content.enableForTextBox);
                    vm.content.originalEnableForTextArea = angular.copy(vm.content.enableForTextArea);

                    // Broadcast configuration loaded event
                    $scope.$broadcast('configurationLoaded', vm.content);
                }
            })
            .catch(function (error) {
                notificationsService.error("Error", "Failed to load configuration");
                console.error('Load error:', error);
            });

        // Remove previous $scope.toggleEnableForText and add toggle function on vm
        vm.toggleEnableForText = function (option) {
            if (option === 'TextBox') {
                vm.content.enableForTextBox = !vm.content.enableForTextBox;
            } else if (option === 'TextArea') {
                vm.content.enableForTextArea = !vm.content.enableForTextArea;
            }
        };

        // Listen for configUpdated event and update vm.content accordingly
        $scope.$on('configUpdated', function (event, data) {
            vm.content.enableForTextBox = data.enableForTextBox;
            vm.content.enableForTextArea = data.enableForTextArea;
        });

        // Initialize with changes detection
        vm.hasChanges = false;

        // Watch for changes in content
        $scope.$watch('vm.content', function (newVal, oldVal) {
            if (oldVal) {
                vm.hasChanges = true;
            }
            // Broadcast changes to child scopes
            $scope.$broadcast('vmContentUpdated', newVal);
        }, true);
    }

    angular.module('umbraco').controller('Genie.Controller', ['$scope', 'notificationsService', 'genieService', GenieController]);
})();