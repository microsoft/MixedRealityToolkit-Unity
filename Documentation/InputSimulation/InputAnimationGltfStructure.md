# Input Animation glTF Structure

When exporting input animation as a glTF file, the various animated input properties are converted into position and rotation animation channels for nodes in the scene graph. Abstract properties such as hand tracking state are also encoded in node animations.

## Camera Node

The [main camera from the CameraCache](xref:Microsoft.MixedReality.Toolkit.Utilities.CameraCache.Main) is exported as head movement. When importing from a glTF file the first camera node in the file is expected to encode the head movement. The name of the node is not used to identify the main camera.

## Hand State

Tracking and Pinching state of the left and right hand are encoded in node position animations. The X component is used to animate the boolean values, where 1.0 means the hand is tracking/pinching and 0.0 means it is not.

The following node names are used for hand state information:

* "Hand.Left.Tracking"
* "Hand.Right.Tracking"
* "Hand.Left.Pinching"
* "Hand.Right.Pinching"

The position of the nodes in the scene hierarchy does not matter. The exporter will group hand nodes under a common parent node for convenience, but any node with a matching name will be recognized by the importer.

## Joint Nodes

Joint positions and orientations are encoded using node transforms. The nodes for joints are identified using the following naming scheme:

"Hand._HANDEDNESS_.Joint._JOINT_"

where _HANDEDNESS_ can either "Left" or "Right", and _JOINT_ can be one of the following:

* "Wrist"
* "Palm"
* "ThumbMetacarpalJoint"
* "ThumbProximalJoint"
* "ThumbDistalJoint"
* "ThumbTip"
* "IndexMetacarpal"
* "IndexKnuckle"
* "IndexMiddleJoint"
* "IndexDistalJoint"
* "IndexTip"
* "MiddleMetacarpal"
* "MiddleKnuckle"
* "MiddleMiddleJoint"
* "MiddleDistalJoint"
* "MiddleTip"
* "RingMetacarpal"
* "RingKnuckle"
* "RingMiddleJoint"
* "RingDistalJoint"
* "RingTip"
* "PinkyMetacarpal"
* "PinkyKnuckle"
* "PinkyMiddleJoint"
* "PinkyDistalJoint"
* "PinkyTip"
* "None"

The position of the nodes in the scene hierarchy does not matter. The exporter will group hand nodes under a common parent node for convenience, but any node with a matching name will be recognized by the importer.
