# HoloToolkit-Unity
This is effectively part of the existing HoloToolkit, but this is the repository that will contain all Unity specific components.
The HoloToolkit is a collection of scripts and components intended to accelerate development of holographic applications targeting Windows Holographic.

## Required Software
- Unity Editor Version: [2017.1.0f3](https://unity3d.com/unity/whats-new/unity-2017.1.0)
- Visual Studio 2017

## Contributing to this project:
Please go over the [Contributing guidelines](CONTRIBUTING.md) to learn more about the process and thinking.

## Wondering how to simply get started?
Please go over the [Getting started guide](GettingStarted.md) to learn more about just getting off the ground quickly.

## Release cadence for Master:
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

## Roadmap for Master:

| Target Unity release  | Release timeframe | Master release tag | Toolkit release features |
| --------------------- | ----------------- | ------------------ | ------------------------ |
| 2017.1.0              | Early Aug 2017    | v1.2017.1.0        | - Updating to Unity's beta build. - 3D Keyboard for text entry.|
| 2017.2.0              | End Sep 2017      | v1.2017.2.0        | - Updating master with Windows Mixed Reality support (xR namespace). - RI Dev_Unity_2017.2.0 into master. - Toolkit will work on both HoloLens and immersive headsets. - Windows Mixed Reality motion controller support.|
| 2017.3.0              | End Dec 2017      | v1.2017.3.0        | - 360 video player prefabs. - Scriptable foveated rendering prefabs. |

## Release cadence for development branches:
- Development branches are great for incubating features before they make their way into master.
- These branches can be feature work or experimental features.
- Development branches might not be on the same Unity versions as master.
- For being merged into Master the dev branch will have to follow the cadence laid out for master.
- Development branches might choose to mark releases on a faster cadence based on customer/project needs.
- Recommend marking a development branch tag every 2 weeks as needed to ensure stability before making up to master.
- Development branches might be deleted once merged into master. If more work is pending, it's ok to continue working in them.

## Roadmap for branch Dev_Unity_2017.2.0:

| Target Unity release  | Release timeframe | Branch release tag | Branch pre-release features |
| --------------------- | ----------------- | ------------------ | ------------------------ |
| MRTP9                 | End July 2017     | v1.Dev.MRTP9       | - Windows Mixed Reality support for working on both HoloLens and immersive headsets. - Gamepad support. - Windows Mixed Reality motion controller support. |
| MRTP13                | Early Aug 2017    | v1.Dev.MRTP13      | - Updating to MRTP13. - Windows Mixed Reality motion controller rendering with glTF format read from WU driver. |
| 2017.2.0              | Mid Aug 2017      | v1.Dev.2017.2.0    | - Adapting to Unity's breaking changes of xR namespace and others. |
| 2017.2.0              | End Sep 2017      | v1.Dev.2017.2.1    | - Merge branch into master. |

## Upcoming Breaking Changes:

- **Renaming HoloToolkit-Unity repository to MixedRealityToolkit-Unity** to align with product direction.
- Technically **all your checkins and redirect links will continue to work as-is** but we wanted to give a better heads up on this.
- All other dependent repositories will undergo a similar name change.
- We are **not breaking toolkit folder names and namespaces at this time.**
- Instead we are taking a staggered approach for breaking changes based on developer feedback.

| Breaking change description                     | Release timeframe | Notes                    |
| ----------------------------------------------- | ----------------- | ------------------------ |
| Rename repository to MixedRealityToolkit-Unity. | Mid Aug 2017      | - Recommend you do: $git remote set-url origin new_url. - Recommend reading: https://help.github.com/articles/renaming-a-repository ; https://github.com/blog/1508-repository-redirects-are-here |
| Updating toolkit namespace to MixedReality      | Nov 2017          | - Update folder names, class names and namespace names post 2017.2.0 release. |

## Future work planning:
- Updating landing page for the ReadMe.md to help app developers find content better.
- Updating API documentation using tools like Doxygen and not writing them manually. This is something we will work with all of you on.
- Update Wiki to host API documentation instead.
- Move reusable features from MixedReality Design Labs project into toolkit.

## HoloToolkit contains the following feature areas:

1. [Input](Assets/HoloToolkit/Input/README.md)
2. [Sharing](Assets/HoloToolkit/Sharing/README.md)
3. [Spatial Mapping](Assets/HoloToolkit/SpatialMapping/README.md)
4. [Spatial Understanding](Assets/HoloToolkit/SpatialUnderstanding/README.md)
5. [Spatial Sound](Assets/HoloToolkit/SpatialSound/README.md)
6. [Utilities](Assets/HoloToolkit/Utilities/README.md)
7. [Build](Assets/HoloToolkit/Build/README.md)

To learn more about individual HoloLens feature areas, please read the [Wiki](https://github.com/Microsoft/HoloToolkit-Unity/wiki) section.

To learn how to add the HoloToolkit to your project see the [Getting Started](GettingStarted.md) guide.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
