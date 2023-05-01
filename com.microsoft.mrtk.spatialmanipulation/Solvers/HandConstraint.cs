// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Provides a solver that constrains the target to a region safe for
    /// hand constrained interactive content. While this solver is intended
    /// to work with articulated hands, it also works with motion controllers. 
    /// </summary>
    [RequireComponent(typeof(HandBounds))]
    [AddComponentMenu("MRTK/Spatial Manipulation/Solvers/Hand Constraint")]
    public class HandConstraint : Solver
    {
        // This array intentionally leaves out AtopPalm. The zones of interest reside
        // around, not above the hand.
        private static readonly SolverSafeZone[] handSafeZonesClockWiseRightHand =
            new SolverSafeZone[]
            {
                SolverSafeZone.UlnarSide,
                SolverSafeZone.AboveFingerTips,
                SolverSafeZone.RadialSide,
                SolverSafeZone.BelowWrist
            };

        [Header("Hand Constraint")]
        [SerializeField]
        [Tooltip("Which part of the hand to move the solver towards. The ulnar side of the hand is recommended for most situations.")]
        private SolverSafeZone safeZone = SolverSafeZone.UlnarSide;

        /// <summary>
        /// Which part of the hand to move the tracked object towards. The ulnar side of the hand is recommended for most situations.
        /// </summary>
        public SolverSafeZone SafeZone
        {
            get => safeZone;
            set => safeZone = value;
        }

        [SerializeField]
        [Tooltip("Additional offset to apply to the intersection point with the hand bounds along the intersection point normal.")]
        private float safeZoneBuffer = 0.15f;

        /// <summary>
        /// Additional offset to apply to the intersection point with the hand bounds.
        /// </summary>
        public float SafeZoneBuffer
        {
            get => safeZoneBuffer;
            set => safeZoneBuffer = value;
        }

        [SerializeField]
        [Tooltip("Should the solver continue to move when the opposite hand (hand which is not being tracked) is near the tracked hand. This can improve stability when one hand occludes the other.")]
        private bool updateWhenOppositeHandNear = false;

        /// <summary>
        /// Should the solver continue to move when the opposite hand (hand which is not being tracked) is near the tracked hand. This can improve stability when one hand occludes the other."
        /// </summary>
        public bool UpdateWhenOppositeHandNear
        {
            get => updateWhenOppositeHandNear;
            set => updateWhenOppositeHandNear = value;
        }

        [SerializeField]
        [Tooltip("When a hand is activated for tracking, should the cursor(s) be disabled on that hand?")]
        private bool hideHandCursorsOnActivate = true;

        /// <summary>
        /// When a hand is activated for tracking, should the cursor(s) be disabled on that hand?
        /// </summary>
        public bool HideHandCursorsOnActivate
        {
            get => hideHandCursorsOnActivate;
            set => hideHandCursorsOnActivate = value;
        }

        [SerializeField]
        [Tooltip("Specifies how the solver should rotate when tracking the hand.")]
        private SolverRotationBehavior rotationBehavior = SolverRotationBehavior.LookAtMainCamera;

        /// <summary>
        /// Specifies how the solver should rotate when tracking the hand. 
        /// </summary>
        public SolverRotationBehavior RotationBehavior
        {
            get => rotationBehavior;
            set => rotationBehavior = value;
        }

        [SerializeField]
        [Tooltip("Specifies how the solver's offset relative to the hand will be computed.")]
        private SolverOffsetBehavior offsetBehavior = SolverOffsetBehavior.LookAtCameraRotation;

        /// <summary>
        /// Specifies how the solver's offset relative to the hand will be computed.
        /// </summary>
        public SolverOffsetBehavior OffsetBehavior
        {
            get => offsetBehavior;
            set => offsetBehavior = value;
        }

        [SerializeField]
        [Tooltip("Additional offset to apply towards the user.")]
        private float forwardOffset = 0;

        /// <summary>
        /// Additional offset to apply towards the user.
        /// </summary>
        public float ForwardOffset
        {
            get => forwardOffset;
            set => forwardOffset = value;
        }

        [SerializeField]
        [Tooltip("Additional degree offset to apply from the stated SafeZone. Ignored if Safe Zone is Atop Palm." +
        " Direction is clockwise on the left hand and anti-clockwise on the right hand.")]
        private float safeZoneAngleOffset = 0;

        /// <summary>
        /// Additional degree offset to apply clockwise from the stated SafeZone.
        /// Direction is clockwise on the left hand and anti-clockwise on the right hand.
        /// </summary>
        public float SafeZoneAngleOffset
        {
            get => safeZoneAngleOffset;
            set => safeZoneAngleOffset = value;
        }

        [SerializeField]
        [Tooltip("Event which is triggered when zero hands to one hand is tracked.")]
        private UnityEvent onFirstHandDetected = new UnityEvent();

        /// <summary>
        /// Event which is triggered when zero hands to one hand is tracked.
        /// </summary>
        public UnityEvent OnFirstHandDetected
        {
            get => onFirstHandDetected;
            set => onFirstHandDetected = value;
        }

        [SerializeField]
        [Tooltip("Event which is triggered when all hands are lost.")]
        private UnityEvent onLastHandLost = new UnityEvent();

        /// <summary>
        /// Event which is triggered when all hands are lost.
        /// </summary>
        public UnityEvent OnLastHandLost
        {
            get => onLastHandLost;
            set => onLastHandLost = value;
        }

        [SerializeField]
        [Tooltip("Event which is triggered when a hand begins being tracked.")]
        private UnityEvent onHandActivate = new UnityEvent();

        /// <summary>
        /// Event which is triggered when a hand begins being tracked.
        /// </summary>
        public UnityEvent OnHandActivate
        {
            get => onHandActivate;
            set => onHandActivate = value;
        }

        [SerializeField]
        [Tooltip("Event which is triggered when a hand stops being tracked.")]
        private UnityEvent onHandDeactivate = new UnityEvent();

        /// <summary>
        /// Event which is triggered when a hand stops being tracked.
        /// </summary>
        public UnityEvent OnHandDeactivate
        {
            get => onHandDeactivate;
            set => onHandDeactivate = value;
        }

        private Handedness previousHandedness = Handedness.None;

        public Handedness Handedness => previousHandedness;

        public XRNode? trackedNode = null;

        protected HandBounds handBounds = null;

        private readonly Quaternion handToWorldRotation = Quaternion.Euler(-90.0f, 0.0f, 180.0f);

        private static readonly ProfilerMarker SolverUpdatePerfMarker =
            new ProfilerMarker("[MRTK] HandConstraint.SolverUpdate");

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            using (SolverUpdatePerfMarker.Auto())
            {
                if (SolverHandler.TrackedTargetType != TrackedObjectType.HandJoint &&
                    SolverHandler.TrackedTargetType != TrackedObjectType.ControllerRay)
                {
                    // Solver HandConstraint requires TrackedObjectType of type HandJoint or Interactor (ControllerRay)
                    return;
                }

                XRNode? prevTrackedNode = trackedNode;

                if (SolverHandler.CurrentTrackedHandedness != Handedness.None)
                {
                    trackedNode = GetControllerNode(SolverHandler.CurrentTrackedHandedness);
                    bool isValidController = IsValidController(trackedNode);
                    if (!isValidController)
                    {
                        // Attempt to switch hands by asking solver handler to prefer the other controller if available
                        SolverHandler.PreferredTrackedHandedness = SolverHandler.CurrentTrackedHandedness.GetOppositeHandedness();
                        SolverHandler.RefreshTrackedObject();

                        trackedNode = GetControllerNode(SolverHandler.CurrentTrackedHandedness);
                        isValidController = IsValidController(trackedNode);
                        if (!isValidController)
                        {
                            trackedNode = null;
                        }
                    }

                    if (isValidController && SolverHandler.TransformTarget != null)
                    {
                        if (updateWhenOppositeHandNear || !IsOppositeHandNear(trackedNode))
                        {
                            GoalPosition = CalculateGoalPosition();
                            GoalRotation = CalculateGoalRotation();
                        }
                    }
                }
                else
                {
                    trackedNode = null;
                }

                // Calculate if events should be fired
                Handedness newHandedness = trackedNode.HasValue ? trackedNode.Value.ToHandedness() : Handedness.None;
                if (previousHandedness == Handedness.None && newHandedness != Handedness.None)
                {
                    previousHandedness = newHandedness;
                    OnFirstHandDetected.Invoke();
                    OnHandActivate.Invoke();
                }
                else if (previousHandedness != Handedness.None && newHandedness == Handedness.None)
                {
                    previousHandedness = newHandedness;
                    OnLastHandLost.Invoke();
                    OnHandDeactivate.Invoke();
                }
                else if (previousHandedness != newHandedness)
                {
                    OnHandDeactivate.Invoke();
                    previousHandedness = newHandedness;
                    OnHandActivate.Invoke();
                }

                UpdateWorkingPositionToGoal();
                UpdateWorkingRotationToGoal();
            }
        }

        /// <summary>
        /// Determines if a hand meets the requirements for use with constraining the tracked object.
        /// </summary>
        /// <param name="hand">The XRNode representing the hand to check against.</param>
        /// <returns>True if this hand should be used from tracking.</returns>
        protected virtual bool IsValidController(XRNode? hand)
        {
            return (hand.HasValue &&
                ((hand.Value == XRNode.LeftHand) || (hand.Value == XRNode.RightHand)));
        }

        private static readonly ProfilerMarker CalculateGoalPositionPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraint.CalculateGoalPosition");

        /// <summary>
        /// Performs a ray vs AABB test to determine where the solver can constrain the tracked object without intersection.
        /// The "safe zone" is calculated as if projected into the horizontal and vertical plane of the camera.
        /// </summary>
        /// <returns>The new goal position.</returns>
        protected virtual Vector3 CalculateGoalPosition()
        {
            using (CalculateGoalPositionPerfMarker.Auto())
            {
                Vector3 goalPosition = SolverHandler.TransformTarget.position;

                if (trackedNode.HasValue &&
                    handBounds.LocalBounds.TryGetValue(trackedNode.Value.ToHandedness(), out Bounds trackedHandBounds))
                {
                    HandJointPose? palmPose = GetPalmPose(trackedNode);

                    // If we somehow were unable to obtain a palm pose, we just quit;
                    // we require a valid palm pose to perform the hand-space transformations.
                    if (palmPose.HasValue == false)
                    {
                        return goalPosition;
                    }

                    Ray ray = CalculateGoalPositionRay(
                        goalPosition,
                        SolverHandler.TransformTarget,
                        trackedNode,
                        safeZone,
                        OffsetBehavior,
                        safeZoneAngleOffset);
                    trackedHandBounds.Expand(safeZoneBuffer);

                    // We need to transform the ray into hand-space before performing the AABB intersection.
                    ray.origin = Quaternion.Inverse(palmPose.Value.Rotation) * (ray.origin - palmPose.Value.Position);
                    ray.direction = Quaternion.Inverse(palmPose.Value.Rotation) * ray.direction;

                    if (trackedHandBounds.IntersectRay(ray, out float distance))
                    {
                        var localSpaceHit = ray.origin + ray.direction * distance;

                        // As hand bounds are computed and raycasted in palm-relative space,
                        // we must transform the hit target back into global space.
                        if (palmPose.HasValue)
                        {
                            goalPosition = palmPose.Value.Rotation * (localSpaceHit) + palmPose.Value.Position;
                            Vector3 goalToCam = Camera.main.transform.position - goalPosition;
                            if (goalToCam.magnitude > Mathf.Epsilon)
                            {
                                goalPosition += (goalToCam).normalized * ForwardOffset;
                            }
                        }
                    }
                }

                return goalPosition;
            }
        }

        private static readonly ProfilerMarker CalculateGoalRotationPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraint.CalculateGoalRotation");

        /// <summary>
        /// Determines the solver's goal rotation based off of the SolverRotationBehavior.
        /// </summary>
        /// <returns>The new goal rotation.</returns>
        protected virtual Quaternion CalculateGoalRotation()
        {
            using (CalculateGoalRotationPerfMarker.Auto())
            {
                Quaternion goalRotation = SolverHandler.TransformTarget.rotation;

                switch (rotationBehavior)
                {
                    case SolverRotationBehavior.LookAtMainCamera:
                        {
                            goalRotation = Quaternion.LookRotation(GoalPosition - Camera.main.transform.position);
                        }
                        break;

                    case SolverRotationBehavior.LookAtTrackedObject:
                        {
                            goalRotation *= handToWorldRotation;
                        }
                        break;
                }

                if (rotationBehavior != SolverRotationBehavior.None)
                {
                    var additionalRotation = SolverHandler.AdditionalRotation;

                    // Invert the yaw based on handedness to allow the rotation to look similar on both hands.
                    if (trackedNode.Value == XRNode.RightHand)
                    {
                        additionalRotation.y *= -1.0f;
                    }

                    goalRotation *= Quaternion.Euler(additionalRotation.x, additionalRotation.y, additionalRotation.z);
                }

                return goalRotation;
            }
        }

        private static readonly ProfilerMarker IsOppositeHandNearPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraint.IsOppositeHandNear");

        /// <summary>
        /// Performs an intersection test to see if the left hand is near
        /// the right hand or vice versa.
        /// </summary>
        /// <param name="hand">The hand to check against.</param>
        /// <returns>True, when hands are near each other.</returns>
        protected virtual bool IsOppositeHandNear(XRNode? hand)
        {
            using (IsOppositeHandNearPerfMarker.Auto())
            {
                if (IsValidController(hand))
                {
                    Handedness handedness = hand.Value.ToHandedness();

                    if (handBounds.GlobalBounds.TryGetValue(handedness.GetOppositeHandedness(), out Bounds oppositeHandBounds) &&
                        handBounds.GlobalBounds.TryGetValue(handedness, out Bounds trackedHandBounds))
                    {
                        // Double the size of the hand bounds to allow for greater tolerance.
                        trackedHandBounds.Expand(trackedHandBounds.extents);
                        oppositeHandBounds.Expand(oppositeHandBounds.extents);

                        if (trackedHandBounds.Intersects(oppositeHandBounds))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private static readonly ProfilerMarker CalculateRayForSafeZonePerfMarker =
            new ProfilerMarker("[MRTK] HandConstraint.CalculateRayForSafeZone");

        private Ray CalculateRayForSafeZone(
            Vector3 origin,
            Transform targetTransform,
            XRNode? hand,
            SolverSafeZone handSafeZone,
            SolverOffsetBehavior offsetBehavior,
            float angleOffset = 0)
        {
            using (CalculateRayForSafeZonePerfMarker.Auto())
            {
                Debug.Assert(hand.HasValue);

                Vector3 direction;
                Vector3 lookAtCamera = targetTransform.transform.position - Camera.main.transform.position;

                switch (handSafeZone)
                {
                    default:
                    case SolverSafeZone.UlnarSide:
                        {
                            if (offsetBehavior == SolverOffsetBehavior.TrackedObjectRotation)
                            {
                                direction = targetTransform.right;
                            }
                            else
                            {
                                direction = Vector3.Cross(lookAtCamera, Vector3.up);
                                direction = IsPalmFacingCamera(hand) ? direction : -direction;
                            }

                            if (hand == XRNode.LeftHand)
                            {
                                direction = -direction;
                            }
                        }
                        break;

                    case SolverSafeZone.RadialSide:
                        {

                            if (offsetBehavior == SolverOffsetBehavior.TrackedObjectRotation)
                            {
                                direction = -targetTransform.right;
                            }
                            else
                            {
                                direction = Vector3.Cross(lookAtCamera, Vector3.up);
                                direction = IsPalmFacingCamera(hand) ? direction : -direction;
                            }

                            if (hand == XRNode.RightHand)
                            {
                                direction = -direction;
                            }
                        }
                        break;

                    case SolverSafeZone.AboveFingerTips:
                        {
                            if (offsetBehavior == SolverOffsetBehavior.TrackedObjectRotation)
                            {
                                direction = targetTransform.forward;
                            }
                            else
                            {
                                direction = Camera.main.transform.up;
                            }
                        }
                        break;

                    case SolverSafeZone.BelowWrist:
                        {
                            if (offsetBehavior == SolverOffsetBehavior.TrackedObjectRotation)
                            {
                                direction = -targetTransform.forward;
                            }
                            else
                            {
                                direction = -Camera.main.transform.up;
                            }
                        }
                        break;

                    case SolverSafeZone.AtopPalm:
                        {
                            // This is always palm-pose dependent, as we are extruding
                            // the up vector away from the palm pose, regardless of the desired
                            // rotation behavior. If no palm pose is available, we use the
                            // camera view vector as an approximation.
                            HandJointPose? palmPose = GetPalmPose(hand);
                            if (palmPose.HasValue)
                            {
                                direction = Quaternion.AngleAxis((hand.Value == XRNode.LeftHand) ? angleOffset : -angleOffset, palmPose.Value.Forward) * -palmPose.Value.Up;
                            }
                            else
                            {
                                direction = -lookAtCamera;
                            }
                        }
                        break;
                }

                return new Ray(origin + direction, -direction);
            }
        }

        private static readonly ProfilerMarker CalculateGoalPositionRayPerfMarker =
            new ProfilerMarker("[MRTK] HandConstraint.CalculateGoalPositionRay");

        /// <summary>
        /// Compute a ray from the target's previous position to its desired position
        /// </summary>
        private Ray CalculateGoalPositionRay(
            Vector3 origin,
            Transform targetTransform,
            XRNode? hand,
            SolverSafeZone handSafeZone,
            SolverOffsetBehavior offsetBehavior,
            float angleOffset)
        {
            using (CalculateGoalPositionRayPerfMarker.Auto())
            {
                Debug.Assert(hand.HasValue);

                if (angleOffset == 0)
                {
                    return CalculateRayForSafeZone(
                        origin,
                        targetTransform,
                        hand,
                        handSafeZone,
                        offsetBehavior);
                }

                angleOffset %= 360;
                while (angleOffset < 0)
                {
                    angleOffset = (angleOffset + 360) % 360;
                }

                if (handSafeZone == SolverSafeZone.AtopPalm)
                {
                    return CalculateRayForSafeZone(
                        origin,
                        targetTransform,
                        hand,
                        handSafeZone,
                        offsetBehavior,
                        angleOffset);
                }

                float offset = angleOffset / 90;
                int intOffset = Mathf.FloorToInt(offset);
                float fracOffset = offset - intOffset;

                int currentSafeZoneClockwiseIdx = Array.IndexOf(handSafeZonesClockWiseRightHand, handSafeZone);

                SolverSafeZone intPartSafeZoneClockwise =
                    handSafeZonesClockWiseRightHand[(currentSafeZoneClockwiseIdx + intOffset) % handSafeZonesClockWiseRightHand.Length];

                SolverSafeZone fracPartSafeZoneClockwise =
                    handSafeZonesClockWiseRightHand[(currentSafeZoneClockwiseIdx + intOffset + 1) % handSafeZonesClockWiseRightHand.Length];

                Ray intSafeZoneRay = CalculateRayForSafeZone(
                    origin,
                    targetTransform,
                    hand,
                    intPartSafeZoneClockwise,
                    offsetBehavior);

                Ray fracPartSafeZoneRay = CalculateRayForSafeZone(
                    origin,
                    targetTransform,
                    hand,
                    fracPartSafeZoneClockwise,
                    offsetBehavior);

                Vector3 direction = Vector3.Lerp(
                    -intSafeZoneRay.direction,
                    -fracPartSafeZoneRay.direction,
                    fracOffset).normalized;

                return new Ray(origin + direction, -direction);
            }
        }

        /// <summary>
        /// Evaluates whether or not the palm of the specified hand is facing the
        /// camera.
        /// </summary>
        /// <param name="hand">The XRNode representing the hand to evaluate.</param>
        /// <returns>
        /// True of the palm is facing the camera, or false.
        /// </returns>
        private bool IsPalmFacingCamera(XRNode? hand)
        {
            Debug.Assert(hand.HasValue);

            HandJointPose? palmPose = GetPalmPose(hand);

            if (palmPose.HasValue)
            {
                return (Vector3.Dot(palmPose.Value.Up, Camera.main.transform.forward) > 0.0f);
            }

            return false;
        }

        private static readonly ProfilerMarker GetPalmPosePerfMarker =
            new ProfilerMarker("[MRTK] HandConstraint.GetPalmPose");

        /// <summary>
        /// Returns the pose of the palm of the specified hand.
        /// </summary>
        /// <param name="hand">The hand for which the palm pose is requested.</param>
        /// <returns>
        /// <see cref="HandJointPose"/> containing the pose of the palm, or null.
        /// </returns>
        private HandJointPose? GetPalmPose(XRNode? hand)
        {
            using (GetPalmPosePerfMarker.Auto())
            {
                Debug.Assert(hand.HasValue);
                HandJointPose? jointPose = null;

                if (XRSubsystemHelpers.HandsAggregator != null &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(
                    TrackedHandJoint.Palm,
                    hand.Value,
                    out HandJointPose pose))
                {
                    jointPose = pose;
                }

                return jointPose;
            }
        }

        /// <summary>
        /// Returns the XRNode that matches the desired handedness.
        /// </summary>
        /// <param name="handedness">The handedness of the returned controller</param>
        /// <returns>The IMixedRealityController for the desired handedness, or null if none are present.</returns>
        protected XRNode? GetControllerNode(Handedness handedness)
        {
            if (!SolverHandler.IsValidHandedness(handedness)) { return null; }
            return (handedness == Handedness.Left) ? XRNode.LeftHand : XRNode.RightHand;
        }

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();

            handBounds = GetComponent<HandBounds>();

            // Initially no hands are tacked or active.
            trackedNode = null;
            OnHandDeactivate.Invoke();

            if (SolverHandler.TrackedTargetType != TrackedObjectType.HandJoint &&
                SolverHandler.TrackedTargetType != TrackedObjectType.Interactor)
            {
                Debug.LogWarning("Solver HandConstraint requires TrackedObjectType of type HandJoint or Interactor.");
            }
        }

        #endregion MonoBehaviour Implementation

        #region Enums

        /// <summary>
        /// Specifies how the solver's offset relative to the hand / controller will be computed.
        /// </summary>
        public enum SolverOffsetBehavior
        {
            /// <summary>
            /// Uses the object-to-head vector to compute an offset independent
            /// of hand / controller rotation.
            /// </summary>
            LookAtCameraRotation,

            /// <summary>
            /// Uses the hand / controller rotation to compute an offset independent
            /// of look at camera rotation.
            /// </summary>
            TrackedObjectRotation
        }

        /// <summary>
        /// Specifies how the solver should rotate when tracking a hand or motion controller. 
        /// </summary>
        public enum SolverRotationBehavior
        {
            /// <summary>
            /// The solver simply follows the rotation of the tracked object. 
            /// </summary>
            None = 0,

            /// <summary>
            /// The solver faces the main camera (user).
            /// </summary>
            LookAtMainCamera = 2,

            /// <summary>
            /// The solver faces the tracked object. A controller to world transformation is
            /// applied to work with traditional user facing UI (-z is forward).
            /// </summary>
            LookAtTrackedObject = 3
        }

        /// <summary>
        /// Specifies a zone that is safe for the constraint to solve to without intersecting the hand.
        /// Safe zones may differ slightly from motion controller to motion controller, it's recommended to
        /// pick the safe zone best suited for your intended controller and application.
        /// </summary>
        public enum SolverSafeZone
        {
            /// <summary>
            /// On the left controller with palm up, the area right of the palm.
            /// </summary>
            UlnarSide = 0,

            /// <summary>
            /// On the left controller with palm up, the area left of the palm.
            /// </summary>
            RadialSide = 1,

            /// <summary>
            /// Above the longest finger tips.
            /// </summary>
            AboveFingerTips = 2,

            /// <summary>
            /// Below where the controller meets the arm.
            /// </summary>
            BelowWrist = 3,

            /// <summary>
            /// Floating above the palm, towards the "inside" of the hand (opposite side of knuckles),
            /// based on the "down" vector of the palm joint (i.e., the grabbing-side of the hand)
            /// </summary>
            AtopPalm = 4
        }

        #endregion Enums
    }
}
