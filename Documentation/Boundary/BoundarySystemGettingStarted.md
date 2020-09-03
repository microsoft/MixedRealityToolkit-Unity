# Boundary system

The Boundary system provides support for visualizing Virtual Reality boundary components in mixed reality applications. Boundaries define the area in which users can safely move around while wearing a VR headset. Boundaries are an important component of a mixed reality experience to help users avoid unseen obstacles while wearing a VR headset.

Many Virtual Reality platforms provide an automatic display, for example a white outline superimposed on the virtual world as the user or their controller nears the boundary. The Mixed Reality Toolkit's Boundary System extends this feature to enable the display of an outline of the tracked area, a floor plane and other features that can be used to provide additional information to users.

## Getting started

Adding support for boundaries requires two key components of the Mixed Reality Toolkit: the Boundary System and a Virtual Reality platform configured with a boundary.

1. [Enable](#enable-boundary-system) the boundary system
2. [Configure](#configure-boundary-visualization) the boundary visualization
3. [Build and deploy](#build-and-deploy) to a VR platform with a configured boundary

## Enable boundary system

The Boundary System is managed by the MixedRealityToolkit object (or another [service registrar](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar) component).

The following steps presume use of the MixedRealityToolkit object. Steps required for other service registrars may be different.

1. Select the MixedRealityToolkit object in the scene hierarchy.

    ![MRTK Configured Scene Hierarchy](../../Documentation/Images/MRTK_ConfiguredHierarchy.png)

1. Navigate the Inspector panel to the Boundary System section and check Enable

    ![Enable the Boundary System](../../Documentation/Images/Boundary/MRTKConfig_Boundary.png)

1. Select the Boundary System implementation. The default class implementation provided by the MRTK is the [`MixedRealityBoundarySystem`](xref:Microsoft.MixedReality.Toolkit.Boundary.MixedRealityBoundarySystem)

    ![Select the Boundary System Implementation](../../Documentation/Images/Boundary/BoundarySelectSystemType.png)

> [!NOTE]
> All Boundary System implementation must extend the [`IMixedRealityBoundarySystem`](xref:Microsoft.MixedReality.Toolkit.Boundary.IMixedRealityBoundarySystem)

## Configure boundary visualization

The [Boundary System uses a configuration profile](ConfiguringBoundaryVisualization.md) to specify which boundary components are to be displayed and to configure their appearance.

![Boundary Visualization Options](../../Documentation/Images/Boundary/BoundaryVisualizationProfile.png)

> [!NOTE]
> Users of the default profile, `DefaultMixedRealityBoundaryVisualizationProfile` (Assets/MRTK/SDK/Profiles) will have the boundary system pre-configured to display a floor plane, the play area and the tracked area.

## Build and deploy

Once the boundary system is configured with the desired visualization options, the project can be built deployed to the target platform.

> [!NOTE]
> Unity Play Mode enables in-editor visualization of the configured boundary. This feature enables rapid development and testing without requiring the build and deploy step. Be sure to do final acceptance testing using an built and deployed version of the application, running on the target hardware and platform.

## Accessing boundary system via code

If enabled and configured, the Boundary System can be accessed via the CoreServices static helper class. The reference can then be used to dynamically change the Boundary parameters and access related GameObjects managed by the system.

```c#
// Hide Boundary Walls at runtime
CoreServices.BoundarySystem.ShowBoundaryWalls = false;

// Get Unity GameObject for the floor visualization in scene
GameObject floorVisual = CoreServices.BoundarySystem.GetFloorVisualization();
```

## See also

- [Boundary API documentation](xref:Microsoft.MixedReality.Toolkit.Boundary)
- [Configuring the Boundary Visualization](ConfiguringBoundaryVisualization.md)
