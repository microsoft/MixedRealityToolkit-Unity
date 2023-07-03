// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a HandsAggregatorSubsystem needs to be able to provide.
    /// Both the HandsAggregatorSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// </summary>
    /// <remarks>
    /// HandsAggregators aggregate skeletal hand joint data from all available sources.
    /// Implementations can aggregate hand joint data from multiple APIs, or from multiple
    /// HandsSubsystems, or from any other source they choose.
    /// Recommended use is for aggregating from all loaded HandsSubsystems.
    /// See <see cref="MRTKHandsAggregatorSubsystem"/> for the MRTK implementation.
    /// </remarks>
    public interface IHandsAggregatorSubsystem : ISubsystem
    {
        /// <summary>
        /// The play space local pose of the near interaction point.
        /// This will be the index finger tip of a fully tracked articulated hand,
        /// or a best estimate of the equivalent location when less information is
        /// available on the platform.
        /// </summary>
        bool TryGetNearInteractionPoint(XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// The play space local pose of the grab/pinch location. This is typically
        /// halfway between the thumb tip and the index tip.
        /// </summary>
        bool TryGetPinchingPoint(XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// How pinched the specific hand is.
        /// </summary>
        /// <param name="isReadyToPinch">Represents whether the hand is in a pinch ready pose or not, within the camera's FOV and palm facing away from the user.</param>
        /// <param name="isPinching">If hand is not pinching at all, this will be false.</param>
        /// <param name="pinchAmount">This value will be <c>0</c> for no pinch, <c>1</c> for fully pinched, or any floating point value if in between.</param>
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
