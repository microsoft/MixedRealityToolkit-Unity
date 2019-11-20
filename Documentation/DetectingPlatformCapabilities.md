# Detecting platform capabilities

A common question asked of the MRTK involves knowing which specific device (ex: Microsoft HoloLens 2) is being
used to run an application. Identifying the exact hardware can be challenging on different platforms. The MRTK
instead provides a way identify specific capabilities of the system (for example, if the system has the articulated
hands capability).

## Capabilities

The Mixed Reality Toolkit provides the [MixedRealityCapability](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)
enumeration which defines a set of capabilities for which an application can query at runtime. This enum contains the 
complete set of capabilities for which the Mixed Reality Toolkit supports checking.

### Input System capabilities

The input system supports querying the following capabilities.

| Capability | Description |
|---|---|
| ArticulatedHand | Articulated hand input |
| EyeTracking | Eye gaze targeting |
| GGVHand | Gaze-Gesture-Voice hand input |
| MotionController | Motion controller input |
| VoiceCommand | Voice commands using app defined keywords |
| VoiceDictation | Voice to text dictation |

This example checks to see if the input system has loaded a data provider with support for articulated hands.

``` C#
// Get the input system.
IMixedRealityInputSystem inputSystem = null;
MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
if (inputSystem == null)
{
    // Failed to get the input system.
}

bool supportsArticulatedHands = false;

IMixedRealityCapabilityCheck capabilityCheck = inputSystem as IMixedRealityCapabilityCheck;
if (capabilityCheck != null)
{
    supportsArticulatedHands = capabilityCheck.CheckCapability(MixedRealityCapability.ArticulatedHand);
}
```

### Spatial Awareness capabilities

The spatial awareness system supports querying the following capabilities.

| Capability | Description |
|---|---|
| SpatialAwarenessMesh | Spatial meshes |
| SpatialAwarenessPlane | Spatial planes |
| SpatialAwarenessPoint | Spatial points |

This example checks to see if the spatial awareness system has loaded a data provider with support for spatial meshes.

``` C#
// Get the spatial awareness system.
IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;
MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
if (spatialAwarenessSystem == null)
{
    // Failed to get the spatial awareness system.
}

bool supportsSpatialMesh = false;

IMixedRealityCapabilityCheck capabilityCheck = spatialAwarenessSystem as IMixedRealityCapabilityCheck;
if (capabilityCheck != null)
{
    supportsSpatialMesh = capabilityCheck.CheckCapability(MixedRealityCapability.SpatialAwarenessMesh);
}
```

## See Also

- [IMixedRealityCapabilityCheck API documentation](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck)
- [MixedRealityCapability enum documentation](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)
