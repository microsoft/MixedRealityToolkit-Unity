// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Augments the HandConstraint to also check if the palm is facing the user before activation.
    /// </summary>
    public class HandConstraintPalmUp : HandConstraint
    {
        [Header("Palm Up")]
        [SerializeField]
        [Tooltip("TODO")]
        private float jointFacingThreshold = 0.3f;

        /// <summary>
        /// TODO
        /// </summary>
        public float JointFacingThreshold
        {
            get { return jointFacingThreshold; }
            set { jointFacingThreshold = value; }
        }

        /// <summary>
        /// Determines if a hand meets the requirements for use with constraining the tracked object and determines if the 
        /// palm is currently facing the user.
        /// </summary>
        /// <param name="hand">The hand to check against.</param>
        /// <returns>True if this hand should be used from tracking.</returns>
        protected override bool IsHandActive(IMixedRealityHand hand)
        {
            if (!base.IsHandActive(hand))
            {
                return false;
            }

            MixedRealityPose pose;

            if (hand.TryGetJoint(TrackedHandJoint.Palm, out pose))
            {
                return Vector3.Dot(pose.Up, CameraCache.Main.transform.forward) > jointFacingThreshold;
            }

            return true;
        }
    }
}
