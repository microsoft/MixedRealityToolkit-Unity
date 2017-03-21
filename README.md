# HoloToolkit-Unity
This is effectively part of the existing HoloToolkit, but this is the repository that will contain all Unity specific components.
The HoloToolkit is a collection of scripts and components intended to accelerate development of holographic applications targeting Windows Holographic.

**Current Unity Editor Project Version: Early access 5.6.0b11** **Not accessible broadly yet.**

**Branch: HoloToolkit-Unity RS2 with Windows Mixed Reality support for immersive headsets.**

## Prerequisites:
1. [Development PC specs](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools#developing_for_immersive_headsets) Developing for immersive headsets.
2. [Holograms 100](https://developer.microsoft.com/en-us/windows/mixed-reality/holograms_100) has been updated with how to setup Windows Mixed Reality in your app.
3. [How to navigate the Windows Mixed Reality home](https://developer.microsoft.com/en-us/windows/mixed-reality/navigating_the_windows_mixed_reality_home).
4. Development PC needs to be on RS2 SDK and builds as below:
    4.1. Launch Settings > Updates and Security > Windows Insider Program > Get Insider Preview builds 
    4.2. Release = rs2_release
5. Early access private build of Unity 5.6.0b11 with RS2 API support. This build of Unity is currently invite only.
    5.1. Please read more about [Immersive headset details](https://developer.microsoft.com/en-us/windows/mixed-reality/immersive_headset_details).
6. [Visual Studio 2017](https://www.visualstudio.com/downloads/).

## Please note:
1. We will not merge this branch into master until we have a publicly available Unity that supports it and a HoloLens build that works with these new APIs.
2. You can use this branch with RS2 builds on your PC and deploy it to RS1 HoloLens flight.
3. RS2 flights don't ship on HoloLens.

## What's new?
**GamepadInput** supports the Xbox controller and maps it to the input manager events.
You can use the Xbox controller A to tap.
Press A and hold to do hold started, canceled and completed.
Press A and left joystick to trigger the navigation events.
Press the Y button to teleport in your world.
Press the B button to return back to your original location.

**Playspace** folder has the scripts that support the StageRoot component that helps define the 0,0,0 for your immersive applications.
Prefabs help to draw the floor for immersive headsets and also renders the playspace bounds if you wish to display those in your application.

**External\How To** docs folder is meant to help everyone with migrating forward or any simple doubts they might have about the process.
Please feel free to grow all these sections. We can't wait to see your additions!

We've also done some future proofing work to add support for motion controllers. 
However, current model is gaze and commit with your head gaze and the Xbox controller.

## HoloToolkit contains the following feature areas:

1. [Input](Assets/HoloToolkit/Input/README.md)
2. [Sharing](Assets/HoloToolkit/Sharing/README.md)
3. [Spatial Mapping](Assets/HoloToolkit/SpatialMapping/README.md)
4. [Spatial Understanding](Assets/HoloToolkit/SpatialUnderstanding/README.md)
5. [Spatial Sound](Assets/HoloToolkit/SpatialSound/README.md)
6. [Utilities](Assets/HoloToolkit/Utilities/README.md)
7. [Build](Assets/HoloToolkit/Build/README.md)
8. [Playspace](Assets/HoloToolkit/Playspace/README.md)

To learn more about individual HoloLens feature areas, please read the [Wiki](https://github.com/Microsoft/HoloToolkit-Unity/wiki) section.

To learn how to add the HoloToolkit to your project see the [Getting Started](GettingStarted.md) guide.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
