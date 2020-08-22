# Pointers

![Pointer](../../Documentation/Images/Pointers/MRTK_Pointer_Main.png)

This article explains how to configure and respond to Pointer input in practice, compared to [Pointer Architecture](../Architecture/InputSystem/ControllersPointersAndFocus.md)

Pointers are instanced automatically at runtime when a new controller is detected. More than one pointer can be attached to a controller. For example, with the default pointer profile, Windows Mixed Reality controllers get both a line and a parabolic pointer for normal selection and teleportation respectively.

## Pointer configuration

Pointers are configured as part of the Input System in MRTK via a [`MixedRealityPointerProfile`](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityPointerProfile). This type of profile is assigned to a [`MixedRealityInputSystemProfile`](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile) in the MRTK Configuration inspector. The Pointer profile determines the cursor, types of Pointers available at runtime, and how those pointers communicate with each other to decide which one is active.

- *Pointing Extent* - Defines the max distance for which a Pointer can interact with a GameObject.

- *Pointing Raycast Layer Masks* - This is a prioritized array of LayerMasks to determine which possible GameObjects any given Pointer can interact with and the order of interaction to attempt. This may be useful to ensure Pointers interact with UI elements first before other scene objects.
![Pointer Profile Example](../Images/Input/Pointers/PointerProfile.PNG)

### Pointer options configuration

The default MRTK Pointer Profile configuration includes the following pointer classes and associated prefabs out-of-box. The list of pointers available to the system at runtime is defined under *Pointer Options* in the Pointer profile. Developers can utilize this list to reconfigure existing Pointers, add new Pointers, or delete one.

![Pointer Options Profile Example](../Images/Input/Pointers/PointerOptionsProfile.PNG)

Each Pointer entry is defined by the following set of data:

- *Controller Type* - The set of controllers that a pointer is valid for.
  - For example, the *PokePointer* is responsible for "poking" objects with a finger, and is, by default marked as only supporting the articulated hand controller type. Pointers are only instantiated when a controller becomes available and in particular the *Controller Type* defines what controllers this pointer prefab can be created with.

- *Handedness* - allows for a pointer to only being instantiated for a specific hand (left/right)

> [!NOTE]
> Setting the *Handedness* property of a Pointer entry to *None* will effectively disable it from the system as an alternative to removing that Pointer from the list.

- *Pointer Prefab* - This prefab asset will be instantiated when a controller matching the specified controller type and handedness starts being tracked.

It is possible to have multiple pointers associated with a controller. For example, in `DefaultHoloLens2InputSystemProfile` (Assets/MRTK/SDK/Profiles/HoloLens2/)
the articulated hand controller is associated with the *PokePointer*, *GrabPointer*, and the *DefaultControllerPointer* (i.e hand rays).

> [!NOTE]
> MRTK provides a set of pointer prefabs in *Assets/MRTK/SDK/Features/UX/Prefabs/Pointers*. A new custom prefab can be built as long as it contains one of the pointer scripts in *Assets/MRTK/SDK/Features/UX/Scripts/Pointers* or any other script implementing [`IMixedRealityPointer`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer).

### Default pointer classes

The following classes are the out-of-box MRTK pointers available and defined in the default *MRTK Pointer Profile* outlined above. Each pointer prefab provided under *Assets/MRTK/SDK/Features/UX/Prefabs/Pointers* contains one of the pointer components attached.

![MRTK Default Pointers](../Images/Input/Pointers/MRTK_Pointers.png)

#### Far pointers

##### [`LinePointer`](xref:Microsoft.MixedReality.Toolkit.Input.LinePointer)

 *LinePointer*, a base pointer class, draws a line from the source of the input (i.e. the controller) in the pointer direction and supports a single ray cast in this direction. Generally, children classes such as the [`ShellHandRayPointer`](xref:Microsoft.MixedReality.Toolkit.Input.ShellHandRayPointer) and the teleport pointers are instantiated and utilized (which also draw lines to indicate where teleportation will end up at) instead of this class which primarily provides common functionality.

