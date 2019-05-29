# Load Operations
## Content Loading
All load operations in the Scene System are asynchronous, and by default all Content Loading is additive.

### Defaul Loading Behavior


```
await LoadContent("MyContentScene");
```

`ContentSceneNames` provides an array of available content scenes in order of build index. You can check whether these scenes are loaded via `IsContentLoaded(string contentName)`

### Single Scene Loading
The equivalent of a single scene load can be achieved via LoadContent. This will asynchronously unload all loaded content scenes, then load "SceneName." Manager and lighting scenes will not be affected:

```
await LoadContent("MyContentScene", LoadSceneMode.Single);
```

### Next / Previous Scene Loading
Content can be singly loaded in order of build index. This is useful for showcase applications that take users through a set of demonstration scenes one-by-one. You can load the next / previous content like so:

```
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

if (pressedNextSceneButton && sceneSystem.NextContentExists) 
{
    await sceneSystem.LoadNextContent();
}

if (pressedPrevSceneButton && sceneSystem.PrevContentExists)
{
    await sceneSystem.LoadPrevContent();
}
```

### Loading by Tag

# Lighting Scene Operations
The default lighting scene defined in your profile is loaded on startup. That scene remains loaded until `SetLightingScene` is called. This is the only way to load lighting scenes.

```
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

sceneSystem.SetLightingScene("MyLightingSceneName", LightingSceneTransitionType.None);
```


## Lighting Scene Transitions
`transitionType` controls the style of the transition to new lighting scene. The available styles are:

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
Your profile contains cached versions of the lighting settings kept in your lighting scenes. If those settings change in your scenes, you will need to cache them to ensure things look as you expect at runtime. Your profile will display a warning when it suspects your cached settings are out of date. Clicking `Update Cached Lighting Settings` will load each of your lighting scenes, extracts their settings, then stores them in the profile.

## Editor Behavior
One goal of lighting scenes is to ensure your content is lit correctly while editing. To this end, the Scene Service will keep a Lighting Scene loaded at all times, and will copy that scene's lighting settings to the current active scene. (By default the active scene will be set to a Content Scene of your choice.)

You can change which lighting scene is loaded by opening the Scene System's service inspector. In edit mode you can instantaneously transition between lighting scenes. In play mode, you can preview transitions.

Note: *Typically the active scene determines your lighting settings in editor. However we choose not to use this feature to enforce lighting settings, because the active scene is also where newly created objects are placed by default, and lighting scenes are only permitted to contain lighting components. Instead, the current Lighting Scene's settings are automatically copied to the active scene's settings instead. Note that this will result in your content scene's lighting settings being over-written.*