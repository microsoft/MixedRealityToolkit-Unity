# How to add near interaction in MRTK
Near interactions come in the form of touches and grabs. Touches occur when a finger touches a collider or surface. Grabs occur when a user grabs a collider by either pinching their fingers together or grabbing. You can listen for touch and grab events by implementing the `IMixedRealityPointer` interface and looking at the type of pointer that triggered the `OnPointerDown` method.

Because near interactions are dispatched by pointers, you need to make sure to have these near interaction pointers in the scene. You also need to add `NearInteractionGrabbable` or `NearInteractionTouchable` to each collidable you want to be near grababable / touchable. 

> NOTE: For touch events it is also possible to configured your objects to raise touch evenets, listen to these by implementing the `IMixedRealityTouchHandler`

> NOTE: If you want to add touch interactions to Unity UI, you need to add a sepcial type of `NearInteractionTouchable`, read below for more details.

# How to add near grab interactions to your scene
1. Make sure you have a sphere pointer in you MRTK configuration. If you are using the Default MRTK configuration or the HL2 configuration, you have this already. This is under the mrtk profile / input system / pointers. Look for a prefab called "GrabPointer". The prefab needs to have a SpherePointer attached.

1. Pick a object that you want to raise grab events. Any ancestor of that object will be able to receive grabbable events. For example, you can add a cube to the scene.

1. Add a collider to that object. If you added a cube above.

1. Add a `NearInteractionGrabbable` to your object

1. On that object or one of its ancenstors, add the following component:

```
public class NearGrabTest : Monobehaviour, IMixedRealityPointerHandler
{
    public void OnPointerDown()
    {
        // If pointer is a Sphere pointer, then print "i've been grabbed!"
    }
}
```



## Why Near Interaction Grabbables and Touchables
Why we need grabbables and touchable components. Imagine we didn't have near interaction grabbable and touchables. When a user grasps, how would we decide what gameobjects to dispatch to? We only want to dispatch to the closest object, so we would look for the collider closest to the hand. But, what if that collider is in fact not interactive? We need a way to tell input system to ignore that collider. We could accomplish this just with layers, so have a "grabbable" layer, or a layer mask indicating grabbable objects. But, each object can only have one layer. Let's say we want to have something that's grabbable and touchable, but that not all touchable things should be grabbable. We would then need a grabbable and a touchable layer (to ensure that not all touchable things are grabbable). But then we would perhaps need a third layer 'grab and touchable', or to create two colliders -- one that is for the grabbable geometry, and one for the touchable geometry. Additionally, developers would need to think about and manage these layers themselves.  



# Examples

## Create a game object in code that listens for touch events

```

```



# Example code: Instantiate object and make it near grabbable