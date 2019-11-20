# Updating the Microsoft Mixed Reality Toolkit

- [2.0.0 to 2.1.0](#updating-200-to-210)
- [RC2 to 2.0.0](#updating-rc2-to-200)

## Updating 2.0.0 to 2.1.0

- [API changes](#api-changes-in-210)
- [Profile changes](#profile-changes-in-210)

### API changes in 2.1.0

**BaseNearInteractionTouchable**

The `BaseNearInteractionTouchable` has been modified to mark the `OnValidate` method as virtual. Classes that extend `BaseNearInteractionTouchable` (ex: `NearInteractionTouchableUnityUI`) have been updated to reflect this change.

**ColliderNearInteractionTouchable**

The `ColliderNearInteractionTouchable` class has been deprecated. Please update code references to use `BaseNearInteractionTouchable`.

**IMixedRealityMouseDeviceManager**

**_Added_**

`IMixedRealityMouseDeviceManager` has been added `CursorSpeed` and `WheelSpeed` properties. These properties allow applications to specify a multiplier value by which the speed of the cursor and wheel, respectively will be scaled.

This is a breaking change and requires existing mouse device manager implementations to be modified .

>[!NOTE]
>This change is not backwards compatible with version 2.0.0.

**_Deprecated_**

The `MouseInputProfile` property has been marked as obsolete and will be removed from a future version of the Microsoft Mixed Reality Toolkit. It is recommended that application code no longer use this property.

**Interactable**

The following methods and properties have been deprecated and will be removed from a future version of the Microsoft Mixed Reality Toolkit. The recommendation is to update application code per the guidance contained in the Obsolete attribute and displayed in the console.

- `public bool Enabled`
- `public bool FocusEnabled`
- `public void ForceUpdateThemes()`
- `public bool IsDisabled`
- `public bool IsToggleButton`
- `public int GetDimensionIndex()`
- `public State[] GetStates()`
- `public bool RequiresFocus`
- `public void ResetBaseStates()`
- `public virtual void SetCollision(bool collision)`
- `public virtual void SetCustom(bool custom)`
- `public void SetDimensionIndex(int index)`
- `public virtual void SetDisabled(bool disabled)`
- `public virtual void SetFocus(bool focus)`
- `public virtual void SetGesture(bool gesture)`
- `public virtual void SetGestureMax(bool gesture)`
- `public virtual void SetGrab(bool grab)`
- `public virtual void SetInteractive(bool interactive)`
- `public virtual void SetObservation(bool observation)`
- `public virtual void SetObservationTargeted(bool targeted)`
- `public virtual void SetPhysicalTouch(bool touch)`
- `public virtual void SetPress(bool press)`
- `public virtual void SetTargeted(bool targeted)`
- `public virtual void SetToggled(bool toggled)`
- `public virtual void SetVisited(bool visited)`
- `public virtual void SetVoiceCommand(bool voice)`

**NearInteractionTouchableSurface**

The `NearInteractionTouchableSurface` class has been added and now serves as the base class for `NearInteractionTouchable` and `NearInteractionTouchableUnityUI`.

### Profile changes in 2.1.0

**Hand tracking profile**

The hand mesh and joint visualizations now have a separate editor and player settings. The hand tracking profile has been updated to allow for setting these visualizations to; Nothing, Everything, Editor or Player.

![Hand visualization modes](Images/ReleaseNotes/HandTrackingVisualizationModes.png)

Custom hand tracking profiles may need to be updated to work correctly with version 2.1.0.

>[!NOTE]
>This change is not backwards compatible with version 2.0.0.

**Input simulation profile**

The input simulation system has been upgraded, which changes a few settings in the input simulation profile. Some changes can not be migrated automatically and users may find that profiles are using default values.

1. All KeyCode and mouse button bindings in the profile have been replaced with a generic `KeyBinding` struct, which stores the type of binding (key or mouse) as well as the actual binding code (KeyCode or mouse button number respectively). The struct has its own inspector, which allows unified display and offers an "auto-bind" tool to quickly set key bindings by pressing the respective key instead of selecting from a huge dropdown list.

    * FastControlKey
    * ToggleLeftHandKey
    * ToggleRightHandKey
    * LeftHandManipulationKey
    * RightHandManipulationKey

1. `MouseLookToggle` was previously included in the `MouseLookButton` enum as `InputSimulationMouseButton.Focused`, it is now a separate option. When enabled, the camera will keep rotating with the mouse after releasing the button, until the escape key is pressed.
1. `HandDepthMultiplier` default value has been lowered from 0.1 to 0.03 to accommodate some changes to the input simulation. If the camera moves too fast when scrolling, try lowering this value.
1. Keys for rotating hands have been removed, hand rotation is now controlled by the mouse as well. Holding `HandRotateButton` (Ctrl) together with the left/right hand manipulation key (LShift/Space) will enable hand rotation.
1. A new axis "UpDown" has been introduced to the input axis list. This controls camera movement in the vertical and defaults to Q/E keys as well as the controller trigger buttons.

For more information on these changes, please see the [input simulation service](InputSimulation/InputSimulationService.md) article.

**Mouse data provider profile**

The mouse data provider profile has been updated to expose the new `CursorSpeed` and `WheelSpeed` properties. Existing custom profiles will automatically have default values provided. When the profile is saved, these new values will be persisted.

**Controller mapping profile**

Some axes and input types have been updated in 2.1.0, especially around the OpenVR platform. Please be sure to select **MixedRealityToolkit -> Utilities -> Update -> Controller Mapping Profiles** when upgrading. This will update any custom Controller Mapping Profiles with the updated axes and data, while leaving your custom-assigned input actions intact.

## Updating RC2 to 2.0.0

Between the RC2 and 2.0.0 releases of the Microsoft Mixed Reality Toolkit, changes were made that may impact existing projects. This document describes those changes and how to update projects to the 2.0.0 release.

- [API changes](#api-changes-in-200)
- [Assembly name changes](#assembly-name-changes-in-200)

### API changes in 2.0.0

Since the release of RC2, there have been a number of API changes including some that may break existing projects. The following sections describe the changes that have occurred between the RC2 and 2.0.0 releases.

**MixedRealityToolkit**

The following public properties on the MixedRealityToolkit object have been deprecated.

- `RegisteredMixedRealityServices` no longer contains the collection of registered extensions services and data providers.

To access extension services, use `MixedRealityServiceRegistry.TryGetService<T>`. To access data providers, cast the service instance to [`IMixedRealityDataProviderAccess`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProviderAccess) and use `GetDataProvider<T>`.

Use [`MixedRealityServiceRegistry`](xref:Microsoft.MixedReality.Toolkit.MixedRealityServiceRegistry) or [`CoreServices`](xref:Microsoft.MixedReality.Toolkit.CoreServices) instead for the following deprecated properties

- `ActiveSystems`
- `InputSystem`
- `BoundarySystem`
- `CameraSystem`
- `SpatialAwarenessSystem`
- `TeleportSystem`
- `DiagnosticsSystem`
- `SceneSystem`

**CoreServices**

The [`CoreServices`](xref:Microsoft.MixedReality.Toolkit.CoreServices) class is the replacement for the static system accessors (ex: BoundarySystem) found in the `MixedRealityToolkit` object.

>[!IMPORTANT]
>The `MixedRealityToolkit` system accessors have been deprecated in version 2.0.0 and will be removed in a future release of the MRTK.

The following code example illustrates the old and the new pattern.

``` c#
// Old
GameObject playAreaVisualization = MixedRealityToolkit.BoundarySystem?.GetPlayAreaVisualization();

// New
GameObject playAreaVisualization = CoreServices.BoundarySystem?.GetPlayAreaVisualization();
```

Using the new CoreSystem class will ensure that your application code will not need updating if you change the application to use a different service registrar (ex: one of the experimental service managers).

**IMixedRealityRaycastProvider**

With the addition of the IMixedRealityRaycastProvider, the input system configuration profile was changed. If you have a custom profile, you may receive the errors in the following image when you run your application.

![Selecting the Raycast provider](Images/ReleaseNotes/UnableToRegisterRaycastProvider.png)

To fix these, please add an IMixedRealityRaycastProvider instance to your input system profile.

![Selecting the Raycast provider](Images/ReleaseNotes/SelectRaycastProvider.png)

**Event System**

- The `IMixedRealityEventSystem` old API methods `Register` and `Unregister` have been marked as obsolete. They are preserved for backwards compatibility.
- `InputSystemGlobalListener` has been marked as obsolete. Its functionality has not changed.
- `BaseInputHandler` base class has been changed from `InputSystemGlobalListener` to `InputSystemGlobalHandlerListener`. This is a breaking change for any descendants of `BaseInputHandler`.

**_Motivation behind the change_**

The old event system API `Register` and `Unregister` could potentially cause multiple issues in runtime, main being:

- If a component registers for global events, it would receive global input events of *all* types.
- If one of the components on an object registers for global input events, all components on this object will receive global input events of *all* types.
- If two components on the same object register to global events, and then one is disabled in runtime, the second one stops receiving global events.

New API `RegisterHandler` and `UnregisterHandler`:

- Provides an explicit and granular control over which input events should be listened to globally and which should be focused-based.
- Allows multiple components on the same object to listen to global events independently on each other.

**_How to migrate_**

- If you have been calling `Register`/`Unregister` API directly before, replace these calls with calls to `RegisterHandler`/`UnregisterHandler`. Use handler interfaces you implement as generic parameters. If you implement multiple interfaces, and several of them listen to global input events, call `RegisterHandler` multiple times.
- If you have been inheriting from `InputSystemGlobalListener`, change inheritance to `InputSystemGlobalHandlerListener`. Implement `RegisterHandlers` and `UnregisterHandlers` abstract methods. In the implementation call `inputSystem.RegisterHandler` (`inputSystem.UnregisterHandler`) to register on all handler interfaces you want to listen global events for.
- If you have been inheriting from `BaseInputHandler`, implement `RegisterHandlers` and `UnregisterHandlers` abstract methods (same as for `InputSystemGlobalListener`).

**_Examples of migration_**

```csharp
// Old
class SampleHandler : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
{
    private void OnEnable()
    {
        InputSystem?.Register(gameObject);
    }

    private void OnDisable()
    {
        InputSystem?.Unregister(gameObject);
    }
}

// Migrated
class SampleHandler : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
{
    private void OnEnable()
    {
        InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

    private void OnDisable()
    {
        InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }
}
```

```csharp
// Old
class SampleHandler2 : InputSystemGlobalListener, IMixedRealitySpeechHandler
{
}

// Migrated
class SampleHandler2 : InputSystemGlobalHandlerListener, IMixedRealitySpeechHandler
{
    private void RegisterHandlers()
    {
        InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    }

    private void UnregisterHandlers()
    {
        InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
    }
}

// Alternative migration
class SampleHandler2 : MonoBehaviour, IMixedRealitySpeechHandler
{
    private void OnEnable()
    {
        IMixedRealityInputSystem inputSystem;
        if (MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            inputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
        }
    }

    private void OnDisable()
    {
        IMixedRealityInputSystem inputSystem;
        if (MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
        {
            inputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
        }
    }
}
```

**Spatial Awareness**

The IMixedRealitySpatialAwarenessSystem and IMixedRealitySpatialAwarenessObserver interfaces have taken multiple breaking changes as described below.

**_Changes_**

The following method(s) have been renamed to better describe their usage.

- `IMixedRealitySpatialAwarenessSystem.CreateSpatialObjectParent` has been renamed to `IMixedRealitySpatialAwarenessSystem.CreateSpatialAwarenessObservationParent` to clarify its usage.

**_Additions_**

Based on customer feedback, support for easy removal of previously observed spatial awareness data has been added.

- `IMixedRealitySpatialAwarenessSystem.ClearObservations()`
- `IMixedRealitySpatialAwarenessSystem.ClearObservations<T>(string name)`
- `IMixedRealitySpatialAwarenessObserver.ClearObservations()`

**Solvers**

Some solver components and the SolverHandler manager class has changed to fix various bugs and for more intuitive usage.

**_SolverHandler_**

- Class no longer extends from `ControllerFinder`
- `TrackedObjectToReference` public property deprecated and has been renamed to `TrackedTargetType`
- `TrackedObjectType` deprecates left & right controller values. Instead use `MotionController` or `HandJoint` values and update new `TrackedHandedness` property to limit tracking to left or right controller

**_InBetween_**

- `TrackedObjectForSecondTransform` public property deprecated and has been renamed to `SecondTrackedObjectType`
- `AttachSecondTransformToNewTrackedObject()` has been removed. To update the solver, modify the public properties (i.e `SecondTrackedObjectType`)

**_SurfaceMagnetism_**

- `MaxDistance` public property deprecated and has been renamed to `MaxRaycastDistance`
- `CloseDistance` public property deprecated and has been renamed to `ClosestDistance`
- Default value for `RaycastDirectionMode` is now `TrackedTargetForward` which raycasts in the direction of the tracked target transform forward
- `OrientationMode` enum values, `Vertical` and `Full`, have been renamed to `TrackedTarget` and `SurfaceNormal` respectively
- `KeepOrientationVertical` public property has been added to control whether orientation of associated GameObject remains vertical

**Buttons**

- [`PressableButton`](xref:Microsoft.MixedReality.Toolkit.UI.PressableButton) now has `DistanceSpaceMode` property set to `Local` as default. This allows buttons to be scaled while still be pressable

**Clipping Sphere**

The ClippingSphere interface has changed to mirror the APIs found in the ClippingBox and ClippingPlane.

The ClippingSphere's Radius property is now implicitly calculated based on the transform scale. Before developers would have to specify the radius of the ClippingSphere in the inspector. If you want to change the radius, just update the transform scale of the transform as you normally would.

**NearInteractionTouchable and PokePointer**

- NearInteractionTouchable does not handle Unity UI canvas touching any longer. The NearInteractionTouchableUnityUI class must be used for Unity UI touchables now.
- ColliderNearInteractionTouchable is the new base class for touchables based on colliders, i.e. every touchable except NearInteractionTouchableUnityUI.
- BaseNearInteractionTouchable.DistFront has been moved and renamed to PokePointer.TouchableDistance
    This is the distance and which the PokePointer can interact with touchables. Previously each touchable had it's own maximum interaction distance, but now this is defined in the PokePointer which allows better optimization.
- BaseNearInteractionTouchable.DistBack has been renamed to PokeThreshold
    This makes it clear that PokeThreshold is the counterpart to DebounceThreshold. A touchable is activated when the PokeThreshold is crossed, and released when DebounceThreshold is crossed.

**ReadOnlyAttribute**

The `Microsoft.MixedReality.Toolkit` namespace has been added to `ReadOnlyAttribute`, `BeginReadOnlyGroupAttribute`, and `EndReadOnlyGroupAttribute`.

**PointerClickHandler**

The `PointerClickHandler` class has been deprecated. The `PointerHandler` should be used instead, it provides the same functionality.

**HoloLens clicker support**

The HoloLens clicker's controller mappings have changed from being an unhanded [`WindowsMixedRealityController`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityController) to being an unhanded [`WindowsMixedRealityGGVHand`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityGGVHand). To account for this, an automatic updater will run the first time you open your ControllerMapping profile. Please open any custom profiles at least once after upgrading to 2.0.0 in order to trigger this one-time migration step.

**InteractableHighlight**

The `InteractableHighlight` class has been deprecated. The `InteractableOnFocus` class and `FocusInteractableStates` asset should be used instead. To create a new `Theme` asset for the `InteractableOnFocus`, right click in the project window and select *Create* > *Mixed Reality Toolkit* > *Interactable* > *Theme*.

**HandInteractionPanZoom**

`HandInteractionPanZoom` has been moved to the UI namespace as it was not an input component. `HandPanEventData` has also been moved into this namespace, and simplified to correspond with other UI event data.

### Assembly name changes in 2.0.0

In The 2.0.0 release, all of the official Mixed Reality Toolkit assembly names and their associated assembly definition (.asmdef) files have been updated to fit the following pattern.

```c#
Microsoft.MixedReality.Toolkit[.<name>]
```

In some instances, multiple assemblies have been merged to create better unity of their contents. If your project uses custom .asmdef files, they may require updating.

The following tables describe how the RC2 .asmdef file names map to the 2.0.0 release. All assembly names match the .asmdef file name.

**MixedRealityToolkit**

| RC2 | 2.0.0 |
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

**MixedRealityToolkit.Providers**

| RC2 | 2.0.0 |
| --- | --- |
| MixedRealityToolkit.Providers.OpenVR.asmdef | Microsoft.MixedReality.Toolkit.Providers.OpenVR.asmdef |
| MixedRealityToolkit.Providers.WindowsMixedReality.asmdef | Microsoft.MixedReality.Toolkit.Providers.WindowsMixedReality.asmdef |
| MixedRealityToolkit.Providers.WindowsVoiceInput.asmdef | Microsoft.MixedReality.Toolkit.Providers.WindowsVoiceInput.asmdef |

**MixedRealityToolkit.Services**

| RC2 | 2.0.0 |
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

**MixedRealityToolkit.SDK**

| RC2 | 2.0.0 |
| --- | --- |
| MixedRealityToolkit.SDK.asmdef | Microsoft.MixedReality.Toolkit.SDK.asmdef |
| MixedRealityToolkit.SDK.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.SDK.Inspectors.asmdef |

**MixedRealityToolkit.Examples**

| RC2 | 2.0.0 |
| --- | --- |
| MixedRealityToolkit.Examples.asmdef | Microsoft.MixedReality.Toolkit.Examples.asmdef |
| MixedRealityToolkit.Examples.Demos.Gltf.asmdef | Microsoft.MixedReality.Toolkit.Demos.Gltf.asmdef |
| MixedRealityToolkit.Examples.Demos.StandardShader.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.Demos.StandardShader.Inspectors.asmdef |
| MixedRealityToolkit.Examples.Demos.Utilities.InspectorFields.asmdef | Microsoft.MixedReality.Toolkit.Demos.InspectorFields.asmdef |
| MixedRealityToolkit.Examples.Demos.Utilities.InspectorFields.Inspectors.asmdef | Microsoft.MixedReality.Toolkit.Demos.InspectorFields.Inspectors.asmdef |
| MixedRealityToolkit.Examples.Demos.UX.Interactables.asmdef | Microsoft.MixedReality.Toolkit.Demos.UX.Interactables.asmdef |
