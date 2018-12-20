# Mixed Reality Toolkit Configuration Guide

![](/External/ReadMeImages/MRTK_Logo_Rev.png)

The Mixed Reality Toolkit centralizes as much of the configuration required to manage the toolkit as possible (except for true runtime "things").

**This guide is a simple walkthrough for each of the configuration screens currently available for the toolkit, more in-depth guides for each of the features is coming soon.**

Configuration profiles provide reusable blocks of configuration that can be used and swapped out at runtime (with the exception of the InputActions profile) to meet the demands for most Mixed Reality projects.  This allows you to style your configuration for different input types (Driving vs Flying) or different behavior's your project needs.

> For more details on profile use, please check the [Configuration Profile Usage Guide]() (Coming soon()

In some cases, we also allow you to swap out the underlying system that provides a capability with either your own service or an alternate implementation (e.g. swapping out the speech provider from an OS version to one on Azure)

> For more detail on writing your own compatible systems for use in the toolkit, please see the [Guide to building Registered Services]() (Coming soon)

## The main Mixed Reality Toolkit Configuration profile

The main configuration profile, which is attached to the *MixedRealityToolkit* GameObject in your Scene, provides the main entry point for the Toolkit in your project.

> The Mixed Reality Toolkit "locks" the default configuration screens to ensure you always have a common start point for your project and we encourage you to start defining your own settings as your project evolves.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ActiveConfiguration.png)

> All the "default" profiles for the Mixed Reality Toolkit can be found in the SDK project in the following folder

> Assets\MixedRealityToolkit-SDK\Profiles

When you open the main Mixed Reality Toolkit Configuration Profile, you will see the following screen in the inspector:

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_MixedRealityToolkitConfigurationScreen.png)

> If you select a MixedRealityToolkitConfigurationProfile asset without the MixedRealityToolkit in the scene, it will ask you if you want the MRTK to automatically setup the scene for you.  This is optional, however, there must be an active MixedRealityToolkit object in the scene to access all the configuration screens.

This houses the current active runtime configuration for the project.

> Note, almost any profile can be swapped out at runtime, with the exception of the InputActions configuration (see later).  The profiles with then automatically adapt to the new configuration / runtime environment automatically.

From here you can navigate to all the configuration profiles for the MRTK, including:

