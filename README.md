# HoloToolkit-Unity
This is effectively part of the existing HoloToolkit, but this is the repo that will contain all Unity specific components.
The HoloToolkit is a collection of scripts and components intended to accelerate the development of holographic applications targeting Windows Holographic.

---

## Plugins
Folder containing the native and managed binaries being compiled from the HoloToolkit repository.

---

## Utilities
Useful common concepts that you can leverage in your apps.

### Billboard.cs
Rotates a hologram so it is always facing towards the camera.

### Interpolator.cs
A MonoBehaviour that interpolates a transform's position, rotation or scale.

### Singleton.cs
A base class to make a MonoBehaviour follow the singleton design pattern.

### DirectionIndicator.cs
Show a GameObject around the cursor that points in the direction of the GameObject which this script is attached to.

You must provide GameObjects for the **_Cursor_** and **_DirectionIndicatorObject_** public fields.

**_Cursor_** The object in your scene that is being used as the cursor.  The direction indicator will be rendered around this cursor.

**_DirectionIndicatorObject_** The object that will point in the direction toward the object which this script is attached to.  This object can be a 2D or 3D object.

**DirectionIndicatorColor** The color you want the DirectionIndicatorObject to be.  The material on the DirectionIndicatorObject will need to support the color or TintColor property for this field to work.  Otherwise the DirectionIndicatorObject will continue to render as its exported color.

**TitleSafeFactor** The percentage the GameObject can be within the view frustum for the DirectionIndicatorObject to start appearing.  A value of 0 will display the DirectionIndicatorObject when the GameObject leaves the view.  0.1 will display when the GameObject is 10% away from the edge of the view.  -0.1 will display when the GameObject is 10% out of view.

### FixedAngularSize.cs
Causes a hologram to maintain a fixed angular size, which is to say it occupies the same pixels in the view regardless of its distance from the camera.

### SimpleTagalong.cs
A Tagalong that stays at a fixed distance from the camera and always seeks to have a part of itself in the view frustum of the camera.

### Tagalong.cs
A Tagalong that extends SimpleTagalong that allows for specifying the minimum and target percentage of the object to keep in the view frustum of the camera and that keeps the Tagalong object in front of other holograms including the Spatial Mapping Mesh.

---

## Input
Any scripts that leverage the HoloLens input features namely Gaze, Gesture and Voice.

### CursorManager.cs
Control the position and rotation of a cursor that follows the user's gaze.

You must provide GameObjects for the **_CursorOnHologram_** and **_CursorOffHologram_** public fields.

**_CursorOnHologram_** Cursor object to display when you are gazing at a hologram.

**_CursorOffHologram_** Cursor object to display when you are not gazing at a hologram.

**DistanceFromCollision** Distance, in meters, to offset the cursor from a collision with a hologram in the scene.  This is to prevent the cursor from being occluded.

### GazeManager.cs
Perform raycasts in the user's gaze direction to get the position and normals of any collisions.

**MaxGazeDistance** The maximum distance to raycast.  Any holograms beyond this value will not be raycasted to.

**RaycastLayerMask** The Unity layers to raycast against.  If you have holograms that should not be raycasted against, like a cursor, do not include their layers in this mask.

### GazeStabilizer.cs
Stabilize the user's gaze to account for head jitter.

**StoredStabilitySamples** Number of samples that you want to iterate on.  A larger number will be more stable.

**PositionDropOffRadius** Position based distance away from gravity well.

**DirectionDropOffRadius** Direction based distance away from gravity well.

**PositionStrength** Position lerp interpolation factor.

**DirectionStrength** Direction lerp interpolation factor.

**StabilityAverageDistanceWeight** Stability average weight multiplier factor.

**StabilityVarianceWeight** Stability variance weight multiplier factor.

### HandGuidance.cs
Show a GameObject when a gesturing hand is about to lose tracking.

You must provide GameObjects for the **_Cursor_** and **_HandGuidanceIndicator_** public fields.

**_Cursor_** The object in your scene that is being used as the cursor.  The hand guidance indicator will be rendered around this cursor.

**_HandGuidanceIndicator_** GameObject to display when your hand is about to lose tracking.

