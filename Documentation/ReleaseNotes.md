# Microsoft Mixed Reality Toolkit Release Notes

- [Version 2.0.0](#version-200)

## Version 2.0.0

- [Upgrading projects](#upgrading-projects-to-200)
- [What's new](#whats-new-in-200)
- [Known issues](#known-issues-in-200)

This release of the Microsoft Mixed Reality Toolkit 2.0.0 supports the following devices and platforms.

- Microsoft HoloLens 2
- Microsoft HoloLens (1st gen)
- Windows Mixed Reality Immersive headsets
- OpenVR

The following software is required.

- Microsoft Visual Studio (2017 or 2019) Community Edition or higher
- Windows 10 SDK 18362 or later (installed by the Visual Studio Installer)
- Unity 2018.4, 2019.1 or 2019.2

### Upgrading projects to 2.0.0

Since the RC2 release, there have been several changes that may impact your application projects,
including some files moving to new folder locations. For the smoothest upgrade path in your
projects, please use the following steps.

1. Close Unity
1. Delete all **MixedRealityToolkit** (you may not have all listed folders)
    - MixedRealityToolkit
    - MixedRealityToolkit.Examples
    - MixedRealityToolkit.Extensions
    - MixedRealityToolkit.Providers
    - MixedRealityToolkit.SDK
    - MixedRealityToolkit.Services
    - MixedRealityToolkit.Tools
1. Delete your **Library** folder
1. Re-open your project in Unity
1. Import the new unity packages
    - Foundation - _Import this package first_
    - (Optional) Tools
    - (Optional) Extensions,
    - (Optional) Examples are optional)
1. For each scene in your project
    - Delete **MixedRealityToolkit** and **MixedRealityPlayspace**, if present, from the Hierarchy
    - Select **MixedRealityToolkit -> Add to Scene and Configure**

> [!Important]
> Some profiles have been changed (properties have been added) in 2.0.0. If you have created custom
profiles, please open them to verify that all of the updated properties are correctly configured.

### What's new in 2.0.0

**Default HoloLens (1st gen) profile**

We have added a new profile for HoloLens (1st gen) development that includes some of the
recommended MRTK configurations for best performance.

To configure your application for HoloLens (1st gen) optimized settings, set the
Mixed Reality Toolkit's **Active Profile** to **DefaultHoloLens1ConfigurationProfile**.

![Default HoloLens (1st gen) Configuration Profile](../../Documentation/Images/ReleaseNotes/DefaultHoloLens1ConfigurationProfile.png)

**CoreServices**

The CoreServices static class works in conjunction with the MixedRealityServiceRegistry
to provide applications with a fast and convenient mechanism to aquire instances of core
services (ex: Input System).

To access extension service instances, use [MixedRealityServiceRegistry.TryGetService< T >](). << link >>

**IMixedRealityRaycastProvider**

<< >>

**SpatialObjectMeshObserver**

We have added the SpatialObjectMeshObserver to improve developer productivity when working
with the Spatial Awareness system. This observer reads mesh data from imported 3D models
and uses them to simulate environmental data from devices such as Microsoft HoloLens 2.

SpatialObjectMeshObserver is not enabled in the default profiles, please see the
[Spatial Awareness Gettniving Started](SpatialAwareness/SpatialAwarenessGettingStarted.md) article
for more information on configuring your application. 

**Multi-scene support**

 << extension >>

 **Input Animation Recording**
 
 <<>>

**Optimize Window**

<< >>

**Extension Service Creation Wizard**

<< >>

**Service managers (experimental)**

This release adds service managers to enable the light-weight addition of specific Microsoft 
Mixed Reality Toolkit features, such as the Spatial Awareness system, individually.

These service managers are imported as part of the Foundation package and are located in the
MixedRealityToolkit.SDK\Experimental\Features folder and are a work in progress.

Service manager prefabs are provided for the following services.

- BoundarySystem
- CameraSystem
- DiagnosticsSystem
- InputSystem
- SpatialAwarenessSystem
- TeleportSystem (requires the Input System)

To use, drag and drop the desired prefab into your heirarchy and select the configuration
profile. 

> [!Note]
> These service managers are currently experimental, may have issues and
are subject to change. Please file any and all issues you encounter on GitHub

**Updated Architecture Documentation**

<< >>

### Known issues in 2.0.0

The sections below highlight some of the known issues in the Microsoft Mixed Reality Toolkit
2.0.0.

**VR/Immersive devices: Content in some demo scenes is placed below the user**

Some demo scenes contained in the Examples package are optimized for HoloLens devices. These scenes
may place objects below the user when run on VR/Immersive devices. To work around this issue, 
select the **Scene Content** object, in the Hierarchy, and set the Transform's Position Y value to **1.5**.

![Adjusting Scene Content Height](../../Documentation/Images/ReleaseNotes/AdjustContentHeight.png)

**Unity 2019: Could not copy the file HolographicAppRemoting.dll**

There is a known issue with version 3.0.0 of the Windows Mixed Reality package for Unity 2019. If
your project has this version installed, you will see the following error when compiling in Microsoft
Visual Studio.

To work around the issues, please check for a newer version or roll back to version 3.0.2 using Window > Package Manager in the Unity editor.

**Runtime profile swapping**

MRTK 2.0.0 does not fully support profile swapping at runtime. This feature is being
investigated for a future release.
