# Scene Types

Scenes have been divided into three types, and each type has a different function.

![](../../Documentation/Images/SceneSystem/MRTK_SceneSystemEditorSceneHierarchy.png)

## Content Scenes
These are the scenes you're used to dealing with. Any kind of content can be stored in them, and they can be loaded or unloaded in any combination.

Content scenes are enabled by default. Any scenes included in your profile's `Content Scenes` array can be loaded / unloaded by the service.

___

## Manager scenes
A single scene with a required MixedRealityToolkit instance. This scene will be loaded first on launch and will remain loaded for the lifetime of the app. The manager scene can also host other objects that should never be destroyed. This is the preferred alternative to DontDestroyOnLoad.

To enable this feature, check `Use Manager Scene` in your profile and drag a scene object into the `Manager Scene` field.

___

## Lighting Scenes
A set of scenes which store lighting information and lighting objects. Only one can be loaded at a time, and their settings can be blended during loads for smooth lighting transitions.

Unity's lighting settings - ambient light, skyboxes, etc - can be tricky to manage when using additive loading because they're tied to individual scenes and override behavior is not straightforward. In practice this can cause confusion when assets are authored in lighting conditions that don't obtain at runtime.

![](../../Documentation/Images/SceneSystem/MRTK_SceneSystemLightingSettings.png)

The Scene System uses lighting scenes to ensure that these settings remain consistent regardless of what scenes are loaded or active, both in edit mode and in play mode.

To enable this feature, check `Use Lighting Scene` in your profile and populate the `Lighting Scenes` array.

### Cached Lighting Settings
Your profile stores cached copies of the lighting settings kept in your lighting scenes. If those settings change in your lighting scenes, you will need to update your cache to ensure lighting appears as expected in play mode. Your profile will display a warning when it suspects your cached settings are out of date. Clicking `Update Cached Lighting Settings` will load each of your lighting scenes, extract their settings, then store them in your profile.

![](../../Documentation/Images/SceneSystem/MRTK_SceneSystemCachedLightingSettings.png)

### Editor Behavior
One benefit of using lighting scenes is knowing your content is lit correctly while editing. To this end, the Scene Service will keep a lighting scene loaded at all times, and will copy that scene's lighting settings to the current active scene.\*

You can change which lighting scene is loaded by opening the Scene System's [service inspector.](../MixedRealityConfigurationGuide.md#inspectors) In edit mode you can instantaneously transition between lighting scenes. In play mode, you can preview transitions.

![](../../Documentation/Images/SceneSystem/MRTK_SceneSystemServiceInspector.png)

\**Note: Typically the active scene determines your lighting settings in editor. However we choose not to use this feature to enforce lighting settings, because the active scene is also where newly created objects are placed by default, and lighting scenes are only permitted to contain lighting components. Instead, the current lighting scene's settings are automatically copied to the active scene's settings instead. Keep in mind that this will result in your content scene's lighting settings being over-written.*