**HandGuidanceThreshold** When to start showing the HandGuidanceIndicator.  1 is out of view, 0 is centered in view.

### KeywordManager.cs
Allows you to specify keywords and methods in the Unity Inspector, instead of registering them explicitly in code.  
**IMPORTANT**: Please make sure to add the microphone capability in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

**_KeywordsAndResponses_** Set the size as the number of keywords you'd like to listen for, then specify the keywords and method responses to complete the array.

**RecognizerStart** Set this to determine whether the keyword recognizer will start immediately or if it should wait for your code to tell it to start.

---

## SpatialMappingComponent
A unified set of scripts adhering to best practices for providing physics or rendering support for Spatial Mapping.


How to use:

1. Enable the "SpatialPerception" capability in Player Settings -> Windows Store -> Publishing Settings -> Capabilities.
2. Add a SpatialMappingCollider or SpatialMappingRenderer component onto a GameObject and spatial mapping will just start working

If you want spatial mapping to work wherever the user travels, attach the components to the camera and the specified bounds will move with the user. If you want collisions to continue working even after walking away, you may want to leave a second SpatialMappingCollider around the playspace where collisions should continue. 

If you want physics collisions and to render the spatial mapping mesh for an area, you should add both a SpatialMappingCollider and a SpatialMappingRenderer

Though all of the defaults are usable out of the box with no customization, you can also customize the component to your scenario. Through script, additions such as PlaneFinding.cs can also be used with these components.

These components by default implement caching of removed Spatial Mapping meshes such that removed meshes will still be present for at least 10 updates after they are actually removed. This number and the feature can be configured via script. This feature enables mesh a great distance from the user to not be removed as well as quick rehydration of mesh only removed as a user moved away and back to a single location.

**IMPORTANT**: Please make sure to add the Spatial Perception capability in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.

### SpatialMappingCollider.cs
This component is for doing physics against the Spatial Mapping mesh.

**Enable Collisions**: (Default: **yes**). Whether or not to create a collider for RigidBody physics

**Physic Material**: (Default: **none**). Custom PhysicMaterial to define physical interactions (like bounciness) with the mesh.

**Physics Layer**: (Default: **default**). The layer to apply to the meshes for Raycasting.

**Freeze Mesh Updates**: (Default: **no**). When enabled, no further updates will be processed. Use this to delay initial starting of Spatial Mapping processing or to stop updates at a certain point.

**Bounding Volume**: (Default: **Bounding Box**). The shape of the bounds to observe for new surfaces. The choices are Bounding Box with Extents or Sphere with Radius.

**Extents**: (Default: **(10, 10, 10)**). The extents of the bounding box in meters. Only applicable with Bounding Box set for Bounding Volume.

**Radius**: (Default: **10**). The radius of the bounding sphere in meters. Only applicable with Sphere set for Bounding Volume.

**Level of Detail**: (Default: **Low**). The amount of detail in the resulting meshes. Possible values are Low, Medum, and High. Higher levels of detail will result in lower performance.

**Time Between Updates**: (Default: **2.5**). The frequency in seconds for when updates to Spatial Mapping surfaces will be processed.

### SpatialMappingRenderer.cs
This component is for rendering the Spatial Mapping mesh directly.

**Rendering Mode**: (Default: **Occlusion**). How to render the mesh. Occlusion will cause the mesh to occlude holograms behind it. Material will apply the specified material. None will cause the meshes to not render at all.

**Rendering Material**: (Default: **Wireframe**). The material to render the Spatial Mapping mesh with. This is only relevant when Rendering Mode is set to Material. The default materials are kept in your Resources\HoloToolkit\ folder.

**Freeze Mesh Updates**: (Default: **no**). When enabled, no further updates will be processed. Use this to delay initial starting of Spatial Mapping processing or to stop updates at a certain point.

**Bounding Volume**: (Default: **Bounding Box**). The shape of the bounds to observe for new surfaces. The choices are Bounding Box with Extents or Sphere with Radius.

**Extents**: (Default: **(10, 10, 10)**). The extents of the bounding box in meters. Only applicable with Bounding Box set for Bounding Volume.

**Radius**: (Default: **10**). The radius of the bounding sphere in meters. Only applicable with Sphere set for Bounding Volume.

