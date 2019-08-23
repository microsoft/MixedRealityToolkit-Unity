# How to add near interaction in MRTK
Near interactions come in the form of touches and grabs. Touches occur when a finger touches a collider or surface. Grabs occur when a user grabs a collider by either pinching their fingers together or grabbing. You can listen for touch and grab events by implementing the `IMixedRealityPointer` interface and looking at the type of pointer that triggered the `OnPointerDown` method.

Because near interactions are dispatched by pointers, you need to make sure to have these near interaction pointers in the scene. You also need to add `NearInteractionGrabbable` or `NearInteractionTouchable` to each collidable you want to be near grababable / touchable. 

> NOTE: For touch events it is also possible to configured your objects to raise touch evenets, listen to these by implementing the `IMixedRealityTouchHandler`

> NOTE: If you want to add touch interactions to Unity UI, you need to add a sepcial type of `NearInteractionTouchable`, read below for more details.

# How to add near grab interactions to your scene
1. Make sure you have a sphere pointer in you MRTK configuration. If you are using the Default MRTK configuration or the HL2 configuration, you have this already. This is under the mrtk profile / input system / pointers. Look for a prefab called "GrabPointer". The prefab needs to have a `SpherePointer` component.

1. Pick a object that you want to raise pointer events. Any ancestor of that object will be able to receive pointer events. For example, you can add a cube to the scene.

1. Add a collider to that object. If you added a cube above.

1. Add a `NearInteractionGrabbable` to your object

1. On that object or one of its ancestors, add the following component:

```
public class NearGrabTest : Monobehaviour, IMixedRealityPointerHandler
{
    public void OnPointerDown()
    {
        // If pointer is a Sphere pointer, then print "i've been grabbed!"
    }
}
```

# How to add near touch interactions to your scene
> Note: If you have UnityUI that you would like to make work with touch events, please read the next section

1. Make sure you have a poke pointer in your MRTK configuration. If you are using the Default MRTK configuration or the HL2 configuration, you will have this already. This is under the mrtk profile / input system / pointers. Look for a prefab called "PokePointer". The prefab needs to have a `PokePointer` attached.

1.  Pick a object that you want to raise pointer events. Any ancestor of that object will be able to receive pointer events. For example, you can add a cube to the scene.

1. Add a collider to that object. If you added a cube above.

1. Add a `NearInteractionTouchable` to your object (link to doc here). You can configure your `NearInteractionTouchable` to raise either pointer events or touch events.

#### TODO: UPDATE WITH POINTER* events
1. If you configured your touchable to dispatch **pointer events**, then on that object or one of its ancestors, add the following component:

```
public class NearGrabTest : Monobehaviour, IMixedRealityPointerHandler
{
    public void OnPointerDown()
    {
        // If pointer is a Poke pointer, then print "i've been touched!"
    }
}
```

#### TODO: UPDATE WITH TOUCH* events
1. If you configured your touchable to dispatch **touch events**, then on that object or one of its ancestors, add the following component:

```
public class NearGrabTest : Monobehaviour, IMixedRealityTouchHandler
{
    public void OnTouchDown()
    {
        // If pointer is a Poke pointer, then print "i've been touched!"
    }
}
```

# How to add near touch interactions to UnityUI
MRTK supports using touch interactions and far interactions with Unity UI. To do this, do the following:

1. Make sure you have a poke pointer in your MRTK configuration. If you are using the Default MRTK configuration or the HL2 configuration, you will have this already. This is under the mrtk profile / input system / pointers. Look for a prefab called "PokePointer". The prefab needs to have a `PokePointer` attached.

1. Add a Unity UI canvas

1. Blah blah about Unity UI (I think that's somewhere)

1. Add `NearInteractionTouchableUnityUI`. Important: Make sure you add Touchable**UnityUI**, otherwise input will not dispatch properly.

## Why Near Interaction Grabbables and Touchables
Why we need grabbables and touchable components. Imagine we didn't have near interaction grabbable and touchables. When a user grasps, how would we decide what gameobjects to dispatch to? We only want to dispatch to the closest object, so we would look for the collider closest to the hand. But, what if that collider is in fact not interactive? We need a way to tell input system to ignore that collider. We could accomplish this just with layers, so have a "grabbable" layer, or a layer mask indicating grabbable objects. But, each object can only have one layer. Let's say we want to have something that's grabbable and touchable, but that not all touchable things should be grabbable. We would then need a grabbable and a touchable layer (to ensure that not all touchable things are grabbable). But then we would perhaps need a third layer 'grab and touchable', or to create two colliders -- one that is for the grabbable geometry, and one for the touchable geometry. Additionally, developers would need to think about and manage these layers themselves.  


# Examples

## Grab events: Create a simple draggable cube from code

```
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



## Touch events: Create a paintable surface

```

```

