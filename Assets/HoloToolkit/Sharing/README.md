## [Sharing]()
Sharing and networking components for rapid prototyping in Unity for building shared experiences.

Ensure you have the following capabilities set:
1. Enable the "SpatialPerception" capability in Player Settings -> Windows Store -> Publishing Settings -> Capabilities.
2. For using Sharing components, you will also need to set the InternetClientServer, PrivateNetworkClientServer, and Microphone capabilities.

### [Editor](Editor)
Enables the HoloToolkit menu option in the Unity top tool bar.

### [Plugins](Plugins)
Contains compiled architecture specific binaries for SharingClient.dll which are required by the Unity application for accessing sharing APIs.
Binaries are compiled from the native [HoloToolkit\Sharing](https://github.com/Microsoft/HoloToolkit/tree/master/Sharing).

### [Prefabs](Prefabs)
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

### [Scripts](Scripts)
Scripts related to the sharing and networking features.

#### SDK
Contains scripts compiled from the native [HoloToolkit\Sharing](https://github.com/Microsoft/HoloToolkit/tree/master/Sharing) repository and using the SWIG tool to generate different language bindings.

#### Utilities
Utility scripts for the Sharing.prefab.
Also scripts for logging, launching processes, math utilities.

### [Tests](Tests)
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
##### [Go back up to the table of contents.](../../../README.md)
---