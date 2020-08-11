# Mixed Reality Toolkit packages

The Microsoft Mixed Reality Toolkit is provided as a collection of packages.

- [Asset packages](#asset-packages)
- [Unity Package Manager](#unity-package-manager)

## Asset packages

For developers using Unity 2018.4, or newer, MRTK is distributed as a collection of asset (.unitypackage) packages. The contents of these packages are described in the sections that follow.

- [Foundation](#foundation)
- [Extensions](#extensions)
- [Tools](#tools)
- [Test Utilities](#test-utilities)
- [Examples](#examples)

### Foundation

The Microsoft.MixedRealityToolkit.Unity.Foundation package includes the core components required to create a mixed reality application.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Core | | Interface and type definitions, base classes, standard shader. |
| MRTK/Core/Providers | | Platform agnostic data providers |
| | Hands | Base class support and services for hand tracking. |
| | InputAnimation | Input recording and playback. |
| | [InputSimulation](InputSimulation/InputSimulationService.md) | In-editor input simulation, including hand and eye tracking. |
| | [ObjectMeshObserver](SpatialAwareness/SpatialObjectMeshObserver.md) | Spatial awareness observer using a 3D model as the data. |
| | UnityInput | Common input devices (joystick, mouse, etc.) implemented via Unity's input API. |
| MRTK/Providers | | Platform specific data providers |
| | LeapMotion | Support for the UltraLeap Leap Motion controller. |
| | OpenVR | Support for OpenVR devices. |
| | Oculus | Support for Oculus devices, such as the Quest. |
| | [UnityAR](CameraSystem/UnityArCameraSettings.md) | (Experimental) Camera settings provider enabling MRTK use with mobile AR devices. |
| | WindowsMixedReality | Support for Windows Mixed Reality devices, including Microsoft HoloLens and immersive headsets. |
| | Windows | Support for Microsoft Windows specific APIs, for example speech and dictation. |
| | XR SDK | (Experimental) Support for [Unity's new XR framework](https://blogs.unity3d.com/2020/01/24/unity-xr-platform-updates/) in Unity 2019.3 and newer. |
| MRTK/SDK | | |
| | Experimental | Experimental features, including shaders, user interface controls and individual system managers. |
| | Features | Functionality that builds upon the Foundation package. |
| | Profiles | Default profiles for the Microsoft Mixed Reality Toolkit systems and services. |
| | StandardAssets | Common assets; models, textures, materials, etc. |
| MRTK/Services | | |
| | [BoundarySystem](Boundary/BoundarySystemGettingStarted.md) | System implementing VR boundary support. |
| | [CameraSystem](CameraSystem/CameraSystemOverview.md) | System implementing camera configuration and management. |
| | [DiagnosticsSystem](Diagnostics/DiagnosticsSystemGettingStarted.md) | System implementing in application diagnostics, for example a visual profiler. |
| | [InputAnimation](InputSimulation/InputAnimationRecording.md) | Support for recording head movement and hand tracking data. |
| | [InputSimulation](InputSimulation/InputSimulationService.md) | Support for in-editor simulation of hand and eye input. |
| | [InputSystem](Input/Overview.md) | System providing support for accessing and handling user input. |
| | [SceneSystem](SceneSystem/SceneSystemGettingStarted.md) | System providing multi-scene application support. |
| | [SpatialAwarenessSystem](SpatialAwareness/SpatialAwarenessGettingStarted.md) | System providing support for awareness of the user's environment. |
| | [TeleportSystem](TeleportSystem/Overview.md) | System providing support for teleporting (moving about the experience in jumps). |
| MRTK/StandardAssets | | MRTK Standard shader, basic materials and other standard assets for mixed reality experiences |

### Extensions

The optional Microsoft.MixedRealityToolkit.Unity.Extensions package includes additional services that extend the functionality of the Microsoft Mixed Reality Toolkit.

> [!NOTE]
> The extensions package requires Microsoft.MixedRealityToolkit.Unity.Foundation.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Extensions | |
| | HandPhysicsService | Service that adds physics support to articulated hands. |
| | LostTrackingService | Service that simplifies handing of tracking loss on Microsoft HoloLens devices. |
| | [SceneTransitionService](Extensions/SceneTransitionService/SceneTransitionServiceOverview.md) | Service that simplifies adding smooth scene transitions. |

### Tools

The optional Microsoft.MixedRealityToolkit.Unity.Tools package includes helpful tools that enhance the mixed reality development experience using the Microsoft Mixed Reality Toolkit.
These tools are located in the **Mixed Reality Toolkit > Utilities** menu in the Unity Editor.

> [!NOTE]
> The tools package requires Microsoft.MixedRealityToolkit.Unity.Foundation.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tools | |
| | [DependencyWindow](Tools/DependencyWindow.md) | Tool that creates a dependency graph of assets in a project. |
| | [ExtensionServiceCreator](Tools/ExtensionServiceCreationWizard.md) | Wizard to assist in creating extension services. |
| | [OptimizeWindow](Tools/OptimizeWindow.md) | Utility to help automate configuring a mixed reality project for the best performance in Unity. |
| | ReserializeAssetsUtility | Provides support for reserializing specific Unity files. |
| | [RuntimeTools/Tools/ControllerMappingTool](Tools/ControllerMappingTool.md) | Utility enabling developers to quickly determine Unity mappings for hardware controllers. |
| | ScreenshotUtility | Enables capturing application images in the Unity editor. |
| | TextureCombinerWindow | Utility to combine graphics textures. |

### Test Utilities

The optional Microsoft.MixedRealityToolkit,.TestUtilities package includes a collection of utilities used by the Mixed Reality team to author unit and other validation tests.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tests | |
| | TestUtilities | Methods to simplify creation of play mode tests, including hand simulation utilities. |

### Examples

The optional Microsoft.MixedRealityToolkit.Unity.Examples package includes demonstration projects that illustrate the features of the Microsoft Mixed Reality Toolkit.

> [!NOTE]
> The examples package requires Microsoft.MixedRealityToolkit.Unity.Foundation.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Examples | | |
| | Demos | Simple scenes illustrating one or two related features. |
| | Experimental | Demo scenes illustrating experimental features. |
| | Inspectors | Unity Editor inspectors used by demo scenes. |
| | StandardAssets | Common assets shared by multiple demo scenes. |

## Unity Package Manager

When using Unity 2019.4, or newer, the Microsoft Mixed Reality Toolkit is available using the Unity Package Manager. The following sections describe the contents of these packages.

- [Foundation](#foundation-upm)
- [Standard Assets](#standard-assets-upm)
- [Extensions](#extensions-upm)
- [Tools](#tools-upm)
- [Test Utilities](#test-utilities-upm)
- [Examples](#examples-upm)

### Foundation (UPM)

The Unity Package Manager version of the MRTK foundation package (`com.microsoft.mixedreality.toolkit.foundation`) contains most of the same content as the [asset package version](#foundation). 

One notable exception is the MRTK/StandardAssets folder. This folder has been moved into the new [standard assets package](#standard-assets-upm).

> [!Note]
> When importing the foundation package using the Unity Package Manager, the standard assets package will also be imported.

### Standard Assets (UPM)

The standard assets package (`com.microsoft.mixedreality.toolkit.standardassets`) is exclusive to the MRTK Unity Package Manager distribution. 

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Examples | | |
| | Audio | Sound files that provide common user interface action acknowledgement. |
| | Fonts | Font files that closely match the typeface used in the Microsoft HoloLens and Windows Mixed Reality shell experiences. |
| | FontsSDFTextures | Textures used to produce high quality fonts when scaled. |
| | Icons | Common icons used in the Mixed Reality Toolkit. |
| | Materials | Basic materials (ex: solid and translucent colors) that use the MRTK shaders. |
| | Shaders | Collection of shaders optimized for mixed reality devices. |
| | Textures | Common textures used in the Mixed Reality Toolkit. |

> [!Note]
> Standard assets is a stand alone component that can be used independently from the other MRTK packages. It is required by the [foundation](#foundation-upm) and will be included when any MRTK package is imported.

### Extensions (UPM)

The extensions UPM package (`com.microsoft.mixedreality.toolkit.extensions`) contains the same content as the [extensions asset package](#extensions). 

> [!Note]
> When importing the extensions UPM package, sample scenes are not imported by default.

### Tools (UPM)

The tools UPM package (`com.microsoft.mixedreality.toolkit.tools`) contains the same content as the [tools asset package](#tools). 

### Test Utilities (UPM)

The test utilities UPM package (`com.microsoft.mixedreality.toolkit.testutilities`) contains the same content as the [test utilities asset package](#test-utilities). 

### Examples (UPM)

The examples UPM package (`com.microsoft.mixedreality.toolkit.examples`) contains the same content as the [examples asset package](#examples). 

> [!Note]
> When importing the examples UPM package, sample scenes are not imported by default.

## See also

- [Getting Started with the MRTK](GettingStartedWithTheMRTK.md)
- [NuGet Packaging](MRTKNuGetPackage.md)