* [Experience Settings](#experience)
* [Camera Settings](#camera)
* [Input System Settings](#input-system-settings)
* [Boundary Settings](#boundary)
* [Teleporting Settings](#teleportation)
* [Spatial Awareness Settings](#spatialawareness)
* [Diagnostics Settings](#diagnostic)
* [Additional Services Settings](#services)
* [Input Actions Settings](#inputactions)
* [Input Actions Rules](#inputactionrules)
* [Pointer Configuration](#pointer)
* [Gestures Configuration](#gestures)
* [Speech Commands](#speech)
* [Controller Mapping Configuration](#mapping)
* [Controller Visualization Settings](#visualization)

These configuration profiles are detailed below in their relevant sections:

From here you can navigate to all the configuration profiles for the MRTK, including:

---
<a name="experience"/>

## Experience Settings

Located on the main Mixed Reality Toolkit configuration page, this setting defines the default operation for the Mixed Reality environment for your project.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ExperienceSettings.png)


---
<a name="camera"/>

## Camera Settings

The camera settings define how the camera will be setup for your Mixed Reality project, defining the generic clipping, quality and transparency settings.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_CameraProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.

---
<a name="inputsystem"/>

## Input System Settings

The Mixed Reality Project provides a robust and well-trained input system for routing all the input events around the project which is selected by default.

> The MRTK also allows you to write your own Input System and you can use the selection below to switch the system used without rewriting the toolkit.  For more information on writing your own systems, [please see this guide to building Registered Services]() (Coming soon)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputSystemSelection.png)

Behind the Input System provided by the MRTK are several other systems, these help to drive and manage the complex inter-weavings required to abstract out the complexities of a multi-platform / mixed reality framework.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputSystemProfile.png)

Each of the individual profiles are detailed below:

* [Focus Settings](#focus-settings)
* [Input Actions Settings](#input-actions-settings)
* [Input Actions Rules](#inputactionrules)
* [Pointer Configuration](#pointer)
* [Gestures Configuration](#gestures)
* [Speech Commands](#speech)
* [Controller Mapping Configuration](#mapping)
* [Controller Visualization Settings](#visualization)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
<a name="boundary"/>

## Boundary Visualization Settings

The boundary system translates the perceived boundary reported by the underlying platforms boundary / guardian system.  The Boundary visualizer configuration gives you the ability to automatically show the recorded boundary within your scene relative to the user's position.  The boundary will also react / update based on where the user teleports within the scene.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_BoundaryVisualizationProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
<a name="teleportation"/>

## Teleportation System Selection

The Mixed Reality Project provides a full featured Teleportation system for managing teleportation events in the project which is selected by default.

> The MRTK also allows you to write your own Teleportation System and you can use the selection below to switch the system used without rewriting the toolkit.  For more information on writing your own systems, [please see this guide to building Registered Services]() (Coming soon)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_TeleportationSystemSelection.png)

---
<a name="spatialawareness"/>

## Spatial Awareness Settings

The Mixed Reality Project provides a rebuilt Spatial Awareness system for working with spatial scanning systems in the project which is selected by default.
You can view the architecture behind the [MRTK Spatial Awareness system here](/Documentation/Architecture/SpatialAwarenessSystemArchitecture.md).

> The MRTK also allows you to write your own Spatial Awareness System and you can use the selection below to switch the system used without rewriting the toolkit.  For more information on writing your own systems, [please see this guide to building Registered Services]() (Coming soon)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpatialAwarenessSystemSelection.png)

The Mixed Reality Toolkit Spatial Awareness configuration lets you tailor how the system starts, whether it is automatically when the application starts or later programmatically as well as setting the extents for the Field of View.

It also lets you configure the Mesh and surface settings, further customizing how your project understands the environment around you. 

This is only applicable for devices that can provide a scanned environment, such as the HoloLens (and other devices in the future)

> Note, the Spatial Awareness system is still in active development, please report any issues or requests in the [MRTK Issues section on GitHub](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpatialAwarenessProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
<a name="diagnostic"/>

## Diagnostics Settings

An optional but highly useful feature of the MRTK is the plugin Diagnostics functionality. This presents a style of debug log in to the scene

> The MRTK also allows you to write your own Diagnostic System and you can use the selection below to switch the system used without rewriting the toolkit.  For more information on writing your own systems, [please see this guide to building Registered Services]() (Coming soon)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_DiagnosticsSystemSelection.png)

The diagnostics profile provides several simple systems to monitor whilst the project is running, including a handy On/Off switch to enable / disable the display pane in the scene.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_DiagnosticsProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
<a name="services"/>

## Additional Services Settings

One of the more advanced areas of the Mixed Reality Toolkit is its [service locator pattern](https://en.wikipedia.org/wiki/Service_locator_pattern) implementation which allows the registering of any "Service" with the framework. This allows the framework to be both extended with new features / systems easily but also allows for projects to take advantage of these capabilities to register their own runtime components.

> You can read more about the underlying framework and it's implementation in [Stephen Hodgson's article on the Mixed Reality Framework](https://medium.com/@stephen_hodgson/the-mixed-reality-framework-6fdb5c11feb2)

Any registered service still gets the full advantage of all of the Unity events, without the overhead and cost of implementing a MonoBehaviour or clunky singleton patterns.  This allows for pure C# components with no scene overhead for running both foreground and background processes, e.g. spawning systems, runtime gamelogic, or practically anything else.

Check out the [Guide to building Registered Services]() (Coming Soon) for more details about creating your own services

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_RegisteredServiceProfidersProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
<a name="inputactions"/>

## Input Actions Settings

Input Actions provide a way to abstract any physical interactions and input from a runtime project.  All physical input (from Controllers / hands / mouse / etc) is translated in to a logical Input Action for use in your runtime project.  This ensures no matter where the input comes from, your project simply implements these actions as "Things to do" or "Interact with" in your scenes.

To create a new Input Action, simply click the "Add a new Action" button and enter a friendly text name for what it represents.  You then only need select an Axis (the type of data) the action is meant to convey, or in the case of physical controllers, the physical input type it can be attached to, for example:

| Axis Constraint | Data Type | Description | Example use |
| :--- | :--- | :--- | :--- |
| None | No data | Used for an empty action or event | Event Trigger |
| Raw (reserved) | object | Reserved for future use | N/A |
| Digital | bool | A boolean on or off type data | A controller button |
| Single Axis | float | A single precision data value | A ranged input, e.g. a trigger |
| Dual Axis | Vector2 | A dual float type date for multiple axis | A Dpad or Thumbstick |
| Three Dof Position | Vector3 | Positional type data from with 3 float axis | 3D position style only controller |
| Three Dof Rotation | Quaternion | Rotational only input with 4 float axis | A Three degrees style controller, e.g. Oculus Go controller |
| Six Dof | Mixed Reality Pose (Vector3, Quaternion) | A position and rotation style input with both Vector3 and Quaternion components | A motion controller or Pointer |

Events utilizing Input Actions are not limited to physical controllers and can still be utilized within the project to have runtime effects generate new actions.

> Input Actions are one of the few components which is not editable at runtime, they are a design time configuration only.  This profile should not be swapped out whilst the project is running due to the framework (and your projects) dependency on the ID's generated for each action.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputActionsProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
<a name="inputactionrules"/>

## Input Actions Rules

Input Action Rules provide a way to automatically translate an event raised for one Input Action in to different actions based on its data value.  These are managed seamlessly within the framework and do not incur any performance costs.

For example, converting the single Dual Axis input event from a DPad in to the 4 corresponding Dpad Up / DPad Down / Dpad Left / Dpad Right actions. (as shown in the image below)

> This could also be done i your own code, but seeing as this was a very common patter, the framework provides a mechanism to do this "out of the box"

Input Action Rules can be configured for any of the available input axis. However, Input actions from one Axis type can be translated to another Input Action of the same Axis type.  You can map a Dual Axis action to another Dual Axis action, but not to a Digital or None action.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputActionRulesProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
<a name="pointer"/>

## Pointer Configuration

Pointers are used to drive interactivity in the scene from any input device, giving both a direction and hit test with any object in a scene (that has a collider attached, or is a UI component).  Pointers are by default automatically configured for controllers, headsets (gaze/focus) and mouse/touch input.

Pointers can also be visualized within the active scene using one of the many Line components provided by the Mixed Reality Toolkit, or any of your own if they implement the MRTK IMixedRealityPointer interface.

> See the [Guide to Pointers documentation]() **Coming Soon** for more information on creating your own pointers.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputPointerProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.

* Pointing Extent: Determines the global pointing extent for all pointers, including gaze.
* Pointing Raycast Layer Masks: Determines which layers pointers will raycast against.
* Debug Draw Pointing Rays: A debug helper for visualizing the rays used for raycasting.
* Debug Draw Pointing Rays Colors: A set of colors to use for visualizing.
* Gaze cursor prefab: Makes it easy to specify a global gaze cursor for any scene.

There's an additional helper button to quickly jump to the Gaze Provider to override some specific values for Gaze if needed.

---
<a name="gestures"/>

## Gestures Configuration

Gestures are a system specific implementation allowing you to assign Input Actions to the various "Gesture" input methods provided by various SDK's (e.g. HoloLens).

> Note, the current implementation is for the HoloLens only and will be enhanced for other systems as they are added to the Toolkit in the future (no dates yet).

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_GesturesProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
<a name="speech"/>

## Speech Commands

Like Gestures, some runtime platforms also provide intelligent Speech to Text functionality with the ability to generate "Commands" that can be received by a Unity project.  This configuration profile allows you to configure registered "words" and translate them in to Input Actions that can be received by your project. (they can also be attached to keyboard actions if required)

> The system currently only supports speech when running on Windows 10 platforms, e.g. HoloLens and Windows 10 desktop and will be enhanced for other systems as they are added to the Toolkit in the future (no dates yet).

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpeechCommandsProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
<a name="mapping"/>

## Controller Mapping Configuration

One of the core configuration screens for the Mixed Reality Toolkit is the ability to configure and map the various types of controllers that can be utilized by your project.

The configuration screen below allows you to configure any of the controllers currently recognized by the toolkit.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ControllerMappingProfile.png)

The MRTK provides a default configuration for the following controllers / systems:

* Mouse (including 3D spatial mouse support)
* Touch Screen
* Xbox controllers
* Windows Mixed Reality controllers
* HoloLens Gestures
* HTC Vive wand controllers
* Oculus Touch controllers
* Oculus Remote controller
* Generic OpenVR devices (advanced users only)

Clicking on the Image for any of the pre-built controller systems allows you to configure a single Input Action for all its corresponding inputs, for example, see the Oculus Touch controller configuration screen below:

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_OculusTouchConfigScreen.png)

There is also an advanced screen for configuring other OpenVR or Unity input controllers that are not identified above.

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
<a name="visualization"/>

## Controller Visualization Settings

In addition to the Controller mapping, a separate configuration profile is provided to customize how your controllers are presented within your scenes.

This can be configured at a "Global" (all instances of a controller for a specific hand) or specific to an individual controller type / hand.

> The MRTK does not currently support native SDK's controller models as Unity does not yet provide the capability to load / render gLTF models, which is the default type of models provided by most SDKs.  This will be enhanced when this is available.

If your controller representation in the scene needs to be offset from the physical controller position, then simply set that offset against the controller model's prefab.  (e.g. setting the transform position of the controller prefab with an offset position)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ControllerVisualizationProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.