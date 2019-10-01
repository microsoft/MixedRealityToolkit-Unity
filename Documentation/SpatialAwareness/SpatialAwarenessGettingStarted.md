# Spatial Awareness

![Spatial Awareness](../../Documentation/Images/SpatialAwareness/MRTK_SpatialAwareness_Main.png)

The Spatial Awareness system provides real-world environmental awareness in mixed reality applications. When introduced on Microsoft HoloLens, spatial awareness provided a collection of meshes, representing the geometry of the environment, which allowed for compelling interactions between holograms and the real-world.

## Getting Started

Adding support for spatial awareness requires two key components of the Mixed Reality Toolkit: the spatial awareness system and a supported platform provider.

1. [Enable](#enable-spatial-awareness) the spatial awareness system
2. [Register](#register-observers) and [configure](#configure-observers) one or more spatial observers to provide mesh data
3. [Build and deploy](#build-and-deploy) to a platform that supports spatial awareness

### Enable the Spatial Awareness system

The spatial awareness system is managed by the MixedRealityToolkit object (or another [service registrar](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar) component). 

Follow the steps below to enable or disable the Spatial Awareness system in the MixedRealityToolkit profile.

> [!NOTE]
TIDIDIDIDI

TODO: Insert table here for default profiles*

The steps below are not necessary for users of the default profile (i.e DefaultMixedRealityToolkitConfigurationProfile) which has this system already enabled. 
> The spatial awareness system is disabled by default on the default HoloLens 2 profile (i.e DefaultHoloLens2ConfigurationProfile), and the intent of this is to avoid the visual overhead of calculating and rendering the meshes.

1. Select the MixedRealityToolkit object in the scene hierarchy.

![MRTK Configured Scene Hierarchy](../../Documentation/Images/MRTK_ConfiguredHierarchy.png)

2. Navigate the Inspector panel to the Spatial Awareness System section and check *Enable Spatial Awareness System*

![Enable Spatial Awareness](../../Documentation/Images/SpatialAwareness/MRTKConfig_SpatialAwareness.png)

3. Select the Spatial Awareness System implementation

![Select the Spatial Awareness System Implementation](../../Documentation/Images/SpatialAwareness/SpatialAwarenessSelectSystemType.png)

### Register observers

Before the spatial awareness system can provide applications with data about the real-world, at least one spatial observer must be registered. Spatial observers are generally platform specific components that act as the provider for surfacing various types of mesh data from a platform specific endpoint (i.e HoloLens).

1. Open or expand the Spatial Awareness System profile

![Spatial Awareness System Profile](../../Documentation/Images/SpatialAwareness/SpatialAwarenessProfile.png)

1. Click the "Add Spatial Observer" button
1. Select the desired Spatial Observer implementation type

![Select the Spatial Observer Implementation](../../Documentation/Images/SpatialAwareness/SpatialAwarenessSelectObserver.png)

1. [Modify configuration properties on the observer](ConfiguringSpatialAwarenessMeshObserver.md) as necessary

> [!NOTE]
> Users of the default profile (DefaultMixedRealitSpatialAwarenessSystemProfile) will have the spatial awareness system pre-configured for the Windows Mixed Reality platform which uses
the [`WindowsMixedRealitySpatialMeshObserver`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.SpatialAwareness.WindowsMixedRealitySpatialMeshObserver) class.

### Build and Deploy

Once the spatial awareness system is configured with the desired observer(s), the project can be built and deployed to the target platform.

> [!IMPORTANT]
> If targeting the Windows Mixed Reality platform (ex: HoloLens), it is important to ensure the [Spatial Perception capability](https://docs.microsoft.com/en-us/windows/mixed-reality/spatial-mapping-in-unity) is enabled in order to use the Spatial Awareness system on device. 

> [!WARNING]
> Some platforms, including Microsoft HoloLens, provide support for remote execution from within Unity. This feature enables rapid development and testing without requiring the build and deploy step. Be sure to do final acceptance testing using a built and deployed version of the application, running on the target hardware and platform.

## See Also

- [Spatial Awareness API documentation](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness)
- [Configuring the Spatial Awareness Mesh Observer](ConfiguringSpatialAwarenessMeshObserver.md)
- [Spatial Object Mesh Observer](SpatialObjectMeshObserver.md)
- [Creating a spatial awareness system data provider](CreateDataProvider.md)
- [Spatial Mapping Overview WMR](https://docs.microsoft.com/en-us/windows/mixed-reality/spatial-mapping)
- [Spatial Mapping in Unity WMR](https://docs.microsoft.com/en-us/windows/mixed-reality/spatial-mapping-in-unity)