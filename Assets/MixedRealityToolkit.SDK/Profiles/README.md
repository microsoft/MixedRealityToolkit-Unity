# Mixed Reality Toolkit - SDK - Elements - Profiles

This folder contains example MRTK profiles used to configure your solution.


This includes:

## [MixedRealityConfigurationProfile]()

The main configuration profile for the Mixed Reality Toolkit, hosting the start up and manager initialization options for the framework.

## [MixedRealityInputActionsProfile]()

Input Actions catalogue for your project, defining the logical actions your project will perform for any given input / axis type

## [MixedRealityControllerConfigurationProfile](MixedRealityControllerConfigurationProfile.md)

Central configuration file for controllers to be used in your project. Allows the registration of controllers for various SDK's and map the inputs of those controllers to the logical actions used in your project.

Additionally, allows you to set the models to be used for those controllers, whether they are the SDK's default, a single generic model per hand or specific models for each controller type.

## [MixedRealityCameraProfile]()

Camera profile options for the project, including clipping and skybox settings.

## [MixedRealitySpeechCommandsProfile]()

Similar to the Input Actions, allows you to define a set of recognised keywords and assign them to logical Input actions in your project.

## [MixedRealityDiagnosticsProfile]()

Configuration for showing diagnostic data while using your project.
