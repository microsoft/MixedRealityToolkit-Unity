## [Input]()
Scripts that leverage the HoloLens input features namely Gaze, Gesture and Voice.

This contains a fully-featured **input module**, which allows you to handle various types of input and send them to any game object being currently gazed at, or any fallback object. It also includes a **cursor** similar to the HoloLens shell cursor that fully leverages the Unity's animation system.

### [Input Module Design](InputModuleDesign)
The input module is designed to be extensible: it could support various input mechanisms and various types of gazers.

Each input source (hands, gestures, others) implement a **IInputSource** interface. The interface defines various events that the input sources can trigger. The input sources register themselves with the InputManager, whose role it is to forward input to the appropriate game objects. Input sources can be dynamically enabled / disabled as necessary, and new input sources can be created to support different input devices.

Game objects that want to consume input events can implement one or many **input interfaces**, such as:

- **IFocusable** for focus enter and exit. The focus can be triggered by the user's gaze or any other gaze source.
- **IHoldHandle** for the Windows hold gesture.
- **IInputHandler** for source up and down. The source can be a hand that tapped, a clicker that was pressed, etc.
- **IInputClickHandler** for source clicked. The source can be a hand that tapped, a clicker that was pressed, etc.
- **IManipulationHandler** for the Windows manipulation gesture.
- **INavigationnHandler** for the Windows navigation gesture.
- **ISourceStateHandler** for the source detected and source lost events.
- **ISpeechHandler** for voice commands.

The **input manager** listens to the various events coming from the input sources, and also takes into account the gaze. Currently, that gaze is always coming from the GazeManager class, but this could be extended to support multiple gaze sources if the need arises.

By default, input events are sent to the currently focused game object, if that object implements the appropriate interface. Modals input handlers can also be added to the input manager: these modal handlers will take priority over the currently focused object Fallback handlers can also be defined, so that the application can react to global inputs that aren't targeting a specific element. Any event sent by the input manager always bubbles up from the object to its ancestors. 

In recap, the input manager forwards the various input sources events to the appropriate game object, using the following order:

1. The registered modal input handlers, in LIFO (Last-In First-Out) order of registration
2. The currently focused object
3. The fallback input handlers, in LIFO order of registration

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

This also includes a fake input source that allows you to simulate hand input when in the editor. By default, this can be done by holding Shift (left hand) or Space (right hand), moving the mouse to move the hand and use the left mouse button to tap.

### [Scripts](Scripts)
Scripts related to the input features.

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

#### Gaze
##### BaseRayStabilizer.cs
A base abstract class for a stabilizer that takes as input position and orientation, and performs operations on them to stabilize or smooth that data.

##### GazeManager.cs
Singleton component in charge of managing the gaze vector. This is where you can define which layers are considered when gazing at objects. Optionally, the gaze manager can reference a ray stabilizer that will be used to stabilize the gaze of the user.

- **MaxGazeCollisionDistance :** the maximum distance to raycast. Any holograms beyond this value will not be raycasted to.
- **RaycastLayers :** the Unity layers to raycast against. If you have holograms that should not be raycasted against, like a cursor, do not include their layers in this mask.
- **Stabilizer :** stabilizer to use to stabilize the gaze. If not set, the gaze will not be stabilized.
- **Gaze Transform :** the transform to use as the source of the gaze. If not set, will default to the main camera.

##### GazeStabilizer.cs
Stabilize the user's gaze to account for head jitter.

- **StoredStabilitySamples** Number of samples that you want to iterate on.  A larger number will be more stable.

#### InputEvents
##### BaseInputEventData.cs
Base class for all input event data. An input event data is what is sent as a parameter to all input events.

##### HoldEventData.cs
Event data for an event coming from the hold gesture.

##### IFocusable.cs
Interface that a game object can implement to react to focus enter/exit.

##### IHoldHandler.cs
Interface that a game object can implement to react to hold gestures.

##### IInputHandler.cs
Interface that a game object can implement to react to simple pointer-like inputs.

##### IManipulationHandler.cs
Interface that a game object can implement to react to manipulation gestures.

##### INavigationHandler.cs
Interface that a game object can implement to react to navigation gestures.

##### ISourceStateHandler.cs
Interface that a game object can implement to react to source state changes, such as when an input source is detected or lost.

##### InputEventData.cs
Event data for an event that represents an input interaction such as a tap / click.

##### ManipulationEventData.cs
Event data for an event coming from the manipulation gesture.

##### NavigationEventData.cs
Event data for an event coming from the navigation gesture.

##### SourceStateEventData.cs
Event data for an event that represents an input source being detected or lost.

#### GestureManipulator.cs
A component for moving an object via the GestureManager manipulation gesture.

When an active GestureManipulator component is attached to a GameObject it will subscribe 
to GestureManager's manipulation gestures, and move the GameObject when a ManipulationGesture occurs. 
If the GestureManipulator is disabled it will not respond to any manipulation gestures. 
 
