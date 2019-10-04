# Scrolling Object Collection

The ScrollingObjectCollection an Object Collection that natively scrolls 3d objects. It supports scrolling pressable buttons and interactables as well as non-interactive objects. This collection supports both near and far input. In order to use ScrollingObjectCollection, your objects must use the MRTK Standard Shader in order for the clipping effect to work properly.

## Prerequisites
- All objects in collection must use the MRTK standard shader
- Every object in the collection must have a collider with a <see cref="NearInteractionTouchable"/>. All collision testing is currently done using these collidables, ScrollableCollection does not yet support a static/nonmoving backing collider.
- All objects in collection need to be the same size currently
- The 'cell size' in the scrolling collection should match the size of every object in the collection.
 
There are additional requirements when using buttons:
- PressableButton.ReleaseOnTouch must be disabled.
- PhysicalPressEventRouter.InteractableOnClick most be set to EventOnClickCompletion or EventOnPress.
- 
- At edit time, ScrollingObjectCollection can automatically fix these components. But when dynamically instantiating Prefabs or components, make sure these properties are set properly.

# How it works

ScrollingObjectCollection subscribes itself as a global listener for Touch and Pointer events, filtering for events that correspond to the items in the list. Initially, the Collection doesn't do anything and lets events pass through to the child objects, this allows child objects to be poked and selected as expected. Once the ScrollingObjectCollection has deemed an interaction a "drag", the collection begins marking all subsequent eventData as used and begins scrolling the list on the set axis.

When using touch, the list will continue to scroll, until the PokePointer has crossed the touch plane in front of the list.

