# Lighting scene operations

The default lighting scene defined in your profile is loaded on startup. That lighting scene remains loaded until `SetLightingScene` is called.

```c#
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

sceneSystem.SetLightingScene("MorningLighting");
```

## Lighting setting transitions

`transitionType` controls the style of the transition to new lighting scene.

```c#
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
