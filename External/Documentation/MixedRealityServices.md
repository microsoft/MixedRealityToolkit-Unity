# What makes a Mixed Reality Feature

To avoid the performance overheads of the `MonoBehaviour` class, all Services (Systems, Features, or Modules that require independent operation in a Mixed Reality Solution, e.g. Input, Boundary, SpatialAwareness) are required to be discrete plain old c# classes which implement `IMixedRealityService` and is registered with the `MixedRealityOrchestrator`.

The `MixedRealityOrchestrator` then coordinates all referencing between services and ensures that they receive all appropriate events (E.g. Awake/initialize, Update, Destroy) as well as facilitating the finding of other services when needed.

Additionally, the `MixedRealityOrchestrator` also maintains the active VR/XR/AR SDK in use in the running project, to initialize the active device based on attached hardware and instigate proper operation.

## A Service

An individual service can be any functionality that needs to be implemented in the project. Traditionally some projects use Singletons which need to be alive in the scene, but this pattern has its advantages and disadvantages. We've decided to break away from this pattern in favor of a hybrid approach that brings several benefits over the traditional Singleton implementations with MonoBehaviours, namely:

* Performance - without the overhead of a MonoBehaviour, [script updates are approximately 80% faster and doesn't require a `GameObject` to live in the scene](https://blogs.unity3d.com/2015/12/23/1k-update-calls/).
* Reference-ability - Services can be discovered from the `MixedRealityOrchestrator` a lot faster and easier than searching `GameObjects` in a scene or using `FindObjectsOfType<T>`.
* No Type dependency - Though a method similar to Dependency Injection, services can be decoupled from their type, this means the concrete implementation can be swapped out at any time without adversely affecting code that consumes it (E.G. Replacing the default InputSystem with your custom one, so long as you've fully implemented each interface).
* Multi-scene usage - If a service does need to know about a `Transform` position in a scene, it can simply reference, or create, a `GameObject` _rather than be a component attached to it_. This makes it a lot easier to find and use the service when the project spans multiple scenes.

## MixedReality Runtime Services

The `MixedRealityOrchestrator` is also extensible enough that is can support runtime as well as design time service registrations, the key differences being:

### Mixed Reality Core Systems

* Can only have a single implementation registered (e.g. InputSystem, there can only be one)

### System Registration

```C#
MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new DefaultMixedRealityInputSystem());
```

### System Retrieval

```C#
var inputSystem = MixedRealityOrchestrator.Instance.GetService<IMixedRealityInputSystem>();
```

### Mixed Reality Components

* Supports as many instances as you like, stored by interface type `IMixedRealityComponent`
* Also supports being named, in case you need an individual service of a type that has a specific name

### Component Registration

```C#
// Add test component 1
MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1());

// Add test component 2
MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2() { Name = "Tester2" });
```

### Component Retrieval

```C#
// Retrieve Component1
var component1 = MixedRealityOrchestrator.Instance.GetService(typeof(ITestComponent1));

// Retrieve component named Tester2
TestComponent2 component2 = (TestComponent2)MixedRealityOrchestrator.Instance.GetService(typeof(ITestComponent2), "Tester2");

// Get all components of Type 2
var type2Components = MixedRealityOrchestrator.Instance.GetService(typeof(ITestComponent2));
```

To keep things simple, the `MixedRealityOrchestrator` can already distinguish between each type of service and will handle them accordingly, so it is the same code for registering and retrieving all types of services.

## Service Interfaces

The Service container uses a predefined Interface type for storage and retrieval of any Service, this ensures there are no hard dependencies within the Mixed Reality Toolkit, so that each subsystem can easily be swapped out with another (so long as it confirms to the interface).

Current System interfaces provided by the Mixed Reality Toolkit include:

* IMixedRealityInputSystem
* IMixedRealityBoundarySystem
* IMixedRealityTeleportSystem

When creating your own implementations of these systems, you must ensure each complies with the interfaces provided by the Mixed Reality Toolkit (e.g. if you replace the InputSystem with another of your own design).

Additionally, if you create your own Runtime Services, these must also have their own Interface which derives from `IMixedRealityComponent` to ensure it is supported by the `MixedRealityOrchestrator`.

> All Services must also inherit from the **BaseService** class or implement `IMixedRealityService`, to implement the functions required by the `MixedRealityOrchestrator` so they're life-cycles are handled appropriately. (E.G. Initialize, Update, Destroy are called correctly.)