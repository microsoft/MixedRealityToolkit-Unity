# IMixedRealitySpatialAwarenessMeshHandler Interface

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers |

<img src="Images/IMixedRealitySpatialAwarenessMeshHandler.png">

## void OnMeshAdded([MixedRealitySpatialEventData](./MixedRealitySpatialEventData.md) eventData)

Called when a new surface mesh has been identified by the spatial awareness system.

## void OnMeshUpdated([MixedRealitySpatialEventData](./MixedRealitySpatialEventData.md) eventData)

Called when an existing surface mesh has been modified by the spatial awareness system.

## OnMeshDeleted([MixedRealitySpatialEventData](./MixedRealitySpatialEventData.md) eventData)

Called when an existing surface mesh has been discarded by the spatial awareness system.

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [MixedRealitySpatialAwarenessMeshHandler Class](./MixedRealitySpatialAwarenessMeshHandler.md)
- [IMixedRealitySpatialAwarenessBaseDescription](./IMixedRealitySpatialAwarenessBaseDescription.md)