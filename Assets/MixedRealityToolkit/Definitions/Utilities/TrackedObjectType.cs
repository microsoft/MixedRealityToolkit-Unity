// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public enum TrackedObjectType
    {
        /// <summary>
        /// Calculates position and orientation from the main camera.
        /// </summary>
        Head = 0,
        /// <summary>
        /// Calculates position and orientation from the motion-tracked controller.
        /// </summary>
        MotionController,
        /// <summary>
        /// Calculates position and orientation from a tracked hand joint
        /// </summary>
        HandJoint,
        /// <summary>
        /// Calculates position and orientation from a tracked hand joint
        /// </summary>
        CustomOverride,
    }
}