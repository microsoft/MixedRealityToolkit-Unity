// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;
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

        [SerializeField]
        [Tooltip("With this active, solver will activate the UI after the palm threshold has been met and the user gazes at the activation point")]
        private bool useGazeActivation = false;

        /// <summary> 
        /// With this active, solver will activate after the palm threshold has been met and the user gazes at the activation point
        /// </summary>
        public bool UseGazeActivation
        {
            get => useGazeActivation;
            set => useGazeActivation = value;
        }

        [SerializeField]
        [Tooltip("The distance between the planar intersection of the eye gaze ray and the activation transform. Uses square magnitude between two points for distance")]
        [Range(0.0f, .1f)]
        private float eyeGazeProximityThreshold = .005f;
        /// <summary>
        /// The distance threshold calculated between the planar intersection of the eye gaze ray and the activation transform. Uses square magnitude between two points for distance
        /// </summary>
        public float EyeGazeProximityThreshold
        {
            get => eyeGazeProximityThreshold;
            set => eyeGazeProximityThreshold = value;
        }

        [SerializeField]
        [Tooltip("The distance between the planar intersection of the head gaze ray and the activation transform. Uses square magnitude between two points for distance")]
        [Range(0.0f, .1f)]
        private float headGazeProximityThreshold = .02f;

        /// <summary>
        /// The distance threshold calculated between the planar intersection of the head gaze ray and the activation transform. Uses square magnitude between two points for distance
        /// </summary>
        public float HeadGazeProximityThreshold
        {
            get => eyeGazeProximityThreshold;
            set => eyeGazeProximityThreshold = value;
        }

        private bool targetWorldLocked = false;

        /// <summary>
        /// Refects whether the current solver object is world-locked or not
        /// </summary>
        public bool TargetWorldLocked
        {
            get => targetWorldLocked;
            set => targetWorldLocked = value;
        }

        private bool gazeActivationAlreadyTriggered = false;

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

                        if (useGazeActivation && (!gazeActivationAlreadyTriggered || targetWorldLocked))
                        {
                            return IsUserGazeMeetingThresholdRequirements(jointedHand);
                        }
                    }

                    gazeActivationAlreadyTriggered = palmFacingThresholdMet ? gazeActivationAlreadyTriggered : false;

                    return palmFacingThresholdMet;
                }
                else
                {
                    Debug.LogError("HandConstraintPalmUp requires controllers of type IMixedRealityHand to perform hand activation tests.");
                }

                return palmFacingThresholdMet;
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the palm is currently facing the user; and if required, is it currently flat
        /// </summary>
        /// <param name="jointedHand"></param>
        /// <param name="palmPose"></param>
        /// <param name="palmCameraAngle"></param>
        /// <returns></returns>
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
        /// <param name="jointedHand"></param>
        /// <returns></returns>
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
                if (GenerateHandPlaneAndActivationPoint(jointedHand, out handPlane, out activationPoint) && 
                    handPlane.Raycast(gazeRay, out distanceToHandPlane))
                {
                        // Now that we know the dist to the plane, create a vector at that point
                        Vector3 gazePosOnPlane = gazeRay.origin + gazeRay.direction.normalized * distanceToHandPlane;
                        Vector3 PlanePos = handPlane.ClosestPointOnPlane(gazePosOnPlane);
                        float gazePosDistToActivationPosition = (activationPoint - PlanePos).sqrMagnitude;
                        float gazeActivationThreshold = usedEyeGaze ? eyeGazeProximityThreshold : headGazeProximityThreshold;
                        bool gazeActivated = gazeActivationAlreadyTriggered = (gazePosDistToActivationPosition < gazeActivationThreshold);

                        return gazeActivated;
                    }
                }

            return false;
        }

        /// <summary>
        /// Coroutine function called by the ManipulationHandler of the attached object whenever the object is done 
        /// being manipulated by the user. This triggers a coroutine that checks to see whether the object should 
        /// reattach to the hand.
        /// </summary>
        public void StartWorldLockReattachCheckCorotine()
        {
            StartCoroutine(WorldLockedReattachCheck());
        }

        private bool GenerateHandPlaneAndActivationPoint(IMixedRealityHand jointedHand, out Plane handPlane, out Vector3 activationPoint)
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
                activationPoint = handPlane.ClosestPointOnPlane(CalculateActivationPointBasedOnCurrentSafeZone(jointedHand));
                return true;
            }
            else // Otherwise, set the activation point and plane to default values and return false
            {
                handPlane = new Plane();
                activationPoint = Vector3.zero;
                return false;
            }
        }

        private Vector3 CalculateActivationPointBasedOnCurrentSafeZone(IMixedRealityHand jointedHand)
        {
            MixedRealityPose referenceJoint1;
            MixedRealityPose referenceJoint2;

            switch (SafeZone)
            {
                case SolverSafeZone.AboveFingerTips:
                    if (!jointedHand.TryGetJoint(TrackedHandJoint.MiddleTip, out referenceJoint1) ||
                        !jointedHand.TryGetJoint(TrackedHandJoint.RingTip, out referenceJoint2))
                    { 
                        return Vector3.zero;
                    }
                    break;
                case SolverSafeZone.BelowWrist:
                    return jointedHand.TryGetJoint(TrackedHandJoint.Wrist, out referenceJoint1) ? referenceJoint1.Position : Vector3.zero;

                case SolverSafeZone.RadialSide:
                    if (!jointedHand.TryGetJoint(TrackedHandJoint.Wrist, out referenceJoint1) ||
                        !jointedHand.TryGetJoint(TrackedHandJoint.PinkyKnuckle, out referenceJoint2))
                    {
                        return Vector3.zero;
                    }
                    break;
                case SolverSafeZone.UlnarSide:
                default:
                    if (!jointedHand.TryGetJoint(TrackedHandJoint.IndexKnuckle, out referenceJoint1) ||
                        !jointedHand.TryGetJoint(TrackedHandJoint.ThumbProximalJoint, out referenceJoint2))
                    {
                        return Vector3.zero;
                    }
                    break;                       
            }

            return Vector3.Lerp(referenceJoint1.Position, referenceJoint2.Position, .5f);
        }

        /// <summary>
        /// Coroutine function that's invoked when the attached object becomes world-locked. It uses the 
        /// logical checks invoked during IsValidController to determine whether the menu should reattach
        /// to the hand or not.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WorldLockedReattachCheck()
        {
            while (targetWorldLocked && useGazeActivation)
            {
                MixedRealityPose palmPose;
                var jointedHand = GetController(SolverHandler.CurrentTrackedHandedness) as IMixedRealityHand;
                if (jointedHand != null)
                {
                    if (jointedHand.TryGetJoint(TrackedHandJoint.Palm, out palmPose))
                    {
                        float palmCameraAngle = Vector3.Angle(palmPose.Up, CameraCache.Main.transform.forward);
                        if (IsPalmMeetingThresholdRequirements(jointedHand, palmPose, palmCameraAngle) &&
                            IsUserGazeMeetingThresholdRequirements(jointedHand))
                        {
                            gazeActivationAlreadyTriggered = false;
                            targetWorldLocked = false;
                            SolverHandler.UpdateSolvers = true;
                        }
                    }
                }

                yield return null;
            }
        }
    }
}
