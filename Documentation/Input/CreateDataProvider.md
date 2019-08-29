# Creating an input system data provider

The Mixed Reality Toolkit input system is an extensible system for enabling input device support.
It is the input system data provider (also called device manager) that is largely responsible for providing
support for new hardware platforms.

This article describes how to create custom data providers, also called device managers, for the input system. The example code shown here is
from the [`WindowsMixedRealityDeviceManager`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityDeviceManager).

> The complete code used in this example can be found in the MixedRealityToolkit.Providers\WindowsMixedReality folder.

## Namespace and folder structure

> [!Important]
> If an input system data provider is being submitted to the [Mixed Reality Toolkit repository](https://github.com/Microsoft/MixedRealityToolkit-Unity), the
namespace **must** begin with Microsoft.MixedReality.Toolkit (ex: Microsoft.MixedReality.Toolkit.WindowsMixedReality) and the code should be
located in a folder beneath MixedRealityToolkit.Providers (ex: MixedRealityToolkit.Providers\WindowsMixedReality).

### Namespace

Data providers are required to have a namespace to mitigate potential name collisions. It is recommended that the namespace includes the following components.

- Company name
- Feature area

For example, an input data provider created by the Contoso company may be "Contoso.MixedReality.Input".

### Recommended folder structure

It is recommended that the source code for data providers be layed out in a folder heirarchy as shown in the following image.

![Example folder structure](../Images/SpatialAwareness/ExampleProviderFolderStructure.png)

Where ContosoInput contains the implementation of the data provider, the Editor folder contains the inspector (and any other Unity editor specific code) and Profiles
contains one or more pre-made profiles.

## Implement the data provider

### Specify interface and/or base class inheritance

All input system data providers must implement the [`IMixedRealityInputDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputDeviceManager)
interface, which specifies the minimium functionality required by the input system. The MRTK foundation includes the [`BaseInputDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputDeviceManager)
class which provides a default implementation of this required functionality. For devices that build upon Unity's UInput class, the [`UnityJoystickManager`](xref:Microsoft.MixedReality.Toolkit.Input.UnityInput.UnityJoystickManager)
class can be used as a base class.

> [!Note]
> The `BaseInputDeviceManager` and `UnityJoystickManager` classes provide the required `IMixedRealityInputDeviceManager` implementation.

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
> The `BaseInputDevicemManager` class, via the `BaseService` class, provides only an empty implementations for `IMixedRealityDataProvider` methods. The details of these methods are generally data provider specific.

The methods that should be implemented by the data provider are:

- `Destroy()`
- `Disable()`
- `Enable()`
- `Initialize()`
- `Reset()`
- `Update()`

### Implement the data provider logic

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

To enable applications to respond to input from the user, the data provider raises notification events corresponding to controller state changes as defined in the [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler)
and [`IMixedRealityInputHandler<T>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) interfaces.

For digital (button) type controls, raise the OnInputDown and OnInputUp events.

``` c#
// inputAction is the input event that is to be raised.

if (interactionSourceState.touchpadPressed)
{
    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, inputAction);
}
else
{
    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, inputAction);
}
```

For analog controls (ex: touchpad position) the InputChanged event should be raised.

``` c#
InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.touchpadPosition);
```

### Apply the MixedRealityDataProvider attribute

The final step of creating an input system data provider is to apply the [`MixedRealityDataProvider`](xref:Microsoft.MixedReality.Toolkit.MixedRealityDataProviderAttribute)
attribute to the class. This step enables setting the default profile and platform(s) for the provider, when selected in the input system profile.

``` c#
[MixedRealityDataProvider(
    typeof(IMixedRealityInputSystem),
    SupportedPlatforms.WindowsUniversal,
    "Windows Mixed Reality Device Manager")]
public class WindowsMixedRealityDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
{ }
```

## Create the profile and inspector

In the Mixed Reality Toolkit, data providers are configured using [profiles](../Profiles/Profiles.md).

Data providers with additional configuration options (ex: [InputSimulationService](../InputSimulation/InputSimulationService.md)) should create a profile and inspector to allow
customers to modify the behavior to best suit the needs of the application.

> The complete code for the example in this section can be found in the MixedRealityToolkit.Services\InputSimulation folder.

### Define the profile

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

Profile inspectors are the user interface for configuring and viewing profile contents. Each profile inspector should extend the
[`BaseMixedRealityToolkitConfigurationProfileInspector]() class.

``` c#
[CustomEditor(typeof(MixedRealityInputSimulationProfile))]
public class SpatialObjectMeshObserverProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
{ }
```

The `CustomEditor` attribute informs Unity the type of asset to which the inspector applies.

## Create assembly definition(s)

The Mixed Reality Toolkit uses assembly definition ([.asmdef](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html)) files to specify dependencies
between components as well as to assist Unity in reducing compilation time.

It is recommended that assembly definition files are created for all data providers and their editor components.

Using the [folder structure](#recommended-folder-structure) in the earlier example, there would be two .asmdef files for the ContosoInput data provider.

The first assembly definition is for the data provider. For this example, it will be called ContosoInput and will be located in the example's ContosoInput folder.
This assembly definition must specify a dependency on Microsoft.MixedReality.Toolkit and any other assemblies upon which it depends.

The ContosoInputEditor assembly definition will specify the profile inspector and any editor specific code. This file must be located in the root folder of the editor code. In this example,
the file will be located in the ContosoInput\Editor folder. This assembly definition will contain a reference to the ContosoInput assembly as well as:

- Microsoft.MixedReality.Toolkit
- Microsoft.MixedReality.Toolkit.Editor.Inspectors
- Microsoft.MixedReality.Toolkit.Editor.Utilities

## Register the data provider

Once created, the data provider can be registered with the input system be used in the application.

![Registered input system data providers](../Images/Input/RegisteredServiceProviders.png)

## See also

- [Input system](Overview.md)
- [`BaseInputDeviceManager` class](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputDeviceManager)
- [`IMixedRealityInputDeviceManager` interface](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputDeviceManager)
- [`IMixedRealityInputHandler` interface](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler)
- [`IMixedRealityInputHandler<T>` interface](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1)
- [`IMixedRealityDataProvider` interface](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider)
- [`IMixedRealityCapabilityCheck` interface](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck)