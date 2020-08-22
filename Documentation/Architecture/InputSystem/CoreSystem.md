# Core system

At the heart of the input system is the [InputSystem](../../Input/Overview.md), which is a service that is responsible for initializing and operating all of the input related functionality associated with the MRTK.

> [!NOTE]
> It is assumed that readers have already read and have a basic understanding of the
> [terminology](Terminology.md) section.

This service is responsible for:

- Reading the [input system profile](../../MixedRealityConfigurationGuide.md#input-system-settings)
- Starting the configured [data providers](../../Input/InputProviders.md) (for example, `Windows Mixed Reality Device Manager` and `OpenVR Device Manager`).
- Instantiation of the [GazeProvider](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGazeProvider), which is a component that is responsible for providing HoloLens (1st generation) style head gaze information
  in addition to HoloLens 2 style eye gaze information.
- Instantiation of the [FocusProvider](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusProvider), which is a component that is responsible for determining objects that have focus. This
  is described in more depth in the [pointers and focus](ControllersPointersAndFocus.md#pointers-and-focus) section of the
  documentation.
- Providing registration points for all input events (as [global listeners](#global-listeners)).
- Providing event dispatch capabilities for those input events.

## Input events

Input events are generally fired on two different channels:

### Objects in focus

Events can be sent directly to a GameObject that has focus. For example, an object might
have a script that implements [`IMixedRealityTouchHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityTouchHandler).
This object would get touch events when focused by a hand that is near it. These types of
events go "up" the GameObject hierarchy until it finds a GameObject that is capable of handling
the event.

This is done by using [ExecuteHierarchy](https://docs.unity3d.com/ScriptReference/EventSystems.ExecuteEvents.ExecuteHierarchy.html) from within the default input system implementation.

### Global listeners

Events can be sent to global listeners. It's possible to register for all input events by using
the input system's [`IMixedRealityEventSystem`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityEventSystem)
interface. It's recommended to use the [RegisterHandler](xref:Microsoft.MixedReality.Toolkit.IMixedRealityEventSystem.RegisterHandler``1(IEventSystemHandler))
method for registering for global events - the deprecated `Register` function will cause listeners
to get notified of ALL input events, rather than just input events of a particular type
(where type is defined by the event interface).

Note that [fallback listeners](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystem.PushFallbackInputHandler(GameObject))
are another type of global listeners which are also discouraged because they will receive
every single input event that hasn't been handled elsewhere in the scene.

### Order of event dispatch

Generally, events are sent to listeners in the following way. Note that if any of the steps below mark
the event as [handled](https://docs.unity3d.com/ScriptReference/EventSystems.AbstractEventData-used.html),
the event dispatch process stops.

1. Event is sent to global listeners.
2. Event is sent to modal dialogs of the focused object.
3. Event is sent to the focused object.
4. Event is sent to fallback listeners.

## Device managers and data providers

These entities are responsible for interfacing with lower-level APIs (such as Windows Mixed Reality APIs,
or OpenVR APIs) and translating data from those systems into ones that fit the MRTK's higher
level input abstractions. They are responsible for detecting, creating, and managing the lifetime of
[controllers](ControllersPointersAndFocus.md#controllers).

The basic flow of a device manager involves:

1. The device manager is instantiated by the input system service.
2. The device manager registers with its underlying system (for example, the Windows Mixed Reality
   device manager will register for [input](../../Input/InputEvents.md) and [gesture](../../Input/Gestures.md#gesture-events) events.
3. It creates controllers that it discovers from the underlying system (for example
   the provider could detect the presence of articulated hands)
4. In its Update() loop, call UpdateController() to poll for the new state of the underlying system
   and update its controller representation.
