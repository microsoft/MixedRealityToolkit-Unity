# MixedRealitySpatialAwarenessMeshHandler Class

| Toolkit Layer | Namespace |
| --- | --- |
| SDK - Surface Awareness Package | Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessMeshHandler class provides the default implementation of the [IMixedRealitySpatialAwarenessMeshHandler](./IMixedRealitySpatialAwarenessMeshHandler.md) interface.

## void OnMeshAdded([MixedRealitySpatialEventData](./MixedRealitySpatialEventData.md) eventData)

Called when a new surface mesh has been identified by the spatial awareness system.

## void OnMeshUpdated([MixedRealitySpatialEventData](./MixedRealitySpatialEventData.md) eventData)

Called when an existing surface mesh has been modified by the spatial awareness system.

## OnMeshDeleted([MixedRealitySpatialEventData](./MixedRealitySpatialEventData.md) eventData)

Called when an existing surface mesh has been discarded by the spatial awareness system.

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [IMixedRealitySpatialAwarenessMeshHandler Interface](./IMixedRealitySpatialAwarenessMeshHandler.md)
