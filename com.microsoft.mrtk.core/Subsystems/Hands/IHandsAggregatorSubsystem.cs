// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a subsystem needs to be able to provide to aggregator multiple 
    /// <see cref="IHandsSubsystem"/> subsystems.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both the <see cref="IHandsAggregatorSubsystem"/> implementations and the associated provider
    /// must implement this interface, preferably with a direct one to one mapping
    /// between the provider surface and the subsystem surface.
    /// </para>
    /// <para>
    /// Implementations of <see cref="IHandsAggregatorSubsystem"/> aggregate skeletal hand joint
    ///  data from all available sources. Implementations can aggregate hand joint data from multiple 
    /// APIs, or from multiple subsystems, or from any other source they choose.
    /// Recommended use is for aggregating from all loaded HandsSubsystems.
    /// See the <c>MRTKHandsAggregatorSubsystem</c> class for the MRTK implementation.
    /// </para>
    /// </remarks>
    public interface IHandsAggregatorSubsystem : ISubsystem
    {
        /// <summary>
        /// The play space local pose of the near interaction point.
        /// </summary>
        /// <remarks>
        /// This will be the index finger tip of a fully tracked articulated hand,
        /// or a best estimate of the equivalent location when less information is
        /// available on the platform.
        /// </remarks>
        /// <returns><see langword="true"/> if the joint pose was found, and <see langword="false"/> otherwise.</returns>
        bool TryGetNearInteractionPoint(XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// The play space local pose of the grab/pinch location. 
        /// </summary>
        /// <remarks>
        /// This is typically halfway between the thumb tip and the index tip.
        /// </remarks>
        /// <returns><see langword="true"/> if the joint pose was found, and <see langword="false"/> otherwise.</returns>
        bool TryGetPinchingPoint(XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// How pinched the specific hand is.
        /// </summary>
        /// <param name="hand">The hand node being queried.</param>
        /// <param name="isReadyToPinch">Represents whether the hand is in a pinch ready pose or not, within the camera's FOV and palm facing away from the user.</param>
        /// <param name="isPinching">If hand is not pinching at all, this will be false.</param>
        /// <param name="pinchAmount">This value will be <c>0</c> for no pinch, <c>1</c> for fully pinched, or any floating point value if in between.</param>
        bool TryGetPinchProgress(XRNode hand, out bool isReadyToPinch, out bool isPinching, out float pinchAmount);

        /// <summary>
        /// Whether the palm of the given handedness is facing away from the user
        /// </summary>
        /// <param name="hand">The hand node being queried.</param>
        /// <param name="palmFacingAway">Will be <see langword="true"/> if the palm is facing away from the other, <see langword="false"/> otherwise.</param>
        /// <returns><see langword="true"/> if the hand was found, and <see langword="false"/> otherwise.</returns>
        bool TryGetPalmFacingAway(XRNode hand, out bool palmFacingAway);

        /// <summary>
        /// Queries a specific hand joint, specified by <paramref name="joint"/>.
        /// </summary>
        /// <param name="joint">Identifier of the requested joint.</param>
        /// <param name="hand">The hand node being queried.</param>
        /// <param name="jointPose">The resulting joint pose that was found.</param>
        /// <returns><see langword="true"/> if the joint pose was found, and <see langword="false"/> otherwise.</returns>
        bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose);

        /// <summary>
        /// Get a read-only reference to the entire hand.
        /// </summary>
        /// <remarks>
        /// Joint poses are returned in an order consistent where each index matches up with the <see cref="Microsoft.MixedReality.Toolkit.TrackedHandJoint"/> enumeration.
        /// </remarks>
        /// <param name="hand">The hand node being queried.</param>
        /// <param name="jointPoses">The resulting joint poses that were found.</param>
        /// <returns><see langword="true"/> if the hand was found, and <see langword="false"/> otherwise.</returns>
        bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses);
    }
}