**Level of Detail**: (Default: **Medium**). The amount of detail in the resulting meshes. Possible values are Low, Medum, and High. Higher levels of detail will result in lower performance.

**Time Between Updates**: (Default: **2.5**). The frequency in seconds for when updates to Spatial Mapping surfaces will be processed.

---

## SpatialMapping
Additional scripts that leverage SpatialMapping.

We recommend using the SpatialMappingCollider and/or the SpatialMapppingRenderer for default Spatial Mapping requirements. If you need the ability to load the mesh from a file or the network, these scrips are more appropriate.

PlaneFinding addon that can be used to find planar surfaces (ie: walls/floors/tables/etc) in the mesh data returned by Spatial Mapping.
**IMPORTANT**: Please make sure to add the Spatial Perception capability in your app, in Unity under  
Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities  
or in your Visual Studio Package.appxmanifest capabilities.
If using the RemoteMapping components, you will also need to set the InternetClientServer, PrivateNetworkClientServer, and Microphone capabilities. 

### FileSurfaceObserver.cs
A SpatialMappingSource that loads spatial mapping data from a file in Unity.

**MeshFileName** Name of file to use when saving or loading surface mesh data.

### MeshSaver.cs
Static class that can read and write mesh data to the file specified in FileSurfaceObserver.cs.

### PlaneFinding.cs
Unity script that wraps the native PlaneFinding DLL. Used by SurfaceMeshesToPlanes.cs.

### RemoteMappingManager.cs
Allows sending meshes remotely from HoloLens to Unity.

### RemoteMeshSource.cs
Networking component that runs on the HoloLens and can send meshes to Unity.

**ServerIP** The IPv4 address of the machine running the Unity editor.

**ConnectionPort** The network port of the Unity machine that will recieve spatial mapping data from the HoloLens.

### RemoteMeshTarget.cs
SpatialMappingSource object that runs in the Unity editor and recieve spatial mapping data from the HoloLens.

**ServerIP** The IPv4 address of the machine running the Unity editor.

**ConnectionPort** The network port of the Unity machine that will recieve mesh data from the HoloLens.

### RemoveSurfaceVertices.cs
A spatial processing component that will remove any spatial mapping vertices that fall within the specified bounding volumes.

**BoundsExpansion** The amount, if any, to expand each bounding volume by.

### SimpleMeshSerializer.cs
Static class that converts a Unity mesh to an array of bytes. Used by MeshSaver.cs to serialize and deserialize mesh data.

### SpatialMappingManager.cs
Manages interactions between the application and all spatial mapping data sources (file, observer, network).

**PhysicsLayer** The physics layer to use for all spatial mapping mesh data.

**SurfaceMaterial** The material to apply when rendering the spatial mapping mesh data.

**DrawVisualMeshes** Determines if spatial mapping meshes will be rendered.

**CastShadows** Determines if spatial mapping meshes can cast shadows.

### SpatialMappingObserver.cs
Adds and updates spatial mapping data for all surfaces discovered by the SurfaceObserver running on the HoloLens.

**TrianglesPerCubicMeter** Level of detail to use for each mesh found by the SurfaceObserver.

**Extents** Extents of the observation volume which expand out from the camera's position.

**TimeBetweenUpdates** Time to wait (sec) before processing updates from the SurfaceObserver.

### SpatialMappingSource.cs
Generates and retrieves meshes based on spatial mapping data coming from the current source object (file, observer, network). SpatialMappingManager.cs manages switching between source types and interacting with this class.

### SurfaceMeshesToPlanes.cs
A spatial processing component that can find and create planes based on spatial mapping meshes. Uses PlaneFinding.cs and requires the PlaneFinding plugin.

**ActivePlanes** Collection of planes found within the spatial mapping data.

**_SurfacePlanePrefab_** A GameObject that will be used for generating planes. If no prefab is provided, a Unity cube primitive will be used instead.

**MinArea** Minimum area required for a plane to be created.

**DrawPlanes** Bit mask which specifies the type of planes that should be rendered (walls, floors, ceilings, etc).

**DestroyPlanes** Bit mask which specifies the type of planes that should be discarded.

