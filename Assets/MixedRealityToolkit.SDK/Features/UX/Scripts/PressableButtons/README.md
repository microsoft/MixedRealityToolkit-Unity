This folder contains the scripts necessary for making Interactable buttons that support Speech, Far Interaction and Near Interaction (Physical hand pressing)

## High Level Description

PhysicalButtonMovement is similar to HandInteractionPress.
It uses External Object Targeting (EOT) to move an Interactable element along a press direction vector.
PhysicalButtonMovement needs a large BoxCollider on the Ignore Raycast layer (which it sets automatically)
It tracks the position of both index tip fingers and uses this to move the button as needed.

## States and Events

It then calls into a PhysicalPressEventRouter and calls events related to Touch, Press, Unpress (Default Click), and Untouch.

 * 	Touch: When a finger is contacting with the button.
 * 	Press: After the button has been pressed more than a threshhold amount.
 * 	Unpress/Click: When the button has been released while in Press state.
 * 	Untouch: When the finger is no longer touching the button.

The PhysicalPressEventRouter receives events and sets Interactable's InteractableState accordingly.
It also calls OnPointerClicked events, allowing for the PhysicalButtonMovement script to trigger Interactable.OnClicked UnityEvents (which speech/far interaction also execute)

If PhysicalPressEventRouter does not satisfy your use case, you can write your own and utilize it instead.

## Touch Button & Button Cage

Pressable buttons feature a normally invisible mesh called a 'Button Cage' or a 'Button Box'
This mesh has one direction as the button face (Negative Z).
This mesh has a button shader applied to it. The shader observes the position of several global positions (assigned in the GlobalShaderProximityAssigner)
TouchButton triggers the glow splash when the joint positions enter into a collider or meshrenderer's bounds.

## Prefabs

A Prefab is provide in MRTK.Examples/Demos/HandTracking/Prefabs/PressableButtons.

## PhysicalButtonMovement Customization

You can customize variables within the PressableButtons, such as the dimensions of the detection box collider.
- PhysicalButtonMovement's box collider can have a variety of dimensions. It usually wants to be several times deeper than the button itself (whose box collider dimensions matter for gaze/far interaction)
- PhysicalButtonMovement's box collider can be larger on the X and Y axes if you want to have a more generous pressable region. This can result in close neighbor buttons getting pressed simultaneously, so be sure to avoid that case.
- PhysicalButtonMovement's Z size controls how deep the user can press before the button is released (a continuous depth press does not trigger a default click). **Be sure to update ButtonSizeRelativeToCollider in PhysicalButtonMovement.cs**

## Dimensions

The sizing of .032 x .032 x .016 Unity units is the default dimensions of a button.
The default sizing of the EOT Collider is .032 x .032 x .128 Unity units. This gives a decent pressable depth even if a developer adjusts the maximum press depth beyond the default .05 Unity units.