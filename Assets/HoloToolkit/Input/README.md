## [Input]()
Scripts that leverage the HoloLens input features namely Gaze, Gesture and Voice.

### [Prefabs](Prefabs)
Prefabs related to the input features.

#### BasicCursor.prefab
Torus shaped basic cursor that follows the user's gaze around.

#### Cursor.prefab
Torus shaped CursorOnHolograms when user is gazing at holograms and point light CursorOffHolograms when user is gazing away from holograms.

#### CursorWithFeedback.prefab
Torus shaped cursor that follows the user's gaze and HandDetectedFeedback asset to give feedback to user when their hand is detected in the ready state.

#### FocusedObjectKeywordManager.prefab
Keyword manager pre-wired to send messages to object being currently focused via FocusedObjectMessageSender component.
You can simply drop this into your scene and be able to send arbitrary messages to currently focused object.

#### SelectedObjectKeywordManager.prefab
Keyword manager pre-wired to send messages to object being currently selected via SelectedObjectMessageSender comoponent.
You can simply drop this into your scene and be able to send arbitrary messages to currently selected object.

### [Scripts](Scripts)
Scripts related to the input features.

#### BasicCursor.cs
1. Decides when to show the cursor.
2. Positions the cursor at the gazed hit location.
3. Rotates the cursor to match hologram normals.

#### CursorFeedback.cs
CursorFeedback class takes GameObjects to give cursor feedback to users based on different states.

#### CursorManager.cs
CursorManager class takes Cursor GameObjects. One that is a Cursor on Holograms and another Cursor off Holograms.

1. Shows the appropriate Cursor when a hologram is hit.
2. Places the appropriate Cursor at the hit position.
3. Matches the Cursor normal to the hit surface.

You must provide GameObjects for the **_CursorOnHologram_** and **_CursorOffHologram_** public fields.

**_CursorOnHologram_** Cursor object to display when you are gazing at a hologram.

**_CursorOffHologram_** Cursor object to display when you are not gazing at a hologram.

**DistanceFromCollision** Distance, in meters, to offset the cursor from a collision with a hologram in the scene.  This is to prevent the cursor from being occluded.

#### GazeManager.cs
Perform Physics.Raycast in the user's gaze direction to get the position and normals of any collisions.

**MaxGazeDistance** The maximum distance to Raycast.  Any holograms beyond this value will not be raycasted to.

**RaycastLayerMask** The Unity layers to raycast against.  If you have holograms that should not be raycasted against, like a cursor, do not include their layers in this mask.

#### GazeStabilizer.cs
Stabilize the user's gaze to account for head jitter.

**StoredStabilitySamples** Number of samples that you want to iterate on.  A larger number will be more stable.

**PositionDropOffRadius** Position based distance away from gravity well.

**DirectionDropOffRadius** Direction based distance away from gravity well.

**PositionStrength** Position lerp interpolation factor.

**DirectionStrength** Direction lerp interpolation factor.

**StabilityAverageDistanceWeight** Stability average weight multiplier factor.

**StabilityVarianceWeight** Stability variance weight multiplier factor.

#### GestureManager.cs
GestureManager provides access to several different input gestures, including Tap and Manipulation. 

When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
GestureManager then sends a message to that game object.  It also has an **OverrideFocusedObject** which lets you send gesture input to a specific object by overriding the gaze.

Using Manipulation requires subscribing to the ManipulationStarted events and then querying information about the manipulation gesture via ManipulationOffset and ManipulationHandPosition.  See GestureManipulator for an example.

#### GestureManipulator.cs
A component for moving an object via the GestureManager manipulation gesture.

When an active GestureManipulator component is attached to a GameObject it will subscribe 
to GestureManager's manipulation gestures, and move the GameObject when a ManipulationGesture occurs. 
If the GestureManipulator is disabled it will not respond to any manipulation gestures. 
 
This means that if multiple GestureManipulators are active in a given scene when a manipulation 
gesture is performed, all the relevant GameObjects will be moved.  If the desired behavior is that only 
a single object be moved at a time, it is recommended that objects which should not be moved disable 
their GestureManipulators, then re-enable them when necessary (e.g. the object is focused). 

#### HandGuidance.cs
Show a GameObject when a gesturing hand is about to lose tracking.

