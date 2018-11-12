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

* **[htk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/htk_development)** - HoloToolkit development branch, active for fixes needed by existing customers.

* **[Dev_Working_Branch](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/Dev_Working_Branch)** - Obsolete. Persists for reference purposes as vNext is completed.

* **[mrtk_development](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_development)** - Current Mixed Reality Toolkit (vNext) active development

---
## Feature Branches

These relate to new components / features that are actively being developed for consideration to be included in the main development branch.
These should not be considered stable and main contain breaking features, **use at your own risk**.

> This list may not be up to date as development during the vNext beta is quite rapid, so simply use this as a guide and [ask in Slack](https://holodevelopersslack.azurewebsites.net/) regarding future features / capabilities.

For example, here are some of the main area that are currently being worked on:

* [feature/LuminPlatform](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/LuminPlatform) - Experimental Magic Leap device integration.
* [feature/spatialAwarenessPlanes](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/spatialAwarenessPlanes) - Extensions to the Spatial Awareness framework for  plane detection.
* [feature/mrtk_interactable](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_interactable) - New SDK components / features and ported features from the HTK
* [feature/mrtk_packageManager](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/mrtk_packageManager) - New MRTK delivery development (NuGet)
* [feature_mrtk_state_sharing](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature_mrtk_state_sharing) - Advanced networking / Sharing solution
* [feature/sharing_photon_preview](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/sharing_photon_preview) - Photon Based networking / sharing solution
* feature/SteamVR - (future) Adding native SteamVR support

## Other branches
Other branches are usually temporary or other experimental WIP.  occasionally you will see "Staging" or "Stabilization" branches which are used temporarily for the Mixed Reality Toolkit release process
