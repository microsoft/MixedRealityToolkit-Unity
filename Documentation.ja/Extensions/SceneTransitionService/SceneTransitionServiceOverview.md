# Scene transition service

This extension simplifies the business of fading out a scene, displaying a progress indicator, loading a scene, then fading back in.

Scene operations are driven by the SceneSystem service, but any Task-based operation can be used to drive a transition.

## Enabling the extension

To enable the extension, open your RegisteredServiceProvider profile. Click Register a new Service Provider to add a new configuration. In the Component Type field, select SceneTransitionService. In the Configuration Profile field, select the default scene transition profile included with the extension.

## Profile options

### Use default progress indicator

If checked, the default progress indicator prefab will be used when no progress indicator object is provided when calling `DoSceneTransition.` If a progress indicator object is provided, the default will be ignored.

### Use fade color

If checked, the transition service will apply a fade during your transition. This setting can be changed at runtime via the service's `UseFadeColor` property.

### Fade color

Controls the color of the fade effect. Alpha is ignored. This setting can be changed at runtime prior to a transition via the service's `FadeColor` property.

### Fade targets

Controls which cameras will have a fade effect applied to them. This setting can be changed at runtime via the service's `FadeTargets` property.

Setting | Targeted Cameras
--- | --- | ---
Main | Applies fade effect to the main camera.
UI | Applies fade effect to cameras on the UI layer. (Does not affect overlay UI)
All | Applies to both main and UI cameras.
Custom | Applies to a custom set of cameras provided via `SetCustomFadeTargetCameras`

### Fade out time / fade in time

Default settings for the duration of a fade on entering / exiting a transition. These settings can be changed at runtime via the service's `FadeOutTime` and `FadeInTime` properties.

### Camera fader type

Which `ICameraFader` class to use for applying a fade effect to cameras. The default `CameraFaderQuad` class instantiates a quad with a transparent material in front of the target camera close to the clip plane. Another approach might be to use a post effects system.

## Using the extension

You use the transition service by passing Tasks that are run while the camera is faded out.

### Using scene system tasks

In most cases you will be using Tasks supplied by the SceneSystem service:

```c#
private async void TransitionToScene()
{
    IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
    ISceneTransitionService transition = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();

    // Fades out
    // Runs LoadContent task
    // Fades back in
    await transition.DoSceneTransition(
            () => sceneSystem.LoadContent("TestScene1")
        );
}
```

### Using custom tasks

In other cases you may want to perform a transition without actually loading a scene:

```c#
private async void TransitionToScene()
{
    ISceneTransitionService transition = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();

    // Fades out
    // Resets scene
    // Fades back in
    await transition.DoSceneTransition(
            () => ResetScene()
        );
}

private async Task ResetScene()
{
    // Go through all enemies in the current scene and move them back to starting positions
}
```

Or you may want to load a scene without using the SceneSystem service:

```c#
private async void TransitionToScene()
{
    ISceneTransitionService transition = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();

    // Fades out
    // Loads scene using Unity's scene manager
    // Fades back in
    await transition.DoSceneTransition(
            () => LoadScene("TestScene1")
        );
}

private async Task LoadScene(string sceneName)
{
    AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    while (!asyncOp.isDone)
    {
        await Task.Yield();
    }
}
```

### Using multiple tasks

You can also supply multiple tasks, which will be executed in order:

```c#
private async void TransitionToScene()
{
    IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
    ISceneTransitionService transition = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();

    // Fades out
    // Sets time scale to 0
    // Fades out audio to 0
    // Loads TestScene1
    // Fades in audio to 1
    // Sets time scale to 1
    // Fades back in
    await transition.DoSceneTransition(
            () => SetTimescale(0f),
            () => FadeAudio(0f, 1f),
            () => sceneSystem.LoadContent("TestScene1"),
            () => FadeAudio(1f, 1f),
            () => SetTimescale(1f)
        );
}

private async Task SetTimescale(float targetTime)
{
    Time.timeScale = targetTime;
    await Task.Yield();
}

private async Task FadeAudio(float targetVolume, float duration)
{
    float startTime = Time.realtimeSinceStartup;
    float startVolume = AudioListener.volume;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
        AudioListener.volume = Mathf.Lerp(startVolume, targetVolume, Time.realtimeSinceStartup - startTime / duration);
        await Task.Yield();
       }
       AudioListener.volume = targetVolume;
}
```

## Using the progress indicator

A progress indicator is anything that implements the `IProgressIndicator` interface. This can take the form of a splash screen, a 3D tagalong loading indicator, or anything else that provides feedback about transition progress.

If `UseDefaultProgressIndicator` is checked in the SceneTransitionService profile, a progress indicator will be instantiated when a transition begins. For the duration of the transition this indicator's `Progress` and `Message` properties can be accessed via that service's `SetProgressValue` and `SetProgressMessage` methods.

```c#
private async void TransitionToScene()
{
    IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
    ISceneTransitionService transition = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();

    ListenToSceneTransition(sceneSystem, transition);

    await transition.DoSceneTransition(
            () => sceneSystem.LoadContent("TestScene1")
        );
}

private async void ListenToSceneTransition(IMixedRealitySceneSystem sceneSystem, ISceneTransitionService transition)
{
    transition.SetProgressMessage("Starting transition...");

    while (transition.TransitionInProgress)
    {
        if (sceneSystem.SceneOperationInProgress)
        {
            transition.SetProgressMessage("Loading scene...");
            transition.SetProgressValue(sceneSystem.SceneOperationProgress);
        }
        else
        {
            transition.SetProgressMessage("Finished loading scene...");
            transition.SetProgressValue(1);
        }

        await Task.Yield();
    }
}
```

Alternatively, when calling `DoSceneTransition` you can supply your own progress indicator via the optional `progressIndicator` argument. This will override the default progress indicator.
