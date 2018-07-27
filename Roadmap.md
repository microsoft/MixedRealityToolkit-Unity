# Roadmap

This document outlines the roadmap of the Mixed Reality Toolkit.

# Upcoming Releases

The following table lists the releases planned through the end of 2018 for the Mixed Reality Toolkit. Release details, including backlog items, can be found on the [GitHub project pages](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects).

| Product | Version | Timeline | Project board |
| --- | --- | --- | --- |
| [Mixed Reality Toolkit](#mixed-reality-toolkit-mrtk-roadmap) | 2018.7.0.0 (Alpha) | July 2018 | [Version Next Alpha](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/11) |
| [Mixed Reality Toolkit](#mixed-toolkit-toolkit-roadmap) | 2018.8.0.0 (Beta) | August 2018 | [Version Next Beta](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/12) |
| [Mixed Reality Toolkit](#mixed-reality-toolkit-roadmap) | 2018.9.0.0 | September 2018 | [2018.9.0.0 (Version Next initial release)](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects/14) |
| [Mixed Reality Toolkit](#mixed-toolkit-toolkit-roadmap) | 2018.10.0.0 | October 2018 | |
| [Mixed Reality Toolkit](#mixed-toolkit-toolkit-roadmap) | 2018.11.0.0 | November 2018 | |

Release details, including backlog items, can be found on the [GitHub project pages](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects).

# Mixed Reality Toolkit (MRTK) Roadmap

The Mixed Reality Toolkit (also known as "vNext") is an all-new product, built to be cross MR/AR/VR/XR platform by design. There are two planned pre-releases after which the Mixed Reality Toolkit will become the primary product.

The Mixed Reality Toolkit will require Unity 2018 or newer.

> When Unity releases an LTS (Long Term Support) product, the Mixed Reality Toolkit will branch (ex: mrtk_2018_LTS) and require an LTS release. This branch will be targeted at developers who wish to have the most stable MRTK on which to build. It is expected that the branched Mixed Reality Toolkit will receive bug fixes and no new features. The mrtk_development branch will update to the next Unity Tech release (ex: 2019.1) and **may** drop backward compatibility.

## New Architecture

The MRTK is being rewritten to be cross AR/VR/MR/XR platform and to be more modular. The core layer will contain the interfaces and definitions required to build manager components (ex: input manager) as well as the abstractions needed to support multiple platforms. The SDK layer is where pre-built / reusable components will be delivered. The SDK will be further modularized to make it simple to opt-in to specific functionality (ex: UX controls).

## July 2018 - Alpha

The Mixed Reality Toolkit (MRTK) Alpha, released in late July 2018 is being released as a single package (core, sdk and examples) as the team introduces the new architecture. This release does **not** contain all of the features and functionality of the HoloToolkit releases.

The theme(s) for the Alpha release are

- Minimum Viable Product
    - New architecture
    - VR Boundary support
    - Input and Interactions
    - Locomotion / Teleportation
    - Standard shader

Supported platforms 

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
- OpenVR

## August 2018 - Beta

The MRTK Beta release will introduce initial support for modularization. It is also when the vast majority of HoloToolkit features will be added. Supported platforms will be unchanged from the Alpha release.

> It is important to note that the goal is feature parity and **not** zero app code change. There will be consolidation of duplicated functionality as well as namespace, component and API changes.

The theme(s) for the Beta release are

- HoloToolkit feature parity
    - Not including the HoloLens-iOS Spectator View feature
- Improved Core and SDK layer alignment
    - Components are expected to move to their "final" layer
- Core and SDK feature packages
    - SDK features (ex: UX controls) will be in separate packages 
- Bug fixes

Supported platforms 

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
- OpenVR

## September 2018 - First Release

The September 2018 (2018.9.0.0) release marks the first official release of MRTK vNext and will support the same platforms as the Beta. With this release, the Mixed Reality Toolkit will replace the HoloToolkit, which will be transitioned into sustained engineering mode.

The theme(s) for the 2018.9.0.0 release are

- Fit and finish work
    - Package management UI
- Bug fixes

Supported platforms 

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
- OpenVR

## October 2018

The October 2018 (2018.10.0.0) release is planned to be the first update to add additional device support.

The theme(s) for the 2018.10.0.0 release are

- Bug fixes
- New device support
- Features
    - Spectator View

Supported platforms

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
- OpenVR
- Phone based AR
    - ARKit
    - ARCore

## November 2018

The November 2018 (2018.11.0.0) release will continue to add additional device support.

The theme(s) for the 2018.11.0.0 release are

- Bug fixes
- New device support

Supported platforms

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
- OpenVR
- Phone based AR
    - ARKit
    - ARCore
- TBD

## December 2018

There is no planned December 2018 MRTK release. Releases will resume in January 2019.
