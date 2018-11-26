## New Features with Fall Creators Update!

### Prerequisites:
1. See [Development PC specs](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools#developing_for_immersive_headsets) for tips on developing for immersive headsets.
2. [Holograms 100](https://developer.microsoft.com/en-us/windows/mixed-reality/holograms_100) has been updated with how to setup Windows Mixed Reality in your app.
3. [How to navigate the Windows Mixed Reality home](https://developer.microsoft.com/en-us/windows/mixed-reality/navigating_the_windows_mixed_reality_home).
4. Development PC needs to be on the Fall Creators Update (Version 1709)
5. [Visual Studio 2017](https://www.visualstudio.com/downloads/).
    1. Install the **10.0.16299.0 SDK** via Visual Studio Installer.
6. Unity 2017.2.0f3 MRTP with Mixed Reality API support. This build of Unity can be found [here](http://beta.unity3d.com/download/edcd66fb22ae/download.html).
    1. Please read more about [Immersive headset details](https://developer.microsoft.com/en-us/windows/mixed-reality/immersive_headset_details).

<img src="External/ReadMeImages/MotionControllerTest_Teleport.png" width="500px">

If you're looking for **Controller models**:
* See the [**MotionControllerTest**](Assets/HoloToolkit-Examples/Input/Scenes/MotionControllerTest.unity) scene.
* See:
    * ControllerVisualizer.cs
    * ControllerInfo.cs
    * GLTFComponentStreamingAssets.cs
    * The entire Utilities\Scripts\GLTF folder.
* **IMPORTANT** Requires the **10.0.16299.0 SDK**, or you will not be able to build these scripts.
    - You can install the SDK using the Visual Studio Installer.
* **IMPORTANT** Currently, motion controller's GLTF 3D model is only visible when you deploy through Visual Studio. In Unity's game mode, you should assign override model. <img src="External/ReadMeImages/MotionControllerTest_ModelOverride.png" width="700px">

If you're looking for **teleporting**:
* See the [**MotionControllerTest**](Assets/HoloToolkit-Examples/Input/Scenes/MotionControllerTest.unity) scene.
* Controls are the same as the Cliff House, using either an Xbox controller or motion controllers. Thumbstick up for teleport, down for backup, left/right for rotation.
* See:
    * MixedRealityTeleport.cs
    * MixedRealityCameraParent.prefab
    * TeleportMarker.prefab
    
If you're looking for **Xbox Controller Input** via the MixedRealityToolkit's InputManager:
* See the [**XboxControllerExample**](Assets/HoloToolkit-Examples/Input/Scenes/XboxControllerExample.unity) scene.
    
If you're looking for **Boundary** tools:
* See the [**BoundaryTest**](Assets/HoloToolkit-Examples/Boundary/Scenes/BoundaryTest.unity) scene.
* The **Boundary** folder has the scripts that support defining the floor for your immersive applications.
* The scripts help to draw the floor for immersive headsets and also allows you to check if an object is within those bounds.
* See:
    * BoundaryManager.cs
    * MixedRealityCameraParent.prefab
    
If you're looking for **Motion controller Grab Mechanics**:
* See  [**MotionControllersGrabMechanics**](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master/Assets/HoloToolkit-Examples/MotionControllers-GrabMechanics) page.
<img src="External/ReadMeImages/MRTK_MotionController_GrabMechanics.jpg" width="500px">