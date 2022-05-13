// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that constrains the target to a region safe for hand constrained interactive content.
    /// This solver is intended to work with <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> but also works with <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityController"/>. 
    /// </summary>
    [RequireComponent(typeof(HandBounds))]
    [AddComponentMenu("Scripts/MRTK/SDK/HandConstraint")]
    public class HandConstraint : Solver
    {
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

        // Subtract one here because AtopPalm is not along the palm, in clockwise direction
        private static readonly SolverSafeZone[] handSafeZonesClockWiseRightHand = new SolverSafeZone[] {
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

        /// <summary>
        /// Specifies how the solver should rotate when tracking the hand. 
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
            /// The solver faces the tracked object. A hand to world transformation is applied to work with 
            /// traditional user facing UI (-z is forward).
            /// </summary>
            LookAtTrackedObject = 3
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

        /// <summary>
        /// Specifies how the solver's offset relative to the hand will be computed.
        /// </summary>
        public enum SolverOffsetBehavior
        {
            /// <summary>
            /// Uses the object-to-head vector to compute an offset independent of hand rotation.
            /// </summary>
            LookAtCameraRotation,
            /// <summary>
            /// Uses the hand rotation to compute an offset independent of look at camera rotation.
            /// </summary>
            TrackedObjectRotation
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

        private Handedness previousHandedness = Handedness.None;

        public Handedness Handedness => previousHandedness;

        protected IMixedRealityController trackedController = null;
        protected HandBounds handBounds = null;

        private readonly Quaternion handToWorldRotation = Quaternion.Euler(-90.0f, 0.0f, 180.0f);

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            if (SolverHandler.TrackedTargetType != TrackedObjectType.HandJoint &&
                SolverHandler.TrackedTargetType != TrackedObjectType.ControllerRay)
            {
                // Requires HandJoint or ControllerRay target type.
                return;
            }

            var prevTrackedController = trackedController;

            if (SolverHandler.CurrentTrackedHandedness != Handedness.None)
            {
                trackedController = GetController(SolverHandler.CurrentTrackedHandedness);
                bool isValidController = IsValidController(trackedController);
                if (!isValidController)
                {
                    // Attempt to switch hands by asking solver handler to prefer the other controller if available
                    SolverHandler.PreferredTrackedHandedness = SolverHandler.CurrentTrackedHandedness.GetOppositeHandedness();
                    SolverHandler.RefreshTrackedObject();

                    trackedController = GetController(SolverHandler.CurrentTrackedHandedness);
                    isValidController = IsValidController(trackedController);
                    if (!isValidController)
                    {
                        trackedController = null;
                    }
                }

                if (isValidController && SolverHandler.TransformTarget != null)
                {
                    if (updateWhenOppositeHandNear || !IsOppositeHandNear(trackedController))
                    {
                        GoalPosition = CalculateGoalPosition();
                        GoalRotation = CalculateGoalRotation();
                    }
                }
            }
            else
            {
                trackedController = null;
            }

            // Calculate if events should be fired
            var newHandedness = trackedController == null ? Handedness.None : trackedController.ControllerHandedness;
            if (previousHandedness.IsNone() && !newHandedness.IsNone())
            {
                // Toggle cursor off for hand that is going to support the hand menu
                StartCoroutine(ToggleCursors(trackedController, false, true));

                previousHandedness = newHandedness;
                OnFirstHandDetected.Invoke();
                OnHandActivate.Invoke();
            }
            else if (!previousHandedness.IsNone() && newHandedness.IsNone())
            {
                // Toggle cursors back on for the hand that is no longer supporting the solver
                StartCoroutine(ToggleCursors(prevTrackedController, true));

                previousHandedness = newHandedness;
                // toggle cursor on for trackedHand
                OnLastHandLost.Invoke();
                OnHandDeactivate.Invoke();
            }
            else if (previousHandedness != newHandedness)
            {
                // Switching controllers. Toggle cursors back on for the previous controller and toggle off for the new supported controller
                StartCoroutine(ToggleCursors(prevTrackedController, true));
                StartCoroutine(ToggleCursors(trackedController, false, true));

                OnHandDeactivate.Invoke();
                previousHandedness = newHandedness;
                OnHandActivate.Invoke();
            }

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        /// <summary>
        /// Determines if a hand meets the requirements for use with constraining the tracked object.
        /// </summary>
        /// <param name="controller">The controller to check against.</param>
        /// <returns>True if this hand should be used from tracking.</returns>
        protected virtual bool IsValidController(IMixedRealityController controller)
        {
            if (controller == null)
            {
                return false;
            }

            // Check to make sure none of the hand's pointer's a locked. We don't want to track a hand which is currently
            // interacting with something else.
            foreach (var pointer in controller.InputSource.Pointers)
            {
                if (pointer.IsFocusLocked)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs a ray vs AABB test to determine where the solver can constrain the tracked object without intersection.
        /// The "safe zone" is calculated as if projected into the horizontal and vertical plane of the camera.
        /// </summary>
        /// <returns>The new goal position.</returns>
        protected virtual Vector3 CalculateGoalPosition()
        {
            var goalPosition = SolverHandler.TransformTarget.position;
            Bounds trackedHandBounds;

            if (trackedController != null &&
                handBounds.LocalBounds.TryGetValue(trackedController.ControllerHandedness, out trackedHandBounds))
            {
                MixedRealityPose? palmPose = GetPalmPose(trackedController);

                // If we somehow were unable to obtain a palm pose, we just quit;
                // we require a valid palm pose to perform the hand-space transformations.
                if (palmPose.HasValue == false)
                {
                    return goalPosition;
                }

                float distance;
                Ray ray = CalculateGoalPositionRay(
                    goalPosition, SolverHandler.TransformTarget,
                    trackedController, safeZone, OffsetBehavior, safeZoneAngleOffset);
                trackedHandBounds.Expand(safeZoneBuffer);

                // We need to transform the ray into hand-space before performing the AABB intersection.
                ray.origin = Quaternion.Inverse(palmPose.Value.Rotation) * (ray.origin - palmPose.Value.Position);
                ray.direction = Quaternion.Inverse(palmPose.Value.Rotation) * ray.direction;

                if (trackedHandBounds.IntersectRay(ray, out distance))
                {
                    var localSpaceHit = ray.origin + ray.direction * distance;

                    // As hand bounds are computed and raycasted in palm-relative space,
                    // we must transform the hit target back into global space.
                    if (palmPose.HasValue)
                    {
                        goalPosition = palmPose.Value.Rotation * (localSpaceHit) + palmPose.Value.Position;
                        Vector3 goalToCam = CameraCache.Main.transform.position - goalPosition;
                        if (goalToCam.magnitude > Mathf.Epsilon)
                        {
                            goalPosition += (goalToCam).normalized * ForwardOffset;
                        }
                    }
                }
            }

            return goalPosition;
        }

        /// <summary>
        /// Determines the solver's goal rotation based off of the SolverRotationBehavior.
        /// </summary>
        /// <returns>The new goal rotation.</returns>
        protected virtual Quaternion CalculateGoalRotation()
        {
            var goalRotation = SolverHandler.TransformTarget.rotation;

            switch (rotationBehavior)
            {
                case SolverRotationBehavior.LookAtMainCamera:
                {
                    goalRotation = Quaternion.LookRotation(GoalPosition - CameraCache.Main.transform.position);
                    break;
                }

                case SolverRotationBehavior.LookAtTrackedObject:
                {
                    goalRotation *= handToWorldRotation;
                    break;
                }
            }

            if (rotationBehavior != SolverRotationBehavior.None)
            {
                var additionalRotation = SolverHandler.AdditionalRotation;

                // Invert the yaw based on handedness to allow the rotation to look similar on both hands.
                if (trackedController.ControllerHandedness.IsRight())
                {
                    additionalRotation.y *= -1.0f;
                }

                goalRotation *= Quaternion.Euler(additionalRotation.x, additionalRotation.y, additionalRotation.z);
            }
            return goalRotation;
        }

        /// <summary>
        /// Enables/disables all cursors on the currently tracked hand.
        /// </summary>
        /// <param name="controller">Controller target to search for pointers</param>
        /// <param name="visible">Is the cursor visible?</param>
        /// <param name="frameDelay">Delay one frame before performing the toggle to allow the pointers to instantiate their cursors.</param>
        protected virtual IEnumerator ToggleCursors(IMixedRealityController controller, bool visible, bool frameDelay = false)
        {
            if (controller == null)
            {
                yield break;
            }

            if (hideHandCursorsOnActivate)
            {
                if (frameDelay)
                {
                    yield return null;
                }

                foreach (var pointer in controller?.InputSource.Pointers)
                {
                    pointer?.BaseCursor?.SetVisibility(visible);
                }
            }
        }

        /// <summary>
        /// Performs an intersection test to see if the left hand is near the right hand or vice versa.
        /// </summary>
        /// <param name="controller">The hand to check against.</param>
        /// <returns>True, when hands are near each other.</returns>
        protected virtual bool IsOppositeHandNear(IMixedRealityController controller)
        {
            if (controller != null)
            {
                if (handBounds.Bounds.TryGetValue(controller.ControllerHandedness.GetOppositeHandedness(), out Bounds oppositeHandBounds) &&
                    handBounds.Bounds.TryGetValue(controller.ControllerHandedness, out Bounds trackedHandBounds))
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

        private static Ray CalculateRayForSafeZone(
            Vector3 origin, Transform targetTransform, IMixedRealityController hand, SolverSafeZone handSafeZone, SolverOffsetBehavior offsetBehavior, float angleOffset = 0)
        {
            Vector3 direction;
            Vector3 lookAtCamera = targetTransform.transform.position - CameraCache.Main.transform.position;

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

                    if (hand.ControllerHandedness.IsLeft())
                    {
                        direction = -direction;
                    }
                    break;
                }

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

                    if (hand.ControllerHandedness.IsRight())
                    {
                        direction = -direction;
                    }
                    break;
                }

                case SolverSafeZone.AboveFingerTips:
                {
                    if (offsetBehavior == SolverOffsetBehavior.TrackedObjectRotation)
                    {
                        direction = targetTransform.forward;
                    }
                    else
                    {
                        direction = CameraCache.Main.transform.up;
                    }
                    break;
                }

                case SolverSafeZone.BelowWrist:
                {
                    if (offsetBehavior == SolverOffsetBehavior.TrackedObjectRotation)
                    {
                        direction = -targetTransform.forward;
                    }
                    else
                    {
                        direction = -CameraCache.Main.transform.up;
                    }
                    break;
                }

                case SolverSafeZone.AtopPalm:
                {
                    // This is always palm-pose dependent, as we are extruding
                    // the up vector away from the palm pose, regardless of the desired
                    // rotation behavior. If no palm pose is available, we use the
                    // camera view vector as an approximation.
                    MixedRealityPose? palmPose = GetPalmPose(hand);
                    if (palmPose.HasValue)
                    {
                        direction = Quaternion.AngleAxis(hand.ControllerHandedness.IsLeft() ? angleOffset : -angleOffset, palmPose.Value.Forward) * -palmPose.Value.Up;
                    }
                    else
                    {
                        direction = -lookAtCamera;
                    }
                    break;
                }
            }

            return new Ray(origin + direction, -direction);
        }

        /// <summary>
        /// Compute a ray from the target's previous position to its desired position
        /// </summary>
        private static Ray CalculateGoalPositionRay(Vector3 origin, Transform targetTransform, IMixedRealityController hand,
            SolverSafeZone handSafeZone, SolverOffsetBehavior offsetBehavior, float angleOffset)
        {
            if (angleOffset == 0)
            {
                return CalculateRayForSafeZone(origin, targetTransform, hand, handSafeZone, offsetBehavior);
            }

            angleOffset %= 360;
            while (angleOffset < 0)
            {
                angleOffset = (angleOffset + 360) % 360;
            }

            if (handSafeZone == SolverSafeZone.AtopPalm)
            {
                return CalculateRayForSafeZone(origin, targetTransform, hand, handSafeZone, offsetBehavior, angleOffset);
            }

            float offset = angleOffset / 90;
            int intOffset = Mathf.FloorToInt(offset);
            float fracOffset = offset - intOffset;

            int currentSafeZoneClockwiseIdx = System.Array.IndexOf(handSafeZonesClockWiseRightHand, handSafeZone);

            SolverSafeZone intPartSafeZoneClockwise = handSafeZonesClockWiseRightHand[(currentSafeZoneClockwiseIdx + intOffset) % handSafeZonesClockWiseRightHand.Length];
            SolverSafeZone fracPartSafeZoneClockwise = handSafeZonesClockWiseRightHand[(currentSafeZoneClockwiseIdx + intOffset + 1) % handSafeZonesClockWiseRightHand.Length];

            Ray intSafeZoneRay = CalculateRayForSafeZone(origin, targetTransform, hand, intPartSafeZoneClockwise, offsetBehavior);
            Ray fracPartSafeZoneRay = CalculateRayForSafeZone(origin, targetTransform, hand, fracPartSafeZoneClockwise, offsetBehavior);

            Vector3 direction = Vector3.Lerp(-intSafeZoneRay.direction, -fracPartSafeZoneRay.direction, fracOffset).normalized;
            return new Ray(origin + direction, -direction);
        }

        private static bool IsPalmFacingCamera(IMixedRealityController hand)
        {
            MixedRealityPose? palmPose = GetPalmPose(hand);

            if (palmPose.HasValue)
            {
                return (Vector3.Dot(palmPose.Value.Up, CameraCache.Main.transform.forward) > 0.0f);
            }

            return false;
        }

        private static MixedRealityPose? GetPalmPose(IMixedRealityController hand)
        {
            MixedRealityPose palmPose;

            if (hand is IMixedRealityHand jointedHand && jointedHand.TryGetJoint(TrackedHandJoint.Palm, out palmPose))
            {
                return palmPose;
            }

            return null;
        }

        /// <summary>
        /// Returns true if the given controller is a valid target for this solver.
        /// </summary>
        /// <remarks>
        /// <para>Certain types of controllers (i.e. Xbox controllers) do not contain a handedness
        /// and should not trigger the HandConstraint to show its corresponding UX.</para>
        /// </remarks>
        private static bool IsApplicableController(IMixedRealityController controller)
        {
            return !controller.ControllerHandedness.IsNone();
        }

        /// <summary>
        /// Returns the first detected controller in the input system that matches the passed-in handedness
        /// </summary>
        /// <param name="handedness">The handedness of the returned controller</param>
        /// <returns>The IMixedRealityController for the desired handedness, or null if none are present.</returns>
        protected static IMixedRealityController GetController(Handedness handedness)
        {
            foreach (IMixedRealityController c in CoreServices.InputSystem.DetectedControllers)
            {
                if (c.ControllerHandedness.IsMatch(handedness))
                {
                    return c;
                }
            }

            return null;
        }

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();

            handBounds = GetComponent<HandBounds>();

            // Initially no hands are tacked or active.
            trackedController = null;
            OnLastHandLost.Invoke();
            OnHandDeactivate.Invoke();

            if (SolverHandler.TrackedTargetType != TrackedObjectType.HandJoint &&
                SolverHandler.TrackedTargetType != TrackedObjectType.ControllerRay)
            {
                Debug.LogWarning("Solver HandConstraint requires TrackedObjectType of type HandJoint or ControllerRay.");
            }
        }

        #endregion MonoBehaviour Implementation
    }
}
