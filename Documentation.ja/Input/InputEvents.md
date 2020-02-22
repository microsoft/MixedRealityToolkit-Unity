# Input events

The list below outlines all available input event interfaces to be implemented by a custom MonoBehaviour component. These interfaces will be called by the MRTK input system to handle custom app logic based on user input interactions. [Pointer input events](pointers.md#pointer-event-interfaces) are handled slightly differently than the standard input event types below.

> [!IMPORTANT]
> By default, a script will receive input events only if it is the GameObject in focus by a pointer or parent of a GameObject in focus.

| Handler | Events | Description |
| --- | :---: | --- |
| [`IMixedRealitySourceStateHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySourceStateHandler) | Source Detected / Lost | Raised when an input source is detected/lost, like when an articulated hand is detected or lost track of. |
| [`IMixedRealitySourcePoseHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySourcePoseHandler) | Source Pose Changed | Raised on source pose changes. The source pose represents the general pose of the input source. Specific poses, like the grip or pointer pose in a six DOF controller, can be obtained via `IMixedRealityInputHandler<MixedRealityPose>`. |
| [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler) | Input Down / Up | Raised on changes to binary inputs like buttons. |
| [`IMixedRealityInputHandler<T>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | Input Changed | Raised on changes to inputs of the given type. **T** can take the following values: <br/> - *float* (e.g returns analog trigger)<br/> - *Vector2* (e.g returns gamepad thumbstick direction) <br/> - *Vector3* (e.g return position of tracked device) <br/> - *Quaternion* (e.g returns orientation of tracked device)<br/> - [MixedRealityPose](xref:Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose) (e.g. returns fully tracked device) |
| [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) | Speech Keyword Recognized | Raised on recognition of one of the keywords configured in the *Speech Commands Profile*. |
| [`IMixedRealityDictationHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityDictationHandler) | Dictation<br/> Hypothesis <br/> Result <br/> Complete <br/> Error | Raised by dictation systems to report the results of a dictation session. |
| [`IMixedRealityGestureHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler) | Gesture events on: <br/> Started <br/> Updated <br/> Completed <br/> Canceled | Raised on gesture detection. |
| [`IMixedRealityGestureHandler<T>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) | Gesture Updated / Completed | Raised on detection of gestures containing additional data of the given type. See [**gesture events**](Gestures.md#gesture-events) for details on possible values for **T**. |
| [`IMixedRealityHandJointHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityHandJointHandler) | Hand Joints Updated | Raised by articulated hand controllers when hand joints are updated. |
| [`IMixedRealityHandMeshHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityHandMeshHandler) | Hand Mesh Updated | Raised by articulated hand controllers when a hand mesh is updated. |
| [`IMixedRealityInputActionHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputActionHandler) | Action Started / Ended | Raise to indicate action start and end for inputs mapped to actions. |

## Input events in action

At the script level, input events can be consumed by implementing one of the event handler interfaces shown in the table above. When an input event fires via a user interaction, the following takes place:

1. The MRTK input system recognizes that an input event has occurred.
1. The MRTK input system fires the relevant interface function of the input event to all [registered global input handlers](#register-for-global-input-events)
1. For every active pointer registered with the input system:
    1. The input system determines which GameObject is in focus for the current pointer.
    1. The input system utilizes [Unity's event system](https://docs.unity3d.com/Manual/EventSystem.html) to fire the relevant interface function for all matching components on the focused GameObject.
    1. If at any point an input event has been [marked as used](#how-to-stop-input-events), the process will end and no further GameObjects will receive callbacks.
        - Example: Components implementing the interface [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) will be searched for when a speech command is recognized.
        - Note: The Unity event system will bubble up to search the parent GameObject if no components matching the desired interface are found on the current GameObject.
1. If no global input handlers are registered and no GameObject is found with a matching component/interface, then the input system will call each fallback registered input handler

> [!NOTE]
> [Pointer input events](pointers.md#pointer-event-interfaces) are handled in a slightly different manner than the input event interfaces listed above. In particular, pointer input events are handled only by the GameObject in focus by the pointer that fired the input event - as well as any global input handlers. Regular input events are handled by GameObjects in focus for all active pointers.

### Input event interface example

The code below demonstrates use of the [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) interface. When the user says the words "smaller" or "bigger" while focusing on a GameObject with this `ShowHideSpeechHandler` class, the GameObject will scale itself by half or twice as much.

```c#
public class ShowHideSpeechHandler : MonoBehaviour, IMixedRealitySpeechHandler
{
    ...

    void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData.Command.Keyword == "smaller")
        {
            transform.localScale *= 0.5f;
        }
        else if (eventData.Command.Keyword == "bigger")
        {
            transform.localScale *= 2.0f;
        }
    }
}
```

> [!NOTE]
> [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) input events require that the desired keywords are pre-registered in the [MRTK Speech Commands Profile](Speech.md).

## Register for global input events

To create a component that listens for global input events, disregarding what GameObject may be in focus, a component must register itself with the Input System. Once registered, any instances of this MonoBehaviour will receive input events along with any GameObject(s) currently in focus and other global registered listeners.

If an input event has been [marked as used](#how-to-stop-input-events), global registered handlers will still all receive callbacks. However, no focused GameObjects will receive the event.

### Global input registration example

```c#
public class GlobalHandListenerExample : MonoBehaviour,
    IMixedRealitySourceStateHandler, // Handle source detected and lost
    IMixedRealityHandJointHandler // handle joint position updates for hands
{
    private void OnEnable()
    {
        // Instruct Input System that we would like to receive all input events of type
        // IMixedRealitySourceStateHandler and IMixedRealityHandJointHandler
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

    private void OnDisable()
    {
        // This component is being destroyed
        // Instruct the Input System to disregard us for input event handling
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }

    // IMixedRealitySourceStateHandler interface
    public void OnSourceDetected(SourceStateEventData eventData)
    {
        var hand = eventData.Controller as IMixedRealityHand;

        // Only react to articulated hand input sources
        if (hand != null)
        {
            Debug.Log("Source detected: " + hand.ControllerHandedness);
        }
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        var hand = eventData.Controller as IMixedRealityHand;

        // Only react to articulated hand input sources
        if (hand != null)
        {
            Debug.Log("Source lost: " + hand.ControllerHandedness);
        }
    }

    public void OnHandJointsUpdated(
                InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        MixedRealityPose palmPose;
        if (eventData.InputData.TryGetValue(TrackedHandJoint.Palm, out palmPose))
        {
            Debug.Log("Hand Joint Palm Updated: " + palmPose.Position);
        }
    }
}
```

## Register for fallback input events

Fallback input handlers are similar to registered global input handlers but are treated as a last resort for input event handling. Only if no global input handlers were found and no GameObjects are in focus will fallback input handlers be leveraged.

### Fallback input handler example

```c#
public class GlobalHandListenerExample : MonoBehaviour,
    IMixedRealitySourceStateHandler // Handle source detected and lost
{
    private void OnEnable()
    {
        CoreServices.InputSystem?.PushFallbackInputHandler(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.PopFallbackInputHandler();
    }

    // IMixedRealitySourceStateHandler interface
    public void OnSourceDetected(SourceStateEventData eventData)
    {
        ...
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        ...
    }
}
```

## How to stop input events

Every input event interface provides a [`BaseInputEventData`](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputEventData) data object as a parameter to each function on the interface. This event data object extends from Unity's own [`AbstractEventData`](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.AbstractEventData.html).

In order to stop an input event from propagating through its execution [as outlined](#input-events-in-action), a component can call [`AbstractEventData.Use()`](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.AbstractEventData.Use.html) to mark the event as used. This will stop any other GameObjects from receiving the current input event, with the exception of global input handlers.

> [!NOTE]
> A component that calls the `Use()` method will stop other GameObjects from receiving it. However, other components on the current GameObject will still receive the input event and fire any related interface functions.

## See also

- [Pointers](Pointers.md)
- [Speech](Speech.md)
- [Input State](InputState.md)
