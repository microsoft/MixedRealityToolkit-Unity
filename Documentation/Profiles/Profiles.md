# Profiles

One of the main ways that the MRTK is configured is through the many profiles available
in the foundation package. The main MixedRealityToolkit object has a profile, which is
essentially a ScriptableObject which may contain references to other ScriptableObjects,
each of which are designed to configure the behavior of their corresponding sub-systems.

For example, the Input system's behavior is governed by its input profile. An example
of such a profile is [here](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/DefaultMixedRealityInputSystemProfile.asset). In order to edit this, it's usually recommended
to use the in-editor inspector:

<img src="../../Documentation/Images/Profiles/input_profile.png" width="650px" style="display:block;"><br/>
<sup>Profile Inspector</sup>

> **Note:** While it is intended that profiles can be swapped out at runtime, this currently
> doesn't work - [see this issue](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4289)

## Default Profile

The MRTK provides a set of default profiles which cover most platforms and scenarios that
the MRTK supports - for example, when you select the [DefaultMixedRealityToolkitConfigurationProfile](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/DefaultMixedRealityToolkitConfigurationProfile.asset)
you will be able to try our scenarios on VR (OpenVR, WMR) and HoloLens (1 and 2). Note that because
this is a general use profile, it's not optimized for any particular use case - if you want to have
more performant/specific settings that are better on other platforms, please check out the other
profiles below, which are slightly tweaked to be better on their respective platforms.

## HoloLens 2 Profile

The MRTK also provides a default profile that is optimized for deployment and testing on
the HoloLens 2: [DefaultHoloLens2ConfigurationProfile](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/HoloLens2/DefaultHoloLens2ConfigurationProfile.asset).
When prompted to choose a profile for the MixedRealityToolkit object, use this profile instead
of the default selected profile.

The key differences between the HoloLens2 profile and the Default Profile are:

- Boundary System has been disabled (there are no boundaries in AR)
- Teleport System has been disabled (primarily a VR concept)
- Spatial Awareness System has been disabled (spatial meshes won't render, but this can be turned back
  on by following the [instructions here](../SpatialAwareness/SpatialAwarenessGettingStarted.md). Spatial
  meshes are turned off by default based on client feedback - it is an interesting visualization to see
  initially but is typically turned off to avoid the visual distraction and the additional performance hit of
  having it on.
- The eye tracking provider and settings have been enabled
- Eye simulation has been enabled by default
- Hand mesh visualization is disabled (there is a performance overhead associated with using hand meshes)
- Camera profile settings are set to match such that the editor quality and player quality are the same.
  (This is different from the default camera profile where Opaque displays are set to higher quality -
  this change makes it so that in-editor quality will be lower, which will more closely match what will
  be rendered on the device)
  
