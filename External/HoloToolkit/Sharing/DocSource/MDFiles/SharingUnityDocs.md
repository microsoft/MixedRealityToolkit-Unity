Sharing Unity Integration
============

## Description
The Sharing Unity integration is meant to bridge the sharing library provided for HoloLens applications with Unity. It provides convenient C# scripts that allow any application to quickly add networking capabilities such as:

- Spawning game objects
- Deleting game objects
- Syncing data across devices
- Syncing game object transforms in real time

## Architecture

The Unity integration uses an architecture similar to MVC (model-view-controller). The application can define its object model through the various Sync* classes, and various components then allow you to react to object model changes in order for your application to react appropriately.

## Major Components

### SharingStage.cs
A Singleton behaviour that is in charge of managing the core networking layer for the application. The SharingStage has the following responsibilities:

- Server configuration (address, port and client role)
- Establish and manage the server connection
- Create and initialize the synchronized data model (SyncRoot)
- Create the ServerSessionsTracker that tracks all sessions on the server
- Create the SessionUsersTracker that tracks the users in the current session

### AutoJoinSession.cs
A behaviour component that allows the application to automatically join the specified session as soon as the application has a valid server connection. This class will also maintain the session connection throughout the application lifetime.

This is mostly meant to be used to quickly test networking in an application. In most cases, some session management code should be written to allow users to join/leave sessions according to the desired application flow. 

### SessionUsersTracker.cs
The SessionUsersTracker keeps track of the current session and its users. It also exposes events that are triggered whenever users join or leave the current session.

### ServerSessionsTracker.cs
The ServerSessionsTracker is in charge of listing the various sessions that exist on the server, and exposes events related to all of these sessions. This is also the class that allows the application to join or leave a session.

### SyncRoot
Root of the synchronized data model, under which every element of the model should be located. The SharingStage will automatically create and initialize the SyncRoot at application startup.

### SpawnManager
A SpawnManager is in charge of spawning the appropriate objects based on changes to an array of data model objects. It also manages the lifespan of these spawned objects.

This is an abstract class from which you can derive a custom SpawnManager to react to specific synchronized objects being added or removed from the data model.

### PrefabSpawnManager
A SpawnManager that creates a GameObject based on a prefab when a new SyncSpawnedObject is created in the data model. This class can spawn prefabs in reaction to the addition/removal of any object that inherits from SyncSpawnedObject, thus allowing applications to dynamically spawn prefabs as needed.

The PrefabSpawnManager registers to the SyncArray of InstantiatedPrefabs in the SyncRoot object.

The various classes can be linked to a prefab from the editor by specifying which class corresponds to which prefab. Note that the class field is currently a string that has to be typed in manually ("SyncSpawnedObject", for example): this could eventually be improved through a custom editor script.


## Sync Model

HoloToolkit.Sharing has the ability to synchronize data across any application connected to a given session. Conflict resolution is automatically handled by the framework, at whichever level the conflict occurs.

### Primitive Types
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

### Defining the Sync Model
By default, the SyncRoot object (which inherits from SyncObject) only contains an array of InstantiatedPrefabs, which may not be enough for your application.

For any type inheriting from SyncObject, you can easily add new children primitives by using the SyncData attribute, such as in the following example:


	public class MySyncObject : SyncObject
	{
	    [SyncData]
	    public SyncSpawnArray<MyOtherSyncObject> OtherSyncObject;

		[SyncData]
		public SyncFloat FloatValue;
	}

Any SyncPrimitive tagged with the [SyncData] attribute will automatically be added to the data model and synced in the current HoloToolkit.Sharing session.