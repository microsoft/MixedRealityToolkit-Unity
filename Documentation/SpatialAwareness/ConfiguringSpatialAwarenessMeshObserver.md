# Configuring the Spatial Awareness Mesh Observer

The following two items must be defined first when configuring a Spatial Mesh Observer profile for the [Spatial Awareness system](SpatialAwarenessGettingStarted.md).

1. The concrete observer type implementation
1. list of supported platform(s) to run this observer

> [!NOTE]
> All observers must extend and [IMixedRealitySpatialAwarenessMeshObserver](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver) interface. Furthermore, the default implementation provided by the Mixed Reality Toolkit is the [WindowsMixedRealitySpatialMeshObserver](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.SpatialAwareness.WindowsMixedRealitySpatialMeshObserver) for the Windows Mixed Reality platform.

![Mesh Observer General Settings](../../Documentation/Images/SpatialAwareness/SpatialAwarenessMeshObserverProfile_TypesPlatforms.png)

## General Settings

![Mesh Observer General Settings](../../Documentation/Images/SpatialAwareness/MeshObserverGeneralSettings.png)

**Startup Behavior**

The startup behavior specifies whether the observer will begin running when first instantiated. The two options are:

* *Auto Start* - The default value whereby the observer will begin operation after initialization
* *Manual Start* - The Observer will wait to be directed to start

If using *Manual Start*, one must [resume and suspend them at runtime via code](UsageGuide.md#starting-and-stopping-mesh-observation).

**Update Interval**

The time, in seconds, between requests to the platform to update spatial mesh data. Typical values fall in the range of 0.1 and 5.0 seconds. 

**Is Stationary Observer**

Indicates whether or not the observer is to remain stationary or to move and update with the user.

When stationary, if the user moves further than the [Observation Extents](#observation-extents) away from the origin, there will be no mesh data calculated until the users moves closer back to the origin.

**Observer Shape**

The observer shape defines the type of volume that the mesh observer will use when observing meshes. The supported options are:

* [Axis Aligned Cube](#axis-aligned-cube) - A rectangular shape that stays aligned with the axes of the world coordinate system, as determined at application startup.
* [User Aligned Cube](#user-aligned-cube) - A rectangular shape that rotates to align with the users local coordinate system.
* [Sphere](#sphere) - A spherical volume with a center at the world space origin. The X value of the [Observation Extents](#observation-extents) property will be used as the radius of the sphere.

**Observation Extents**

The observation extents define the distance from the observation point that meshes will be observed.

## Physics Settings

![Mesh Observer Physics Settings](../../Documentation/Images/SpatialAwareness/MeshObserverPhysicsSettings.png)

**Physics Lyer**

The physics layer specifies which layer on which the spatial mesh objects will be placed in order to interact with the Unity Physics and RayCast systems. The Mixed Reality Toolkit reserves layer 31 by default for use by Spatial Awareness observers.

**Recalculate Normals**

Specifies whether or not the mesh observer will recalculate the normals of the mesh following observation. This setting is available to ensure applications receive meshes that contain valid normals data on platforms that do not return them with meshes. 

## Level of Detail Settings

![Mesh Observer Level of Detail Settings](../../Documentation/Images/SpatialAwareness/MeshObserverLevelOfDetailSettings.png)

**Level of Detail**

Specifies the level of detail (LOD) of the spatial mesh data. Currently defined values are Coarse, Fine and Custom.

* *Coarse* - Places a smaller impact on application performance and is an excellent choice for navigation/plane finding.

* *Fine* - Generally exacts a higher impact on application performance and is a great option for occlusion meshes.

* *Custom* - Requires the application to specify the **Triangles / Cubic Meter** property and allows applications to tune the accuracy vs. performance impact of the spatial mesh observer.

> [!NOTE]
> It is not guaranteed that all **Triangles/Cubic Meter** values are honored by all platforms. Experimentation and profiling is highly recommended when using a custom LOD. 

**Triangles per Cubic Meter**

When using the *Custom* [Level of Detail](#level-of-detail) setting, this property specifies the triangle density for the spatial mesh.

## Display Settings

![Mesh Observer Display Settings](../../Documentation/Images/SpatialAwareness/MeshObserverDisplaySettings.png)

### Display Option

Specifies how spatial meshes are to be displayed by the observer. Supported values are None, Visible and Occlusion. Setting Visible or Occlusion instructs the observer to select the appropriate material. Specifying None causes the observer to not render the mesh.

> [!WARNING]
> Setting Display Option to None does _not_ stop the observer from running. If you wish to stop the observer, applications will need to call one of the following [IMixedRealitySpatialAwarenessSystem]

sdfsdfsdfasdfsdfsdfsdaf

TODO: Point out to the usage guide

(xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessSystem) methods:
>
> - SuspendObserver&lt;T&gt;(string name)
> - SuspendObservers()
> - SuspendObservers&lt;T&gt;()

**Visible Material**

Indicates the material to be used when visualizing the spatial mesh.

**Occlusion Material**

Indicates the material to be used to cause the spatial mesh to occlude holograms.

## See Also

- [Spatial Awareness System](SpatialAwarenessGettingStarted.md)
- [Configuring Spatial Awareness system via Code](UsageGuide.md)
- [IMixedRealitySpatialAwarenessObserver API documentation](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObserver)
- [IMixedRealitySpatialAwarenessMeshObserver API documentation](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver)
- [BaseSpatialObserver API documentation](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialObserver)
