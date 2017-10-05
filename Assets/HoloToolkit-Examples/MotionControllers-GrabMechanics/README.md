# [MotionControllers-GrabMechanics]()

Useful scripts and examples for implementing various types of grab interaction with Windows Mixed Reality motion controller.

![](/External/ReadMeImages/MRTK_MotionController_GrabMechanics.jpg)

# Basic Architecture
 
The Grab family consists of these categories of scripts: 
- Grabber (used to pick things up) 
- Grabbable (the things you pick up) 
- ScalableObject (to scale object with two hands) 
- ThrowableObject (to make object throwable) 
- UsableObject (activate item after grab) 
- GrabbableColor (optional: attach to grabbable obj to see changes in grab state) 
 
ScalableObject, ThrowableObject and UsableObject rely on having a Grabbable script attached to the same Game Object; however, a Grabbable script is completely independent and indifferent about any other script attached to the same Game Object.  


# Types of Scripts in the Grab Family and their Main Options 
 
### BaseGrabbable - Abstract base class
- GrabSpot (optional: a grab attach location different than the transform position) 
- Grab Style ("exclusive" means this is only object the grabber can have attached) 

### GrabbableChild - Makes the grabbed object a child of the grabber
### GrabbableFixedJoint - Creates a breakable fixed joint to fasten to grabber
- Adjust runtime unity joint properties (i.e. breakforce, breaktorque, spring, dampening, etc) 
### GrabbableSpringJoint - Creates a breakable spring joint with a bouncy quality
- Adjust runtime unity joint properties  
### GrabbableTrackFollow - Allows the Grabber to “lead” the grabbed object with lag
- Lag distance 
### GrabbableSimple - Same as TrackFollow but no lag - grabbed object perfectly matches position and/or rotation of the grabbed object
- Follow position (set to true by default)  
- Follow rotation (set to true by default)   
### ScalableObject - Allows user to scale an object with two hands
- Scale Multiplier (amplify or dampen the amount of scaling)
### ThrowableObject 
- Throw Multiplier (amplify or dampen the throw velocity)  
- Throw Curves (Coming Soon! - Adjust curvature of the throw over time on x, y, z axis)  
### UsableObject 
- Handedness (which hand can grab/use this object. Unknown = either hand) 
- PressType (what type of button press activates the usable object) 


# Expectations for Extending or Adding to the Grab Family  
- To create a new script for this family, you usually want to extend from a Base script of a similar category. For example, for a new type of grabbing script, extend from the base script and add the special functionality in the new script. This way you can ensure that the new grab mechanism will work neatly with the grabbers and other types of scripts that depend on it 
- There are several basic events (or Actions) that are fired from existing scripts. Some examples include OnGrabStart, OnRelease, OnContact, and OnGrabStateChange) Subscribe to the action to create functionality that is cued at a particular moment. 
- Check to make sure every new item you add works with every other item in the grab family. For example, if you add a new type of throwing script, make sure it extends from BaseThrowable, and also works with each type of grabbing script, like Grab via child and GrabFixedJoint 

# Critical Sequence for Grab and UnGrab 

# What Happens When we Grab an Object?  
### Grabber 
- Controller raw data reports that the Grab Button is pressed (see subscription to InteractionSourcePressed or InteractionSourceUpdated) 
- GrabStart() subscribes to the above, and fires 
- TryGrabWith (on BaseGrabbable) gets called to check for success 
- If successful, add the proposed grabbable object to the grabber’s grabbedObjectList 
- If grabbedObjectList > 0 change this Grabber’s GrabState to single or multi 
  
### Grabbable Object 
- TryGrabWith() - Called from Grabber (see above) 
- StartGrab() (called from TryGrabWith()) - Children Override for specific behavior (i.e.setting rigidbody to kinematic so gravity does not interfere with grab) 
- AttachToGrabber - Children Override with specific behavior (For example used to create a joint if necessary) 
- OnGrabbed Action Fires - ScalableObject, RotatableObject UsableObject subscribe to this to their actions 


# What Happens When we Let Go of an Object?  
  
### Grabber 
- Controller raw data reports that the Grab Button is released (see subscription to InteractionSourcePressed or InteractionSourceUpdated) 
- GrabEnd() subscribes to the above. Clears the grabbedObjectList 
- if grabbed Object list is 0, GrabState of this Grabber changes to "Inactive" 
  
### Grabbable Object 
- In OnStayGrab() if activeGrabbers array/list is empty, then DetachFromGrabber() 
- DetachFromGrabber() ------ mostly overridden by children of BaseGrabbable. (For example used to destroy a joint in a FixedJoint grab) 
- In OnStayGrab() if GrabState of this object changes to Inactive (which it will be if the activeGrabbersList is empty), then call EndGrab() 
- EndGrab() fires OnReleased Action----- other objects in the grabbing family (mainly Throwable) subscribe to OnRelease to perform  
- Throw function is called if present (or anything else subscribed to OnRelease...) 


# Proposed Future Updates 
- Write joint system to move away from Unity-based joints. This should help with two hand grab objects that currently rely on spring joints.  
- Add Throw modifiers to allow devs to adjust the curvature of a thrown object over time 
- In this readme, add a chart of recommended combinations (e.g "GrabSimple works well with ThrowableObject") 
- ScalableObject adjust for Distance - if something is far away, scale it up more.
- ScalableObject scale by Distance/Velocity - scales based on either distance between the two grabbers or scale by velocity.
 
# Known Bugs 
- On the touch rotate example, the Rotatable Object seems to only respond to the left touchpad, despite checks for "handedness" associated with the grabbing controller 
- The forward of the controllerCubes (the user's hands) sometimes become offset from the true forward of the controllers 


---
##### [Go back up to the table of contents.](../../../README.md)
---
