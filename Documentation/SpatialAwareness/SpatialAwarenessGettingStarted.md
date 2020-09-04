# Spatial awareness

![Spatial Awareness](../../Documentation/Images/SpatialAwareness/MRTK_SpatialAwareness_Main.png)

The Spatial Awareness system provides real-world environmental awareness in mixed reality applications. When introduced on Microsoft HoloLens, Spatial Awareness provided a collection of meshes, representing the geometry of the environment, which allowed for compelling interactions between holograms and the real-world.

> [!NOTE]
> At this time, the Mixed Reality Toolkit does not ship with Spatial Understanding algorithms as originally packaged in the HoloToolkit. Spatial Understanding generally involves transforming Spatial Mesh data to create simplified and/or grouped Mesh data such as planes, walls, floors, ceilings, etc.

## Getting started

Adding support for Spatial Awareness requires two key components of the Mixed Reality Toolkit: the Spatial Awareness system and a supported platform provider.

1. [Enable](#enable-the-spatial-awareness-system) the Spatial Awareness system
2. [Register](#register-observers) and [configure](ConfiguringSpatialAwarenessMeshObserver.md) one or more spatial observers to provide mesh data
3. [Build and deploy](#build-and-deploy) to a platform that supports Spatial Awareness

### Enable the spatial awareness system

The Spatial Awareness system is managed by the MixedRealityToolkit object (or another [service registrar](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar) component). Follow the steps below to enable or disable the *Spatial Awareness system* in the *MixedRealityToolkit* profile.

The Mixed Reality Toolkit ships with a few default pre-configured profiles. Some of these have the Spatial Awareness system enabled OR disabled by default. The intent of this pre-configuration, particularly for when disabled, is to avoid the visual overhead of calculating and rendering the meshes.

| Profile | System Enabled by Default |
| --- | --- |
| `DefaultHoloLens1ConfigurationProfile` (Assets/MRTK/SDK/Profiles/HoloLens1) | False |
| `DefaultHoloLens2ConfigurationProfile` (Assets/MRTK/SDK/Profiles/HoloLens2) | False |
| `DefaultMixedRealityToolkitConfigurationProfile` (Assets/MRTK/SDK/Profiles) | True |

1. Select the MixedRealityToolkit object in the scene hierarchy to open in the Inspector Panel.

    ![MRTK Configured Scene Hierarchy](../../Documentation/Images/MRTK_ConfiguredHierarchy.png)

1. Navigate to the *Spatial Awareness System* section and check *Enable Spatial Awareness System*

    ![Enable Spatial Awareness](../../Documentation/Images/SpatialAwareness/MRTKConfig_SpatialAwareness.png)

1. Select the desired Spatial Awareness system implementation type. The [`MixedRealitySpatialAwarenessSystem`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.MixedRealitySpatialAwarenessSystem) is the default provided.

    ![Select the Spatial Awareness System Implementation](../../Documentation/Images/SpatialAwareness/SpatialAwarenessSelectSystemType.png)

### Register observers

Services in the Mixed Reality Toolkit can have [Data Provider services](../Architecture/SystemsExtensionsProviders.md) that supplement the main service with platform specific data and implementation controls. An example of this is the Mixed Reality Input System which has [multiple data providers](../Input/InputProviders.md) to get controller and other related input information from various platform-specific APIs.

The Spatial Awareness system is similar in that data providers supply the system with mesh data about the real-world. The Spatial Awareness profile must have at least one Spatial Observer registered. Spatial Observers are generally platform specific components that act as the provider for surfacing various types of mesh data from a platform specific endpoint (i.e HoloLens).

1. Open or expand the *Spatial Awareness System profile*

    ![Spatial Awareness System Profile](../../Documentation/Images/SpatialAwareness/SpatialAwarenessProfile.png)

1. Click the *"Add Spatial Observer"* button
1. Select the desired *Spatial Observer implementation type*

    ![Select the Spatial Observer Implementation](../../Documentation/Images/SpatialAwareness/SpatialAwarenessSelectObserver.png)

1. [Modify configuration properties on the observer](ConfiguringSpatialAwarenessMeshObserver.md) as necessary

> [!NOTE]
> Users of the `DefaultMixedRealityToolkitConfigurationProfile` (Assets/MRTK/SDK/Profiles) will have the Spatial Awareness system pre-configured for the Windows Mixed Reality platform which uses
the [`WindowsMixedRealitySpatialMeshObserver`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.SpatialAwareness.WindowsMixedRealitySpatialMeshObserver) class.

### Build and deploy

Once the Spatial Awareness system is configured with the desired observer(s), the project can be built and deployed to the target platform.

> [!IMPORTANT]
> If targeting the Windows Mixed Reality platform (ex: HoloLens), it is important to ensure the [Spatial Perception capability](https://docs.microsoft.com/windows/mixed-reality/spatial-mapping-in-unity) is enabled in order to use the Spatial Awareness system on device.

> [!WARNING]
> Some platforms, including Microsoft HoloLens, provide support for remote execution from within Unity. This feature enables rapid development and testing without requiring the build and deploy step. Be sure to do final acceptance testing using a built and deployed version of the application, running on the target hardware and platform.

## Next steps

After following the procedures above to enable the Spatial Awareness system, the system can be configured and controlled in more detail.

Information for configuring observers in inspector:

- [Configuring Observers for on device usage](ConfiguringSpatialAwarenessMeshObserver.md)
- [Configuring Observers for in-editor usage](SpatialObjectMeshObserver.md)

Information for controlling and extending observers via code:

- [Configuring Observers via Code](UsageGuide.md)
- [Creating a custom Observer](CreateDataProvider.md)

## See also

- [Spatial Awareness API documentation](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness)
- [Spatial Mapping Overview WMR](https://docs.microsoft.com/windows/mixed-reality/spatial-mapping)
- [Spatial Mapping in Unity WMR](https://docs.microsoft.com/windows/mixed-reality/spatial-mapping-in-unity)
