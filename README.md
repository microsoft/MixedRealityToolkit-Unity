![Mixed Reality Toolkit](Documentation/Images/MRTK_Logo_Rev.png)

# What is the Mixed Reality Toolkit

MRTK-Unity is a Microsoft driven project that provides a set of components and features to accelerate cross-platform MR app development in Unity. Here are some things MRTK does:

* Provides the **basic building blocks for Unity development on HoloLens, Windows Mixed Reality, and OpenVR**.
* Enables **rapid prototyping** via in-editor simulation that allows you to see changes immediately.
* Designed as an **extensible framework** that provides developers ability to swap out core components.
* **Supports a wide range of platforms**, including
  * Microsoft HoloLens
  * Microsoft HoloLens 2
  * Windows Mixed Reality headsets
  * OpenVR headsets (HTC Vive / Oculus Rift)

# Getting Started with MRTK

| [![Getting Started and Documentation](Documentation/Images/MRTK_Icon_GettingStarted.png)](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/GettingStartedWithTheMRTK.html)<br/>[Getting Started](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/GettingStartedWithTheMRTK.html)| [![Getting Started](Documentation/Images/MRTK_Icon_ArchitectureOverview.png)](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Architecture/Overview.html)<br/>[MRTK Overview](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Architecture/Overview.html)| [![Feature Guides](Documentation/Images/MRTK_Icon_FeatureGuides.png)](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_Button.html)<br/>[Feature Guides](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_Button.html)| [![API Reference](Documentation/Images/MRTK_Icon_APIReference.png)](https://microsoft.github.io/MixedRealityToolkit-Unity/api/Microsoft.MixedReality.Toolkit.html)<br/>[API Reference](https://microsoft.github.io/MixedRealityToolkit-Unity/api/Microsoft.MixedReality.Toolkit.html)|
|:---|:---|:---|:---|

# Build Status

| Branch | CI Status | Docs Status |
|---|---|---|
| `mrtk_development` |[![CI Status](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_apis/build/status/public/mrtk_CI?branchName=mrtk_development)](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_build/latest?definitionId=15)|[![Docs Status](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_apis/build/status/public/mrtk_docs?branchName=mrtk_development)](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_build/latest?definitionId=7)

# Required Software

 | [![Windows SDK 18362+](Documentation/Images/MRTK170802_Short_17.png)](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk) [Windows SDK 18362+](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk)| [![Unity](Documentation/Images/MRTK170802_Short_18.png)](https://unity3d.com/get-unity/download/archive) [Unity 2018.4.x](https://unity3d.com/get-unity/download/archive)| [![Visual Studio 2019](Documentation/Images/MRTK170802_Short_19.png)](http://dev.windows.com/downloads) [Visual Studio 2019](http://dev.windows.com/downloads)| [![Simulator (optional)](Documentation/Images/MRTK170802_Short_20.png)](https://go.microsoft.com/fwlink/?linkid=2098508) [Simulator (optional)](https://go.microsoft.com/fwlink/?linkid=2098508)|
| :--- | :--- | :--- | :--- |
| To build apps with MRTK v2, you need the Windows 10 May 2019 Update SDK. <br> To run apps for immersive headsets, you need the Windows 10 Fall Creators Update. | The Unity 3D engine provides support for building mixed reality projects in Windows 10 | Visual Studio is used for code editing, deploying and building UWP app packages | The Emulators allow you test your app without the device in a simulated environment |

# Feature Areas

| ![Input System](Documentation/Images/MRTK_Icon_InputSystem.png) [Input System](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Overview.html)<br/>&nbsp;  | ![Hand Tracking<br/> (HoloLens 2)](Documentation/Images/MRTK_Icon_HandTracking.png) [Hand Tracking<br/> (HoloLens 2)](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSystem/HandTracking.html) | ![Eye Tracking<br/> (HoloLens 2)](Documentation/Images/MRTK_Icon_EyeTracking.png) [Eye Tracking<br/> (HoloLens 2)](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/EyeTracking/EyeTracking_Main.html) | ![Profiles](Documentation/Images/MRTK_Icon_Profiles.png) [Profiles](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/MixedRealityConfigurationGuide.html)<br/>&nbsp; | ![Gaze + Gesture<br/> (HoloLens)](Documentation/Images/MRTK_Icon_GazeSelect.png) [Gaze + Gesture<br/> (HoloLens)](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Gestures.html) |
| :--- | :--- | :--- | :--- | :--- |
| ![UI Controls](Documentation/Images/MRTK_Icon_UIControls.png) [UI Controls](https://microsoft.github.io/MixedRealityToolkit-Unity/README.html#ui-and-interaction-building-blocks)<br/>&nbsp; | ![Solvers](Documentation/Images/MRTK_Icon_Solver.png) [Solvers](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_Solver.html)<br/>&nbsp; | ![Multi-Scene<br/> Manager](Documentation/Images/MRTK_Icon_SceneSystem.png) [Multi-Scene<br/> Manager](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SceneSystem/SceneSystemGettingStarted.html) | ![Spatial<br/> Awareness](Documentation/Images/MRTK_Icon_SpatialUnderstanding.png) [Spatial<br/> Awareness](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html) | ![Diagnostic<br/> Tool](Documentation/Images/MRTK_Icon_Diagnostics.png) [Diagnostic<br/> Tool](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Diagnostics/DiagnosticsSystemGettingStarted.html) |
| ![MRTK Standard Shader](Documentation/Images/MRTK_Icon_StandardShader.png) [MRTK Standard Shader](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_MRTKStandardShader.html?q=shader) | ![Speech & Dictation](Documentation/Images/MRTK_Icon_VoiceCommand.png) [Speech](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Speech.html)<br/> & [Dictation](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Dictation.html) | ![Boundary<br/>System](Documentation/Images/MRTK_Icon_Boundary.png) [Boundary<br/>System](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Boundary/BoundarySystemGettingStarted.html)| ![In-Editor<br/>Simulation](Documentation/Images/MRTK_Icon_InputSystem.png) [In-Editor<br/>Simulation](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSimulation/InputSimulationService.html) | ![Experimental<br/>Features](Documentation/Images/MRTK_Icon_Experimental.png) [Experimental<br/>Features](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/ExperimentalFeatures.html)|

# UI and Interaction Building blocks

|  [![Button](Documentation/Images/Button/MRTK_Button_Main.png)](Documentation/README_Button.md) [Button](Documentation/README_Button.md) | [![Bounding Box](Documentation/Images/BoundingBox/MRTK_BoundingBox_Main.png)](Documentation/README_BoundingBox.md) [Bounding Box](Documentation/README_BoundingBox.md) | [![Manipulation Handler](Documentation/Images/ManipulationHandler/MRTK_Manipulation_Main.png)](Documentation/README_ManipulationHandler.md) [Manipulation Handler](Documentation/README_ManipulationHandler.md) |
|:--- | :--- | :--- |
| A button control which supports various input methods including HoloLens 2's articulated hand | Standard UI for manipulating objects in 3D space | Script for manipulating objects with one or two hands |
|  [![Slate](Documentation/Images/Slate/MRTK_Slate_Main.png)](Documentation/README_Slate.md) [Slate](Documentation/README_Slate.md) | [![System Keyboard](Documentation/Images/SystemKeyboard/MRTK_SystemKeyboard_Main.png)](Documentation/README_SystemKeyboard.md) [System Keyboard](Documentation/README_SystemKeyboard.md) | [![Interactable](Documentation/Images/Interactable/InteractableExamples.png)](Documentation/README_Interactable.md) [Interactable](Documentation/README_Interactable.md) |
| 2D style plane which supports scrolling with articulated hand input | Example script of using the system keyboard in Unity  | A script for making objects interactable with visual states and theme support |
|  [![Solver](Documentation/Images/Solver/MRTK_Solver_Main.png)](Documentation/README_Solver.md) [Solver](Documentation/README_Solver.md) | [![Object Collection](Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Main.jpg)](Documentation/README_ObjectCollection.md) [Object Collection](Documentation/README_ObjectCollection.md) | [![Tooltip](Documentation/Images/Tooltip/MRTK_Tooltip_Main.png)](Documentation/README_Tooltip.md) [Tooltip](Documentation/README_Tooltip.md) |
| Various object positioning behaviors such as tag-along, body-lock, constant view size and surface magnetism | Script for laying out an array of objects in a three-dimensional shape | Annotation UI with flexible anchor/pivot system which can be used for labeling motion controllers and objects |
|  [![Slider](Documentation/Images/Slider/MRTK_UX_Slider_Main.jpg)](Documentation/README_Sliders.md) [Slider](Documentation/README_Sliders.md) | [![MRTK Standard Shader](Documentation/Images/MRTKStandardShader/MRTK_StandardShader.jpg)](Documentation/README_MRTKStandardShader.md) [MRTK Standard Shader](Documentation/README_MRTKStandardShader.md) | [![Hand Menu](Documentation/Images/Solver/MRTK_UX_HandMenu.png)](Documentation/README_Solver.md#hand-menu-with-handconstraint-and-handconstraintpalmup) [Hand Menu](Documentation/README_Solver.md#hand-menu-with-handconstraint-and-handconstraintpalmup) |
| Slider UI for adjusting values supporting direct hand tracking interaction | MRTK's Standard shader supports various Fluent design elements with performance | Hand-locked UI for quick access, using Hand Constraint Solver |
|  [![App Bar](Documentation/Images/AppBar/MRTK_AppBar_Main.png)](Documentation/README_AppBar.md) [App Bar](Documentation/README_AppBar.md) | [![Pointers](Documentation/Images/Pointers/MRTK_Pointer_Main.png)](Documentation/Input/Pointers.md) [Pointers](Documentation/Input/Pointers.md) | [![Fingertip Visualization](Documentation/Images/Fingertip/MRTK_FingertipVisualization_Main.png)](Documentation/README_FingertipVisualization.md) [Fingertip Visualization](Documentation/README_FingertipVisualization.md) |
| UI for Bounding Box's manual activation | Learn about various types of pointers | Visual affordance on the fingertip which improves the confidence for the direct interaction |
|  [![Hand Joint Chaser](Documentation/Images/HandJointChaser/MRTK_HandJointChaser_Main.jpg)](Documentation/README_HandJointChaser.md) [Hand Joint Chaser](Documentation/README_HandJointChaser.md) | [![Spatial Awareness](Documentation/Images/SpatialAwareness/MRTK_SpatialAwareness_Main.png)](Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.md) [Spatial Awareness](Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.md) | [![Voice Command](Documentation/Images/Input/MRTK_Input_Speech.png)](Documentation/Input/Speech.md) [Voice Command](Documentation/Input/Speech.md) |
| Demonstrates how to use Solver to attach objects to the hand joints | Make your holographic objects interact with the physical environments | Scripts and examples for integrating speech input |
|  [![Eye Tracking: Target Selection](Documentation/Images/EyeTracking/mrtk_et_targetselect.png)](Documentation/EyeTracking/EyeTracking_TargetSelection.md) [Eye Tracking: Target Selection](Documentation/EyeTracking/EyeTracking_TargetSelection.md) | [![Eye Tracking: Navigation](Documentation/Images/EyeTracking/mrtk_et_navigation.png)](Documentation/EyeTracking/EyeTracking_Navigation.md) [Eye Tracking: Navigation](Documentation/EyeTracking/EyeTracking_Navigation.md) | [![Eye Tracking: Heat Map](Documentation/Images/EyeTracking/mrtk_et_heatmaps.png)](Documentation/EyeTracking/EyeTracking_ExamplesOverview.md#visualization-of-visual-attention) [Eye Tracking: Heat Map](Documentation/EyeTracking/EyeTracking_ExamplesOverview.md#visualization-of-visual-attention) |
| Combine eyes, voice and hand input to quickly and effortlessly select holograms across your scene | Learn how to auto scroll text or fluently zoom into focused content based on what you are looking at| Examples for logging, loading and visualizing what users have been looking at in your app |

# Tools

|  [![Optimize Window](Documentation/Images/MRTK_Icon_OptimizeWindow.png)](Documentation/Tools/OptimizeWindow.md) [Optimize Window](Documentation/Tools/OptimizeWindow.md) | [![Dependency Window](Documentation/Images/MRTK_Icon_DependencyWindow.png)](Documentation/Tools/DependencyWindow.md) [Dependency Window](Documentation/Tools/DependencyWindow.md) | ![Build Window](Documentation/Images/MRTK_Icon_BuildWindow.png) Build Window | [![Input recording](Documentation/Images/MRTK_Icon_InputRecording.png)](Documentation/InputSimulation/InputAnimationRecording.md) [Input recording](Documentation/InputSimulation/InputAnimationRecording.md) |
|:--- | :--- | :--- | :--- |
| Automate configuration of Mixed Reality projects for performance optimizations | Analyze dependencies between assets and identify unused assets |  Configure and execute end-to-end build process for Mixed Reality applications | Record and playback head movement and hand tracking data ins editor |

# Example Scenes

Explore MRTK's various types of interactions and UI controls in [this example scene](Documentation/README_HandInteractionExamples.md).

You can find  other example scenes under [**Assets/MixedRealityToolkit.Examples/Demos**](/Assets/MixedRealityToolkit.Examples/Demos) folder.

[![Example Scene](Documentation/Images/MRTK_Examples.png)](Documentation/README_HandInteractionExamples.md)

# MRTK Examples Hub

With MRTK Examples Hub, you can try various example scenes in MRTK.
You can find pre-built app packages for HoloLens(x86), HoloLens 2(ARM), and Windows Mixed Reality immersive headsets(x64) under [**Release Assets**](https://github.com/microsoft/MixedRealityToolkit-Unity/releases/tag/v2.0.0) folder. [Use the Windows Device Portal to install apps on HoloLens](https://docs.microsoft.com/en-us/hololens/hololens-install-apps#use-the-windows-device-portal-to-install-apps-on-hololens).

[![Example Scene](Documentation/Images/MRTK_ExamplesHub.png)](Documentation/README_HandInteractionExamples.md)


# Sample App

[Periodic Table of the Elements](https://github.com/Microsoft/MRDL_Unity_PeriodicTable) is an open-source sample app which demonstrates how to use MRTK's input system and building blocks to create an app experience for HoloLens and Immersive headsets. Read the porting story: [Bringing the Periodic Table of the Elements app to HoloLens 2 with MRTK v2](https://medium.com/@dongyoonpark/bringing-the-periodic-table-of-the-elements-app-to-hololens-2-with-mrtk-v2-a6e3d8362158)

[![Periodic Table of the Elements](Documentation/Images/MRDL_PeriodicTable.jpg)](https://medium.com/@dongyoonpark/bringing-the-periodic-table-of-the-elements-app-to-hololens-2-with-mrtk-v2-a6e3d8362158)

# Engage with the Community

- Join the conversation around MRTK on [Slack](https://holodevelopers.slack.com/).

- Ask questions about using MRTK on [Stack Overflow](https://stackoverflow.com/questions/tagged/mrtk) using the **MRTK** tag.

- Search for [known issues](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues) or file a [new issue](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues) if you find something broken in MRTK code.

- Join our weekly community shiproom to hear directly from the feature team. (link coming soon)

- Deep dive into project plan and learn how you can contribute to MRTK in our [wiki](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki).

- For issues related to Windows Mixed Reality that aren't directly related to the MRTK, check out the [Windows Mixed Reality Developer Forum](https://forums.hololens.com/).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Useful resources on the Mixed Reality Dev Center

| ![Discover](Documentation/Images/mrdevcenter/icon-discover.png) [Discover](https://docs.microsoft.com/en-us/windows/mixed-reality/)| ![Design](Documentation/Images/mrdevcenter/icon-design.png) [Design](https://docs.microsoft.com/en-us/windows/mixed-reality/design)| ![Develop](Documentation/Images/mrdevcenter/icon-develop.png) [Develop](https://docs.microsoft.com/en-us/windows/mixed-reality/development)| ![Distribute)](Documentation/Images/mrdevcenter/icon-distribute.png) [Distribute](https://docs.microsoft.com/en-us/windows/mixed-reality/implementing-3d-app-launchers)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| Learn to build mixed reality experiences for HoloLens and immersive headsets (VR).          | Get design guides. Build user interface. Learn interactions and input.     | Get development guides. Learn the technology. Understand the science.       | Get your app ready for others and consider creating a 3D launcher. |

# Useful resources on Azure

| ![Spatial Anchors](Documentation/Images/mrdevcenter/icon-azurespatialanchors.png)<br> [Spatial Anchors](https://docs.microsoft.com/en-us/azure/spatial-anchors/)| ![Speech Services](Documentation/Images/mrdevcenter/icon-azurespeechservices.png) [Speech Services](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/)| ![Vision Services](Documentation/Images/mrdevcenter/icon-azurevisionservices.png) [Vision Services](https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/)|
| :------------------------| :--------------------- | :---------------------- |
| Spatial Anchors is a cross-platform service that allows you to create Mixed Reality experiences using objects that persist their location across devices over time.| Discover and integrate Azure powered speech capabilities like speech to text, speaker recognition or speech translation into your application.| Identify and analyze your image or video content using Vision Services like computer vision, face detection, emotion recognition or video indexer. |

# Learn more about the MRTK Project

You can find our planning material on [our wiki](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki) under Project Management Section. You can always see the items the team is actively working on in the Iteration Plan issue.

# How to Contribute

View the [**How To Contribute**](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/How-to-Contribute) wiki page for the most up to date instructions on contributing to the Mixed Reality Toolkit!

**For details on the different branches used in the Mixed Reality Toolkit repositories, check this [Branch Guide here](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/Branch-Guide).**
