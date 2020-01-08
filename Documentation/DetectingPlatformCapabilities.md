# Detecting platform capabilities

A common question asked of the MRTK involves knowing which specific device (ex: Microsoft HoloLens 2) is being
used to run an application. Identifying the exact hardware can be challenging on different platforms. Instead, the MRTK
provides a way to identify specific capabilities at runtime, (e.g. if the current device endpoint supports articulated hands).

## Capabilities

The Mixed Reality Toolkit provides the [`MixedRealityCapability`](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)
enumeration, which defines a set of capabilities for which an application can query at runtime.

### Input system capabilities

The default MRTK Input System supports querying the following capabilities:

| Capability | Description |
|---|---|
| ArticulatedHand | Articulated hand input |
| EyeTracking | Eye gaze targeting |
| GGVHand | Gaze-Gesture-Voice hand input |
| MotionController | Motion controller input |
| VoiceCommand | Voice commands using app defined keywords |
| VoiceDictation | Voice to text dictation |

The example code below checks to see if the input system has loaded a data provider with support for articulated hands.

```c#
bool supportsArticulatedHands = false;

IMixedRealityCapabilityCheck capabilityCheck = CoreServices.InputSystem as IMixedRealityCapabilityCheck;
if (capabilityCheck != null)
{
    supportsArticulatedHands = capabilityCheck.CheckCapability(MixedRealityCapability.ArticulatedHand);
}
```

### Spatial awareness capabilities

The default MRTK Spatial Awareness system supports querying the following capabilities:

| Capability | Description |
|---|---|
| SpatialAwarenessMesh | Spatial meshes |
| SpatialAwarenessPlane | Spatial planes |
| SpatialAwarenessPoint | Spatial points |

This example checks to see if the spatial awareness system has loaded a data provider with support for spatial meshes.

```c#
bool supportsSpatialMesh = false;

IMixedRealityCapabilityCheck capabilityCheck = CoreServices.SpatialAwarenessSystem as IMixedRealityCapabilityCheck;
if (capabilityCheck != null)
{
    supportsSpatialMesh = capabilityCheck.CheckCapability(MixedRealityCapability.SpatialAwarenessMesh);
}
```

## See also

- [IMixedRealityCapabilityCheck API documentation](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck)
- [MixedRealityCapability enum documentation](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)
