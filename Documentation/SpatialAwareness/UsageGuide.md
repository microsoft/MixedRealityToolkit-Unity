# Configuring mesh observers via code

This article will discuss some of the key mechanisms and APIs to programmatically configure the [Spatial Awareness system](SpatialAwarenessGettingStarted.md) and related *Mesh Observer* data providers.

## Accessing mesh observers

Mesh Observer classes that implement the [`IMixedRealitySpatialAwarenessMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver) interface provide platform-specific mesh data to the Spatial Awareness system. Multiple Observers can be configured in the Spatial Awareness profile.

Accessing the data providers of the Spatial Awareness system is mostly the same as for any other Mixed Reality Toolkit service. The Spatial Awareness service must be casted to the [`IMixedRealityDataProviderAccess`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProviderAccess) interface to access via the `GetDataProvider<T>` APIs, which can then be utilized to access the Mesh Observer objects directly at runtime.

```c#
// Use CoreServices to quickly get access to the IMixedRealitySpatialAwarenessSystem
var spatialAwarenessService = CoreServices.SpatialAwarenessSystem;

// Cast to the IMixedRealityDataProviderAccess to get access to the data providers
var dataProviderAccess = spatialAwarenessService as IMixedRealityDataProviderAccess;

var meshObserver = dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
```

The `CoreServices.GetSpatialAwarenessSystemDataProvider<T>()` helper simplifies this access pattern as demonstrated below.

```c#
// Get the first Mesh Observer available, generally we have only one registered
var meshObserver = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

// Get the SpatialObjectMeshObserver specifically
var meshObserverName = "Spatial Object Mesh Observer";
var spatialObjectMeshObserver = dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>(meshObserverName);
```

## Starting and stopping mesh observation

One of the most common tasks when dealing with the Spatial Awareness system is turning the feature off/on dynamically at runtime. This is done per Observer via the [`IMixedRealitySpatialAwarenessObserver.Resume`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObserver.Resume) and [`IMixedRealitySpatialAwarenessObserver.Suspend`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObserver.Suspend) APIs.

```c#
// Get the first Mesh Observer available, generally we have only one registered
var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

// Suspends observation of spatial mesh data
observer.Suspend();

// Resumes observation of spatial mesh data
observer.Resume();
```

This code functionality can also be simplified via access through the Spatial Awareness system directly.

```c#
var meshObserverName = "Spatial Object Mesh Observer";
CoreServices.SpatialAwarenessSystem.ResumeObserver<IMixedRealitySpatialAwarenessMeshObserver>(meshObserverName);
```

### Starting and stopping all mesh observation

It is generally convenient to start/stop all mesh observation in the application. This can be achieved through the helpful Spatial Awareness system APIs, [`ResumeObservers()`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessSystem.ResumeObservers) and [`SuspendObservers()`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessSystem.SuspendObservers).

```c#
// Resume Mesh Observation from all Observers
CoreServices.SpatialAwarenessSystem.ResumeObservers();

// Suspend Mesh Observation from all Observers
CoreServices.SpatialAwarenessSystem.SuspendObservers();
```

## Enumerating and accessing the meshes

Accessing the meshes can be done per Observer and then enumerating through the
meshes known to that Mesh Observer via the [`IMixedRealitySpatialAwarenessMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver) API.

If running in editor, one can use the [`AssetDatabase.CreateAsset()`](https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateAsset.html) to save the `Mesh` object to an asset file.

If running on device, there are many community and store plugins available to serialize the `MeshFilter` data into a model file type([OBJ Example](http://wiki.unity3d.com/index.php/ObjExporter)).

```c#
// Get the first Mesh Observer available, generally we have only one registered
var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

// Loop through all known Meshes
foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
{
    Mesh mesh = meshObject.Filter.mesh;
    // Do something with the Mesh object
}
```

## Showing and hiding the spatial mesh

It's possible to programmatically hide/show meshes using the sample code below:

```c#
// Get the first Mesh Observer available, generally we have only one registered
var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

// Set to not visible
observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;

// Set to visible and the Occlusion material
observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Occlusion;
```

## Registering for mesh observation events

Components can implement the `IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>` and then register with the Spatial Awareness system to receive Mesh Observation events.

The `DemoSpatialMeshHandler` (Assets/MRTK/Examples/Demos/SpatialAwareness/Scripts) script is a useful example and starting point for listening to Mesh Observer events.

This is a simplified example of *DemoSpatialMeshHandler* script and Mesh Observation event listening.

```c#
// Simplify type
using SpatialAwarenessHandler = IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>;

public class MyMeshObservationExample : MonoBehaviour, SpatialAwarenessHandler
{
    private void OnEnable()
    {
        // Register component to listen for Mesh Observation events, typically done in OnEnable()
        CoreServices.SpatialAwarenessSystem.RegisterHandler<SpatialAwarenessHandler>(this);
    }

    private void OnDisable()
    {
        // Unregister component from Mesh Observation events, typically done in OnDisable()
        CoreServices.SpatialAwarenessSystem.UnregisterHandler<SpatialAwarenessHandler>(this);
    }

    public virtual void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
    {
        // Do stuff
    }

    public virtual void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
    {
        // Do stuff
    }

    public virtual void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
    {
        // Do stuff
    }
}
```

## See also

- [Spatial Awareness Getting Started](SpatialAwarenessGettingStarted.md)
- [Configuring the Spatial Awareness Mesh Observer](ConfiguringSpatialAwarenessMeshObserver.md)
- [Spatial Awareness API documentation](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness)
