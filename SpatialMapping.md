## [SpatialMapping](Assets/HoloToolkit/SpatialMapping)
Scripts that leverage SpatialMapping related features.

1. Enable the "SpatialPerception" capability in Player Settings -> Windows Store -> Publishing Settings -> Capabilities.
2. If using the RemoteMapping components, you will also need to set the InternetClientServer, PrivateNetworkClientServer, and Microphone capabilities. 

**IMPORTANT**: Please make sure to add the Spatial Perception capability in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

PlaneFinding addon that can be used to find planar surfaces (ie: walls/floors/tables/etc) in the mesh data returned by Spatial Mapping.

### [Scripts](Assets/HoloToolkit/SpatialMapping/Scripts)

The following scripts give you the ability to load the spatial mapping mesh from a file or the network into your Unity scene for debugging in the editor.

#### FileSurfaceObserver.cs
A SpatialMappingSource that loads spatial mapping data from a file in Unity.

**MeshFileName** Name of file to use when saving or loading surface mesh data.

#### MeshSaver.cs
Static class that can read and write mesh data to the file specified in FileSurfaceObserver.cs.

#### PlaneFinding.cs
Unity script that wraps the native PlaneFinding DLL. Used by SurfaceMeshesToPlanes.cs.

#### RemoteMappingManager.cs
Allows sending meshes remotely from HoloLens to Unity.

#### RemoteMeshSource.cs
Networking component that runs on the HoloLens and can send meshes to Unity.

**ServerIP** The IPv4 address of the machine running the Unity editor.

**ConnectionPort** The network port of the Unity machine that will receive spatial mapping data from the HoloLens.

#### RemoteMeshTarget.cs
SpatialMappingSource object that runs in the Unity editor and receive spatial mapping data from the HoloLens.

**ServerIP** The IPv4 address of the machine running the Unity editor.

**ConnectionPort** The network port of the Unity machine that will receive mesh data from the HoloLens.

#### RemoveSurfaceVertices.cs
A spatial processing component that will remove any spatial mapping vertices that fall within the specified bounding volumes.

**BoundsExpansion** The amount, if any, to expand each bounding volume by.

#### SimpleMeshSerializer.cs
Static class that converts a Unity mesh to an array of bytes. Used by MeshSaver.cs to serialize and deserialize mesh data.

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

#### TapToPlace.cs
Simple script to add to a GameObject that allows users to tap and place the GameObject along the spatial mapping mesh.
Requires GazeManager, GestureManager, and SpatialMappingManager in the scene.

### [Shaders](Assets/HoloToolkit/SpatialMapping/Shaders)

#### Occlusion.shader
A basic occlusion shader that can be used to occlude objects behind spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data. If you want to create an occlusion 'window', a better shader to use is WindowOcclusion.shader.

#### Wireframe.shader
A basic wire frame shader that can be used for rendering spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data.

### [Tests](Assets/HoloToolkit/SpatialMapping/Tests)

#### PlaneFinding.unity
To use this sample code, load the PlaneFinding scene and hit Play. The PlaneFinding algorithm will run in a loop. Switch to the scene view to see a visualization of the planes found.
The PlaneFindingTest component exposes a couple of properties that let you manipulate the PlaneFinding API parameters in real-time and observe their impact on the algorithm.

NOTE: In the interest of simplicity, this test script calls the PlaneFinding APIs directly from the main Unity thread in Update(). 
In a real application, the PlaneFinding APIs **MUST** be called from a background thread in order to avoid stalling the rendering thread and causing a drop in frame rate.

#### SpatialProcessing.unity
The SpatialProcessing scene tests the two processing scripts available in HoloToolkit: SufraceMeshesToPlanes and RemoveSurfaceVertices. 
If you already have a .room file saved, it will automatically load the file and run in Unity. 
If not, you can use the RemoteMapping prefab to send/save mesh files from the HoloLens. You can also run this test directly in the HoloLens.
This scene will scan your area for 15 seconds and then convert all meshes to planes. If a floor plane is found, it will remove vertices from surface meshes that fall within the bounds of any active plane.

#### TapToPlace.unity
This scene is the minimum setup to use the TapToPlace script.  It includes GazeManager, GestureManager, and SpatialMapping prefab.  BasicCursor prefab is included for ease of use.
There is a cube in the scene with TapToPlace added on it.  Gaze at and tap the cube.  It will move along the spatial mapping mesh based on user's gaze.  
When tap is performed again, it will place the cube.


---
##### [Go back up to the table of contents.](README.md)
---