# Porting guide

## Controller and hand input

### Setup and configuration

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Type                      | Specific events for buttons, with input type info when relevant. | Action / Gesture based input, passed along via events. |
| Setup                     | Place the InputManager in the scene. | Enable the input system in the [Configuration Profile](MixedRealityConfigurationGuide.md) and specify a concrete input system type. |
| Configuration             | Configured in the Inspector, on each individual script in the scene. | Configured via the Mixed Reality Input System Profile and its related profile, listed below. |

Related profiles:

* Mixed Reality Controller Mapping Profile
* Mixed Reality Controller Visualization Profile
* Mixed Reality Gestures Profile
* Mixed Reality Input Actions Profile
* Mixed Reality Input Action Rules Profile
* Mixed Reality Pointer Profile

[Gaze Provider](xref:Microsoft.MixedReality.Toolkit.Input.GazeProvider) settings are modified on the Main Camera object in the scene.

Platform support components (e.g., Windows Mixed Reality Device Manager) must be added to their corresponding service's data providers.

### Interface and event mappings

Some events no longer have unique events and now contain a [MixedRealityInputAction](Input/InputActions.md). These actions are specified in the Input Actions profile and mapped to specific controllers and platforms in the Controller Mapping profile. Events like `OnInputDown` should now check the MixedRealityInputAction type.

Related input systems:

* [Input Overview](Input/Overview.md)
* [Input Events](Input/InputEvents.md)
* [Input Pointers](Input/Pointers.md)

