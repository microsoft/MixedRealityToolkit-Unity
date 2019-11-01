// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public enum TrackedObjectType
    {
        /// <summary>
        /// Calculates position and orientation from the main camera.
        /// </summary>
        Head = 0,

        /// <summary>
        /// (Obsolete) Calculates position and orientation from the left motion-tracked controller.
        /// </summary>
        [Obsolete("Use TrackedObjectType.MotionController and TrackedHandedness instead")]
        MotionControllerLeft = 1,
        /// <summary>
        /// (Obsolete) Calculates position and orientation from the right motion-tracked controller.
        /// </summary>
        [Obsolete("Use TrackedObjectType.MotionController and TrackedHandedness instead")]
        MotionControllerRight = 2,
        /// <summary>
        /// (Obsolete) Calculates position and orientation from a tracked hand joint on the left hand.
        /// </summary>
        [Obsolete("Use TrackedObjectType.HandJoint and TrackedHandedness instead")]
        HandJointLeft = 3,
        /// <summary>
        /// (Obsolete) Calculates position and orientation from a tracked hand joint on the right hand.
        /// </summary>
        [Obsolete("Use TrackedObjectType.HandJoint and TrackedHandedness instead")]
        HandJointRight = 4,

        /// <summary>
        /// Calculates position and orientation from the system-calculated ray of available controller (i.e motion controllers, hands, etc.)
        /// </summary>
        ControllerRay = 5,
        /// <summary>
        /// Calculates position and orientation from a tracked hand joint
        /// </summary>
        HandJoint = 6,
        /// <summary>
        /// Calculates position and orientation from a tracked hand joint
        /// </summary>
        CustomOverride = 7,
    }
}