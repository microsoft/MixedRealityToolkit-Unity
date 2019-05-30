// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities.Solvers
{
    /// <summary>
    /// Augments the HandConstraint to also check if the palm is facing the user before activation.
    /// </summary>
    public class HandConstraintPalmUp : HandConstraint
    {
        [Header("Palm Up")]
        [SerializeField]
        [Tooltip("The angle (in degrees) of the cone between the palm's up and camera's forward have to match.")]
        [Range(0.0f, 90.0f)]
        private float facingThreshold = 80.0f;

        /// <summary>
        /// The angle (in degrees) of the cone between the palm's up and camera's forward have to match.
        /// </summary>
        public float FacingThreshold
        {
            get { return facingThreshold; }
            set { facingThreshold = value; }
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
                return Vector3.Angle(pose.Up, CameraCache.Main.transform.forward) < facingThreshold;
            }

            return true;
        }
    }
}
