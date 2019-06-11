<img src="Documentation/Images/MRTK_Logo_Rev.png">

# What is the Mixed Reality Toolkit

MRTK is a Microsoft driven open source project. 

MRTK-Unity provides a set of foundational components and features to accelerate MR app development in Unity. The latest Release of MRTK (V2) supports HoloLens/HoloLens 2, Windows Mixed Reality, and OpenVR platforms.
 
* Provides the **basic building blocks for unity development on HoloLens, Windows Mixed Reality, and OpenVR**.
* Showcases UX best practices with **UI controls that match Windows Mixed Reality and HoloLens Shell**. 
* **Enables rapid prototyping** via in-editor simulation that allows you to see changes immediately.
* Is **extensible**. Provides devs ability to swap out core components and extend the framework.
* **Supports a wide range of platforms**, including
  * Microsoft HoloLens
  * Microsoft HoloLens 2
  * Windows Mixed Reality headsets
  * OpenVR headsets (HTC Vive / Oculus Rift)

# Build Status

| Branch | CI Status | Docs Status |
|---|---|---|
| `mrtk_development` |[![CI Status](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_apis/build/status/public/mrtk_CI?branchName=mrtk_development)](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_build/latest?definitionId=15)|[![Docs Status](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_apis/build/status/public/mrtk_docs)](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_build/latest?definitionId=7)

