# Input Simulation Service

The Input Simulation Service emulates the behaviour of devices and platforms that may not be available in the Unity editor. Examples include:

* HoloLens or VR device head tracking
* HoloLens hand gestures
* HoloLens 2 articulated hand tracking
* HoloLens 2 eye tracking

Users can use a conventional keyboard and mouse combination to control simulated devices at runtime. This allows testing of interactions in the Unity editor without first deploying to a device.

> [!WARNING]
> This does not work when using Unity's XR Holographic Emulation > Emulation Mode = "Simulate in Editor". Unity's in-editor simulation will take control away from MRTK's input simulation. In order to use the MRTK input simulation service, you will need to set XR Holographic Emulation to Emulation Mode = *"None"*

## Enabling the Input Simulation Service

Input simulation is enabled by default in MRTK.

Input simulation is an optional [Mixed Reality service](../MixedRealityServices.md). It can be added as a data provider in the [Input System profile](../Input/InputProviders.md).

* __Type__ must be _Microsoft.MixedReality.Toolkit.Input > InputSimulationService_.
* __Platform(s)__ by default includes all _Editor_ platforms, since the service uses keyboard and mouse input.

## Input simulation tools window

Enable the input simulation tools window from the  _Mixed Reality Toolkit > Utilities > Input Simulation_ menu. This window provides access to the state of input simulation during play mode.

## Viewport Buttons

