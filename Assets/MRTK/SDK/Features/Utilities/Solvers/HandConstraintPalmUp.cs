// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Augments the HandConstraint to also check if the palm is facing the user before activation. This solver only works 
    /// with <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> controllers, with other <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityController"/> types this solver will behave just like its base class.
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

        [SerializeField]
        [Tooltip("With this active, solver will activate the UI after the palm threshold has been met and the user gazes at the activation point. If eye gaze information is not available, the head gaze will be used.")]
        private bool useGazeActivation = false;

        /// <summary> 
        /// With this active, solver will activate after the palm threshold has been met and the user gazes at the activation point.  If eye gaze information is not available, the head gaze will be used.
        /// </summary>
        public bool UseGazeActivation
        {
            get => useGazeActivation;
            set => useGazeActivation = value;
        }

        [SerializeField]
        [Tooltip("The distance between the planar intersection of the eye gaze ray and the activation transform. Uses square magnitude between two points for distance")]
        [Range(0.0f, .2f)]
        private float eyeGazeProximityThreshold = .01f;
        /// <summary>
        /// The distance threshold calculated between the planar intersection of the eye gaze ray and the activation transform. Uses square magnitude between two points for distance
        /// </summary>
        public float EyeGazeProximityThreshold
        {
            get => eyeGazeProximityThreshold;
            set => eyeGazeProximityThreshold = value;
        }

        [SerializeField]
        [Tooltip("The distance between the planar intersection of the head gaze ray and the activation transform. Uses square magnitude between two points for distance. This is used if eye gaze isn't available for the user")]
        [Range(0.0f, .2f)]
        private float headGazeProximityThreshold = .04f;

        /// <summary>
        /// The distance threshold calculated between the planar intersection of the head gaze ray and the activation transform. Uses square magnitude between two points for distance
        /// This is used if eye gaze isn't available for the user
        /// </summary>
        public float HeadGazeProximityThreshold
        {
            get => headGazeProximityThreshold;
            set => headGazeProximityThreshold = value;
        }

        private bool gazeActivationAlreadyTriggered = false;

        /// <summary>
        /// Determines if a controller meets the requirements for use with constraining the tracked object and determines if the 
        /// palm is currently facing the user. This function will modify the position and rotation behavior of the hand constraint if 
        /// the followHandUntilFacingCamera variable is enabled.
        /// </summary>
        /// <param name="controller">The hand to check against.</param>
        /// <returns>True if this hand should be used for tracking.</returns>
        protected override bool IsValidController(IMixedRealityController controller)
        {
            if (!base.IsValidController(controller))
            {
                return false;
            }

            if (!controller.IsPositionAvailable)
            {
                // A fully populated hand controller will have position 
                // information available.
                return false;
            }

            bool palmFacingThresholdMet = false;

            if (controller is IMixedRealityHand jointedHand)
            {
                if (jointedHand.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose palmPose))
                {
                    float palmCameraAngle = Vector3.Angle(palmPose.Up, CameraCache.Main.transform.forward);

                    palmFacingThresholdMet = IsPalmMeetingThresholdRequirements(jointedHand, palmPose, palmCameraAngle);

                    // If using hybrid hand rotation, we proceed with additional checks
                    if (palmFacingThresholdMet)
                    {
                        if (followHandUntilFacingCamera)
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

                        if (useGazeActivation && (!gazeActivationAlreadyTriggered || !SolverHandler.UpdateSolvers))
                        {
                            return IsUserGazeMeetingThresholdRequirements(jointedHand);
                        }
                    }

                    if (!palmFacingThresholdMet)
                    {
                        gazeActivationAlreadyTriggered = false;
                    }

                    return palmFacingThresholdMet;
                }

                return palmFacingThresholdMet;
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the palm is currently facing the user; and if required, is it currently flat.
        /// </summary>
        /// <param name="jointedHand">Reference to the user hand that is checked if the palm is meeting the threshold requirements</param>
        /// <param name="palmPose">Reference to the palm pose that's used to determine if the hand is flat or not</param>
        /// <param name="palmCameraAngle">The palm's current angle that's used to determine if it meets the min threshold or not</param>
        /// <returns>True if the palm is meeting the threshold requirements</returns>
        private bool IsPalmMeetingThresholdRequirements(IMixedRealityHand jointedHand, MixedRealityPose palmPose, float palmCameraAngle)
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

            // Check if the palm angle meets the prescribed threshold
            return palmCameraAngle < facingCameraTrackingThreshold;
        }

        /// <summary>
        /// Checks to see if the user is currently gazing at the activation point; it first attempts to do so 
        /// using eyegaze, and then falls back to head-based gaze if eyegaze isn't available for use.
        /// </summary>
        /// <param name="jointedHand">Hand reference to the user's hand that is used to determine if user gaze meets the gaze threshold</param>
        /// <returns>True if the user's gaze is within the proximity threshold of the activation point (both relative to the hand plane)</returns>
        private bool IsUserGazeMeetingThresholdRequirements(IMixedRealityHand jointedHand)
        {
            Ray gazeRay;

            bool usedEyeGaze = InputRayUtils.TryGetRay(InputSourceType.Eyes, Handedness.Any, out gazeRay);

            if (usedEyeGaze || InputRayUtils.TryGetRay(InputSourceType.Head, Handedness.Any, out gazeRay))
            {
                // Define the activation point as a vector between the wrist and pinky knuckle; then cast it against the plane to get a smooth location
                Vector3 activationPoint;
                Plane handPlane;
                float distanceToHandPlane;

                // If we can generate the handplane/are able to set an activation point on it, and then are able to raycast against it
                if (TryGenerateHandPlaneAndActivationPoint(jointedHand, out handPlane, out activationPoint) &&
                    handPlane.Raycast(gazeRay, out distanceToHandPlane))
                {
                    // Now that we know the dist to the plane, create a vector at that point
                    Vector3 gazePosOnPlane = gazeRay.origin + gazeRay.direction.normalized * distanceToHandPlane;
                    Vector3 planePos = handPlane.ClosestPointOnPlane(gazePosOnPlane);
                    float gazePosDistToActivationPosition = (activationPoint - planePos).sqrMagnitude;
                    float gazeActivationThreshold = usedEyeGaze ? eyeGazeProximityThreshold : headGazeProximityThreshold;
                    gazeActivationAlreadyTriggered = (gazePosDistToActivationPosition < gazeActivationThreshold);

                    return gazeActivationAlreadyTriggered;
                }
            }

            return false;
        }

        /// <summary>
        /// Coroutine function called by the ManipulationHandler of the attached object whenever the object is done 
        /// being manipulated by the user. This triggers a coroutine that checks to see whether the object should 
        /// reattach to the hand.
        /// </summary>
        public void StartWorldLockReattachCheckCoroutine()
        {
            StartCoroutine(WorldLockedReattachCheck());
        }

        /// <summary>
        /// This function attempts to generate a hand plane based on the wrist, index knuckle and pinky knuckle joints present in the hand.
        /// On a success, it then calls GenerateActivationPoint to try to generate a hand-based activation point that the user
        /// needs to gaze at to activate the constrained object.
        /// On a failure, it assigns them to be default values and then returns false
        /// </summary>
        /// <param name="jointedHand">Hand reference to the user's hand that is used to generate the hand plane and activation point</param>
        /// <param name="handPlane">Out Plane that represents the hand and is raycasted against to determine whether the users gaze is close to the activation point or not</param>
        /// <param name="activationPoint">Out Vector3 that represents the point on the hand-based plane to determine whether the menu activates or not</param>
        /// <returns>True if the function can properly generate an activation point using the hand-based plane</returns>
        private bool TryGenerateHandPlaneAndActivationPoint(IMixedRealityHand jointedHand, out Plane handPlane, out Vector3 activationPoint)
        {
            // Generate the hand plane that we're using to generate a distance value.
            // This is done by using the index knuckle, pinky knuckle, and wrist
            MixedRealityPose indexKnuckle;
            MixedRealityPose pinkyKnuckle;
            MixedRealityPose wrist;

            if (jointedHand.TryGetJoint(TrackedHandJoint.IndexKnuckle, out indexKnuckle) &&
                jointedHand.TryGetJoint(TrackedHandJoint.PinkyKnuckle, out pinkyKnuckle) &&
                jointedHand.TryGetJoint(TrackedHandJoint.Wrist, out wrist))
            {
                handPlane = new Plane(indexKnuckle.Position, pinkyKnuckle.Position, wrist.Position);
                Vector3 generatedActivationPoint;
                if (TryGenerateActivationPoint(jointedHand, out generatedActivationPoint))
                {
                    activationPoint = handPlane.ClosestPointOnPlane(generatedActivationPoint);
                    return true;
                }
                else
                {
                    activationPoint = Vector3.zero;
                    return false;
                }
            }
            else // Otherwise, set the activation point and plane to default values and return false
            {
                handPlane = new Plane();
                activationPoint = Vector3.zero;
                return false;
            }
        }

        /// <summary>
        /// This function attempts to generate an activation point based on what the currently-selected safe zone is.
        /// activate the attached menu. If joints successfully obtained, assigns activation point for currently selected
        /// safe zone and returns true. On failure, assigns it Vector3.zero and returns false.
        /// </summary>
        /// <param name="jointedHand">An instance of the user's hand that is being used to generate an activation point for the menu</param>
        /// <param name="activationPoint"> Out Vector3 that represents the generated activation point for the selected safe zone</param>
        /// <returns>True if all of the joints used for generating the activation point are present (and the point can be calculated)</returns>
        private bool TryGenerateActivationPoint(IMixedRealityHand jointedHand, out Vector3 activationPoint)
        {
            TrackedHandJoint referenceJoint1;
            TrackedHandJoint referenceJoint2;
            MixedRealityPose referenceJointPose1;
            MixedRealityPose referenceJointPose2;
            activationPoint = Vector3.zero;

            switch (SafeZone)
            {
                case SolverSafeZone.AboveFingerTips:
                    referenceJoint1 = TrackedHandJoint.MiddleTip;
                    referenceJoint2 = TrackedHandJoint.RingTip;
                    break;
                case SolverSafeZone.BelowWrist:
                    if (jointedHand.TryGetJoint(TrackedHandJoint.Wrist, out referenceJointPose1))
                    {
                        activationPoint = referenceJointPose1.Position;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case SolverSafeZone.AtopPalm:
                    referenceJoint1 = TrackedHandJoint.Palm;
                    referenceJoint2 = TrackedHandJoint.Palm;
                    break;
                case SolverSafeZone.RadialSide:
                    referenceJoint1 = TrackedHandJoint.IndexKnuckle;
                    referenceJoint2 = TrackedHandJoint.ThumbProximalJoint;
                    break;
                case SolverSafeZone.UlnarSide:
                default:
                    referenceJoint1 = TrackedHandJoint.PinkyKnuckle;
                    referenceJoint2 = TrackedHandJoint.Wrist;
                    break;
            }


            if (!jointedHand.TryGetJoint(referenceJoint1, out referenceJointPose1) ||
                !jointedHand.TryGetJoint(referenceJoint2, out referenceJointPose2))
            {
                return false;
            }


            activationPoint = Vector3.Lerp(referenceJointPose1.Position, referenceJointPose2.Position, .5f);
            return true;
        }

        /// <summary>
        /// Coroutine function that's invoked when the attached object becomes world-locked. It uses the 
        /// logical checks invoked during IsValidController to determine whether the menu should reattach
        /// to the hand or not.
        /// </summary>
        private IEnumerator WorldLockedReattachCheck()
        {
            while (!SolverHandler.UpdateSolvers && useGazeActivation)
            {
                MixedRealityPose palmPose;
                if (GetController(SolverHandler.CurrentTrackedHandedness) is IMixedRealityHand jointedHand)
                {
                    if (jointedHand.TryGetJoint(TrackedHandJoint.Palm, out palmPose))
                    {
                        float palmCameraAngle = Vector3.Angle(palmPose.Up, CameraCache.Main.transform.forward);
                        if (IsPalmMeetingThresholdRequirements(jointedHand, palmPose, palmCameraAngle) &&
                            IsUserGazeMeetingThresholdRequirements(jointedHand))
                        {
                            gazeActivationAlreadyTriggered = false;
                            SolverHandler.UpdateSolvers = true;
                        }
                    }
                }

                yield return null;
            }
        }


        #region MonoBehaviour Implementation

        /// <summary>
        /// When enabled, ensure that there are no outlying status changes that would prevent HandConstraintPalmUp from 
        /// properly working (like gazeActivationAlreadyTriggered being set to true previously)
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            gazeActivationAlreadyTriggered = false;
        }

        #endregion MonoBehaviour Implementation
    }
}
