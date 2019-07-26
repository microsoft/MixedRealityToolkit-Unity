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

        [SerializeField]
        [Tooltip("Do the fingers on the hand need to be straitened, rather than curled, to form a flat hand shape.")]
        private bool requireFlatHand = false;

        /// <summary>
        /// Do the fingers on the hand need to be straitened, rather than curled, to form a flat hand shape.
        /// </summary>
        public bool RequireFlatHand
        {
            get { return requireFlatHand; }
            set { requireFlatHand = value; }
        }

        [SerializeField]
        [Tooltip("The angle (in degrees) of the cone between the palm's up and triangle's normal formed from the palm, to index, to ring finger tip have to match.")]
        [Range(0.0f, 90.0f)]
        private float flatHandThreshold = 45.0f;

        /// <summary>
        /// The angle (in degrees) of the cone between the palm's up and triangle's normal formed from the palm, to index, to ring finger tip have to match.
        /// </summary>
        public float FlatHandThreshold
        {
            get { return flatHandThreshold; }
            set { flatHandThreshold = value; }
        }

        /// <summary>
        /// Determines if a hand meets the requirements for use with constraining the tracked object and determines if the 
        /// palm is currently facing the user.
        /// </summary>
        /// <param name="hand">The hand to check against.</param>
        /// <returns>True if this hand should be used from tracking.</returns>
        protected override bool IsHandActive(IMixedRealityController hand)
        {
            if (!base.IsHandActive(hand))
            {
                return false;
            }

            MixedRealityPose palmPose;

            var jointedHand = hand as IMixedRealityHand;

            if (jointedHand != null)
            {
                if (jointedHand.TryGetJoint(TrackedHandJoint.Palm, out palmPose))
                {
                    if (requireFlatHand)
                    {
                        // Check if the triangle's normal formed from the palm, to index, to ring finger tip roughly matches the palm normal.
                        MixedRealityPose indexTipPose, ringTipPose;

                        if (jointedHand.TryGetJoint(TrackedHandJoint.IndexTip, out indexTipPose) &&
                            jointedHand.TryGetJoint(TrackedHandJoint.RingTip, out ringTipPose))
                        {
                            var handNormal = Vector3.Cross(indexTipPose.Position - palmPose.Position,
                                                           ringTipPose.Position - indexTipPose.Position).normalized;
                            handNormal *= (jointedHand.ControllerHandedness == Handedness.Right) ? 1.0f : -1.0f;

                            if (Vector3.Angle(palmPose.Up, handNormal) > flatHandThreshold)
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("HandConstraintPalmUp requires controllers of type IMixedRealityHand to perform hand activation tests.");
                }

                return Vector3.Angle(palmPose.Up, CameraCache.Main.transform.forward) < facingThreshold;
            }

            return true;
        }
    }
}
