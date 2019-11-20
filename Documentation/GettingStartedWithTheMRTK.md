# Getting Started with MRTK

![MRTK Logo](../Documentation/Images/MRTK_Logo_Rev.png)

The Mixed Reality Toolkit (MRTK) is a cross-platform toolkit for building Mixed Reality experiences for Virtual Reality (VR) and Augmented Reality (AR).

## Prerequisites

To get started with the Mixed Reality Toolkit you will need:

* [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
* [Unity 2018.4.x, 2019.1.x or 2019.2.x](https://unity3d.com/get-unity/download/archive)

  MRTK supports both IL2CPP and .NET scripting backends on Unity 2018

* [Latest MRTK release](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases)
* [Windows SDK 18362+](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk).

  This is necessary if you are building a UWP app for WMR, HoloLens 1, or HoloLens 2. This is not necessary
  when building for OpenVR.

## Getting started tutorials

If you are new to MRTK, or MR development, we recommend you check out the [Getting started tutorials](https://docs.microsoft.com/en-us/windows/mixed-reality/mrlearning-base) which uses MRTK v2. Check out [MRTK 101: How to use Mixed Reality Toolkit Unity for Basic Interactions (HoloLens 2, HoloLens, Windows Mixed Reality, Open VR)](https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-101) to learn about core building blocks.

## Add MRTK to your Unity Project

Mixed Reality Toolkit is now available for download on NuGet.org, for details see [MRTK NuGet Package](MRTKNuGetPackage.md).

### Get the latest MRTK Unity packages

1. Go to the  [MRTK release page](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases).
1. Under Assets, download
    - `Microsoft.MixedRealityToolkit.Unity.Foundation.unitypackage`
    - `Microsoft.MixedRealityToolkit.Unity.Extensions.unitypackage`
    - `Microsoft.MixedRealityToolkit.Unity.Tools.unitypackage`
    - `Microsoft.MixedRealityToolkit.Unity.Examples.unitypackage`

For additional delivery mechanisms, please see [Downloading the MRTK](DownloadingTheMRTK.md).

### Switch your Unity project to the target platform

The next step **Import MRTK packages into your Unity project** will apply changes to your project specifically for the platform that is selected in the project at that moment you import them.

You should make sure that you select the correct platform before following the next step.

For instance, if you want to create a HoloLens application, switch to Universal Windows Platform:

- Open menu : File > Build Settings
- Select **Universal Windows Platform** in the **Platform** list
- Click on the **Switch Platform** button

### Import MRTK packages into your Unity project

1. Create a new Unity project, or open an existing project. When creating a project, make sure to select "3D" as the template type.
1. Import the `Microsoft.MixedRealityToolkit.Unity.Foundation.unitypackage` you downloaded by going into "Asset -> Import Package -> Custom Package", selecting the .unitypackage file, ensure all items to import are checked, and then selecting "Import".
1. Import the `Microsoft.MixedRealityToolkit.Unity.Examples.unitypackage` following the same steps as above. The examples package is optional and contains useful demonstration scenes for current MRTK features.
1. Import the `Microsoft.MixedRealityToolkit.Unity.Tools.unitypackage` following the same steps as for the foundation package. These tools package is optional and contains useful tools, such as the ExtensionServiceCreator, that enhance the MRTK developer experience.
1. Import the `Microsoft.MixedRealityToolkit.Unity.Extensions.unitypackage` following the same steps as for the foundation package. The extensions package provides a set of useful optional components for the MRTK.

After importing the Foundation package, you may see a setup prompt like the following:

![UnitySetupPrompt](../Documentation/Images/MRTK_UnitySetupPrompt.png)

MRTK is attempting to set up your project for building Mixed Reality solutions by doing the following:

 * Enable XR Settings for your current platform (enabling the XR checkbox).
 * Force Text Serialization / Visible Meta files (recommended for Unity projects using source control).

Accepting these options is completely optional, but recommended.

Some prefabs and assets require TextMesh Pro, meaning you have to have the TextMesh Pro package installed and the assets in your project (Window -> TextMeshPro -> Import TMP Essential Resources). **After you import TMP Essentials Resources, you need to restart Unity to see changes**.

### Open and run the HandInteractionExamples scene in editor

[![HandInteractionExample scene](../Documentation/Images/MRTK_Examples.png)](README_HandInteractionExamples.md)

The [hand interaction examples scene](README_HandInteractionExamples.md) is a great place to get started because it shows a wide variety of UX controls and interactions in MRTK. 

1. Open the **HandInteractionExamples** scene under `Assets\MixedRealityToolkit.Examples\Demos\HandTracking\Scenes\HandInteractionExamples`

2. You may get a prompt asking you to import "TMP Essentials".

![TMP Essentials](../Documentation/Images/getting_started/MRTK_GettingStarted_TMPro.png)

If you get such a prompt, select "Import TMP essentials" button. "TMP Essentials" refers to Text Mesh Pro plugin, which some of the MRTK examples use for improved text rendering. (See [Text in Unity](https://docs.microsoft.com/en-us/windows/mixed-reality/text-in-unity) for more detailed information)

3. Close the TMP dialog. After this you need to reload the scene. You can do this by double clicking the scene in the project tab.

4. Press the play button.

## Using the In-Editor Hand Input Simulation to test a scene

The in-editor input simulation allows you to test virtual object behavior given a specific type of input such as [hands](InputSimulation/InputSimulationService.md#hand-simulation) or [eyes](EyeTracking/EyeTracking_BasicSetup.md#simulating-eye-tracking-in-the-unity-editor).

How to move around in the scene: 
- Use **W/A/S/D** keys to move the camera forward/left/back/right.
- Use **Q/E** to move the camera vertically.
- Press and hold the **right mouse button** to rotate the camera.

How to simulate hand input:
- Press and hold the **space bar** to enable the right hand. 
- While holding the space bar, move your mouse to move the hand.
- Use the mouse **scroll wheel** to adjust the depth of the hand.
- Click the **left mouse button** to simulate pinch gesture.
- Use **T/Y** keys to make the hand persistent in the view.
- Hold **CTRL** key and move the mouse to rotate the hand.

Have fun exploring the scene! You can learn more about the UI controls [in the hand interaction examples guide](README_HandInteractionExamples.md). Also, read through [input simulation docs](InputSimulation/InputSimulationService.md) to learn more about in-editor hand input simulation in MRTK.

Congratulations, you just used your first MRTK scene. Now onto creating your own experiences...

### Add MRTK to a new scene or new project

1. Create a new Unity project, or start a new scene in your current project. 

2. Make sure you have imported the MRTK packages (we recommend both Foundation and Examples, though Examples is not required) following [the steps above](#import-mrtk-packages-into-your-unity-project).

3. From the menu bar, select Mixed Reality Toolkit -> Add to Scene and Configure

![Configure to scene](../Documentation/Images/MRTK_ConfigureScene.png)

4. You will see a prompt like this:

![MRTK Configure Dialog](../Documentation/Images/MRTK_ConfigureDialog.png)

Click "OK".

5. You will then be prompted to choose an MRTK Configuration profile. Double click "DefaultMixedRealityToolkitConfigurationProfile".

![MRTK Select Configure Dialog](../Documentation/Images/MRTK_SelectConfigurationDialog.png)

> **NOTE**: Note that if you are getting started on the HoloLens or HoloLens 2, you should choose the "DefaultHoloLens1ConfigurationProfile" or "DefaultHoloLens2ConfigurationProfile" instead.
> See the [profiles](Profiles/Profiles.md#hololens-2-profile) for more information on the differences between 
> DefaultMixedRealityToolkitConfigurationProfile and DefaultHoloLens2ConfigurationProfile.

You will then see the following in your Scene hierarchy:

![MRTK Scene Setup](../Documentation/Images/MRTK_SceneSetup.png)

Which contains the following:

* **Mixed Reality Toolkit** - The toolkit itself, providing the central configuration entry point for the entire framework.
* **MixedRealityPlayspace** - The parent object for the headset, which ensures the headset / controllers and other required systems are managed correctly in the scene.
* The Main Camera is moved as a child to the Playspace - Which allows the playspace to manage the camera in conjunction with the SDKs

**Note** While working in your scene, **DO NOT move the Main Camera** (or the playspace) from the scene origin (0,0,0).  This is controlled by the MRTK and the active SDK.
If you need to move the players start point, then **move the scene content and NOT the camera**!

6. Hit play and test out hand simulation by pressing spacebar.

You are now ready to build and deploy to device! Follow the steps instructions at [Build and Deploy MRTK](BuildAndDeploy.md).

## Next steps

Here are some suggested next steps:

* Check out [MRTK 101: How to use Mixed Reality Toolkit Unity for Basic Interactions](https://docs.microsoft.com/en-us/windows/mixed-reality/mrtk-101) to learn about how to achieve common spatial interactions such as grab, move, scale, and rotate.
* Learn about the UX controls available in MRTK in [UI and interaction building blocks](../README.md#ui-and-interaction-building-blocks).
* Try [MRTK Examples Hub](README_ExampleHub.md) (pre-built app packages are included in the release page for your convenience)
* Learn how to work with the MRTK Configuration profile in the [mixed reality configuration guide](MixedRealityConfigurationGuide.md).
* Learn about the [MRTK's Architecture](../Documentation/Architecture/Overview.md)
* Learn about the [MRTK's Input System](../Documentation/Input/Overview.md)
* Learn about the [MRTK's Tools](../README.md#tools) that will empower your mixed reality design and development.
* Read through [input simulation guide](InputSimulation/InputSimulationService.md) to learn how to simulate hand input in editor.

## Upgrading from the HoloToolkit (HTK/MRTK v1)

There is not a direct upgrade path from the HoloToolkit to Mixed Reality Toolkit v2 due to the rebuilt framework. However, it is possible to import the MRTK into your HoloToolkit project and migrate your implementation. For more information please see the [HoloToolkit to Mixed Reality Toolkit Porting Guide](HTKToMRTKPortingGuide.md)
