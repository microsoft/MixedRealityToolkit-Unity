// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that constrains the target to a region safe for hand constrained interactive content.
    /// This solver is intended to work with <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand"/> but also works with <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityController"/>. 
    /// </summary>
    [RequireComponent(typeof(HandBounds))]
    public class HandConstraint : Solver, IMixedRealitySourceStateHandler
    {
        /// <summary>
        /// Specifies a zone that is safe for the constraint to solve to without intersecting the hand.
        /// Safe zones may differ slightly from motion controller to motion controller, it's recommended to
        /// pick the safe zone best suited for your intended controller and application.
        /// </summary>
        public enum SolverSafeZone
        {
            /// <summary>
            /// On the left hand with palm up, the area right of the palm.
            /// </summary>
            UlnarSide = 0,
            /// <summary>
            /// On the left hand with palm up, the area left of the palm.
            /// </summary>
            RadialSide = 1,
            /// <summary>
            /// Above the longest finger tips.
            /// </summary>
            AboveFingerTips = 2,
            /// <summary>
            /// Below where the hand meets the arm.
            /// </summary>
            BelowWrist = 3
        }

        [Header("Hand Constraint")]
        [SerializeField]
        [Tooltip("Which part of the hand to move the solver towards. The ulnar side of the hand is recommended for most situations.")]
        private SolverSafeZone safeZone = SolverSafeZone.UlnarSide;

        /// <summary>
        /// Which part of the hand to move the tracked object towards. The ulnar side of the hand is recommended for most situations.
        /// </summary>
        public SolverSafeZone SafeZone
        {
            get { return safeZone; }
            set { safeZone = value; }
        }

        [SerializeField]
        [Tooltip("Additional offset to apply to the intersection point with the hand bounds along the intersection point normal.")]
        private float safeZoneBuffer = 0.15f;

        /// <summary>
        /// Additional offset to apply to the intersection point with the hand bounds.
        /// </summary>
        public float SafeZoneBuffer
        {
            get { return safeZoneBuffer; }
            set { safeZoneBuffer = value; }
        }

        [SerializeField]
        [Tooltip("Should the solver continue to move when the opposite hand (hand which is not being tracked) is near the tracked hand. This can improve stability when one hand occludes the other.")]
        private bool updateWhenOppositeHandNear = false;

        /// <summary>
        /// Should the solver continue to move when the opposite hand (hand which is not being tracked) is near the tracked hand. This can improve stability when one hand occludes the other."
        /// </summary>
        public bool UpdateWhenOppositeHandNear
        {
            get { return updateWhenOppositeHandNear; }
            set { updateWhenOppositeHandNear = value; }
        }

        [SerializeField]
        [Tooltip("When a hand is activated for tracking, should the cursor(s) be disabled on that hand?")]
        private bool hideHandCursorsOnActivate = true;

        /// <summary>
        /// When a hand is activated for tracking, should the cursor(s) be disabled on that hand?
        /// </summary>
        public bool HideHandCursorsOnActivate
        {
            get { return hideHandCursorsOnActivate; }
            set { hideHandCursorsOnActivate = value; }
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
        [Tooltip("Specifies how the solver should rotate when tracking the hand. ")]
        private SolverRotationBehavior rotationBehavior = SolverRotationBehavior.LookAtMainCamera;

        /// <summary>
        /// Specifies how the solver should rotate when tracking the hand. 
        /// </summary>
        public SolverRotationBehavior RotationBehavior
        {
            get { return rotationBehavior; }
            set { rotationBehavior = value; }
        }

        [SerializeField]
        [Tooltip("Event which is triggered when a hand begins being tracked.")]
        private UnityEvent onHandActivate = null;

        /// <summary>
        /// Event which is triggered when a hand begins being tracked.
        /// </summary>
        public UnityEvent OnHandActivate
        {
            get { return onHandActivate; }
            set { onHandActivate = value; }
        }

        [SerializeField]
        [Tooltip("Event which is triggered when a hand stops being tracked.")]
        private UnityEvent onHandDeactivate = null;

        /// <summary>
        /// Event which is triggered when a hand stops being tracked.
        /// </summary>
        public UnityEvent OnHandDeactivate
        {
            get { return onHandDeactivate; }
            set { onHandDeactivate = value; }
        }

        [SerializeField]
        [Tooltip("Event which is triggered when zero hands to one hand is tracked.")]
        private UnityEvent onFirstHandDetected = null;

        /// <summary>
        /// Event which is triggered when zero hands to one hand is tracked.
        /// </summary>
        public UnityEvent OnFirstHandDetected
        {
            get { return onFirstHandDetected; }
            set { onFirstHandDetected = value; }
        }

        [SerializeField]
        [Tooltip("Event which is triggered when all hands are lost.")]
        private UnityEvent onLastHandLost = null;

        /// <summary>
        /// Event which is triggered when all hands are lost.
        /// </summary>
        public UnityEvent OnLastHandLost
        {
            get { return onLastHandLost; }
            set { onLastHandLost = value; }
        }

        protected IMixedRealityController trackedHand = null;
        protected List<IMixedRealityController> handStack = new List<IMixedRealityController>();
        protected HandBounds handBounds = null;
        protected bool autoTransitionBetweenHands = false;

        private IMixedRealityInputSystem inputSystem = null;
        private readonly Quaternion handToWorldRotation = Quaternion.Euler(-90.0f, 0.0f, 180.0f);

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService(out inputSystem);
                }

                return inputSystem;
            }
        }

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            // Determine the new active hand.
            IMixedRealityController newActivehand = null;

            foreach (var hand in handStack)
            {
                if (IsHandActive(hand))
                {
                    newActivehand = hand;
                    break;
                }
            }

            // Track the new active hand.
            if (trackedHand == null || trackedHand != newActivehand)
            {
                ChangeTrackedObjectType(newActivehand);
            }

            // Update the goal position and rotation if a tracked hand is present.
            if (trackedHand != null && SolverHandler.TransformTarget != null)
            {
                if (updateWhenOppositeHandNear || !IsOppositeHandNear(trackedHand))
                {
                    GoalPosition = CalculateGoalPosition();
                    GoalRotation = CalculateGoalRotation();
                }
            }


            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        /// <summary>
        /// Determines if a hand meets the requirements for use with constraining the tracked object.
        /// </summary>
        /// <param name="hand">The hand to check against.</param>
        /// <returns>True if this hand should be used from tracking.</returns>
        protected virtual bool IsHandActive(IMixedRealityController hand)
        {
            // If transitioning between hands is not allowed, make sure the TrackedObjectType matches the hand.
            if (!autoTransitionBetweenHands)
            {
                return (SolverHandler.TrackedTargetType == TrackedObjectType.HandJoint || 
                        SolverHandler.TrackedTargetType == TrackedObjectType.MotionController)  &&
                       (SolverHandler.TrackedHandness == Handedness.Both || 
                        hand.ControllerHandedness == SolverHandler.TrackedHandness);
            }

            // Check to make sure none of the hand's pointer's a locked. We don't want to track a hand which is currently
            // interacting with something else.
            foreach (var pointer in hand.InputSource.Pointers)
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

            if (trackedHand != null &&
                handBounds.Bounds.TryGetValue(trackedHand.ControllerHandedness, out trackedHandBounds))
            {
                float distance;
                Ray ray = CalculateProjectedSafeZoneRay(goalPosition, trackedHand, safeZone);
                trackedHandBounds.Expand(safeZoneBuffer);

                if (trackedHandBounds.IntersectRay(ray, out distance))
                {
                    goalPosition = ray.origin + ray.direction * distance;
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
                switch (trackedHand.ControllerHandedness)
                {
                    default:
                    case Handedness.Left:
                        {
                            goalRotation *= Quaternion.Euler(additionalRotation.x, additionalRotation.y, additionalRotation.z);
                        }
                        break;

                    case Handedness.Right:
                        {
                            goalRotation *= Quaternion.Euler(additionalRotation.x, -additionalRotation.y, additionalRotation.z);
                        }
                        break;
                }
            }

            return goalRotation;
        }

        /// <summary>
        /// Enables/disables all cursors on the currently tracked hand.
        /// </summary>
        /// <param name="visible">Is the cursor visible?</param>
        /// <param name="frameDelay">Delay one frame before performing the toggle to allow the pointers to instantiate their cursors.</param>
        protected virtual IEnumerator ToggleCursor(bool visible, bool frameDelay = false)
        {
            if (hideHandCursorsOnActivate)
            {
                var cachedTrackedHand = trackedHand;

                if (frameDelay)
                {
                    yield return null;
                }

                foreach (var pointer in cachedTrackedHand?.InputSource.Pointers)
                {
                    pointer?.BaseCursor?.SetVisibility(visible);
                }
            }
        }

        /// <summary>
        /// Performs an intersection test to see if the left hand is near the right hand or vice versa.
        /// </summary>
        /// <param name="hand">The hand to check against.</param>
        /// <returns>True, when hands are near each other.</returns>
        protected virtual bool IsOppositeHandNear(IMixedRealityController hand)
        {
            if (hand != null)
            {
                Bounds oppositeHandBounds, trackedHandBounds;

                if (handBounds.Bounds.TryGetValue((hand.ControllerHandedness == Handedness.Left) ? Handedness.Right : Handedness.Left, out oppositeHandBounds) &&
                    handBounds.Bounds.TryGetValue(hand.ControllerHandedness, out trackedHandBounds))
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

        /// <summary>
        /// Swaps out the currently tracked hand while triggered appropriate events.
        /// </summary>
        /// <param name="hand">Which hand to track now.</param>
        private void ChangeTrackedObjectType(IMixedRealityController hand)
        {
            if (hand != null)
            {
                if (SolverHandler.TrackedTargetType == TrackedObjectType.HandJoint || 
                    SolverHandler.TrackedTargetType == TrackedObjectType.MotionController)
                {
                    if (SolverHandler.TrackedHandness != hand.ControllerHandedness)
                    {
                        SolverHandler.TrackedHandness = hand.ControllerHandedness;

                        // Move the currently tracked hand to the top of the stack.
                        handStack.Remove(hand);
                        handStack.Insert(0, hand);
                    }

                    if (trackedHand == null)
                    {
                        trackedHand = hand;
                        onHandActivate?.Invoke();
                    }
                    else
                    {
                        StartCoroutine(ToggleCursor(true));
                        trackedHand = hand;
                    }

                    // Wait one frame to disable the cursor in case one hasn't been instantiated yet.
                    StartCoroutine(ToggleCursor(false, true));
                }
                else
                {
                    Debug.LogWarning("ChangeTrackedObjectType failed because TrackedTargetType is not of type HandJoint or MotionController.");
                }
            }
            else
            {
                if (trackedHand != null)
                {
                    StartCoroutine(ToggleCursor(true));
                    trackedHand = null;
                    onHandDeactivate?.Invoke();
                }
            }
        }

        private static Ray CalculateProjectedSafeZoneRay(Vector3 origin, IMixedRealityController hand, SolverSafeZone handSafeZone)
        {
            Vector3 direction;

            switch (handSafeZone)
            {
                default:
                case SolverSafeZone.UlnarSide:
                    {
                        direction = Vector3.Cross(CameraCache.Main.transform.forward, Vector3.up);
                        direction = IsPalmFacingCamera(hand) ? direction : -direction;

                        if (hand.ControllerHandedness == Handedness.Left)
                        {
                            direction = -direction;
                        }
                    }
                    break;

                case SolverSafeZone.RadialSide:
                    {
                        direction = Vector3.Cross(CameraCache.Main.transform.forward, Vector3.up);
                        direction = IsPalmFacingCamera(hand) ? direction : -direction;

                        if (hand.ControllerHandedness == Handedness.Right)
                        {
                            direction = -direction;
                        }
                    }
                    break;

                case SolverSafeZone.AboveFingerTips:
                    {
                        direction = CameraCache.Main.transform.up;
                    }
                    break;

                case SolverSafeZone.BelowWrist:
                    {
                        direction = -CameraCache.Main.transform.up;
                    }
                    break;
            }

            return new Ray(origin + direction, -direction);
        }

        private static bool IsPalmFacingCamera(IMixedRealityController hand)
        {
            MixedRealityPose palmPose;
            var jointedHand = hand as IMixedRealityHand;

            if ((jointedHand != null) && jointedHand.TryGetJoint(TrackedHandJoint.Palm, out palmPose))
            {
                return (Vector3.Dot(palmPose.Up, CameraCache.Main.transform.forward) > 0.0f);
            }

            return false;
        }

        /// <summary>
        /// Returns true if the given controller is a valid target for this solver.
        /// </summary>
        /// <remarks>
        /// Certain types of controllers (i.e. Xbox controllers) do not contain a handedness
        /// and should not trigger the HandConstraint to show its corresponding UX.
        /// </remarks>
        private static bool IsApplicableController(IMixedRealityController controller)
        {
            return controller.ControllerHandedness != Handedness.None;
        }

        #region MonoBehaviour Implementation

        protected override void Awake()
        {
            base.Awake();

            // Auto transition between hands if the solver is set to track either hand.
            autoTransitionBetweenHands = SolverHandler.TrackedHandness == Handedness.Both;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);

            handBounds = GetComponent<HandBounds>();

            // Initially no hands are tacked or active.
            trackedHand = null;
            onLastHandLost?.Invoke();
            onHandDeactivate?.Invoke();
        }

        protected virtual void OnDisable()
        {
            InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            var hand = eventData.Controller;

            if (hand != null && IsApplicableController(hand) && !handStack.Contains(hand))
            {
                if (handStack.Count == 0)
                {
                    onFirstHandDetected?.Invoke();
                }

                handStack.Add(hand);
            }
        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            var hand = eventData.Controller;

            if (hand != null && IsApplicableController(hand))
            {
                handStack.Remove(hand);

                if (handStack.Count == 0)
                {
                    onLastHandLost?.Invoke();
                }
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation
    }
}
