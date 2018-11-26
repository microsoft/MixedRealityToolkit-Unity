## [Utilities]()
Useful components that make development or debugging easier. 
They are also components which don't rely one a particular device feature to work and are generic.

### [Prefabs](Prefabs)
---
Common useful prefabs not particularly related to a particular HoloLens feature.

#### FPSDisplay.prefab
Simple Tagalong billboard displaying application's frames per second.

#### HeadsUpDirectionIndicator.prefab
A drop in direction indicator that stays in the users view at all times.

#### HeadsUpDirectionIndicatorPointer.prefab
A quad based pointer to be used with the HeadsUpDirectionIndicator prefab to create an out of box direction indicator.

### [Scenes](Scenes)
---

#### HeadsetAdjustment.unity
Helps the user adjust the headset so that he/she gets the full experience in demo scenarios. 

When demoing an app, it is important to ensure that the user has the headset on properly. This scene shows a border at the edge of the user's field of view and instructs him/her to adjust the headset until all four edges are visible. The user (or the demonstrator) can then air-tap or say "I'm ready" to proceed to the actual experience. 

This scene is designed to be the first scene the user sees, once the experience is ready to begin. Once the user is ready, it loads either the next scene in the sequence or a scene specified by the NextSceneName property of the HeadsetAdjustment script.

#### SceneLauncher.unity
Creates a menu allowing you to jump to any scene in your project.

To run the SceneLauncher:

1. In Unity > File  Build Settings > Scenes in Build, drag in the scene files that you would like to run.
    *  If you would like to use all scenes in the project, search for t:Scene in the Project tab, select all, and drag all scenes into Scenes in Build.
2. After adding scenes to the build, you can use the Scene Mapping setting in the SceneLauncher prefab to control which scenes are included or not included in the SceneLauncher menu.
3. Build and run your project. (Or open the SceneLauncher scene and hit Play in the Unity editor.) A grid will be created with a button for each of your scenes.
4. Air tap a scene to load it.
5. When finished with a scene, say "Scene Launcher" or press the Backspace key to return the the scene list.

### [Scripts](Scripts)
---
Utilitiy Scripts.

#### [Editor](Scripts/Editor)
---
Editor Specific Scripts.

