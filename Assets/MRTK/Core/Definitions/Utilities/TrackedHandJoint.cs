// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The supported tracked hand joints.
    /// </summary>
    /// <remarks>See https://en.wikipedia.org/wiki/Interphalangeal_joints_of_the_hand#/media/File:Scheme_human_hand_bones-en.svg for joint name definitions.</remarks>
    public enum TrackedHandJoint
    {
        None = 0,
        /// <summary>
        /// The wrist.
        /// </summary>
        Wrist,
        /// <summary>
        /// The palm.
        /// </summary>
        Palm,
        /// <summary>
        /// The lowest joint in the thumb (down in your palm).
        /// </summary>
        ThumbMetacarpalJoint,
        /// <summary>
        /// The thumb's second (middle-ish) joint.
        /// </summary>
        ThumbProximalJoint,
        /// <summary>
        /// The thumb's first (furthest) joint.
        /// </summary>
        ThumbDistalJoint,
        /// <summary>
        /// The tip of the thumb.
        /// </summary>
        ThumbTip,
        /// <summary>
        /// The lowest joint of the index finger.
        /// </summary>
        IndexMetacarpal,
        /// <summary>
        /// The knuckle joint of the index finger.
        /// </summary>
        IndexKnuckle,
        /// <summary>
        /// The middle joint of the index finger.
        /// </summary>
        IndexMiddleJoint,
        /// <summary>
        /// The joint nearest the tip of the index finger.
        /// </summary>
        IndexDistalJoint,
        /// <summary>
        /// The tip of the index finger.
        /// </summary>
        IndexTip,
        /// <summary>
        /// The lowest joint of the middle finger.
        /// </summary>
        MiddleMetacarpal,
        /// <summary>
        /// The knuckle joint of the middle finger. 
        /// </summary>
        MiddleKnuckle,
        /// <summary>
        /// The middle joint of the middle finger.
        /// </summary>
        MiddleMiddleJoint,
        /// <summary>
        /// The joint nearest the tip of the finger.
        /// </summary>
        MiddleDistalJoint,
        /// <summary>
        /// The tip of the middle finger.
        /// </summary>
        MiddleTip,
        /// <summary>
        /// The lowest joint of the ring finger.
        /// </summary>
        RingMetacarpal,
        /// <summary>
        /// The knuckle of the ring finger.
        /// </summary>
        RingKnuckle,
        /// <summary>
        /// The middle joint of the ring finger.
        /// </summary>
        RingMiddleJoint,
        /// <summary>
        /// The joint nearest the tip of the ring finger.
        /// </summary>
        RingDistalJoint,
        /// <summary>
        /// The tip of the ring finger.
        /// </summary>
        RingTip,
        /// <summary>
        /// The lowest joint of the pinky finger.
        /// </summary>
        PinkyMetacarpal,
        /// <summary>
        /// The knuckle joint of the pinky finger.
        /// </summary>
        PinkyKnuckle,
        /// <summary>
        /// The middle joint of the pinky finger.
        /// </summary>
        PinkyMiddleJoint,
        /// <summary>
        /// The joint nearest the tip of the pink finger.
        /// </summary>
        PinkyDistalJoint,
        /// <summary>
        /// The tip of the pinky.
        /// </summary>
        PinkyTip
    }
}