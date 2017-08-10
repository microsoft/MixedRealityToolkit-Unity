<img src="External/ReadMeImages/MRTK_Logo_DarkBG7.png">

# What is Mixed Reality Toolkit (MRTK)?
The Mixed Reality Toolkit is a collection of scripts and components intended to accelerate development of applications targeting Microsoft HoloLens and Windows Mixed Reality headsets.
The project is aimed at reducing barriers to entry to create mixed reality applications and contribute back to the community as we all grow.

MixedRealityToolkit-Unity uses code from the base MixedRealityToolkit repository and makes it easier to consume in [Unity](https://unity3d.com/).

<img src="External/ReadMeImages/MixedRealityStack.png">

Learn more about [mixed reality](https://developer.microsoft.com/en-us/windows/mixed-reality/mixed_reality).

[github-rel]:                 https://github.com/Microsoft/HoloToolkit-Unity/releases/latest
[github-rel-badge]:            https://img.shields.io/github/tag/microsoft/holotoolkit-unity.svg?style=flat-square&label=release&colorB=007ec6
[![Github Release][github-rel-badge]][github-rel]

# Feature areas
| ![Input](External/ReadMeImages/MRTK170802_Short_03.png) [Input](Assets/HoloToolkit/Input/README.md)| ![Sharing](External/ReadMeImages/MRTK170802_Short_04.png) [Sharing](Assets/HoloToolkit/Sharing/README.md)| ![Spatial Mapping](External/ReadMeImages/MRTK170802_Short_05.png) [Spatial Mapping](Assets/HoloToolkit/SpatialMapping/README.md)| ![Spatial Understanding](External/ReadMeImages/MRTK170802_Short_06.png) [Spatial Understanding](Assets/HoloToolkit/SpatialUnderstanding/README.md)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| Scripts that leverage inputs such as gaze, gesture, voice and motion controllers.             | Sharing library enables collaboration across multiple devices.  | Scripts that allow applications to bring the real world into the digital using HoloLens.        | Tailor experiences based on room semantics like couch, wall etc. |
| ![Spatial Sound](External/ReadMeImages/MRTK170802_Short_09.png) [Spatial Sound](Assets/HoloToolkit/SpatialSound/README.md)| ![UI Controls](External/ReadMeImages/MRTK170802_Short_10.png) [UI Controls](Assets/HoloToolkit/UI/README.md)| ![Utilities](External/ReadMeImages/MRTK170802_Short_11.png) [Utilities](Assets/HoloToolkit/Utilities/README.md)| ![Build](External/ReadMeImages/MRTK170802_Short_12.png) [Build](Assets/HoloToolkit/Build/README.md)|
| Scripts to help plug spatial audio into your application.           | Building blocks for creating good UI in your application like common controls.    | Common helpers and tools that you can leverage in your application.      | Build and deploy automation window for Unity Editor.|


# Required Software
| ![Windows 10 Creators Update](External/ReadMeImages/MRTK170802_Short_17.png) [Windows 10 Creators Update](https://www.microsoft.com/software-download/windows10)| ![Unity](External/ReadMeImages/MRTK170802_Short_18.png) [Unity](https://unity3d.com/get-unity/download/archive?_ga=2.81762199.1436901961.1502315389-1970488254.1488922991)| ![Visual Studio 2017](External/ReadMeImages/MRTK170802_Short_19.png) [Visual Studio 2017](http://dev.windows.com/downloads)| ![Hololens Emulator(optional)](External/ReadMeImages/MRTK170802_Short_20.png) [Hololens Emulator(optional)](https://go.microsoft.com/fwlink/?linkid=852626)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| To develop apps for mixed reality headsets, you need Creators Update.           | The Unity engine is an easy way to get started building a mixed reality app.     | Visual Studio is used for code editing, deploying and building UWP app packages.       | Emulator allows you test your app without the device in a simulated environment. |

# Getting started with MRTK
| ![Hololens Emulator(optional)](External/ReadMeImages/MRTK170802c_Short_22.png) Quick start | ![Hololens Emulator(optional)](External/ReadMeImages/MRTK170802c_Short_23.png) Contributing to this project |
| :--------------------- | :----------------- |
| Please go over the [Getting started guide](GettingStarted.md) to learn more about getting off the ground quickly. | Please go over the [Contributing guidelines](CONTRIBUTING.md) to learn more about the process and thinking. |


# Roadmaps
### Master branch
| Target Unity release  | Release timeframe | Master release tag | Toolkit release features |
| --------------------- | ----------------- | ------------------ | ------------------------ |
| 2017.1.0              | Early Aug 2017    | v1.2017.1.0        | - Updating to Unity's beta build. - 3D Keyboard for text entry.|
| 2017.2.0              | End Sep 2017      | v1.2017.2.0        | - Updating master with Windows Mixed Reality support (xR namespace). - RI Dev_Unity_2017.2.0 into master. - Toolkit will work on both HoloLens and immersive headsets. - Windows Mixed Reality motion controller support.|
| 2017.3.0              | End Dec 2017      | v1.2017.3.0        | - 360 video player prefabs. - Scriptable foveated rendering prefabs. |
### Dev_Unity_2017.2.0 branch

| Target Unity release  | Release timeframe | Branch release tag | Branch pre-release features | Status |
| --------------------- | ----------------- | ------------------ | --------------------------- | --------------------------- |
| MRTP9                 | End July 2017     | v1.Dev.MRTP9       | - Windows Mixed Reality support for working on both HoloLens and immersive headsets. - Gamepad support. - Windows Mixed Reality motion controller support. | [Complete: v1.Dev.MRTP9](https://github.com/Microsoft/HoloToolkit-Unity/releases/tag/v1.Dev.MRTP9) |
| MRTP13                | Early Aug 2017    | v1.Dev.MRTP13      | - Updating to MRTP13. - Windows Mixed Reality motion controller rendering with glTF format read from WU driver. | [Complete: v1.Dev.MRTP13](https://github.com/Microsoft/HoloToolkit-Unity/releases/tag/v1.Dev.MRTP13) |
| 2017.2.0              | Mid Aug 2017      | v1.Dev.2017.2.0    | - Adapting to Unity's breaking changes of xR namespace and others. | In progress |
| 2017.2.0              | End Sep 2017      | v1.Dev.2017.2.1    | - Merge branch into master. | Not started. |

# Upcoming Breaking Changes

- **Renaming HoloToolkit-Unity repository to MixedRealityToolkit-Unity** to align with product direction.
- Technically **all your checkins and redirect links will continue to work as-is** but we wanted to give a better heads up on this.
- All other dependent repositories will undergo a similar name change.
- We are **not breaking toolkit folder names and namespaces at this time.**
- Instead we are taking a staggered approach for breaking changes based on developer feedback.

| Breaking change description                     | Release timeframe | Notes                    |
| ----------------------------------------------- | ----------------- | ------------------------ |
| Rename repository to MixedRealityToolkit-Unity. | Mid Aug 2017      | - Recommend you do: $git remote set-url origin new_url. - Recommend reading: https://help.github.com/articles/renaming-a-repository ; https://github.com/blog/1508-repository-redirects-are-here |
| Updating toolkit namespace to MixedReality      | Nov 2017          | - Update folder names, class names and namespace names post 2017.2.0 release. |

# Future work planning
- Updating landing page for the ReadMe.md to help app developers find content better.
- Updating API documentation using tools like Doxygen and not writing them manually. This is something we will work with all of you on.
- Update Wiki to host API documentation instead.
- Move reusable features from MixedReality Design Labs project into toolkit.

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

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
