# Gestures

Gestures are input events based on human hands. There are two types of devices that raise gesture input events in MRTK:

- Windows Mixed Reality devices such as HoloLens. This describes pinching motions ("Air Tap") and tap-and-hold gestures.

  For more information on HoloLens gestures see the [Windows Mixed Reality Gestures documentation](https://docs.microsoft.com/windows/mixed-reality/gestures).

  [`WindowsMixedRealityDeviceManager`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityDeviceManager) wraps the [Unity XR.WSA.Input.GestureRecognizer](https://docs.unity3d.com/ScriptReference/XR.WSA.Input.GestureRecognizer.html) to consume Unity's gesture events from HoloLens devices.

- Touch screen devices.

  [`UnityTouchController`](xref:Microsoft.MixedReality.Toolkit.Input.UnityInput) wraps the [Unity Touch class](https://docs.unity3d.com/ScriptReference/Touch.html) that supports physical touch screens.

Both of these input sources use the _Gesture Settings_ profile to translate Unity's Touch and Gesture events respectively into MRTK's [Input Actions](InputActions.md). This profile can be found under the _Input System Settings_ profile.

<img src="../../Documentation/Images/Input/GestureProfile.png" style="max-width:100%;">

## Gesture events

Gesture events are received by implementing one of the gesture handler interfaces: [`IMixedRealityGestureHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler) or [`IMixedRealityGestureHandler<TYPE>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) (see table of [event handlers](InputEvents.md)).

See [Example Scene](#example-scene) for an example implementation of a gesture event handler.

When implementing the generic version, the *OnGestureCompleted* and *OnGestureUpdated* events can receive typed data of the following types:

- `Vector2` - 2D position gesture. Produced by touch screens to inform of their [`deltaPosition`](https://docs.unity3d.com/ScriptReference/Touch-deltaPosition.html).
- `Vector3` - 3D position gesture. Produced by HoloLens to inform of:
  - [`cumulativeDelta`](https://docs.unity3d.com/ScriptReference/XR.WSA.Input.ManipulationUpdatedEventArgs-cumulativeDelta.html) of a manipulation event
  - [`normalizedOffset`](https://docs.unity3d.com/ScriptReference/XR.WSA.Input.NavigationUpdatedEventArgs-normalizedOffset.html) of a navigation event
- `Quaternion` - 3D rotation gesture. Available to custom input sources but not currently produced by any of the existing ones.
- `MixedRealityPose` - Combined 3D position/rotation gesture. Available to custom input sources but not currently produced by any of the existing ones.

## Order of events

There are two principal chains of events, depending on user input:

- "Hold":
    1. Hold tap:
        * start _Manipulation_
    1. Hold tap beyond [HoldStartDuration](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile.HoldStartDuration):
        * start _Hold_
    1. Release tap:
        * complete _Hold_
        * complete _Manipulation_

- "Move":
    1. Hold tap:
        * start _Manipulation_
    1. Hold tap beyond [HoldStartDuration](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile.HoldStartDuration):
        * start _Hold_
    1. Move hand beyond [NavigationStartThreshold](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile.NavigationStartThreshold):
        * cancel _Hold_
        * start _Navigation_
    1. Release tap:
        * complete _Manipulation_
        * complete _Navigation_

## Example scene

The **HandInteractionGestureEventsExample** (Assets/MRTK/Examples/Demos/HandTracking/Scenes) scene shows how to use the pointer Result to spawn an object at the hit location.

The `GestureTester` (Assets/MRTK/Examples/Demos/HandTracking/Script) script is an example implementation to visualize gesture events via GameObjects. The handler functions change the color of indicator objects and display the last recorded event in text objects in the scene.
