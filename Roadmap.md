# Roadmap

### Upcoming releases
| Release | Development branch | Timeline | Project board |
| --- | --- | --- | --- |
| 2017.4.0.0 | may18_dev | May 2018 | [May 2018](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/8) |
| 2017.4.1.0 | june18_dev | June 2018 | [June 2018](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/9) |
| [vNext](#mrtk-version-next) | MRTK-Version-Next | Summer 2018 | [MRTK vNext](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/5) |

### Most recent releases
| Release | Branch | Tag | Project board | Tested Unity Versions |
| --- | --- | --- | --- | --- |
| [2017.2.1.4](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/2017.2.1.4) | master | 2017.2.1.4 | [2017.2.1.4](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/7) | 5.6.5 - 2017.4.1 |

# Master branch
Releases from [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master) are targeted, primarily at the 2017 LTS release from Unity and support Windows Mixed Reality ([Microsoft HoloLens](https://www.microsoft.com/en-us/hololens) and [Immersive](https://docs.microsoft.com/en-us/windows/mixed-reality/immersive-headset-hardware-details)) devices. The MRTK team strives to maintain compatibility for older versions of Unity (2017.1 and newer). It is expected that newer releases (2018.1 and newer) will continue to work.

Please file [issues](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues) if you encounter issues with specific versions of Unity.

## Release cadence
The master branch is on a monthly release cadence (ex: 2017.2.1.4 in April 2018, 2017.4.0.0 in May 2018, etc.). Precice timing within the month may vary based on planned features and number of issues being addressed.

As needed, patch and hot fix releases will be released between the sceduled cadence.

### Note on version numbers
Starting with the May 2018 release, the MRTK version numbers are adopting [Semantic versioning](https://semver.org/) rather than aligning with a specific Unity release.

The MRTK will use the first two version fields (2017.4) to indicate the recommended version is the Unity 2017 LTS version. The third field will indicate the MRTK revision number (ex: 2017.4.2). The fourth will always be zero (2017.4.2.0).

## Development branches
For the [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master) branch releases, there is a unique developement / stabilization branch for each release. Please refer to the [Upcoming releases table](#upcoming-releases), or the [Projects](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects) page for specifics.

Please see [Branch management plan](#branch-management-plan) for more details.


# MRTK Version Next
MixedRealityToolkit Version Next aims to further extend the capabilities of the toolkit and also introduce new features, including the capability to support more VR/AR/XR platforms beyond Microsoft's own Windows Mixed Reality platform.

The vNext branch is taking all the best lessons learned from the original Mixed Reality Toolkit and refactoring / restructuring it to both:

Support a wider audience, allowing solutions to be built that will run on multiple VR / AR / XR platforms such as Windows Mixed Reality, Steam/Open VR and OpenXR (initially)

Provide an easier to use SDK, to enable rapid prototyping and ease adoption for new users (or users of previous frameworks)

Ensure an extensive framework for advanced integrators, with the ability to swap out core components with their own should they wish to, or simply extend the framework to add new capabilities.

> Learn more about the architecture behind [Windows Mixed Reality - vNext](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/MRTK-Version-Next/MRTK-vNext.md) here.

> Learn more about the approach behind the [Windows Mixed Reality - vNext SDK](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/MRTK-Version-Next/MRTK-SDK.md) here.

## Release cadence
Following the Alpha release (June 2018), releases (beta and official) are planned to be released on a monthly cadence.

## Milestones
The current plan for the development of Version Next is detailed in the following table. Please note that releases and dates are subject to change.

| Release | Development branch | Timeline | Project board |
| --- | --- | --- | --- |
| Alpha (2018.6.0.0) | MRTK-Version-Next | June 2018 | tbd |
| Beta 1 (2018.7.0.0) | development_MRTK | July 2018 | tbd |
| Beta 2 (2017.8.0.0) | development_MRTK | August 2018 | tbd |
| 2018.9.0.0 | development_MRTK | September 2018 | tbd | 

## Development branches
During the Alpha milestone, the development branch will be MRTK-Version-Next.

Post-alpha releases will share a development branch with each release having a unique stabilization branch.

Please see [Branch management plan](#branch-management-plan) for more details.

### Versioning

The MRTK Version Next is adopting [Semantic versioning](https://semver.org/) rather than aligning with a specific Unity release.

Version Next will use the first two version fields to indicate the release year and month (ex: 2018.6). The third field will indicate the MRTK revision number (ex: 2018.6.1). The fourth will always be zero (2018.6.1.0). 

# Branch mangement plan

## Pre-Version Next release
Prior to the release of Version Next, the repository's default branch is [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master).

Each release based on [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master) will have a unique development / stabilization branch where work will be performed and from which release candidates will be created.

While the Version Next Alpha release is being developed, there will be a single [vNext branch](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/MRTK-Version-Next).

At the time that feature complete is declared for the Alpha release, three new branches will be created:
- release_MRTK (replaces MRTK-Version-Next)
- development_MRTK (single development branch for future MRTK work)
- alpha (stabilization branch for the Alpha release) 

Once the branches are created, MRTK-Version-Next will be deleted and [Dev_Working_Branch](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/Dev_Working_Branch) will be locked (no Pull Requests accepted).

## Post-Version Next release
With the release of Version Next, the default branch for the repository will change to release_MRTK and [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master)  will be branched to become HoloToolkit_SustainedEngineering. At the time of the branching, master will be deleted.

### Sustained engineering
As appropriate (critical, wide impace issues) HoloToolkit_SustainedEngineering will be branched for development and stabilization of a hotfix.


# Future work planning
- Automated build for MRTK-Unity.
- Unity packages for each release will be published to the Unity asset store.
- Investigate using the Unity Package Manager to modularize the MRTK.
- [Academy content](https://github.com/Microsoft/HolographicAcademy) will be updated with each major toolkit release.
- Updating API documentation using tools like DocFx and not writing them manually. This is something we will work with all of you on.
- Improved samples and test cases
