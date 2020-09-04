# Profiles

One of the main ways that the MRTK is configured is through the many profiles available in the foundation package. The main [`MixedRealityToolkit`](xref:Microsoft.MixedReality.Toolkit.MixedRealityToolkit) object in a scene will have the active profile, which is essentially a ScriptableObject. The top level MRTK Configuration Profile contains sub-profile data for each core of the primary core systems, each of which are designed to configure the behavior of their corresponding sub-systems. Furthermore, these sub-profiles are also Scriptable Objects and thus can contain references to other profile objects one level below them. There is essentially an entire tree of connected profiles that make up the configuration information for how to initialize the MRTK sub-systems and features.

For example, the Input system's behavior is governed by an input system profile, for example the `DefaultMixedRealityInputSystemProfile` (Assets/MRTK/SDK/Profiles). It's highly recommended to always modify the profile ScriptableObject assets via the in-editor inspector.

<img src="../../Documentation/Images/Profiles/input_profile.png" width="650px" style="display:block;">
<sup>Profile Inspector</sup>

> [!NOTE]
> While it is intended that profiles can be swapped out at runtime, this [currently does not work](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4289)

## Default profile

The MRTK provides a set of default profiles which cover most platforms and scenarios that the MRTK supports. For example, when you select the `DefaultMixedRealityToolkitConfigurationProfile` (Assets/MRTK/SDK/Profiles) you will be able to try out scenarios on VR (OpenVR, WMR) and HoloLens (1 and 2).

Note that because this is a general use profile, it's not optimized for any particular use case. If you want to have
more performant/specific settings that are better on other platforms, see the other profiles below, which are slightly tweaked to be better on their respective platforms.

## HoloLens 2 profile

The MRTK also provides a default profile that is optimized for deployment and testing on
the HoloLens 2: `DefaultHoloLens2ConfigurationProfile` (Assets/MRTK/SDK/Profiles/HoloLens2).

When prompted to choose a profile for the MixedRealityToolkit object, use this profile instead
of the default selected profile.

The key differences between the HoloLens2 profile and the Default Profile are:

**Disabled** Features:

- [Boundary System](../Boundary/BoundarySystemGettingStarted.md)
- [Teleport System](../TeleportSystem/Overview.md)
- [Spatial Awareness System](../SpatialAwareness/SpatialAwarenessGettingStarted.md)
- [Hand mesh visualization](../Input/HandTracking.md) (due to performance overhead)

**Enabled** Systems:

- The [Eye Tracking provider](../EyeTracking/EyeTracking_Main.md)
- Eye input simulation

Camera profile settings are set to match so that the editor quality and player quality are the same. This is different from the default camera profile where Opaque displays are set to a higher quality. This change means that in-editor quality will be lower, which will more closely match what will be rendered on the device.
  
> [!NOTE]
> The Spatial Awareness system is turned off by default based on client feedback - it is an interesting visualization to see
  initially but is typically turned off to avoid the visual distraction and the additional performance hit of
  having it on. The system can be re-enabled by following the [instructions here](../SpatialAwareness/SpatialAwarenessGettingStarted.md).
