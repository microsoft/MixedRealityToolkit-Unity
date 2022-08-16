// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a HandsAggregatorSubsystem needs to be able to provide.
    /// Both the HandsAggregatorSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// <remarks>
    /// HandsAggregators aggregate skeletal hand joint data from all available sources.
    /// Implementations can aggregate hand joint data from multiple APIs, or from multiple
    /// HandsSubsystems, or from any other source they choose.
    /// Recommended use is for aggregating from all loaded HandsSubsystems.
    /// See <see cref="MRTKHandsAggregatorSubsystem"> for the MRTK implementation.
    /// </remarks>
    /// </summary>
    public interface IHandsAggregatorSubsystem
    {
        /// <summary>
        /// The playspace-local pose of the near interaction point.
        /// This will be the index finger tip of a fully tracked articulated hand,
        /// or a best estimate of the equivalent location when less information is
        /// available on the platform.
        /// </summary>
        bool TryGetNearInteractionPoint(XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// The playspace-local pose of the "root" of the hand. This is used for
        /// hand rays, or any other case where a reasonable palm root position is
        /// needed. This will return the controller position if no hand data is
        /// available on the platform.
        /// </summary>
        [Obsolete("Use TryGetJoint(TrackedHandJoint.Palm...) instead.")]
        bool TryGetHandCenter(XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// The playspace-local pose of the grab/pinch location. This is typically
        /// halfway between the thumb tip and the index tip.
        /// </summary>
        bool TryGetPinchingPoint(XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// How pinched the specific hand is.
        /// </summary>
        /// <param name="isReadyToPinch"> returns whether the hand is in a pinch ready pose or not (within the camera's FOV and palm facing away from the user) </param>
        /// <param name="isPinching"> If hand is not pinching at all, isPinching will be false. </param>
        /// <param name="pinchAmount"> 0 for no pinch, 1 for fully pinched, or any floating point value in between. </param>
        /// </returns>
        bool TryGetPinchProgress(XRNode hand, out bool isReadyToPinch, out bool isPinching, out float pinchAmount);

        /// <summary>
        /// Whether the palm of the given handedness is facing away from the user
        /// </summary>
        /// <param name="palmFacingAway">returns true if the palm is facing away from the other, false otherwise. </param>
        bool TryGetPalmFacingAway(XRNode hand, out bool palmFacingAway);

        /// <summary>
        /// Queries a specific hand joint, specified by <paramref name="joint"/>.
        /// </summary>
        /// <param name="joint">Identifier of the requested joint.</param>
        bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// Get a read-only reference to the entire hand.
        /// Joint poses are returned in an order consistent where each index matches up with the <see cref="Microsoft.MixedReality.Toolkit.TrackedHandJoint"/> enum
        /// </summary>
        bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses);
    }
}
