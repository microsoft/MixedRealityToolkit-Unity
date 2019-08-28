# Creating an input system data provider

The Mixed Reality Toolkit input  system is a flexible and extensible system for enabling input device support.
It is the input system data provider (also called device manager) that is largely responsible for providing
support for new hardware platforms.

This article describes how to create custom data providers for the input system. The example code shown here is
from the [`WidnowsMixedRealityDeviceManager`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityDeviceManager).


## Implement the device manager

> The complete code for the examples in this section are from the MixedRealityToolkit.Providers\WindowsMixedReality\WindowsMixedRealityDeviceManager.cs file.

### Specify interface and/or base class inheritance

All input system data providers must implement the [`IMixedRealityInputDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputDeviceManager)
interface, which specifies the minimium functionality required by the input system. The MRTK foundation includes the [`BaseInputDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputDeviceManager)
class which provides a default implementation of this required functionality.

Using the example of the [`WindowsMixedRealityDeviceManager`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityDeviceManager), the following code specifies
that the class uses the [`BaseInputDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputDeviceManager) and implments the
[`IMixedRealityCapabilityCheck`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck) interface.

``` c#
public class WindowsMixedRealityDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
{ }
```

> `IMixedRealityCapabilityCheck` is used by the `WindowsMixedRealityDeviceManager` to indicate that it provides support for a set of input capabilities, specifically; articulated hands,
gaze-gesture-voice hands and motion controllers.

### Implement the IMixedRealityDataProvider methods

Once the class has been defined, the next step is to provide the implementation of the [`IMixedRealityDataProvider`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider)
interface.

> [!Note]
> The `BaseInputDevicemManager` class, via the `BaseService` class , provides only an empty implementations for `IMixedRealityDataProvider` methods. The details of these methods are generally data provider specific.

The methods that should be implemented by the data provider are:

- `Destroy`
- `Disable`
- `Enable`
- `Initialize`
- `Reset`
- `Update`

### Implement the device manager logic

The next step is to add the logic for managing the input devices, including any controllers to be supported.

### Implement the controller classes

 The example of the `WindowsMixedRealityDeviceManager` defines and implements the following controller classes.

> The source code for each of these classes can be found in the MixedRealityToolkit.Providers\WindowsMixedReality folder.

- WindowsMixedRealityArticulatedHand.cs
- WindowsMixedRealityController.cs
- WindowsMixedRealityGGVHand.cs

> [!Note]
> Not all device managers will support multiple controller types.

### Raise notification events

To enable applications to respond to input from the user, the data provider raises notification events corresponding to controller state changes as defined in the [`IMixedRealityInputHandler`1`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler)
and [`IMixedRealityInputHandler<T>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) interfaces.

> The complete source code for the examples in this section can be found in the MixedRealityToolkit.Providers\WindowsMixedReality\WindowsMixedRealityController.cs file.

For digital (button) type controls, raise the OnInputDown and OnInputUp events.

``` c#
if (interactionSourceState.touchpadPressed)
{
    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
}
else
{
    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
}

```

For analog controls (ex: touchpad position) the InputChanged event should be raised.

``` c#
InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.touchpadPosition);
```

### Apply the MixedRealityDataProvider attribute

The final step of creating an input system data provider is to apply the [`MixedRealityDataProvider`](xref:Microsoft.MixedReality.Toolkit.MixedRealityDataProviderAttribute)
attribute to the class. This is an optional step that allows for setting the default profile and platform(s) for the provider, when selected in the input system profile.

``` c#
[MixedRealityDataProvider(
    typeof(IMixedRealityInputSystem),
    SupportedPlatforms.WindowsUniversal,
    "Windows Mixed Reality Device Manager")]
public class WindowsMixedRealityDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
{ }
```

## Create the profile and inspector

In the Mixed Reality Toolkit, data providers are configured using profiles. These are serialized objects that can easily be shared between applications by simply copying and
pasting a .asset file.

Data providers with additional configuration options (ex: [InputSimulationService](../InputSimulation/InputSimulationService.md)) should create a profile and inspector to allow
customers to modify the behavior to best suit the needs of the application.

### Define the profile

> The complete code for the example in this section are from the MixedRealityToolkit.Services\InputSimulation\MixedRealityInputSimulationProfile.cs file.

Profile contents should mirror the accessible properties of the observer (ex: update interval). All of the user configurable properties defined in each
interface should be contained with the profile.

``` c#
[CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Simulated Input Profile", fileName = "MixedRealityInputSimulationProfile", order = (int)CreateProfileMenuItemIndices.InputSimulation)]
public class MixedRealityInputSimulationProfile : BaseMixedRealityProfile
{ }
```

The `CreateAssetMenu` attribute can be applied to the profile class to enable customers to create a profile instance using the 
**Create > Assets > Mixed Reality Toolkit > Profiles** menu.

### Implement the inspector

> The complete code for the example in this section are from the MixedRealityToolkit.Providers\ObjectMeshObserver\SpatialObjectMeshObserverProfileInspector.cs file.

Profile inspectors are the user interface for configuring and viewing profile contents. Each profile inspector should extend the
[`BaseMixedRealityToolkitConfigurationProfileInspector]() class.

``` c#
[CustomEditor(typeof(MixedRealityInputSimulationProfile))]
public class MixedRealityInputSimulationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
{ }
```

The `CustomEditor` attribute informs Unity the type of asset to which the imspector applies.

## Register the device manager

Once created, the device manager can be registered with the input system be used in the application.

![Registered input system data providers](../Images/Input/RegisteredServiceProviders.png)

## See also

- [Input system](Overview.md)
- [`BaseInputDeviceManager` class](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputDeviceManager)
- [`IMixedRealityInputDeviceManager` interface](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputDeviceManager)
- [`IMixedRealityInputHandler` interface](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler)
- [`IMixedRealityInputHandler<T>` interface](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1)
- [`IMixedRealityDataProvider` interface](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider)
- [`IMixedRealityCapabilityCheck` interface](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck)