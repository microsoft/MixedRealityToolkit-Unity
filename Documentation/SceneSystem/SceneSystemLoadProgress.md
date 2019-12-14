# Monitoring content loading

## Scene operation progress

When content is being loaded or unloaded, the `SceneOperationInProgress` property will return true. You can monitor the progress of this operation via the `SceneOperationProgress` property.

The `SceneOperationProgress` value is the average of all current async scene operations. At the start of a content load, `SceneOperationProgress` will be zero. Once fully completed, `SceneOperationProgress` will be set to 1 and will remain at 1 until the next operation takes place. Note that only content scene operations affect these properties.

These properties reflect the state of an *entire operation* from start to finish, even if that operation includes multiple steps:

```c#
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

// First do an additive scene load
// SceneOperationInProgress will be true for the duration of this operation
// SceneOperationProgress will show 0-1 as it completes
await sceneSystem.LoadContent("ContentScene1");

// Now do a single scene load
// This will result in two actions back-to-back
// First "ContentScene1" will be unloaded
// Then "ContentScene2" will be loaded
// SceneOperationInProgress will be true for the duration of this operation
// SceneOperationProgress will show 0-1 as it completes
sceneSystem.LoadContent("ContentScene2", LoadSceneMode.Single)
```

### Progress examples

`SceneOperationInProgress` can be useful if activity should be suspended while content is being loaded:

```c#
public class FooManager : MonoBehaviour
{
    private void Update()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        // Don't update foos while a scene operation is in progress
        if (sceneSystem.SceneOperationInProgress)
        {
            return;
        }

        // Update foos
        ...
    }
    ...
}
```

`SceneOperationProgress` can be used to display progress dialogs:

```c#
public class ProgressDialog : MonoBehaviour
{
    private void Update()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        if (sceneSystem.SceneOperationInProgress)
        {
            DisplayProgressIndicator(sceneSystem.SceneOperationProgress);
        }
        else
        {
            HideProgressIndicator();
        }
    }
    ...
}
```

---

## Monitoring with actions

The Scene System provides several actions to let you know when scenes are being loaded or unloaded. Each action relays the name of the affected scene.

If a load or unload operation involves multiple scenes, the relevant actions will be invoked once per affected scene. They are also invoked all at once when the load or unload operation is *fully completed.* For this reason it's recommended that you use *OnWillUnload* actions to detect content that *will* be destroyed, as opposed to using *OnUnloaded* actions to detect destroyed content after the fact.

On the flip side, because *OnLoaded* actions are only invoked when all scenes are activated and fully loaded, using *OnLoaded* actions to detect and use new content is guaranteed to be safe.

Action | When it's invoked | Content Scenes | Lighting Scenes | Manager Scenes
--- | --- | --- | --- | --- | ---
`OnWillLoadContent` | Just prior to a content scene load | • | |  
`OnContentLoaded` | After all content scenes in a load operation have been fully loaded and activated | • | |
`OnWillUnloadContent` | Just prior to a content scene unload operation | • | |
`OnContentUnloaded` | After all content scenes in an unload operation have been fully unloaded | • | |
`OnWillLoadLighting` | Just prior to a lighting scene load | | • |
`OnLightingLoaded` | After a lighting scene has been fully loaded and activated| | • |
`OnWillUnloadLighting` | Just prior to a lighting scene unload | | • |
`OnLightingUnloaded` | After a lighting scene has been fully unloaded | | • |
`OnWillLoadScene` | Just prior to a scene load | • | • | •
`OnSceneLoaded` | After all scenes in an operation are fully loaded and activated | • | • | •
`OnWillUnloadScene` | Just prior to a scene unload | • | • | •
`OnSceneUnloaded` | After a scene is fully unloaded |  • | • | •

### Action examples

Another progress dialog example using actions and a coroutine instead of Update:

```c#
public class ProgressDialog : MonoBehaviour
{
    private bool displayingProgress = false;

    private void Start()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        sceneSystem.OnWillLoadContent += HandleSceneOperation;
        sceneSystem.OnWillUnloadContent += HandleSceneOperation;
    }

    private void HandleSceneOperation (string sceneName)
    {
        // This may be invoked multiple times per frame - once per scene being loaded or unloaded.
        // So filter the events appropriately.
        if (displayingProgress)
        {
            return;
        }

        displayingProgress = true;
        StartCoroutine(DisplayProgress());
    }

    private IEnumerator DisplayProgress()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        while (sceneSystem.SceneOperationInProgress)
        {
            DisplayProgressIndicator(sceneSystem.SceneOperationProgress);
            yield return null;
        }

        HideProgressIndicator();
        displayingProgress = false;
    }

    ...
}
```

---

## Controlling scene activation

By default content scenes are set to activate when loaded. If you want to control scene activation manually, you can pass a `SceneActivationToken` to any content load method. If multiple content scenes are being loaded by a single operation, this activation token will apply to all scenes.

```c#
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

SceneActivationToken activationToken = new SceneActivationToken();

// Load the content and pass the activation token
sceneSystem.LoadContent(new string[] { "ContentScene1", "ContentScene2", "ContentScene3" }, LoadSceneMode.Additive, activationToken);

// Wait until all users have joined the experience
while (!AllUsersHaveJoinedExperience())
{
    await Task.Yield();
}

// Let scene system know we're ready to activate all scenes
activationToken.AllowSceneActivation = true;

// Wait for all scenes to be fully loaded and activated
while (sceneSystem.SceneOperationInProgress)
{
    await Task.Yield();
}

// Proceed with experience
```

---

## Checking which content is loaded

The `ContentSceneNames` property provides an array of available content scenes in order of build index. You can check whether these scenes are loaded via `IsContentLoaded(string contentName)`.

```c#
IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

string[] contentSceneNames = sceneSystem.ContentSceneNames;
bool[] loadStatus = new bool[contentSceneNames.Length];

for (int i = 0; i < contentSceneNames.Length; i++>)
{
    loadStatus[i] = sceneSystem.IsContentLoaded(contentSceneNames[i]);
}
```
