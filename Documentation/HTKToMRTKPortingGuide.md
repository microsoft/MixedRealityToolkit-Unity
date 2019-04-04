# Porting Guide

## Controller and hand input

### Setup and configuration

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Type                      | Specific events for buttons, with input type info when relevant. | Action / Gesture based input, passed along via events. |
| Setup                     | Place the InputManager in the scene. | Enable the input system in the Configuration Profile and specify a concrete input system type. |
| Configuration             | Configured in the Inspector, on each individual script in the scene. | Configured via the Mixed Reality Input System Profile and its related profile, listed below. |

Related profiles:
* Mixed Reality Controller Mapping Profile
* Mixed Reality Controller Visualization Profile
* Mixed Reality Gestures Profile
* Mixed Reality Input Actions Profile
* Mixed Reality Input Action Rules Profile
* Mixed Reality Pointer Profile

Gaze Provider settings are modified on the Main Camera object in the scene.

Platform support components (e.g., Windows Mixed Reality Device Manager) must be added to the Mixed Reality Registered Services Profile.

### Interface and event mappings

Some events no longer have unique events and now contain a MixedRealityInputAction. These actions are specified in the Input Actions profile and mapped to specific controllers and platforms in the Controller Mapping profile. Events like `OnInputDown` should now check the MixedRealityInputAction type.

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
| Configuration             | Camera settings configuration performed on prefab instance. | Camera settings configured in Mixed Reality Camera Profile. |

## Speech

### Keyword Recognition

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add a SpeechInputSource to your scene. | Keyword service (e.g., Windows Speech Input Manager) must be added to the Mixed Reality Registered Services Profile. |
| Configuration             | Recognized keywords are configured in the SpeechInputSource’s inspector. | Keywords are configured in the Mixed Reality Speech Commands Profile. |
| Event handlers            | `ISpeechHandler` | `IMixedRealitySpeechHandler` |

### Dictation

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add a DictationInputManager to your scene. | Dictation support requires service (e.g., Windows Dictation Input Manager) to be added to the Mixed Reality Registered Services Profile. |
| Event handlers            | `IDictationHandler` | `IMixedRealityDictationHandler` |

## Spatial Awareness / Mapping

### Mesh

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Add the SpatialMapping prefab to the scene. | Enable the Spatial Awareness System in the Configuration Profile and add a spatial observer (e.g., Windows Mixed Reality Spatial Mesh Observer) to the Registered Services Profile. |
| Configuration             | Configure the scene instance in the inspector. | Configure the settings on each spatial observer's profile. |

### Planes

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| Setup                     | Use the `SurfaceMeshesToPlanes` script. | Not yet implemented. |

### Spatial Understanding

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
| Setup                     | Sharing service: Add Sharing prefab to the scene. UNet: Use SharingWithUNET example. | In-progress in GitHub feature branch (feature/mrtk_sharing) |
| Configuration             | Configure the scene instances in the inspector. | In-progress in GitHub feature branch (feature/mrtk_sharing) |

## Solvers

Solvers are configured and should behave the same way as the HoloToolkit.

## Utilities

Some Utilities have been reconciled as duplicates with the Solver system. Please file an issue if any of your needed scripts are missing.

| HTK 2017 |  MRTK v2  |
|----------|-----------|
| Billboard.cs | RadialView.cs |
| Tagalong.cs | RadialView.cs or Orbital.cs |
| FixedAngularSize.cs | ConstantViewSize.cs |
| FpsDisplay.cs | Diagnostics System (in Configuration Profile) |
| NearFade.cs | Built-in to MixedRealityStandard.shader |

## UX

### Object Collection

Object collections are configured and should behave the same way as the HoloToolkit.