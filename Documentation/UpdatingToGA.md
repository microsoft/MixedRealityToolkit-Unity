# Updating to the General Availability (GA) Release

Between the RC2 and GA releases of the Microsoft Mixed Reality Toolkit, changes were made that may impact existing projects. This document describes those changes and how to update projects to the GA release.

- [API changes](#api-changes)
- [Assembly name changes](#assembly-name-changes)

## API changes

Since the release of RC2, there have been a number of API changes including some that may break existing projects. The following sections describe the changes that have occurred between the RC2 and GA releases.

- [Event System](#event-system)
- [Spatial Awareness](#spatial-awareness)

### Event System

**Changes**

- The `IMixedRealityEventSystem` old API methods `Register` and `Unregister` have been marked as obsolete. They are preserved for backwards compatibility.
- `InputSystemGlobalListener` has been marked as obsolete. Its functionality has not changed.
- `BaseInputHandler` base class has been changed from `InputSystemGlobalListener` to `InputSystemGlobalHandlerListener`. This is a breaking change for any descendants of `BaseInputHandler`.

**Motivation behind the change**

The old event system API `Register` and `Unregister` could potentially cause multiple issues in runtime, main being:

- If a component registers for global events, it would receive global input events of *all* types.
- If one of the components on an object registers for global input events, all components on this object will receive global input events of *all* types.
- If two components on the same object register to global events, and then one is disabled in runtime, the second one stops receiving global events.

New API `RegisterHandler` and `UnregisterHandler`:

- Provides an explicit and granular control over which input events should be listened to globally and which should be focused-based.
- Allows multiple components on the same object to listen to global events independently on each other.

**How to migrate**

- If you have been calling `Register`/`Unregister` API directly before, replace these calls with calls to `RegisterHandler`/`UnregisterHandler`. Use handler interfaces you implement as generic parameters. If you implement multiple interfaces, and several of them listen to global input events, call `RegisterHandler` multiple times.
- If you have been inheriting from `InputSystemGlobalListener`, change inheritance to `InputSystemGlobalHandlerListener`. Implement `RegisterHandlers` and `UnregisterHandlers` abstract methods. In the implementation call `inputSystem.RegisterHandler` (`inputSystem.UnregisterHandler`) to register on all handler interfaces you want to listen global events for.
- If you have been inheriting from `BaseInputHandler`, implement `RegisterHandlers` and `UnregisterHandlers` abstract methods (same as for `InputSystemGlobalListener`).

**Examples of migration**

<pre><code>
// Old
class SampleHandler : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
{
    private void OnEnable()
    {
        <b>InputSystem?.Register(gameObject);</b>
    }

    private void OnDisable()
    {
        <b>InputSystem?.Unregister(gameObject);</b>
    }
}

// Migrated
class SampleHandler : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
{
    private void OnEnable()
    {
        <b>InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);</b>
        <b>InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);</b>
    }

    private void OnDisable()
    {
        <b>InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);</b>
        <b>InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);</b>
    }
}
</code></pre>

<pre><code>
// Old
class SampleHandler2 : <b>InputSystemGlobalListener</b>, IMixedRealitySpeechHandler
{
}

// Migrated
class SampleHandler2 : <b>InputSystemGlobalHandlerListener</b>, IMixedRealitySpeechHandler
{
    <b>private void RegisterHandlers()
    {
        InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    }

    private void UnregisterHandlers()
    {
        InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
    }</b>
}

// Alternative migration
class SampleHandler2 : <b>MonoBehaviour</b>, IMixedRealitySpeechHandler
{
    private void <b>OnEnable</b>()
    {
        IMixedRealityInputSystem inputSystem;
        if (MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            inputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
        }
    }

    private void <b>OnDisable</b>()
    {
        IMixedRealityInputSystem inputSystem;
        if (MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            inputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
        }
    }</b>
}
</code></pre>

### Spatial Awareness

The IMixedRealitySpatialAwarenessSystem and IMixedRealitySpatialAwarenessObserver interfaces have taken multiple breaking changes as described below.

**Changes**

The following method(s) have been renamed to better describe their usage.

- IMixedRealitySpatialAwarenessSystem.CreateSpatialObjectParent has been renaned to IMixedRealitySpatialAwarenessSystem.CreateSpatialAwarenessObservationParent to clarify its usage.

**Additions**

Based on customer feedback, support for easy removal of previously observed spataial awareness data has been added.

- IMixedRealitySpatialAwarenessSystem.ClearObservations()
- IMixedRealitySpatialAwarenessSystem.ClearObservations\<T\>(string name)
- IMixedRealitySpatialAwarenessObserver.ClearObservations()

### NearInteractionTouchable and PokePointer

- NearInteractionTouchable does not handle Unity UI canvas touching any longer. The NearInteractionTouchableUnityUI class must be used for Unity UI touchables now.
- ColliderNearInteractionTouchable is the new base class for touchables based on colliders, i.e. every touchable except NearInteractionTouchableUnityUI.
- BaseNearInteractionTouchable.DistFront has been moved and renamed to PokePointer.TouchableDistance
    This is the distance and which the PokePointer can interact with touchables. Previously each touchable had it's own maximum interaction distance, but now this is defined in the PokePointer which allows better optimization.
- BaseNearInteractionTouchable.DistBack has been renamed to PokeThreshold
    This makes it clear that PokeThreshold is the counterpart to DebounceThreshold. A touchable is activated when the PokeThreshold is crossed, and released when DebounceThreshold is crossed.

## Assembly name changes

