# HoloToolkit-Unity
This is effectively part of the existing HoloToolkit, but this is the repo that will contain all Unity specific components.
The HoloToolkit is a collection of scripts and components intended to accelerate the development of holographic applications targeting Windows Holographic

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

## SpatialMapping
Any scripts that leverage SpatialMapping.
PlaneFinding addon that can be used to find planar surfaces (ie: walls/floors/tables/etc) in the mesh data returned by Spatial Mapping.

### PlaneFinding.cs
Unity script that wraps the native PlaneFinding DLL

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

---

## Tests
This folder contains sample/test code that demonstrates how to use certain scripts.  

### SpatialMapping::PlaneFinding
To use this sample code, start by creating a new Unity scene.  Create an empty GameObject and attach the PlaneFindingTest script to it. Then drag the FakeSpatialMappingMesh onto this GameObject to create a child object that renders the mesh.

Now hit Play. The PlaneFinding algorithm will run in a loop, visualing the planes found via Unity editor gizmos. The PlaneFindingTest component exposes a couple of properties that let you manipulate the PlaneFinding API parameters in realtime and observe their impact on the algorithm.

NOTE: In the interest of simplicity, this test script calls the PlaneFinding APIs directly from the main Unity thread in Update(). In a real application, the PlaneFinding APIs **MUST** be called from a background thread in order to avoid stalling the rendering thread and causing a drop in framerate.

---