# Pointers #

![Pointer](../Documentation/Images/Pointers/MRTK_Pointer_Main.png)

A [pointer](./Input/Pointers.md) is something attached to a controller that gives focus and dispatches [inputs]((./Input/Overview.md)) to the game object it is pointing to. 

For a game object to be able to receive focus it must have a collider (so it can be hit by physics raycasts) and belong to one of the layers defined in the pointer raycast layer masks in the pointer profile.

Pointers are instantiated automatically at runtime when a new controller is detected. The pointers that are created for each controller type are defined in the *pointer options* in the *pointer profile*. You can have more than one pointer attached to a controller; for example, with the default pointer profile, WMR controllers get both a line and a parabolic pointer for normal selection and teleportation respectively. Pointers communicate with each other to decide which one is active.

MRTK provides a set of pointer prefabs in *Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Pointers*. You can use your own prefabs as long as they contain one of the pointer scripts in *Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Pointers* or any other script implementing `IMixedRealityPointer`.

![Pointer Profile](../Documentation/Images/Pointers/MRTK_PointerProfile.jpg)

## MRTK pointer prefabs ##

**Line pointer**
A line pointer is a ray attached to a controller. The line starts at the controller's base, and its pointing direction matches the pointing direction of the controller. 

<img src="../Documentation/Images/Pointers/MRTK_Pointers_Line.png" width="600">

Line pointers are commonly used when you have two controllers which have a position, and a pointing direction. 

For motion controllers like in Oculus, Vive, Windows Mixed Reality, the rotation will match the rotation of the controller.

For other controllers like HoloLens 2 articulated hands, the rotation matches the system-provided pointing pose of the hand.

**GGV pointer**
GGV stands for "Gaze, Gesture, Voice"<sup>[2](https://docs.microsoft.com/en-us/windows/mixed-reality/gaze)</sup>. The GGV pointer's position and direction is driven by the head's position and rotation. The pointer is used to provide input that matches the HoloLens V1 input style of head gaze + airtap<sup>[3](https://docs.microsoft.com/en-us/windows/mixed-reality/gestures)</sup>.

In the pointer profile you can see that the HoloLens V1 input system is provided for you via the mapping of "GGVHand" (V1 HoloLens hand) to the GGVPointer. 

<img src="../Documentation/Images/Pointers/MRTK_GGVPointer_HL1.jpg" width="600">

You can also simulate the V1 HoloLens GGV behavior on HoloLens 2 by mapping the articulated hand to the GGVPointer.

<img src="../Documentation/Images/Pointers/MRTK_GGVPointer_HL2.jpg" width="600">

**Grab pointer / sphere pointer**
Grab pointer is used for near interactions, specifically for grabbing items near the hand / controller. 

<img src="../Documentation/Images/Pointers/MRTK_GrabPointer.jpg" width="600">

**NOTE:** Only items with *Near Interaction Grabbable* components will respond to the sphere pointer.

Sphere pointer properties:
* *Sphere Cast Radius*: The radius for the sphere used to query for grabbable objects.
* *Debug Mode*: If true, draw the sphere that is used to query for grabbable objects.

**Poke pointer**
Poke pointer is for near interactions, specifically for touching objects via the hand / controller.

<img src="../Documentation/Images/Pointers/MRTK_PokePointer.jpg" width="600">

Only items with *Near Interaction Touchable* components will respond to the poke pointer. When configuring your *Near Interaction Touchable*, make sure to configure the *localForward* parameter to point out of the front of the button or other object you wish to make touchable. Also make sure that the touchable's *bounds* matches the bounds of your touchable object. 

Poke pointer properties:
* *distBack*: How far input point can go behind surface before release event occurs.
* *distFront*: How far input point needs to be in front of surface before press event occurs.
* *debounceThreshold*: Once touching, the distance back hand needs to be pulled from surface before a touch release occurs.

Other parameters:
* *eventsToReceive*: If set to touch, the object will receive _OnTouchDown_ _OnTouchUpdate_ and _OnTouchUp_ events. If set to pointer, the object will receive _OnPointerDown_ and _OnPointerUp_ events.
* *touchableSurface*: Use this to help compute the bounds of the surface. Bounds will automatically adjust to the attached box collider, Unity UI element, or can be set manually by adjusting _bounds_.
* *visuals*: Game object used to render finger tip visual (the ring on finger, by default).
* *line*: Optional line to draw from fingertip to the active input surface.

**Mouse pointer**
Mouse pointer is used for interacting with the mouse. This pointer will only be used if the active input is a mouse. By default, the MRTK profile does not provide a mouse as an input provider, as mouse input is instead used to simulate hand input. You may add a mouse in your MRTK profile by modifying the *Registered Service Providers* in your MRTK profile.

![Mouse pointer](../Documentation/Images/Pointers/MRTK_MousePointer.jpg)

**Teleport pointer**
Teleport pointer is used for teleportation. This pointer will only be active when using a controller that supports teleportation (such as a motion controller like in Oculus, Vive, Windows Mixed Reality).

<img src="../Documentation/Images/Pointers/MRTK_Pointers_Parabolic.png" width="600">
