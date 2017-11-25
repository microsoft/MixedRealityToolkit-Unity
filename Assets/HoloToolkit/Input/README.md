## [Input]()

## Table of Contents

- [Overview](#overview)
- [Input System Diagrams](#input-system-diagrams)
- [Input Module Design](#input-module-design)
- [Prefabs](#prefabs)
- [Scripts](#scripts)
- [Test Prefabs](#test-prefabs)
- [Test Scripts](#test-scripts)
- [Tests](#tests)

## Overview
This contains a fully-featured input system, which allows you to handle various types of input and send them to any `GameObject` being currently gazed at, or any fallback object. It also includes a few example cursors that fully leverages the Unity's animation system.
This input system uses and extends Unity's default `EventSystem` so there's no need for a custom input module.
This input system is cross platform and can be easily extended with custom input sources, events, and input handlers.
This input system works with Unity’s native uGUI.
Input Module Design
Each input source (Motion Controllers, Hands, Gestures, Mouse, Keyboard, etc.) implement a `IInputSource` interface. The interface defines various events that the input sources can trigger. The input sources register themselves with the `InputManager`, whose role it is to forward input to the appropriate game objects. Input sources can be dynamically enabled / disabled as necessary, and new input sources can be created to support different input devices.
The `InputManager` listens to the various events coming from the input sources, and also takes into account the current gaze and pointers.
By default, input events are sent to the currently focused game object, if that object implements the appropriate interface.  Any event sent by the `InputManager` always bubbles up from the focused GameObject, to each of its ancestors.
If you wish to be able to send input events to `GameObject`s that do not require gaze (such as voice commands), you will need to register the handler as a Global Listener.
Modals input handlers can also be added to the `InputManager`. These modal handlers will take priority over the currently focused object. Fallback handlers can also be defined, so that the application can react to global inputs that aren't targeting a specific element.
The `InputManager` forwards the various input sources events to the appropriate game object, using the following order:
Registered Global Listeners
The registered modal input handlers, in LIFO (Last-In First-Out) order of registration
The currently focused `GameObject`
The fallback input handlers, in LIFO (Last-In First-Out) order of registration
`GameObject`s that want to consume input events can implement one or more input interfaces.  These interface handlers follow a specific hierarchy:

### Generic Cross Platform Input Handlers

- `ISourceStateHandler` for all source detected/source lost events.
- `IFocusable` for focus enter and exit. The focus can be triggered by the user's gaze or any other gaze source.
- `IInputHandler` for source up, down, and pressed events.
    - Similar to [Unity’s `Input` class](https://docs.unity3d.com/ScriptReference/Input.html) `GetButtonDown`, `GetButtonUp`, and `GetButton` methods.
    - Only Pointers will raise UI events for Source Up and Source Down.
- `IInputClickHandler` for source “clicked”.  The Default Pointer Sources that raise the click handler include:
    - Mouse Clicks
    - When a user says “Select”
    - Hand Taps
    - Clicker Presses
    - Motion Controller Trigger Presses
- `IXboxControllerHandler` for Cross Platform Xbox One Controller events.

### Windows Specific Input Handlers

- `ISpeechHandler` for voice commands.
- `IDictationHandler` for speech to text dictation.
### UWP Specific Input Handlers
- `IHoldHandler` for the Windows hold gesture.
- `IManipulationHandler` for the Windows manipulation gesture.
- `INavigationHandler` for the Windows navigation gesture.
Motion Controller Specific Interfaces
- `IControllerInputHandler` for input position events from the thumbstick and TouchPad.
- `ISelectHandler` for selection pressed amount changes.
- `IControllerTouchpadHandler` for TouchPad press and touch events.
- `ISourcePositionHandler` to get pointer and grip position events.
- `ISourceRotationHandler` to get pointer and grip rotation events.

## Getting Started

First, it’s important to read and review Microsoft’s developer guidelines for [Gaze](https://developer.microsoft.com/en-us/windows/mixed-reality/gaze), [Gestures](https://developer.microsoft.com/en-us/windows/mixed-reality/gestures), [Voice](https://developer.microsoft.com/en-us/windows/mixed-reality/voice_input), and [Motion Controllers](https://developer.microsoft.com/en-us/windows/mixed-reality/motion_controllers).  When setting up input for your own projects you’ll need to keep a few things in mind.

- The InputManager.prefab is required
- A cursor that inherits from the ICursor interface is required.
- The `GameObect`(s) you want to interact with are required to have a Collider component.
- When using uGUI, all world space canvases are required to use the UIRaycastCamera as their event camera.
- When consuming events in your input handlers, it’s highly suggested you call `eventData.Use();` so the event doesn’t fall through to other input handlers.

### Detecting Input Sources

1. Create a new scene
2. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
3. Create an empty GameObject
4. Rename the GameObject to InputSourceHandler
5. Attach a `SetGlobalListener` component to the InputSourceHander GameObject
6. Create a new script named `InputSourceHandler`
7. Attach the `InputSourceHandler` to your InputSourceHander GameObject
8. Open the new script in any text editor
9. Implement the `ISourceStateHandler` interface
10. Add logic for handling source detection and loss

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class InputSourceDetectionHandler : MonoBehaviour, ISourceStateHandler
{
    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.LogFormat("OnSourceDetected\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        Debug.LogFormat("OnSourceLost\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }
}
```

### Getting Focus

1. Create a new scene
2. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
3. Create a Cube
4. Create a new script named `HighlightHandler`
5. Attach the `HighlightHandler` to your Cube
6. Open the new script in any text editor
7. Implement the `IFocusable` interface

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class HighlightHandler : MonoBehaviour, IFocusable
{
    void IFocusable.OnFocusEnter()
    {
    }

    void IFocusable.OnFocusExit()
    {
    }

}
```

8. Add Color, Renderer, and Material fields

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

[RequireComponent(typeof(Renderer))]
public class HighlightHandler : MonoBehaviour, IFocusable
{
    [SerializeField]
    private Color highlightColor;

    private Color initialColor;
    private Renderer rendererInstance;
    private Material materialInstance;

    void IFocusable.OnFocusEnter()
    {
    }

    void IFocusable.OnFocusExit()
    {
    }
}
```

9. Cache initial Color, Renderer, and Material in Awake
10. Properly destroy the Material instance in OnDestroy

```
    private void Awake()
    {
        rendererInstance = GetComponent<Renderer>();
        materialInstance = rendererInstance.material;
        initialColor = materialInstance.color;
    }

    private void OnDestroy()
    {
        Destroy(materialInstance);
    }
```

11. Handle color changes in implemented methods

```
    void IFocusable.OnFocusEnter()
    {
        materialInstance.color = highlightColor;
    }

    void IFocusable.OnFocusExit()
    {
        materialInstance.color = initialColor;
    }
```

### Getting Clicks and Generic Input

1. Create a new scene
2. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
3. Create a Cube
4. Create a new script named `InputHandler`
5. Attach the `InputHandler` to your Cube
6. Open the new script in any text editor
7. Implement the `IInputClickHandler` and `IInputHandler` interfaces
8. Add logic for when Input Click events and Input Up/Down events

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class InputHandler : MonoBehaviour, IInputClickHandler, IInputHandler
{
    void IInputHandler.OnInputUp(InputEventData eventData)
    {
        Debug.LogFormat("OnInputUp\r\nSource: {0}  SourceId: {1}  InteractionPressKind: {2}", eventData.InputSource, eventData.SourceId, eventData.PressType);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IInputHandler.OnInputDown(InputEventData eventData)
    {
        Debug.LogFormat("OnInputDown\r\nSource: {0}  SourceId: {1}  InteractionPressKind: {2}", eventData.InputSource, eventData.SourceId, eventData.PressType);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        Debug.LogFormat("OnInputClicked\r\nSource: {0}  SourceId: {1}  InteractionPressKind: {2}  TapCount: {3}", eventData.InputSource, eventData.SourceId, eventData.PressType, eventData.TapCount);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }
}
```

### Xbox Controller Input

1. Create a new scene
2. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
3. Create an empty GameObject
4. Rename the GameObject to XboxControllerSource
5. Create a new script named `XboxControllerHandler`
6. Attach the `XboxControllerHandler` to your XboxControllerSource GameObject
7. Open the new script in any text editor
8. Your new `XboxControllerHandler` class should inherit from `XboxControllerHandlerBase`
9. Add logic for handling source detection/loss and `XboxInputUpdate`

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class XboxInputHandler : XboxControllerHandlerBase
{
    public override void OnSourceDetected(SourceStateEventData eventData)
    {
        base.OnSourceDetected(eventData);
        Debug.LogFormat("Joystick {0} with id: \"{1}\" Connected", GamePadName, eventData.SourceId);
    }

    public override void OnSourceLost(SourceStateEventData eventData)
    {
        Debug.LogFormat("Joystick {0} with id: \"{1}\" Disconnected", GamePadName, eventData.SourceId);
        base.OnSourceLost(eventData);
    }

    public override void OnXboxInputUpdate(XboxControllerEventData eventData)
    {
        // FYI, eventData.Use() is called in the base implementation.
        base.OnXboxInputUpdate(eventData);

        Debug.LogFormat(
            "{19}\n" +
            "LS Horizontal: {0:0.000} Vertical: {1:0.000}\n" +
            "RS Horizontal: {2:0.000} Vertical: {3:0.000}\n" +
            "DP Horizontal: {4:0.000} Vertical: {5:0.000}\n" +
            "Left Trigger:  {6:0.000} Right Trigger: {7:0.000} Shared Trigger: {8:0.00}\n" +
            "A: {9} B: {10} X: {11} Y: {12}\n" +
            "LB: {13} RB: {14} " +
            "LS: {15} RS: {16}\n" +
            "View: {17} Menu: {18}\n",
            eventData.XboxLeftStickHorizontalAxis, eventData.XboxLeftStickVerticalAxis,
            eventData.XboxRightStickHorizontalAxis, eventData.XboxRightStickVerticalAxis,
            eventData.XboxDpadHorizontalAxis, eventData.XboxDpadVerticalAxis,
            eventData.XboxLeftTriggerAxis, eventData.XboxRightTriggerAxis, eventData.XboxSharedTriggerAxis,
            eventData.XboxA_Pressed, eventData.XboxB_Pressed, eventData.XboxX_Pressed, eventData.XboxY_Pressed,
            eventData.XboxLeftBumper_Pressed, eventData.XboxRightBumper_Pressed,
            eventData.XboxLeftStick_Pressed, eventData.XboxRightStick_Pressed,
            eventData.XboxView_Pressed, eventData.XboxMenu_Pressed,
            GamePadName);
    }
}
```

### Windows Platform Specific Input

1. Voice Input
2. Create a new scene
3. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
4. Create a Cube
5. Create a new script named `SpeechHandler`
6. Attach the `SpeechHandler` to your Cube
7. Open the new script in any text editor
8. Implement the `ISpeechHandler` interface
9. Add switch statement for `eventData.RecognizedText` with a case for each command you wish to use.
    - _Note: It’s helpful to use `ToLower()` to get better matches._

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class SpeechHandler : MonoBehaviour, ISpeechHandler
{
    void ISpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        switch (eventData.RecognizedText.ToLower())
        {
            case "your voice command":
                DoAction();
                break;
        }
    }

    public void DoAction()
    {
        // TODO: Action
    }
}
```
### Dictation Input

1. Create a new scene
2. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
3. Create an empty GameObject
4. Rename the GameObject to DictationHandler
5. Create a new script named `DictationHandler`
6. Attach the new `DictationHandler` script to your DictationHandler GameObject
7. Open the new script in any text editor
8. Implement the `IInputClickHandler` and `IDictationHandler` interfaces
9. Add fields for the initial silence timeout, auto silence timeout, and total allowable recording time.
10. Add fields for the text output
11. Add a flag for recording

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class DictationHandler : MonoBehaviour, IInputClickHandler, IDictationHandler
{
    [SerializeField]
    [Range(0.1f, 5f)]
    [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.")]
    private float initialSilenceTimeout = 5f;

    [SerializeField]
    [Range(5f, 60f)]
    [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input.")]
    private float autoSilenceTimeout = 20f;

    [SerializeField]
    [Range(1, 60)]
    [Tooltip("Length in seconds for the manager to listen.")]
    private int recordingTime = 10;

    private string lastOutput;
    private string speechToTextOutput = string.Empty;
    public string SpeechToTextOutput { get { return speechToTextOutput; } }

    private bool isRecording;
}
```

12. Add logic for handling the recording toggle when DictationHander GameObject is clicked

```

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ToggleRecording();
    }

    private void ToggleRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            StartCoroutine(DictationInputManager.StopRecording());
        }
        else
        {
            isRecording = true;
            StartCoroutine(DictationInputManager.StartRecording(initialSilenceTimeout, autoSilenceTimeout, recordingTime));
        }
    }
```

13. Add logic for handling dictation results

```
    void IDictationHandler.OnDictationHypothesis(DictationEventData eventData)
    {
        speechToTextOutput = eventData.DictationResult;
    }

    void IDictationHandler.OnDictationResult(DictationEventData eventData)
    {
        speechToTextOutput = eventData.DictationResult;
    }

    void IDictationHandler.OnDictationComplete(DictationEventData eventData)
    {
        speechToTextOutput = eventData.DictationResult;
    }

    void IDictationHandler.OnDictationError(DictationEventData eventData)
    {
        isRecording = false;
        speechToTextOutput = eventData.DictationResult;
        Debug.LogError(eventData.DictationResult);
        StartCoroutine(DictationInputManager.StopRecording());
    }
```

14. Add logic for displaying the results

```
    private void Update()
    {
        if (!string.IsNullOrEmpty(speechToTextOutput) && !lastOutput.Equals(speechToTextOutput))
        {
            Debug.Log(speechToTextOutput);
            lastOutput = speechToTextOutput;
        }
    }
```

### UWP Specific Input
1. Create a new scene
2. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
3. Create a Cube
4. Create a new script named `InputHandler`
5. Attach the `InputHandler` to your Cube
6. Open the new script in any text editor
7. Implement the `IHoldHandler`,`IManipulationHandler`, and `INavigationHandler` interfaces
8. Add logic for each event

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class InputHandler : MonoBehaviour, IHoldHandler, IManipulationHandler, INavigationHandler
{
    void IHoldHandler.OnHoldStarted(HoldEventData eventData)
    {
        Debug.LogFormat("OnHoldStarted\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IHoldHandler.OnHoldCompleted(HoldEventData eventData)
    {
        Debug.LogFormat("OnHoldCompleted\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IHoldHandler.OnHoldCanceled(HoldEventData eventData)
    {
        Debug.LogFormat("OnHoldCanceled\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulationStarted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulationCompleted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulationCanceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
    {
        Debug.LogFormat("OnNavigationStarted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.NormalizedOffset.x,
            eventData.NormalizedOffset.y,
            eventData.NormalizedOffset.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void INavigationHandler.OnNavigationUpdated(NavigationEventData eventData)
    {
        Debug.LogFormat("OnNavigationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.NormalizedOffset.x,
            eventData.NormalizedOffset.y,
            eventData.NormalizedOffset.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
    {
        Debug.LogFormat("OnNavigationCompleted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.NormalizedOffset.x,
            eventData.NormalizedOffset.y,
            eventData.NormalizedOffset.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
    {
        Debug.LogFormat("OnNavigationCanceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.NormalizedOffset.x,
            eventData.NormalizedOffset.y,
            eventData.NormalizedOffset.z);

        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }
}
```

### Motion Controllers Specific Input

1. Create a new scene
2. Run the MRTK scene wizard via `MixedRealityToolkit/Configure/Apply Scene Settings`
3. Create an empty GameObject
4. Rename the GameObject to MotionControllerHandler
5. Create a new script named `MotionControllerHandler`
6. Attach the new `MotionControllerHandler` script to your MotionControllerHandler GameObject
7. Open the new script in any text editor
8. Implement the `ISelectHandler`, `IControllerTouchpadHandler`, `ISourcePositionHandler`, and `ISourceRotationHandler` interfaces
9. Add logic for each event

```
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class MotionControllerHandler : MonoBehaviour, ISelectHandler, IControllerTouchpadHandler, ISourcePositionHandler, ISourceRotationHandler
{
    void IControllerInputHandler.OnInputPositionChanged(InputPositionEventData eventData)
    {
        Debug.LogFormat("OnRotationChanged\r\nSource: {0}  SourceId: {1}  Input Position: {2}",
            eventData.InputSource, eventData.SourceId, eventData.Position);
    }

    void ISelectHandler.OnSelectPressedAmountChanged(SelectPressedEventData eventData)
    {
        Debug.LogFormat("OnRotationChanged\r\nSource: {0}  SourceId: {1}  Select Press Amount: {2}",
            eventData.InputSource, eventData.SourceId, eventData.PressedAmount);
    }

    void IControllerTouchpadHandler.OnTouchpadTouched(InputEventData eventData)
    {
        Debug.LogFormat("OnRotationChanged\r\nSource: {0}  SourceId: {1}  InteractionPressKind: {2}",
            eventData.InputSource, eventData.SourceId, eventData.PressType);
    }

    void IControllerTouchpadHandler.OnTouchpadReleased(InputEventData eventData)
    {
        Debug.LogFormat("OnRotationChanged\r\nSource: {0}  SourceId: {1}  InteractionPressKind: {2}",
            eventData.InputSource, eventData.SourceId, eventData.PressType);
    }

    void ISourcePositionHandler.OnPositionChanged(SourcePositionEventData eventData)
    {
        Debug.LogFormat("OnRotationChanged\r\nSource: {0}  SourceId: {1}  Pointer Rotation: {2}  Grip Rotation {3}",
            eventData.InputSource, eventData.SourceId, eventData.PointerPosition, eventData.GripPosition);
    }

    void ISourceRotationHandler.OnRotationChanged(SourceRotationEventData eventData)
    {
        Debug.LogFormat("OnRotationChanged\r\nSource: {0}  SourceId: {1}  Pointer Rotation: {2}  Grip Rotation {3}",
            eventData.InputSource, eventData.SourceId, eventData.PointerRotation, eventData.GripRotation);
    }
}
```

### [Prefabs](Prefabs)
Prefabs related to the input features.

#### BasicCursor.prefab
Torus shaped basic cursor that follows the user's gaze around.

#### Cursor.prefab
Torus shaped CursorOnHolograms when user is gazing at holograms and point light CursorOffHolograms when user is gazing away from holograms.

#### CursorWithFeedback.prefab
Torus shaped cursor that follows the user's gaze and HandDetectedFeedback asset to give feedback to user when their hand is detected in the ready state.

#### DefaultCursor.prefab
3D animated cursor that follows the user's gaze and uses the Unity animation system to handle its various states. This cursor imitates the HoloLens Shell cursor.

#### HoloLensCamera.prefab
Unity camera that has been customized for Holographic development.
1. Camera.Transform set to 0,0,0
2. 'Clear Flags' changed to 'Solid Color'
3. Color set to R:0, G:0, B:0, A:0 as black renders transparent in HoloLens.
4. Set the recommended near clipping plane.
5. Allows manual movement of the camera when in editor

#### InputManager.prefab
Input system that manages gaze and various input sources currently supported by HoloLens, such as hands and gestures.

This also includes a fake input source that allows you to simulate input when in the editor. By default, this can be done by holding Shift (left input source) or Space (right input source), moving the mouse to move the source and using the left mouse button to tap.

#### MixedRealityCamera.prefab
Camera capabale of rendering for HoloLens and occluded Windows Mixed Reality enabled devices.
MixedRealityCameraManager.cs exposes some defaults for occluded aka opaque displays Vs HoloLens.
You can either use the defaults that have been set or customize them to match your application requirements.

**For HoloLens:**
1. 'Clear Flags' is set to 'Solid Color' and Color to 'Clear' as black renders transparent in HoloLens.
2. Near clip plane is set to 0.85 per comfort recommendations.
3. Quality Settings to be Fastest.

**For occluded aka opaque devices:**
1. 'Clear Flags' is set to Skybox
2. Near clip plane is set to 0.3 which is typical for VR applications.
3. Quality Settings to be Fantastic as it uses the PC GPU to render content.

#### MixedRealityCameraParent.prefab
This prefab is used when you want to enable teleporting on mixed reality enabled occluded devices.
In order to prevent the MainCamera position from being overwritten in the next update we use a parent GameObject.

#### MixedRealityCameraParentWithControllers.prefab
This prefab is used when you want to enable teleporting on mixed reality enabled occluded devices, as well as motion controller visualization.
In order to prevent the MainCamera position from being overwritten in the next update we use a parent GameObject.

### [Scripts](Scripts)
Scripts related to the input features.

##### ControllerVisualizer.cs
Use this to visualize a 6DoF controller in your application. Add this script to a GameObject as a child of the MainCamera, or use the MixedRealityCameraParentWithControllers prefab. Either specify a shader to use for the [glTF](https://www.khronos.org/gltf) model or add GameObject overrides to represent the controllers.

- **leftControllerOverride** [Optional] A prefab to spawn to represent the left controller. This will automatically move and reorient when the controller is moved.
- **rightControllerOverride** [Optional] A prefab to spawn to represent the right controller. This will automatically move and reorient when the controller is moved.
- **touchpadTouchedOverride** [Optional] A prefab to spawn to represent the user's touch location on the touchpad. This will automatically move when the user moves their touch location. Default is a sphere.
- **GLTFShader** [Optional, if using overrides] If using the controller's built-in [glTF](https://www.khronos.org/gltf) model, this will be the shader applied to the resulting GameObject.

##### ControllerDebug.cs
This can be used to load a [glTF](https://www.khronos.org/gltf) file in the editor, as well as displaying input and source data from motion controllers on a text panel for debugging purposes on the device.
- **LoadGLTFFile** A boolean to specify if a glTF file should be loaded from StreamingAssets.
- **GLTFName** The name of the GLTF file to be loaded from StreamingAssets.
- **touchpadTouchedOverride** [Optional] A prefab to spawn on the touchpad of the glTF file specified. Default is a sphere.
- **GLTFShader** [Optional, if not loading a glTF model] If loading a [glTF](https://www.khronos.org/gltf) model, this will be the shader applied to the resulting GameObject.
- **TextPanel** This is the text display where controller state info will be logged, for on-device debugging purposes.

##### SetGlobalListener.cs
Add this to a GameObject to register it as a global listener on the InputManager. This means it will receive events from the InputManager even while not focused.

#### Cursor

##### AnimatedCursor.cs
Animated cursor is a cursor driven using an animator to inject state information and animate accordingly.

##### Cursor.cs
Abstract class that a concrete class should implement to make it easy to create a custom cursor. This provides the basic logic to show a cursor at the location a user is gazing.
1. Decides when to show the cursor.
2. Positions the cursor at the gazed hit location.
3. Rotates the cursor to match hologram normals.

##### CursorModifier.cs
CursorModifier is a component that can be added to any game object with a collider to modify how a cursor reacts when on that collider.

##### ICursor.cs
Cursor interface that any cursor must implement.

##### ICursorModifier.cs
Interface that any cursor modifier must implement to provide basic overrides for cursor behaviour.

##### MeshCursor.cs
Cursor whose states are represented by one or many meshes.

##### ObjectCursor.cs
Cursor whose states are represented by one or many game objects.

##### SpriteCursor.cs
Cursor whose states are represented by colored sprites.

#### Focus
With Windows Mixed Reality enabled devices and motion controllers, you can have different ways of representing the user's attention aka pointing.
You can use the conventional **'gaze and commit'** style interactions.
In these your cursor follows your head gaze.
With pointing ray enabled motion controllers you can perform the **'point and commit'** style interactions.
In these cases, your cursor will follow the pointing ray from the motion controller. 
Focus classes are meant to act as a bridge between these mechanisms. 

**Currently, we recommend using gaze and commit style interactions.**

##### FocusDetails.cs
FocusDetails struct contains information about which game object has the focus currently. Much like RaycastHit.
Also contains information about the normal of that point.

##### FocusManager.cs
Focus manager is the bridge that handles different types of pointing sources like gaze cursor or pointing ray enabled motion controllers.
If you dont have pointing ray enabled controllers, it defaults to GazeManager.

##### InputSourcePointer.cs
Class implementing IPointingSource to demonstrate how to create a pointing source.
This is consumed by SimpleSinglePointerSelector.

##### IPointingSource.cs
Implement this interface to register your pointer as a pointing source. This could be gaze based or motion controller based.

##### RegisterPointableCanvas.cs
Script to register a Canvas component so it's capable of being focused at for 'point and commit' scenarios.

##### SimpleSinglePointerSelector
Script shows how to create your own 'point and commit' style pointer which can steal cursor focus using a pointing ray supported motion controller.
This class uses the InputSourcePointer to define the rules of stealing focus when a pointing ray is detected with a motion controller that supports pointing.

#### GamePad

##### XboxControllerData.cs
Data class that carries the input data for the event handler.

##### XboxControllerMapping.cs
Defines the controller mapping for the input source.

##### XboxControllerMappingTypes.cs
Controller axis and button types.

#### Gaze

##### BaseRayStabilizer.cs
A base abstract class for a stabilizer that takes as input position and rotation, and performs operations on them to stabilize or smooth that data.

##### GazeManager.cs
Singleton component in charge of managing the gaze vector. This is where you can define which layers are considered when gazing at objects. Optionally, the gaze manager can reference a ray stabilizer that will be used to stabilize the gaze of the user.

- **MaxGazeCollisionDistance :** the maximum distance to raycast. Any holograms beyond this value will not be raycasted to.
- **RaycastLayers :** the Unity layers to raycast against. If you have holograms that should not be raycasted against, like a cursor, do not include their layers in this mask.
- **Stabilizer :** stabilizer to use to stabilize the gaze. If not set, the gaze will not be stabilized.
- **Gaze Transform :** the transform to use as the source of the gaze. If not set, will default to the main camera.

##### GazeStabilizer.cs
Stabilize the user's gaze to account for head jitter.

- **StoredStabilitySamples** Number of samples that you want to iterate on.  A larger number will be more stable.

##### MixedRealityTeleport.cs
This script teleports the user to the location being gazed at when Y was pressed on a Gamepad.
You must have an Xbox gamepad attached to use this script. It also works in the Unity editor.

#### InputEventData

##### BaseInputEventData.cs
Base class for all input event data. An input event data is what is sent as a parameter to all input events.

##### DictationEventData.cs
Event data for an event coming from dictation.

##### GamePadEventData.cs
Event data for an event coming from a generic gamepad.

##### HoldEventData.cs
Event data for an event coming from the hold gesture.

##### InputClickedEventData.cs
Event data for an event that represents a click, including the number of taps.

##### InputEventData.cs
Event data for an event that represents an input interaction such as a tap / click.

##### InputXYEventData.cs
Event data for an event that represents changes in the X-axis and Y-axis for a thumbstick or touchpad.

##### ManipulationEventData.cs
Event data for an event coming from the manipulation gesture.

##### NavigationEventData.cs
Event data for an event coming from the navigation gesture.

##### PointerSpecificEventData.cs
Event data for an event that represents a pointer entering or exiting focus on an object.

##### SourceRotationEventData.cs
Event data for an event that represents a change in rotation of an input source.

##### SourcePositionEventData.cs
Event data for an event that represents a change in position of an input source.

##### SourceStateEventData.cs
Event data for an event that represents an input source being detected or lost.

##### SpeechKeywordRecognizedEventData.cs
Event data for an event that represents a recognition from a PhraseRecognizer.

##### TriggerEventData.cs
Event data for an event that represents the amount of depression of a trigger.

##### XboxControllerEventData.cs
Event data for an event coming from an Xbox controller source.

#### InputHandlers

##### IControllerInputHandler.cs
Interface that a game object can implement to react to a controller's trigger value changing or touchpad/thumbstick XY changing.

##### IControllerTouchpadHandler.cs
Interface that a game object can implement to react to a controller's touchpad being touched or released.

##### IDictationHandler.cs
Interface that a game object can implement to react to dictations.

##### IFocusable.cs
Interface that a game object can implement to react to focus enter/exit.

#### IGamePadHandler.cs
Interface that a game object can implement to react to gamepad events.

##### IHoldHandler.cs
Interface that a game object can implement to react to hold gestures.

##### IInputClickHandler.cs
Interface that a game object can implement to react to taps / clicks.

##### IInputHandler.cs
Interface that a game object can implement to react to an input button being pressed or released.

##### IManipulationHandler.cs
Interface that a game object can implement to react to manipulation gestures.

##### INavigationHandler.cs
Interface that a game object can implement to react to navigation gestures.

##### IPointerSpecificFocusable.cs
Interface that a game object can implement to react to a specific pointer's focus enter/exit.


##### ISourcePositionHandler.cs
Interface that a game object can implement to react to a source's position changing.

##### ISourceRotationHandler.cs
Interface that a game object can implement to react to a source's rotation changing.


##### ISourceStateHandler.cs
Interface that a game object can implement to react to source state changes, such as when an input source is detected or lost.

##### ISpeechHandler.cs
Interface that a game object can implement to react to a keyword being recognized.

##### IXboxControllerHandler.cs
Interface that a game object can implement to react to Xbox Controller events.

#### InputSources

##### BaseInputSource.cs
Abstract base class for an input source that implements IInputSource. Defines the various abstract functions that any input source needs to implement, and provides some default implementations.

##### DictationInputManager.cs
Singleton class that implements  the DictationRecognizer to convert the user's speech to text. The DictationRecognizer exposes dictation functionality and supports registering and listening for hypothesis and phrase completed events.

**IMPORTANT**: Please make sure to add the Microphone capabilities in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

- **InitialSilenceTimeout** :  The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.
- **AutoSilenceTimeout** : The time length in seconds before dictation recognizer session ends due to lack of audio input.
- **RecordingTime** : Length in seconds for the manager to listen.

##### EditorInputSource.cs
Input source for in-Editor input source information, which can be used to simulate hands and controllers in the Unity editor.

##### GamePadInputSource.cs
Base class that all gamepad input sources should inherit from.

##### GesturesInput.cs
Input source for gestures information from the WSA APIs, which gives access to various system supported gestures.

##### IInputSource.cs
Interface for an input source. An input source is any input mechanism that can be used as the source of user interactions.

##### SpeechInputSource.cs
Allows you to specify keywords and keyboard shortcuts in the Unity Inspector, instead of registering them explicitly in code. Keywords are handled by scripts that implement ISpeechHandler.cs.  You can utilize keywords with the SpeechInputHandler component by assigning game objects and specifying a Unity Event trigger.

**IMPORTANT**: Please make sure to add the Microphone capabilities in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

**Persistent Keywords** Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.

**RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

**KeywordsAndKeys** Set the size as the number of keywords you'd like to listen for, then specify the keywords to complete the array.

**RecognitionConfidenceLevel** The confidence level for the keyword recognizer.

##### SupportedInputInfo.cs
Enumeration of the supported input infor for Unity WSA APIs.

##### XboxControllerInputSource.cs
Allows you to specity **EventSystem** axis and button overrides and custom controller mappings.

**Horizontal Axis** Sets the horizontal axis override.
**Vertical Axis** Sets the vertical axis override.
**Submit Button** Sets the submit button override.
**Cancel Button** Sets the cancel button override.

**Use Custom Mapping** Enables custom mapping for your controller.  Strings should match the input mapping from `Edit/Project Settings/Input`.

#### Interactions

##### HandDraggable.cs
Allows dragging an object in space with your hand on HoloLens. Just attach the script to a game object to make it movable.

#### Utilities

##### HandDraggable.cs
Allows dragging an object in space with your hand on HoloLens. Just attach the script to a game object to make it movable.

##### TapToPlace.cs
Allows users to tap on an object to move its position.

##### SetGlobalListener.cs
Used to register the GameObject on the InputManager as a global listener.

##### TriggerButton.cs
Very simple class that implements basic logic for a trigger button.

#### Voice

**IMPORTANT**: Please make sure to add the Microphone capabilities in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

##### KeywordAndKeyCode.cs
Struct that facilitates the storage of keyword and keycode pairs.

- **_KeywordsAndResponses_** Set the size as the number of keywords you'd like to listen for, then specify the keywords and method responses to complete the array.

- **RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

##### SpeechInputHandler.cs
Used to assign a Unity Event to a keyword stored in the SpeechInputSource component.

##### TriggerButton.cs
Very simple class that implements basic logic for a trigger button.

### [Test Prefabs](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit-Tests/Input/Prefabs)

Prefabs used in the various test scenes, which you can use as inspiration to build your own.

#### FocusedObjectKeywordManager.prefab
Keyword manager pre-wired to send messages to object being currently focused via FocusedObjectMessageSender component.
You can simply drop this into your scene and be able to send arbitrary messages to currently focused object.

#### SelectedObjectKeywordManager.prefab
Keyword manager pre-wired to send messages to object being currently selected via SelectedObjectMessageSender component.
You can simply drop this into your scene and be able to send arbitrary messages to currently selected object.

### [Test Scripts](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit-Tests/Input/Scripts)

#### FocusedObjectMessageSender.cs
Sends Unity message to currently focused object.
FocusedObjectMessageSender.SendMessageToFocusedObject needs to be registered as a response in KeywordManager
to enable arbitrary messages to be sent to currently focused object.

#### MicStream.cs
Lets you access beam-formed microphone streams from the HoloLens to optimize voice and/or room captures, which is impossible to do with Unity's Microphone object. Takes the data and inserts it into Unity's AudioSource object for easy handling. Also lets you record indeterminate-length audio files from the Microphone to your device's Music Library, also using beam-forming.

#### SelectedObjectMessageSender.cs
Sends Unity message to currently selected object.
SelectedObjectMessageSender.SendMessageToSelectedObject needs to be registered as a response in KeywordManager
to enable arbitrary messages to be sent to currently selected object.

#### SelectedObjectMessageReceiver.cs
Example on how to handle messages send by SelectedObjectMessageSender.
In this particular implementation, selected object color it toggled on selecting object and clearing selected object.

#### SimpleGridGenerator.cs
A grid of dynamic objects to illustrate sending messages to prefab instances created at runtime as opposed
to only static objects that already exist in the scene.

#### GazeResponder.cs
This class implements IFocusable to respond to gaze changes.
It highlights the object being gazed at.

#### TapResponder.cs
This class implements IInputClickHandler to handle the tap gesture.
It increases the scale of the object when tapped.

#### NavigationRotateResponder.cs
This class implements INavigationHandler to handle the navigation gesture.
It rotates the object left or right based on X movement.

### [Tests](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit-Tests/Input/Scenes)
Tests related to the input features. To use the scene:

1. Navigate to the Tests folder.
2. Double click on the test scene you wish to explore.
3. Either click "Play" in the unity editor or File -> Build Settings.
4. Add Open Scenes, Platform -> Windows Store, SDK -> Universal 10, Build Type -> D3D, Check 'Unity C# Projects'.
5. Click 'Build' and create an App folder. When compile is done, open the solution and deploy to device.

#### BasicCursor.unity 
Shows the basic cursor following the user's gaze and hugging the test sphere in the scene.

#### Cursor.unity 
Shows the cursor on holograms hugging the test sphere in the scene and cursor off holograms when not gazing at the sphere.

#### CursorWithFeedback.unity 
Shows the cursor hugging the test sphere in the scene and displays hand detected asset when hand is detected in ready state.

#### FocusedObjectKeywords.unity
Example on how to send keyword messages to currently focused dynamically instantiated object.
Gazing on an object and saying "Make Smaller" and "Make Bigger" will adjust object size.

#### InputTapTest.unity
Test scene shows you in a simple way, how to respond to user's gaze using the Input module.
It also shows you how to respond to the user's tap gesture.

In this scene, under the InputManager prefab > GesturesInput, the script GamepadInput.cs helps map the Xbox gamepad buttons to gestures.
Press A to air tap.

#### NavigationRotateTest.unity
Test scene shows how to respond to user's navigation gesture by rotating the cube along Y axis.

In this scene, under the InputManager prefab > GesturesInput, the script GamepadInput.cs helps map the Xbox gamepad buttons to gestures.
With A pressed rotate the left joystick to trigger the navigation gesture.

#### GamepadTest.unity
This scene has some objects in there which respond to the Xbox gamepad input.
It also has the HoloToolkitCameraParent prefab which is useful for testing teleporting scenarios for occluded devices.
Cube:
Press A to air tap.
Press A and left joystick to rotate the cube.

Quad:
Gaze at the quad and press Y to teleport to that location.
Press B to return to the original location.

#### KeywordManager.unity
Shows how to use the KeywordManager.cs script to add keywords to your scene.

1. Select whether you want the recognizer to start automatically or when you manually start it.
2. Specify the number of keywords you want.
3. Type the word or phrase you'd like to register as the keyword and, if you want, set a key code to use in the Editor. You can also use an attached microphone with the Editor.
4. Press the + to add a response. Then, drag a GameObject with the script you want to call into the "None (Object)" field.
5. Select the script and method to call or variable to set from the "No Function" dropdown. Add any parameters, if necessary, into the field below the dropdown.

When you start the scene, your keywords will automatically be registered on a KeywordRecognizer, and the recognizer will be started (or not) based on your Recognizer Start setting.

#### ManualCameraControl.unity

This scene shows how to manually control the camera.  The script is on the main camera of the scene.  When preview mode in Unity is activated, the user can move around the scene using WASD and look around using right-mouse-button + mouse. 

#### MicrophoneStream.unity
Example usage of MicStream.cs to select and record beam-formed audio from the hololens. In editor, the script lets you choose if you want to beam-form capture on voice or on the room. When running, press 'Q' to start the stream you selected, 'W' will stop the stream, 'A' starts recording a wav file, and 'S' stops the recording, saves it to your Music library, and prints the full path of the audio clip.

#### MotionControllerTest.unity
This scene shows how to render motion controllers in your app. It also contains a debug panel to help diagnose the state of a connected controller.

#### SelectedObjectKeywords.unity
Example on how to send keyword messages to currently selected dynamically instantiated object.
Gazing on an object and saying "Select Object" will persistently select that object for interaction with voice commands,
after which the user can also adjust object size with "Make Smaller" and "Make Bigger" voice commands and finally clear
currently selected object by saying "Clear Selection".

#### OverrideFocusedObjectTest.unity
Test scene shows you in a simple way, how to route input to an object not being gazed/focused at.
Useful for scenarios like placing head locked content or clicking around to create objects.

#### SpeechInputSource.unity

Shows how to use the SpeechInputSource.cs script to add keywords to your scene.

1. Select whether you want the recognizer to start automatically or when you manually start it.
2. Specify the number of keywords you want.
3. Type the word or phrase you'd like to register as the keyword and, if you want, set a key code to use in the Editor. You can also use an attached microphone with the Editor.
4. Attach a script that implements ISpeechHandler.cs to the object in the scene that will require the gaze focus to execute the command. You should register this script with the InputManager.cs as a global listener to handle keywords that don't require a focused object.

When you start the scene, your keywords will automatically be registered on a KeywordRecognizer, and the recognizer will be started (or not) based on your Recognizer Start setting.

#### 

---
##### [Go back up to the table of contents](#table-of-contents)
##### [Go back to the main page.](../../../README.md)
---
