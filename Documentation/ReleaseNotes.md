# Microsoft Mixed Reality Toolkit release notes

- [What's new](#whats-new-in-240)
- [Breaking changes](#breaking-changes-in-240)
- [Updating guidance](Updating.md#upgrading-to-a-new-version-of-mrtk)

This release of the Microsoft Mixed Reality Toolkit supports the following devices and platforms.

- Microsoft HoloLens 2
- Microsoft HoloLens (1st gen)
- Windows Mixed Reality Immersive headsets
- OpenVR
- (Experimental) Unity 2019.3 XR platform
- Mobile AR via Unity AR Foundation
  - Android
  - iOS

The following software is required.

- [Microsoft Visual Studio](https://visualstudio.microsoft.com) (2017 or 2019) Community Edition or higher
- [Windows 10 SDK](https://developer.microsoft.com/windows/downloads/windows-10-sdk) 18362 or later (installed by the Visual Studio Installer)
- [Unity](https://unity3d.com/get-unity/download) 2018.4 LTS or 2019 (2019.3 recommended)

**NuGet requirements**

If importing the [Mixed Reality Toolkit NuGet packages](MRTKNuGetPackage.md), the following software is recommended.

- [NuGet for Unity 2.0.0 or newer](https://github.com/GlitchEnzo/NuGetForUnity/releases/latest)

### What's new in 2.4.0

**MRTK folder layout changes**

This version of MRTK modifies the layout of the MRTK folder structure. This change encapsulates all MRTK code into a single folder hierarchy and reduces the total path length of all MRTK files.

| Previous Folder | New Folder |
| --- | --- |
| MixedRealityToolkit | MRTK\Core |
| MixedRealityToolkit.Examples | MRTK\Examples |
| MixedRealityToolkit.Extensions | MRTK\Extensions |
| MixedRealityToolkit.Providers | MRTK\Providers |
| MixedRealityToolkit.SDK | MRTK\SDK |
| MixedRealityToolkit.Services | MRTK\Services |
| MixedRealityToolkit.Tests | MRTK\Tests |
| MixedRealityToolkit.Tools | MRTK\Tools |

> [!IMPORTANT]
> The `MixedRealityToolkit.Generated` contains customer generated files and remains unchanged.

**MRTK Toolbox**

![MRTK Toolbox](Images/Tools/MRTKToolboxWindow.png)

The [MRTK Toolbox](README_Toolbox.md) is a Unity editor window utility that makes it easy to discover and spawn MRTK UX prefab components into the current scene. Items can be filtered in view by using the search bar at the top of the window. The toolbox window is designed to spawn MRTK out-of-box prefabs into the current scene.

**Button Config Helper added to Pressable Buttons**
![Button Config Helper](https://user-images.githubusercontent.com/9789716/70167111-e3175600-167a-11ea-9c52-444509c06105.gif)
This new feature makes it easy to change the icon and text of the buttons. Icon supports quad, sprite, and TextMesh Pro's SDF font texture. See MRTK's [Button documentation](README_Button.md#how-to-change-the-icon-and-text) for the details.

**New HoloLens 2-style Toggle Buttons - Checkbox, Switch, Radio**
<br/><img src="https://user-images.githubusercontent.com/13754172/75299797-df631d80-57ea-11ea-8857-8ef647df0aca.gif" width="450">
<br/><img src="https://user-images.githubusercontent.com/13754172/75299783-d6724c00-57ea-11ea-88b1-85e4a585212f.gif" width="450">

**Hand Menu Improvements**

Hand menu has been adapted in many applications. One of the biggest issue we found is the accidental false activation while manipulating objects or interacting with other content, etc. New 'Gaze Activation' option added to HandConstraintPalmUp.cs to prevent false activations. With this option, the menu does not accidentally show up, until the user look at the hand.<br/>
![0416_HandMenu_02](https://user-images.githubusercontent.com/13754172/79507261-4aabbd80-7fec-11ea-95c4-6e3f4bd18c11.gif)

**Hand Menu Examples update**

[New] Large menu interaction example 1: Grab & Pull menu to world-lock<br/>
![0416_HandMenu_03](https://user-images.githubusercontent.com/13754172/79507983-90b55100-7fed-11ea-9062-630c892950cb.gif)

[New] Large menu interaction example 2 - Auto world-lock on hand drop<br/>
![0416_HandMenu_04](https://user-images.githubusercontent.com/13754172/79508227-f9043280-7fed-11ea-995f-ac3cfe42fe65.gif)

**Dialog (Experimental)**
<br/><img src="Images/Dialog/MRTK_UX_Dialog_Main.png" width="450">

Dialog UI has been ported over from HoloToolkit with new HoloLens 2 shell-style design updates.

**Dock (Experimental)**
<br/><img src="https://user-images.githubusercontent.com/621574/76669327-65e86080-6548-11ea-85a3-f84f6b367f97.gif" width="450">

This control enables moving objects in and out of predetermined positions, to create palettes, shelves and navigation bars.

**Unity Profiler markers**

This version of MRTK has added Unity Profiler markers to the input system and data providers. These markers provide detailed information on where time is spent in
the MRTK input system that can be used to help optimize applications.

Markers take the format of "[MRTK] ClassWithoutNamespace.Method".

![Profiler Markers](Images/ReleaseNotes/ProfilerMarkers.png)

**WindowsApiChecker: IsMethodAvailable(), IsPropertyAvailable() and IsTypeAvailable()**

This version of MRTK adds three new methods to the [`WindowsApiChecker`](xref:Microsoft.MixedReality.Toolkit.Windows.Utilities.WindowsApiChecker) class: `IsMethodAvailable`, `IsPropertyAvailable` and `IsTypeAvailable`. These methods allow for checking for feature support on Windows 10 and are prefered over using the `UniversalApiContractV#_IsAvailable` properties.

**Helpers to get text input fields working with MixedRealityKeyboard for UnityUI, TextMeshPro (Experimental)**

<img src="https://user-images.githubusercontent.com/168492/77582981-86e07800-6e9d-11ea-86e5-bf2c0840296c.png" width="300" />

We have introduced two helper components, [`UI_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.UI_KeyboardInputField) and [`TMP_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.TMP_KeyboardInputField) that can be added to text input fields in Unity UI to enable the HoloLens 2 and Windows Mixed Reality Keyboard to show up when the fields are clicked.

For more information, see - [Mixed Reality Keyboard Helpers](../Assets/MRTK/SDK/Experimental/MixedRealityKeyboard/README_MixedRealityKeyboard.md).

**Grid Object Collection Alignment Options**

<img src="https://user-images.githubusercontent.com/39840334/79289136-5c228780-7e7d-11ea-82b4-07959e42c3ed.gif" width="300" />

We have added the ability to choose how the elements in the grid are aligned, whether they are aligned in the center or along the left/right axis (top/bottom axis when doing row then column layout)

**Grid Object Collection Anchor Changes**

<img src="https://user-images.githubusercontent.com/39840334/79516745-17bff480-8001-11ea-8492-cfa953c451da.gif" width="300" />

We made changes to Grid Object Collection behavior to be more in line with Unity's layout group behaviors by aligning the anchor along an object's central axis. The old Grid Object Collection behavior can be toggled with the `AnchorAlongAxis` field.

**Adjusted input simulation camera control**

Camera control speed using in-editor input simulation is slower for a smoother experience and is now untied from framerate. Fast camera control now activated with Right Shift instead of Right Ctrl

**Hands-free GGV input simulation**

<img src="https://user-images.githubusercontent.com/39840334/79164615-40908180-7d96-11ea-8195-6be34d4df8d6.gif" width="300"/>

We've enabled the ability to interact with objects without bringing hands within the in-editor input simulation service. Rotate the camera so that the gaze cursor is over an interactable object, and click on the left mouse button to interact with it.

**Audio Spatializer Selection in MRTK configuration dialog**

The audio spatializer can now be specified in the MRTK configuration dialog. Installing new spatializers, such as the [Microsoft Spatializer](https://www.nuget.org/packages/Microsoft.SpatialAudio.Spatializer.Unity/), will re-prompt to allow for easy selection.

![MRTK Configuration Select Spatializer](Images/ReleaseNotes/SpatializerSelection.png)

### Breaking changes in 2.4.0

**Eye gaze setup change**

This version of MRTK modifies the steps required for eye gaze setup. The _'IsEyeTrackingEnabled'_ checkbox can be found in the gaze settings of the input pointer profile. Checking this box will enable eye based gaze, rather then the default head based gaze.

For more information on these changes and complete instructions for eye tracking setup, please see the [eye tracking](EyeTracking/EyeTracking_BasicSetup.md) article.
