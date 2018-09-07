# MixedRealitySpatialAwarenessMeshEventData Class

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwareness |

The MixedRealitySpatialAwarenessMeshEventData derives from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md) and adds the information required for applications to understand changes that occur in the spatial awareness system’s mesh subsystem.

*Some events may not leverage all properties within this class, in those instances a neutral value will be set.*

<img src="Images/MixedRealitySpatialAwarenessMeshEventData.png">

## EventTime

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md).*

## EventType

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md).*

## Id

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](./MixedRealitySpatialAwarenessBaseEventData.md).*

## GameObject

*Inherited from [MixedRealitySpatialAwarenessBaseEventData](#mixedrealityspatialawarenessbaseeventdata).*

## MeshDescription

| Type |
| --- |
| [IMixedRealitySpatialAwarenessMeshDescription](./IMixedRealitySpatialAwarenessMeshDescription.md) |

For MeshAdded and MeshUpdated events, this will contain the mesh description. For MeshRemoved, the value will be null.

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [MixedRealitySpatialAwarenessBaseEventData Class](./MixedRealitySpatialAwarenessBaseEventData.md)
- [IMixedRealitySpatialAwarenessMeshDescription Interface](./IMixedRealitySpatialAwarenessMeshDescription.md)
- [IMixedRealitySpatialAwarenessMeshHandler Interface](./IMixedRealitySpatialAwarenessMeshHandler.md)
