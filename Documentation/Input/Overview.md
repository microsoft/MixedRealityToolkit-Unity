# Input overview

The Input System in MRTK allows you to:

- Consume inputs from a variety of input sources, such as 6 DOF controllers, articulated hands or speech, via input events.
- Define abstract actions, like *Select* or *Menu*, and associate them to different inputs.
- Setup pointers attached to controllers to drive UI components via focus and pointer events.

<img src="../../Documentation/Images/Input/MRTK_InputSystem.png" style="display:block;margin-left:auto;margin-right:auto;">
<sup>Overview of MRTK Input System</sup>

Inputs are produced by [**Input Data Providers(Device Manager)**](InputProviders.md). Each provider corresponds to a particular source of input: Open VR, Windows Mixed Reality (WMR), Unity Joystick, Windows Speech, etc. Providers are added to your project via the **Registered Service Providers Profile** in the *Mixed Reality Toolkit* component and will produce [**Input Events**](InputEvents.md) automatically when the corresponding input sources are available (e.g. when a WMR controller is detected or a gamepad connected).

[**Input Actions**](InputActions.md) are abstractions over raw inputs meant to help isolate application logic from the specific input sources producing an input. It can be useful, for example, to define a *Select* action and map it to the left mouse button, a button in a gamepad and a trigger in a 6 DOF controller. You can then have your application logic listen for *Select* input action events instead of having to be aware of all the different inputs that can produce it. Input Actions are defined in the **Input Actions Profile**, found within the *Input System Profile* in the *Mixed Reality Toolkit* component.

[**Controllers**](Controllers.md) are created by *input providers* when input devices are detected and destroyed when they're lost or disconnected. The WMR input provider, for example, will create *WMR controllers* for 6 DOF devices and *WMR articulated hand controllers* for articulated hands. Controller inputs can be mapped to input actions via the **Controller Mapping Profile**, inside the *Input System Profile*. Inputs events raised by controllers will include the associated input action, if any.

Controllers can have [**Pointers**](Pointers.md) attached to them that query the scene to determine the game object with focus and raise [**Pointer Events**](Pointers.md#pointer-event-interfaces) on it. As an example, our *line pointer* performs a raycast against the scene using the controller pose to compute the origin and direction of the ray. The pointers created for each controller are set up in the **Pointer Profile**, under the *Input System Profile*.

<img src="../../Documentation/Images/Input/MRTK_Input_EventFlow.png" width="200px" style="display:block;margin-left:auto;margin-right:auto;">
<sup>Event flow.</sup>

While you can handle [input events directly in UI components](InputEvents.md), it is recommended to use [pointer events](pointers.md#pointer-event-interfaces) to keep the implementation device-independent.

MRTK also provides several convenience methods to query input state directly in a device-independent way. See [Accessing input state in MRTK](InputState.md) for more details.
