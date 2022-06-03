// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a HandsSubsystem needs to be able to provide.
    /// Both the HandsSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// </summary>
    public interface IHandsSubsystem
    {
        /// <summary>
        /// Get a read-only reference to the entire hand.
        /// </summary>
        bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses);

        /// <summary>
        /// Query a single joint.
        /// </summary>
        bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose);
    }
}