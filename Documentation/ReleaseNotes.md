# Microsoft Mixed Reality Toolkit release notes

- [What's new](#whats-new-in-240)
- [Breaking changes](#breaking-changes-in-240)
- [Updating guidance](Updating.md#upgrading-to-a-new-version-of-mrtk)
- [Known issues](#known-issues-in-240)

This release of the Microsoft Mixed Reality Toolkit supports the following devices and platforms.

- Microsoft HoloLens 2
- Microsoft HoloLens (1st gen)
- Windows Mixed Reality Immersive headsets
- OpenVR
- (Experimental) Unity 2019.3 XR platform
- Mobile AR via Unity AR Foundation
  - Android
  - iOS
- Ultraleap Hand Tracking

The following software is required.

- [Microsoft Visual Studio](https://visualstudio.microsoft.com) (2017 or 2019) Community Edition or higher
- [Windows 10 SDK](https://developer.microsoft.com/windows/downloads/windows-10-sdk) 18362 or later (installed by the Visual Studio Installer)
- [Unity](https://unity3d.com/get-unity/download) 2018.4 LTS or 2019 (2019.3 recommended)

**NuGet requirements**

If importing the [Mixed Reality Toolkit NuGet packages](MRTKNuGetPackage.md), the following software is recommended.

- [NuGet for Unity 2.0.0 or newer](https://github.com/GlitchEnzo/NuGetForUnity/releases/latest)

### What's new in 2.4.0

**Ultraleap Hand Tracking Support**

The [Leap Motion Data Provider](CrossPlatform/LeapMotionMRTK.md) enables articulated hand tracking for VR applications and is also useful for rapid prototyping in the editor.  The data provider can be configured to use the Leap Motion Controller mounted on a headset or placed on a desk face up.

A [Leap Motion Controller](https://www.ultraleap.com/product/leap-motion-controller/) is required to use this data provider.

![LeapMotionIntroGif](Images/CrossPlatform/LeapMotion/LeapMotionSideBySide2.gif)

**Migration window**

![Migration window](Images/MigrationWindow/MRTK_Migration_Window.png)

MRTK now comes with a migration tool that will help you upgrade deprecated components to their newer
versions and to keep existing code working even as MRTK makes breaking changes.

**It is generally recommended to run the migration tool after pulling a new version of MRTK** to
ensure that as much of your project will be auto-adjusted to the latest MRTK code.

The [migration window](Tools/MigrationWindow.md) can be found in 'Mixed Reality Toolkit > Utilities >
Migration Window'. It it part of the **Tools** package.

It currently supports:

- Upgrading ManipulationHandler and BoundingBox to their newer versions ObjectManipulator and BoundsControl.
- Updating custom button icons to work correctly with the new Button Config Helper.

Note that BoundsControl is still in experimental phase and therefore API or properties might still change in the next version.

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

**Tap to Place**

[Tap to Place](README_TapToPlace.md) is a far interaction component used to easily place a game object on surface. Tap to Place uses a combination of two clicks and head movement to place an object.

![TapToPlace](Images/Solver/TapToPlace/TapToPlaceIntroGif.gif)

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

This version of MRTK adds three new methods to the [`WindowsApiChecker`](xref:Microsoft.MixedReality.Toolkit.Windows.Utilities.WindowsApiChecker) class: `IsMethodAvailable`, `IsPropertyAvailable` and `IsTypeAvailable`. These methods allow for checking for feature support on Windows 10 and are preferred over using the `UniversalApiContractV#_IsAvailable` properties.

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

**Button Config Helper**

<img src="https://user-images.githubusercontent.com/168492/81211778-bb5d4e80-8f88-11ea-94c7-33cf265586df.png" width="300" />

The Button Config Helper is an editor feature that makes it easier to customize MRTK buttons. It's now much easier to:

- Update the button label text
- Add a button click event listener
- Change the button icon

**Audio Spatializer Selection in MRTK configuration dialog**

The audio spatializer can now be specified in the MRTK configuration dialog. Installing new spatializers, such as the [Microsoft Spatializer](https://www.nuget.org/packages/Microsoft.SpatialAudio.Spatializer.Unity/), will re-prompt to allow for easy selection.

![MRTK Configuration Select Spatializer](Images/ReleaseNotes/SpatializerSelection.png)

**Object manipulator graduated to SDK**

![Object manipulator](../Documentation/Images/ManipulationHandler/MRTK_Manipulation_Main.png)

ObjectManipulator now graduated to SDK and is no longer an experimental feature. This control is replacing the existing ManipulationHandler class which is now deprecated. ObjectManipulator comes with a new more flexible constraint system and correctly responds to physics. A full feature list and guide how to set up can be found in [object manipulator documentation](README_ObjectManipulator.md).
Users can take advantage of the new [migration window](Tools/MigrationWindow.md) to upgrade their existing gameobject using ManipulationHandler to ObjectManipulator

**Bounds control improvements**

We extensively increased test coverage for bounds control this version and addressed one of the biggest pain points of users of bounding box: bounds control will now no longer recreate its visuals on configuration changes. Also it now supports reconfiguring any property during runtime. Also the properties DrawTetherWhenManipulating and HandlesIgnoreCollider are now configurable per handle type.

### Breaking changes in 2.4.0

**Eye Gaze API**

The `UseEyeTracking` property from `GazeProvider` implementation of `IMixedRealityEyeGazeProvider` was renamed to `IsEyeTrackingEnabled`.

If you did this previously...

```csharp
if (CoreServices.InputSystem.GazeProvider is GazeProvider gazeProvider)
{
    gazeProvider.UseEyeTracking = true;
}
```

Do this now...

```csharp
if (CoreServices.InputSystem.GazeProvider is GazeProvider gazeProvider)
{
    gazeProvider.IsEyeTrackingEnabled = true;
}
```

**Eye gaze setup**

This version of MRTK modifies the steps required for eye gaze setup. The _'IsEyeTrackingEnabled'_ checkbox can be found in the gaze settings of the input pointer profile. Checking this box will enable eye based gaze, rather then the default head based gaze.

For more information on these changes and complete instructions for eye tracking setup, please see the [eye tracking](EyeTracking/EyeTracking_BasicSetup.md) article.

### Known issues in 2.4.0

**MRTK Configurator dialog does not show 'Enable MSBuild for Unity' in Unity 2019.3**

An issue exists where enabling MSBuild for Unity in 2019.3 may result in an infinite loop restoring packages ([#7239](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7239)).

As a workaround, the Microsoft.Windows.DotNetWinRT package can be imported using [NuGet for Unity](https://github.com/GlitchEnzo/NuGetForUnity/releases/latest).

**Duplicate Assembly Version and Multiple Precompiled Assemblies Unity 2018.4**

If the platform is switched from Standalone to UWP and then back to Standalone in Unity 2018.4, the following errors might be in the console:

```
PrecompiledAssemblyException: Multiple precompiled assemblies with the same name Microsoft.Windows.MixedReality.DotNetWinRT.dll included for the current platform. Only one assembly with the same name is allowed per platform. Assembly paths
```

```
Assets\MRTK\Examples\Demos\HandTracking\Scenes\Utilities\InspectorFields\AssemblyInfo.cs(6,12): error CS0579: Duplicate 'AssemblyVersion' attribute
```

These errors are due to issues in the deletion process with MSBuildForUnity.  To resolve the issue, while in Standalone, delete the Dependencies folder at the root of Assets and restart unity.

For a more details see [Issue 7948](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7948).

**Applications appearing as a 2D slate on Unity 2019.3**

When using Unity 2019.3, enabling XR support does not configure a default SDK (legacy) or plugin (XR Mangement). This results in applications being constrained to a 2D slate. Details on resolving this can be found in the [Building and Deploying MRTK article](BuildAndDeploy.md#unity-20193-and-hololens).

**Unity 2019.3: ARM build architecture**

There is a [known issue](https://issuetracker.unity3d.com/issues/enabling-graphics-jobs-in-2019-dot-3-x-results-in-a-crash-or-nothing-rendering-on-hololens-2) in Unity 2019.3 that causes errors when selecting ARM as the build architecture in Visual Studio. The recommended work around is to build for ARM64. If that is not an option, please disable **Graphics Jobs** in **Edit** > **Project Settings** > **Player** > **Other Settings**. For more information see [Building and Deploying](BuildAndDeploy.md#unity-20193-and-hololens).

**Runtime profile swapping**

MRTK does not fully support profile swapping at runtime. This feature is being investigated for a future release. Please see issues [4289](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4289), [5465](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/5465) and [5466](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/5466) for more information.

**Unity 2018: .NET Backend and AR Foundation**

There is an issue in Unity 2018 where, building a Universal Windows Platform project using the .NET scripting backend, the Unity AR Foundation package will fail.

To work around this issue, please perform one of the following steps:

- Switch the scripting backend to IL2CPP
- In the Build Settings window, uncheck **Unity C# Projects"
