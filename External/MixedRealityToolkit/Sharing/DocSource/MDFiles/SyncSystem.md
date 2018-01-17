Sync System                       {#syncsystem}
============
## Overview

We need to be able to allow a user to change values and settings in their app, and those changes need to be reflected in the apps of all the other users in the same session.  

We also need to be able to handle conflicting changes, as a remote user may be attempting to change variables at the same time as the local user, but the results of their changes must always resolve to the same state.  

Finally, all changes must be immediately visible to all other users, so remote users can see the local users changes in real time.  

To do this, we have developed a synchronization system based on the Operational Transform that allows user code to provide it with an arbitrary set of hierarchical data, then make modifications to that state data that is reflected in the state of remote users.  


## Key Concepts

* Operational Transform
	+ Operational Transform is a system of keeping a set of data (the 'state') consistent across multiple remote machines while users are simultaneously making changes to it.  It is most commonly used to co-edit documents online, so we had to make some enhancements to support arbitrary object-oriented data.
	+ In an Operational Transform system, all machines begin with the same initial state.  Changes to the shared state are performed through a pre-defined set of atomic operations.  When a user makes a change to their local state, they are creating a list of operations then applying them locally before sending them off to remote machines to be applied there.
	+ When a machine receives remote operations, it may have had several changes applied locally that the remote machine was not aware of when it sent its operations.  So the incoming operations must be transformed against the locally applied operations before they are applied.
	+ All transformations of operations A and B such that (A,B) => (A', B') must keep the invariant that starting from a common state, applying A then B' will yield the same state as B then A'
	+ Operations are applied one at a time, such that A and B are both starting from the same state
* Hierarchy of Elements
	+ Need to support arbitrary data
	+ Should be natural to use with Object-Oriented languages
	+ Atomic unit of data: Element
	+ Elements have different types: Integer element, float element, object element
	+ Maps to OO concepts:
		+ Object element represents a 'Class', contain other elements
		+ Other elements represent member variables of the class
		+ Object elements can be children of other object elements, forming a hierarchy
		+ Object elements are notified whenever their immediate children are modified remotely
			+ Its up to the user code to keep the class values and sync element values the same
* Xguids
	+ Each element in the sync system has a unique id (XGUID) that lets us quickly identify which element has changed
	+ Each guid is a hash of the text string of its location in the sync hierarchy.  For example root/Plans/Plan1/Activities/Activity1/Target651
	+ By using a hash of the path, it allows disconnected systems to create the same elements, then have them merge properly when they connect in a session
* Ownership
	+ Each element has an 'owner'
	+ Owner is either user in the session or the Invalid user, which is used to represent unowned elements
	+ When a system leaves a session
		+ The server removes elements owned by the user that left, forward changes to remaining systems
		+ The system that left deletes all elements except his/her own and global data
