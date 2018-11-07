# Mixed Reality Toolkit Branch guide

The Mixed Reality Toolkit uses multiple working branches within the project to manage the various components of delivery, these can be broken down to:

---
## Release Branches

Release branches are frozen code linked to a specific released asset, the code held within these branches should match 1-1 with their corresponding asset.
This ensures whether you download the asset or the code, there are no differences in the delivery.

* **[master](https://github.com/Microsoft/MixedRealityToolkit-Unity) - [HoloToolkit 2017.4.2.0](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/2017.4.2.0)** - Current HoloToolkit release branch.

* **[mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release) - [Mixed Reality Toolkit 2018.9.0](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/tag/2018.9.0-Beta) (vNext Beta)** - Current Mixed Reality Toolkit (vNext) beta release.

---
## Development Branches

Development branches relate to the current active and tested development state of the product.  These should be considered "stable", however bugs may be introduced during the course of development.

* **Dev_Working_Branch/[htk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/htk_development)** - Legacy HoloToolkit development - Currently frozen and due to be deprecated.

* **[mrtk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_development)** - Current Mixed Reality Toolkit (vNext) active development

---
## Feature Branches

These relate to new components / features that are actively being developed for consideration to be included in the main development branch.
These should not be considered stable and main contain breaking features, **use at your own risk**.

> This list may not be up to date as development during the vNext beta is quite rapid, so simply use this as a guide and [ask in Slack](https://holodevelopersslack.azurewebsites.net/) regarding future features / capabilities.

* [feature/LuminPlatform](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/LuminPlatform) - (@StephenHodgson) Experimental Magic Leap device integration.
* [feature/spatialAwarenessPlanes](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/spatialAwarenessPlanes) - (@Keveleigh) - Extensions to the Spatial Awareness framework for  plane detection.
* [feature/mrtk_documentation](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_documentation) - (@SimonDarksideJ) In progress MRTK documentation
* [feature/mrtk_interactable](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_interactable) - (@killerantz) New SDK components / features and ported features from the HTK
* [feature/mrtk_packageManager](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_packageManager) - (@ryzngard) New MRTK delivery development (NuGet)
* [feature_mrtk_state_sharing](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature_mrtk_state_sharing) - (@Railboy) Advanced networking / Sharing solution
* [feature/mrtk_sharing](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_sharing) - (@Sean0ng) Photon Based networking / sharing solution
* [feature/mrtk_audioSystem](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_audioSystem) - (@davidkline-ms) new MRTK audio solution
* [feature/mrtk_audioManager](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_audioManager) - (@davidkline-ms) new MRTK general audio manager

## Other branches
Other branches are usually temporary or other experimental WIP.  occasionally you will see "Staging" or "Stabilization" branches which are used temporarily for the Mixed Reality Toolkit release process