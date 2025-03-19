angular.module('umbraco')
    .controller('ConfigController', function ($scope, $timeout) {
        // Listen for configuration loaded event
        $scope.$on('configurationLoaded', function (event, content) {
            // Ensure model properties match the view bindings
            function ensureModelProperties(models) {
                return models.map(model => ({
                    name: model.name,
                    modelId: model.modelId,
                    apiKey: model.apiKey,
                    endpoint: model.endpoint,
                    provider: model.provider,
                    model: model.model
                }));
            }

            $scope.textModels = ensureModelProperties(content.textModels);
            $scope.imageModels = ensureModelProperties(content.imageModels);
            $scope.selectedTextModel = content.selectedTextModel;
            $scope.selectedImageModel = content.selectedImageModel;
            // Ensure default values for toggles
            $scope.enableForTextBox = angular.isDefined(content.enableForTextBox) ? content.enableForTextBox : false;
            $scope.enableForTextArea = angular.isDefined(content.enableForTextArea) ? content.enableForTextArea : false;

            // Update original copies
            $scope.originalTextModels = angular.copy($scope.textModels);
            $scope.originalImageModels = angular.copy($scope.imageModels);
        });

        // Get initial scope setup
        var init = function () {
            var parentScope = $scope.$parent;
            while (parentScope && !parentScope.vm) {
                parentScope = parentScope.$parent;
            }

            if (parentScope && parentScope.vm && parentScope.vm.content) {
                // Get models from parent
                $scope.textModels = parentScope.vm.content.textModels;
                $scope.imageModels = parentScope.vm.content.imageModels;

                // Set selected models
                if (parentScope.vm.content.selectedTextModel) {
                    $scope.selectedTextModel = $scope.textModels.find(
                        m => m.name === parentScope.vm.content.selectedTextModel.name
                    );
                }

                if (parentScope.vm.content.selectedImageModel) {
                    $scope.selectedImageModel = $scope.imageModels.find(
                        m => m.name === parentScope.vm.content.selectedImageModel.name
                    );
                }

                // Setup watchers for selection changes
                $scope.$watch('selectedTextModel', function (newVal) {
                    if (newVal) {
                        parentScope.vm.content.selectedTextModel = newVal;
                        parentScope.vm.hasChanges = true;
                    }
                });

                $scope.$watch('selectedImageModel', function (newVal) {
                    if (newVal) {
                        parentScope.vm.content.selectedImageModel = newVal;
                        parentScope.vm.hasChanges = true;
                    }
                });
            }
        };

        // Wait for parent scope to be ready
        $timeout(init, 0);

        // Initial configuration
        $scope.textModels = [
            {
                name: 'OpenAI',
                modelId: 'gpt-3.5-turbo',
                apiKey: '',
                endpoint: ''
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
        ];

        $scope.imageModels = [
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
                apiKey: '',
                model: ''
            }
        ];

        // Keep backups of the original state
        $scope.originalTextModels = angular.copy($scope.textModels);
        $scope.originalImageModels = angular.copy($scope.imageModels);

        $scope.showFullKey = {};
        $scope.editMode = {};
        $scope.selectedService = null;
        $scope.hasChanges = false;
        $scope.activeTab = 'text'; // Default to text tab

        $scope.maskApiKey = function (key) {
            if (!key) return 'No API key set';
            if (key.length <= 6) return '*'.repeat(key.length);
            return key.substring(0, 6) + '*'.repeat(key.length - 6);
        };

        $scope.toggleKey = function (serviceName, $event) {
            if ($event) {
                $event.stopPropagation();
            }
            $scope.showFullKey[serviceName] = !$scope.showFullKey[serviceName];
        };

        $scope.toggleEdit = function (service, $event) {
            if ($event) {
                $event.stopPropagation();
            }
            $scope.editMode[service.name] = !$scope.editMode[service.name];
        };

        $scope.selectService = function (serviceName) {
            $scope.selectedService = serviceName;
        };

        $scope.switchTab = function (tab) {
            $scope.activeTab = tab;
            $scope.selectedService = null;
        };

        // Track changes in the form
        $scope.$watch('[textModels, imageModels]', function (newVal, oldVal) {
            if (oldVal && (!angular.equals(newVal[0], $scope.originalTextModels) ||
                !angular.equals(newVal[1], $scope.originalImageModels))) {
                $scope.hasChanges = true;
            }
        }, true);

        // Save all changes
        $scope.saveAllChanges = function () {
            $scope.originalTextModels = angular.copy($scope.textModels);
            $scope.originalImageModels = angular.copy($scope.imageModels);
            $scope.hasChanges = false;
        };

        // Discard all changes
        $scope.discardChanges = function () {
            $scope.textModels = angular.copy($scope.originalTextModels);
            $scope.imageModels = angular.copy($scope.originalImageModels);
            $scope.hasChanges = false;
            Object.keys($scope.editMode).forEach(key => {
                $scope.editMode[key] = false;
            });
        };

        // Add selected model tracking
        $scope.selectedTextModel = null;
        $scope.selectedImageModel = null;

        // Remove the problematic watch and replace with a simpler one
        $scope.$watch('[textModels, imageModels, selectedTextModel, selectedImageModel]', function (newVal) {
            if ($scope.$parent.vm && $scope.$parent.vm.content) {
                var vm = $scope.$parent.vm;

                // Check for actual changes
                var hasModelChanges = !angular.equals(newVal[0], $scope.originalTextModels) ||
                    !angular.equals(newVal[1], $scope.originalImageModels);
                var hasSelectionChanges = newVal[2] !== $scope.selectedTextModel ||
                    newVal[3] !== $scope.selectedImageModel;

                $scope.hasChanges = hasModelChanges || hasSelectionChanges;
                vm.hasChanges = $scope.hasChanges;
            }
        }, true);

        // Add toggle function to ConfigController:
        $scope.toggleEnableForText = function (option) {
            if (option === 'TextBox') {
                $scope.enableForTextBox = !$scope.enableForTextBox;
                if ($scope.$parent.vm && $scope.$parent.vm.content) {
                    $scope.$parent.vm.content.enableForTextBox = $scope.enableForTextBox;
                }
            } else if (option === 'TextArea') {
                $scope.enableForTextArea = !$scope.enableForTextArea;
                if ($scope.$parent.vm && $scope.$parent.vm.content) {
                    $scope.$parent.vm.content.enableForTextArea = $scope.enableForTextArea;
                }
            }
            $scope.hasChanges = true;
            // Emit event to update parent controller's configuration
            $scope.$emit('configUpdated', {
                enableForTextBox: $scope.enableForTextBox,
                enableForTextArea: $scope.enableForTextArea
            });
        };
    });
