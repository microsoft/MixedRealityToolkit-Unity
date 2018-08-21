# Mixed Reality Toolkit Spatial Awareness System Architecture

The Mixed Reality Spatial Awareness System is intended to abstract the specific implementation details of platforms, such as the Microsoft HoloLens, that provide support for mapping the real-world and providing the data to Mixed Reality experiences.

As of this writing, the Mixed Reality Toolkit will provide support for Spatial Awareness on the Microsoft HoloLens. As additional platforms are supported, such as ARKit and ARCore, this support will be expanded to encompass any features and/or limitations of those systems.

Where possible, we are designing for common functionality (ex: identification of planar surfaces) well as access to the lowest level of available data (ex: mesh or point data).

It is expected that many platforms may not support one or more of the interfaces defined herein. In fact, some may not support spatial awareness at all. On those platforms, the system must gracefully fail and provide the developer with appropriate data (null, empty collections, etc.) in return.

Each interface defined will implement one or more Properties, Methods and/or Events (PMEs) that can be accessed by application code.

<img src="Images/SpatialAwarenessSystemArchitecture.png">

## Interfaces

## IMixedRealitySpatialAwarenessSystem

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.Interfaces.SpatialAwarenessSystem |

The IMixedRealitySpatialAwarenessSystem is the interface that defines the requirements of the spatial awareness system. The interface is divided, logically into multiple sections. As new functionality is added, the appropriate settings section is to be defined.

### General System Controls

The spatial awareness system contains data and methods that configure and control the overall spatial awareness system.

#### StartObserverSuspended

Indicates that the developer intends for the spatial observer to not return data until explicitly resumed. This allows the application to decide precisely when it wishes to begin receiving spatial data notifications. 

#### ObservationExtents

Gets or sets the size of the volume from which individual observations will be made. This is not the total size of the observable space.

#### UpdateInterval

Gets or sets the frequency, in seconds, at which the spatial observer updates.

#### IsObserverRunning

Indicates the current running state of the spatial observer. 

*This is a read-only property, set by the spatial awareness system.*

#### ResumeObserver()

Starts / restarts the spatial observer. This will cause spatial observation events (ex: MeshAddedEvent) to resume being sent.

#### SuspendObserver()

Stops / pauses the spatial observer. This will cause spatial observation events to be suspended until ResumeObserver is called.

### Mesh Handling Controls

The mesh handling section contains the data and methods that configure and control the representation of data as a collection of meshes.

For platforms that do not natively support returning observation data as a mesh, implementations can optionally process the native data before providing it to the caller.

#### Use Mesh System

Gets or sets a value that indicates if the spatial mesh subsystem is in use by the application. Turning this off will suspend all mesh events and cause the subsystem to return an empty collection when the GetMeshes method is called.

#### MeshPhysicsLayer

Get or sets the desired Unity Physics Layer on which to set the spatial mesh.

#### MeshPhysicsLayerMask

Gets the bit mask that corresponds to the value specified in MeshPhysicsLayer.

*This is a read-only property set by the spatial awareness system.*

#### MeshLevelOfDetail

