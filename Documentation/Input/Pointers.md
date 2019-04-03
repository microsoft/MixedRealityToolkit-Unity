# Pointers

Pointers are instanced automatically at runtime when a new controller is detected. You can have more than one pointer attached to a controller; for example, with the default pointer profile, WMR controllers get both a line and a parabolic pointer for normal selection and teleportation respectively. Pointers communicate with each other to decide which one is active. The pointers created for each controller are set up in the **Pointer Profile**, under the *Input System Profile*.

<img src="../../External/ReadMeImages/Input/PointerProfile.png" style="max-width:100%;">

MRTK provides a set of pointer prefabs in *Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Pointers*. You can use your own prefabs as long as they contain one of the pointer scripts in *Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Pointers* or any other script implementing [`IMixedRealityPointer`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer).

## Pointer Events

To receive pointer events, implement one of the following interfaces in your script:

Handler | Events | Description
--- | --- | ---
[`IMixedRealityFocusChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusChangedHandler) | Before Focus Changed / Focus Changed | Raised on both the game object losing focus and the one gaining it every time a pointer changes focus.
[`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusHandler) | Focus Enter / Exit | Raised on the game object gaining focus when the first pointer enters it and on the one losing focus when the last pointer leaves it.
[`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler) | Pointer Down / Up / Clicked | Raised to report pointer input.
[`IMixedRealityTouchHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityTouchHandler) | Touch Started / Updated / Completed | Raised by touch-aware pointers like [**PokePointer**](xref:Microsoft.MixedReality.Toolkit.Input.PokePointer) to report touch activity.