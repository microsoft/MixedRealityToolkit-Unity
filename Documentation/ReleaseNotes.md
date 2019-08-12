# Microsoft Mixed Reality Toolkit Release Notes

- [Version 2.0.0](#version-2.0.0)

## Version 2.0.0

- [What's new](#whats-new-in-2.0.0)
- [Known issues](#known-issues-in-2.0.0)
- [Changes](#changes-in-2.0.0)

This release of the Microsoft Mixed Reality Toolkit 2.0.0 supports the following devices and platforms.

- Microsoft HoloLens 2
- Microsoft HoloLens (1st gen)
- Windows Mixed Reality Immersive headsets
- OpenVR

The following software is required.

- Microsoft Visual Studio (2017 or 2019) Community Edition or higher
- Windows 10 SDK 18362 or later (installed by the Visual Studio Installer)
- Unity 2018.4, 2019.1 or 2019.2

### What's new in 2.0.0

**Default HoloLens (1st gen) profile**

We have added a new profile for HoloLens (1st gen) development that includes some of the
recommended MRTK configurations for best performance.

**CoreServices**

The CoreServices static class works in conjunction with the MixedRealityServiceRegistry
to provide applications with a fast and convenient mechanism to aquire instances of core
services (ex: Input System).

**SpatialObjectMeshObserver**

We have added the SpatialObjectMeshObserver to improve developer productivity when working
with the Spatial Awareness system. This observer reads mesh data from imported 3D models
and uses them to simulate environmental data from devices such as Microsoft HoloLens 2.

SpatialObjectMeshObserver is not enabled in the default profiles, please see the
[Spatial Awareness Geting Started](SpatialAwareness/SpatialAwarenessGettingStarted.md) article
for more information on configuring your application. 

**Service managers (experimental)**

This release adds service managers to enable the light-weight addition of specific Microsoft 
Mixed Reality Toolkit features, such as the Spatial Awareness system, individually.

These service managers are imported as part of the Foundation package and are located in the
MixedRealityToolkit.SDK\Experimental\Features folder and are a work in progress. 

To use, drag and drop the desired prefab into your heirarchy and select the configuration
profile. 

> Please note that these service managers are currently experimental, may have issues and
are subject to change. Please file any and all issues you encounter on GitHub

### Known issues in 2.0.0

**Runtime profile swapping**

MRTK 2.0.0 does not fully support profile swapping at runtime. This feature is being
investigated for a future release.

### Changes in 2.0.0

For a complete list of changes in the Microsoft Mixed Reality Toolkit 2.0.0, please review
the [recent pull requests](https://github.com/microsoft/MixedRealityToolkit-Unity/pulls?page=2&q=is%3Apr+is%3Aclosed) 
on GitHub.