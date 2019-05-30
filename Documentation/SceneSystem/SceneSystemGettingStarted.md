# Scene System Overview

## When to use the scene system
If your project consists of a single scene, the Scene System probably isn't necessary. The scene system is most useful when:

- You're used to single scene loading, but you don't like the way it destroyes the MixedRealityToolkit instance.
- You want a simple way to to additively load multiple scenes to construct your experience.
- You want a simple way to keep track of load operations in progress, or a simple way to control scene activation for multiple scenes being loaded at once.
- You want to keep lighting consistent and predictable across all your scenes.

# Editor Settings
By default the Scene System enforces several behaviors in the Unity editor. If you find any of these behaviors heavy-handed they can be disabled in the **Editor Settings** section of your Scene System profile.

- `Editor Manage Build Settings:` If true, the service will update your build settings automatically, ensuring that all manager, lighting and content scenes are added. Disable this if you want total control over build settings.

- `Editor Enforce Scene Order:` If true, the service will ensure manager scene is displayed first in scene heirarchy, followed by lighting and then content. Disable this if you want total control over scene heirarchy.

- `Editor Manage Loaded Scenes:` If true, service will ensure that manager scenes and lighting scenes are always loaded. Disable if you want total control over which scenes are loaded in editor.

- `Editor Enforce lighting scene Types:` If true, service will ensure that only the lighting-related components defined in `PermittedLightingSceneComponentTypes` are allowd in lighting scenes. Disable if you want total control over the content of lighting scenes.

![](../Images/SceneSystem/MRTK_SceneSystemProfileEditorSettings.png)

# Main Concepts

Scenes have been divided into three types, and each type has a different function.

![](../Images/SceneSystem/MRTK_SceneSystemEditorSceneHeirarchy.png)

## Content Scene
These are the scenes you're used to dealing with. Any kind of content can be stored in them, and they can be loaded or unloaded in any combination.

Content scenes are enabled by default. Any scenes included in your profile's `ContentScenes` array can be loaded / unloaded by the service.

## Manager scene
A single scene with a required MixedRealityToolkit instance. This scene will be loaded first on launch and will remain loaded for the lifetime of the app. This is the preferred alternative to DontDestroyOnLoad.

Your manager scene is a single scene with a MixedRealityToolkit instance. This scene will be loaded first on launch and will remain loaded for the lifetime of the app. The manager scene can also host other objects that should never be destroyed.

To enable this feature, check `Use Manager Scene` in your profile and drag a scene object into the `Manager Scene` field.

## Lighting Scene
A set of scenes which store lighting information and lighting objects. Only one can be loaded at a time, and their settings can be blended during loads for smooth lighting transitions.

The benefits of lighting scenes aren't as straightforward as the manager scene. Unity's lighting settings - ambient light, skyboxes, etc - can be tricky to manage. When using single loading, the last loaded scene will determine the lighting settings. When using additive loading, the active scene determines lighting settings, and unless you set the active scene manually, the *first* loaded scene remains the active scene. While viewing content in the editor this can be even more confusing. lighting scenes can help avoid this confusion.

![](../Images/SceneSystem/MRTK_SceneSystemLightingSettings.png)

To enable this feature, check `Use Lighting Scene` in your profile and populate the `Lighting Scenes` array.

(Note that if these scenes contain any component types, those components will be moved out of your lighting scene. It's best to start with an empty scene.)

# Next Steps

- [Content Scene Loading](SceneSystemContentLoading.md)
- [Monitoring Content Loading.](SceneSystemLoadProgress.md)
- [Lighting Scene Loading](SceneSystemLightingScenes.md)