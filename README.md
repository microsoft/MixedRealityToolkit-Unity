# MixedRealityToolkit Dev_Unity_2017.2.0 branch
This is a development branch for Windows Mixed Reality immersive headset support.

**IMPORTANT** You will also need a recent Windows Insider SDK installed, or you will not be able to build this branch. Please install the SDK found [here](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK).

[unity-download]:                 https://unity3d.com/unity/beta
[unity-version-badge]:            https://img.shields.io/badge/current%20unity%20editor%20version-2017.2.0f1-green.svg
[![Github Release][unity-version-badge]][unity-download]

## New Features!
If you're looking for **Controller models**:
* See the [**MotionControllerTest**](Assets/HoloToolkit/Input/Tests/Scenes/MotionControllerTest.unity) scene.
* See:
    * ControllerVisualizer.cs
    * ControllerInfo.cs
    * GLTFComponentStreamingAssets.cs
    * The entire Utilities\Scripts\GLTF folder.
* **IMPORTANT** You will also need a recent Windows Insider SDK installed, or you will not be able to build these scripts. Please install the SDK found [here](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK).

If you're looking for **teleporting**:
* See the [**MotionControllerTest**](Assets/HoloToolkit/Input/Tests/Scenes/MotionControllerTest.unity) scene.
* Controls are the same as the Cliff House, using either an Xbox controller or motion controllers.
* See:
    * MixedRealityTeleport.cs
    * MixedRealityCameraParent.prefab
    * TeleportMarker.prefab
    
If you're looking for **Xbox controller input** via the MixedRealityToolkit's InputManager:
* See the [**InputTapTest**](Assets/HoloToolkit/Input/Tests/Scenes/InputTapTest.unity) scene.
* You can use the Xbox controller A to tap.
* Press A and hold to do hold started, canceled and completed.
* Press A and left joystick to trigger the navigation events.
* See:
    * GamepadInput.cs
    
If you're looking for **Boundary** tools:
* See the [**BoundaryTest**](Assets/HoloToolkit/Boundary/Tests/Scenes/BoundaryTest.unity) scene.
* The **Boundary** folder has the scripts that support defining the floor for your immersive applications.
* The scripts help to draw the floor for immersive headsets and also allows you to check if an object is within those bounds.
* See:
    * BoundaryManager.cs
    * MixedRealityCameraParent.prefab
    
**External\How To** docs folder is meant to help everyone with migrating forward or any simple doubts they might have about the process.
Please feel free to grow all these sections. We can't wait to see your additions!

## Prerequisites:
1. See [Development PC specs](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools#developing_for_immersive_headsets) for tips on developing for immersive headsets.
2. [Holograms 100](https://developer.microsoft.com/en-us/windows/mixed-reality/holograms_100) has been updated with how to setup Windows Mixed Reality in your app.
3. [How to navigate the Windows Mixed Reality home](https://developer.microsoft.com/en-us/windows/mixed-reality/navigating_the_windows_mixed_reality_home).
4. Development PC needs to be on a recent Windows Insider SDK and a Windows Insider build as below.
    1. Launch Settings > Updates and Security > Windows Insider Program > Get Insider Preview builds 
    2. Install the SDK found [here](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK).
5. Unity beta 2017.2.0 with Mixed Reality API support. This build of Unity can be found [here](https://unity3d.com/unity/beta).
    1. Please read more about [Immersive headset details](https://developer.microsoft.com/en-us/windows/mixed-reality/immersive_headset_details).
6. [Visual Studio 2017](https://www.visualstudio.com/downloads/).

## Please note:
1. We will not merge this branch into master until we have a publicly available non-beta Unity that supports it and a HoloLens build that works with these new APIs.
2. You can use this branch with Windows Creators Update builds on your PC and deploy it to Windows Anniversary Update HoloLens flight.
3. Windows Creators Update flights don’t ship on HoloLens.

## MixedRealityToolkit contains the following feature areas:

1. [Boundary](Assets/HoloToolkit/Boundary/README.md)
2. [Build](Assets/HoloToolkit/Build/README.md)
3. [Input](Assets/HoloToolkit/Input/README.md)
4. [Sharing](Assets/HoloToolkit/Sharing/README.md)
5. [Spatial Mapping](Assets/HoloToolkit/SpatialMapping/README.md)
6. [Spatial Sound](Assets/HoloToolkit/SpatialSound/README.md)
7. [Spatial Understanding](Assets/HoloToolkit/SpatialUnderstanding/README.md)
8. [Utilities](Assets/HoloToolkit/Utilities/README.md)


To learn how to add the MixedRealityToolkit to your project see the [Getting Started](GettingStarted.md) guide.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
