<img src="External/ReadMeImages/MRTK_Logo_Rev.png">

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
  * Microsoft Immersive headsets (IHMD)
  * Windows Mixed Reality headsets
  * OpenVR headsets (HTC Vive / Oculus Rift)

# Build Status

| Branch | Status |
|---|---|
| `mrtk_development` |[![Build status](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_apis/build/status/public/mrtk_development-CI)](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_build/latest?definitionId=1)|

 # Required Software
 | [![Windows 10 Creators Update](External/ReadMeImages/MRTK170802_Short_17.png)](https://www.microsoft.com/software-download/windows10) [Windows 10 FCU](https://www.microsoft.com/software-download/windows10)| [![Unity](External/ReadMeImages/MRTK170802_Short_18.png)](https://unity3d.com/get-unity/download/archive) [Unity 3D](https://unity3d.com/get-unity/download/archive)| [![Visual Studio 2017](External/ReadMeImages/MRTK170802_Short_19.png)](http://dev.windows.com/downloads) [Visual Studio 2017](http://dev.windows.com/downloads)| [![Simulator (optional)](External/ReadMeImages/MRTK170802_Short_20.png)](https://go.microsoft.com/fwlink/?linkid=852626) [Simulator (optional)](https://go.microsoft.com/fwlink/?linkid=852626)|
| :--- | :--- | :--- | :--- |
| To develop apps for Windows Mixed Reality headsets, you need the Windows 10 Fall Creators Update | The Unity 3D engine provides support for building mixed reality projects in Windows 10 | Visual Studio is used for code editing, deploying and building UWP app packages | The Emulators allow you test your app without the device in a simulated environment |

# Feature Areas

- Input System
- Articulated Hands + Gestures (HoloLens 2)
- Eye Tracking (HoloLens 2)
- Voice Commanding
- Gaze + Select (HoloLens)
- Controller Visualization
- Teleportation
- UI Controls
- Solver and Interactions
- Spatial Understanding
- Diagnostic Tool

# Getting Started with MRTK 
1. [Download MRTK](Documentation/DownloadingTheMRTK.md)
2. Follow this [Getting Started Guide](Documentation/GettingStartedWithTheMRTK.md)
3. Check out [Mixed Reality Toolkit configuration guide](Documentation/MixedRealityConfigurationGuide.md)
4. Check out building blocks and example scenes(see the table below)

### More Documentation
Find this readme, other documentation articles and the MRTK api reference on our [MRTK Dev Portal on github.io](https://microsoft.github.io/MixedRealityToolkit-Unity/). 

# UI and Interaction Building blocks
|  [![Button](External/ReadMeImages/Button/MRTK_Button_Main.png)](Documentation/README_Button.md) [Button](Documentation/README_Button.md) | [![Bounding Box](External/ReadMeImages/BoundingBox/MRTK_BoundingBox_Main.png)](Documentation/README_BoundingBox.md) [Bounding Box](Documentation/README_BoundingBox.md) | [![Manipulation Handler](External/ReadMeImages/ManipulationHandler/MRTK_Manipulation_Main.png)](Documentation/README_ManipulationHandler.md) [Manipulation Handler](Documentation/README_ManipulationHandler.md) |
|:--- | :--- | :--- |
| A button control which supports various input methods including HoloLens 2's articulated hand | Standard UI for manipulating objects in 3D space | Script for manipulating objects with one or two hands |
|  [![Slate](External/ReadMeImages/Slate/MRTK_Slate_Main.png)](Documentation/README_Slate.md) [Slate](Documentation/README_Slate.md) | [![System Keyboard](External/ReadMeImages/SystemKeyboard/MRTK_SystemKeyboard_Main.png)](Documentation/README_SystemKeyboard.md) [System Keyboard](Documentation/README_SystemKeyboard.md) | [![Interactable](External/ReadMeImages/Interactable/InteractableExamples.png)](Documentation/README_Interactable.md) [Interactable](Documentation/README_Interactable.md) |
| 2D style plane which supports scrolling with articulated hand input | Example script of using the system keyboard in Unity  | A script for making objects interactable with visual states and theme support |
|  [![Solver](External/ReadMeImages/Solver/MRTK_Solver_Main.png)](Documentation/README_Solver.md) [Solver](Documentation/README_Solver.md) | [![Object Collection](External/ReadMeImages/ObjectCollection/MRTK_ObjectCollection_Main.png)](Documentation/README_ObjectCollection.md) [Object Collection](Documentation/README_ObjectCollection.md) | [![Tooltip](External/ReadMeImages/Tooltip/MRTK_Tooltip_Main.png)](Documentation/README_Tooltip.md) [Tooltip](Documentation/README_Tooltip.md) |
| Various object positioning behaviors such as tag-along, body-lock, constant view size and surface magnetism | Script for lay out an array of objects in a three-dimensional shape | Annotation UI with flexible anchor/pivot system which can be used for labeling motion controllers and object. |
|  [![App Bar](External/ReadMeImages/AppBar/MRTK_AppBar_Main.png)](Documentation/README_AppBar.md) [App Bar](Documentation/README_AppBar.md) | [![Pointers](External/ReadMeImages/Pointers/MRTK_Pointer_Main.png)](Documentation/README_Pointers.md) [Pointers](Documentation/README_Pointers.md) | [![Fingertip Visualization](External/ReadMeImages/Fingertip/MRTK_FingertipVisualization_Main.png)](Documentation/README_FingertipVisualization.md) [Fingertip Visualization](Documentation/README_FingertipVisualization.md) |
| UI for Bounding Box's manual activation | Learn about various types of pointers | Visual affordance on the fingertip which improves the confidence for the direct interaction |
|  [![Eye Tracking: Target Selection](External/ReadMeImages/EyeTracking/mrtk_et_targetselect.png)](Documentation/EyeTracking/EyeTracking_TargetSelection.md) [Eye Tracking: Target Selection](Documentation/EyeTracking/EyeTracking_TargetSelection.md) | [![Eye Tracking: Navigation](External/ReadMeImages/EyeTracking/mrtk_et_navigation.png)](Documentation/EyeTracking/EyeTracking_Navigation.md) [Eye Tracking: Navigation](Documentation/EyeTracking/EyeTracking_Navigation.md) | [![Eye Tracking: Heat Map](External/ReadMeImages/EyeTracking/mrtk_et_heatmaps.png)](Documentation/EyeTracking/EyeTracking_Navigation.md) [Eye Tracking: Heat Map](Documentation/EyeTracking/EyeTracking_Visualization.md) |
| Use eyes, voice and hand input in combination for quickly selecting holograms across your scene. | Learn how to auto scroll text or fluently zoom into focused content based on what you are looking at.| Examples for logging, loading and visualizing what users have been looking at in your app. |

# Example Scene
You can find various types of interactions and UI controls in [this example scene](Documentation/README_HandInteractionExamples.md).

[![Example Scene](External/ReadMeImages/MRTK_Examples.png)](Documentation/README_HandInteractionExamples.md)

# Engage with the Community

Join the conversation around MRTK on [Slack](https://holodevelopers.slack.com/).

Ask questions about using MRTK on [Stack Overflow](https://stackoverflow.com/questions/tagged/mrtk) using the **MRTK** tag.

Search for solutions or file new issues in [GitHub](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues).

Deep dive into future plans and learn how you can contribute to MRTK in our [wiki](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki).

Join our weekly community shiproom to hear directly from the feature team. (link coming soon) 

For issues related to Windows Mixed Reality that aren't directly related to the MRTK, check out the [Windows Mixed Reality Developer Forum](https://forums.hololens.com/).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Useful Resources on Microsoft Windows Dev Center
| ![Academy](External/ReadMeImages/icon_academy.png) [Academy](https://developer.microsoft.com/en-us/windows/mixed-reality/academy)| ![Design](External/ReadMeImages/icon_design.png) [Design](https://developer.microsoft.com/en-us/windows/mixed-reality/design)| ![Development](External/ReadMeImages/icon_development.png) [Development](https://developer.microsoft.com/en-us/windows/mixed-reality/development)| ![Community)](External/ReadMeImages/icon_community.png) [Community](https://developer.microsoft.com/en-us/windows/mixed-reality/community)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| See code examples. Do a coding tutorial. Watch guest lectures.          | Get design guides. Build user interface. Learn interactions and input.     | Get development guides. Learn the technology. Understand the science.       | Join open source projects. Ask questions on forums. Attend events and meetups. |

### Learn more about MRTK Project 
You can find our planning material on [our wiki](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki) under Project Management Section. You can always see the items the team is actively working on in the Iteration Plan issue. 

### How to Contribute
View the [**How To Contribute**](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/How-to-Contribute) wiki page for the most up to date instructions on contributing to the Mixed Reality Toolkit!

### For details on the different branches used in the Mixed Reality Toolkit repositories, check this [Branch Guide here](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/Branch-Guide).
