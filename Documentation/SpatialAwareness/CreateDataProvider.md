# Creating a spatial awareness system data provider

The Mixed Reality Toolkit spatial awareness system is a flexible and extensible system for providing applications
with data about real world environments.

This article describes how to create custom data providers, also called observers, for the spatial awareness system. The example code shown here is
from the [SpatialObjectMeshObserver](SpatialObjectMeshObserver.md).

## Define the spatial data object

The first step in creating a spatial awareness data provider is determining the type of data (ex: meshes or planes)it will provider to applications.

All spatial data objects must implement the [`IMixedRealitySpatialAwrenessObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObject)
interface.

The Mixed Reality Toolkit foundation provides the following spatial objects that can be used or extended in new data providers.

- [`BaseSpatialAwarenessObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialAwarenessObject)
- [`SpatialAwarenessMeshObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessMeshObject)
- [`SpatialAwarenessPlanarObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessPlanarObject)

## Implement the observer

### Specify interface and/or base class inheritance

All spatial awareness data providers must implement the [`IMixedRealitySpatialAwarenessObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObserver)
interface, which specifies the minimium functionality required by the spatial awareness system. The MRTK foundation includes the [`BaseSpatialObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialObserver)
class which provides a default implementation of this required functionality.

> [!Note]
> It is recommended that data providers use or define an interface that exposes significant settings and data (ex: level of detail) to enable applications
to customize behavior and access relevant data.

Using the example of the [`SpatialObjectMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver.SpatialObjectMeshObserver), the following code specifies
that the class uses the [`BaseSpatialAwarenessObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialAwarenessObject) and implments the
[`IMixedRealitySpatialAwarenessMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver) and
[`IMixedRealityCapabilityCheck`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck) interfaces.

> `IMixedRealityCapabilityCheck` is used by the `SpatialObjectMeshObserver` to indicate that provides support for the SpatialAwarenessMesh capability.

``` c#
public class SpatialObjectMeshObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessMeshObserver, IMixedRealityCapabilityCheck
{ }
```

### Implement the IMixedRealityDataProvider methods

Once the class has been defined, the next step is to provide the implementation of the [`IMixedRealityDataProvider`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider)
interface.

> [!Note]
> The `BaseSpatialObserver` class, does not provide the implementation for `IMixedRealityDataProvider`. The implementation of these methods
is generally observer specific.

Coninuing with the example of the [`SpatialObjectMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver.SpatialObjectMeshObserver), the following
code illustrates simple implementations of [`IMixedRealityDataProvider`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider).

``` c#
        public override void Initialize()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>(EventSystem.current);

            ReadProfile();

            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                Resume();
            }
        }

        public override void Update()
        {
            if (!IsRunning) { return; }

            SendMeshObjects();
        }

        public override void Reset()
        {
            CleanupObserver();
            Initialize();
        }

        public override void Enable()
        {
            // Resume iff we are not running and had been disabled while running.
            if (!IsRunning && autoResume)
            {
                Resume();
            }
        }

        public override void Disable()
        {
            // Remember if we are currently running when Disable is called.
            autoResume = IsRunning;

            // If we are disbled while running...
            if (IsRunning)
            {
                // Suspend the observer
                Suspend();
            }
        }

        public override void Destroy()
        {
            Disable();
            CleanupObserver();
        }
```

> [!Note]
> The previous example uses two variables that are not defined; meshEventData and autoResume.
`meshEventData` will be described in the [Observation change notifications](#observation-chang-notifications)
section. `autoResume` is an implementation detail of the example observer and is not discussed in this artice.

### Implement the observer logic

The next step is to add the logic of the observer by implementing the specific observer interface, for
example [`IMixedRealitySpatialAwarenessMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver).

> [!Note]
> This portion of the data provider will typically be platform specific.

### Observation change notifications

To enable applications to respond to changes in the observer's understanding of the environment, the next
step is to send the observation change notifications.

The example of the [`SpatialObjectMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver.SpatialObjectMeshObserver)
raises events when meshes are added or removed. The following code demonstrates raising and event when mesh data is added.

``` c#
// The data to be sent when mesh observation events occur.
// This member variable is initialized as part of the Initialize() method.
private MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> meshEventData = null;

/// <summary>
/// Sends the observations using the mesh data contained within the configured 3D model.
/// </summary>
private void SendMeshObjects()
{
    if (!sendObservations) { return; }

    if (spatialMeshObject != null)
    {
        MeshFilter[] meshFilters = spatialMeshObject.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            SpatialAwarenessMeshObject meshObject = SpatialAwarenessMeshObject.Create(
                meshFilters[i].sharedMesh,
                MeshPhysicsLayer,
                $"Spatial Object Mesh {currentMeshId}",
                currentMeshId,
                ObservedObjectParent);

            meshObject.GameObject.transform.localPosition = meshFilters[i].transform.position;
            meshObject.GameObject.transform.localRotation = meshFilters[i].transform.rotation;

            ApplyMeshMaterial(meshObject);

            meshes.Add(currentMeshId, meshObject);

            // Initialize the meshEventData variable with data for the added event.
            meshEventData.Initialize(this, currentMeshId, meshObject);
            // Raise the event via the spatial awareness system.
            SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshAdded);

            currentMeshId++;
        }
    }

    sendObservations = false;
}
```

> [!Note]
> By it's nature, the `SpatialObjectMeshObserver` does not raise updated events. Please see the implementation of the
`WindowsMixedRealitySpatialMeshObserver` (in the MixedRealityToolkit.Providers\WindowsMixedReality\WindowsMixedRealitySpatialMeshObserver.cs file) for an
example of raising an updated event for an observed mesh. 

### Apply the MixedRealityDataProvider attribute

The final step of creating a spatial awareness data provider is to apply the [`MixedRealityDataProvider`](Microsoft.MixedReality.Toolkit.MixedRealityDataProviderAttribute)
attribute to the class. This is an optional step that allows for setting the default profile and platform(s) for the observer, when selected in the spatial awareness profile.

``` c#
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsEditor | SupportedPlatforms.MacEditor | SupportedPlatforms.LinuxEditor,
        "Spatial Object Mesh Observer",
        "ObjectMeshObserver/Profiles/DefaultObjectMeshObserverProfile.asset",
        "MixedRealityToolkit.Providers")]
    public class SpatialObjectMeshObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessMeshObserver, IMixedRealityCapabilityCheck
    { } 
```

## Create the profile and inspector

## Define the assemblies

## Register the observer

## See also

- [Spatial awarenes system](SpatialAwarenessGettingStarted.md)
- [IMixedRealitySpatialAwarenessObject interface](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObject)
- [BaseSpatialAwarenessObject class](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialAwarenessObject)
- [SpatialAwarenessMeshObject class](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessMeshObject)
- [SpatialAwarenessPlanarObject class](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessPlanarObject)
- [IMixedRealitySpatialAwarenessObserver interface](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObserver)
- [BaseSpatialObserver class](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialObserver)
- [IMixedRealitySpatialAwarenessMeshObserver interface](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessMeshObserver)
- [IMixedRealityDataProvider](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider)
