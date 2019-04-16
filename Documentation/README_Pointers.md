# Pointers
![Pointer](../External/ReadMeImages/Pointers/MRTK_Pointer_Main.png)

A pointer is something attached to a controller that gives focus and dispatches inputs to the game object it is pointing to. Learn more about the MRTK input system [here](./Input/Overview.md), and a scripting/engineering overview of pointers [here](./Input/Pointers.md).

For a game object to be able to receive focus it must have a collider (so it can be hit by physics raycasts) and belong to one of the layers defined in the Pointer Raycast Layer Masks in the Pointer Profile.

Pointers are instantiated automatically at runtime when a new controller is detected. The pointers that are created for each controller type are defined in the _Pointer Options_ in the _Pointer Profile_. You can have more than one pointer attached to a controller; for example, with the default pointer profile, WMR controllers get both a line and a parabolic pointer for normal selection and teleportation respectively. Pointers communicate with each other to decide which one is active.

MRTK provides a set of pointer prefabs in _Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Pointers_. You can use your own prefabs as long as they contain one of the pointer scripts in _Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Pointers_ or any other script implementing `IMixedRealityPointer`.

![Pointer Profile](../External/ReadMeImages/Pointers/MRTK_PointerProfile.jpg)

## MRTK Pointer Prefabs

### Line Pointer
<img src="../External/ReadMeImages/Pointers/MRTK_Pointers_Line.png" width="600">


A line pointer is a ray attached to a controller. The line starts at the controller's base, and its pointing direction matches the pointing direction of the controller. 

Line pointers are commonly used when you have two controllers which have a position, and a pointing direction. 

For motion controllers like in Oculus, Vive, Windows Mixed Reality, the rotation will match the rotation of the controller.

For other controllers like HoloLens 2 articulated hands, the rotation matches the system-provided pointing pose of the hand.


### GGV Pointer
GGV stands for "Gaze, Gesture, Voice"<sup>[2](https://docs.microsoft.com/en-us/windows/mixed-reality/gaze)</sup>. The GGV pointer's position and direction is driven by the head's position and rotation. The pointer is used to provide input that matches the HoloLens V1 input style of head gaze + airtap<sup>[3](https://docs.microsoft.com/en-us/windows/mixed-reality/gestures)</sup>.

In the pointer profile you can see that the V1 HoloLens input system is provided for you via the mapping of "GGVHand" (V1 HoloLens hand) to the GGVPointer. 

<img src="../External/ReadMeImages/Pointers/MRTK_GGVPointer_HL1.jpg" width="600">

You can also simulate the V1 HoloLens GGV behavior on HoloLens 2 by mapping the articulated hand to the GGVPointer.

<img src="../External/ReadMeImages/Pointers/MRTK_GGVPointer_HL2.jpg" width="600">



### Grab Pointer / Sphere Pointer
<img src="../External/ReadMeImages/Pointers/MRTK_GrabPointer.jpg" width="600">

Grab pointer is used for near interactions, specifically for grabbing items near the hand / controller. 

**NOTE:** Only items with _Near Interaction Grabbable_ components will respond to the sphere pointer.

#### Sphere Pointer Properties
Sphere Cast Radius - The radius for the sphere used to query for grabbable objects.
Debug Mode - If true, draw the sphere that is used to query for grabbable objects


### Poke Pointer
<img src="../External/ReadMeImages/Pointers/MRTK_PokePointer.jpg" width="600">

Poke pointer is for near interactions, specifically for touching objects via the hand / controller.

**NOTE:** Only items with _Near Interaction Touchable_ components will respond to the poke pointer.

#### Poke Pointer Properties
- distBack - how far input point can go behind surface before release event occurs
- distFront - how far input point needs to be in front of surface before press event occurs
- debounceThreshold - once touching, the distance back hand needs to be pulled from surface before a touch release occurs.

#### Configuring NearInteractionTouchable
When configuring your _Near Interaction Touchable_, make sure to configure the _localForward_ parameter to point out of the front of the button or other object you wish to make touchable. Also make sure that the touchable's _bounds_ matches the bounds of your touchable object. 

Other parameters:

- eventsToReceive - if set to Touch, the object will receive _OnTouchDown_ _OnTouchUpdate_ and _OnTouchUp_ events. If set to pointer, the object will receive _OnPointerDown_ and _OnPointerUp_ events.
- touchableSurface - use this to help compute the bounds of the surface. Bounds will automatically adjust to the attached BoxCollider, UnityUI element, or can be set manually by adjusting _bounds_.
- visuals - Game Object used to render finger tip visual (the ring on finger, by default)
- line - optional line to draw from fingertip to the active input surface.



### Mouse pointer
![Mouse Pointer](../External/ReadMeImages/Pointers/MRTK_MousePointer.jpg)

Mouse pointer is used for interacting with the mouse. This pointer will only be used if the active input is a mouse. By default, the MRTK Profile does not provide a mouse as an input provider, as mouse input is instead used to simulate hand input. You may add a mouse in your MRTK profily by modifying the _Registered Service Providers_ in your MRTK profile.

### Teleport pointer
<img src="../External/ReadMeImages/Pointers/MRTK_Pointers_Parabolic.png" width="600">

Teleport pointer is used for teleportation. This pointer will only be active when using a controller that supports teleportation (such as a motion controller like in Oculus, Vive, Windows Mixed Reality).