Gets or sets the level of detail, as a [MixedRealitySpatialAwarenessMeshLevelOfDetail](#mixedrealityspatialawarenessmeshlevelofdetail) value, for the returned spatial mesh. Setting this value to Custom, implies that the developer is specifying a custom value for TrianglesPerCubicMeter.

Specifying any other value will cause TrianglesPerCubicMeter to be overwritten.

#### TrianglesPerCubicMeter

Gets or sets the level of detail, in triangles per cubic meter, for the returned spatial mesh. 

When specifying Coarse or Fine for the MeshLevelOfDetail, this value will be automatically overwritten.

#### RecalculateNormals

Gets or sets the value indicating if the spatial awareness system to generate normal for the returned meshes as some platforms may not support returning normal along with the spatial mesh. 

#### RenderMeshes

Gets or sets a value indicating if the mesh subsystem is to automatically display surface meshes within the application.

When enabled, the meshes will be added to the scene and rendered using the configured MeshMaterial.

#### MeshMaterial

Gets or sets the material to be used when rendering spatial meshes.

#### RaiseMeshAdded()

The spatial awareness system will call the OnMeshAdded method of the appropriate [IMixedRealitySpatialAwarenessMeshHandler](#imixedrealityspatialawarenesshandler)(s) to alert them of the new mesh.

#### RaiseMeshUpdated()

The spatial awareness system will call the OnMeshUpdated method of the appropriate [IMixedRealitySpatialAwarenessMeshHandler](#imixedrealityspatialawarenesshandler)(s) to alert them of the mesh changes.

#### RaiseMeshDeleted()

The spatial awareness system will call the OnMeshUpdated method of the appropriate [IMixedRealitySpatialAwarenessMeshHandler](#imixedrealityspatialawarenesshandler)(s) to alert them of the mesh removal.

### Surface Finding Controls

The surface finding section contains the data and methods that configure and control the representation of data as a collection of planar surfaces.

#### UseSurfaceFindingSystem

Indicates if the surface finding subsystem is in use by the application. Turning this off will suspend all surface events.

#### SurfacePhysicsLayer

Get or sets the desired Unity Physics Layer on which to set spatial surfaces.

#### SurfacePhysicsLayerMask

Gets the bit mask that corresponds to the value specified in SurfacePhysicsLayer. 

*This is a read-only property set by the spatial awareness system.*

#### SurfaceFindingMinimumArea

Gets or sets the minimum surface area, in square meters, that must be satisfied before a surface is identified.

#### RenderFloorSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display floor surfaces within the application.

When enabled, the surfaces will be added to the scene and rendered using the configured material.

#### FloorSurfaceMaterial

Gets or sets the material to be used when rendering planar surface(s) identified as a floor.

#### RenderCeilingSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display ceiling surfaces within the application.

When enabled, the surfaces will be added to the scene and rendered using the configured material.

#### CeilingSurfaceMaterial

Gets or sets the material to be used when rendering planar surface(s) identified as a ceiling.

#### RenderWallSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display wall surfaces within the application.

When enabled, the surfaces will be added to the scene and rendered using the configured material.

#### WallSurfaceMaterial

Gets or sets the material to be used when rendering planar surface(s) identified as a wall.

#### RenderPlatformSurfaces

Gets or sets a value indicating if the surface subsystem is to automatically display raised horizontal platform surfaces within the application.

When enabled, the surfaces will be added to the scene and rendered using the configured material.

#### PlatformSurfaceMaterial

Gets or sets the material to be used when rendering planar surface(s) identified as a raised horizontal platform.

#### RaiseSurfaceAdded()

The spatial awareness system will call the OnSurfaceAdded event of the appropriate [IMixedRealitySpatialAwarenessSurfaceFindingHandler](#imixedrealityspatialawarenesssurfacefindinghandler)(s) to alert them of the new planar surface.

#### RaiseSurfaceUpdated()

The spatial awareness system will call the OnSurfaceUpdated event of the appropriate [IMixedRealitySpatialAwarenessSurfaceFindingHandler](#imixedrealityspatialawarenesssurfacefindinghandler)(s) to alert them of the planar surface changes.

#### RaiseSurfaceDeleted()

The spatial awareness system will call the OnSurfaceUpdated event of the appropriate [IMixedRealitySpatialAwarenessSurfaceFindingHandler](#imixedrealityspatialawarenesssurfacefindinghandler)(s) to alert them of the planar surface removal.

#### GetSurfaces()

Returns the collection of surfaces identified by the surface finding subsystem.

## IMixedRealitySpatialAwarenessMeshHandler

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.Interfaces.SpatialAwarenessSystem.Handlers |

### OnMeshAdded()

| Arguument | Data Type |
| --- | --- |
| eventData | [MixedRealitySpatialMeshEventData](#mixedrealityspatialmesheventdata) |

Called when a new surface mesh has been identified by the spatial awareness system.

### OnMeshUpdated()

| Arguument | Data Type |
| --- | --- |
| eventData | [MixedRealitySpatialMeshEventData](#mixedrealityspatialmesheventdata) |

Called when an existing surface mesh has been modified by the spatial awareness system.

### OnMeshDeleted()

| Arguument | Data Type |
| --- | --- |
| eventData | [MixedRealitySpatialMeshEventData](#mixedrealityspatialmesheventdata) |

Called when an existing surface mesh has been discarded by the spatial awareness system.

## IMixedRealitySpatialAwarenessSurfaceFindingHandler

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.Interfaces.SpatialAwarenessSystem.Handlers |

### OnSurfaceAdded()

| Arguument | Data Type |
| --- | --- |
| eventData | [MixedRealitySpatialSurfaceEventData](#mixedrealityspatialsurfaceeventdata) |

Called when a new planar surface has been identified by the spatial awareness system.

### OnSurfaceUpdated()

| Arguument | Data Type |
| --- | --- |
| eventData | [MixedRealitySpatialSurfaceEventData](#mixedrealityspatialsurfaceeventdata) |

Called when an existing planar surface has been modified by the spatial awareness system.

### OnSurfaceDeleted()

| Arguument | Data Type |
| --- | --- |
| eventData | [MixedRealitySpatialSurfaceEventData](#mixedrealityspatialsurfaceeventdata) |

Called when an existing planar surface has been discarded by the spatial awareness system.

# Classes

## MixedRealitySpatialAwarenessSystem

| Toolkit Layer | Namespace |
| --- | --- |
| SDK - Surface Awareness Package | Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessSystem class provides the default implementation of the spatial awareness system. Implements the [IMixedRealitySpatialAwarenessSystem](#imixedrealityspatialawarenesssystem) interface.

## MixedRealitySpatialAwarenessMeshHandler

| Toolkit Layer | Namespace |
| --- | --- |
| SDK - Surface Awareness Package | Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessMeshHandler provides the default implementation of the spatial awareness system. Implements the default implementation of the [IMixedRealitySpatialAwarenessMeshHandler](#imixedrealityspatialawarenessmeshhandler) interface.

## MixedRealitySpatialAwarenessSurfaceFindingHandler

| Toolkit Layer | Namespace |
| --- | --- |
| SDK - Surface Awareness Package | Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessSurfaceFindingHandler provides the default implementation of the [IMixedRealitySpatialAwarenessSurfaceFindingHandler](#imixedrealityspatialawarenesssurfacefindinghandler) interface.

# System Profile Management Classes and Types

## MixedRealitySpatialAwarenessProfile

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwarenessSystem |

The MixedRealitySpatialAwarenessProfile derives from Unity’s ScriptableObject and enables the developer to configure the spatial awareness system.

The following settings map one-to-one to properties of the same name defined in the [IMixedRealitySpatialAwarenessSystem](#imixedrealityspatialawarenesssystem) interface.

The data types, default values and the supported range of values, if appropriate, are detailed.

### General Configuration Settings

#### StartObserverSuspended

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

#### ObservationExtents

| Type | Default Value | Range |
| --- | --- | --- |
| Vector3 | Vector.one * 10 (10x10x10 meter cube) | TBD |

#### UpdateInterval

| Type | Default Value | Range |
| --- | --- | --- |
| Single | 3.5 (seconds) | 0.0 - 5.0 (seconds) |

Setting 0.0 indicates that the data should be updated at the platform's highest available frequency.

### Mesh Handler Configuration Settings

#### UseMeshSystem

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | True | n/a |

#### MeshPhysicsLayer

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 31 | 0 - 31 |

#### MeshLevelOfDetail

| Type | Default Value | Range |
| --- | --- | --- |
| MixedRealitySpatialAwarenessLevelOfDetail | Coarse | Custom, Coarse, Fine |

#### MeshTrianglesPerCubicMeter

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 0 | 0 - Int32.MaxValue |

#### RecalculateNormals

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | True | n/a |

#### DisplayMeshes

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | True | n/a |

#### MeshMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | MRTK_Wireframe | n/a |

### Surface Finding Handler Configuration Settings

#### UseSurfaceFindingSystem

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

#### SurfaceFindingPhysicsLayer

| Type | Default Value | Range |
| --- | --- | --- |
| Int32 | 31 | 0 - 31 |

#### SurfaceFindingUpdateInterval

| Type | Default Value | Range |
| --- | --- | --- |
| Single | TBD (seconds) | 0.0 - TBD (seconds) |

#### SurfaceFindingMinimumArea

| Type | Default Value | Range |
| --- | --- | --- |
| Single | 0.025 (square meters) | TBD (square meters) |

#### RenderFloorSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

#### FloorSurfaceMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

#### RenderCeilingSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

#### CeilingSurfaceMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

#### RenderWallSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

#### FloorWallMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

#### RenderPlatformSurfaces

| Type | Default Value | Range |
| --- | --- | --- |
| Boolean | False | n/a |

#### FloorPlatformMaterial

| Type | Default Value | Range |
| --- | --- | --- |
| Material | TBD | n/a |

## MixedRealitySpatialAwarenessProfileInspector

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Inspectors.Profiles |

The MixedRealitySpatialAwarenessProfileInspector derives from MixedRealityBaseConfigurationProfileInspector to provide a visual means of modifying the spatial awareness profile using the Unity Inspector.

Each of the properties defined under [MixedRealitySpatialAwarenessProfile](#mixedrealityspatialawarenessprofile) are represented as elements in the user interface and are persisted when the user saves.

## MixedRealitySpatialAwarenessSurfaceTypes

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.Defintitions.SpatialAwareness |

``` C#
public enum MixedRealitySpatialAwarenessSurfaceTypes
{
    /// <summary>
    /// An unknown / unsupported type of surface.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The environment’s floor.
    /// </summary>
    Floor,

    /// <summary>
    /// The environment’s ceiling.
    /// </summary>
    Ceiling,

    /// <summary>
    /// A wall within the user’s space.
    /// </summary>
    Wall,

    /// <summary>
    /// A raised, horizontal surface such as a shelf.
    /// </summary>
    /// <remarks>
    Platforms, like floors, that can be used for placing objects 
    /// requiring a horizontal surface.
    /// </remarks>
    Platform
}
```

## MixedRealitySpatialAwarenessMeshLevelOfDetail

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.Defintitions.SpatialAwareness |

``` C#
public enum MixedRealitySpatialAwarenessMeshLevelOfDetail
{
    /// <summary>
    /// The custom level of detail allows specifying a custom value for
    /// MeshTrianglesPerCubicMeter.
    /// </summary>
    Custom = -1,

   /// <summary>
    /// The coarse level of detail is well suited for identifying large
    /// environmental features, such as floors and walls.
    /// </summary>
    Coarse = 0,

    /// <summary>
    /// The fine level of detail is well suited for using as an occlusion
    /// mesh.
    /// </summary>
    Fine = 2000
}
```

# Event Data Classes and Types

## MixedRealitySpatialMeshEventData

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.EventDatum.SpatialAwareness |

The MixedRealitySpatialMeshEventData provides the information required for applications to understand changes that occur in the spatial awareness system’s mesh subsystem. 

*Some events may not leverage all properties within this class, in those instances a neutral value will be set.*

### EventTime

| Type |
| --- |
| DateTime |

The time at which the event occurred.

### EventType

| Type |
| --- |
| [MixedRealitySpatialAwarenessEventType](#mixedrealityspatialawarenesseventtype) |

The type of event that has occurred.

### MeshId

| Type |
| --- |
| UInt32 |

An identifier assigned to a specific mesh in the spatial awareness system. 

### MeshData

| Type |
| --- |
| Mesh |

For MeshAdded and MeshUpdated events, this will contain the mesh data. For MeshDeleted, the value will be null.

## MixedRealitySpatialSurfaceEventData

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.EventDatum.SpatialAwareness |

The MixedRealitySpatialSurfaceEventData provides the information required for applications to understand changes that occur in the spatial awareness system’s surface finding subsystem. Note: Some events may not leverage all properties within this class, in those instances a neutral value will be set.

### EventTime

| Type |
| --- |
| DateTime |

The time at which the event occurred. The value will be in the device's configured time zone.

### EventType

| Type |
| --- |
| [MixedRealitySpatialAwarenessEventType](#mixedrealityspatialawarenesseventtype) |

The type of event that has occurred. The value will be Added, Updated or Deleted.

### SurfaceId

| Type |
| --- |
| UInt32 |

An identifier assigned to a specific planar surface in the spatial awareness system. 

### SurfaceData

| Type |
| --- |
| Bounds |

For SurfaceAdded and SurfaceUpdated events, this will contain the mesh data. For SurfaceDeleted, the value will be null.

## MixedRealitySpatialAwarenessEventType

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwareness |

``` C#
public enum MixedRealitySpatialAwarenessEventType
{
    /// <summary>
    /// A spatial awareness subsystem is reporting that a new spatial element 
    /// has been identified.
    /// </summary>
    Added = 0,

    /// <summary>
    /// A spatial awareness subsystem is reporting that an existing spatial
    /// element has been modified.
    /// </summary>
    Updated,

    /// <summary>
    /// A spatial awareness subsystem is reporting that an existing spatial
    /// element has been discarded.
    /// </summary>
    Deleted
}
```