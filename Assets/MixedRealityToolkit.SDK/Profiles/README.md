# Mixed Reality Toolkit - SDK - Elements - Profiles

This folder contains example MRTK profiles used to configure your solution.

This includes:

## [MixedRealityConfigurationProfile](../../../Documentation/MixedRealityConfigurationGuide.md)

The main configuration profile for the Mixed Reality Toolkit, hosting the start up and manager initialization options for the framework.

## [MixedRealityInputActionsProfile](../../../Documentation/Input/InputActions.md)

Input Actions catalogue for your project, defining the logical actions your project will perform for any given input / axis type

## [MixedRealityControllerConfigurationProfile](MixedRealityControllerConfigurationProfile.md)

Central configuration file for controllers to be used in your project. Allows the registration of controllers for various SDKs and map the inputs of those controllers to the logical actions used in your project.

Additionally, allows you to set the models to be used for those controllers, whether they are the SDK's default, a single generic model per hand or specific models for each controller type.

## [MixedRealityCameraProfile](../../../Documentation/CameraSystem/CameraSystemOverview.md)

Camera profile options for the project, including clipping and skybox settings.

## [MixedRealitySpeechCommandsProfile](../../../Documentation/Input/Speech.md)

Similar to the Input Actions, allows you to define a set of recognized keywords and assign them to logical Input actions in your project.

## [MixedRealityDiagnosticsProfile](../../../Documentation/Diagnostics/ConfiguringDiagnostics.md)

Configuration for showing diagnostic data while using your project.
