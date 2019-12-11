// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Augments the HandConstraint to also check if the palm is facing the user before activation. This solver only works 
    /// with <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> controllers, with other <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityController"/> types this solver will behave just like it's base class.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/HandConstraintPalmUp")]
    public class HandConstraintPalmUp : HandConstraint
    {
        [Header("Palm Up")]
        [SerializeField]
        [Tooltip("The angle (in degrees) of the cone between the palm's up and camera's forward have to match. Only supported by IMixedRealityHand controllers.")]
        [Range(0.0f, 90.0f)]
        private float facingThreshold = 80.0f;

        /// <summary>
        /// The angle (in degrees) of the cone between the palm's up and camera's forward have to match. Only supported by <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> controllers.
        /// </summary>
        public float FacingThreshold
        {
            get { return facingThreshold; }
            set { facingThreshold = value; }
        }

        [SerializeField]
        [Tooltip("Do the fingers on the hand need to be straightened, rather than curled, to form a flat hand shape. Only supported by IMixedRealityHand controllers.")]
        private bool requireFlatHand = false;

        /// <summary>
        /// Do the fingers on the hand need to be straightened, rather than curled, to form a flat hand shape. Only supported by <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> controllers.
        /// </summary>
        public bool RequireFlatHand
        {
            get { return requireFlatHand; }
            set { requireFlatHand = value; }
        }

        [SerializeField]
        [Tooltip("The angle (in degrees) of the cone between the palm's up and triangle's normal formed from the palm, to index, to ring finger tip have to match. Only supported by IMixedRealityHand controllers.")]
        [Range(0.0f, 90.0f)]
        private float flatHandThreshold = 45.0f;

        /// <summary>
        /// The angle (in degrees) of the cone between the palm's up and triangle's normal formed from the palm, to index, to ring finger tip have to match. Only supported by <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> controllers.
        /// </summary>
        public float FlatHandThreshold
        {
            get { return flatHandThreshold; }
            set { flatHandThreshold = value; }
        }

        /// <summary>
        /// Determines if a controller meets the requirements for use with constraining the tracked object and determines if the 
        /// palm is currently facing the user.
        /// </summary>
        /// <param name="controller">The hand to check against.</param>
        /// <returns>True if this hand should be used from tracking.</returns>
        protected override bool IsValidController(IMixedRealityController controller)
        {
            if (!base.IsValidController(controller))
            {
                return false;
            }

            MixedRealityPose palmPose;

            var jointedHand = controller as IMixedRealityHand;

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