### SurfacePlane.cs
Generates planes and classifies them by type (wall, ceiling, floor, table, unknown). Should be a component on the SurfacePlanePrefab used by SurfaceMeshesToPlanes.cs.

**PlaneThickness** How thick each plane should be.

**UpNormalThreshold** Threshold for acceptable normals. Used to determine if a plane is horizontal or vertical.

**FloorBuffer** Max distance from the largest floor plane before a horizontal plane will be classified as a table.

**CeilingBuffer** Max distance from the largest ceiling plane before a horizontal plane will be classified as a table.

**WallMaterial** Material to use when rendering wall plane types.

**FloorMaterial** Material to use when rendering ceiling plane types.

**TableMaterial** Material to use when rendering table plane types.

**UnknownMaterial** Material to use when rendering unknown plane types.

---

## Shaders

### LambertianConfigurable.cginc
Code shared between LambertianConfigurable.shader and LambertianConfigurableTransparent.shader.

### LambertianConfigurable.shader
Feature configurable per-pixel lambertian shader.  Use when higher quality lighting is desired, but specular highlights are not needed.

### LambertianConfigurableTransparent.shader
Feature configurable per-pixel lambertian transparent shader.  Use when higher quality lighting and transparency are desired, but specular highlights are not needed.

### StandardFast.shader
Higher performance drop-in replacement for the Unity Standard Shader.  Use when very high quality lighting (including reflections) is needed.

### UnlitConfigurable.cginc
Code shared between UnlitConfigurable.shader and UnlitConfigurableTransparent.shader.

### UnlitConfigurable.shader
Feature configurable unlit shader.  Use when no lighting is desired.

### UnlitConfigurableTransparent.shader
Feature configurable unlit transparent shader.  Use when transprency and no lighting are desired.

### VertexLitConfigurable.cginc
Code shared between VertexLitConfigurable.shader and VertexLitConfigurableTransparent.shader.

### VertexLitConfigurable.shader
Feature configurable vertex lit shader.  Use when a higher performance but lower precision lighting tradeoff is acceptable.

### VertexLitConfigurableTransparent.shader
Feature configurable vertex lit transparent shader.  Use when a higher performance but lower precision lighting tradeoff is acceptable, and transprency is needed.

### Occlusion.shader
A basic occlusion shader that can be used to occlude objects behind spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data.

### Wireframe.shader
A basic wireframe shader that can be used for rendering spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data.

---

## Tests
This folder contains sample/test code that demonstrates how to use certain scripts.  

### SpatialMapping::PlaneFinding
To use this sample code, start by creating a new Unity scene.  Create an empty GameObject and attach the PlaneFindingTest script to it. Then drag the FakeSpatialMappingMesh onto this GameObject to create a child object that renders the mesh.

Now hit Play. The PlaneFinding algorithm will run in a loop, visualing the planes found via Unity editor gizmos. The PlaneFindingTest component exposes a couple of properties that let you manipulate the PlaneFinding API parameters in realtime and observe their impact on the algorithm.

NOTE: In the interest of simplicity, this test script calls the PlaneFinding APIs directly from the main Unity thread in Update(). In a real application, the PlaneFinding APIs **MUST** be called from a background thread in order to avoid stalling the rendering thread and causing a drop in framerate.

### Input
In this folder you will see different scenes like BasicCursor, Cursor, CursorWithFeedback that show you a few different ways of building cursors for you applications.

### Sharing
Sharing scene demonstrates how to use the Sharing prefabs for networking and sharing custom messages with clients. 
It also demonstrates how to share world anchors between clients to establish a shared space.

Before deploying this scene to your HoloLens:
1. On a PC in Unity click the menu option HoloToolkit->Launch Sharing Service
2. In Hierarchy panel, click on Sharing game object
3. In the Inspector panel, in the Server Address field enter your PC IP or the computer Name
4. Save the scene and deploy.

### SpatialMappingComponent
This scene shows an example of a static playspace which preserves physics around it (marked by a cube) while maintaining physics and wireframe rendering of Spatial Mapping around the camera. Additionally, you can tap to drop a cube in front of the camera with a Rigidbody component to interact with physics.

---
