<img src="External/ReadMeImages/MRTK_Logo_Rev.png">

# What is MixedRealityToolkit

MRTK is a Microsoft Driven opensource project. 

MRTK-Unity provides a set of foundational components and features to accelerate MR app development in Unity. Latest Release of MRTK (V2) supports HoloLens/HoloLens 2, WMR, and OpenVR platform. 

# What's MRTK-Unity good for 
 
- Provide basic features as an easy to use SDK to reduce the barrier-to-entry to get started.
- Enable rapid prototyping by providing the basic building blocks for MR app development. 
- Showcase best practices in MR with UI controls and interactions that matches the WMR and HoloLens Shell. 
- Support a wide audience, allowing solutions to be built that will run on multiple VR / AR / XR platforms such as Mixed Reality, Steam/Open VR.
- Ensure an extensive framework for advanced integrators, with the ability to swap out core components with their own should they wish to, or simply extend the framework to add new capabilities.

# Build Status

| Branch | Status |
|---|---|
| `mrtk_development` |[![Build status](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_apis/build/status/public/mrtk_development-CI)](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_build/latest?definitionId=1)|

 # Required Software
| [![Windows 10 Creators Update](External/ReadMeImages/MRTK170802_Short_17.png)](https://www.microsoft.com/software-download/windows10) [Windows 10 FCU](https://www.microsoft.com/software-download/windows10)| [![Unity](External/ReadMeImages/MRTK170802_Short_18.png)](https://unity3d.com/get-unity/download/archive) [Unity 3D](https://unity3d.com/get-unity/download/archive)| [![Visual Studio 2017](External/ReadMeImages/MRTK170802_Short_19.png)](http://dev.windows.com/downloads) [Visual Studio 2017](http://dev.windows.com/downloads)| [![Simulator (optional)](External/ReadMeImages/MRTK170802_Short_20.png)](https://go.microsoft.com/fwlink/?linkid=852626) [Simulator (optional)](https://go.microsoft.com/fwlink/?linkid=852626)|
| :--- | :--- | :--- | :--- |
| To develop apps for mixed reality headsets, you need the Windows 10 Fall Creators Update | The Unity 3D engine provides support for building mixed reality projects in Windows 10 | Visual Studio is used for code editing, deploying and building UWP app packages | The Emulators allow you test your app without the device in a simulated environment |


# Supported Platform 

The Mixed Reality Toolkit V2 will includes many APIs to accelerate the development of MR / XR / VR / AR projects for a range of supported devices, starting with

 - Microsoft HoloLens
 - Microsoft HoloLens 2
 - Microsoft Immersive headsets (IHMD)
 - OpenVR (HTC Vive / Oculus Rift)
 
# Feature areas

- Input System
- Articulated Hands + Gestures (HoloLens 2)
- Eye Tracking (HoloLens2)
- Voice Commanding
- Gaze + Select (HoloLens)
- Controller Visualization
- Teleportation
- UI Controls
- Solver and Interactions
- Spatial Understanding
- Diagnostic Tool


# Getting Started with MRTK 

You can find out how to use MRTK to develop for Windows Mixed Reality on the MS Developer Site.

The Mixed Reality team have prepared a few guides for getting up to speed on using the new Mixed Reality Toolkit, which can be found here:

* [Downloading the Mixed Reality Toolkit](Documentation/DownloadingTheMRTK.md)
* [Getting Started with the Mixed Reality Toolkit](Documentation/GettingStartedWithTheMRTK.md)
* [Mixed Reality Toolkit configuration guide](Documentation/MixedRealityConfigurationGuide.md)

Find this readme, other documentation articles and the MRTK api reference on our [MRTK Dev Portal on github.io](https://microsoft.github.io/MixedRealityToolkit-Unity/). 

# Building blocks for UI and Interactions
|  [![Button](/External/ReadMeImages/Button/MRTK_Button_Main.png)](/Documentation/README_Button.md) [Button](/Documentation/README_Button.md) | [![Bounding Box](/External/ReadMeImages/BoundingBox/MRTK_BoundingBox_Main.png)](/Documentation/README_BoundingBox.md) [Bounding Box](/Documentation/README_BoundingBox.md) | [![Manipulation Handler](/External/ReadMeImages/ManipulationHandler/MRTK_Manipulation_Main.png)](/Documentation/README_ManipulationHandler.md) [Manipulation Handler](/Documentation/README_ManipulationHandler.md) |
|:--- | :--- | :--- |
| A button control which supports various input methods including HoloLens2's articulated hand | Standard UI for manipulating objects in 3D space | Script for manipulating objects with one or two hands |
|  [![Slate](/External/ReadMeImages/Slate/MRTK_Slate_Main.png)](/Documentation/README_Slate.md) [Slate](/Documentation/README_Slate.md) | [![System Keyboard](/External/ReadMeImages/SystemKeyboard/MRTK_SystemKeyboard_Main.png)](/Documentation/README_SystemKeyboard.md) [System Keyboard](/Documentation/README_SystemKeyboard.md) | [![Interactable](/External/ReadMeImages/Interactable/InteractableExamples.png)](/Documentation/README_Interactable.md) [Interactable](/Documentation/README_Interactable.md) |
| 2D style plane which supports scrolling with articulated hand input | Example script of using the system keyboard in Unity  | A script for making objects interactable with visual states and theme support |
|  [![Solver](/External/ReadMeImages/Solver/MRTK_Solver_Main.png)](/Documentation/README_Solver.md) [Solver](/Documentation/README_solver.md) | [![Object Collection](/External/ReadMeImages/ObjectCollection/MRTK_ObjectCollection_Main.png)](/Documentation/README_ObjectCollection.md) [Object Collection](/Documentation/README_ObjectCollection.md) | [![Tooltip](/External/ReadMeImages/Tooltip/MRTK_Tooltip_Main.png)](/Documentation/README_Tooltip.md) [Tooltip](/Documentation/README_Tooltip.md) |
| Various object positioning behaviors such as tag-along, body-lock, constant view size and surface magnetism | Script for lay out an array of objects in a three-dimensional shape | Annotation UI with flexible anchor/pivot system which can be used for labeling motion controllers and object. |
|  [![App Bar](/External/ReadMeImages/AppBar/MRTK_AppBar_Main.png)](/Documentation/README_AppBar.md) [App Bar](/Documentation/README_AppBar.md) | [![Pointers](/External/ReadMeImages/Pointers/MRTK_Pointer_Main.png)](/Documentation/README_Pointers.md) [Pointers](/Documentation/README_Pointers.md) | [![Fingertip Visualization](/External/ReadMeImages/Fingertip/MRTK_FingertipVisualization_Main.png)](/Documentation/README_FingertipVisualization.md) [Fingertip Visualization](/Documentation/README_FingertipVisualization.md) |
| UI for Bounding Box's manual activation | Learn about various types of pointers | Visual affordance on the fingertip which improves the confidence for the direct interaction |

# Example Scene
You can find various types of interactions and UI controls in this example scene.

[![Button](/External/ReadMeImages/MRTK_Examples.png)](/Documentation/README_HandInteractionExamples.md)

# Engage with the Community

Join the conversation around MRTK on [Slack](https://holodevelopers.slack.com/).

Ask questions about using MRRTK on [Stack Overflow](https://stackoverflow.com/questions/tagged/mrtk).

Search for solution or file a new issue in [GitHub](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues) if you find something broken in MRTK code.

Deep dive into project plan and learn how you can contribute to MRTK in our [wiki](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki).  

Join our weekly community shiproom to hear directly from the feature team. (link to come soon) 

For issues related to Windows Mixed Reality that aren't directly related to the MRTK, check out the [Windows Mixed Reality Developer Forum](https://forums.hololens.com/).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Useful resources on Microsoft Windows Dev Center
| ![Academy](External/ReadMeImages/icon_academy.png) [Academy](https://developer.microsoft.com/en-us/windows/mixed-reality/academy)| ![Design](External/ReadMeImages/icon_design.png) [Design](https://developer.microsoft.com/en-us/windows/mixed-reality/design)| ![Development](External/ReadMeImages/icon_development.png) [Development](https://developer.microsoft.com/en-us/windows/mixed-reality/development)| ![Community)](External/ReadMeImages/icon_community.png) [Community](https://developer.microsoft.com/en-us/windows/mixed-reality/community)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| See code examples. Do a coding tutorial. Watch guest lectures.          | Get design guides. Build user interface. Learn interactions and input.     | Get development guides. Learn the technology. Understand the science.       | Join open source projects. Ask questions on forums. Attend events and meetups. |

### Learn more about MRTK Progect 
You can find our planning material on [our wiki](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki) under Project Management Section. You can always see the items the team is actively working on in the Iteration Plan issue. 

### How to Contribute
View the [**How To Contribute**](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/How-to-Contribute) wiki page for the most up to date instructions on contributing to the Mixed Reality Toolkit!

### For details on the different branches used in the Mixed Reality Toolkit repositories, check this [Branch Guide here](https://github.com/Microsoft/MixedRealityToolkit-Unity/wiki/Branch-Guide).
