# Scrolling Object Collection

The ScrollingObjectCollection is an extension component of the Object Collection system that natively scrolls 3d objects (including Interactables). This collection supports both touch and pointer input. In order to use ScrollingObjectCollection, your objects must use the MRTK Standard Shader in order for the clipping effect to work properly.

# How it works

ScrollingObjectCollection subscribes itself as a global listener for Touch and Pointer events, filtering for events that correspond to the items in the list. Initially, the Collection doesn't do anything and lets events pass through to the child objects, this allows child objects to be poked and selected as expected. Once the ScrollingObjectCollection has deemed an interaction a "drag", the collection begins marking all subsequent eventData as used and begins scrolling the list on the set axis.

When using touch, the list will continue to scroll, until the PokePointer has crossed the touch plane in front of the list.

* Note: Drag is defined by amount of time between touchStart/PointerDown (DragTimeThreshold) and the amount of movement on the desired axis (HandDeltaMagThreshold).

# When using PressableButtons

This control works out of the box with MRTK's PressableButtons. The only caveat is how they are set up.

- PressableButton.ReleaseOnTouch must be disabled.
- PhysicalPressEventRouter.InteractableOnClick most be set to EventOnClickCompletion.

At edit time, ScrollingObjectCollection can automatically fix these components. But when dynamically instantiating Prefabs or components, make sure these properties are set properly.