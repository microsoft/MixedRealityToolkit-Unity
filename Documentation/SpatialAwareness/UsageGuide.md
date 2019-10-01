# Configuring Mesh Observers via code



TSDFSDFSDFSDF
TODO: Rename to be like title in yaml


This document contains some guides for common tasks to programmatically access the [Spatial Awareness system](SpatialAwarenessGettingStarted.md).



## Starting and stopping mesh observation

### Control via System

eed to call one of the following [IMixedRealitySpatialAwarenessSystem](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessSystem) methods:

- ResumeObserver&lt;T&gt;(string name)
- ResumeObservers()
- ResumeObservers&lt;T&gt;()

### Control via Observer

It's possible to programmatically suspend and resume mesh observation. The sample code below shows
how to access a particular observer (the mixed reality spatial mesh observer) to pause and then
immediately resume observation.

[`IMixedRealitySpatialAwarenessMeshObserver.Resume()`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver.Resume())
[`IMixedRealitySpatialAwarenessMeshObserver.Suspend()`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver.Suspend())

```C#
if (MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out var service))
{
    IMixedRealityDataProviderAccess dataProviderAccess = service as IMixedRealityDataProviderAccess;

    IMixedRealitySpatialAwarenessMeshObserver observer =
        dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

    // Suspends observation.
    observer.Suspend();

    // Resumes observation.
    observer.Resume();
}
```

## Enumerating and accessing the meshes

Accessing the meshes that are currently known to the spatial awareness system involves first querying
for the spatial awareness system, then getting a hold of the IMixedRealitySpatialAwarenessMeshObserver
(note that the base spatial awareness observer has no notion of meshes), and then enumerating those
meshes known to that observer.

Note that the sample below assumes that you have a single spatial mesh observer (which is the default)
unless you have extended the spatial awareness system.

```C#
if (MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out var service))
{
    IMixedRealityDataProviderAccess dataProviderAccess = service as IMixedRealityDataProviderAccess;

    IMixedRealitySpatialAwarenessMeshObserver observer =
        dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

    foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
    {
        Mesh mesh = meshObject.Filter.mesh;
    }
}
```

## Registering for mesh added and removed events

Please see the [DemoSpatialMeshHandler](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.Examples/Demos/SpatialAwareness/Scripts/DemoSpatialMeshHandler.cs)
for an example of how to listen to mesh events.

## Hiding the spatial mesh

It's possible to programmatically hide meshes using the sample code below:

```C#
if (MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out var service))
    {
        IMixedRealityDataProviderAccess dataProviderAccess = service as IMixedRealityDataProviderAccess;

        IMixedRealitySpatialAwarenessMeshObserver observer =
            dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        
        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
    }
}
```
