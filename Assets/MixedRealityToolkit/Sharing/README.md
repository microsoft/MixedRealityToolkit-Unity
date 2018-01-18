## [Sharing]()
The MixedRealityToolkit.Sharing library allows applications to span multiple devices, and enables holographic collaboration.  

Originally developed for OnSight, a collaboration between SOTA (a Microsoft studio) and NASA to enhance their existing Mars rover planning tool with HoloLens, MixedRealityToolkit.Sharing enables users to use multiple devices for a task by allowing the apps running on each device communicate and stay in sync seamlessly in real time.  

Users can also collaborate with other users (who are also using multiple devices) who may be in the same room or working remotely.

## Table of Contents

- [Features](#features)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)
- [Plugins](#plugins)
- [Prefabs](#prefabs)
- [Scripts](#scripts)
- [Test Prefabs](#test-prefabs)
- [Test Scripts](#test-scripts)
- [Tests](#tests)

### Features
---

#### Lobby & Session system
* Discover available sessions or create your own
* Permanent or ‘until empty’ session lifetime
* See user status: current session, mute state
* Easy to discover and hop between sessions

#### Anchor Sharing
* Users in a session can be in the same or different physical rooms
* Users can share the location of Holographic 'anchors' they place in their room with other users in the same room
* Users joining late can download all anchors in the session
* Allows multiple users to see shared holograms

#### Synchronization System
Synchronize data across all participants in session
* Everyone in session guaranteed to see the same thing
* Automatic conflict resolution for simultaneous conflicting remote changes
* Real-time: See remote changes as they happen
* Shared data sets automatically merged when new users join a session
* Responsive: no delay in your own changes
* Ownership: your data leaves session when you do

#### Group Voice Chat
Support for VOIP is built-in
* Server-based mixing lowers processing and bandwidth requirements for clients

#### Visual Pairing
Connect devices just by looking at them
* One device displays a QR code with connection info and security code
* Other device sees QR code, connects, and validates with security code
* Can also detect location in 3D space using built-in fiducial marker support

#### Profiler
Profiling and debugging an experience that spans multiple devices is challenging.  So MixedRealityToolkit.Sharing provides an app that can connect to multiple devices at once and aggregates their timings and debug output in a single place

#### Sync Model

MixedRealityToolkit.Sharing has the ability to synchronize data across any application connected to a given session. Conflict resolution is automatically handled by the framework, at whichever level the conflict occurs.

##### Primitive Types
The following primitives are natively supported by the sync system. The C# class that corresponds to each primitive is written in parentheses.

- Boolean (SyncBool)
- Double (SyncDouble)
- Float (SyncFloat)
- Integer (SyncInteger)
- Long (SyncLong)
- Object, which is a container class that can have child primitives (SyncObject
- String (SyncString)

On top of the native primitives above, the following types are supported in the C# layer:

- Quaternion (SyncQuaternion)
- Transform (SyncTransform)
- Unordered array (SyncArray)
- Vector3 (SyncVector3)

Other types can be built for your own application as needed by inheriting from SyncObject in a similar way to what SyncVector3 and SyncTransform do.

##### Defining the Sync Model
By default, the SyncRoot object (which inherits from SyncObject) only contains an array of InstantiatedPrefabs, which may not be enough for your application.

For any type inheriting from SyncObject, you can easily add new children primitives by using the SyncData attribute, such as in the following example:


	public class MySyncObject : SyncObject
	{
	    [SyncData]
	    public SyncSpawnArray<MyOtherSyncObject> OtherSyncObject;

		[SyncData]
		public SyncFloat FloatValue;
	}

Any SyncPrimitive tagged with the [SyncData] attribute will automatically be added to the data model and synced in the current MixedRealityToolkit.Sharing session.

### Configuration
---
Ensure you have the Sharing Service Feature enabled in `MixedRealityToolkit -> Configure -> Apply HoloLens Project Settings`.

Enabling the Sharing Service will also enable these UWP capabilities:

1. InternetClientServer
2. PrivateNetworkClientServer

Enabling the Sharing Service will also uppack a new directory in your projects root folder named `External`.

To run the Sharing Service `MixedRealityToolkit -> Launch Sharing Service`.
This will create a new instance of the server on your machine.

For a production environment, follow the instructions from the main MixedRealityToolkit: [Running the Server](../../../External/MixedRealityToolkit/Sharing/DocSource/MDFiles/GettingStarted.md#running-the-server).

### Troubleshooting
---
- Double check the Server Address on your sharing stage component in your scene matches the address shown in the sharing service console.
- Make sure all devices are connected to the same Wireless Local Area Network.
- Ensure all firewall settings are correct.  Windows firewall gives you options to enable/disable by network type (private, public, home), make sure you're enabling the firewall for your connection's type.

#### Invalid Schema Version

```
SharingService [..\..\Source\Common\Private\SessionListHandshakeLogic.cpp (67)]: 
***************************************************************
List Server Handshake Failed: Invalid schema version. 
Expected: 17, got 15 
Please sync to latest XTools
***************************************************************
```

- Ensure you're using the latest binaries of the sharing service found at `MixedRealityToolkit-Unity\External\MixedRealityToolkit\Sharing\Server`.

### [Plugins](Plugins)
---
Contains compiled architecture specific binaries for SharingClient.dll which are required by the Unity application for accessing sharing APIs.
Binaries are compiled from the native [MixedRealityToolkit\Sharing](https://github.com/Microsoft/MixedRealityToolkit/tree/master/Sharing).

### [Prefabs](Prefabs)
---
Prefabs related to the sharing and networking features.

#### Sharing.prefab
1. Enables sharing and networking in your Unity application.
2. Allows you to communicate between a Windows and non-Windows device.

**SharingStage.cs** allows you to be a Primary Client (typical case).
**Server Address** is the IP address of the machine running the MixedRealityToolkit -> Launch Sharing Service.
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
Enables users to start the Sharing Service, Sharing Manager, and Profiler from the Unity Editor via the MixedRealityToolkit Menu.

##### SharingStageEditor.cs
Draws the default Sharing Stage Inspector and adds the SyncRoot Hierarchy view so users can quickly verify Sync Object updates.

#### [SDK](Scripts/SDK)
---
Contains scripts compiled from the native [MixedRealityToolkit\Sharing](https://github.com/Microsoft/MixedRealityToolkit/tree/master/Sharing) repository and using the SWIG tool to generate different language bindings.

#### [Spawning](Scripts/Spawning)
---
Scripts for spawning objects using the sharing service.

##### PrefabSpawnerManager.cs
A SpawnManager that creates a GameObject based on a prefab when a new SyncSpawnedObject is created in the data model. This class can spawn prefabs in reaction to the addition/removal of any object that inherits from SyncSpawnedObject, thus allowing applications to dynamically spawn prefabs as needed.

The PrefabSpawnManager registers to the SyncArray of InstantiatedPrefabs in the SyncRoot object.

The various classes can be linked to a prefab from the editor by specifying which class corresponds to which prefab. Note that the class field is currently a string that has to be typed in manually ("SyncSpawnedObject", for example): this could eventually be improved through a custom editor script.

##### SpawnManager.cs
A SpawnManager is in charge of spawning the appropriate objects based on changes to an array of data model objects. It also manages the lifespan of these spawned objects.

This is an abstract class from which you can derive a custom SpawnManager to react to specific synchronized objects being added or removed from the data model.

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
Default implementation of a MonoBehaviour that allows other components of a game object access the shared data model as a raw SyncObject instance.

##### ISyncModelSccessor.cs
Interface that allows a components of a game object access the shared data model set by a SpawnManager.

##### TransformSynchronizer.cs
Synchronizer to update and broadcast a transform object through our data model.

#### [Utilities](Scripts/Utilities)
---
Scripts for sync service utilities.

##### AutoJoinSession.cs
A MonoBehaviour component that allows the application to automatically join the specified session as soon as the application has a valid server connection. This class will also maintain the session connection throughout the application lifetime.

This is mostly meant to be used to quickly test networking in an application. In most cases, some session management code should be written to allow users to join/leave sessions according to the desired application flow.

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
The ServerSessionsTracker is in charge of listing the various sessions that exist on the server, and exposes events related to all of these sessions. This is also the class that allows the application to join or leave a session.  Instance is created by Sharing Stage when a connection is found.

#### SessionUsersTracker.cs
The SessionUsersTracker keeps track of the current session and its users. It also exposes events that are triggered whenever users join or leave the current session.  Instance is created by Sharing Stage when a connection is found.

#### SharingStage.cs
A Singleton MonoBehaviour that is in charge of managing the core networking layer for the application. The SharingStage has the following responsibilities:

- Server configuration (address, port and client role)
- Establish and manage the server connection
- Create and initialize the synchronized data model (SyncRoot)
- Create the ServerSessionsTracker that tracks all sessions on the server
- Create the SessionUsersTracker that tracks the users in the current session

#### SyncRoot.cs
Root of the synchronized data model, under which every element of the model should be located. The SharingStage will automatically create and initialize the SyncRoot at application startup.

#### SyncSettings.cs
Collection of sharing sync settings, used by the MixedRealityToolkit Sharing sync system to figure out which data model classes need to be instantiated when receiving data that inherits from SyncObject.

#### SyncStateListener.cs
C# wrapper for the Sharing SyncListener, making changes available through the Action class.

### [Test Prefabs](../../MixedRealityToolkit-Examples/Sharing/Prefabs)
---
Prefabs used in the various test scenes, which you can use as inspiration to build your own.

#### SpawnTestCube.prefab
Simple Cube prefab with a Transform, Mesh Filter, Box Collider, Mesh Renderer, and Default Sync Model Accessor components.

#### SpawnTestSphere.prefab
A simple Sphere prefab with a Transform, Mesh Filter, Sphere Collider, and Mesh Renderer components.
Purposefully missing Default Sync Model Accessor component for SharingSpawnTest.

### [Test Scripts](../../MixedRealityToolkit-Examples/Sharing/Scripts)
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

#### SyncObjectSpawner.cs
Class that handles spawning and deleting sync objects for the `SpawningTest.scene`.  Uses the `KeywordManager` to spawn objects using voice and keyboard input.

|Voice Command|Key Command|Description|
|---|---|---|
| Spawn Basic | Key `I`| Spawns a cube with a `SyncSpawnedObject` basic sync model.|
| Spawn Custom | Key `O`| Spawns a sphere with a `SyncSpawnTestSphere` custom sync model.|
| Delete Object | Key `M`| Deletes both sync model types.|

#### SyncSpawnTestSphere.cs
Class that demonstrates a custom class using sync model attributes.

#### UserNotifications.cs
Used to demonstrate how to get notifications when users leave and enter room.

### [Tests](../../MixedRealityToolkit-Examples/Sharing/Scenes)
---
Tests related to the sharing features. To use the each scene:

1. Navigate to the Tests folder.
2. Double click on the test scene you wish to explore.
3. Either click "Play" in the unity editor or File -> Build Settings.
4. Add Open Scenes, Platform -> Windows Store, SDK -> Universal 10, Build Type -> D3D, Check 'Unity C# Projects'.
5. Enable all required [capabilities](configuration).
6. Click 'Build' and create an App folder. When compile is done, open the solution and deploy to device.

#### SharingTest.unity 
This test demonstrates how to use the Sharing prefabs for networking and sharing custom messages with clients. 
It also demonstrates how to share world anchors between clients to establish a shared space.

1. Ensure to launch the sharing service using: MixedRealityToolkit -> Launch Sharing service
2. Enter the IP address displayed in the console window into the Server Address of the Sharing object.
3. **CustomMessages.cs** shows how to communicate specific information across clients.
4. **ImportExportAnchorManager.cs** shows how to create anchors and share them with other clients using the sharing service.
5. **RemoteHeadManager.cs** draw cubes on remote heads of users joining the session.

#### RoomAndAnchorTest.unity
This test demonstrates how to create new rooms and anchors inside your application.
It also demonstrates how to upload and download new anchors.

1. Ensure to launch the sharing service using: MixedRealityToolkit -> Launch Sharing service
2. Enter the IP address displayed in the console window into the Server Address of the Sharing object.
3. **RoomTest.cs** shows how to create, join, and leave rooms; also shows how to create and download anchors.

#### SharingSpawnTest.unity
This test demonstrates how to spawn and delete sync objects in your scene and across your networked clients.

1. Ensure to launch the sharing service using: MixedRealityToolkit -> Launch Sharing service
2. Enter the IP address displayed in the console window into the Server Address of the Sharing object.
3. **PrefabSpawnManager.cs** enables you to store prefab references to use when spawning.
4. **SyncObjectSpawner.cs** demonstrates how to spawn and delete sync objects, as well as custom class types.

---
##### [Go back up to the table of contents](#table-of-contents)
##### [Go back to the main page.](../../../README.md)
---
