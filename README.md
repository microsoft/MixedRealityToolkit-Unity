<img src="External/ReadMeImages/MRTK_Logo_Rev.png">

# What is MixedRealityToolkit-Unity?
The Mixed Reality Toolkit is a collection of scripts and components intended to accelerate development of applications targeting Microsoft HoloLens and Windows Mixed Reality headsets.
The project is aimed at reducing barriers to entry to create mixed reality applications and contribute back to the community as we all grow.

MixedRealityToolkit-Unity uses code from the base [MixedRealityToolkit](https://github.com/Microsoft/MixedRealityToolkit) and makes it easier to consume in [Unity](https://unity3d.com/).

<img src="External/ReadMeImages/MixedRealityStack.png">

Learn more about [mixed reality](https://developer.microsoft.com/en-us/windows/mixed-reality/mixed_reality).

[github-rel]:                  https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/latest
[github-rel-badge]:            https://img.shields.io/github/tag/microsoft/MixedRealityToolkit-unity.svg?style=flat-square&label=latest%20master%20branch%20release&colorB=007ec6
[![Github Release][github-rel-badge]][github-rel]

[unity-download]:                 http://beta.unity3d.com/download/edcd66fb22ae/download.html
[unity-version-badge]:            https://img.shields.io/badge/current%20unity%20editor%20version-2017.2.0f3%20MRTP-green.svg
[![Github Release][unity-version-badge]][unity-download]

Looking to upgrade your projects to Windows Mixed Reality? [Follow the Upgrade Guide](/UpgradeGuide.md).

# Feature areas
| ![Input](External/ReadMeImages/MRTK170802_Short_03.png) [Input](Assets/HoloToolkit/Input/README.md)                                               | ![Sharing](External/ReadMeImages/MRTK170802_Short_04.png) [Sharing](Assets/HoloToolkit/Sharing/README.md)   | ![Spatial Mapping](External/ReadMeImages/MRTK170802_Short_05.png) [Spatial Mapping](Assets/HoloToolkit/SpatialMapping/README.md)| 
| :------------------------------------------------------------------------------------------------------------------------------------------------ | :---------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------------ |
| Scripts that leverage inputs such as gaze, gesture, voice and motion controllers.                                                                 | Sharing library enables collaboration across multiple devices.                                              | Scripts that allow applications to bring the real world into the digital using HoloLens.                                        | 
| ![Spatial Sound](External/ReadMeImages/MRTK170802_Short_09.png) [Spatial Sound](Assets/HoloToolkit/SpatialSound/README.md)                        | ![UX Controls](External/ReadMeImages/MRTK170802_Short_10.png) [UX Controls](Assets/HoloToolkit/UX/README.md)| ![Utilities](External/ReadMeImages/MRTK170802_Short_11.png) [Utilities](Assets/HoloToolkit/Utilities/README.md)                 | 
| Scripts to help plug spatial audio into your application.                                                                                         | Building blocks for creating good UX in your application like common controls.                              | Common helpers and tools that you can leverage in your application.                                                             |
| ![Spatial Understanding](External/ReadMeImages/MRTK170802_Short_06.png) [Spatial Understanding](Assets/HoloToolkit/SpatialUnderstanding/README.md)| ![Build](External/ReadMeImages/MRTK170802_Short_12.png) [Build](Assets/HoloToolkit/BuildAndDeploy/README.md)| ![Boundary](External/ReadMeImages/MRTK170802_Short_06.png) [Boundary](Assets/HoloToolkit/Boundary/README.md)                       |
| Tailor experiences based on room semantics like couch, wall etc.                                                                                  | Build and deploy automation window for Unity Editor.                                                        | Scripts that help with rendering the floor and boundaries for Immersive Devices.

# Required Software
| ![Windows 10 Creators Update](External/ReadMeImages/MRTK170802_Short_17.png) [Windows 10 Creators Update](https://www.microsoft.com/software-download/windows10)| ![Unity](External/ReadMeImages/MRTK170802_Short_18.png) [Unity](https://unity3d.com/get-unity/download/archive)| ![Visual Studio 2017](External/ReadMeImages/MRTK170802_Short_19.png) [Visual Studio 2017](http://dev.windows.com/downloads)| ![Hololens Emulator(optional)](External/ReadMeImages/MRTK170802_Short_20.png) [Hololens Emulator(optional)](https://go.microsoft.com/fwlink/?linkid=852626)|
| :-------------------------------------------------------------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------- |
| To develop apps for mixed reality headsets, you need the Fall Creators Update.                                                                                  | The Unity engine is an easy way to get started building a mixed reality app.                                   | Visual Studio is used for code editing, deploying and building UWP app packages.                                           | Emulator allows you test your app without the device in a simulated environment.                                                                           |

# Getting started with MRTK
| ![Hololens Emulator(optional)](External/ReadMeImages/MRTK170802c_Short_22.png) Quick start                        | ![Hololens Emulator(optional)](External/ReadMeImages/MRTK170802c_Short_23.png) Contributing to this project |
| :---------------------------------------------------------------------------------------------------------------- | :---------------------------------------------------------------------------------------------------------- |
| Please go over the [Getting started guide](GettingStarted.md) to learn more about getting off the ground quickly. | Please go over the [Contributing guidelines](CONTRIBUTING.md) to learn more about the process and thinking. |

## New Features with Fall Creators Update!

### Prerequisites:
1. See [Development PC specs](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools#developing_for_immersive_headsets) for tips on developing for immersive headsets.
2. [Holograms 100](https://developer.microsoft.com/en-us/windows/mixed-reality/holograms_100) has been updated with how to setup Windows Mixed Reality in your app.
3. [How to navigate the Windows Mixed Reality home](https://developer.microsoft.com/en-us/windows/mixed-reality/navigating_the_windows_mixed_reality_home).
4. Development PC needs to be on the Fall Creators Update (Version 1709)
5. [Visual Studio 2017](https://www.visualstudio.com/downloads/).
    1. Install the 10.0.16299.0 SDK via Visual Studio Installer.
6. Unity 2017.2.0f3 MRTP with Mixed Reality API support. This build of Unity can be found [here](http://beta.unity3d.com/download/edcd66fb22ae/download.html).
    1. Please read more about [Immersive headset details](https://developer.microsoft.com/en-us/windows/mixed-reality/immersive_headset_details).

<img src="External/ReadMeImages/MotionControllerTest_Teleport.png" width="700px">

If you're looking for **Controller models**:
* See the [**MotionControllerTest**](Assets/HoloToolkit-Examples/Input/Scenes/MotionControllerTest.unity) scene.
* See:
    * ControllerVisualizer.cs
    * ControllerInfo.cs
    * GLTFComponentStreamingAssets.cs
    * The entire Utilities\Scripts\GLTF folder.
* **IMPORTANT** Requires the 10.0.16299.0 SDK, or you will not be able to build these scripts.
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

# Roadmaps
### Master branch
| Target Unity release  | Release timeframe | Master release tag | Toolkit release features |
| --------------------- | ----------------- | ------------------ | ------------------------ |
| 2017.1.0              | Early Aug 2017    | v1.2017.1.0        | <ul><li>Updating to Unity's beta build.</li><li>3D Keyboard for text entry.</li></ul>|
| 2017.2.0              | End Oct 2017      | v1.2017.2.0        | <ul><li>Updating master with Windows Mixed Reality support (xR namespace).</li><li>RI Dev_Unity_2017.2.0 into master.</li><li>Toolkit will work on both HoloLens and immersive headsets.</li><li>Windows Mixed Reality motion controller support.</li></ul>|
| 2017.3.0              | End Dec 2017      | v1.2017.3.0        | <ul><li>360 video player prefabs.</li><li>Scriptable foveated rendering prefabs.</li></ul>|
### Dev_Unity_2017.2.0 branch

| Target Unity release  | Release timeframe | Branch release tag | Branch pre-release features | Status |
| --------------------- | ----------------- | ------------------ | --------------------------- | --------------------------- |
| MRTP9                 | End July 2017     | v1.Dev.MRTP9       | <ul><li>Windows Mixed Reality support for working on both HoloLens and immersive headsets.</li><li>Gamepad support.</li><li>Windows Mixed Reality motion controller support.</li></ul>| [Complete: v1.Dev.MRTP9](https://github.com/Microsoft/HoloToolkit-Unity/releases/tag/v1.Dev.MRTP9) |
| MRTP13                | Early Aug 2017    | v1.Dev.MRTP13      | <ul><li>Updating to MRTP13.</li><li>Windows Mixed Reality motion controller rendering with glTF format read from WU driver.</li></ul>| [Complete: v1.Dev.MRTP13](https://github.com/Microsoft/HoloToolkit-Unity/releases/tag/v1.Dev.MRTP13) |
| 2017.2.0              | Early Oct 2017      | v1.Dev.2017.2.0    | <ul><li>Adapting to Unity's breaking changes of xR namespace and others.</li></ul>| Complete |
| 2017.2.0              | End Mid 2017      | v1.Dev.2017.2.1    | <ul><li>Merge branch into master.</li></ul>| In Progress |

# Upcoming Breaking Changes

- **Renaming HoloToolkit-Unity repository to MixedRealityToolkit-Unity** to align with product direction.
- Technically **all your checkins and redirect links will continue to work as-is** but we wanted to give a better heads up on this.
- All other dependent repositories will undergo a similar name change.
- We are **not breaking toolkit folder names and namespaces at this time.**
- Instead we are taking a staggered approach for breaking changes based on developer feedback.

| Breaking change description                     | Release timeframe | Notes                    |
| ----------------------------------------------- | ----------------- | ------------------------ |
| Rename repository to MixedRealityToolkit-Unity. | Mid Aug 2017      | <ul><li>Recommend you do: $git remote set-url origin new_url.</li><li>Recommend reading: https://help.github.com/articles/renaming-a-repository ; https://github.com/blog/1508-repository-redirects-are-here</li></ul>|
| Updating toolkit namespace to MixedReality      | Nov 2017          | <ul><li>Update folder names, class names and namespace names post 2017.2.0 release.</li></ul>|

# Future work planning
- Updating landing page for the ReadMe.md to help app developers find content better.
- Updating API documentation using tools like Doxygen and not writing them manually. This is something we will work with all of you on.
- Update Wiki to host API documentation instead.
- Move reusable features from [Mixed Reality Design Labs](https://github.com/Microsoft/MRDesignLabs_Unity) project into toolkit.

# Release cadence
### Master branch
- **Master branch releases** will align with **major Unity releases marking a release every 3 months**. Please read the [Unity roadmap](https://unity3d.com/unity/roadmap).
- Each release will be marked as a GitHub [release tag](https://github.com/Microsoft/HoloToolkit-Unity/releases). You can consume this release by:
	- Using the zip file available in the release notes
	- Unity packages available in the release notes
	- Syncing to the specific git tag you would like to lock on.
- Release tag name convention: v1.Unity release major number.Unity release minor number.Iteration number
	- For example: For Unity version 2017.1.0 our release tag would be **v1.2017.1.0**
	- If we marked another release with the same Unity version: **v1.2017.1.1**
- Unity packages for each release will be published to the Unity asset store.
- Unity packages for each feature area like Input/SpatialMapping will also be added to the release notes.
- Academy content will be updated with each major toolkit release.

### Development branches
- Development branches are great for incubating features before they make their way into master.
- These branches can be feature work or experimental features.
- Development branches might not be on the same Unity versions as master.
- For being merged into Master the dev branch will have to follow the cadence laid out for master.
- Development branches might choose to mark releases on a faster cadence based on customer/project needs.
- Recommend marking a development branch tag every 2 weeks as needed to ensure stability before making up to master.
- Development branches might be deleted once merged into master. If more work is pending, it's ok to continue working in them.

**External\How To** docs folder is meant to help everyone with migrating forward or any simple doubts they might have about the process.
Please feel free to grow all these sections. We can't wait to see your additions!

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Useful resources on Microsoft Windows Dev Center
| ![Academy](External/ReadMeImages/icon_academy.png) [Academy](https://developer.microsoft.com/en-us/windows/mixed-reality/academy)| ![Design](External/ReadMeImages/icon_design.png) [Design](https://developer.microsoft.com/en-us/windows/mixed-reality/design)| ![Development](External/ReadMeImages/icon_development.png) [Development](https://developer.microsoft.com/en-us/windows/mixed-reality/development)| ![Community)](External/ReadMeImages/icon_community.png) [Community](https://developer.microsoft.com/en-us/windows/mixed-reality/community)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| See code examples. Do a coding tutorial. Watch guest lectures.          | Get design guides. Build user interface. Learn interactions and input.     | Get development guides. Learn the technology. Understand the science.       | Join open source projects. Ask questions on forums. Attend events and meetups. |
