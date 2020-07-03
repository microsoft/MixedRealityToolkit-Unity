// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Mixed Reality Toolkit device definition, used to instantiate and manage a specific device / SDK
    /// </summary>
    public interface IMixedRealityHandJointService : IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Get a game object following the hand joint.
        /// </summary>
        Transform RequestJointTransform(TrackedHandJoint joint, Handedness handedness);

        bool IsHandTracked(Handedness handedness);
    }
}