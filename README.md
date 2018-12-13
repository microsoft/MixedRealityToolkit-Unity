<img src="External/ReadMeImages/MRTK_Logo_Rev.png">

# What is MixedRealityToolkit-vNext
The V1 Mixed Reality Toolkit is a collection of scripts and components intended to accelerate development of applications targeting Microsoft HoloLens and Windows Mixed Reality headsets.

> ### For details on the different branches used in the Mixed Reality Toolkit repositories, check this [Branch Guide here](/BranchGuide.md).

This new version of the MixedRealityToolkit aims to further extend the capabilities of the toolkit and also introduce new features, including the capability to support more VR/AR/XR platforms beyond Microsoft's own Mixed Reality setup.

The vNext branch is taking all the best lessons learned from the original Mixed Reality Toolkit and refactoring / restructuring it to both:

* Support a wider audience, allowing solutions to be built that will run on multiple VR / AR / XR platforms such as Mixed Reality,  Steam/Open VR and OpenXR (initially)

* Provide an easier to use SDK, to enable rapid prototyping and ease adoption for new users (or users of previous frameworks)

* Ensure an extensive framework for advanced integrators, with the ability to swap out core components with their own should they wish to, or simply extend the framework to add new capabilities.

> Learn more about [Windows Mixed Reality](https://www.microsoft.com/en-gb/windows/windows-mixed-reality) here.

> Learn more about the architecture behind [Windows Mixed Reality - vNext](/MRTK-vNext.md) here.

> Learn more about the approach behind the [Windows Mixed Reality - vNext SDK](/MRTK-SDK.md) here.

# Feature areas
The Mixed Reality Toolkit vNext will includes many APIs to accelerate the development of MR / XR / VR / AR projects for a range of supported devices, including (but not limited to)

 - Microsoft HoloLens
 - Microsoft Immersive headsets (IHMD)
 - Steam VR (HTC Vive / Oculus Rift)
 - OpenXR platforms

 # Required Software
| [![Windows 10 Creators Update](External/ReadMeImages/MRTK170802_Short_17.png)](https://www.microsoft.com/software-download/windows10) [Windows 10 FCU](https://www.microsoft.com/software-download/windows10)| [![Unity](External/ReadMeImages/MRTK170802_Short_18.png)](https://unity3d.com/get-unity/download/archive) [Unity 3D](https://unity3d.com/get-unity/download/archive)| [![Visual Studio 2017](External/ReadMeImages/MRTK170802_Short_19.png)](http://dev.windows.com/downloads) [Visual Studio 2017](http://dev.windows.com/downloads)| [![Simulator (optional)](External/ReadMeImages/MRTK170802_Short_20.png)](https://go.microsoft.com/fwlink/?linkid=852626) [Simulator (optional)](https://go.microsoft.com/fwlink/?linkid=852626)|
| :--- | :--- | :--- | :--- |
| To develop apps for mixed reality headsets, you need the Windows 10 Fall Creators Update | The Unity 3D engine provides support for building mixed reality projects in Windows 10 | Visual Studio is used for code editing, deploying and building UWP app packages | The Emulators allow you test your app without the device in a simulated environment |

# Getting started with MRTK-vNext

The Mixed Reality team have prepared a few guides for getting up to speed on using the new Mixed Reality Toolkit, which can be found here:

* [Downloading the Mixed Reality Toolkit](/Documentation/DownloadingTheMRTK.md)
* [Getting Started with the Mixed Reality Toolkit](/Documentation/GettingStartedWithTheMRTK.md)
* [Mixed Reality Toolkit configuration guide](/Documentation/MixedRealityConfigurationGuide.md)

More guides to follow.


> Any queries, questions or feedback on using the Mixed Reality Toolkit should be [raised as Issues](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues) on the site.  let us know what you like / dislike or think is incredibly awesome!
> 
> Alternatively, reach out to us on the [HoloDevelopers slack channels](https://holodevelopersslack.azurewebsites.net/)


# Examples and QuickStart scenes
One radical change to the Mixed Reality Toolkit vNext, will be the standards and approaches to real world example scenes.

New examples will follow strict guidelines, such as:

* Each example must have a use and demonstrate a real world test case (no tests).
* Each example will use a standardized template, so all examples have the same look and feel.
* Each sample will be fully documented, detailing both the use case it is demonstrating and how to implement the features demonstrated.

> Check the "Work In Progress" section of the [Windows Mixed Reality - vNext SDK](/MRTK-SDK.md) for a peek at the first new example.


**External\How To** docs folder is meant to help everyone with migrating forward or any simple doubts they might have about the process.
Please feel free to grow all these sections. We can't wait to see your additions!

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Useful resources on Microsoft Windows Dev Center
| ![Academy](External/ReadMeImages/icon_academy.png) [Academy](https://developer.microsoft.com/en-us/windows/mixed-reality/academy)| ![Design](External/ReadMeImages/icon_design.png) [Design](https://developer.microsoft.com/en-us/windows/mixed-reality/design)| ![Development](External/ReadMeImages/icon_development.png) [Development](https://developer.microsoft.com/en-us/windows/mixed-reality/development)| ![Community)](External/ReadMeImages/icon_community.png) [Community](https://developer.microsoft.com/en-us/windows/mixed-reality/community)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| See code examples. Do a coding tutorial. Watch guest lectures.          | Get design guides. Build user interface. Learn interactions and input.     | Get development guides. Learn the technology. Understand the science.       | Join open source projects. Ask questions on forums. Attend events and meetups. |

# Building the Artifacts

## Requirements

### NuGet
[NuGet](https://www.nuget.org/downloads) is the package manager for .Net and you'll need to have it available in the PATH.

### UnitySetup
The build process leverages [UnitySetup](https://www.powershellgallery.com/packages/UnitySetup), an OSS PowerShell Module from Microsoft. 

Install from PowerShell:

```powershell
Install-Module UnitySetup -Scope CurrentUser
```

### Git
If you do not specify a version, then [Git](https://git-scm.com/downloads) is used to find relevant tags. In this case it will need to be available in the PATH.

## Run the Build
Simply execute the build script as such:

```powershell
.\build.ps1 -Version '1.2.3'
```
For help and examples simply use the PowerShell help command:
```
help .\build.ps1 -Detailed
```

> Note: If you don't specify `-Version <version>` the script will try to infer it from tags pointing to the current git commit. An error is produced if you don't have a tag and no version is provided.
| See code examples. Do a coding tutorial. Watch guest lectures.          | Get design guides. Build user interface. Learn interactions and input.     | Get development guides. Learn the technology. Understand the science.       | Join open source projects. Ask questions on forums. Attend events and meet-ups. |