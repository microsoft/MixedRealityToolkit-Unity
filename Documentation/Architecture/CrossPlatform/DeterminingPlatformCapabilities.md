# Determining platform capabilities

A common question asked of the MRTK involves knowing which specific device (ex: Microsoft HoloLens 2) is being
used to run an application. The goal behind that knoweldge generally centers around knowing whether or not a
particular feature (ex: articulated hands) is available.

Mixed reality devices are evolving rapidly and any attempt at capturing a table mapping features to specific
hardware will quickly become untennable.

## Checking for platform capabilities

To help answer the root question of whether or not an application can leverage a specific feature, the Mixed Reality
Toolkit provides the [IMixedRealityCapabilityCheck](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck)
interface.

### Querying for a capability

The most common pattern for determining support for a specific capability is to query the service.

The following example checks to see if the input system has loaded a data provider with support for articulated hands.

```
bool supportsArticulatedHands = false;

IMixedRealityCapabilityCheck capabilityCheck = inputSystem as IMixedRealityCapabilityCheck;
if (capabilityCheck != null)
{
    supportsArticulatedHands = capabilityCheck.CheckCapability(MixedRealityCapability.ArticulatedHand);
}
```

### Capabilites

The Mixed Reality Toolkit provides the [MixedRealityCapability](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)
enumeration which defines a set of capabilities for which an application can query at runtime. This enum contains the 
complete set of capabilities for which the Mixed Reality Toolkit supports checking.

#### Input System capabilies

The following capabilities apply to the [input system](../Input/Overview.md).

- ArticulatedHand
- EyeTracking
- GGVHand
- MotionController
- VoiceCommand
- VoiceDication

#### Spatial Awareness capabilities

The following capabilities apply to the [spatial awareness](../SpatialAwareness/SpatialAwarenessGettingStarted.md) system.

- SpatialAwarenessMesh
- SpatialAwarenessPlane
- SpatialAwaarenessPoint

## See Also

- [IMixedRealityCapabilityCheck API documentation](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck)
- [MixedRealityCapability enum documentation](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)