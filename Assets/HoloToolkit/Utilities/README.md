## [Utilities]()
Useful common concepts that you can leverage in your application.

### [Prefabs](Prefabs)
Common useful prefabs not particularly related to a particular HoloLens feature.

#### FPSDisplay.prefab
Simple Tagalong billboard displaying application's frames per second.

#### HeadsUpDirectionIndicator.prefab
A drop in direction indicator that stays in the users view at all times.

#### HeadsUpDirectionIndicatorPointer.prefab
A quad based pointer to be used with the HeadsUpDirectionIndicator prefab to create an out of box direction indicator.k

#### Main Camera.prefab
Unity camera that has been customized for Holographic development.
1. Camera.Transform set to 0,0,0
2. 'Clear Flags' changed to 'Solid Color'
3. Color set to R:0, G:0, B:0, A:0 as black renders transparent in HoloLens.
4. Set the recommended near clipping plane.

### [Scripts](Scripts)

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

#### HeadsUpDirectionIndicator.cs
Spawns a user specified "pointer" object and startup and alligns it to aim at a target object which keeping the pointer in view at all times.

#### Interpolator.cs
A MonoBehaviour that interpolates a transform's position, rotation or scale.

#### SphereBasedTagalong.cs
A simple Tagalong that stays inside a sphere at a fixed distance from the camera. Very cheap implementation with smoothing capability.

#### SimpleTagalong.cs
A Tagalong that stays at a fixed distance from the camera and always seeks to have a part of itself in the view frustum of the camera.

#### Tagalong.cs
A Tagalong that extends SimpleTagalong that allows for specifying the minimum and target percentage of the object to keep in the view frustum of the camera and that keeps the Tagalong object in front of other holograms including the Spatial Mapping Mesh.

#### Singleton.cs
A base class to make a MonoBehaviour follow the singleton design pattern.

#### TextToSpeechManager.cs
Provides dynamic Text to Speech. Speech is generated using the UWP SpeechSynthesizer and then played through a Unity AudioSource. Both plain text and SSML are supported.   

### [Shaders](Shaders)

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

#### UnlitNoDepthTest.shader
Render with a single texture but ignore depth test resuls so object always appears on top.

#### VertexLitConfigurable.cginc
Code shared between VertexLitConfigurable.shader and VertexLitConfigurableTransparent.shader.

#### VertexLitConfigurable.shader
Feature configurable vertex lit shader.  Use when a higher performance but lower precision lighting trade-off is acceptable.

#### VertexLitConfigurableTransparent.shader
Feature configurable vertex lit transparent shader.  Use when a higher performance but lower precision lighting trade-off is acceptable, and transparency is needed.

#### WindowOcclusion.shader
A simple occlusion shader that can be used to hide other objects. This prevents other objects from being rendered by drawing invisible 'opaque' pixels to the depth buffer. This shader differs from Occlusion.shader in that it doesn't have any depth offset, so it should sort as expected with other objects adjacent to the window.

### [Tests](Tests)

#### HeadsUpDirectionIndicator.unity
This scene shows 7 marker objects with 7 HeadsUpDirectionIndicators pointing to each. Each indicator has a label that matches its corresponding marker. 6 of the marker/indicator pairs are used to test the edge cases of axis aligned markers. The 7th is an arbitrary point off of the cartesean axes. From the starting position, the user should be able to follow the direction of each indicator and arrive at the marker with the corresponding axis label. At the start, the labels should be in the following screen locations.

- \-X at the left
- +X at the right
- \-Y at the bottom
- +Y at the top
- \-Z also at the bottom
- +Z in front

#### TextToSpeechManager.unity 

This scene demonstrates how to use TextToSpeechManager.cs.  The script is placed on 3 cubes in the scene. Whenever a cube is activated with an air tap, a text to speech voice will emanate from the cube. The user can also ask "What time is it?" to hear the current time from a voice that stays with the user as they move.

#### WindowOcclusion.unity 

This scene demonstrates how to use WindowOcclusion.shader.  It positions a virtual 'window' directly in front of you when the scene starts. A cube in the back is only visible when viewed through the window because quads around the window use the WindowOcclusion shader.

---
##### [Go back up to the table of contents.](../../../README.md)
---