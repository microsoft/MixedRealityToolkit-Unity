# MixedRealitySpatialAwarenessMeshHandler Class

| Toolkit Layer | Namespace |
| --- | --- |
| SDK - Surface Awareness Package | Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessMeshHandler class provides the default implementation of the [IMixedRealitySpatialAwarenessMeshHandler](./IMixedRealitySpatialAwarenessMeshHandler.md) interface.

## void OnMeshAdded([MixedRealitySpatialMeshEventData](./MixedRealitySpatialMeshEventData.md) eventData)

Called when a new surface mesh has been identified by the spatial awareness system.

## void OnMeshUpdated([MixedRealitySpatialMeshEventData](./MixedRealitySpatialMeshEventData.md) eventData)

Called when an existing surface mesh has been modified by the spatial awareness system.

## OnMeshDeleted([MixedRealitySpatialMeshEventData](./MixedRealitySpatialMeshEventData.md) eventData)

Called when an existing surface mesh has been discarded by the spatial awareness system.

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [IMixedRealitySpatialAwarenessMeshHandler Interface](./IMixedRealitySpatialAwarenessMeshHandler.md)
