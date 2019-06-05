# Spatial Awareness - Usage Guide

This document contains some guides for common tasks that programmatically access the spatial awareness system.

## Enumerating and accessing the meshes

Accessing the meshes that are currently known to the spatial awareness system involves first querying
for the spatial awareness system, then getting a hold of the IMixedRealitySpatialAwarenessMeshObserver
(note that the base spatial awareness observer has no notion of meshes), and then enumerating those
meshes known that observer.

Note that the sample below assumes that you have a single spatial mesh observer (which is the default)
unless you have extended the spatial awareness system.

```C#
if (MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out var service))
{
    IMixedRealitySpatialAwarenessMeshObserver observer = 
        service.GetObserver<IMixedRealitySpatialAwarenessMeshObserver>();
    
    foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
    {
        Mesh mesh = meshObject.Filter.mesh;
    }
}
```

## Registering for mesh added and removed events

Please see the [DemoSpatialMeshHandler](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.Examples/Demos/SpatialAwareness/Scripts/DemoSpatialMeshHandler.cs)
for an example of how to listen to mesh events.