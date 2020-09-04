# Controllers, pointers, and focus

Controllers, pointers, and focus are higher-level concepts that build upon the foundation established by the core input system. Together, they provide a large portion of the mechanism for interacting with objects in the scene.

## Controllers

Controllers are representations of a physical controller (6-degrees of freedom, articulated hand, etc). They are created by device managers and are responsible for communicating with the corresponding underlying system and translating that data into MRTK-shaped data and events.

For example, on the Windows Mixed Reality platform, the [`WindowsMixedRealityArticulatedHand`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityArticulatedHand) is a controller that is responsible for interfacing with the underlying Windows [hand tracking APIs](https://docs.microsoft.com/uwp/api/windows.ui.input.spatial.spatialinteractionsourcestate) to get information about the joints, pose, and other properties of the hand. It is responsible for turning this data into relevant MRTK events (for example, by calling RaisePoseInputChanged or RaiseHandJointsUpdated) and by updating its own internal state so that queries for [`TryGetJointPose`](xref:Microsoft.MixedReality.Toolkit.Input.HandJointUtils.TryGetJointPose(TrackedHandJoint,Handedness,MixedRealityPose@)) will return correct data.

Generally, a controller's lifecycle will involve:

1. A controller gets created by a device manager upon detection of a new source (for example, the device manager detects and starts tracking a hand).

2. In the controller's Update() loop, it calls into its underlying API system.

3. In the same update loop, it raises input event changes by calling directly into the core input system itself (for example, raising HandMeshUpdated, or HandJointsUpdated).

## Pointers and focus

Pointers are used to interact with game objects. This section describes how pointers are created, how they get updated, and how they determine the object(s) that are in focus. It will also cover the different types of pointers that exist and the scenarios in which they are active.

### Pointer categories

Pointers generally fall into one of the following categories:

- **Far pointers**

  These types of pointers are used to interact with objects that are far away from the user (far away is defined as simply “not near”). These types of pointers generally cast lines that can go far into the world and allow the user to interact with and manipulate objects that are not immediately next to them.

- **Near pointers**

  These types of pointers are used to interact with objects that are close enough to the user to grab, touch, and manipulate. Generally, these types of pointers interact with objects by looking for objects in the nearby vicinity (either by doing raycasting at small distances, doing spherical casting looking for objects in the vicinity, or enumerating lists of objects that are considered grabbable/touchable).

- **Teleport pointers**

  These types of pointers plug into the teleportation system to handle moving the user to the location targeted by the pointer.

## Pointer mediation

Because a single controller can have multiple pointers (for example, the articulated hand can have both near and far interaction pointers), there exists a component that is responsible for mediating which pointer should be active.

For example, as the user’s hand approaches a pressable button, the [`ShellHandRayPointer`](xref:Microsoft.MixedReality.Toolkit.Input.ShellHandRayPointer) should stop showing, and the [`PokePointer`](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer) should be engaged.

This is handled by the [`DefaultPointerMediator`](xref:Microsoft.MixedReality.Toolkit.Input.DefaultPointerMediator),
which is responsible for determining which pointers are active, based on the state of all pointers. One of the key things this does is disable far pointers when a near pointer is close to an object (please see [`DefaultPointerMediator`](xref:Microsoft.MixedReality.Toolkit.Input.DefaultPointerMediator)).

It's possible to provide an alternate implementation of the pointer mediator by changing the [`PointerMediator`](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityPointerProfile.PointerMediator) property on the pointer profile.

### How to disable pointers

Because the pointer mediator runs every frame, it ends up controlling the active / inactive state of all pointers. Therefore, if you set a pointer's IsInteractionEnabled property in code, it will get overwritten by the pointer mediator every frame. Instead, you can specify the [`PointerBehavior`](xref:Microsoft.MixedReality.Toolkit.Input.PointerBehavior) to control whether pointers should be on or off yourself. Note that this will only work if you are using the default [`FocusProvider`](xref:Microsoft.MixedReality.Toolkit.Input.FocusProvider) and [`DefaultPointerMediator`](xref:Microsoft.MixedReality.Toolkit.Input.DefaultPointerMediator) in MRTK.

#### Example: Disable hand rays in MRTK

The following code will turn off the hand rays in MRTK:

```c#
// Turn off all hand rays
PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);

// Turn off hand rays for the right hand only
PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Right);
```

The following code will return hand rays to their default behavior in MRTK:

```c#
PointerUtils.SetHandRayPointerBehavior(PointerBehavior.Default);
```

The following code will force hand rays to be on, regardless if near a grabbable:

```c#
// Turn off all hand rays
PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOn);
```

See [`PointerUtils`](xref:Microsoft.MixedReality.Toolkit.Input.PointerUtils) and [`TurnPointersOnOff`](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.DisablePointersExample) for more examples.

### FocusProvider

The [`FocusProvider`](xref:Microsoft.MixedReality.Toolkit.Input.FocusProvider) is the workhorse that is responsible for
iterating over the list of all pointers and figuring out what the focused object is for each pointer.

In each `Update()` call, this will:

1. Update all of the pointers, by raycasting and doing hit detection as-configured by the pointer itself (for example, the sphere pointer could specify the SphereOverlap raycastMode, so FocusProvider will do a sphere-based collision)

2. Update the focused object on a per-pointer basis (i.e. if an object gained focus, it would also trigger events to those object, if an object lost focus, it would trigger focus lost, etc).

### Pointer configuration and lifecycle

[Pointers can be configured](../../Input/Pointers.md) in the *Pointers* section of the input system profile.

The lifetime of a pointer is generally the following:

1. A device manager will detect the presence of a controller. This device manager will then create the set of pointers associated with the controller via a call to [`RequestPointers`](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputDeviceManager).

2. The FocusProvider, in its Update() loop, will iterate over all of the valid pointers and do the associated raycast or hit detection logic. This is used to determine the object that is focused by each particular pointer.

    - Because it's possible to have multiple sources of input active at the same time (for example, two hands active present), it's also possible to have multiple objects that have focus at the same time.

3. The device manager, upon discovering that a controller source was lost, will tear down the pointers associated with the lost controller.
