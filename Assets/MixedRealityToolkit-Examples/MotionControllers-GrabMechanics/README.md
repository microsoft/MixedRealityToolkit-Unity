# [MotionControllers-GrabMechanics]()

Useful scripts and examples for implementing various types of grab interaction with Windows Mixed Reality motion controller.

![](/External/ReadMeImages/MRTK_MotionController_GrabMechanics.jpg)

# Demo Controls
Use the motion controller joysticks for teleportation-based movement. This demo does not implement gaze or pointer based object grabbing. To activate grab mechanics, overlap your mixed reality controllers with a block. This should cause the block to change color. Press the grab button (middle finger) to interact with the block. 

# Basic Architecture
 
The Grab family consists of these categories of scripts: 
- Grabber (used to pick things up) 
- Grabbable (the things you pick up) 
- ScalableObject (to scale object with two hands) 
- ThrowableObject (to make object throwable) 
- UsableObject (activate item after grab) 
- GrabbableColor (optional: attach to grabbable obj to see changes in grab state) 
 
ScalableObject, ThrowableObject and UsableObject rely on having a Grabbable script attached to the same Game Object, but a Grabbable script is completely independent and indifferent about any other script attached to the same Game Object.  


# Class and Inheritance Overview 
 
![](/External/ReadMeImages/MRTK_MotionController_GrabMechanicsDiagram.jpg)


# Expectations for Extending or Adding to the Grab Family  
- To create a new script for this family, you will typically want to extend from a Base[X] script of a similar category. For example, for a new type of grabbing script, extend from the BaseGrabbable class and add the special functionality in the new script. This way you can ensure that the new grab mechanism will work neatly with the grabbers and other classes that depend on it 
- There are several basic events (or Actions) that are fired from existing scripts. Some examples include OnGrabStart, OnRelease, OnContact, and OnGrabStateChange) Subscribe to the action to create functionality that is cued at a particular moment. 
- Check to make sure every new item you add works with every other item in the grab family. For example, if you add a new type of throwing script, make sure it extends from BaseThrowable, and also works with each type of grabbing script, such as GrabbableChild and GrabbableFixedJoint 

# Critical Sequence for Grab and UnGrab 

![](/External/ReadMeImages/MRTK_MotionController_GrabMechanicsDiagram2.jpg)


# How do I Make My Own?
- Add a Grabber.cs script to an object that does the grabbing. (This is often a controller, but may sometimes be a cursor or pointer object.) Make sure to add a RigidBody and a collider to this same object. Set the trigger to true.
- In the hierarchy, go into the [MRTK]MixedRealityCameraParent dropdown, and find a child called Controllers. This gameObject has a script attached called ControllerVisualizer.cs. Make sure your controller prefab is dragged into the public variable for Left Controller Override and Right Controller Override.   
- Add a Grabbable_[X].cs script to the object that will be picked up. Make sure to also add a Rigidbody and Collider to this object. 
- On the grabbable object, you can also add ScalableObject.cs, ThrowableObject.cs and UsableObject.cs (as needed). See chart for recommended combinations.


# Proposed Future Updates 
- Write joint system to move away from Unity-based joints. This should help with two hand grab objects that currently rely on spring joints.  
- Add Throw modifiers to allow devs to adjust the curvature of a thrown object over time 
- In this readme, add a chart of recommended combinations (e.g "GrabSimple works well with ThrowableObject") 
- ScalableObject will soon adjust for Distance (for example, if something is far away, scale it up more).
- ScalableObject will soon scale by Distance OR Velocity 
 
# Known Bugs 
- On the touch rotate example, the Rotatable Object seems to only respond to the left touchpad, despite checks for "handedness" associated with the grabbing controller 
- The forward of the controllerCubes (the user's hands) sometimes become offset from the true forward of the controllers 


---
##### [Go back up to the table of contents.](../../../README.md)
---
