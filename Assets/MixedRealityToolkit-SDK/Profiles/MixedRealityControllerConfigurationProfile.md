# Mixed Reality Controller Configuration Profile

When you need to use input controllers for your Mixed Reality project, they are registered and configured centrally within the Controller configuration profile as you can see here:

![](/External/HowTo/ControllerConfigurationProfile/01-MixedRealityControllerConfigurationProfileInspector.png)

This enables you to very quickly define which SDK's / controllers you want to support in your project and configure how each are intended to work.

The configuration is broken down in to several key components, as detailed below:

## Main Controller Template definition
---
![](/External/HowTo/ControllerConfigurationProfile/02-ControllerTemplateDefinition.png)

In the first section of the configuration, the options are detailed as follows:

### Render Motion Controllers
This defines whether or not controllers should be rendered or not.  By not adding a ControllerVisualizer in to the scene also has the same effect.
> This also disabled the Global Model options below.

### Use Default Models (for future implementation)
This option will use the models for the controller direct from the SDK (where available), so that you don't have to configure a custom model.

### Global Left / Right hand models
These allow you at the top level to define the default model that will be drawn for each controlling hand.  If no hand is available, the Left most model will be used by default.

To alter the position and rotation of the displayed model in relation to the Controller pose, then update the controllers Model Prefab transform values.

> See the "Example Models" section below for the controller models and their recommended transform settings provided in the SDK's "**Standard Assets**" folder.

### Add a New Controller Template

This enables you to add a new controller definition (detailed below) to the profile to add another supported SDK.

## Controller Template
---
![](/External/HowTo/ControllerConfigurationProfile/03-ControllerTemplate.png)

Each controller template allows you to configure any of the supported controllers for the various SDK's that have been enabled through the Mixed Reality Toolkit.
Each controller is added by SDK and the prevailing hand.

> Any SDK Controller types or hands NOT configured will not be detected or used in a running project.

The options for configuring a template are detailed as follows:

### Controller
Your custom name for the controller, just for easy reference

### Controller Type (Work in progress, see list for enabled devices)
A drop down list of supported controllers by the Mixed Reality Toolkit, namely:

* Windows Mixed Reality
    * Motion Controllers
* OpenVR
    * Default (fallback) controllers
    * Oculus Touch
    * Vive Wand 
    * Vive Knuckles (experimental as no one has any, using Unity spec)
* OpenXR
    * To be supported when there is an OpenXR specification
* Windows Gaming (these are coming soon*)
    * Arcade stick
    * Gamepad
    * Joystick
    * Racing Wheel
* Simulator (coming soon)

### Handedness
Which hand is configured for this controller definition
> Both does not configure all controllers at this time.

### Use Default model
For this controller only, use the model for the controller direct from the SDK (where available), so that you don't have to configure a custom model.
> This overrides the Global Model setting

### Override Model
Like the Global Model options, allows you to provide a model to be drawn for this specific controller.
> This overrides the Global Model setting

### Interaction Mappings
The interaction mappings allow you to map logical input actions for use in your project to the various controller inputs available from the physical device.  

These are different for each controller type, as shown below:

| Motion Controller | Oculus Touch | Vive Wand |
|---|---|---|
|![](/External/HowTo/ControllerConfigurationProfile/04-WMRInteractions.png)|![](/External/HowTo/ControllerConfigurationProfile/05-OculusTouchInteractions.png)|![](/External/HowTo/ControllerConfigurationProfile/06-ViveWandInteractions.png)|

> In the future custom mappings may become available, for now they are defined per the devices own specification according to the input definitions set out by Unity

The Action each input can perform, is completely up to you.

> See the [Input Action]() configuration profile for more information.

## Example Models
---

The models provided in the Mixed Reality Toolkit "Standard Assets" folder are as follows:

> These can be found in "Mixed Reality Toolkit SDK / Standard Assets / Controllers"

### Debug Controllers
The Mixed Reality Toolkit provides a set of basic Gizmo style controllers, used to help align your models to the controller position output by the SDK, to help with offset settings.
> Note, the ability to display BOTH the Gizmo and the controller models isn't supported as yet. This will be included in a future release.

We recommend applying the following Prefab transform values to align the controller model when using to align with the users view:
* Position - no change
* Rotation - X 90 (left) -90 (right), Y 0 , Z 0

## Additional models available to download
---

### Oculus Hands (provided via the Oculus Opensource project)
Oculus provide two basic hand models, including animation (although not available through the visualizer currently).  

Source -> [Oculus Hand Package download](https://developer.oculus.com/downloads/package/oculus-hand-models/)

We recommend applying the following Prefab transform values to align the controller model with the users view:
* Position - no change
* Rotation - X 0, Y 0 , Z 90 (left) -90 (right)

### Oculus Rift touch controllers (provided via the Oculus Opensource project)
Oculus also provide animated controller models for their touch controllers.

Source -> [Oculus Sample Framework for Unity Project](https://developer.oculus.com/downloads/package/oculus-sample-framework-for-unity-5-project/)

We recommend applying the following Prefab transform values to align the controller model when using to align with the users view:
* Position - no change
* Rotation - no change

### HTC Vive wand controllers (available in the SteamVR SDK)
Steam provide basic models for the HTC Vive Wand controllers.

Source -> Included as part of the Unity SteamVR SDK, or the Steam VR Client
"(Steam Install Folder)\Steam\steamapps\common\SteamVR\resources\rendermodels"

If you use Steams Models, we recommend applying the following Prefab transform values to align the controller model when using to align with the users view:
* Position - no change
* Rotation - no change