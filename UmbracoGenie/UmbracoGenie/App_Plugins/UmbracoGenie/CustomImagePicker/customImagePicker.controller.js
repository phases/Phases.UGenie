function CustomImagePickerController($scope, editorService) {
    var vm = this;
    vm.model = $scope.model;

    vm.openMediaPicker = function (event) {
        event.stopPropagation();
        var mediaPickerOptions = {
            multiPicker: false,
            submit: function (model) {
                vm.model.value = model.selection[0].image;
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };
        editorService.mediaPicker(mediaPickerOptions);
    };

    vm.add = function (event) {
        event.stopPropagation();
        var aiImagePicker = {
            title: "Generate Image by AI",
            view: "/App_Plugins/UmbracoGenie/CustomImagePicker/aipicker.html",
            size: "medium",
            submit: function (imageUrl) {
                vm.model.value = imageUrl;
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };
        editorService.open(aiImagePicker);
    };

    vm.close = function () {
        if (vm.model.close) {
            vm.model.close();
        }
    };

    vm.remove = function () {
        vm.model.value = null;
    };
}

angular.module("umbraco").controller("CustomImagePicker.controller", CustomImagePickerController);