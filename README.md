# HoloToolkit-Unity
This is effectively part of the existing HoloToolkit, but this is the repository that will contain all Unity specific components.
The HoloToolkit is a collection of scripts and components intended to accelerate development of holographic applications targeting Windows Holographic.

**Current Unity Editor Project Version: 2017.2.0b4**

**Branch: HoloToolkit-Unity Creators Update with Windows Mixed Reality support for immersive headsets.**

## Prerequisites:
1. See [Development PC specs](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools#developing_for_immersive_headsets) for tips on developing for immersive headsets.
2. [Holograms 100](https://developer.microsoft.com/en-us/windows/mixed-reality/holograms_100) has been updated with how to setup Windows Mixed Reality in your app.
3. [How to navigate the Windows Mixed Reality home](https://developer.microsoft.com/en-us/windows/mixed-reality/navigating_the_windows_mixed_reality_home).
4. Development PC needs to be on Creators Update SDK and builds as below. Either:
    1. Launch Settings > Updates and Security > Windows Insider Program > Get Insider Preview builds 
    2. Update to the publicly released Creators Update, build 15063.
5. Early access private build of Unity 5.6.1f1-MRTP9 with Creators Update API support. This build of Unity is currently invite-only.
    1. Please read more about [Immersive headset details](https://developer.microsoft.com/en-us/windows/mixed-reality/immersive_headset_details).
6. [Visual Studio 2017](https://www.visualstudio.com/downloads/).

## Please note:
1. We will not merge this branch into master until we have a publicly available Unity that supports it and a HoloLens build that works with these new APIs.
2. You can use this branch with Windows Creators Update builds on your PC and deploy it to Windows Anniversary Update HoloLens flight.
3. Windows Creators Update flights don’t ship on HoloLens.

## What's new for Immersive headsets in this branch?

**GamepadInput** supports the Xbox controller and maps it to the input manager events.
You can use the Xbox controller A to tap.
Press A and hold to do hold started, canceled and completed.
Press A and left joystick to trigger the navigation events.
Press the Y button to teleport in your world.
Press the B button to return back to your original location.

**Motion Controller** support that renders a prefab where you have the Crytal Key motion controller in your hand. Refer to [MotionControllerTest.unity](https://github.com/Microsoft/HoloToolkit-Unity/blob/Dev_Unity_2017.2.0/Assets/HoloToolkit/Input/Tests/Scenes/MotionControllerTest.unity) scene for how to use the scripts.

**Teleport** capability with new prefabs like [MixedRealityCameraParent.prefab](https://github.com/Microsoft/HoloToolkit-Unity/blob/Dev_Unity_2017.2.0/Assets/HoloToolkit/Input/Tests/Scenes/MotionControllerTest.unity) that help you [teleport](https://github.com/Microsoft/HoloToolkit-Unity/blob/Dev_Unity_2017.2.0/Assets/HoloToolkit/Input/Tests/Scenes/MotionControllerTest.unity) to different locations in your app using the gamepad.

**Stage** folder has the scripts that support the StageRoot component that helps define the 0,0,0 for your immersive applications.
Prefabs help to draw the floor for immersive headsets and also renders the stage bounds if you wish to display those in your application.

**External\How To** docs folder is meant to help everyone with migrating forward or any simple doubts they might have about the process.
Please feel free to grow all these sections. We can't wait to see your additions!

We've also done some future proofing work to add support for motion controllers. 
However, current model is gaze and commit with your head gaze and the Xbox controller.

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
8. [Stage](Assets/HoloToolkit/Stage/README.md)

To learn more about individual HoloLens feature areas, please read the [Wiki](https://github.com/Microsoft/HoloToolkit-Unity/wiki) section.

To learn how to add the HoloToolkit to your project see the [Getting Started](GettingStarted.md) guide.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
