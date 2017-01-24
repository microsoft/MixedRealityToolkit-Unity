HoloToolkit Utilities                        {#mainpage}
============

## Description

Generic utility scripts for Unity that are useful for HoloLens development.

This includes extensions and additions to the Unity APIs for things that:

- Implement low level common operations that a large proportion of Unity developers would use.
- Implement low level functionality that based on common usage logically could be part of the built-in Unity API.

## Prefabs

### Main Camera

Prefab for a HoloLens-compatible camera

## Scripts

### ActionExtensions.cs
Extensions to the action class that provide a method that encapsulates the null check before raising an event.

### Billboard.cs
Rotates a hologram so it is always facing towards the camera.

### ComponentExtensions.cs
Extensions methods for the Unity Component class. THis also includes some component-related extensions for the GameObject class.

### DirectionIndicator.cs
Show a GameObject around the cursor that points in the direction of the GameObject which this script is attached to.
You must provide GameObjects for the Cursor and DirectionIndicatorObject public fields.

- **Cursor :** the object in your scene that is being used as the cursor. The direction indicator will be rendered around this cursor.
- **DirectionIndicatorObject :** the object that will point in the direction toward the object which this script is attached to. This object can be a 2D or 3D object.
- **DirectionIndicatorColor :** the color you want the DirectionIndicatorObject to be. The material on the DirectionIndicatorObject will need to support the color or TintColor property for this field to work. Otherwise the DirectionIndicatorObject will continue to render as its exported color.
- **TitleSafeFactor :** the percentage the GameObject can be within the view frustum for the DirectionIndicatorObject to start appearing. A value of 0 will display the DirectionIndicatorObject when the GameObject leaves the view. 0.1 will display when the GameObject is 10% away from the edge of the view. -0.1 will display when the GameObject is 10% out of view.

### FixedAngularSize.cs
Causes a hologram to maintain a fixed angular size, which is to say it occupies the same pixels in the view regardless of its distance from the camera.

### FpsDisplay.cs
MonoBehaviour that calculates the frames per seconds and shows the FPS in a referenced Text control.

### InterpolationUtilities.cs
Utility methods that perform various forms of interpolation.

### Interpolator.cs
A MonoBehaviour that interpolates a transform's position, rotation or scale.

### ManualCameraControl.cs
A MonoBehaviour class for manually controlling the camera when running in editor. Attach it to the same game object as the main camera.

### NearPlaneFade.cs
A MonoBehaviour that makes objects fade as they get close to the near plane.

### SimpleTagalong.cs
A Tagalong that stays at a fixed distance from the camera and always seeks to have a part of itself in the view frustum of the camera.

### Singleton.cs
A base class to make a MonoBehaviour follow the singleton design pattern.

### Tagalong.cs
A Tagalong that extends SimpleTagalong that allows for specifying the minimum and target percentage of the object to keep in the view frustum of the camera and that keeps the Tagalong object in front of other holograms including the Spatial Mapping Mesh.

### TransformExtensions.cs
Extensions to the Unity Transform class.

### VectorExtensions.cs
Extensions to the Unity Vector structs (Vector2 and Vector3).

## Shaders

### LambertianConfigurable.cginc
Code shared between LambertianConfigurable.shader and LambertianConfigurableTransparent.shader.

### LambertianConfigurable.shader
Feature configurable per-pixel lambertian shader. Use when higher quality lighting is desired, but specular highlights are not needed.

### LambertianConfigurableTransparent.shader
Feature configurable per-pixel lambertian transparent shader. Use when higher quality lighting and transparency are desired, but specular highlights are not needed.

### StandardFast.shader
Higher performance drop-in replacement for the Unity Standard Shader. Use when very high quality lighting (including reflections) is needed.

### UnlitConfigurable.cginc
Code shared between UnlitConfigurable.shader and UnlitConfigurableTransparent.shader.

### UnlitConfigurable.shader
Feature configurable unlit shader. Use when no lighting is desired.

### UnlitConfigurableTransparent.shader
Feature configurable unlit transparent shader. Use when transprency and no lighting are desired.

### VertexLitConfigurable.cginc
Code shared between VertexLitConfigurable.shader and VertexLitConfigurableTransparent.shader.

### VertexLitConfigurable.shader
Feature configurable vertex lit shader. Use when a higher performance but lower precision lighting tradeoff is acceptable.

### VertexLitConfigurableTransparent.shader
Feature configurable vertex lit transparent shader. Use when a higher performance but lower precision lighting tradeoff is acceptable, and transprency is needed.

### Occlusion.shader
A basic occlusion shader that can be used to occlude objects behind spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data.

### Wireframe.shader
A basic wireframe shader that can be used for rendering spatial mapping meshes. Use SpatialMappingManager.SetSurfaceMaterial() to use this material with the spatial mapping data.