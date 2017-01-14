## [SpatialMapping]()
Scripts that leverage SpatialMapping related features.

1. Enable the "SpatialPerception" capability in Player Settings -> Windows Store -> Publishing Settings -> Capabilities.
2. If using the RemoteMapping components, you will also need to set the InternetClientServer, PrivateNetworkClientServer, and Microphone capabilities. 

**IMPORTANT**: Please make sure to add the Spatial Perception capability in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

### [Plugins](Plugins)
PlaneFinding addon that can be used to find planar surfaces (ie: walls/floors/tables/etc) in the mesh data returned by Spatial Mapping.

### [Prefabs](Prefabs)

The following prefabs make it easy to quickly access and visualize spatial mapping data in the HoloLens or in the Unity Editor.

#### RemoteMapping.prefab
Use with SpatialMapping prefab, it allows you to send meshes from the HoloLens to Unity and save/load the meshes for use later.

#### SpatialMapping.prefab
Base prefab which allows you to visualize and access spatial mapping data on the HoloLens. It can also save/load room models that were captured from the Windows Device Portal.

#### SurfacePlane.prefab
Helper prefab which should be referenced by the SurfaceMeshesToPlanes component for classifying planes as floor, ceiling, wall, etc during processing.

### [Scripts](Scripts)

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
Simple extendable script to add to a GameObject that allows users to tap and place the GameObject along the spatial mapping mesh. 

TapToPlace also allows the user to specify a parent GameObject to move along with the selected GameObject.

Requires GazeManager, GestureManager, and SpatialMappingManager in the scene.

### [Scripts\RemoteMapping](Scripts/RemoteMapping)

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

### [Scripts\SpatialProcessing](Scripts)

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

### [Shaders](Shaders)

#### Occlusion.shader
A basic occlusion shader that can be used to occlude objects behind spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data. If you want to create an occlusion 'window', a better shader to use is WindowOcclusion.shader.

#### Wireframe.shader
A basic wire frame shader that can be used for rendering spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data.

#### SpatialMappingTap.shader
Draws a ring originating from a location in space. Useful for showing where a user tapped. Requires a component to drive it's radius and set the tap location in world space.

### [Tests Scenes](Tests/Scenes)

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
##### [Go back up to the table of contents.](../../../README.md)
---
