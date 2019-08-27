# Creating a spatial awareness system data provider

The Mixed Reality Toolkit spatial awareness system is a flexible and extensible system for providing applications
with data about real world environments.

This article describes how to create custom data providers, also called observers, for the spatial awareness system. The example code shown here is
from the [`SpatialObjectMeshObserver`](xref:Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver.SpatialObjectMeshObserver).

> The complete source code used in this example can be found in the MixedRealityToolkit.Providers\ObjectMeshObserver folder.

## Define the spatial data object

The first step in creating a spatial awareness data provider is determining the type of data (ex: meshes or planes)it will provider to applications.

All spatial data objects must implement the [`IMixedRealitySpatialAwrenessObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObject)
interface.

The Mixed Reality Toolkit foundation provides the following spatial objects that can be used or extended in new data providers.

- [`BaseSpatialAwarenessObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialAwarenessObject)
- [`SpatialAwarenessMeshObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessMeshObject)
- [`SpatialAwarenessPlanarObject`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessPlanarObject)

## Implement the observer

> The complete code for the examples in this section are from the MixedRealityToolkit.Providers\ObjectMeshObserver\SpatialObjectMeshObserver.cs file.

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

``` c#
public class SpatialObjectMeshObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessMeshObserver, IMixedRealityCapabilityCheck
{ }
```

> `IMixedRealityCapabilityCheck` is used by the `SpatialObjectMeshObserver` to indicate that itprovides support for the SpatialAwarenessMesh capability.

### Implement the IMixedRealityDataProvider methods

Once the class has been defined, the next step is to provide the implementation of the [`IMixedRealityDataProvider`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider)
interface.

> [!Note]
> The `BaseSpatialObserver` class, via the `BaseService` class , provides only an empty implementations for `IMixedRealityDataProvider` methods. The details of these methods are generally data provider specific.

The methods that should be implemented by the data provider are:

- `Destroy`
- `Disable`
- `Enable`
- `Initialize`
- `Reset`
- `Update`

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

In the Mixed Reality Toolkit, data providers are configured using profiles. These are serialized objects that can easily be shared between applications by simply copying and
pasting a .asset file.

### Define the profile

> The complete code for the example in this section are from the MixedRealityToolkit.Providers\ObjectMeshObserver\SpatialObjectMeshObserverProfile.cs file.

Profile contents should mirror the accessible properties of the observer (ex: update interval). All of the user configurable properties defined in each
interface should be contained with the profile.

Base classes are encouraged if a new observer extends an existing data provider. For example, the [`SpatialObjectMeshObserverProfile`](Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver.SpatialObjectMeshObserverProfile)
extends the [`MixedRealitySpatialAwarenessMeshObserverProfile`](Microsoft.MixedReality.Toolkit.SpatialAwareness.MixedRealitySpatialAwarenessMeshObserverProfile) to enable
customers to provide a 3D model to be used as the environment data.

``` c#
[CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Spatial Object Mesh Observer Profile", fileName = "SpatialObjectMeshObserverProfile", order = 100)]
public class SpatialObjectMeshObserverProfile : MixedRealitySpatialAwarenessMeshObserverProfile
{
    [SerializeField]
    [Tooltip("The model containing the desired mesh data.")]
    private GameObject spatialMeshObject = null;

    /// <summary>
    /// The model containing the desired mesh data.
    /// </summary>
    public GameObject SpatialMeshObject => spatialMeshObject;
}
```

The `CreateAssetMenu` attribute can be applied to the profile class to enable customers to create a profile instance using the 
**Create > Assets > Mixed Reality Toolkit > Profiles** menu.

### Implement the inspector

> The complete code for the example in this section are from the MixedRealityToolkit.Providers\ObjectMeshObserver\SpatialObjectMeshObserverProfileInspector.cs file.

Profile inspectors are the user interface for configuring and viewing profile contents. Each profile inspector should extend the
[`BaseMixedRealityToolkitConfigurationProfileInspector]() class.

The `CustomEditor` attribute informs Unity the type of asset to which the imspector applies.

``` c#
[CustomEditor(typeof(SpatialObjectMeshObserverProfile))]
public class SpatialObjectMeshObserverProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
{ }
```

## Register the observer

Once created, the observer can be registered with the spatial awareness system be used in the application.

![Selecting the spatial object mesh observer](../Images/SpatialAwreness/SelectObjectObserver.png)

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
