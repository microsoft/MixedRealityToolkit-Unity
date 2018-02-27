# Roadmap

### Master branch
| Target Unity release  | Release timeframe | Master release tag | Toolkit release features |
| --- | --- | --- | --- |
| 2017.2.0 | COMPLETED       | v1.2017.2.0        | <ul><li>Updated master with Windows Mixed Reality support (xR namespace).</li><li>RI Dev_Unity_2017.2.0 into master.</li><li>Toolkit will work on both HoloLens and immersive headsets.</li><li>Windows Mixed Reality motion controller support.</li></ul>|
| 2018.1.0              | May 2018      | 2018.1.0        | <ul><li>Mixed Reality Toolkit Version Next.</li></ul>|

# MRTK Version Next
This new version of the MixedRealityToolkit aims to further extend the capabilities of the toolkit and also introduce new features, including the capability to support more VR/AR/XR platforms beyond Microsoft's own Mixed Reality setup.

The vNext branch is taking all the best lessons learned from the original Mixed Reality Toolkit and refactoring / restructuring it to both:

Support a wider audience, allowing solutions to be built that will run on multiple VR / AR / XR platforms such as Mixed Reality, Steam/Open VR and OpenXR (initially)

Provide an easier to use SDK, to enable rapid prototyping and ease adoption for new users (or users of previous frameworks)

Ensure an extensive framework for advanced integrators, with the ability to swap out core components with their own should they wish to, or simply extend the framework to add new capabilities.

> Learn more about the architecture behind [Windows Mixed Reality - vNext](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/MRTK-Version-Next/MRTK-vNext.md) here.

> Learn more about the approach behind the [Windows Mixed Reality - vNext SDK](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/MRTK-Version-Next/MRTK-SDK.md) here.

# Future work planning
- Automated build for MRTK-Unity.
- Building a plan for cross-device/platform VR support.
- Adding 'Standalone' folder containing legacy toolkit scripts that don't depend on the toolkit input module to address developer feedback.
- Unity packages for each release will be published to the Unity asset store.
- Unity packages for each feature area like Input/SpatialMapping will also be added to the release notes. More work needs to happen to ensure feature areas don't have cross dependencies.
- [Academy content](https://github.com/Microsoft/HolographicAcademy) will be updated with each major toolkit release.
- Updating API documentation using tools like Doxygen and not writing them manually. This is something we will work with all of you on.
- Update Wiki to host API documentation instead.
- Move reusable features from [Mixed Reality Design Labs](https://github.com/Microsoft/MRDesignLabs_Unity) project into toolkit.
- Improved samples and test cases

# Release cadence
### Master branch
- **No direct PR's into master branch.** It will be locked and only merges from stable development branches will be taken.
- Exception will be major bug fixes on a case by case basis.
- Please use the [working development branch](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/Dev_Working_Branch) for daily feature work.
- [Working development branch](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/Dev_Working_Branch) will be snapshot every 3 weeks for stabilization into a separate branch. Working branch will still be open for contributions.
- That stabilization branch will be worked on for at least 1 week.
- Only bug fixes will be accepted in the stabilization branch before being merged into master.

- **Master branch releases** will align with **major Unity releases marking a release following Unity cadence**. Please read the [Unity roadmap](https://unity3d.com/unity/roadmap).
- Each release will be marked as a GitHub [release tag](https://github.com/Microsoft/HoloToolkit-Unity/releases). You can consume this release by:
	- Using the zip file available in the release notes
	- Unity packages available in the release notes
	- Syncing to the specific git tag you would like to lock on.
- Release tag name convention: <Unity release major number>.<Unity release minor number>.Iteration number
	- For example: For Unity version 2017.1.0 our release tag would be **2017.1.0.0**
	- If we marked another release with the same Unity version: **2017.1.0.1**


### Development branches
- Development branches are great for incubating features before they make their way into master.
- These branches can be feature work or experimental features.
- Development branches might not be on the same Unity versions as master.
- For being merged into Master the dev branch will have to follow the cadence laid out for master.
- Development branches might choose to mark releases on a faster cadence based on customer/project needs.
- Recommend marking a development branch tag every 2-3 weeks as needed to ensure stability before making up to master.
- Development branches might be deleted once merged into master. If more work is pending, it's ok to continue working in them.
