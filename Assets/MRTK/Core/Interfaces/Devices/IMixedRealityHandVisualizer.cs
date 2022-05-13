// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Hand visualization definition, used to provide access to hand joint objects.
    /// </summary>
    public interface IMixedRealityHandVisualizer : IMixedRealityControllerVisualizer
    {
        /// <summary>
        /// Get a game object following the hand joint.
        /// </summary>
        [Obsolete("Use HandJointUtils.TryGetJointPose instead of this")]
        bool TryGetJointTransform(TrackedHandJoint joint, out Transform jointTransform);
    }
}