# Input actions

[**Input Actions**](InputActions.md) are abstractions over raw inputs meant to help isolating application logic from the specific input sources producing an input. It can be useful, for example, to define a *Select* action and map it to the left mouse button, a button in a gamepad and a trigger in a 6 DOF controller. You can then have your application logic listen for *Select* input action events instead of having to be aware of all the different inputs that can produce it.

## Creating an input action

Input actions are configured in the **Input Actions Profile**, inside the *Input System Profile* in the Mixed Reality Toolkit component, specifying a name for the action and the type of inputs (*Axis Constraint*) it can be mapped to:

<img src="../../Documentation/Images/Input/InputActions.png" style="max-width:100%;">

These are the most mostly commonly used values for **Axis Constraint**:

Axis Constraint | Description
--- | ---
Digital | On/off input like a binary button in a gamepad or mouse.
Single Axis | Single axis analogue input like an analog trigger in a gamepad.
Dual Axis | Dual axis analogue input like a thumbstick.
Six Dof | 3D pose with translation and rotation like the one produced by 6 DOF controllers.

You can find the full list in [`AxisType`](xref:Microsoft.MixedReality.Toolkit.Utilities.AxisType).

## Mapping input to actions

The way you map an input to and action depends on the type of the input source:

### Controller input

Go to the **Controller Input Mapping Profile**, under the *Input System Profile*. There you will find a list of all supported controllers:

<img src="../../Documentation/Images/Input/ControllerInputMappingProfile.PNG" style="max-width:100%;">

Select the one you want to configure and a dialog window will appear with all the controller inputs, allowing you to set an action for each of them:

<img src="../../Documentation/Images/Input/InputActionAssignment.PNG" style="max-width:100%;">

### Speech input

In the **Speech Command Profile**, under the *Input System Profile*, you'll find the list of currently defined speech commands. To map one of them to an action, just select it in the *Action* drop down.

<img src="../../Documentation/Images/Input/SpeechCommandsProfile.png" style="max-width:100%;">

### Gesture input

The **Gestures Profile**, under the *Input System Profile*, contains all defined gestures. You can map each of them to an action by selecting it in the *Action* drop down.

<img src="../../Documentation/Images/Input/GestureProfile.png" style="max-width:100%;">

## Handling input actions

> [!WARNING]
> Currently only input actions of *Digital* type can be handled using the methods described in this section. For other action types, you'll have to handle directly the events for the corresponding inputs instead. For example, to handle a 6 DOF action mapped to controller inputs, you'll have to use [`IMixedRealityGestureHandler<T>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) with T = [`MixedRealityPose`](xref:Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose).

The easiest way to handle input actions is to make use of the [`InputActionHandler`](xref:Microsoft.MixedReality.Toolkit.Input.InputActionHandler) script. This allows you to define the action you want to listen to and react to action started and ended events using Unity Events.

<img src="../../Documentation/Images/Input/InputActionHandler.PNG" style="max-width:100%;">

If you want more control, you can implement the [`IMixedRealityInputActionHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputActionHandler) interface directly in your script. See the [**Input Events**](InputEvents.md) section for more details on event handling via handler interfaces.

## Examples

See `MRTK/Examples/Demos/Input/Scenes/InputActions` for an example scene showing how to create an action, map it to controller, speech and gesture inputs and use it to rotate an object on command.

<img src="../../Documentation/Images/Input/InputActionsExample.PNG" style="max-width:100%;">
