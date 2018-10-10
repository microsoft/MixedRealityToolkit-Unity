# Roadmap

## Upcoming releases

| Release | Development branch | Timeline | Project board |
| --- | --- | --- | --- |
| [MRTK vNext](#mrtk-version-next) | mrtk_development | October 2018 | [MRTK vNext](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/5) |
| 2017.4.3.0 | htk_development | Q1 2019 | [2017.4.3.0](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/17) |

## Most recent releases

| Release | Branch | Tag | Project board | Tested Unity Versions |
| --- | --- | --- | --- | --- |
| [2017.4.2.0](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/2017.4.2.0) | master | 2017.4.2.0 | [2017.4.2.0](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/15) | 2017.1 - 2017.4 |
| [2018.8.0.0_Alpha2.1](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/2018.8.0.0_Alpha2.1) | mrtk_release | 2018.8.0.0_Alpha2.1 | N/A | 2018.1 |

## Master branch

Releases from [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master) are targeted, primarily at the 2017.4 LTS release from Unity and support Windows Mixed Reality ([Microsoft HoloLens](https://www.microsoft.com/en-us/hololens) and [Immersive](https://docs.microsoft.com/en-us/windows/mixed-reality/immersive-headset-hardware-details)) devices. The MRTK team strives to maintain compatibility for older versions of Unity (2017.1 and newer). It is expected that newer releases (2018.1 and newer) will continue to work.

Please file [issues](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues) if you encounter issues with specific versions of Unity.

### Release cadence

The master branch is on a quarterly release cadence (ex: 2017.4.2 in September 2018, 2017.4.3 in Q1 2019, etc.). Precise timing within the month may vary based on planned features and number of issues being addressed.

As needed, patch and hot fix releases will be released between the scheduled cadence.

### Note on version numbers

Starting with the May 2018 release, the MRTK version numbers are adopting [Semantic versioning](https://semver.org/) rather than aligning with a specific Unity release.

The MRTK will use the first version field to represent the product, and should coincide with the intended Unity release (2017, 2018 etc.) Product changes may or may not contain breaking changes, but should be evaluated as high risk to update across. The second number will represent a release, which may add new features or contain breaking changes from the prior release; this should be considered a medium risk update. The third number will be revision, which should contain only bug fixes or minor changes for a release and be safe to upgrade to. The fourth will always be zero (2017.4.2.0). Be sure to always check the release notes for what has changed.

### Development branches

For the [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master) branch releases, development is being done in [htk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/htk_development). Please refer to the [Upcoming releases table](#upcoming-releases), or the [Projects](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects) page for specifics.

Please see [Branch management plan](#branch-management-plan) for more details.

## MRTK Version Next

MixedRealityToolkit Version Next aims to further extend the capabilities of the toolkit and also introduce new features, including the capability to support more AR / XR / VR platforms beyond Microsoft's own Windows Mixed Reality platform.

The vNext branch is taking all the best lessons learned from the original Mixed Reality Toolkit and refactoring / restructuring it to both:

Support a wider audience, allowing solutions to be built that will run on multiple AR / XR / VR platforms such as Windows Mixed Reality, Steam/Open VR and OpenXR (initially)

Provide an easier to use SDK, to enable rapid prototyping and ease adoption for new users (or users of previous frameworks)

Ensure an extensive framework for advanced integrators, with the ability to swap out core components with their own should they wish to, or simply extend the framework to add new capabilities.

> Learn more about the architecture behind [Windows Mixed Reality - vNext](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/MRTK-vNext.md) here.
>
> Learn more about the approach behind the [Windows Mixed Reality - vNext SDK](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/MRTK-SDK.md) here.

### Milestones

The current plan for the development of Version Next is detailed in the following table. Please note that releases and dates are subject to change.

| Release | Development branch | Timeline | Project board |
| --- | --- | --- | --- |
| Beta (2018.9.0.0) | [mrtk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_development) | September 2018 | [2018.9.0.0](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/12) |
| 2018.10.0.0 | [mrtk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_development) | October 2018 | [Version Next Beta](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/14) |

### Development

All development for Version Next is being done in [mrtk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_development).

Please see the [branch management plan](#branch-management-plan) for more details.

### Versioning

The MRTK Version Next is adopting [Semantic versioning](https://semver.org/) rather than aligning with a specific Unity release.

Version Next will use the first two version fields to indicate the release year and month (ex: 2018.6). The third field will indicate the MRTK revision number (ex: 2018.6.1). The fourth will always be zero (2018.6.1.0).

## Branch management plan

### Pre-Version Next release

Prior to the release of Version Next, the repository's default branch is [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master).

Each release based on [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master) will be worked on in htk_development. If work needs to be done for a future release before the current release is locked, additional branches will be created as necessary.

Work for MRTK vNext is being done in the following branches:

- [mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release), for marking releases. No non-release PRs should be opened directly into this branch.
- [mrtk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_development), the single development branch for MRTK work.

### Post-Version Next release

With the release of Version Next, the default branch for the repository will change to mrtk_release and [master](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/master)  will be branched to become htk_release. At the time of the branching, master will be deleted.

### Sustained engineering

As appropriate (critical, wide-impact issues), htk_development will be branched for development and stabilization of a hotfix.

## Future work planning

- Automated build for MRTK-Unity.
- Unity packages for each release will be published to the Unity asset store.
- Investigate using the Unity Package Manager to modularize the MRTK.
- [Academy content](https://github.com/Microsoft/HolographicAcademy) will be updated with each major toolkit release.
- Updating API documentation using tools like DocFx and not writing them manually. This is something we will work with all of you on.
- Improved samples and test cases.