In The GA release, all of the official Mixed Reality Toolkit assembly names and their associated assembly definition (.asmdef) files have been updated to fit the following pattern.

```
Microsoft.MixedReality.Toolkit[.<name>]
```

In some instances, multiple assemblies have been merged to create better unity of their contents. If your project uses custom .asmdef files, they may require updating.

The following tables describe how the RC2 .asmdef file names map to the GA release. All assembly names match the .asmdef file name. 

### MixedRealityToolkit

| RC2 | GA |
| --- | --- |
| MixedRealityToolkit.asmdef | Microsoft.MixedReality.Toolkit.asmdef |
| MixedRealityToolkit.Core.BuildAndDeploy.asmdef | Microsoft.MixedReality.Toolkit.Editor.BuildAndDeploy.asmdef |
| MixedRealityToolkit.Core.Definitions.Utilities.Editor.asmdef | Removed, use Microsoft.MixedReality.Toolkit.Editor.Utilities.asmdef |
| MixedRealityToolkit.Core.Extensions.EditorClassExtensions.asmdef | Microsoft.MixedReality.Toolkit.Editor.ClassExtensions.asmdef
| MixedRealityToolkit.Core.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.Editor.Inspectors.asmdef |
| MixedRealityToolkit.Core.Inspectors.ServiceInspectors.asmdef | Microsoft.MixedReality.Toolkit.Editor.ServiceInspectors.asmdef |
| MixedRealityToolkit.Core.UtilitiesAsync.asmdef | Microsoft.MixedReality.Toolkit.Async.asmdef |
| MixedRealityToolkit.Core.Utilities.Editor.asmdef | Microsoft.MixedReality.Toolkit.Editor.Utilities.asmdef |
| MixedRealityToolkit.Utilities.Gltf.asmdef | Microsoft.MixedReality.Toolkit.Gltf.asmdef |
| MixedRealityToolkit.Utilities.Gltf.Importers.asmdef | Microsoft.MixedReality.Toolkit.Gltf.Importers.asmdef |

### MixedRealityToolkit.Providers

| RC2 | GA |
| --- | --- |
| MixedRealityToolkit.Providers.OpenVR.asmdef | Microsoft.MixedReality.Toolkit.Providers.OpenVR.asmdef |
| MixedRealityToolkit.Providers.WindowsMixedReality.asmdef | Microsoft.MixedReality.Toolkit.Providers.WindowsMixedReality.asmdef |
| MixedRealityToolkit.Providers.WindowsVoiceInput.asmdef | Microsoft.MixedReality.Toolkit.Providers.WindowsVoiceInput.asmdef |

### MixedRealityToolkit.Services

| RC2 | GA |
| --- | --- |
| MixedRealityToolkit.Services.BoundarySystem.asmdef | Microsoft.MixedReality.Toolkit.Services.BoundarySystem.asmdef |
| MixedRealityToolkit.Services.CameraSystem.asmdef | Microsoft.MixedReality.Toolkit.Services.CameraSystem.asmdef |
| MixedRealityToolkit.Services.DiagnosticsSystem.asmdef | Microsoft.MixedReality.Toolkit.Services.DiagnosticsSystem.asmdef |
| MixedRealityToolkit.Services.InputSimulation.asmdef | Microsoft.MixedReality.Toolkit.Services.InputSimulation.asmdef |
| MixedRealityToolkit.Services.InputSimulation.Editor.asmdef | Microsoft.MixedReality.Toolkit.Services.InputSimulation.Editor.asmdef | 
| MixedRealityToolkit.Services.InputSystem.asmdef | Microsoft.MixedReality.Toolkit.Services.InputSystem.asmdef |
| MixedRealityToolkit.Services.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.Services.InputSystem.Editor.asmdef |
| MixedRealityToolkit.Services.SceneSystem.asmdef | Microsoft.MixedReality.Toolkit.Services.SceneSystem.asmdef |
| MixedRealityToolkit.Services.SpatialAwarenessSystem.asmdef | Microsoft.MixedReality.Toolkit.Services.SpatialAwarenessSystem.asmdef |
| MixedRealityToolkit.Services.TeleportSystem.asmdef | Microsoft.MixedReality.Toolkit.Services.TeleportSystem.asmdef |


### MixedRealityToolkit.SDK

| RC2 | GA |
| --- | --- |
| MixedRealityToolkit.SDK.asmdef | Microsoft.MixedReality.Toolkit.SDK.asmdef |
| MixedRealityToolkit.SDK.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.SDK.Inspectors.asmdef |

### MixedRealityToolkit.Examples

| RC2 | GA |
| --- | --- |
| MixedRealityToolkit.Examples.asmdef | Microsoft.MixedReality.Toolkit.Examples.asmdef |
| MixedRealityToolkit.Examples.Demos.Gltf.asmdef | Microsoft.MixedReality.Toolkit.Demos.Gltf.asmdef |
| MixedRealityToolkit.Examples.Demos.StandardShader.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.Demos.StandardShader.Inspectors.asmdef |
| MixedRealityToolkit.Examples.Demos.Utilities.InspectorFields.asmdef | Microsoft.MixedReality.Toolkit.Demos.InspectorFields.asmdef |
| MixedRealityToolkit.Examples.Demos.Utilities.InspectorFields.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.Demos.InspectorFields.Inspectors.asmdef |
| MixedRealityToolkit.Examples.Demos.UX.Interactables.asmdef | Microsoft.MixedReality.Toolkit.Demos.UX.Interactables.asmdef |
