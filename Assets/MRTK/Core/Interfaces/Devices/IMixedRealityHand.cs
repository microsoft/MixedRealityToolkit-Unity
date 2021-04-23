// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Hand definition, used to provide access to hand joints and other data.
    /// </summary>
    public interface IMixedRealityHand : IMixedRealityController
    {
        /// <summary>
        /// Get the current pose of a hand joint.
        /// </summary>
        /// <remarks>
        /// <para>Hand bones should be oriented along the Z-axis, with the Y-axis indicating the "up" direction,
        /// i.e. joints rotate primarily around the X-axis.</para>
        /// </remarks>
        bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose);
    }
}