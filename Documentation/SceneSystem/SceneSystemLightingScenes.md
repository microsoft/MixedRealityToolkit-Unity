# Lighting Scene Operations
The default lighting scene defined in your profile is loaded on startup. That lightin scene remains loaded until `SetLightingScene` is called.

```
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

sceneSystem.SetLightingScene("MorningLighting");
```


## Lighting Scene Transitions
`transitionType` controls the style of the transition to new lighting scene.
```
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

sceneSystem.SetLightingScene("MiddayLighting", LightingSceneTransitionType.CrossFade);
```

The available styles are:

Type | Description | Duration
--- | --- | ---
None | Previous lighting scene is unloaded, new lighting scene is loaded. No transition. | Ignored
FadeToBlack | Previous lighting scene fades out to black. New lighting scene is loaded, then faded up from black. Useful for smooth transitions between locations. | Used
CrossFade | Previous lighting scene fades out as new lighting scene fades in. Useful for smooth transitions between lighting setups in the same location. | Used

Note that some lighting settings cannot be interpolated during transitions. If you want a smooth visual transition these settings will have to remain consistent between lighting scenes.

Setting | Smooth FadeToBlack Transition | Smooth CrossFade Transition
--- | --- | ---
Skybox | No | No
Custom Reflections | No | No
Sun light realtime shadows | Yes | No

## Cached Lighing Settings
Your profile stores cached copies of the lighting settings kept in your lighting scenes. If those settings change in your lighting scenes, you will need to update your cache to ensure lighting appears as expected in play mode. Your profile will display a warning when it suspects your cached settings are out of date. Clicking `Update Cached Lighting Settings` will load each of your lighting scenes, extracts their settings, then stores them in the profile.

![](../Images/SceneSystem/MRTK_SceneSystemCachedLightingSettings.png)

## Editor Behavior
One benefit of using lighting scenes is knowing your content is lit correctly while editing. To this end, the Scene Service will keep a lighting scene loaded at all times, and will copy that scene's lighting settings to the current active scene.\*

You can change which lighting scene is loaded by opening the Scene System's [service inspector.](../MixedRealityConfigurationGuide.md#inspectors) In edit mode you can instantaneously transition between lighting scenes. In play mode, you can preview transitions.

![](../Images/SceneSystem/MRTK_SceneSystemServiceInspector.png)

\**Note: Typically the active scene determines your lighting settings in editor. However we choose not to use this feature to enforce lighting settings, because the active scene is also where newly created objects are placed by default, and lighting scenes are only permitted to contain lighting components. Instead, the current lighting scene's settings are automatically copied to the active scene's settings instead. Keep in mind that this will result in your content scene's lighting settings being over-written.*