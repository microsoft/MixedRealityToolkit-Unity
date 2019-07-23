# What makes a mixed reality feature

To avoid the performance overheads of the `MonoBehaviour` class, all *services* (systems, features, or modules that require independent operation in a mixed reality solution, e.g. input, boundary, spatial awareness) are required to be discrete plain old c# classes which implement `IMixedRealityService` and to register with the `MixedRealityToolkit`.

The `MixedRealityToolkit` then coordinates all referencing between services and ensures that they receive all appropriate events (E.g. Awake/Initialize, Update, Destroy) as well as facilitating the finding of other services when needed.

Additionally, the `MixedRealityToolkit` also maintains the active VR/XR/AR SDK in use in the running project, to initialize the active device based on attached hardware and instigate proper operation.

## A service

An individual service can be any functionality that needs to be implemented in the project. Traditionally some projects use *singletons* which need to be alive in the scene, but this pattern has its advantages and disadvantages. We've decided to break away from this pattern in favor of a hybrid approach that brings several benefits over the traditional singleton implementations with `MonoBehaviours`, namely:

* Performance - without the overhead of a `MonoBehaviour`, [script updates are approximately 80% faster and don't require a `GameObject` to live in the scene](https://blogs.unity3d.com/2015/12/23/1k-update-calls/).
* Reference-ability - services can be discovered from the `MixedRealityToolkit` a lot faster and easier than searching `GameObjects` in a scene or using `FindObjectsOfType<T>`.
* No type dependency - though a method similar to dependency injection, services can be decoupled from their type, this means the concrete implementation can be swapped out at any time without adversely affecting code that consumes it (e.g. replacing the default InputSystem with your custom one, so long as you've fully implemented each interface).
* Multi-scene usage - if a service does need to know about a `transform` position in a scene, it can simply reference, or create, a `GameObject` _rather than be a component attached to it_. This makes it a lot easier to find and use the service when the project spans multiple scenes.

## Service interfaces

The *service* container uses a predefined *interface* type for storage and retrieval of any service, this ensures there are no hard dependencies within the Mixed Reality Toolkit, so that each subsystem can easily be swapped out with another (so long as it conforms to the interface).

Current system interfaces provided by the Mixed Reality Toolkit include:

* [`IMixedRealityInputSystem`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem)
* [`IMixedRealityBoundarySystem`](xref:Microsoft.MixedReality.Toolkit.Boundary.IMixedRealityBoundarySystem)
* [`IMixedRealityTeleportSystem`](xref:Microsoft.MixedReality.Toolkit.Teleport.IMixedRealityTeleportSystem)

When creating your own implementations of these systems, you must ensure each complies with the interfaces provided by the Mixed Reality Toolkit (e.g. if you replace the InputSystem with another of your own design).

> [!NOTE]
> All services must also inherit from the [`BaseService`](xref:Microsoft.MixedReality.Toolkit.BaseService) class or implement [`IMixedRealityService`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityService), to implement the functions required by the `MixedRealityToolkit` so their life-cycles are handled appropriately. (E.G. Initialize, Update, Destroy are called correctly.)