For motion controllers like in Oculus, Vive, and Windows Mixed Reality, the rotation will match the rotation of the controller. For other controllers like HoloLens 2 articulated hands, the rotation matches the system-provided pointing pose of the hand.

<img src="../../Documentation/Images/Pointers/MRTK_Pointers_Line.png" width="400">

##### [`CurvePointer`](xref:Microsoft.MixedReality.Toolkit.Input.CurvePointer)

*CurvePointer* extends the *LinePointer* class by allowing for multi-step ray casts along a curve. This base pointer class is useful for curved instances such as teleportation pointers where the line consistently bends into a parabola.

##### [`ShellHandRayPointer`](xref:Microsoft.MixedReality.Toolkit.Input.ShellHandRayPointer)

The implementation of *ShellHandRayPointer*, which extends from [`LinePointer`](xref:Microsoft.MixedReality.Toolkit.Input.MousePointer), is used as the default for the *MRTK Pointer Profile*. The *DefaultControllerPointer* prefab implements the [`ShellHandRayPointer`](xref:Microsoft.MixedReality.Toolkit.Input.ShellHandRayPointer) class.

##### [`GGVPointer`](xref:Microsoft.MixedReality.Toolkit.Input.GGVPointer)

Also known as the *Gaze/Gesture/Voice (GGV)* pointer, the GGVPointer powers HoloLens 1-style look and tap interactions, primarily via Gaze and Air Tap or Gaze and voice Select interaction. The GGV pointer's position and direction is driven by the head's position and rotation.

##### [`TouchPointer`](xref:Microsoft.MixedReality.Toolkit.Input.TouchPointer)

The *TouchPointer* is responsible for working with Unity Touch input (i.e. touchscreen). These are 'far interactions' because the act of touching the screen will cast a ray from the camera to a potentially far location in the scene.

##### [`MousePointer`](xref:Microsoft.MixedReality.Toolkit.Input.MousePointer)

The *MousePointer* powers a screen to world raycast for far interactions, but for mouse instead of touch.

![Mouse pointer](../../Documentation/Images/Pointers/MRTK_MousePointer.png)

> [!NOTE]
> Mouse support is not available by default in MRTK but can be enabled by adding a new *Input Data Provider* of type [`MouseDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.UnityInput.MouseDeviceManager) to the MRTK input profile and assigning the [`MixedRealityMouseInputProfile`](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityMouseInputProfile) to the data provider.

#### Near pointers

##### [`PokePointer`](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer)

The *[PokePointer](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer)* is used to interact with game objects that support “near interaction touchable.” which are GameObjects that have an attached [`NearInteractionTouchable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) script. In the case of UnityUI, this pointer looks for NearInteractionTouchableUnityUIs.  The PokePointer uses a SphereCast to determine the closest touchable element and is used to power things like the pressable buttons.

 When configuring the GameObject with the [`NearInteractionTouchable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) component, make sure to configure the *localForward* parameter to point out of the front of the button or other object that should be made touchable. Also make sure that the touchable's *bounds* matches the bounds of the touchable object.

Useful Poke Pointer properties:

- *TouchableDistance*: Maximum distance in which a touchable surface can be interacted with
- *Visuals*: Game object used to render finger tip visual (the ring on finger, by default).
- *Line*: Optional line to draw from fingertip to the active input surface.
- *Poke Layer Masks* - A prioritized array of LayerMasks to determine which possible GameObjects the pointer can interact with and the order of interaction to attempt. Note that a GameObject must also have a `NearInteractionTouchable` component in order to interact with a poke pointer.

<img src="../../Documentation/Images/Pointers/MRTK_PokePointer.png" width="400">

##### [`SpherePointer`](xref:Microsoft.MixedReality.Toolkit.Input.SpherePointer)

The *[SpherePointer](xref:Microsoft.MixedReality.Toolkit.Input.SpherePointer)* uses [UnityEngine.Physics.OverlapSphere](https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html) in order to identify the closest [`NearInteractionGrabbable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable) object for interaction, which is useful for "grabbable" input like the `ManipulationHandler`. Similar to the [`PokePointer`](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer)/[`NearInteractionTouchable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) functional pair, in order to be interactable with the Sphere Pointer, the game object must contain a component that is the [`NearInteractionGrabbable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable) script.