##### AutoConfigureMenu.cs
Configuration options derived from Microsoft Documentation [Configuring a Unity Project for HoloLens](https://developer.microsoft.com/en-us/windows/mixed-reality/unity_development_overview#Configuring_a_Unity_project_for_HoloLens).

##### AutoConfigureWindow.cs
Base class for auto configuration build windows.

##### CapabilitySettingsWindow.cs
Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Capability Settings.

##### EditorGUIExtensions.cs
Extensions for the UnityEnditor.EditorGUI class.

##### EditorGUILayoutExtensions.cs
Extensions for the UnityEditor.EditorGUILayout class.

##### EnforceEditorSettings.cs
Sets Force Text Serialization and visible meta files in all projects that use the HoloToolkit.

##### ExternalProcess.cs
Helper class for launching external processes inside of the unity editor.

##### LayerMaskExtensions.cs
Extensions for the UnityEngine.LayerMask class.

##### ProjectSettingsWindow.cs
Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Project Settings.

##### SceneSettingsWindow.cs
Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Scene Settings.

#### [Extensions](Scripts/Extensions)

##### ActionExtensions.cs
Extensions for the action class.  These methods encapsulate the null check before raising an event for an Action.

##### ComponentExtensions.cs
Extensions methods for the Unity Component class.  This also includes some component-related extensions for the GameObjet class.

##### Extensions.cs
A class with general purpose extensions methods.

##### TransformExtensions.cs
An extension method that will get you the full path to an object.

##### VectorExtensions.cs
A collection of useful extension methods for Unity's Vector structs.

#### [InterpolatedValues](Scripts/InterpolatedValues)
---
Interpolated Value Scripts.

##### InterpolatedColor.cs
Provides interpolation over Color.

##### InterpolatedFloat.cs
Provides interpolation over float.

##### InterpolatedQuaternion.cs
Provides interpolation over Quaternion.

##### InterpolatedValue.cs
Base class that provides the common logic for interpolating between values. This class does not inherit from MonoBehaviour in order to enable various scenarios under which it used. To perform the interpolation step, call FrameUpdate.

##### InterpolatedVector2.cs
Provides interpolation over Vector2.

##### InterpolatedVector3.cs
Provides interpolation over Vector3.

##### QuaternionInterpolated.cs
Class to encapsulate an interpolating Quaternion property.
TODO: Remove if redundant to InterpolatedQuaternion.cs

##### Vector3Interpolated.cs
Class to encapsulate an interpolating Vector3 property.
TODO: Remove if reduncatnt to InterpolatedVector3.cs

---

#### ApplicationViewManager.cs
Switchs app to a XAML page view, and waits for a return value from it.

#### Billboard.cs
Rotates a hologram so it is always facing towards the camera.

#### BitManipulator.cs
Helper class for bit manipulation.

#### CircularBuffer.cs
Helper class for transmitting data over network.

#### DirectionIndicator.cs
Show a GameObject around the cursor that points in the direction of the GameObject which this script is attached to.

You must provide GameObjects for the **_Cursor_** and **_DirectionIndicatorObject_** public fields.

**_Cursor_** The object in your scene that is being used as the cursor.  The direction indicator will be rendered around this cursor.

**_DirectionIndicatorObject_** The object that will point in the direction toward the object which this script is attached to.  This object can be a 2D or 3D object.

**DirectionIndicatorColor** The color you want the DirectionIndicatorObject to be.  The material on the DirectionIndicatorObject will need to support the color or TintColor property for this field to work.  Otherwise the DirectionIndicatorObject will continue to render as its exported color.

**TitleSafeFactor** The percentage the GameObject can be within the view frustum for the DirectionIndicatorObject to start appearing.  A value of 0 will display the DirectionIndicatorObject when the GameObject leaves the view.  0.1 will display when the GameObject is 10% away from the edge of the view.  -0.1 will display when the GameObject is 10% out of view.

#### DisplayDependentObjectActivator.cs
Causes a GameObject to activate or disable depending on the settings and type of display the application is launched on.

#### FixedAngularSize.cs
Causes a hologram to maintain a fixed angular size, which is to say it occupies the same pixels in the view regardless of its distance from the camera.

#### FpsDisplay.cs
Simple Behaviour which calculates the average frames per second over a number of frames and shows the FPS in a referenced Text control.

#### HeadsUpDirectionIndicator.cs
Spawns a user specified "pointer" object and startup and alligns it to aim at a target object which keeping the pointer in view at all times.

#### InterpolationUtilities.cs
Static class containing interpolation-related utility functions.

#### Interpolator.cs
A MonoBehaviour that interpolates a transform's position, rotation or scale.

#### MathUtils.cs
Math Utilities class.

#### NearPlaneFade.cs
Updates the shader parameters for use in near plane fading.

#### PriorityQueue.cs
Min-heap priority queue. In other words, lower priorities will be removed from the queue first.
See [Binary Heap](http://en.wikipedia.org/wiki/Binary_heap) for more info.

#### SessionExtensions.cs
Extensions methods for the Session class.

#### SimpleTagalong.cs
A Tagalong that stays at a fixed distance from the camera and always seeks to have a part of itself in the view frustum of the camera.

#### Singleton.cs
A base class to make a MonoBehaviour follow the singleton design pattern.

#### SphereBasedTagalong.cs
A simple Tagalong that stays inside a sphere at a fixed distance from the camera. Very cheap implementation with smoothing capability.

#### StabilizationPlaneModifier.cs
StabilizationPlaneModifier handles the setting of the stabilization plane in several ways.

#### Tagalong.cs
A Tagalong that extends SimpleTagalong that allows for specifying the minimum and target percentage of the object to keep in the view frustum of the camera and that keeps the Tagalong object in front of other holograms including the Spatial Mapping Mesh.

#### TextToSpeechManager.cs
Provides dynamic Text to Speech. Speech is generated using the UWP SpeechSynthesizer and then played through a Unity AudioSource. Both plain text and SSML are supported.

#### Timer.cs
Structure that defines a timer. A timer can be scheduled through the TimerScheduler.

#### TimerScheduler.cs
A scheduler that manages various timers.

#### Utils.cs
Miscellaneous utility methods.

#### VectorRollingStatistics.cs
Vector Statistics used in gaze stabilization.

#### WorldAnchorManager.cs
Wrapper around world anchor store to streamline some of the persistence api busy work.

### [Shaders](Shaders)
---

#### macro.cginc
Preprocessor macros to support shaders

#### HoloToolkitCommon.cginc
Common shader functionality

#### FastConfigurable.shader
Very fast shader that uses the Unity light system.  Compiles down to only performing the operations you're actually using.  Uses material property drawers rather than a custom editor for ease of maintenance.

#### FastConfigurable2Sided.shader
Similar attributes for FastConfigurable with the benefit of 2 sides!

#### WindowOcclusion.shader
A simple occlusion shader that can be used to hide other objects. This prevents other objects from being rendered by drawing invisible 'opaque' pixels to the depth buffer. This shader differs from Occlusion.shader in that it doesn't have any depth offset, so it should sort as expected with other objects adjacent to the window.

### [Tests](https://github.com/Microsoft/HoloToolkit-Unity/tree/master/Assets/HoloToolkit-Tests/Utilities/Scenes)
---
Tests related to the utilities features. To use the scene:

1. Navigate to the Tests folder.
2. Double click on the test scene you wish to explore.
3. Either click "Play" in the unity editor or File -> Build Settings.
4. Add Open Scenes, Platform -> Windows Store, SDK -> Universal 10, Build Type -> D3D, Check 'Unity C# Projects'.
5. Click 'Build' and create an App folder. When compile is done, open the solution and deploy to device.

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
