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
        /// Calculates position and orientation from the left motion-tracked controller.
        /// </summary>
        MotionControllerLeft,
        /// <summary>
        /// Calculates position and orientation from the right motion-tracked controller.
        /// </summary>
        MotionControllerRight,
        /// <summary>
        /// Calculates position and orientation from a tracked hand joint on the left hand.
        /// </summary>
        HandJointLeft,
        /// <summary>
        /// Calculates position and orientation from a tracked hand joint on the right hand.
        /// </summary>
        HandJointRight
    }
}