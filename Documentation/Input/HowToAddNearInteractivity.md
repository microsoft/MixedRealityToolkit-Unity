# How to add near interaction in MRTK
Near interactions come in the form of touches and grabs. Touch and grab events are raised as pointer events by the [PokePointer](Pointers.md#pokepointer) and [SpherePointer](Pointers.md#spherepointer), respectively.

Listen for touch and grab events by implementing [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler) and looking at the type of pointer that triggers your event. If the pointer is a PokePointer, the interaction is a touch. If the pointer is a SpherePointer, the interaction is a near grab. For touch interaction it is also possible to configure objects to raise touch events by implementing the [`IMixedRealityTouchHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityTouchHandler)

Because near interactions are dispatched by pointers, you need to make sure MRTK is configured to create a [SpherePointer](Pointers.md#spherepointer) and a [PokePointer](Pointers.md#pokepointer) whenever articulated hands are present. You also need to add [`NearInteractionGrabbable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable) or [`NearInteractionTouchable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) to each collidable you want to be near grababable / touchable. If you want to add touch interactions to Unity UI, you need to add add [`NearInteractionTouchableUnityUI`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchableUnityUI), read below for more details.

#### Example
Below is a script that will print if an event is a touch or grab.

```csharp
public class PrintPointerEvents : MonoBehaviour, IMixedRealityPointerHandler
{
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (eventData.Pointer is SpherePointer)
        {
            Debug.Log($"Grab start from {eventData.Pointer.PointerName}");
        }
        if (eventData.Pointer is PokePointer)
        {
            Debug.Log($"Touch start from {eventData.Pointer.PointerName}");
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) {}
    public void OnPointerDragged(MixedRealityPointerEventData eventData) {}
    public void OnPointerUp(MixedRealityPointerEventData eventData) {}
}
```

## How to add near grab interactions
1. Make sure there is a [SpherePointer](Pointers.md#spherepointer) in the MRTK pointer profile. The default MRTK profile and the default HoloLens 2 profile already contains a SpherePointer. For custom profiles, look under _Input > Pointers > Pointer Options_. Make sure that for the "articulated hand" controller type there is an entry that points to the GrabPointer prefab under MRTK.SDK/Features/UX/Prefabs, or if using a custom prefab add a [SpherePointer](Pointers.md#spherepointer).

1. Pick an object that to make grabbable. Any ancestor of that object will be able to receive pointer events. For example, you can add a cube to the scene.

1. Make sure there is a collider to that object.

1. Make sure the layer your object is on is a grabbable layer. By default, all layers except _Spatial Awareness_ and _Ignore Raycast_ are grabbable. See which layers are grabbable by inspecting the Grab Layer Masks in your GrabPointer prefab.

1. Add a [`NearInteractionGrabbable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable) to that collider.

1. On that object or one of its ancestors, add a component that implements [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler). For example, add [`ManipulationHandler`](xref:Microsoft.MixedReality.Toolkit.UI.ManipulationHandler).


## How to add near touch interactions to collidables
> If you have UnityUI that you would like to make work with touch events, please read the next section

1. Make sure there is a [PokePointer](Pointers.md#pokepointer) in the MRTK pointer profile.  The default MRTK profile and the default HoloLens 2 profile already contains a SpherePointer. For custom profiles, look under _Input > Pointers > Pointer Options_. Make sure that for the "articulated hand" controller type you there is an entry that points to the PokePointer prefab under MRTK.SDK/Features/UX/Prefabs, or if using a custom prefab, add a [PokePointer](Pointers.md#pokepointer).

1.  Pick a object that should raise pointer events. Any ancestor of that object will be able to receive pointer events. For example, add a cube to the scene.

1. Make sure there is a collider to that object.

1. Make sure the layer your object is on is a touchable layer. By default, all layers except _Ignore Raycast_ are touchable. See which layers are grabbable by inspecting the Touch Layer Masks in the PokePointer prefab.

1. Add a [`NearInteractionTouchable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) to the object. 

1. Set "Events to Receive" to Pointer.

1. Click "Fix bounds" and "Fix center"

1. In the scene inspector, notice a white outline square and arrow. The arrow points to the "front" of the touchable. The collidable will only be touchable from that direction.

1. To make a collider touchable from all directions, add a [`NearInteractionTouchableVolume`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchableVolume) instead.

## How to add near touch interactions to UnityUI
1. Make sure you have a [PokePointer](Pointers.md#pokepointer) in your MRTK pointer profile. If you are using the default MRTK profile or the default HoloLens 2 profile, you will have this already. If you have a custom profile, open your custom profile and go to look under Input / Pointers  / Pointer Options. Make sure that for the "articulated hand" controller type you have an entry that points to the PokePointer prefab under MRTK.SDK/Features/UX/Prefabs, or create your own prefab and add a [PokePointer](Pointers.md#pokepointer).

1. Add a UnityUI canvas to your scene.

1. Add a [`NearInteractionTouchableUnityUI`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchableUnityUI) to your object.  

1. Set "Events to Receive" to Pointer.


## Examples

### Touch events
This example creates a cube, makes it touchable, and change color on touch.
```csharp
public static void MakeChangeColorOnTouch(GameObject target)
{
    // Add and configure the touchable
    var touchable = target.AddComponent<NearInteractionTouchableVolume>();
    touchable.EventsToReceive = TouchableEventType.Pointer;

    var material = target.GetComponent<Renderer>().material;
    // Change color on pointer down and up
    var pointerHandler = target.AddComponent<PointerHandler>();
    pointerHandler.OnPointerDown.AddListener((e) => material.color = Color.green);
    pointerHandler.OnPointerUp.AddListener((e) => material.color = Color.magenta);
}
```

### Grab events
The below example shows how to make a gameobject draggable. Assumes that the game object has a collider on it.

```csharp
public static void MakeNearDraggable(GameObject target)
{
    // Instantiate and add grabbable
    target.AddComponent<NearInteractionGrabbable>();

    // Add ability to drag by reparenting to pointer object on pointer down
    var pointerHandler = target.AddComponent<PointerHandler>();
    pointerHandler.OnPointerDown.AddListener((e) =>
    {
        if (e.Pointer is SpherePointer)
        {
            target.transform.parent = ((SpherePointer)(e.Pointer)).transform;
        }
    });
    pointerHandler.OnPointerUp.AddListener((e) =>
    {
        if (e.Pointer is SpherePointer)
        {
            target.transform.parent = null;
        }
    });
}
```
