# Scrolling object collection

The ScrollingObjectCollection is an Object Collection that natively scrolls 3D objects. It supports scrolling pressable buttons and Interactables as well as non-interactive objects. This collection supports both near and far input. In order to use ScrollingObjectCollection, objects must use the MRTK Standard Shader in order for the clipping effect to work properly.

## Getting started with scrolling object collection

For convenience, there are two ScrollingObjectCollection Prefabs available to use. One is configured to work with 32x92mm PressableButton prefabs, and the other is for any object in a 32x32x32mm container.

Simply drop these prefabs into a scene, add the desired objects, and press "UpdateCollection" to finalize the set up and layout of the Collection.

### Prerequisites

- All objects in collection must use the MRTK standard shader
- Every object in the collection must have a collider with a [`NearInteractionTouchable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionTouchable). All collision testing is currently done using these colliders; ScrollingObjectCollection does not yet support a static/nonmoving backing collider.
- All objects in collection need to be the same size currently, additionally you may get unexpected results if your objects aren't centered in a gameObject.
- For a seamless touchable surface, the 'cell size' in the scrolling collection should match the size of every object in the collection.

There are additional requirements when using buttons:

- PressableButton.ReleaseOnTouch must be disabled.
- PhysicalPressEventRouter.InteractableOnClick most be set to EventOnClickCompletion or EventOnPress.
- At edit time, ScrollingObjectCollection can automatically fix these components. But when dynamically instantiating Prefabs or components, make sure these properties are set properly.

## How it works

ScrollingObjectCollection subscribes itself as a global listener for Touch and Pointer events, filtering for events that correspond to the items in the list. Initially, the Collection doesn't do anything and lets events pass through to the child objects, this allows child objects to be poked and selected as expected. Once the ScrollingObjectCollection has deemed an interaction as a "drag", the collection begins marking all subsequent eventData as used and begins scrolling the list on the set axis.

When using touch, the list will continue to scroll, until the PokePointer has crossed the touch plane in front of the list.
