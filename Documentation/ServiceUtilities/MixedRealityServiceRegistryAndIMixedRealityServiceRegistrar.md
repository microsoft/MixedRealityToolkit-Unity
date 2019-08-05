# What are the MixedRealityServiceRegistry and IMixedRealityServiceRegistrar

The Mixed Reality Toolkit has two very similarly named components that perform related tasks: 
MixedRealityServiceRegistry and IMixedRealityServiceRegistrar. What are they and what purpose does each serve?

## MixedRealityServiceRegistry

The MixedRealityServiceRegistry is the component that contains instances of each registered service
(core systems and extension services). It MixedRealiityServiceRegistry is implemented as a static C#
class and is the recommended pattern to use to acquire service instances in application code.

The following snippet demonstrates acquiring an IMixedRealityInputSystem instance.

```
IMixedRealityInputSystem inputSystem = null;

if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
{
    // Failed to acquire the input system. It may not have been registered
}
```

## IMixedRealityServiceRegistrar

The IMixedRealityServiceRegistrar is the interface that defines the interface implemented by 
components that manage the registration of one or more services. Components that implement
IMixedRealityServiceRegistrar are responsible for adding and removing the data within the 
MixedRealityServiceRegistry. The MixedRealityToolkit object is
one such component.

Other registrars can be cound in the MixedRealityToolkit.SDK.Experimental.Features
folder. These components can be used to add single servce (ex: Spatial Awareness) support
to an application. These single service managers are listed below.

- BoundarySystemManager
- CameraSystemManager
- DiagnosticsSystemManager
- InputSystemManager
- SpatialAwarenessSysstemManager
- TeleportSystemManager

Each of the above components, with the exception of the InputSystemManager, are responsible for
managing the registration and status of a single service type. The InputSystem requires some additional
support services (ex: FocusProvider) that are also managed by the InputSystemManager.

In general, the methods defined by IMixedRealityServiceRegistrar are called internally by service
management components or called by services that require additional service components to function
correctly. Application code should, generally, not call these methods as doing so may cause the application
to behave unpredictably (ex: a cached service instance may become invalid).

## See also

- [IMixedRealityServiceRegistrar API documentation](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar)
- [MixedRealityServiceRegistry API documentation](xref:Microsoft.MixedReality.Toolkit.MixedRealityServiceRegistry)