| HTK 2017 |  MRTK v2  | Action Mapping |
|----------|-----------|----------------|
| `IControllerInputHandler` | [`IMixedRealityInputHandler<Vector2>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | Mapped to the touchpad or thumbstick |
| `IControllerTouchpadHandler` | [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler) | Mapped to the touchpad |
| `IFocusable` | [`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusHandler) | |
| `IGamePadHandler` | [`IMixedRealitySourceStateHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySourceStateHandler) | |
| `IHoldHandler` | [`IMixedRealityGestureHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler) | Mapped to hold in the Gestures Profile |
| `IInputClickHandler` | [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler) |
| `IInputHandler` | [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler) | Mapped to the controller’s buttons or hand tap |
| `IManipulationHandler` | [`IMixedRealityGestureHandler<Vector3>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) | Mapped to manipulation in the Gestures Profile |
| `INavigationHandler` | [`IMixedRealityGestureHandler<Vector3>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) | Mapped to navigation in the Gestures Profile |
| `IPointerSpecificFocusable` | [`IMixedRealityFocusChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusChangedHandler) | |
| `ISelectHandler` | [`IMixedRealityInputHandler<float>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | Mapped to trigger position |
| `ISourcePositionHandler` | [`IMixedRealityInputHandler<Vector3>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) or [`IMixedRealityInputHandler<MixedRealityPose>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | Mapped to pointer position or grip position |
| `ISourceRotationHandler` | [`IMixedRealityInputHandler<Quaternion>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) or [`IMixedRealityInputHandler<MixedRealityPose>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | Mapped to pointer position or grip position |
| `ISourceStateHandler` | [`IMixedRealitySourceStateHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySourceStateHandler) | |
| `IXboxControllerHandler` | [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler) and [`IMixedRealityInputHandler<Vector2>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | Mapped to the various controller buttons and thumbsticks |

## Camera

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Delete MainCamera, add MixedRealityCameraParent / MixedRealityCamera / HoloLensCamera prefab to scene **or** use Mixed Reality Toolkit > Configure > Apply Mixed Reality Scene Settings menu item. | MainCamera parented under MixedRealityPlayspace via Mixed Reality Toolkit > Add to Scene and Configure... |
| Configuration             | Camera settings configuration performed on prefab instance. | Camera settings configured in the [Mixed Reality Camera Profile](xref:Microsoft.MixedReality.Toolkit.MixedRealityCameraProfile). |

## Speech

### Keyword recognition

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add a SpeechInputSource to your scene. | Keyword service (e.g., Windows Speech Input Manager) must be added to the input system's data providers. |
| Configuration             | Recognized keywords are configured in the SpeechInputSource’s inspector. | Keywords are configured in the [Mixed Reality Speech Commands Profile](Input/Speech.md). |
| Event handlers            | `ISpeechHandler` | [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) |

### Dictation

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add a DictationInputManager to your scene. | Dictation support requires service (e.g., Windows Dictation Input Manager) to be added to the Input System's data providers. |
| Event handlers            | `IDictationHandler` | `IMixedRealityDictationHandler`[`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) |

## Spatial awareness / mapping

### Mesh

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add the SpatialMapping prefab to the scene. | Enable the Spatial Awareness System in the Configuration Profile and add a spatial observer (e.g., Windows Mixed Reality Spatial Mesh Observer) to the Spatial Awareness System's data providers. |
| Configuration             | Configure the scene instance in the inspector. | Configure the settings on each spatial observer's profile. |

### Planes

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Use the `SurfaceMeshesToPlanes` script. | Not yet implemented. |

### Spatial understanding

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add the SpatialUnderstanding prefab to the scene. | Not yet implemented. |
| Configuration             | Configure the scene instance in the inspector. | Not yet implemented. |

## Boundary

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add the `BoundaryManager` script to the scene. | Enable the Boundary System in the Configuration Profile. |
| Configuration             | Configure the scene instance in the inspector. | Configure the settings in the Boundary Visualization profile. |

## Sharing

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Sharing service: Add Sharing prefab to the scene. UNet: Use SharingWithUNET example. | In-progress |
| Configuration             | Configure the scene instances in the inspector. | In-progress |

## UX

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Button                     | [Interactable Objects](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_InteractableObjectExample.md) | [Button](README_Button.md) |
| Interactable                     | [Interactable Objects](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_InteractableObjectExample.md) | [Interactable](README_Interactable.md) |
| Bounding Box             | [Bounding Box](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_BoundingBoxGizmoExample.md) | [Bounding Box](README_BoundingBox.md) |
| App Bar             | [App Bar](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_BoundingBoxGizmoExample.md) | [App Bar](README_AppBar.md) |
| One Hand Manipulation (Grb and Move)   | [HandDraggable](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit/Input/Scripts/Utilities/Interactions/HandDraggable.cs) | [Manipulation Handler](README_ManipulationHandler.md) |
| Two Hand Manipulation (Grab/Move/Rotate/Scale)             | [TwoHandManipulatable](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit/Input/Scripts/Utilities/Interactions/TwoHandManipulatable.cs) | [Manipulation Handler](README_ManipulationHandler.md) |
| Keyboard             | [Keyboard prefab]() | [System Keyboard](README_SystemKeyboard.md) |
| Tooltip             | [Tooltip](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_TooltipExample.md) | [Tooltip](README_Tooltip.md) |
| Object Collection             | [Object Collection](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_ObjectCollection.md) | [Object Collection](README_ObjectCollection.md) |
| Solver             | [Solver](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/Utilities/Readme/README_SolverSystem.md) | [Solver](README_Solver.md) |

## Utilities

Some Utilities have been reconciled as duplicates with the Solver system. Please file an issue if any of the scripts you need are missing.

| HTK 2017 |  MRTK v2  |
|----------|-----------|
| Billboard | [`Billboard`](xref:Microsoft.MixedReality.Toolkit.UI.Billboard) |
| Tagalong | [`RadialView`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.RadialView) or [`Orbital`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.Orbital) [Solver](README_Solver.md) |
| FixedAngularSize | [`ConstantViewSize`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.ConstantViewSize) [Solver](README_Solver.md) |
| FpsDisplay | [Diagnostics System](Diagnostics/DiagnosticsSystemGettingStarted.md) (in Configuration Profile) |
| NearFade | Built-in to [Mixed Reality Toolkit Standard shader](README_MRTKStandardShader.md) |
