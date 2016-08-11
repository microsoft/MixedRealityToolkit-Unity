# HoloToolkit-Unity
This is effectively part of the existing HoloToolkit, but this is the repository that will contain all Unity specific components.
The HoloToolkit is a collection of scripts and components intended to accelerate development of holographic applications targeting Windows Holographic.

HoloToolkit contains the following feature areas:

1. [Input](https://github.com/Microsoft/HoloToolkit-Unity#input)
2. [Sharing](https://github.com/Microsoft/HoloToolkit-Unity#sharing)
3. [Spatial Mapping](https://github.com/Microsoft/HoloToolkit-Unity#spatialmapping)
4. [Spatial Sound](https://github.com/Microsoft/HoloToolkit-Unity#spatialsound)
5. [Utilities](https://github.com/Microsoft/HoloToolkit-Unity#utilities-1)
6. [Build](https://github.com/Microsoft/HoloToolkit-Unity#build)

To learn more about individual HoloLens feature areas, please read the [Wiki](https://github.com/Microsoft/HoloToolkit-Unity/wiki) section.

To learn how to add the HoloToolkit to your project see the [Getting Started](GettingStarted.md) guide.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

---

## [CrossPlatform](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/CrossPlatform)
Wrapper scripts for Win32 and WinRT APIs in a single API call that works in the Unity editor and in a UWP application.

---

## [Input](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Input)
Scripts that leverage the HoloLens input features namely Gaze, Gesture and Voice.

### [Prefabs](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Input/Prefabs)
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

### [Scripts](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Input/Scripts)
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
GestureManager creates a gesture recognizer and signs up for a tap gesture. When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
GestureManager then sends a message to that game object. 

It also has an **OverrideFocusedObject** which lets you send gesture input to a specific object by overriding the gaze.

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

### [Tests](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Input/Tests)
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

---
##### [Go back up to the table of contents.](https://github.com/Microsoft/HoloToolkit-Unity#holotoolkit-unity)
---

## [Sharing](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Sharing)
Sharing and networking components for rapid prototyping in Unity for building shared experiences.

Ensure you have the following capabilities set:
1. Enable the "SpatialPerception" capability in Player Settings -> Windows Store -> Publishing Settings -> Capabilities.
2. For using Sharing components, you will also need to set the InternetClientServer, PrivateNetworkClientServer, and Microphone capabilities.

### [Editor](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Sharing/Editor)
Enables the HoloToolkit menu option in the Unity top tool bar.

### [Plugins](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Sharing/Plugins)
Contains compiled architecture specific binaries for SharingClient.dll which are required by the Unity application for accessing sharing APIs.
Binaries are compiled from the native [HoloToolkit\Sharing](https://github.com/Microsoft/HoloToolkit/tree/master/Sharing).

### [Prefabs](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Sharing/Prefabs)
Prefabs related to the sharing and networking features.

#### Sharing.prefab
1. Enables sharing and networking in your Unity application.
2. Allows you to communicate between a Windows and non-Windows device.

**SharingStage.cs** allows you to be a Primary Client (typical case).
**Server Address** is the IP address of the machine running the HoloToolkit -> Launch Sharing Service.
**Server Port** displays the port being used for communicating.

**SharingSessionTracker.cs** keeps track of the players joining and leaving a shared session.

**AutoJoinSession.cs** creates a shared session with Session Name 'Default' which is customizable.
Joins a player to that session if once already exists.

### [Scripts](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Sharing/Scripts)
Scripts related to the sharing and networking features.

#### SDK
Contains scripts compiled from the native [HoloToolkit\Sharing](https://github.com/Microsoft/HoloToolkit/tree/master/Sharing) repository and using the SWIG tool to generate different language bindings.

#### Utilities
Utility scripts for the Sharing.prefab.
Also scripts for logging, launching processes, math utilities.

### [Tests](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Sharing/Tests)
Tests related to the sharing features. To use the scene:
1. Navigate to the Tests folder.
2. Double click on the test scene you wish to explore.
3. Either click "Play" in the unity editor or File -> Build Settings.
4. Add Open Scenes, Platform -> Windows Store, SDK -> Universal 10, Build Type -> D3D, Check 'Unity C# Projects'.
5. Click 'Build' and create an App folder. When compile is done, open the solution and deploy to device.

#### Sharing.unity 
Sharing scene demonstrates how to use the Sharing prefabs for networking and sharing custom messages with clients. 
It also demonstrates how to share world anchors between clients to establish a shared space.

1. Ensure to launch the sharing service using: HoloToolkit -> Launch Sharing service
2. Enter the IP address displayed in the console window into the Server Address of the Sharing object.
3. **CustomMessages.cs** shows how to communicate specific information across clients.
4. **ImportExportAnchorManager.cs** shows how to create anchors and share them with other clients using the sharing service.
5. **RemoteHeadManager.cs** draw cubes on remote heads of users joining the session.

---
##### [Go back up to the table of contents.](https://github.com/Microsoft/HoloToolkit-Unity#holotoolkit-unity)
---

## [SpatialMapping](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping)
Scripts that leverage SpatialMapping related features.

1. Enable the "SpatialPerception" capability in Player Settings -> Windows Store -> Publishing Settings -> Capabilities.
2. If using the RemoteMapping components, you will also need to set the InternetClientServer, PrivateNetworkClientServer, and Microphone capabilities. 

**IMPORTANT**: Please make sure to add the Spatial Perception capability in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

### [Plugins](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping/Plugins)

PlaneFinding addon that can be used to find planar surfaces (ie: walls/floors/tables/etc) in the mesh data returned by Spatial Mapping.

### [Prefabs](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping/Prefabs)

The following prefabs make it easy to quickly access and visualize spatial mapping data in the HoloLens or in the Unity Editor.

#### RemoteMapping.prefab
Use with SpatialMapping prefab, it allows you to send meshes from the HoloLens to Unity and save/load the meshes for use later.

#### SpatialMapping.prefab
Base prefab which allows you to visualize and access spatial mapping data on the HoloLens. It can also save/load room models that were captured from the Windows Device Portal.

#### SurfacePlane.prefab
Helper prefab which should be referenced by the SurfaceMeshesToPlanes component for classifying planes as floor, ceiling, wall, etc during processing.

### [Scripts](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping/Scripts)

The following scripts give you the ability to access spatial mapping data on the HoloLens and load spatial mapping data in the Unity Editor.

#### ObjectSurfaceObserver.cs
A SpatialMappingSource that loads spatial mapping data saved from the Windows Device Portal.

**RoomModel** The room model to use when loading meshes in Unity.

#### SpatialMappingManager.cs
Manages interactions between the application and all spatial mapping data sources (file, observer, network).

**PhysicsLayer** The physics layer to use for all spatial mapping mesh data.

**SurfaceMaterial** The material to apply when rendering the spatial mapping mesh data.

**DrawVisualMeshes** Determines if spatial mapping meshes will be rendered.

**CastShadows** Determines if spatial mapping meshes can cast shadows.

#### SpatialMappingObserver.cs
Adds and updates spatial mapping data for all surfaces discovered by the SurfaceObserver running on the HoloLens.

**TrianglesPerCubicMeter** Level of detail to use for each mesh found by the SurfaceObserver.

**Extents** Extents of the observation volume which expand out from the camera's position.

**TimeBetweenUpdates** Time to wait (sec) before processing updates from the SurfaceObserver.

#### SpatialMappingSource.cs
Generates and retrieves meshes based on spatial mapping data coming from the current source object (file, observer, network). 
SpatialMappingManager.cs manages switching between source types and interacting with this class.

#### TapToPlace.cs
Simple script to add to a GameObject that allows users to tap and place the GameObject along the spatial mapping mesh.
Requires GazeManager, GestureManager, and SpatialMappingManager in the scene.

### [Scripts\RemoteMapping](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping/Scripts/RemoteMapping)

The following scripts allow you to send spatial mapping data from the HoloLens to the Unity Editor and to save/load the meshes for use later.

#### FileSurfaceObserver.cs
A SpatialMappingSource that loads spatial mapping data saved during a remote mapping session.

**MeshFileName** Name of file to use when saving mesh data from the network or loading surface mesh data into Unity.

**SaveFileKey** Key to press when running in the Unity Editor to save meshes that came from the network.

**LoadFileKey** Key to press when running in the Unity Editor to load meshes that were save from the network.

#### MeshSaver.cs
Static class that can read and write mesh data sent during a remote mapping session to the file specified in FileSurfaceObserver.cs.

#### RemoteMappingManager.cs
Allows sending meshes remotely from HoloLens to Unity.

**RemoteMappingKey** The key to press when running in the Unity editor to enable spatial mapping over the network.

**SendMeshesKeyword** The phrase to speak when you are ready to send meshes over the network from HoloLens to Unity.

#### RemoteMeshSource.cs
Networking component that runs on the HoloLens and can send meshes to Unity.

**ServerIP** The IPv4 address of the machine running the Unity editor.

**ConnectionPort** The network port of the Unity machine that will receive spatial mapping data from the HoloLens.

#### RemoteMeshTarget.cs
SpatialMappingSource object that runs in the Unity editor and receive spatial mapping data from the HoloLens.

**ServerIP** The IPv4 address of the machine running the Unity editor.

**ConnectionPort** The network port of the Unity machine that will receive mesh data from the HoloLens.

#### SimpleMeshSerializer.cs
Static class that converts a Unity mesh to an array of bytes. Used by MeshSaver.cs to serialize and deserialize mesh data sent during a remote mapping session.

### [Scripts\SpatialProcessing](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping/Scripts)

The following scripts allow you to process the raw spatial mapping data in order to find planes, remove vertices, etc.

#### PlaneFinding.cs
Unity script that wraps the native PlaneFinding DLL. Used by SurfaceMeshesToPlanes.cs.

#### RemoveSurfaceVertices.cs
A spatial processing component that will remove any spatial mapping vertices that fall within the specified bounding volumes.

**BoundsExpansion** The amount, if any, to expand each bounding volume by.

#### SurfaceMeshesToPlanes.cs
A spatial processing component that can find and create planes based on spatial mapping meshes. Uses PlaneFinding.cs and requires the PlaneFinding plug-in.

**ActivePlanes** Collection of planes found within the spatial mapping data.

**_SurfacePlanePrefab_** A GameObject that will be used for generating planes. If no prefab is provided, a Unity cube primitive will be used instead.

**MinArea** Minimum area required for a plane to be created.

**DrawPlanes** Bit mask which specifies the type of planes that should be rendered (walls, floors, ceilings, etc).

**DestroyPlanes** Bit mask which specifies the type of planes that should be discarded.

#### SurfacePlane.cs
Generates planes and classifies them by type (wall, ceiling, floor, table, unknown). Should be a component on the SurfacePlanePrefab used by SurfaceMeshesToPlanes.cs.

**PlaneThickness** How thick each plane should be.

**UpNormalThreshold** Threshold for acceptable normals. Used to determine if a plane is horizontal or vertical.

**FloorBuffer** Max distance from the largest floor plane before a horizontal plane will be classified as a table.

**CeilingBuffer** Max distance from the largest ceiling plane before a horizontal plane will be classified as a table.

**WallMaterial** Material to use when rendering wall plane types.

**FloorMaterial** Material to use when rendering ceiling plane types.

**TableMaterial** Material to use when rendering table plane types.

**UnknownMaterial** Material to use when rendering unknown plane types.

### [Shaders](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping/Shaders)

#### Occlusion.shader
A basic occlusion shader that can be used to occlude objects behind spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data. If you want to create an occlusion 'window', a better shader to use is WindowOcclusion.shader.

#### Wireframe.shader
A basic wire frame shader that can be used for rendering spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data.

### [Tests](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialMapping/Tests)

#### PlaneFinding.unity
To use this sample code, load the PlaneFinding scene and hit Play. The PlaneFinding algorithm will run in a loop. Switch to the scene view to see a visualization of the planes found.
The PlaneFindingTest component exposes a couple of properties that let you manipulate the PlaneFinding API parameters in real-time and observe their impact on the algorithm.

NOTE: In the interest of simplicity, this test script calls the PlaneFinding APIs directly from the main Unity thread in Update(). 
In a real application, the PlaneFinding APIs **MUST** be called from a background thread in order to avoid stalling the rendering thread and causing a drop in frame rate.

#### RemoteMapping.unity
The RemoteMapping scene uses the SpatialMapping and RemoteMapping prefabs to send spatial mapping data between the HoloLens and the app running in the Unity editor.
To run this test, you must first open port 11000 on your firewall and then set the IPv4 address of your PC in the 'RemoteMeshTarget' and 'RemoteMeshSource' components.
You can then build and deploy to the HoloLens. Once you see the wireframe mesh appear in your HoloLens, press the 'play' button in Unity to run the app in Editor. Ensure that the 'Game view' has focus, and then press the 'N' key (RemoteMappingKey) to switch to using the network as the spatial mapping source in the Editor.
Once you are confident that you have a good mesh, say the 'Send Meshes' (SendMeshesKeyword) to send the meshes from the HoloLens to the Unity Editor.
Press the 'S' key (SaveFileKey) to save the mesh to your PC. Press the 'play' button to stop the app from running in the Unity editor. Now, press 'play' one more time to restart the app. This time, press the 'L' key (LoadFileKey) to load the mesh that you previously saved into the Editor.

#### SpatialProcessing.unity
The SpatialProcessing scene tests the two processing scripts available in HoloToolkit: SufraceMeshesToPlanes and RemoveSurfaceVertices. 
If running in the Editor, the ObjectSurfaceObserver will load the SRMesh.obj file set in the SpatialMapping object of the scene. If you don't already have a file, you can capture one from the '3D View' page of the Windows Device Portal.  If running on the HoloLens, real-world surfaces will be scanned. After 15 seconds, the meshes will be converted to planes. If a floor plane is found, the test will remove vertices from surface meshes that fall within the bounds of any active plane.

#### TapToPlace.unity
This scene is the minimum setup to use the TapToPlace script.  It includes GazeManager, GestureManager, and SpatialMapping prefab.  BasicCursor prefab is included for ease of use. There is a cube in the scene with TapToPlace added on it. Gaze at and tap the cube.  It will move along the spatial mapping mesh based on user's gaze. While the cube is in 'placement' mode, the spatial mapping mesh will be visible. When tap is performed again, the cube will be placed on the mesh and the mesh will no longer be visible.

---
##### [Go back up to the table of contents.](https://github.com/Microsoft/HoloToolkit-Unity#holotoolkit-unity)
---

## [SpatialSound](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialSound)

### [Scripts](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/SpatialSound/Scripts)
Scripts related to the audio features.

**IMPORTANT**: Please make sure to set the MS HRTF Spatializer in your audio settings, in Unity under  
Edit -> Project Settings -> Audio -> Spatializer. You can confirm this setting by adding an AudioSource component to an object and making sure the "Spatialize" checkbox is present.

#### UAudioManager.cs
1. Allows sound designers to set up audio events with playback behaviors.
2. Plays audio events via singleton API.

**PlayEvent(string eventName)** Plays the event matching eventName on an AudioSource component placed on the GameObject containing the UAudioManager component.

**PlayEvent(string eventName, AudioSource primarySource)** Plays the event matching eventName on the AudioSource primarySource. This should be used if you already have an AudioSource component on which to play the sound, as opposed to the previous function, which will look for one or add it if there is no AudioSource present.

**Global Event Instance Limit** The total number of audio events that can be active at once at any given time.

**Global Instance Behavior** Whether the oldest or newest event should be cancelled to honor the instance limit.

**Name** The name of the audio event to be called in script.

**Positioning** Whether a sound should be played in stereo, 3D or using Spatial Sound.

**Room Size** The room model used for Spatial Sound.

**Min Gain** The lowest attenuation value caused by distance.

**Max Gain** The maximum level boost from the sound being closer than Unity Gain Distance.

**Unity Gain Distance** The distance, in meters, at which the sound is neither boosted nor attenuated.

---
##### [Go back up to the table of contents.](https://github.com/Microsoft/HoloToolkit-Unity#holotoolkit-unity)
---

## [Utilities](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Utilities)
Useful common concepts that you can leverage in your application.

### [Prefabs](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Utilities/Prefabs)
Common useful prefabs not particularly related to a particular HoloLens feature.

#### Main Camera.prefab
Unity camera that has been customized for Holographic development.
1. Camera.Transform set to 0,0,0
2. 'Clear Flags' changed to 'Solid Color'
3. Color set to R:0, G:0, B:0, A:0 as black renders transparent in HoloLens.
4. Set the recommended near clipping plane.

### [Scripts](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Utilities/Scripts)

#### Billboard.cs
Rotates a hologram so it is always facing towards the camera.

#### DirectionIndicator.cs
Show a GameObject around the cursor that points in the direction of the GameObject which this script is attached to.

You must provide GameObjects for the **_Cursor_** and **_DirectionIndicatorObject_** public fields.

**_Cursor_** The object in your scene that is being used as the cursor.  The direction indicator will be rendered around this cursor.

**_DirectionIndicatorObject_** The object that will point in the direction toward the object which this script is attached to.  This object can be a 2D or 3D object.

**DirectionIndicatorColor** The color you want the DirectionIndicatorObject to be.  The material on the DirectionIndicatorObject will need to support the color or TintColor property for this field to work.  Otherwise the DirectionIndicatorObject will continue to render as its exported color.

**TitleSafeFactor** The percentage the GameObject can be within the view frustum for the DirectionIndicatorObject to start appearing.  A value of 0 will display the DirectionIndicatorObject when the GameObject leaves the view.  0.1 will display when the GameObject is 10% away from the edge of the view.  -0.1 will display when the GameObject is 10% out of view.

#### FixedAngularSize.cs
Causes a hologram to maintain a fixed angular size, which is to say it occupies the same pixels in the view regardless of its distance from the camera.

#### Interpolator.cs
A MonoBehaviour that interpolates a transform's position, rotation or scale.

#### ManualCameraControl.cs
A script to add to the main camera object so that when running in Play mode in Unity, the user can control the camera using keyboard and mouse.

#### SimpleTagalong.cs
A Tagalong that stays at a fixed distance from the camera and always seeks to have a part of itself in the view frustum of the camera.

#### Singleton.cs
A base class to make a MonoBehaviour follow the singleton design pattern.

#### Tagalong.cs
A Tagalong that extends SimpleTagalong that allows for specifying the minimum and target percentage of the object to keep in the view frustum of the camera and that keeps the Tagalong object in front of other holograms including the Spatial Mapping Mesh.

#### TextToSpeechManager.cs
Provides dynamic Text to Speech. Speech is generated using the UWP SpeechSynthesizer and then played through a Unity AudioSource. Both plain text and SSML are supported.   

### [Shaders](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Utilities/Shaders)

#### LambertianConfigurable.cginc
Code shared between LambertianConfigurable.shader and LambertianConfigurableTransparent.shader.

#### LambertianConfigurable.shader
Feature configurable per-pixel lambertian shader.  Use when higher quality lighting is desired, but specular highlights are not needed.

#### LambertianConfigurableTransparent.shader
Feature configurable per-pixel lambertian transparent shader.  Use when higher quality lighting and transparency are desired, but specular highlights are not needed.

#### StandardFast.shader
Higher performance drop-in replacement for the Unity Standard Shader.  Use when very high quality lighting (including reflections) is needed.

#### UnlitConfigurable.cginc
Code shared between UnlitConfigurable.shader and UnlitConfigurableTransparent.shader.

#### UnlitConfigurable.shader
Feature configurable unlit shader.  Use when no lighting is desired.

#### UnlitConfigurableTransparent.shader
Feature configurable unlit transparent shader.  Use when transparency and no lighting are desired.

#### VertexLitConfigurable.cginc
Code shared between VertexLitConfigurable.shader and VertexLitConfigurableTransparent.shader.

#### VertexLitConfigurable.shader
Feature configurable vertex lit shader.  Use when a higher performance but lower precision lighting trade-off is acceptable.

#### VertexLitConfigurableTransparent.shader
Feature configurable vertex lit transparent shader.  Use when a higher performance but lower precision lighting trade-off is acceptable, and transparency is needed.

#### WindowOcclusion.shader
A simple occlusion shader that can be used to hide other objects. This prevents other objects from being rendered by drawing invisible 'opaque' pixels to the depth buffer. This shader differs from Occlusion.shader in that it doesn't have any depth offset, so it should sort as expected with other objects adjacent to the window.

### [Tests](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Utilities/Tests)

#### ManualCameraControl.unity

This scene shows how to use ManualCameraControl.cs.  The script is on the main camera of the scene.  When preview mode in Unity is activated, the user can move around the scene using WASD and look around using ctrl + mouse. 

#### TextToSpeechManager.unity 

This scene demonstrates how to use TextToSpeechManager.cs.  The script is placed on 3 cubes in the scene. Whenever a cube is activated with an air tap, a text to speech voice will emanate from the cube. The user can also ask "What time is it?" to hear the current time from a voice that stays with the user as they move.

#### WindowOcclusion.unity 

This scene demonstrates how to use WindowOcclusion.shader.  It positions a virtual 'window' directly in front of you when the scene starts. A cube in the back is only visible when viewed through the window because quads around the window use the WindowOcclusion shader.

---
##### [Go back up to the table of contents.](https://github.com/Microsoft/HoloToolkit-Unity#holotoolkit-unity)
---

## [Build](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Build)
Build and deploy automation window for building your VS solution, APPX, installing, launching, and getting the log file (and other related functionality). Requires that the device has been paired with the Editor PC & that the device is connected locally and/or the HTTPS requirement has been disabled in the device portal's security tab.

### [Scripts](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit/Build/Scripts)

#### BuildDeployPortal.cs
Interface function with the device (REST API utility functions)

#### BuildDeployTools.cs
Supports building the APPX from the SLN

#### BuildSLNUtilities.cs
Supports building the project SLN

#### BuildDeployWindow.cs
Editor UI for the window and event functions

---
##### [Go back up to the table of contents.](https://github.com/Microsoft/HoloToolkit-Unity#holotoolkit-unity)
---
