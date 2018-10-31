# Mixed Reality Toolkit Configuration Guide

![](/External/ReadMeImages/MRTK_Logo_Rev.png)

The Mixed Reality Toolkit centralizes as much of the configuration required to manage the toolkit as possible (except for true runtime "things").  

> The Mixed Reality Toolkit "locks" the default configuration screens to ensure you always have a common start point for your project and we encourage you to start defining your own settings as your project evolves.

This are provided by a set of inspector screens which all start from the main configuration entry point in the *MixedRealityToolkit" GameObject in your Scene:

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ActiveConfiguration.png)

> All the "default" profiles for the Mixed Reality Toolkit can be found in the SDK project in the following folder

> Assets\MixedRealityToolkit-SDK\Profiles

When you open the main Mixed Reality Toolkit Configuration Profile, you will see the following screen in the inspector:

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_MixedRealityToolkitConfigurationScreen.png)

> If you select a MixedRealityToolkitConfigurationProfile asset without the MixedRealityToolkit in the scene, it will ask you if you want the MRTK to automatically setup the scene for you.  This is optional, however, there must be an active MixedRealityToolkit object in the scene to access all the configuration screens.

This houses the current active runtime configuration for the project.

> Note, almost any profile can be swapped out at runtime, with the exception of the InputActions configuration (see later).  The profiles with then automatically adapt to the new configuration / runtime environment automatically.

From here you can navigate to all the configuration profiles for the MRTK, including:

* [Experience Settings](#)
* [Camera Settings](#)
* [Input System Settings](#)
* [Boundary Settings](#)
* [Teleporting Settings](#)
* [Spatial Awareness Settings](#)
* [Diagnostics Settings](#)
* [Additional Services Settings](#)
* [Input Actions Settings](#)
* [Input Actions Rules](#)
* [Pointer Configuration](#)
* [Gestures Configuration](#)
* [Speech Commands](#)
* [Controller Mapping Configuration](#)
* [Controller Visualization Settings](#)

These configuration profiles are detailed below in their relevant sections:

From here you can navigate to all the configuration profiles for the MRTK, including:

---
## Experience Settings

Located on the main Mixed Reality Toolkit configuration page, this setting defines the default operation for the Mixed Reality environment for your project.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ExperienceSettings.png)


---
## Camera Settings

The camera settings define how the camera will be setup for your Mixed Reality project, defining the generic clipping, quality and transparency settings.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_CameraProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.

---
## Input System Settings

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputSystemSelection.png)


![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputSystemProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
## Boundary Visualization Settings

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_BoundaryVisualizationProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
## Teleportation System Selection

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_TeleportationSystemSelection.png)

---
## Spatial Awareness Settings

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpatialAwarenessSystemSelection.png)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpatialAwarenessProfile.png.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
## Diagnostics Settings

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_DiagnosticsSystemSelection.png)

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_DiagnosticsProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
## Additional Services Settings

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_RegisteredServiceProfidersProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the main Mixed Reality Toolkit Configuration screen.
---
## Input Actions Settings

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputActionsProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
## Input Actions Rules

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputActionRulesProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
## Pointer Configuration

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputPointerProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
## Gestures Configuration

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_GesturesProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
## Speech Commands

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpeechCommandsProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
## Controller Mapping Configuration

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ControllerMappingProfile.png)

The MRTK provides the default configuration for the following controllers / systems:

* Mouse (including 3D spatial mouse support)
* Touch Screen
* Xbox controllers
* Windows Mixed Reality controllers
* HoloLens Gestures
* HTC Vive wand controllers
* Oculus Touch controllers
* Oculus Remote controller
* Generic OpenVR devices (advanced users only)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.
---
## Controller Visualization Settings

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ControllerVisualizationProfile.png)

> Clicking on the "Back to Configuration Profile" button will take you back to the Mixed Reality Toolkit Input System Settings screen.