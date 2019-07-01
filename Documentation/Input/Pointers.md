# Pointers

Pointers are instanced automatically at runtime when a new controller is detected. You can have more than one pointer attached to a controller; for example, with the default pointer profile, WMR controllers get both a line and a parabolic pointer for normal selection and teleportation respectively. Pointers communicate with each other to decide which one is active. The pointers created for each controller are set up in the **Pointer Profile**, under the *Input System Profile*.

<img src="../../Documentation/Images/Input/PointerProfile.png" style="max-width:100%;">

MRTK provides a set of pointer prefabs in *Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Pointers*. You can use your own prefabs as long as they contain one of the pointer scripts in *Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Pointers* or any other script implementing [`IMixedRealityPointer`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer).

## Events

These are all the pointer-related events raised by MRTK:

Event | Description | Handler
--- | --- | ---
Before Focus Changed / Focus Changed | Raised on both the game object losing focus and the one gaining it every time a pointer changes focus. | [`IMixedRealityFocusChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusChangedHandler)
Focus Enter / Exit | Raised on the game object gaining focus when the first pointer enters it and on the one losing focus when the last pointer leaves it. | [`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusHandler)
Pointer Down / Dragged / Up / Clicked | Raised to report pointer press, drag and release. | [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler)
Touch Started / Updated / Completed | Raised by touch-aware pointers like [**PokePointer**](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer) to report touch activity. | [`IMixedRealityTouchHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityTouchHandler)

To handle them you must implement the corresponding interface in your script. Aditionally, if you want your game object to receive the events raised on all game objects instead of just the ones raised on itself and its children, you must register as a global event listener calling [`Register`](xref:Microsoft.MixedReality.Toolkit.IMixedRealityEventSystem.Register*) on the input system.

> [!WARNING]
> [`IMixedRealityFocusChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusChangedHandler) and [`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusHandler) are not raised as global events. You can only handle them in the objects they are raised on.

Alternatively, you can derive your handler script from [`InputSystemGlobalListener`](xref:Microsoft.MixedReality.Toolkit.Input.InputSystemGlobalListener) if you require global registration or from [`BaseInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.BaseInputHandler) if you want to choose in the inspector whether to register as global listener.

For pointer events, i.e. the ones handled by [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler), we provide further conveniency in the form of [`PointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.PointerHandler). This script allows you to handle pointer events directly via Unity Events.

<img src="../../Documentation/Images/Input/PointerHandler.png" style="max-width:100%;">

## Pointer Result

The pointer [`Result`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer.Result) property contains the current result for the scene query used to determine the object with focus. For a raycast pointer, like the ones created by default for motion controllers, gaze input and hand rays, it will contain the location and normal of the raycast hit.

## Example Scene

The **Pointer Result Example** in `MixedRealityToolkit.Examples\Demos\Input\Scenes\PointerResult` shows how to use the pointer Result to spawn an object at the hit location.

<img src="../../Documentation/Images/Input/PointerResultExample.png" style="max-width:100%;">