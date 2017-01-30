## [Sharing]()
Sharing and networking components for rapid prototyping in Unity for building shared experiences.

Ensure you have the following capabilities set in Player Settings -> Windows Store -> Publishing Settings -> Capabilities:

1. SpatialPerception
2. InternetClientServer
3. PrivateNetworkClientServer
4. Microphone capabilities

### [Plugins](Plugins)
---
Contains compiled architecture specific binaries for SharingClient.dll which are required by the Unity application for accessing sharing APIs.
Binaries are compiled from the native [HoloToolkit\Sharing](https://github.com/Microsoft/HoloToolkit/tree/master/Sharing).

### [Prefabs](Prefabs)
---
Prefabs related to the sharing and networking features.

#### Sharing.prefab
1. Enables sharing and networking in your Unity application.
2. Allows you to communicate between a Windows and non-Windows device.

**SharingStage.cs** allows you to be a Primary Client (typical case).
**Server Address** is the IP address of the machine running the HoloToolkit -> Launch Sharing Service.
**Server Port** displays the port being used for communicating.

**AutoJoinSession.cs** creates a shared session with Session Name 'Default' which is customizable.
Joins a player to that session if once already exists.

### [Scripts](Scripts)
---
Scripts related to the sharing and networking features.

#### [Editor](Scripts/Editor)
---
Scripts for in editor use only.

##### SharingMenu.cs
Enables users to start the Sharing Service, Sharing Manager, and Profiler from the Unity Editor via the HoloToolkit Menu.

##### SharingStageEditor.cs
Draws the default Sharing Stage Inspector and adds the SyncRoot Hierarchy view so users can quickly verify Sync Object updates.

#### [SDK](Scripts/SDK)
---
Contains scripts compiled from the native [HoloToolkit\Sharing](https://github.com/Microsoft/HoloToolkit/tree/master/Sharing) repository and using the SWIG tool to generate different language bindings.

#### [Spawning](Scripts/Spawning)
---
Scripts for spawning objects using the sharing service.

##### PrefabSpawnerManager.cs
Spawn manager that creates a GameObject based on a prefab when a new SyncSpawnedObject is created in the data model.

##### SpawnManager.cs
A SpawnManager is in charge of spawning the appropriate objects based on changes to an array of data model objects to which it is registered. It also manages the lifespan of these spawned objects.

##### SyncSpawnArray.cs
This array is meant to hold SyncSpawnedObject and objects of subclasses. Compared to SyncArray, this supports dynamic types for objects.

##### SyncSpawnedObject.cs
A SpawnedObject contains all the information needed for another device to spawn an object in the same location as where it was originally created on this device.

#### [SyncModel](Scripts/SyncModel)
---
Scripts for syncing data over sharing service.

##### SyncArray.cs
The SyncArray class provides the functionality of an array in the data model. The array holds entire objects, not primitives, since each object is indexed by unique name. Note that this array is unordered.

##### SyncBool.cs
This class implements the boolean primitive for the syncing system.  It does the heavy lifting to make adding new bools to a class easy.

##### SyncDataAttributes.cs
Used to markup SyncObject classes and SyncPrimitives inside those classes, so that they properly get instantiated when using a hierarchical data model that inherits from SyncObject.

##### SyncDouble.cs
This class implements the double primitive for the syncing system.  It does the heavy lifting to make adding new doubles to a class easy.

##### SyncFloat.cs
This class implements the float primitive for the syncing system.  It does the heavy lifting to make adding new floats to a class easy.

##### SyncInteger.cs
This class implements the integer primitive for the syncing system.  It does the heavy lifting to make adding new integers to a class easy.

##### SyncLong.cs
This class implements the long primitive for the syncing system.  It does the heavy lifting to make adding new longs to a class easy.

##### SyncObject.cs
The SyncObject class is a container object that can hold multiple SyncPrimitives.

##### SyncPrimitive.cs
Base primitive used to define an element within the data model.  The primitive is defined by a field and a value.

##### SyncQuaternion.cs
This class implements the Quaternion object primitive for the syncing system.  It does the heavy lifting to make adding new Quaternion to a class easy.

##### SyncString.cs
This class implements the string primitive for the syncing system.  It does the heavy lifting to make adding new strings to a class easy.

##### SyncTransform.cs
This class implements the Transform object primitive for the syncing system.  It does the heavy lifting to make adding new transforms to a class easy.  A transform defines the position, rotation and scale of an object.

##### SyncVector3.cs
This class implements the Vector3 object primitive for the syncing system.  It does the heavy lifting to make adding new Vector3 to a class easy.

#### [Unity](Scripts/Unity)
---
Scripts used to implement unity specific sync services.

##### DefaultSyncModelAccessor.cs
Default implementation of a behaviour that allows other components of a game object access the shared data model as a raw SyncObject instance.

##### ISyncModelSccessor.cs
Interface that allows a components of a game object access the shared data model set by a SpawnManager.

##### TransformSynchronizer.cs
Synchronizer to update and broadcast a transform object through our data model.

#### [Utilities](Scripts/Utilities)
---
Scripts for sync service utilities.

##### AutoJoinSession.cs
Utility class for automatically joining shared sessions without needing to go through a lobby.

##### ConsoleLogWriter.cs
Utility class that writes the sharing service log messages to the Unity Engine's console.

##### DirectPairing.cs
This class enables users to pair with a remote client directly.  One side should use the Receiver role, the other side should use the Connector role.  RemoteAddress and RemotePort are used by the Connector role, LocalPort is used by the Receiver.

#### [VoiceChat](Scripts/VoiceChat)
---
Scripts for Voice Chat service.

##### MicrophoneReceiver.cs
Receives and plays voice data transmitted through the session server. This data comes from other clients running the MicrophoneTransmitter behaviour.

##### MicrophoneTransmitter.cs
Transmits data from your microphone to other clients connected to a SessionServer. Requires any receiving client to be running the MicrophoneReceiver script.

---

#### ServerSessionTracker.cs
The ServerSessionsTracker manages the list of sessions on the server and the users in these sessions.  Instance is created by Sharing Stage when a connection is found.

#### SessionUsersTracker.cs
Keeps track of the users in the current session.  Instance is created by Sharing Stage when a connection is found.

#### SharingStage.cs
he SharingStage is in charge of managing the core networking layer for the application.

#### SyncRoot.cs
Root of the synchronization data model used by this application.

#### SyncSettings.cs
Collection of sharing sync settings, used by the HoloToolkit Sharing sync system to figure out which data model classes need to be instantiated when receiving data that inherits from SyncObject.

#### SyncStateListener.cs
C# wrapper for the Sharing SyncListener, making changes available through the Action class.

### [Test Prefabs](Tests/Prefabs)
---
Prefabs used in the various test scenes, which you can use as inspiration to build your own.

#### SpawnTestCube.prefab
Simple Cube prefab with a Transform, Mesh Filter, Box Collider, Mesh Renderer, and Default Sync Model Accessor components.

#### SpawnTestSphere.prefab
A simple Sphere prefab with a Transform, Mesh Filter, Sphere Collider, and Mesh Renderer components.
Purposefully missing Default Sync Model Accessor component for SharingSpawnTest.

### [Test Scripts](Tests/Scripts)
---
Test Scripts.

#### CustomMessages.cs
Test class for demonstrating how to send custom messages between clients.

#### ImportExportAnchorManager.cs
Manages creating anchors and sharing the anchors with other clients.

#### RemoteHeadManager.cs
Broadcasts the head transform of the local user to other users in the session, and adds and updates the head transforms of remote users.  Head transforms are sent and received in the local coordinate space of the GameObject this component is on.  

#### RoomTest.cs
Test class for demonstrating creating rooms and anchors.

#### SpawnTestKeyboardSpawning.cs
Class that handles spawning sync objects on keyboard presses, for the `SpawningTest.scene`.

#### SyncSpawnTestSphere.cs
Class that demonstrates a custom class using sync model attributes.

### [Tests](Tests/Scenes)
---
Tests related to the sharing features. To use the scene:

1. Navigate to the Tests folder.
2. Double click on the test scene you wish to explore.
3. Either click "Play" in the unity editor or File -> Build Settings.
4. Add Open Scenes, Platform -> Windows Store, SDK -> Universal 10, Build Type -> D3D, Check 'Unity C# Projects'.
5. Click 'Build' and create an App folder. When compile is done, open the solution and deploy to device.

#### SharingTest.unity 
This test demonstrates how to use the Sharing prefabs for networking and sharing custom messages with clients. 
It also demonstrates how to share world anchors between clients to establish a shared space.

1. Ensure to launch the sharing service using: HoloToolkit -> Launch Sharing service
2. Enter the IP address displayed in the console window into the Server Address of the Sharing object.
3. **CustomMessages.cs** shows how to communicate specific information across clients.
4. **ImportExportAnchorManager.cs** shows how to create anchors and share them with other clients using the sharing service.
5. **RemoteHeadManager.cs** draw cubes on remote heads of users joining the session.

#### RoomAndAnchorTest.unity
This test demonstrates how to create new rooms and anchors inside your application.
It also demonstrates how to upload and download new anchors.

1. Ensure to launch the sharing service using: HoloToolkit -> Launch Sharing service
2. Enter the IP address displayed in the console window into the Server Address of the Sharing object.
3. **RoomTest.cs** shows how to create, join, and leave rooms; also shows how to create and download anchors.

#### SharingSpawnTest.unity
This test demonstrates how to spawn sync objects in your scene and across your networked clients.

1. Ensure to launch the sharing service using: HoloToolkit -> Launch Sharing service
2. Enter the IP address displayed in the console window into the Server Address of the Sharing object.
3. **PrefabSpawnManager.cs** enables you to store prefab references to use when spawning.
4. **SpawnTestKeyboardSpawning** demonstrates how to spawn sync objects, as well as custom class types.

---
##### [Go back up to the table of contents.](../../../README.md)
---
