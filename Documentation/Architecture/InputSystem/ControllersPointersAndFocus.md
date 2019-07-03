# Controllers, Pointers, and Focus

Controllers, pointers, and focus are higher level concepts that build upon the foundation established
by the core input system. Together they provide a large portion of the mechanism for interacting
with objects in the scene.

## Controllers

Controllers are representations of a physical controller (6-degree of freedom, articulated hand, etc).
They are created by device managers, and are responsible for communicating with the corresponding
underlying system and translating that data into MRTK-shaped data and events.

For example, on the Windows Mixed Reality platform, the
[WindowsMixedRealityArticulatedHand](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityArticulatedHand)
is a controller that is responsible for interfacing with the underlying Windows
[hand tracking APIs](https://docs.microsoft.com/en-us/uwp/api/windows.ui.input.spatial.spatialinteractionsourcestate) to get
information about the joints, pose, and other properties of the hand. It is responsible for turning this data into relevant
MRTK events (for example, by calling RaisePoseInputChanged or RaiseHandJointsUpdated) and by updating its own internal
state so that queries for [TryGetJointPose](xref:Microsoft.MixedReality.Toolkit.Input.HandJointUtils.TryGetJointPose(TrackedHandJoint,Handedness,MixedRealityPose@))
will return correct data.

Generally, a controller's lifecycle will involve:

1. A controller gets created by a device manager upon detection of a new source (for example, the
   detects and starts tracking a hand).
2. In the controller's Update() loop, it calls into its underlying API system.
3. In the same update loop, it raises input event changes by calling directly into the
   core input system itself (for example, raising HandMeshUpdated, or HandJointsUpdated).

## Pointers and focus

Pointers are used to interact with game objects. This section describes how pointers are created,
how they get updated, and how they determine the object(s) that are in focus. It will
also cover the different types of pointers that exist and the scenarios in which they are active.

### Pointer categories

Pointers generally fall into one of the following categories:

- Far pointers

  These types of pointers are used to interact with objects that are far away from the user (where
  far away is defined as simply “not near”). These types of pointers generally cast lines that
  can go far into the world and allow the user the interact with and manipulate objects that
  aren’t immediately next to them.

- Near pointers

  These types of pointers are used to interact with objects that are close enough to the user to
  grab, touch, and manipulate. Generally these types of pointers interact with objects by looking
  for objects in the nearby vicinity (either by doing raycasting at small distances, doing spherical
  casting looking for objects in the vicinity, or enumerating lists of objects that are considered
  grabbable/touchable).

- Teleport pointers

  These types of pointers plug into the teleportation system to handle moving the user to the location
  targetted by the pointer.

## Far pointers

The following are far pointers that come with the MRTK:

### ShellHandRayPointer

[This pointer](xref:Microsoft.MixedReality.Toolkit.Input.ShellHandRayPointer) shoots a line from
the palm of the hand of the articulated hand controller.

### GGVPointer

The [Gaze/Gesture/Voice pointer](xref:Microsoft.MixedReality.Toolkit.Input.GGVPointer)
(aka HoloLens 1 pointer). This is the pointer that powers HoloLens 1-style look and tap interactions.

### TouchPointer

[This pointer](xref:Microsoft.MixedReality.Toolkit.Input.TouchPointer) is responsible for working
with Unity Touch input (i.e. touchscreen). These are 'far interactions' because the act of touching
the screen will cast a ray from the camera to a potentially far location in the scene.

### MousePointer

[This pointer](xref:Microsoft.MixedReality.Toolkit.Input.MousePointer) powers a screen to world raycast for far interactions, but for mouse instead of touch.

### LinePointer

[A base pointer class](xref:Microsoft.MixedReality.Toolkit.Input.MousePointer) that draws lines from the source of the input (i.e. the controller) in the pointer direction. This is also used by other things like the Shell Hand Ray Pointer and the teleport pointers (which also draw lines to indicate where teleportation will end up at).

## Near pointers

### PokePointer

[A pointer](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer) that is used to interact with game objects
that support “near interaction touchable.” GameObjects support "near interaction touchable" when they
have attach the [NearInteractionTouchable](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) script.
The PokePointer uses a SphereCast to determine the closest touchable (or in the case of interaction with
UnityUI, enumerate through the list of NearInteractionTouchableUnityUIs). This type of pointer/NearInteractionTouchable combination is used to power things like the pressable buttons and the piano
keys (in the hand interaction examples scene).

### SpherePointer

[A pointer](xref:Microsoft.MixedReality.Toolkit.Input.SpherePointer) that uses
[UnityEngine.Physics.OverlapSphere](https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html)
in order to identify the closest [NearInteractionGrabbable](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable)
object that should be interacted with. Similar to the PokePointer/NearInteractionTouchable functional
pair, in order to be interactable with the Sphere Pointer, the game object must contain a component that
is the NearInteractionGrabbable script. This type of pointer/NearInteractionGrabbable combination is used
to power higher level scripts like the ManipulationHandler (which provides rotate/resize/translate
capabilities).

## Teleport pointers

### TeleportPointer and ParabolicTeleportPointer

Both are pointers which, when action is taken (i.e. the teleport button is pressed) will raise a teleport request with the teleport system in order to move the user. 

## Pointer Mediation

Because a single controller can have multiple pointers (for example, the articulated hand can have both
near and far interaction pointers), there exists a component that is responsible for mediating which
pointer should be active.

For example, as the user’s hand approaches a pressable button, the ShellHandRayPointer should stop
showing, and the PokePointer should be engaged.

This is handled by the [DefaultPointerMediator](xref:Microsoft.MixedReality.Toolkit.Input.DefaultPointerMediator),
which is responsible for determining which pointers based on the state of all pointers.
One of the key things this does is [disable far pointers when a near pointer is close to an
object](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Pointers/DefaultPointerMediator.cs#L127)

It's possible to provide an alternate implementation of the pointer mediator by changing the
[PointerMediator](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityPointerProfile.PointerMediator)
property on the pointer profile.

### FocusProvider

[This is the workhorse](xref:Microsoft.MixedReality.Toolkit.Input.FocusProvider) that is responsible for
iterating over the list of all pointers and figuring out what the focused object is for each pointer.

In each Update() call, this will:

1. Update all of the pointers, by raycasting and doing hit detection as-configured by the pointer itself
   (for example, the sphere pointer could specify the SphereOverlap raycastMode, so FocusProvider will do a sphere-based collision)
2. Update the focused object on a per-pointer basis (i.e. if an object gained focus, it would also trigger
   events to those object, if an object lost focus, it would trigger focus lost, etc).


### Pointer configuration and lifecycle

Pointers can be figured in the **Pointers** section of the input system profile:

![](../../Images/Input/PointerProfile.png)

A pointer is defined by the following set of data:

1. The set of controllers that a pointer is valid for. For example, the PokePointer
   is responsible for "poking" objects with a finger, and is, by default, marked as
   only supporting the articulated hand controller type.
2. The handedness that the pointer supports. All inbox pointers are configured to
   work with any hand - it's possible to restrict a pointer to only being instantiated
   for a specific hand (left/right) by changing its handedness configuration.
3. The actual pointer prefab that will be instantiated when a controller matching the
   specified controller type and handedness starts being tracked.

It is possible to have multiple pointers associated with a controller. For example,
in the [default HoloLens 2 profile](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/HoloLens2/DefaultHoloLens2InputSystemProfile.asset)
the articulated hand controller is associated with the PokePointer, GrabPointer, and the
DefaultControllerPointer (which has hand rays).

The lifetime of a pointer generally looks like:

1. A device manager will detect the presence of a controller - this device manager will
   then create the set of pointers associated with the controller (via a call to
   [RequestPointers](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputDeviceManager)).
2. The FocusProvider, in its Update() loop, will iterate over all of the valid pointers
   and do the associated raycast or hit detection logic - this is used to determine
   the object that is focused by each particular pointer.

   Because it's possible to have multiple sources of input active at the same time (for example,
   two hands active present), it's also possible to have multiple objects that have focus at the
   same time.
3. The device manager, upon discovering that a controller source was lost, will tear down
   the pointers associated with the lost controller.






