// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Augments the HandConstraint to also check if the palm is facing the user before activation.
    /// This solver only works with articulated hands, with other controller types this solver will
    /// behave the same as <see cref="HandConstraint"/>.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Solvers/Hand Constraint (Palm Up)")]
    public class HandConstraintPalmUp : HandConstraint
    {
        [Header("Palm Up")]
        [SerializeField]
        [FormerlySerializedAs("facingThreshold")]
        [Tooltip("The angle (in degrees) of the cone between the palm's up and camera's forward have to match.")]
        [Range(0.0f, 90.0f)]
        private float facingCameraTrackingThreshold = 80.0f;

        /// <summary>
        /// The angle (in degrees) of the cone between the palm's up and camera's forward have to match.
        /// </summary>
        public float FacingCameraTrackingThreshold
        {
            get => facingCameraTrackingThreshold;
            set => facingCameraTrackingThreshold = value;
        }

        [SerializeField]
        [Tooltip("Do the fingers on the hand need to be straightened, rather than curled, to form a flat hand shape.")]
        private bool requireFlatHand = false;

        /// <summary>
        /// Do the fingers on the hand need to be straightened, rather than curled, to form a flat hand shape.
        /// </summary>
        public bool RequireFlatHand
        {
            get => requireFlatHand;
            set => requireFlatHand = value;
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

        private static readonly ProfilerMarker IsValidControllerPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraintPalmUp.IsValidController");

        /// <summary>
        /// Determines if a hand meets the requirements for use with constraining the 
        /// tracked object and determines if the palm is currently facing the user.
        /// </summary>
        /// <param name="hand">XRNode representing the hand to validate.</param>
        /// <returns>
        /// True if this hand should be used for tracking.
        /// </returns>
        /// <remarks>
        /// This method will modify the position and rotation behavior of the hand
        /// constraint if the followHandUntilFacingCamera variable is enabled.
        /// </remarks>
        protected override bool IsValidController(XRNode? hand)
        {
            using (IsValidControllerPerfMarker.Auto())
            {
                if (!base.IsValidController(hand))
                {
                    return false;
                }

                bool palmFacingThresholdMet = false;

                if (XRSubsystemHelpers.HandsAggregator != null &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, hand.Value, out HandJointPose palmPose))
                {
                    float dotProduct = Vector3.Dot(palmPose.Up, Camera.main.transform.forward);
                    if (dotProduct >= 0)
                    {
                        float palmCameraAngle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

                        palmFacingThresholdMet = IsPalmMeetingThresholdRequirements(hand.Value, palmPose, palmCameraAngle);

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
                                return IsUserGazeMeetingThresholdRequirements(hand.Value);
                            }
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
        }

        private static readonly ProfilerMarker IsPalmMeetingThresholdRequirementsPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraintPalmUp.IsPalmMeetingThresholdRequirements");

        /// <summary>
        /// Checks to see if the palm is currently facing the user; and if required, 
        /// if it is currently flat.
        /// </summary>
        /// <param name="hand">
        /// XRNode representing the user's hand that is checked if the palm is
        /// meeting the threshold requirements.
        /// </param>
        /// <param name="palmPose">
        /// Pose used to determine if the hand is flat.
        /// </param>
        /// <param name="palmCameraAngle">
        /// The palm's current angle that's used to determine if it meets the min threshold.
        /// </param>
        /// <returns>
        /// True if the palm is meeting the threshold requirements, or false.
        /// </returns>
        private bool IsPalmMeetingThresholdRequirements(
            XRNode hand,
            HandJointPose palmPose,
            float palmCameraAngle)
        {
            using (IsPalmMeetingThresholdRequirementsPerfMarker.Auto())
            {
                if (requireFlatHand)
                {
                    // Check if the triangle's normal formed from the palm, to index, to ring finger tip roughly matches the palm normal.
                    if (XRSubsystemHelpers.HandsAggregator != null &&
                        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, hand, out HandJointPose indexTipPose) &&
                        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.RingTip, hand, out HandJointPose ringTipPose))
                    {
                        var handNormal = Vector3.Cross(indexTipPose.Position - palmPose.Position,
                                                       ringTipPose.Position - indexTipPose.Position).normalized;
                        handNormal *= (hand.ToHandedness() == Handedness.Right) ? 1.0f : -1.0f;

                        if (Vector3.Angle(palmPose.Up, handNormal) > flatHandThreshold)
                        {
                            return false;
                        }
                    }
                }

                // Check if the palm angle meets the prescribed threshold
                return palmCameraAngle < facingCameraTrackingThreshold;
            }
        }

        private static readonly ProfilerMarker IsUserGazeMeetingThresholdRequirementsPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraintPalmUp.IsUserGazeMeetingThresholdRequirements");

        /// <summary>
        /// Checks to see if the user is currently gazing at the activation point; it first attempts to do so 
        /// using eyegaze, and then falls back to head-based gaze if eyegaze isn't available for use.
        /// </summary>
        /// <param name="hand">
        /// The XRNode representing the user's hand that is used to determine if user gaze meets the gaze threshold.
        /// </param>
        /// <returns>
        /// True if the user's gaze is within the proximity threshold of the activation point (both relative to the
        /// hand plane), or false.
        /// </returns>
        private bool IsUserGazeMeetingThresholdRequirements(XRNode hand)
        {
            using (IsUserGazeMeetingThresholdRequirementsPerfMarker.Auto())
            {
                Ray? gazeRay = null;
                bool usedEyeGaze = false;

                if (controllerLookup != null &&
                    controllerLookup.GazeController != null &&
                    (controllerLookup.GazeController.currentControllerState.inputTrackingState &
                    (InputTrackingState.Position | InputTrackingState.Rotation)) > 0)
                {
                    gazeRay = new Ray(
                            controllerLookup.GazeController.transform.position,
                            controllerLookup.GazeController.transform.forward);
                    usedEyeGaze = true;
                }
                else
                {
                    gazeRay = new Ray(
                        Camera.main.transform.position,
                        Camera.main.transform.forward);
                }

                if (gazeRay.HasValue)
                {
                    // Define the activation point as a vector between the wrist and pinky knuckle; then cast it against the plane to get a smooth location
                    // If we can generate the handplane/are able to set an activation point on it, and then are able to raycast against it
                    if (TryGenerateHandPlaneAndActivationPoint(hand, out Plane handPlane, out Vector3 activationPoint) &&
                        handPlane.Raycast(gazeRay.Value, out float distanceToHandPlane))
                    {
                        // Now that we know the dist to the plane, create a vector at that point
                        Vector3 gazePosOnPlane = gazeRay.Value.origin + gazeRay.Value.direction.normalized * distanceToHandPlane;
                        Vector3 planePos = handPlane.ClosestPointOnPlane(gazePosOnPlane);
                        float gazePosDistToActivationPosition = (activationPoint - planePos).sqrMagnitude;
                        float gazeActivationThreshold = usedEyeGaze ? eyeGazeProximityThreshold : headGazeProximityThreshold;
                        gazeActivationAlreadyTriggered = (gazePosDistToActivationPosition < gazeActivationThreshold);

                        return gazeActivationAlreadyTriggered;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Coroutine function called by the ObjectManipulator of the attached object whenever the object is done 
        /// being manipulated by the user. This triggers a coroutine that checks to see whether the object should 
        /// reattach to the hand.
        /// </summary>
        public void StartWorldLockReattachCheckCoroutine()
        {
            StartCoroutine(WorldLockedReattachCheck());
        }

        private static readonly ProfilerMarker TryGenerateHandPlaneAndActivationPointPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraintPalmUp.TryGenerateHandPlaneAndActivationPoint");

        /// <summary>
        /// This function attempts to generate a hand plane based on the wrist, index knuckle and pinky
        /// knuckle joints present in the hand. On a success, it then calls GenerateActivationPoint to
        /// try to generate a hand-based activation point that the user needs to gaze at to activate the
        /// constrained object. On a failure, it assigns them to be default values and then returns false
        /// </summary>
        /// <param name="hand">
        /// XRNode representing the user's hand that is used to generate the hand plane and activation point.
        /// </param>
        /// <param name="handPlane">
        /// Out Plane that represents the hand and is raycasted against to determine whether the users gaze is
        /// close to the activation point.
        /// </param>
        /// <param name="activationPoint">
        /// Out Vector3 that represents the point on the hand-based plane to determine if  the menu activates.
        /// </param>
        /// <returns>
        /// True if the function can properly generate an activation point using the hand-based plane.
        /// </returns>
        private bool TryGenerateHandPlaneAndActivationPoint(
            XRNode hand,
            out Plane handPlane,
            out Vector3 activationPoint)
        {
            using (TryGenerateHandPlaneAndActivationPointPerfMarker.Auto())
            {
                // Generate the hand plane that we're using to generate a distance value.
                // This is done by using the index knuckle, pinky knuckle, and wrist
                if (XRSubsystemHelpers.HandsAggregator != null &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexProximal, hand, out HandJointPose indexKnuckle) &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleProximal, hand, out HandJointPose pinkyKnuckle) &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, hand, out HandJointPose wrist))
                {
                    handPlane = new Plane(indexKnuckle.Position, pinkyKnuckle.Position, wrist.Position);
                    if (TryGenerateActivationPoint(hand, out Vector3 generatedActivationPoint))
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
        }

        private static readonly ProfilerMarker TryGenerateActivationPointPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraintPalmUp.TryGenerateActivationPoint");

        /// <summary>
        /// This function attempts to generate an activation point based on what the currently-selected safe zone is.
        /// activate the attached menu. If joints successfully obtained, assigns activation point for currently selected
        /// safe zone and returns true. On failure, assigns it Vector3.zero and returns false.
        /// </summary>
        /// <param name="hand">An instance of the user's hand that is being used to generate an activation point for the menu</param>
        /// <param name="activationPoint"> Out Vector3 that represents the generated activation point for the selected safe zone</param>
        /// <returns>True if all of the joints used for generating the activation point are present (and the point can be calculated)</returns>
        private bool TryGenerateActivationPoint(
            XRNode hand,
            out Vector3 activationPoint)
        {
            using (TryGenerateActivationPointPerfMarker.Auto())
            {
                TrackedHandJoint referenceJoint1;
                TrackedHandJoint referenceJoint2;
                HandJointPose referenceJointPose1;
                activationPoint = Vector3.zero;

                switch (SafeZone)
                {
                    case SolverSafeZone.AboveFingerTips:
                        referenceJoint1 = TrackedHandJoint.MiddleTip;
                        referenceJoint2 = TrackedHandJoint.RingTip;
                        break;

                    case SolverSafeZone.BelowWrist:
                        if (XRSubsystemHelpers.HandsAggregator != null &&
                            XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Wrist, hand, out referenceJointPose1))
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
                        referenceJoint1 = TrackedHandJoint.IndexProximal;
                        referenceJoint2 = TrackedHandJoint.ThumbProximal;
                        break;

                    case SolverSafeZone.UlnarSide:
                    default:
                        referenceJoint1 = TrackedHandJoint.LittleProximal;
                        referenceJoint2 = TrackedHandJoint.Wrist;
                        break;
                }

                if (XRSubsystemHelpers.HandsAggregator == null ||
                    !XRSubsystemHelpers.HandsAggregator.TryGetJoint(referenceJoint1, hand, out referenceJointPose1) ||
                    !XRSubsystemHelpers.HandsAggregator.TryGetJoint(referenceJoint2, hand, out HandJointPose referenceJointPose2))
                {
                    return false;
                }


                activationPoint = Vector3.Lerp(referenceJointPose1.Position, referenceJointPose2.Position, .5f);
                return true;
            }
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
                XRNode? hand = SolverHandler.CurrentTrackedHandedness.ToXRNode();

                if (hand.HasValue)
                {
                    if (XRSubsystemHelpers.HandsAggregator != null &&
                        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, hand.Value, out HandJointPose palmPose))
                    {
                        float palmCameraAngle = Vector3.Angle(palmPose.Up, Camera.main.transform.forward);
                        if (IsPalmMeetingThresholdRequirements(hand.Value, palmPose, palmCameraAngle) &&
                            IsUserGazeMeetingThresholdRequirements(hand.Value))
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
