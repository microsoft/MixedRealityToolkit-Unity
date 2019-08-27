# Roadmap

This document outlines the roadmap of the Mixed Reality Toolkit.

# Current Release 

[Microsoft Mixed Reality Toolkit v2.0.0 RC2](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/v2.0.0-RC2)

# Upcoming Releases
 
| Product | Description | Timeline | Project board |
| --- | --- | --- | --- |
|MRTK V2 RC 2| UX update and polish to better support HoloLens 2 interactions. | June 2019 |  |
|MRTK V2 Release | First release of MRTK V2 that supports HoloLens 1, HoloLens 2, WMR, OpenVR | July 2019 | |

Release details, including backlog items, can be found on the [GitHub project pages](https://github.com/Microsoft/MixedRealityToolkit-Unity/projects).

# Mixed Reality Toolkit (MRTK) Roadmap

The Mixed Reality Toolkit (also known as "vNext") is an all-new product, built to be cross MR/AR/VR/XR platform by design. There are two planned pre-releases after which the Mixed Reality Toolkit will become the primary product.

The Mixed Reality Toolkit will require Unity 2018.4.

> When Unity releases an LTS (Long Term Support) product, the Mixed Reality Toolkit will update to the LTS release. MRTK will also support the latest non-beta (ex: 2019.1) tech branch version of Unity, at the time at which MRTK was released.

## New Architecture

The MRTK is being rewritten to be cross AR/VR/MR/XR platform and to be more modular. The core layer will contain the interfaces and definitions required to build manager components (ex: input manager) as well as the abstractions needed to support multiple platforms. The SDK layer is where pre-built / reusable components will be delivered. The SDK will be further modularized to make it simple to opt-in to specific functionality (ex: UX controls).


# Release Plans
## Alpha - Released 

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

## Beta - Released 

The MRTK Beta release (2018.9.0-Beta) will introduce initial support for modularization. It is also when the vast majority of HoloToolkit features will be added. Supported platforms will be unchanged from the Alpha release.

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

## Beta 2 - Released 

The Beta 2 (v2.0.0-Beta2) release marks the first official release of MRTK vNext and will support the same platforms as the Beta. With this release, the Mixed Reality Toolkit will replace the HoloToolkit, which will be transitioned into sustained engineering mode.

The theme(s) for the Beta 2 release are

- Documentation 
- Quality and Performance 

Supported platforms 

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
- OpenVR

## RC1 - Released

RC1 release (v2.0.0-RC1) was the first update to include HoloLens 2 capabilities

The theme(s) for the release are

- HoloLens 2 support
- API contract complete 

Supported platforms

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
    - HoloLens 2
- OpenVR

## RC2 - Planned 

RC2 release is planned to add and polish features to support HoloLens 2 interactions with hands and eyes.

The theme(s) for the release are

- UX Polish 
- Performance 

Supported platforms

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
    - HoloLens 2
- OpenVR

## MRTK V2 Release - Planned 

First official release of MRTK V2 that includes HoloLens 2 support and addresses critical feedbacks from microsoft partners and community partners! 

The theme(s) for the release are

- Address feedback issues. 

Supported platforms

- Windows Mixed Reality
    - Immersive Headsets
    - Microsoft HoloLens
    - HoloLens 2
- OpenVR
