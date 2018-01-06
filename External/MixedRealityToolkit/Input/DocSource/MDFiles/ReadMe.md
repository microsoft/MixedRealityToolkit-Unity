HoloToolkit Input                        {#mainpage}
============

## Description

Any content that leverage the HoloLens input features, namely Gaze, Gesture and Voice.

This contains a fully-featured **input module**, which allows you to handle various types of input and send them to any game object being currently gazed at, or any fallback object. It also includes a **cursor** similar to the HoloLens shell cursor that fully leverages the Unity's animation system.

## Input Module Design
The input module is designed to be extensible: it could support various input mechanisms and various types of gazers.

Each input source (hands, gestures, others) implement a **IInputSource** interface. The interface defines various events that the input sources can trigger. The input sources register themselves with the InputManager, whose role it is to forward input to the appropriate game objects. Input sources can be dynamically enabled / disabled as necessary, and new input sources can be created to support different input devices.

Game objects that want to consume input events can implement one or many **input interfaces**, such as:

- **IFocusable** for focus enter and exit. The focus can be triggered by the user's gaze or any other gaze source.
- **IHoldHandle** for the Windows hold gesture.
- **IInputHandler** for source up, down and clicked. The source can be a hand that tapped, a clicker that was pressed, etc.
- **IManipulationHandler** for the Windows manipulation gesture.

The **input manager** listens to the various events coming from the input sources, and also takes into account the gaze. Currently, that gaze is always coming from the GazeManager class, but this could be extended to support multiple gaze sources if the need arises.

By default, input events are sent to the currently focused game object, if that object implements the appropriate interface. Modals input handlers can also be added to the input manager: these modal handlers will take priority over the currently focused object Fallback handlers can also be defined, so that the application can react to global inputs that aren't targeting a specific element. Any event sent by the input manager always bubbles up from the object to its ancestors. 

In recap, the input manager forwards the various input sources events to the appropriate game object, using the following order:

1. The registered modal input handlers, in LIFO order of registration
2. The currently focused object
3. The fallback input handlers, in LIFO order of registration

The input manager also has a pointer to the currently active **Cursor**, allowing it to be accessed from there. The cursor currently also depends on the gaze coming from the GazeManager class.

## Prefabs

### DefaultCursor
A 3D gaze cursor that had different animation states and is similar to the one available in the HoloLens OS. This can be used as a base to create more complex cursors, by adding different states to its animator.

### HandsManager
Prefab containing everything that is needed to handle hands input. This includes real hands input, editor hands input to simulate hands in the editor and the Windows gestures input.

### HoloLensCamera
Defines a HoloLens-ready Unity camera that can also be moved from the editor.

### InputManager
Contains an input manager and a stabilized gaze manager.

## Top-level Scripts

### InputManager.cs
Class that manages various kinds of inputs and forwards them to the appropriate game object, typically the game object being gazed at.

### KeyboardManager.cs
Singleton script that allows other scripts to register for (or inject) Keyboard events.

## Cursor Scripts

### Cursor.cs
The main script that defines the behavior of a cursor. This consumes the GazeManager to figure out where the cursor should.

## CursorModifier.cs
A component that can be added to any game object that has a collider to allow that game object to modify the cursor when the user is gazing at it. This supports more advanced features such as snapping the cursor, hiding the cursor, triggering custom cursor animation events and offsetting the cursor.

## Gaze Scripts
### BaseRayStabilizer.cs
An abstract class that defines the functions that a ray stabilizer should implement. A ray stabilizer can stabilize anything that has a position and an orientation (and thus can cast a ray).

### GazeManager.cs
Singleton component in charge of managing the gaze vector. This is where you can define which layers are considered when gazing at objects. Optionally, the gaze manager can reference a ray stabilizer that will be used to stabilize the gaze of the user.

- **MaxGazeCollisionDistance :** the maximum distance to raycast. Any holograms beyond this value will not be raycasted to.
- **RaycastLayers :** the Unity layers to raycast against. If you have holograms that should not be raycasted against, like a cursor, do not include their layers in this mask.
- **Stabilizer :** stabilizer to use to stabilize the gaze. If not set, the gaze will not be stabilized.
- **Gaze Transform :** the transform to use as the source of the gaze. If not set, will default to the main camera.

### GazeStabilizer.cs
Stabilize the user's gaze to account for head jitter.

- **StoredStabilitySamples :** number of samples that you want to iterate on. A larger number will be more stable.
- **PositionDropOffRadius :** position based distance away from gravity well.
- **DirectionDropOffRadius :** direction based distance away from gravity well.
- **PositionStrength :** position lerp interpolation factor.
- **DirectionStrength :** direction lerp interpolation factor.
- **StabilityAverageDistanceWeight :** stability average weight multiplier factor.
- **StabilityVarianceWeight :** stability variance weight multiplier factor.

## Gesture Scripts

### HandGuidance.cs
Show a GameObject when a gesturing hand is about to lose tracking.
You must provide GameObjects for the Cursor and HandGuidanceIndicator public fields.

- **Cursor :** the object in your scene that is being used as the cursor. The hand guidance indicator will be rendered around this cursor.
- **HandGuidanceIndicator :** gameObject to display when your hand is about to lose tracking.
- **HandGuidanceThreshold :** when to start showing the HandGuidanceIndicator. 1 is out of view, 0 is centered in view.

## InputEvents Scripts
### BaseInputEventData.cs
Base class for all input event data. An input event data is what is sent as a parameter to all input events.

### HoldEventData.cs
Event data for an event coming from the hold gesture.

### IFocusable.cs
Interface that a game object can implement to react to focus enter/exit.

### IHoldHandler.cs
Interface that a game object can implement to react to hold gestures.

### IInputHandler.cs
Interface that a game object can implement to react to simple pointer-like inputs.

### IManipulationHandler.cs
Interface that a game object can implement to react to manipulation gestures.

### InputEventData.cs
Event data for an event that represents an input interaction such as a tap / click.

### ManipulationEventData.cs
Event data for an event coming from the manipulation gesture.

## InputSources Scripts

### BaseInputSource.cs
Abstract base class for an input source that implements IInputSource. Defines the various abstract functions that any input source needs to implement, and provides some default implementations.

### EditorHandsInput.cs
Input source for fake hands information, which can be used to simulate hands input in the Unity editor.

### GesturesInput.cs
Input source for gestures information from the WSA APIs, which gives access to various system supported gestures.

### HandsInput.cs
Input source for raw hands information, which gives finer details about current hand state and position, and has a better refresh rate than the gestures.

### IInputSource.cs
Interface for an input source. Defines

## Interactions Scripts
### HandDraggable.cs
Allows dragging an object in space with your hand on HoloLens. Just attach the script to a game object to make it movable.

## Microphone Scripts
### MicStream.cs
Replaces Unity's Microphone object to allow HoloLens mic stream selection. Works on all Windows 10 devices, but has features that are specifically suited for HoloLens' multiple microphones.

### MicStreamDemo.cs
Demo for the mic stream class.

## UI Scripts
### Button.cs
The button script can be attached to any game object with a collider to make it act like a button. It makes the object gaze-interactable, and receive pressed and released events. It allows you to design an interactable button backed by animations.

### TriggerButton.cs
Very simple class that implements basic logic for a trigger button.

## Voice Scripts
IMPORTANT: To use voice commands, please make sure to add the microphone capability in your app, in Unity under
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities
or in your Visual Studio Package.appxmanifest capabilities.

### KeywordManager.cs
Allows you to specify keywords and methods in the Unity Inspector, instead of registering them explicitly in code.

- **KeywordsAndResponses :** set the size as the number of keywords you'd like to listen for, then specify the keywords and method responses to complete the array.
- **RecognizerStart :** set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

### SpeechInput.cs
Contains the interface for the speech input manager.

### SpeechInputManager.cs
Allows you to define speech input keywords and associated callback methods that are managed by a single class. Any input keyword must be registered in the Start() method of your other components.

## Utilities Scripts

### AxisController.cs
A class that can use keyboard, mouse or joystick input to control up to 3 axis. This is used to control the camera in Unity and create fake hands that can be manipulated from Unity.

### ButtonController.cs
Provides a per-key or per-button component for the manual input controls in the Unity editor. This is used to simulate HoloLens behavior in the ditor.

### ManualGazeControl.cs
A script that can be attached to a camera to manually control it in the Unity editor.

### ManualHandControl.cs
A script that can be used to simulate HoloLens hand movements in the Unity editor. 