// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The supported tracked hand joints.
    /// </summary>
    /// <remarks>See https://en.wikipedia.org/wiki/Interphalangeal_joints_of_the_hand#/media/File:Scheme_human_hand_bones-en.svg for joint name definitions.</remarks>
    public enum TrackedHandJoint
    {
        /// <summary>
        /// The palm.
        /// </summary>
        Palm,

        /// <summary>
        /// The wrist.
        /// </summary>
        Wrist,

        /// <summary>
        /// The lowest joint in the thumb (down in your palm).
        /// </summary>
        ThumbMetacarpal,

        /// <summary>
        /// The thumb's second (middle-ish) joint.
        /// </summary>
        ThumbProximal,

        /// <summary>
        /// The thumb's first (furthest) joint.
        /// </summary>
        ThumbDistal,

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
        IndexProximal,

        /// <summary>
        /// The middle joint of the index finger.
        /// </summary>
        IndexIntermediate,

        /// <summary>
        /// The joint nearest the tip of the index finger.
        /// </summary>
        IndexDistal,

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
        MiddleProximal,

        /// <summary>
        /// The middle joint of the middle finger.
        /// </summary>
        MiddleIntermediate,

        /// <summary>
        /// The joint nearest the tip of the finger.
        /// </summary>
        MiddleDistal,

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
        RingProximal,

        /// <summary>
        /// The middle joint of the ring finger.
        /// </summary>
        RingIntermediate,

        /// <summary>
        /// The joint nearest the tip of the ring finger.
        /// </summary>
        RingDistal,

        /// <summary>
        /// The tip of the ring finger.
        /// </summary>
        RingTip,

        /// <summary>
        /// The lowest joint of the little finger.
        /// </summary>
        LittleMetacarpal,

        /// <summary>
        /// The knuckle joint of the little finger.
        /// </summary>
        LittleProximal,

        /// <summary>
        /// The middle joint of the little finger.
        /// </summary>
        LittleIntermediate,

        /// <summary>
        /// The joint nearest the tip of the little finger.
        /// </summary>
        LittleDistal,

        /// <summary>
        /// The tip of the little finger.
        /// </summary>
        LittleTip,

        /// <summary>
        /// Number of joints total (not counting None).
        /// </summary>
        TotalJoints = LittleTip + 1
    }
}
