# Main Concepts

Scenes have been divided into three types, and each type has a different function.

![](../Images/SceneSystem/MRTK_SceneSystemEditorSceneHeirarchy.png)

## Content Scene
These are the scenes you're used to dealing with. Any kind of content can be stored in them, and they can be loaded or unloaded in any combination.

Content scenes are enabled by default. Any scenes included in your profile's `ContentScenes` array can be loaded / unloaded by the service.

## Manager scene
A single scene with a required MixedRealityToolkit instance. This scene will be loaded first on launch and will remain loaded for the lifetime of the app. The manager scene can also host other objects that should never be destroyed. This is the preferred alternative to DontDestroyOnLoad.

To enable this feature, check `Use Manager Scene` in your profile and drag a scene object into the `Manager Scene` field.

## Lighting Scene
A set of scenes which store lighting information and lighting objects. Only one can be loaded at a time, and their settings can be blended during loads for smooth lighting transitions.

Unity's lighting settings - ambient light, skyboxes, etc - can be tricky to manage when using additive loading because they're tied to invidiual scenes and override behavior is not straightforward. In practice this can cause confusion when assets are authored in lighting conditions that don't obtain at runtime.

![](../Images/SceneSystem/MRTK_SceneSystemLightingSettings.png)

The Scene System uses lighting scenes to ensure that these settings remain consistent regardless of what scenes are loaded or active, both in edit mode and in play mode.

To enable this feature, check `Use Lighting Scene` in your profile and populate the `Lighting Scenes` array.