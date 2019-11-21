# Systems, Extension Services and Data Providers

In the Mixed Reality Toolkit, many of the features are delivered in the form of services. Services are grouped into three
primary categories: systems, extension services and data providers.

## Systems

Systems are services that provide the core functionality of the Mixed Reality Toolkit. All systems are implementations of the
[`IMixedRealityService`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityService) interface.

- [BoundarySystem](../Boundary/BoundarySystemGettingStarted.md)
- CameraSystem
- [DiagnosticsSystem](../Diagnostics/DiagnosticsSystemGettingStarted.md)
- [InputSystem](../Input/Overview.md)
- [SceneSystem](../SceneSystem/SceneSystemGettingStarted.md)
- [SpatialAwarenessSystem](../SpatialAwareness/SpatialAwarenessGettingStarted.md)
- [TeleportSystem](../TeleportSystem/Overview.md)

Each of the listed systems are surfaced in the MixedRealityToolkit component's configuration [profile](../Profiles/Profiles.md).

## Extensions

Extension services are components that extend the functionality of the Mixed Reality Toolkit. All extension services must specify
that they implement the [`IMixedRealityExtensionService`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityExtensionService) interface.

For information on creating extension services, please reference the [Extension services](../Extensions/ExtensionServices.md) article.

To be accessible to the MRTK, extension services are registered and configured using the Extensions section of the MixedRealityToolkit
component's configuration profile.

![Configuring an extension service](../Images/Profiles/ConfiguredExtensionService.png)

## Data Providers

Data providers are components that, per their name, provide data to a Mixed Reality Toolkit service. All data providers must specify that
they implement the [`IMixedRealityDataProvider`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider) interface.

> [!Note]
> Not all services will require data providers. Of the MixedRealityToolkit's systems, the Input and Spatial Awareness systems are the
only services to utilize data providers.

To be accessible to the specific MRTK service, data providers are registered in the service's configuration profile.

Application code accesses data providers via the [`IMixedRealityDataProviderAccess`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProviderAccess) interface.

> [!Important]
> Although `IMixedRealityDataProvider` inherits from `IMixedRealityService`, data providers are not
registered with the `MixedRealityServiceRegistry`. To access data providers, application code must
query the service instance for which they were registered (ex: input system).

### Input

The MRTK input system utilizes only data providers that implement the [`IMixedRealityInputDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputDeviceManager).

![Input system data providers](../Images/Input/RegisteredServiceProviders.PNG)

The following example demonstrates accessing the input simulation provider and toggle the SmoothEyeTracking property.

``` c#
if (CoreServices.InputSystem != null)
{
    IMixedRealityDataProviderAccess dataProviderAccess = CoreServices.InputSystem as IMixedRealityDataProviderAccess;

    if (dataProviderAccess != null)
    {
        IInputSimulationService inputSimulation =
            dataProviderAccess.GetDataProvider<IInputSimulationService>();

        if (inputSimulation != null)
        {
            inputSimulation.SmoothEyeTracking = !inputSimulation.SmoothEyeTracking;
        }
    }
}
```

> [!Note]
> The input system returns only data providers that are supported for the platform on which the
application is running.

For information on writing a data provider for the MRTK input system, please see [creating an input system data provider](../Input/CreateDataProvider.md).

### Spatial Awareness

The MRTK spatial awareness system utilizes only data providers that implement the [`IMixedRealitySpatialAwarenessObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObserver) interface.

![Spatial awareness system data providers](../Images/SpatialAwareness/SpatialAwarenessProfile.png)

The following example demonstrates accessing the registered spatial mesh data providers and changing the visibility of the meshes.

``` c#
if (CoreServices.SpatialAwarenessSystem != null)
{
    IMixedRealityDataProviderAccess dataProviderAccess =
        CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;

    if (dataProviderAccess != null)
    {
        IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers =
            dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

        foreach (IMixedRealitySpatialAwarenessMeshObserver observer in observers)
        {
            // Set the mesh to use the occlusion material
            observer.DisplayOption = SpatialMeshDisplayOptions.Occlusion;
        }
    }
}
```

> [!Note]
> The spatial awareness system returns only data providers that are supported for the platform on which the application is running.

For information on writing a data provider for the MRTK spatial awareness system, please see [creating a spatial awareness system data provider](../SpatialAwareness/CreateDataProvider.md).

## See Also

- [What makes a mixed reality feature](../MixedRealityServices.md)
- [Extension services](../Extensions/ExtensionServices.md)
- [Creating an input system data provider](../Input/CreateDataProvider.md)
- [Creating a spatial awareness system system data provider](../SpatialAwareness/CreateDataProvider.md)
- [IMixedRealityService interface](xref:Microsoft.MixedReality.Toolkit.IMixedRealityService)
- [IMixedRealityDataProvider interface](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider)
- [IMixedRealityExtensionService interface](xref:Microsoft.MixedReality.Toolkit.IMixedRealityExtensionService)
