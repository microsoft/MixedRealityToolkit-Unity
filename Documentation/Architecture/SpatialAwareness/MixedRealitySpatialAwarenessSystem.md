# MixedRealitySpatialAwarenessSystem Class

| Toolkit Layer | Namespace |
| --- | --- |
| SDK - Surface Awareness Package | Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessSystem class provides the default implementation of the spatial awareness system. Implements the [IMixedRealitySpatialAwarenessSystem](./IMixedRealitySpatialAwarenessSystem.md) interface.

## General System Controls

The spatial awareness system contains data and methods that configure and control the overall spatial awareness system.

### StartupBehavior

Gets or sets a value that indicates that the developer intends for the spatial observer to start automatically or wait until explicitly resumed. This allows the application to decide precisely when it wishes to begin receiving spatial data notifications.

### ObservationExtents

Gets or sets the size of the volume from which individual observations will be made. This is not the total size of the observable space.

### UpdateInterval

Gets or sets the frequency, in seconds, at which the spatial observer updates.

### IsObserverRunning

Indicates the current running state of the spatial observer.

*This is a read-only property, set by the spatial awareness system.*

### void ResumeObserver()

Starts / restarts the spatial observer. This will cause spatial observation events (ex: MeshAddedEvent) to resume being sent.

### void SuspendObserver()

Stops / pauses the spatial observer. This will cause spatial observation events to be suspended until ResumeObserver is called.

## Mesh Handling Controls

The mesh handling section contains the data and methods that configure and control the representation of data as a collection of meshes.

For platforms that do not natively support returning observation data as a mesh, implementations can optionally process the native data before providing it to the caller.

### Use Mesh System

Gets or sets a value that indicates if the spatial mesh subsystem is in use by the application. Turning this off will suspend all mesh events and cause the subsystem to return an empty collection when the GetMeshes method is called.

### MeshPhysicsLayer

Get or sets the desired Unity Physics Layer on which to set the spatial mesh.

### MeshPhysicsLayerMask

Gets the bit mask that corresponds to the value specified in MeshPhysicsLayer.

*This is a read-only property set by the spatial awareness system.*

### MeshLevelOfDetail

Gets or sets the level of detail, as a [SpatialAwarenessMeshLevelOfDetail](./SpatialAwarenessMeshLevelOfDetail.md) value, for the returned spatial mesh. Setting this value to Custom, implies that the developer is specifying a custom value for MeshTrianglesPerCubicMeter.

Specifying any other value will cause MeshTrianglesPerCubicMeter to be overwritten.

### MeshTrianglesPerCubicMeter

Gets or sets the level of detail, in triangles per cubic meter, for the returned spatial mesh.

When specifying Coarse or Fine for the MeshLevelOfDetail, this value will be automatically overwritten.

### MeshRecalculateNormals

Gets or sets the value indicating if the spatial awareness system to generate normal for the returned meshes as some platforms may not support returning normal along with the spatial mesh.

### MeshDisplayOption

Gets or sets a value indicating how the mesh subsystem is to display surface meshes within the application.

Applications that wish to process the Meshes should set this value to None.

### MeshVisibleMaterial

Gets or sets the material to be used when displaying spatial meshes.

### MeshOcclusionMaterial

Gets or sets the material to be used when spatial meshes should occlude other objects.

### Dictionary<uint, GameObject> GetMeshes()

Returns the collection of GameObjects being managed by the spatial awareness mesh subsystem.

## Surface Finding Controls

The surface finding section contains the data and methods that configure and control the representation of data as a collection of planar surfaces.

### UseSurfaceFindingSystem

Indicates if the surface finding subsystem is in use by the application. Turning this off will suspend all surface events.

### SurfacePhysicsLayer

Get or sets the desired Unity Physics Layer on which to set spatial surfaces.

### SurfacePhysicsLayerMask

Gets the bit mask that corresponds to the value specified in SurfacePhysicsLayer. 

*This is a read-only property set by the spatial awareness system.*

### SurfaceFindingMinimumArea

Gets or sets the minimum surface area, in square meters, that must be satisfied before a surface is identified.

### DisplayFloorSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display floor surfaces within the application. When enabled, the surfaces will be added to the scene and displayed using the configured FloorSurfaceMaterial.

### FloorSurfaceMaterial

Gets or sets the material to be used when displaying planar surface(s) identified as a floor.

### DisplayCeilingSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display ceiling surfaces within the application. When enabled, the surfaces will be added to the scene and displayed using the configured CeilingSurfaceMaterial.

### CeilingSurfaceMaterial

Gets or sets the material to be used when displaying planar surface(s) identified as a ceiling.

### DisplayWallSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display wall surfaces within the application. When enabled, the surfaces will be added to the scene and displayed using the configured WallSurfaceMaterial.

### WallSurfaceMaterial

Gets or sets the material to be used when displaying planar surface(s) identified as a wall.

### DisplayPlatformSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display raised horizontal platform surfaces within the application. When enabled, the surfaces will be added to the scene and displayed using the configured PlatformSurfaceMaterial.

### PlatformSurfaceMaterial

Gets or sets the material to be used when displaying planar surface(s) identified as a raised horizontal platform.

### Dictionary<uint, GameObject> GetSurfaceObjects()

Returns the collection of GameObjects managed by the surface finding subsystem.

## Handlers

The spatial awareness system raises events on the following handler types to indicate when spatial data is added, updated and removed.

- [IMixedRealitySpatialAwarenessMeshHandler](./IMixedRealitySpatialAwarenessMeshHandler.md)
- [IMixedRealitySpatialAwarenessSurfaceFindingHandler](./IMixedRealitySpatialAwarenessSurfaceFindingHandler.md)

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [IMixedRealitySpatialAwarenessSystem Interface](./IMixedRealitySpatialAwarenessSystem.md)
- [SpatialAwarenessMeshDisplayOptions Enumeration](./SpatialAwarenessMeshDisplayOptions.md)
- [SpatialAwarenessMeshLevelOfDetail Enumeration](./SpatialAwarenessMeshLevelOfDetail.md)