# Required Software

 | [![Windows SDK 18362+](Documentation/Images/MRTK170802_Short_17.png)](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk) [Windows SDK 18362+](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk)| [![Unity](Documentation/Images/MRTK170802_Short_18.png)](https://unity3d.com/get-unity/download/archive) [Unity 2018.4.x](https://unity3d.com/get-unity/download/archive)| [![Visual Studio 2017](Documentation/Images/MRTK170802_Short_19.png)](http://dev.windows.com/downloads) [Visual Studio 2017](http://dev.windows.com/downloads)| [![Simulator (optional)](Documentation/Images/MRTK170802_Short_20.png)](https://go.microsoft.com/fwlink/?linkid=852626) [Simulator (optional)](https://go.microsoft.com/fwlink/?linkid=852626)|
| :--- | :--- | :--- | :--- |
| To build apps with MRTK v2, you need the Windows 10 May 2019 Update SDK. <br> To run apps for Windows Mixed Reality immersive headsets, you need the Windows 10 Fall Creators Update. | The Unity 3D engine provides support for building mixed reality projects in Windows 10 | Visual Studio is used for code editing, deploying and building UWP app packages | The Emulators allow you test your app without the device in a simulated environment |

# Feature Areas
| ![](Documentation/Images/MRTK_Icon_InputSystem.png) Input System<br/>&nbsp;  | ![](Documentation/Images/MRTK_Icon_HandTracking.png) Hand Tracking (HoloLens 2) | ![](Documentation/Images/MRTK_Icon_EyeTracking.png) Eye Tracking (HoloLens 2) | ![](Documentation/Images/MRTK_Icon_VoiceCommand.png) Voice Commanding | ![](Documentation/Images/MRTK_Icon_GazeSelect.png) Gaze + Select (HoloLens) | ![](Documentation/Images/MRTK_Icon_Teleportation.png) Teleportation<br/>&nbsp; |
| :--- | :--- | :--- | :--- | :--- | :--- |
| ![](Documentation/Images/MRTK_Icon_UIControls.png) UI Controls<br/>&nbsp; | ![](Documentation/Images/MRTK_Icon_Solver.png) Solver and Interactions | ![](Documentation/Images/MRTK_Icon_ControllerVisualization.png) Controller Visualization | ![](Documentation/Images/MRTK_Icon_SpatialUnderstanding.png) Spatial Understanding | ![](Documentation/Images/MRTK_Icon_Diagnostics.png) Diagnostic Tool<br/>&nbsp; | ![](Documentation/Images/MRTK_Icon_StandardShader.png) MRTK Standard Shader |

# Getting Started with MRTK 
Please check out the [Getting Started Guide](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/GettingStartedWithTheMRTK.html)

### More Documentation
Find this readme, other documentation articles and the MRTK api reference on our [MRTK Dev Portal on github.io](https://microsoft.github.io/MixedRealityToolkit-Unity/). 

# UI and Interaction Building blocks
|  [![Button](Documentation/Images/Button/MRTK_Button_Main.png)](Documentation/README_Button.md) [Button](Documentation/README_Button.md) | [![Bounding Box](Documentation/Images/BoundingBox/MRTK_BoundingBox_Main.png)](Documentation/README_BoundingBox.md) [Bounding Box](Documentation/README_BoundingBox.md) | [![Manipulation Handler](Documentation/Images/ManipulationHandler/MRTK_Manipulation_Main.png)](Documentation/README_ManipulationHandler.md) [Manipulation Handler](Documentation/README_ManipulationHandler.md) |
|:--- | :--- | :--- |
| A button control which supports various input methods including HoloLens 2's articulated hand | Standard UI for manipulating objects in 3D space | Script for manipulating objects with one or two hands |
|  [![Slate](Documentation/Images/Slate/MRTK_Slate_Main.png)](Documentation/README_Slate.md) [Slate](Documentation/README_Slate.md) | [![System Keyboard](Documentation/Images/SystemKeyboard/MRTK_SystemKeyboard_Main.png)](Documentation/README_SystemKeyboard.md) [System Keyboard](Documentation/README_SystemKeyboard.md) | [![Interactable](Documentation/Images/Interactable/InteractableExamples.png)](Documentation/README_Interactable.md) [Interactable](Documentation/README_Interactable.md) |
| 2D style plane which supports scrolling with articulated hand input | Example script of using the system keyboard in Unity  | A script for making objects interactable with visual states and theme support |
|  [![Solver](Documentation/Images/Solver/MRTK_Solver_Main.png)](Documentation/README_Solver.md) [Solver](Documentation/README_Solver.md) | [![Object Collection](Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Main.png)](Documentation/README_ObjectCollection.md) [Object Collection](Documentation/README_ObjectCollection.md) | [![Tooltip](Documentation/Images/Tooltip/MRTK_Tooltip_Main.png)](Documentation/README_Tooltip.md) [Tooltip](Documentation/README_Tooltip.md) |
| Various object positioning behaviors such as tag-along, body-lock, constant view size and surface magnetism | Script for lay out an array of objects in a three-dimensional shape | Annotation UI with flexible anchor/pivot system which can be used for labeling motion controllers and object. |
|  [![App Bar](Documentation/Images/AppBar/MRTK_AppBar_Main.png)](Documentation/README_AppBar.md) [App Bar](Documentation/README_AppBar.md) | [![Pointers](Documentation/Images/Pointers/MRTK_Pointer_Main.png)](Documentation/README_Pointers.md) [Pointers](Documentation/README_Pointers.md) | [![Fingertip Visualization](Documentation/Images/Fingertip/MRTK_FingertipVisualization_Main.png)](Documentation/README_FingertipVisualization.md) [Fingertip Visualization](Documentation/README_FingertipVisualization.md) |
| UI for Bounding Box's manual activation | Learn about various types of pointers | Visual affordance on the fingertip which improves the confidence for the direct interaction |
|  [![Slider](Documentation/Images/Slider/MRTK_UX_Slider_Main.jpg)](Documentation/README_Sliders.md) [Slider](Documentation/README_Sliders.md) | [![MRTK Standard Shader](Documentation/Images/MRTKStandardShader/MRTK_StandardShader.jpg)](Documentation/README_MRTKStandardShader.md) [MRTK Standard Shader](Documentation/README_MRTKStandardShader.md) | [![Hand Joint Chaser](Documentation/Images/HandJointChaser/MRTK_HandJointChaser_Main.jpg)](Documentation/README_HandJointChaser.md) [Hand Joint Chaser](Documentation/README_HandJointChaser.md) |
| Slider UI for adjusting values supporting direct hand tracking interaction | MRTK's Standard shader supports various Fluent design elements with performance | Demonstrates how to use Solver to attach objects to the hand joints |
|  [![Eye Tracking: Target Selection](Documentation/Images/EyeTracking/mrtk_et_targetselect.png)](Documentation/EyeTracking/EyeTracking_TargetSelection.md) [Eye Tracking: Target Selection](Documentation/EyeTracking/EyeTracking_TargetSelection.md) | [![Eye Tracking: Navigation](Documentation/Images/EyeTracking/mrtk_et_navigation.png)](Documentation/EyeTracking/EyeTracking_Navigation.md) [Eye Tracking: Navigation](Documentation/EyeTracking/EyeTracking_Navigation.md) | [![Eye Tracking: Heat Map](Documentation/Images/EyeTracking/mrtk_et_heatmaps.png)](Documentation/EyeTracking/EyeTracking_Visualization.md) [Eye Tracking: Heat Map](Documentation/EyeTracking/EyeTracking_Visualization.md) |
| Combine eyes, voice and hand input to quickly and effortlessly select holograms across your scene | Learn how to auto scroll text or fluently zoom into focused content based on what you are looking at| Examples for logging, loading and visualizing what users have been looking at in your app |

# Example Scenes
Explore MRTK's various types of interactions and UI controls in [this example scene](Documentation/README_HandInteractionExamples.md).

You can find  other example scenes under [**Assets/MixedRealityToolkit.Examples/Demos**](/Assets/MixedRealityToolkit.Examples/Demos) folder.

[![Example Scene](Documentation/Images/MRTK_Examples.png)](Documentation/README_HandInteractionExamples.md)

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

# Useful resouces on Azure
| ![Spatial Anchors](Documentation/Images/mrdevcenter/icon-azurespatialanchors.png)<br> [Spatial Anchors](https://docs.microsoft.com/en-us/azure/spatial-anchors/)| ![Speech Services](Documentation/Images/mrdevcenter/icon-azurespeechservices.png) [Speech Services](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/)| ![Vision Services](Documentation/Images/mrdevcenter/icon-azurevisionservices.png) [Vision Services](https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/)|       
| :------------------------| :--------------------- | :---------------------- | 
| Spatial Anchors is a cross-platform service that allows you to create Mixed Reality experiences using objects that persist their location across devices over time.| Discover and integrate Azure powered speech capabilities like speech to text, speaker recognition or speech translation into your application.| Identify and analyze your image or video content using Vision Services like computer vision, face detection, emotion recognition or video indexer. | 


### Learn more about MRTK Project 
You can find our planning material on [our wiki](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki) under Project Management Section. You can always see the items the team is actively working on in the Iteration Plan issue. 

### How to Contribute
View the [**How To Contribute**](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/How-to-Contribute) wiki page for the most up to date instructions on contributing to the Mixed Reality Toolkit!

### For details on the different branches used in the Mixed Reality Toolkit repositories, check this [Branch Guide here](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/Branch-Guide).
