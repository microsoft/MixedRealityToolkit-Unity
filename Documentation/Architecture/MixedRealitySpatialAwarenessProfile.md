# MixedRealitySpatialAwarenessProfile Class

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessProfile derives from Unity’s ScriptableObject and enables the developer to configure the spatial awareness system.

The following settings map one-to-one to properties of the same name defined in the [IMixedRealitySpatialAwarenessSystem](../IMixedRealitySpatialAwarenessSystem.md) interface.

The data types, default values and the supported range of values, if appropriate, are detailed.

## General Configuration Settings

### StartObserverSuspended

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### ObservationExtents

| Type | Default Value | Range |
| --- | --- | --- |
| Vector3 | Vector.one * 10 (10x10x10 meter cube) | TBD |

### UpdateInterval

| Type | Default Value | Range |
| --- | --- | --- |
| Single | 3.5 (seconds) | 0.0 - 5.0 (seconds) |

Setting 0.0 indicates that the data should be updated at the platform's highest available frequency.

## Mesh Handler Configuration Settings

### UseMeshSystem

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | True | n/a |

### MeshPhysicsLayer

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 31 | 0 - 31 |

### MeshLevelOfDetail

| Type | Default Value | Range |
| --- | --- | --- |
| [MixedRealitySpatialAwarenessLevelOfDetail](./MixedRealitySpatialAwarenessLevelOfDetail.md) | Coarse | Custom, Coarse, Fine |

### MeshTrianglesPerCubicMeter

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 0 | 0 - Int32.MaxValue |

### RecalculateNormals

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | True | n/a |

### DisplayMeshes

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | True | n/a |

### MeshMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | MRTK_Wireframe | n/a |

## Surface Finding Handler Configuration Settings

### UseSurfaceFindingSystem

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### SurfaceFindingPhysicsLayer

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 31 | 0 - 31 |

### SurfaceFindingUpdateInterval

| Type | Default Value | Range |
| --- | --- | --- |
| Single | TBD (seconds) | 0.0 - TBD (seconds) |

### SurfaceFindingMinimumArea

| Type | Default Value | Range |
| --- | --- | --- |
| Single | 0.025 (square meters) | TBD (square meters) |

### RenderFloorSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### FloorSurfaceMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

### RenderCeilingSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### CeilingSurfaceMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

### RenderWallSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### FloorWallMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

### RenderPlatformSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### FloorPlatformMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [IMixedRealitySpatialAwarenessSystem Interface](./IMixedRealitySpatialAwarenessSystem.md)
- [MixedRealitySpatialAwarenessProfileInspector](./MixedRealitySpatialAwarenessProfileInspector.md)
- [MixedRealitySpatialAwarenessMeshLevelOfDetail](./MixedRealitySpatialAwarenessMeshLevelOfDetail.md)
