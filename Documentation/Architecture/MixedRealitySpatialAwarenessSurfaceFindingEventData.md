# MixedRealitySpatialAwarenessSurfaceFindingEventData Class

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwareness |

The MixedRealitySpatialSurfaceEventData derives from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md) and adds the information required for applications to understand changes that occur in the spatial awareness system’s surface finding subsystem. Note: Some events may not leverage all properties within this class, in those instances a neutral value will be set.

<img src="Images/MixedRealitySpatialAwarenessSurfaceFindingEventData.png">

### EventTime

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md).*

### EventType

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md).*

### Id

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md).*

### GameObject

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md).*

### SurfaceDescription

| Type |
| --- |
| [IMixedRealitySpatialAwarenessPlanarSurfaceDescription](./IMixedRealitySpatialAwarenessPlanarSurfaceDescription.md) |

For SurfaceAdded and SurfaceUpdated events, this will contain the description of the planar surface. For SurfaceRemoved, the value will be null.

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [MixedRealitySpatialAwarenessBaseEventData Class](./MixedRealitySpatialAwarenessBaseEventData.md)
- [IMixedRealitySpatialAwarenessPlanarSurfaceDescription](./IMixedRealitySpatialAwarenessPlanarSurfaceDescription.md)
- [IMixedRealitySpatialAwarenessSurfaceFindingHandler Interface](./IMixedRealitySpatialAwarenessSurfaceFindingHandler.md)