This means that if multiple GestureManipulators are active in a given scene when a manipulation 
gesture is performed, all the relevant GameObjects will be moved.  If the desired behavior is that only 
a single object be moved at a time, it is recommended that objects which should not be moved disable 
their GestureManipulators, then re-enable them when necessary (e.g. the object is focused). 

#### InputSources

##### BaseInputSource.cs
Abstract base class for an input source that implements IInputSource. Defines the various abstract functions that any input source needs to implement, and provides some default implementations.

##### EditorHandsInput.cs
Input source for fake hands information, which can be used to simulate hands input in the Unity editor.

##### GesturesInput.cs
Input source for gestures information from the WSA APIs, which gives access to various system supported gestures.

##### IInputSource.cs
Interface for an input source. An input source is any input mechanism that can be used as the source of user interactions.

##### RawInteractionSourcesInput.cs
Input source for raw interactions sources information, which gives finer details about current source state and position than the standard GestureRecognizer.

#### Interactions Scripts
##### HandDraggable.cs
Allows dragging an object in space with your hand on HoloLens. Just attach the script to a game object to make it movable.

#### Microphone
##### MicStream.cs
Lets you access beam-formed microphone streams from the HoloLens to optimize voice and/or room captures, which is impossible to do with Unity's Microphone object. Takes the data and inserts it into Unity's AudioSource object for easy handling. Also lets you record indeterminate-length audio files from the Microphone to your device's Music Library, also using beam-forming.

Check out Assets/HoloToolkit/Input/Tests/Scripts/MicStreamDemo.cs for an example of implementing these features, which is used in the demo scene at Assets/HoloToolkit/Input/Tests/MicrophoneStream.unity.

**IMPORTANT**: Please make sure to add the Microphone and Music Library capabilities in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

**_KeywordsAndResponses_** Set the size as the number of keywords you'd like to listen for, then specify the keywords and method responses to complete the array.

**RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

#### Voice

**IMPORTANT**: Please make sure to add the Microphone capabilities in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

##### KeywordManager.cs
Allows you to specify keywords and methods in the Unity Inspector, instead of registering them explicitly in code.  

**_KeywordsAndResponses_** Set the size as the number of keywords you'd like to listen for, then specify the keywords and method responses to complete the array.

**RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

##### SpeechInputSource.cs
Allows you to specify keywords and keyboard shortcuts in the Unity Inspector, instead of registering them explicitly in code. Keywords are handled by scripts that implement ISpeechHandler.cs.

Check out Assets/HoloToolkit/Input/Tests/Scripts/SphereKeywords.cs and Assets/HoloToolkit/Input/Tests/Scripts/SphereGlobalKeywords.cs for an example of implementing these features, which is used in the demo scene at Assets/HoloToolkit/Input/Tests/SpeechInputSource.unity.

**_KeywordsAndKeys_** Set the size as the number of keywords you'd like to listen for, then specify the keywords to complete the array.

**RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

##### ISpeechHandler.cs
Interface that a game object can implement to react to speech keywords.

### [Test Prefabs](TestPrefabs)

Prefabs used in the various test scenes, which you can use as inspiration to build your own.

#### FocusedObjectKeywordManager.prefab
Keyword manager pre-wired to send messages to object being currently focused via FocusedObjectMessageSender component.
You can simply drop this into your scene and be able to send arbitrary messages to currently focused object.

#### SelectedObjectKeywordManager.prefab
Keyword manager pre-wired to send messages to object being currently selected via SelectedObjectMessageSender component.
You can simply drop this into your scene and be able to send arbitrary messages to currently selected object.

### [Test Scripts](TestScripts)
#### FocusedObjectMessageSender.cs
Sends Unity message to currently focused object.
FocusedObjectMessageSender.SendMessageToFocusedObject needs to be registered as a response in KeywordManager
to enable arbitrary messages to be sent to currently focused object.

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

### [Tests](Tests)
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

#### SelectedObjectKeywords.unity
Example on how to send keyword messages to currently selected dynamically instantiated object.
Gazing on an object and saying "Select Object" will persistently select that object for interaction with voice commands,
after which the user can also adjust object size with "Make Smaller" and "Make Bigger" voice commands and finally clear
currently selected object by saying "Clear Selection".

#### SpeechInputSource.unity

Shows how to use the SpeechInputSource.cs script to add keywords to your scene.

1. Select whether you want the recognizer to start automatically or when you manually start it.
2. Specify the number of keywords you want.
3. Type the word or phrase you'd like to register as the keyword and, if you want, set a key code to use in the Editor. You can also use an attached microphone with the Editor.
4. Attach a script that implements ISpeechHandler.cs to the object in the scene that will require the gaze focus to execute the command. You should register this script with the InputManager.cs as a global listener to handle keywords that don't require a focused object.

When you start the scene, your keywords will automatically be registered on a KeywordRecognizer, and the recognizer will be started (or not) based on your Recognizer Start setting.

#### 

---
##### [Go back up to the table of contents.](../../../README.md)
---