A prefab for in-editor buttons to control basic hand placement can be specified in the input simulation profile under __Indicators Prefab__. This is an optional utility, the same features can be accessed in the [input simulation tools window](#input-simulation-tools-window).

> [!NOTE]
> The viewport indicators are disabled by default, as they currently sometimes interfere with Unity UI interactions, see issue [#6106](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6106). To enable, add the InputSimulationIndicators prefab to __Indicators Prefab__.

Hand icons show the state of the simulated hands:

* ![Untracked hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Untracked.png "Untracked hand icon") The hand is not tracking. Click to enable the hand.
* ![Tracked hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Tracked.png "Tracked hand icon") The hand is tracked, but not controlled by user. Click to hide the hand.
* ![Controlled hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Controlled.png "Controlled hand icon") The hand is tracked and controlled by user. Click to hide the hand.
* ![Reset hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Reset.png "Reset hand icon") Click to reset the hand to default position.

## Camera Control

Head movement can be emulated by the Input Simulation Service.

### Rotating the camera

1. Hover over the viewport editor window.
    _You may need to click the window to give it input focus if button presses don't work._
1. Press and hold the __Mouse Look Button__ (default: Right mouse button).
1. Move the mouse in the viewport window to rotate the camera.
1. Use the scroll wheel to roll the camera around the view direction.

Camera rotation speed can be configured by changing the __Mouse Look Speed__ setting in the input simulation profile.

Alternatively use the __Look Horizontal__/__Look Vertical__ axes to rotate the camera (default: game controller right thumbstick).

### Moving the camera

Use the __Move Horizontal__/__Move Vertical__ axes to move the camera (default: WASD keys or game controller left thumbstick).

Camera position and rotation angles can be set explicitly in the tools window as well. The camera can be reset to its default using the __Reset__ button.

<iframe width="560" height="315" src="https://www.youtube.com/embed/Z7L4I1ET7GU" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## Hand Simulation

The input simulation supports emulated hand devices. These virtual hands can interact with any object that supports regular hand devices, such as buttons or grabbable objects.

### Hand Simulation Mode

In the [input simulation tools window](#input-simulation-tools-window) the __Hand Simulation Mode__ setting switches between two distinct input models. The default mode can also be set in the input simulation profile.

* _Articulated Hands_: Simulates a fully articulated hand device with joint position data.

   Emulates HoloLens 2 interaction model.

   Interactions that are based on precise positioning of the hand or use touching can be simulated in this mode.

* _Gestures_: Simulates a simplified hand model with air tap and basic gestures.

   Emulates [HoloLens interaction model](https://docs.microsoft.com/en-us/windows/mixed-reality/gestures).

   Focus is controlled using the Gaze pointer. The _Air Tap_ gesture is used to interact with buttons.

### Controlling hand movement

Press and hold the __Left/Right Hand Control Key__ (default: *Left Shift* for left hand and *Space* for right hand) to gain control of either hand. While the manipulation key is pressed, the hand will appear in the viewport. Once the manipulation key is released the hands will disappear after a short __Hand Hide Timeout__.

Hands can be toggle on permanently in the [input simulation tools window](#input-simulation-tools-window) or by pressing the __Toggle Left/Right Hand Key__ (default: *T* for left and *Y* for right). Press the toggle key again to hide the hands again.

Mouse movement will move the hand in the view plane. Hands can be moved further or closer to the camera using the __mouse wheel__.

To rotate hands using the mouse, hold both the __Left/Right Hand Control Key__ (*Left Shift* or *Space*) _and_ the __Hand Rotate Button__ (default: *cntrl* button) then move the mouse to rotate the hand. Hand rotation speed can be configured by changing the __Mouse Hand Rotation Speed__ setting in the input simulation profile.

All hand placement can also changed in the [input simulation tools window](#input-simulation-tools-window), including resetting hands to default.

### Additional profile settings

* __Hand Depth Multiplier__ controls the sensitivity of the mouse scroll wheel depth movement. A larger number will speed up hand zoom.
* __Default Hand Distance__ is the initial distance of hands from the camera. Clicking the __Reset__ button hands will also place hands at this distance.
* __Hand Jitter Amount__ adds random motion to hands. This can be used to simulate inaccurate hand tracking on device, and ensure that interactions work well with noisy input.

<iframe width="560" height="315" src="https://www.youtube.com/embed/uRYfwuqsjBQ" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### Hand Gestures

Hand gestures such as pinching, grabbing, poking, etc. can also be simulated.

1. First enable hand control using the __Left/Right Hand Control Key__ (*Left Shift* or *Space*)

   Alternatively toggle the hands on/off using the toggle keys (*T* or *Y*).

2. While manipulating, press and hold a mouse button to perform a hand gesture.

Each of the mouse buttons can be mapped to transform the hand shape into a different gesture using the _Left/Middle/Right Mouse Hand Gesture_ settings. The _Default Hand Gesture_ is the shape of the hand when no button is pressed.

> [!NOTE]
> The _Pinch_ gesture is the only gesture that performs the "Select" action at this point.

### One-Hand Manipulation

1. Press and  __Left/Right Hand Control Key__ (*Left Shift* or *Space*)
2. Point at object
3. Hold mouse button to pinch
4. Use mouse to move the object
5. Release mouse button to stop interaction

<iframe width="560" height="315" src="https://www.youtube.com/embed/rM0xaHam6wM" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### Two-Hand Manipulation

For manipulating objects with two hands at the same time the persistent hand mode is recommended.

1. Toggle on both hands by pressing the toggle keys (T/Y).
1. Manipulate one hand at a time:
    1. Hold _Space_ to control the right hand
    1. Move the hand to where you want to grab the object
    1. Press mouse button to activate the _Pinch_ gesture. In persistent mode the gesture will remain active when you release the mouse button.
1. Repeat the process with the other hand, grabbing the same object in a second spot.
1. Now that both hands are grabbing the same object, you can move either of them to perform two-handed manipulation.

<iframe width="560" height="315" src="https://www.youtube.com/embed/Qol5OFNfN14" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### GGV Interaction

1. Enable GGV simulation by switching __Hand Simulation Mode__ to _Gestures_ in the [Input Simulation Profile](#enabling-the-input-simulation-service)
1. Rotate the camera to point the gaze cursor at the interactable object (right mouse button)
1. Hold _Space_ to control the right hand
1. Click and hold _left mouse button_ to interact
1. Rotate the camera again to manipulate the object

<iframe width="560" height="315" src="https://www.youtube.com/embed/6841rRMdqWw" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### Eye tracking

[Eye tracking simulation](../EyeTracking/EyeTracking_BasicSetup.md#simulating-eye-tracking-in-the-unity-editor) can be enabled by checking the __Simulate Eye Position__ option in the
[Input Simulation Profile](#enabling-the-input-simulation-service). This should not be used with GGV
style interactions (so ensure that __Hand Simulation Mode__ is set to _Articulated_).
