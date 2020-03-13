# Mixed Reality Toolkit packages

The Microsoft Mixed Reality Toolkit is provided as a collection of packages. The contents of these packages is described in the following sections.

- [Foundation](#foundation)
- [Extensions](#extensions)
- [Tools](#tools)
- [Examples](#examples)

## Foundation

The Microsoft.MixedRealityToolkit.Unity.Foundation package includes the core components required to create a mixed reality application.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Core | | Interface and type definitions, base classes, standard shader. |
| MRTK/Providers | | |
| | [ObjectMeshObserver](SpatialAwareness/SpatialObjectMeshObserver.md) | Spatial awareness observer using a 3D model as the data. |
| | OpenVR | Support for OpenVR devices. |
| | [UnityAR](CameraSystem/UnityArCameraSettings.md) | (Experimental) Camera settings provider enabling MRTK use with mobile AR devices. |
| | WindowsMixedReality | Support for Windows Mixed Reality devices, including Microsoft HoloLens and immersive headsets. |
| | WindowsVoiceInput | Support for speech and dictation on Microsoft Windows platforms. |
| | XRSDK | (Experimental) Support for [Unity's new XR framework](https://blogs.unity3d.com/2020/01/24/unity-xr-platform-updates/) in Unity 2019.3. |
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

## Extensions

The optional Microsoft.MixedRealityToolkit.Unity.Extensions package includes additional services that extend the functionality of the Microsoft Mixed Reality Toolkit.

> [!NOTE]
> The extensions package requires Microsoft.MixedRealityToolkit.Unity.Foundation.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Extensions | |
| | HandPhysicsService | Service that adds physics support to articulated hands. |
| | LostTrackingService | Service that simplifies handing of tracking loss on Microsoft HoloLens devices. |
| | [SceneTransitionService](Extensions/SceneTransitionService/SceneTransitionServiceOverview.md) | Service that simplifies adding smooth scene transitions. |

## Tools

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

## Examples

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

## See also

- [Getting Started with the MRTK](GettingStartedWithTheMRTK.md)
- [NuGet Packaging](MRTKNuGetPackage.md)
