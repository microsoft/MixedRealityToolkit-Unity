# Mixed Reality Toolkit packages

The Mixed Reality Toolkit (MRTK) is a collection of packages that enable cross platform Mixed Reality application development by providing support for Mixed Reality hardware and platforms.

MRTK is available as [asset](#asset-packages) (.unitypackage) packages and via the [Unity Package Manager](#unity-package-manager).


## Asset packages

The MRTK asset (.unitypackage) can be downloaded from [GitHub](https://github.com/microsoft/MixedRealityToolkit-Unity/releases). 

Some of the benefits of using asset packages include:

- Available for Unity 2018.4 and newer
- Easy to make changes to MRTK
  - MRTK is in the Assets folder

Some of the challenges are:

- MRTK is part of the project's Assets folder, leading to
  - Larger projects
  - Slower compilation times
- No dependency management
  - Customers are required to resolve package dependencies manually
- Manual update process
  - Multiple steps
  - Large (3000+ file) source control updates
  - Risk of losing changes made to MRTK 
- Importing the examples package typically means including all examples

The available packages are:

- [Foundation](#foundation-package)
- [Extensions](#extensions-package)
- [Tools](#tools-package)
- [Test utilities](#test-utilities-package)
- [Examples](#examples-package)

These packages are released and supported by Microsoft from source code in the [mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release) branch on GitHub.

### Foundation package

The Mixed Reality Toolkit Foundation is the set of code that enables your application to leverage common functionality across Mixed Reality Platforms.

<img src="../../Documentation/Images/Input/MRTK_Package_Foundation.png" width="350px" style="display:block;">  
<sup>MRTK Foundation Package</sup>

The MRTK Foundation package contains the following.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Core | | Interface and type definitions, base classes, standard shader. |
| MRTK/Core/Providers | | Platform agnostic data providers |
| | Hands | Base class support and services for hand tracking. |
| | [InputAnimation](../InputSimulation/InputAnimationRecording.md) | Support for recording head movement and hand tracking data. |
| | [InputSimulation](../InputSimulation/InputSimulationService.md) | Support for in-editor simulation of hand and eye input. |
| | [ObjectMeshObserver](../SpatialAwareness/SpatialObjectMeshObserver.md) | Spatial awareness observer using a 3D model as the data. |
| | UnityInput | Common input devices (joystick, mouse, etc.) implemented via Unity's input API. |
| MRTK/Providers | | Platform specific data providers |
| | LeapMotion | Support for the UltraLeap Leap Motion controller. |
| | OpenVR | Support for OpenVR devices. |
| | Oculus | Support for Oculus devices, such as the Quest. |
| | [UnityAR](../CameraSystem/UnityArCameraSettings.md) | (Experimental) Camera settings provider enabling MRTK use with mobile AR devices. |
| | WindowsMixedReality | Support for Windows Mixed Reality devices, including Microsoft HoloLens and immersive headsets. |
| | Windows | Support for Microsoft Windows specific APIs, for example speech and dictation. |
| | XR SDK | (Experimental) Support for [Unity's new XR framework](https://blogs.unity3d.com/2020/01/24/unity-xr-platform-updates/) in Unity 2019.3 and newer. |
| MRTK/SDK | | |
| | Experimental | Experimental features, including shaders, user interface controls and individual system managers. |
| | Features | Functionality that builds upon the Foundation package. |
| | Profiles | Default profiles for the Microsoft Mixed Reality Toolkit systems and services. |
| | StandardAssets | Common assets; models, textures, materials, etc. |
| MRTK/Services | | |
| | [BoundarySystem](../Boundary/BoundarySystemGettingStarted.md) | System implementing VR boundary support. |
| | [CameraSystem](../CameraSystem/CameraSystemOverview.md) | System implementing camera configuration and management. |
| | [DiagnosticsSystem](../Diagnostics/DiagnosticsSystemGettingStarted.md) | System implementing in application diagnostics, for example a visual profiler. |
? | [InputSystem](../Input/Overview.md) | System providing support for accessing and handling user input. |
| | [SceneSystem](../SceneSystem/SceneSystemGettingStarted.md) | System providing multi-scene application support. |
| | [SpatialAwarenessSystem](../SpatialAwareness/SpatialAwarenessGettingStarted.md) | System providing support for awareness of the user's environment. |
| | [TeleportSystem](../TeleportSystem/Overview.md) | System providing support for teleporting (moving about the experience in jumps). |
| MRTK/StandardAssets | | MRTK Standard shader, basic materials and other standard assets for mixed reality experiences |

### Extensions package

The optional Microsoft.MixedRealityToolkit.Unity.Extensions package includes additional services that extend the functionality of the Microsoft Mixed Reality Toolkit.

> [!NOTE]
> The extensions package requires Microsoft.MixedRealityToolkit.Unity.Foundation.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Extensions | |
| | [HandPhysicsService](../Extensions/HandPhysicsService/HandPhysicsServiceOverview.md) | Service that adds physics support to articulated hands. |
| | LostTrackingService | Service that simplifies handling of tracking loss on Microsoft HoloLens devices. |
| | [SceneTransitionService](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md) | Service that simplifies adding smooth scene transitions. |

### Tools package

The optional Microsoft.MixedRealityToolkit.Unity.Tools package includes helpful tools that enhance the mixed reality development experience using the Microsoft Mixed Reality Toolkit.
These tools are located in the **Mixed Reality Toolkit > Utilities** menu in the Unity Editor.

> [!NOTE]
> The tools package requires Microsoft.MixedRealityToolkit.Unity.Foundation.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tools | |
| | BuildWindow | Tool that helps simplify the process of building and deploying UWP applications. |
| | [DependencyWindow](../Tools/DependencyWindow.md) | Tool that creates a dependency graph of assets in a project. |
| | [ExtensionServiceCreator](../Tools/ExtensionServiceCreationWizard.md) | Wizard to assist in creating extension services. |
| | [MigrationWindow](../Tools/MigrationWindow.md) | Tool that assists in updating code that uses deprecated MRTK components.  |
| | [OptimizeWindow](../Tools/OptimizeWindow.md) | Utility to help automate configuring a mixed reality project for the best performance in Unity. |
| | ReserializeAssetsUtility | Provides support for reserializing specific Unity files. |
| | [RuntimeTools/Tools/ControllerMappingTool](../Tools/ControllerMappingTool.md) | Utility enabling developers to quickly determine Unity mappings for hardware controllers. |
| | ScreenshotUtility | Enables capturing application images in the Unity editor. |
| | TextureCombinerWindow | Utility to combine graphics textures. |
| | [Toolbox](../README_Toolbox.md) | UI that makes it easy to discover and use MRTK UX components. |

### Test utilities package

The optional Microsoft.MixedRealityToolkit.TestUtilities package is a collection of helper scripts that enable developers to easily [create play mode tests](../Contributing/UnitTests.md#play-mode-tests). These utilities are especially useful for developers creating MRTK components.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tests | |
| | TestUtilities | Methods to simplify creation of play mode tests, including hand simulation utilities. |

### Examples package

The examples package contains demos, sample scripts, and sample scenes that exercise functionality in the foundation package. This package contains the [HandInteractionExample scene](../README_HandInteractionExamples.md) (pictured below) which contains sample objects
that respond to various types of hand input (articulated and non-articulated).

![HandInteractionExample scene](../Images/MRTK_Examples.png)

This package also contains eye tracking demos, which are [documented here](../EyeTracking/EyeTracking_ExamplesOverview.md)

More generally, any new feature in the MRTK should contain a corresponding example in the examples package, roughly following
the same folder structure and location.

> [!NOTE]
> The examples package requires Microsoft.MixedRealityToolkit.Unity.Foundation.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Examples | | |
| | Demos | Simple scenes illustrating one or two related features. |
| | Experimental | Demo scenes illustrating experimental features. |
| | StandardAssets | Common assets shared by multiple demo scenes. |

## Unity Package Manager

For experiences being created using Unity 2019.4 and newer, the MRTK is available via the [Unity Package Manager](https://docs.unity3d.com/Manual/Packages.html).

Some of the benefits of using asset packages include:

- Smaller projects
  - Cleaner Visual Studio solutions
  - Fewer files to check in (MRTK is a simple reference in the `Packages/manifest.json` file)
- Faster compilation
  - Unity does not need to recompile MRTK during building
- Dependency resolution
  - Required MRTK packages are automatically installed when specifying packages with dependencies
- Easy update to new MRTK versions
  - Change the version in the `Packages/manifest.json` file

Some of the challenges are:

- MRTK is immutable
  - Cannot make changes without them being removed during package resolution
- MRTK does not support UPM packages with Unity 2018.4

### Foundation package

The foundation package (`com.microsoft.mixedreality.toolkit.foundation`) forms the basis of the Mixed Reality Toolkit. 

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Core | | Interface and type definitions, base classes, standard shader. |
| MRTK/Core/Providers | | Platform agnostic data providers |
| | Hands | Base class support and services for hand tracking. |
| | [InputAnimation](../InputSimulation/InputAnimationRecording.md) | Support for recording head movement and hand tracking data. |
| | [InputSimulation](../InputSimulation/InputSimulationService.md) | Support for in-editor simulation of hand and eye input. |
| | [ObjectMeshObserver](../SpatialAwareness/SpatialObjectMeshObserver.md) | Spatial awareness observer using a 3D model as the data. |
| | UnityInput | Common input devices (joystick, mouse, etc.) implemented via Unity's input API. |
| MRTK/Providers | | Platform specific data providers |
| | LeapMotion | Support for the UltraLeap Leap Motion controller. |
| | OpenVR | Support for OpenVR devices. |
| | Oculus | Support for Oculus devices, such as the Quest. |
| | [UnityAR](../CameraSystem/UnityArCameraSettings.md) | (Experimental) Camera settings provider enabling MRTK use with mobile AR devices. |
| | WindowsMixedReality | Support for Windows Mixed Reality devices, including Microsoft HoloLens and immersive headsets. |
| | Windows | Support for Microsoft Windows specific APIs, for example speech and dictation. |
| | XR SDK | (Experimental) Support for [Unity's new XR framework](https://blogs.unity3d.com/2020/01/24/unity-xr-platform-updates/) in Unity 2019.3 and newer. |
| MRTK/SDK | | |
| | Experimental | Experimental features, including shaders, user interface controls and individual system managers. |
| | Features | Functionality that builds upon the Foundation package. |
| | Profiles | Default profiles for the Microsoft Mixed Reality Toolkit systems and services. |
| | StandardAssets | Common assets; models, textures, materials, etc. |
| MRTK/Services | | |
| | [BoundarySystem](../Boundary/BoundarySystemGettingStarted.md) | System implementing VR boundary support. |
| | [CameraSystem](../CameraSystem/CameraSystemOverview.md) | System implementing camera configuration and management. |
| | [DiagnosticsSystem](../Diagnostics/DiagnosticsSystemGettingStarted.md) | System implementing in application diagnostics, for example a visual profiler. |
? | [InputSystem](../Input/Overview.md) | System providing support for accessing and handling user input. |
| | [SceneSystem](../SceneSystem/SceneSystemGettingStarted.md) | System providing multi-scene application support. |
| | [SpatialAwarenessSystem](../SpatialAwareness/SpatialAwarenessGettingStarted.md) | System providing support for awareness of the user's environment. |
| | [TeleportSystem](../TeleportSystem/Overview.md) | System providing support for teleporting (moving about the experience in jumps). |

Dependencies:

- Standard Assets (`com.microsoft.mixedreality.toolkit.standardassets`)

### Standard Assets

The standard assets package (`com.microsoft.mixedreality.toolkit.standardassets)` is a collection of components that are recommended for all mixed reality experiences, including:

- MRTK Standard shader
- Basic materials using the MRTK Standard shader 
- Audio files
- Fonts
- Textures
- Icons

> [!Note]
> To avoid breaking changes based on assembly definitions, the scripts used to control some features of the MRTK Standard shader are not included in the standard assets package. These scripts can be found in the foundation package in the `MRTK/Core/Utilities/StandardShader` folder.

Dependencies: none

### Extension packages

The optional extensions package (`com.microsoft.mixedreality.toolkit.extensions)` contains additional components that expand the functionality of the MRTK. 

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Extensions | |
| | [HandPhysicsService](../Extensions/HandPhysicsService/HandPhysicsServiceOverview.md) | Service that adds physics support to articulated hands. |
| | LostTrackingService | Service that simplifies handing of tracking loss on Microsoft HoloLens devices. |
| | [SceneTransitionService](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md) | Service that simplifies adding smooth scene transitions. |
| | Samples~ | A hidden (in the Unity Editor) folder that contains the sample scenes and assets. |

More details on the process of using packages containing example projects can be found in the [Mixed Reality Toolkit and Unity Package Manager](../usingupm.md#using-mixed-reality-toolkit-examples) article.

Dependencies:

- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)

### Tools package

The optional tools package (`com.microsoft.mixedreality.toolkit.tools)` contains tools that are useful for creating mixed reality experiences. In general, these tools are editor components and their code does not ship as part of an application.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tools | |
| | BuildWindow | Tool that helps simplify the process of building and deploying UWP applications. |
| | [DependencyWindow](../Tools/DependencyWindow.md) | Tool that creates a dependency graph of assets in a project. |
| | [ExtensionServiceCreator](../Tools/ExtensionServiceCreationWizard.md) | Wizard to assist in creating extension services. |
| | [MigrationWindow](../Tools/MigrationWindow.md) | Tool that assists in updating code that uses deprecated MRTK components.  |
| | [OptimizeWindow](../Tools/OptimizeWindow.md) | Utility to help automate configuring a mixed reality project for the best performance in Unity. |
| | ReserializeAssetsUtility | Provides support for reserializing specific Unity files. |
| | [RuntimeTools/Tools/ControllerMappingTool](../Tools/ControllerMappingTool.md) | Utility enabling developers to quickly determine Unity mappings for hardware controllers. |
| | ScreenshotUtility | Enables capturing application images in the Unity editor. |
| | TextureCombinerWindow | Utility to combine graphics textures. |
| | [Toolbox](../README_Toolbox.md) | UI that makes it easy to discover and use MRTK UX components. |

Dependencies:

- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)

### Test utilities package

The optional test utilities package (`com.microsoft.mixedreality.toolkit.testutilities`) contains a collection of helper scripts that enable developers to easily create play mode tests. These utilities are especially useful for developers creating MRTK components.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tests | |
| | TestUtilities | Methods to simplify creation of play mode tests, including hand simulation utilities. |
Dependencies:
- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)

### Examples package

The examples package (`com.microsoft.mixedreality.toolkit.examples`), is structured to allow developers to import only the examples of interest.

More details on the process of using packages containing example projects can be found in the [Mixed Reality Toolkit and Unity Package Manager](../usingupm.md#using-mixed-reality-toolkit-examples) article.

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Examples | | |
| | Samples~ | A hidden (in the Unity Editor) folder that contains the sample scenes and assets. |
| | StandardAssets | Common assets shared by multiple demo scenes. |

Dependencies:

- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)

## See also

- [Architecture Overview](../Architecture/Overview.md)
- [Systems, Extension Services and Data Providers](../Architecture/SystemsExtensionsProviders.md)
- [Mixed Reality Toolkit and Unity Package Manager](../usingupm.md)
