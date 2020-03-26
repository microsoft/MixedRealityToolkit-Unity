// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("facingThreshold")]
        [Tooltip("The angle (in degrees) of the cone between the palm's up and camera's forward have to match. Only supported by IMixedRealityHand controllers.")]
        [Range(0.0f, 90.0f)]
        private float facingCameraTrackingThreshold = 80.0f;

        /// <summary>
        /// The angle (in degrees) of the cone between the palm's up and camera's forward have to match. Only supported by <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> controllers.
        /// </summary>
        public float FacingCameraTrackingThreshold
        {
            get => facingCameraTrackingThreshold;
            set => facingCameraTrackingThreshold = value;
        }

        [System.Obsolete("Use FacingCameraTrackingThreshold property instead")]
        public float FacingThreshold
        {
            get => FacingCameraTrackingThreshold;
            set => FacingCameraTrackingThreshold = value;
        }

        [SerializeField]
        [Tooltip("Do the fingers on the hand need to be straightened, rather than curled, to form a flat hand shape. Only supported by IMixedRealityHand controllers.")]
        private bool requireFlatHand = false;

        /// <summary>
        /// Do the fingers on the hand need to be straightened, rather than curled, to form a flat hand shape. Only supported by <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> controllers.
        /// </summary>
        public bool RequireFlatHand
        {
            get => requireFlatHand;
            set => requireFlatHand = value;
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
            get => flatHandThreshold;
            set => flatHandThreshold = value;
        }

        [SerializeField]
        [Tooltip("With this active, solver will follow hand rotation until the menu is sufficiently aligned with the gaze, at which point it faces the camera.")]
        private bool followHandUntilFacingCamera = false;

        /// <summary>
        /// With this active, solver will follow hand rotation until the menu is sufficiently aligned with the gaze, at which point it faces the camera.
        /// </summary>
        public bool FollowHandUntilFacingCamera
        {
            get => followHandUntilFacingCamera;
            set => followHandUntilFacingCamera = value;
        }

        [SerializeField]
        [Tooltip("Angle (in degrees) between hand up and camera forward, below which the hand menu follows the gaze, if followHandUntilFacingCamera is active.")]
        private float followHandCameraFacingThresholdAngle = 60f;

        /// <summary>
        /// Angle (in degrees) between hand up and camera forward, below which the hand menu follows the gaze, if followHandUntilFacingCamera is active.
        /// </summary>
        public float FollowHandCameraFacingThresholdAngle
        {
            get => followHandCameraFacingThresholdAngle;
            set => followHandCameraFacingThresholdAngle = value;
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

            bool palmFacingThresholdMet = false;

            if (jointedHand != null)
            {
                if (jointedHand.TryGetJoint(TrackedHandJoint.Palm, out palmPose))
                {
                    float palmCameraAngle = Vector3.Angle(palmPose.Up, CameraCache.Main.transform.forward);

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

                    // Check if the palm angle meets the prescribed threshold
                    palmFacingThresholdMet = palmCameraAngle < facingCameraTrackingThreshold;

                    // If using hybrid hand rotation, we proceed with additional checks
                    if (followHandUntilFacingCamera && palmFacingThresholdMet)
                    {
                        // If we are above the threshold angle, keep the menu mapped to the tracked object
                        if (palmCameraAngle > followHandCameraFacingThresholdAngle)
                        {
                            RotationBehavior = SolverRotationBehavior.LookAtTrackedObject;
                            OffsetBehavior = SolverOffsetBehavior.TrackedObjectRotation;
                        }
                        // If we are within the threshold angle, we snap to follow the camera
                        else
                        {
                            RotationBehavior = SolverRotationBehavior.LookAtMainCamera;
                            OffsetBehavior = SolverOffsetBehavior.LookAtCameraRotation;
                        }
                    }
                }
                else
                {
                    Debug.LogError("HandConstraintPalmUp requires controllers of type IMixedRealityHand to perform hand activation tests.");
                }

                return palmFacingThresholdMet;
            }

            return true;
        }
    }
}