<img src="../../Documentation/Images/Pointers/MRTK_GrabPointer.jpg" width="400">

Useful Sphere Pointer properties:

- *Sphere Cast Radius*: The radius for the sphere used to query for grabbable objects.
- *Near Object Margin*: The distance on top of the Sphere Cast Radius to query for detecting if an object is near the pointer. Total Near Object detection radius is Sphere Cast Radius + Near Object Margin
- *Near Object Sector Angle*: The angle around the forward axis of the pointer for querying for nearby objects. Makes the `IsNearObject` query function like a cone. This is set to 66 degrees by default to match Hololens 2 behavior

![Sphere pointer modified to only query for objects in the forward direction](https://user-images.githubusercontent.com/39840334/82500569-72d58300-9aa8-11ea-8102-ec9a62832d4e.png)

- *Near Object Smoothing Factor*: Smoothing factor for Near Object detection. If an object is detected in the Near Object Radius, the queried radius then becomes Near Object Radius * (1 + Near Object Smoothing Factor) to reduce the sensitivity and make it harder for an object to leave the detection range.
- *Grab Layer Masks* - A prioritized array of LayerMasks to determine which possible GameObjects the pointer can interact with and the order of interaction to attempt. Note that a GameObject must also have a `NearInteractionGrabbable` to interact with a SpherePointer.
    > [!NOTE]
    > The Spatial Awareness layer is disabled in the default GrabPointer prefab provided by MRTK. This is done to reduce performance impact of doing a sphere overlap query with the spatial mesh. You can enable this by modifying the GrabPointer prefab.
- *Ignore Colliders Not in FOV* - Whether to ignore colliders that may be near the pointer, but not actually in the visual FOV.
This can prevent accidental grabs, and will allow hand rays to turn on when you may be near
a grabbable but cannot see it. The *Visual FOV* is defined via a cone instead of the the typical frustum for performance reasons. This cone is centered and oriented the same as the camera's frustum with a radius equal to half display height(or vertical FOV).

<img src="../../Documentation/Images/Input/Pointers/SpherePointer_VisualFOV.png" width="200">



#### Teleport pointers

- [`TeleportPointer`](xref:Microsoft.MixedReality.Toolkit.Teleport.TeleportPointer) will raise a teleport request when action is taken (i.e the teleport button is pressed) in order to move the user.
- [`ParabolicTeleportPointer`](xref:Microsoft.MixedReality.Toolkit.Teleport.ParabolicTeleportPointer) will raise a teleport request when action is taken (i.e the teleport button is pressed) with a parabolic line raycast in order to move the user.

<img src="../../Documentation/Images/Pointers/MRTK_Pointers_Parabolic.png" width="400">

## Pointer support for mixed reality platforms

The following table details the pointer types that are typically used for the common platforms in MRTK. NOTE:
it's possible to add different pointer types to these platforms. For example, you could add a Poke pointer or Sphere pointer to VR. Additionally, VR devices with a gamepad could use the GGV pointer.

|                     | OpenVR  | Windows Mixed Reality | HoloLens 1 | HoloLens 2 |
|---------------------|---------|-----------------------|------------|------------|
| ShellHandRayPointer | Valid   | Valid                 |            | Valid      |
| TeleportPointer     | Valid   | Valid                 |            |            |
| GGVPointer          |         |                       | Valid      |            |
| SpherePointer       |         |                       |            | Valid      |
| PokePointer         |         |                       |            | Valid      |

## Pointer interactions via code

### Pointer event interfaces

MonoBehaviours that implement one or more of the following interfaces and are assigned to a GameObject with a `Collider` will receive Pointer interactions events as defined by the associated interface.

| Event | Description | Handler |
| --- | --- | --- |
| Before Focus Changed / Focus Changed | Raised on both the game object losing focus and the one gaining it every time a pointer changes focus. | [`IMixedRealityFocusChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusChangedHandler) |
Focus Enter / Exit | Raised on the game object gaining focus when the first pointer enters it and on the one losing focus when the last pointer leaves it. | [`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusHandler)
Pointer Down / Dragged / Up / Clicked | Raised to report pointer press, drag and release. | [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler)
Touch Started / Updated / Completed | Raised by touch-aware pointers like [`PokePointer`](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer) to report touch activity. | [`IMixedRealityTouchHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityTouchHandler)

> [!NOTE]
> [`IMixedRealityFocusChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusChangedHandler) and [`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusHandler) should be handled in the objects they are raised on. It is possible to receive focus events globally but, unlike other input events, global event handler won't block receiving events based on focus (the event will be received by both global handler and a corresponding object in focus).

#### Pointer input events in action

Pointer input events are recognized and handled by the MRTK input system in a similar way as [regular input events](InputEvents.md#input-events-in-action). The difference being that pointer input events are handled only by the GameObject in focus by the pointer that fired the input event, as well as any global input handlers. Regular input events are handled by GameObjects in focus for all active pointers.

1. The MRTK input system recognizes an input event has occurred
1. The MRTK input system fires the relevant interface function for the input event to all registered global input handlers
1. The input system determines which GameObject is in focus for the pointer that fired the event
    1. The input system utilizes the [Unity's Event System](https://docs.unity3d.com/Manual/EventSystem.html) to fire the relevant interface function for all matching components on the focused GameObject
    1. If at any point an input event has been [marked as used](inputevents.md#how-to-stop-input-events), the process will end and no further GameObjects will receive callbacks.
        - Example: Components implementing the interface [`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) will be searched for a GameObject gains or loses focus
        - Note: The Unity Event System will bubble up to search the parent GameObject if no components matching the desired interface are found on the current GameObject..
1. If no global input handlers are registered and no GameObject is found with a matching component/interface, then the input system will call each fallback registered input handlers

#### Example

Below is an example script that changes the color of the attached renderer when a pointer takes or leaves focus or when a pointer selects the object.

```c#
public class ColorTap : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityPointerHandler
{
    private Color color_IdleState = Color.cyan;
    private Color color_OnHover = Color.white;
    private Color color_OnSelect = Color.blue;
    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData)
    {
        material.color = color_OnHover;
    }

    void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
    {
        material.color = color_IdleState;
    }

    void IMixedRealityPointerHandler.OnPointerDown(
         MixedRealityPointerEventData eventData) { }

    void IMixedRealityPointerHandler.OnPointerDragged(
         MixedRealityPointerEventData eventData) { }

    void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        material.color = color_OnSelect;
    }
}
```

### Query pointers

It is possible to gather all pointers currently active by looping through the available input sources (i.e controllers and inputs available) to discover which pointers are attached to them.

```c#
var pointers = new HashSet<IMixedRealityPointer>();

// Find all valid pointers
foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
{
    foreach (var pointer in inputSource.Pointers)
    {
        if (pointer.IsInteractionEnabled && !pointers.Contains(pointer))
        {
            pointers.Add(pointer);
        }
    }
}
```

#### Primary pointer

Developers can subscribe to the FocusProviders PrimaryPointerChanged event to be notified when the primary pointer in focus has changed. This can be extremely useful to identify if the user is currently interacting with a scene via gaze or a hand ray or another input source.

```c#
private void OnEnable()
{
    var focusProvider = CoreServices.InputSystem?.FocusProvider;
    focusProvider?.SubscribeToPrimaryPointerChanged(OnPrimaryPointerChanged, true);
}

private void OnPrimaryPointerChanged(IMixedRealityPointer oldPointer, IMixedRealityPointer newPointer)
{
    ...
}

private void OnDisable()
{
    var focusProvider = CoreServices.InputSystem?.FocusProvider;
    focusProvider?.UnsubscribeFromPrimaryPointerChanged(OnPrimaryPointerChanged);

    // This flushes out the current primary pointer
    OnPrimaryPointerChanged(null, null);
}
```

The `PrimaryPointerExample` (Assets/MRTK/Examples/Demos/Input/Scenes/PrimaryPointer) scene shows how to use the [`PrimaryPointerChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.PrimaryPointerChangedHandler) for events to respond to a new primary pointer.

<img src="../../Documentation/Images/Pointers/PrimaryPointerExample.png" style="max-width:100%;">

### Pointer result

The pointer [`Result`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer.Result) property contains the current result for the scene query used to determine the object with focus. For a raycast pointer, like the ones created by default for motion controllers, gaze input and hand rays, it will contain the location and normal of the raycast hit.

```c#
private void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
{
    var result = eventData.Pointer.Result;
    var spawnPosition = result.Details.Point;
    var spawnRotation = Quaternion.LookRotation(result.Details.Normal);
    Instantiate(MyPrefab, spawnPosition, spawnRotation);
}
```

The `PointerResultExample` scene (Assets/MRTK/Examples/Demos/Input/Scenes/PointerResult/PointerResultExample.unity) shows how to use the pointer [`Result`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer.Result) to spawn an object at the hit location.

<img src="../../Documentation/Images/Input/PointerResultExample.png" style="max-width:100%;">

### Disable pointers

To turn enable and disable pointers (for example, to disable the hand ray), set the [`PointerBehavior`](xref:Microsoft.MixedReality.Toolkit.Input.PointerBehavior) for a given pointer type via [`PointerUtils`](xref:Microsoft.MixedReality.Toolkit.Input.PointerUtils).

```c#
// Disable the hand rays
PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);

// Disable hand rays for the right hand only
PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Right);

