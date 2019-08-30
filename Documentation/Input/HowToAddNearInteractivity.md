# How to add near interaction in MRTK
Near interactions come in the form of touches and grabs. Touch and grab events are raised as pointer events by the [PokePointer](Pointers.md#pokepointer) and [SpherePointer](Pointers.md#spherepointer), respectively.

You can listen for touch and grab events by implementing [IMixedRealityPointerHandler](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler) and looking at the type of pointer that triggers your event. If the pointer is a PokePointer, the interaction is a touch. If the pointer is a SpherePointer, the interaction is a near grab.

Because near interactions are dispatched by pointers, you need to make sure MRTK is configured to create a [SpherePointer](Pointers.md#spherepointer) and a [PokePointer](Pointers.md#pokepointer) whenever articulated hands are present. You also need to add [NearInteractionGrabbable](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable) or [NearInteractionTouchable](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) to each collidable you want to be near grababable / touchable.

For touch interaction it is also possible to configured your objects to raise touch events, listen to these by implementing the [IMixedRealityTouchHandler](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityTouchHandler)

> If you want to add touch interactions to Unity UI, you need to add add [NearInteractionTouchableUnityUI](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchableUnityUI), read below for more details.


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

# How to add near **grab** interactions
1. Make sure you have a sphere pointer in you MRTK pointer profile. If you are using the default MRTK profile or the default HoloLens 2 profile, you will have this already. If you have a custom profile, look under Input / Pointers  / Pointer Options. Make sure that for the "articulated hand" controller type you have an entry that points to the GrabPointer prefab under MRTK.SDK/Features/UX/Prefabs, or create your own prefab and add a [SpherePointer](Pointers.md#spherepointer).

1. Pick an object that you want to make grabbable. Any ancestor of that object will be able to receive pointer events. For example, you can add a cube to the scene.

1. Make sure there is a collider to that object.

1. Add a [NearInteractionGrabbable](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable) to that collider.

1. On that object or one of its ancestors, add a component that implements [IMixedRealityPointerHandler](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler). You can for example add [ManipulationHandler](xref:Microsoft.MixedReality.Toolkit.UI.ManipulationHandler).

# How to add near **touch** interactions to collidables
> If you have UnityUI that you would like to make work with touch events, please read the next section

1. Make sure you have a [PokePointer](Pointers.md#pokepointer) in your MRTK pointer profile. If you are using the default MRTK profile or the default HoloLens 2 profile, you will have this already. If you have a custom profile, look under Input / Pointers  / Pointer Options. Make sure that for the "articulated hand" controller type you have an entry that points to the PokePointer prefab under MRTK.SDK/Features/UX/Prefabs, or create your own prefab and add a [PokePointer](Pointers.md#pokepointer).

1.  Pick a object that you want to raise pointer events. Any ancestor of that object will be able to receive pointer events. For example, you can add a cube to the scene.

1. Make sure there is a collider to that object.

1. Add a [NearInteractionTouchable](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable) to your object. 

1. Set "Events to Receive" to Pointer.

1. Click "Fix bounds" and "Fix center"

1. In the scene inspector, you'll see a white outline square and arrow. The arrow pointer to the "front" of the touchable. The collidable will only be touchable from that direction.

1. If you'd like to make your collider touchable from all directions, add a [NearInteractionTouchableVolume](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchableVolume) instead.

# How to add near **touch** interactions to UnityUI
1. Make sure you have a [PokePointer](Pointers.md#pokepointer) in your MRTK pointer profile. If you are using the default MRTK profile or the default HoloLens 2 profile, you will have this already. If you have a custom profile, look under Input / Pointers  / Pointer Options. Make sure that for the "articulated hand" controller type you have an entry that points to the PokePointer prefab under MRTK.SDK/Features/UX/Prefabs, or create your own prefab and add a [PokePointer](Pointers.md#pokepointer).

1. Add a UnitUI canvas to your scene.

1. Add a [NearInteractionTouchableUnityUI](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchableUnityUI) to your object. 

1. Set "Events to Receive" to Pointer.

## Why Near Interaction Grabbables and Touchables?
Why we need grabbables and touchable components. Imagine we didn't have near interaction grabbable and touchables. When a user grasps, how would we decide what gameobjects to dispatch to? We only want to dispatch to the closest object, so we would look for the collider closest to the hand. But, what if that collider is in fact not interactive? We need a way to tell input system to ignore that collider. We could accomplish this just with layers, so have a "grabbable" layer, or a layer mask indicating grabbable objects. But, each object can only have one layer. Let's say we want to have something that's grabbable and touchable, but that not all touchable things should be grabbable. We would then need a grabbable and a touchable layer (to ensure that not all touchable things are grabbable). But then we would perhaps need a third layer 'grab and touchable', or to create two colliders -- one that is for the grabbable geometry, and one for the touchable geometry. Additionally, developers would need to think about and manage these layers themselves.  


# Examples

## Grab events: Create a simple draggable cube from code

```csharp
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// On startup, creates a cube that can be dragged and resized using near interaction and far interaction
/// </summary>
public class SimpleGrabbableCube : Monobehaviour
{
    private void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<NearInteractionGrabbable>();
        cube.AddComponent<ManipulationHandler>();
        cube.transform.localScale = Vector3.one * 0.1f;
        cube.transform.position = Vector3.forward;
    }
}
```


## Touch events: Create a cube, make it touchable, and change color on touch
```csharp
public class MakeTouchableCube : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var touchable = cube.AddComponent<NearInteractionTouchableVolume>();
        touchable.EventsToReceive = TouchableEventType.Pointer;
        var pointerHandler = cube.AddComponent<PointerHandler>();
        var magentaMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.SDK/StandardAssets/Materials/MRTK_Standard_Magenta.mat", typeof(Material));
        var cubeMaterial = new Material(magentaMaterial);
        cube.GetComponent<Renderer>().material = cubeMaterial;
        pointerHandler.OnPointerDown.AddListener((e) => cubeMaterial.color = Color.green);
        pointerHandler.OnPointerUp.AddListener((e) => cubeMaterial.color = Color.magenta);
    }
}
```