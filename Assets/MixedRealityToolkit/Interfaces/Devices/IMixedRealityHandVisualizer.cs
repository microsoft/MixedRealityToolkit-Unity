// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices
{
    /// <summary>
    /// Hand visualization definition, used to provide access to hand joint objects.
    /// </summary>
    public interface IMixedRealityHandVisualizer : IMixedRealityControllerVisualizer
    {
        /// <summary>
        /// Get a game object following the hand joint.
        /// </summary>
        bool TryGetJointTransform(TrackedHandJoint joint, out Transform jointTransform);
    }
}