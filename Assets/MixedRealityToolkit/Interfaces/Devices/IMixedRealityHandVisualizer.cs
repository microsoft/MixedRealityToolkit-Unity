// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using System;

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