You must provide GameObjects for the **_Cursor_** and **_HandGuidanceIndicator_** public fields.

**_Cursor_** The object in your scene that is being used as the cursor.  The hand guidance indicator will be rendered around this cursor.

**_HandGuidanceIndicator_** GameObject to display when your hand is about to lose tracking.

**HandGuidanceThreshold** When to start showing the HandGuidanceIndicator.  1 is out of view, 0 is centered in view.

#### HandsManager.cs
Keeps track of when the user's hand has been detected in the ready position.

#### KeywordManager.cs
Allows you to specify keywords and methods in the Unity Inspector, instead of registering them explicitly in code.  
**IMPORTANT**: Please make sure to add the microphone capability in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

**_KeywordsAndResponses_** Set the size as the number of keywords you'd like to listen for, then specify the keywords and method responses to complete the array.

**RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

#### Microphone/MicStream.cs
Lets you access beam-formed microphone streams from the HoloLens to optimize voice and/or room captures, which is impossible to do with Unity's Microphone object. Takes the data and inserts it into Unity's AudioSource object for easy handling. Also lets you record indeterminate-length audio files from the Microphone to your device's Music Library, also using beam-forming.

Check out Assets/HoloToolkit/Input/Tests/Scripts/MicStreamDemo.cs for an example of implementing these features, which is used in the demo scene at Assets/HoloToolkit/Input/Tests/MicrophoneStream.unity.

**IMPORTANT**: Please make sure to add the Microphone and Music Library capabilities in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

**_KeywordsAndResponses_** Set the size as the number of keywords you'd like to listen for, then specify the keywords and method responses to complete the array.

**RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

#### FocusedObjectMessageSender.cs
Sends Unity message to currently focused object.
FocusedObjectMessageSender.SendMessageToFocusedObject needs to be registered as a response in KeywordManager
to enable arbitrary messages to be sent to currently focused object.

#### SelectedObjectMessageSender.cs
Sends Unity message to currently selected object.
SelectedObjectMessageSender.SendMessageToSelectedObject needs to be registered as a response in KeywordManager
to enable arbitrary messages to be sent to currently selected object.

#### FocusedObjectMessageReceiver.cs
Example on how to handle messages send by FocusedObjectMessageSender.
In this particular implementation, focused object color it toggled on gaze enter/exit events.

#### SelectedObjectMessageReceiver.cs
Example on how to handle messages send by SelectedObjectMessageSender.
In this particular implementation, selected object color it toggled on selecting object and clearing selected object.

#### SimpleGridGenerator.cs
A grid of dynamic objects to illustrate sending messages to prefab instances created at runtime as opposed
to only static objects that already exist in the scene.

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

#### SelectedObjectKeywords.unity
Example on how to send keyword messages to currently selected dynamically instantiated object.
Gazing on an object and saying "Select Object" will persistently select that object for interaction with voice commands,
after which the user can also adjust object size with "Make Smaller" and "Make Bigger" voice commands and finally clear
currently selected object by saying "Clear Selection".

#### KeywordManager.unity
Shows how to use the KeywordManager.cs script to add keywords to your scene.

1. Select whether you want the recognizer to start automatically or when you manually start it.
2. Specify the number of keywords you want.
3. Type the word or phrase you'd like to register as the keyword and, if you want, set a key code to use in the Editor. You can also use an attached microphone with the Editor.
4. Press the + to add a response. Then, drag a GameObject with the script you want to call into the "None (Object)" field.
5. Select the script and method to call or variable to set from the "No Function" dropdown. Add any parameters, if necessary, into the field below the dropdown.

When you start the scene, your keywords will automatically be registered on a KeywordRecognizer, and the recognizer will be started (or not) based on your Recognizer Start setting.

#### MicrophoneStream.unity
Example usage of MicStream.cs to select and record beam-formed audio from the hololens. In editor, the script lets you choose if you want to beam-form capture on voice or on the room. When running, press 'Q' to start the stream you selected, 'W' will stop the stream, 'A' starts recording a wav file, and 'S' stops the recording, saves it to your Music library, and prints the full path of the audio clip.

---
##### [Go back up to the table of contents.](../../../README.md)
---