# MixedRealitySpatialAwarenessProfile Class

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessProfile derives from Unity’s ScriptableObject and enables the developer to configure the spatial awareness system.

The following settings map one-to-one to properties of the same name defined in the [IMixedRealitySpatialAwarenessSystem](../IMixedRealitySpatialAwarenessSystem.md) interface.

The data types, default values and the supported range of values, if appropriate, are detailed.

## General Configuration Settings

### StartupBehavior

| Type | Default Value | Range |
| --- | --- | --- |
| AutoStartBehavior | AutoStart | AutoStart, Manual |

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
| [SpatialAwarenessLevelOfDetail](./SpatialAwarenessLevelOfDetail.md) | Coarse | Custom, Coarse, Fine |

### MeshTrianglesPerCubicMeter

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 0 | 0 - Int32.MaxValue |

### MeshRecalculateNormals

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | True | n/a |

### MeshDisplayOption

| Type | Default Value | Range |
| --- | --- | --- |
| [SpatialMeshDisplayOptions](./SpatialMeshDisplayOptions.md) | None | None, Visible, Occlusion |

### MeshVisibleMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | n/a | n/a |

### MeshOcclusionMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | n/a | n/a |

## Surface Finding Handler Configuration Settings

### UseSurfaceFindingSystem

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### SurfaceFindingPhysicsLayer

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 31 | 0 - 31 |

### SurfaceFindingMinimumArea

| Type | Default Value | Range |
| --- | --- | --- |
| Single | 0.025 (square meters) | TBD (square meters) |

### DisplayFloorSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### FloorSurfaceMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

### DisplayCeilingSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### CeilingSurfaceMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

### DisplayWallSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

### FloorWallMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

### DisplayPlatformSurfaces

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
- [MixedRealitySpatialAwarenessProfileInspector Class](./MixedRealitySpatialAwarenessProfileInspector.md)
- [SpatialAwarenessMeshDisplayOptions Enumeration](./SpatialAwarenessMeshDisplayOptions.md)
- [SpatialAwarenessMeshLevelOfDetail Enumeration](./SpatialAwarenessMeshLevelOfDetail.md)