// Disable the gaze pointer
PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);

// Set the behavior to match HoloLens 1
// Note, if on HoloLens 2, you must configure your pointer profile to make the GGV pointer show up for articulated hands.
public void SetHoloLens1()
{
    PointerUtils.SetPokePointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
    PointerUtils.SetGrabPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
    PointerUtils.SetRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Any);
    PointerUtils.SetGGVBehavior(PointerBehavior.Default);
}
```

See [`PointerUtils`](xref:Microsoft.MixedReality.Toolkit.Input.PointerUtils) and [`TurnPointersOnOff`](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.DisablePointersExample) for more examples.

## Pointer interactions via editor

For pointer events handled by [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler), MRTK provides further convenience in the form of the [`PointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.PointerHandler) component, which allows pointer events to be handled directly via Unity Events.

<img src="../../Documentation/Images/Pointers/PointerHandler.png" style="max-width:100%;">

## Pointer extent

Far pointers have settings which limit how far they will raycast and interact with other objects in the scene.
By default, this value is set to 10 meters. This value was chosen to remain consistent with the behavior
of the HoloLens shell.

This can be changed by updating the `DefaultControllerPointer` prefab's
[`ShellHandRayPointer`](xref:Microsoft.MixedReality.Toolkit.Input.ShellHandRayPointer) component's
fields:

*Pointer Extent* - This controls the maximum distance that pointers will interact with.

*Default Pointer Extent* - This controls the length of the pointer ray/line that will
render when the pointer is not interacting with anything.

## See also

- [Pointer Architecture](../Architecture/InputSystem/ControllersPointersAndFocus.md)
- [Input Events](InputEvents.md)
