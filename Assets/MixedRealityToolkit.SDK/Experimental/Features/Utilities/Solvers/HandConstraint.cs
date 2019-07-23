// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that constrains the target to a region safe for hand constrained content.
    /// </summary>
    [RequireComponent(typeof(HandBounds))]
    public class HandConstraint : Solver, IMixedRealitySourceStateHandler
    {
        /// <summary>
        /// Specifies a zone that is safe for the constraint to solve to without intersecting the hand.
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

        [Experimental]
        [Header("Hand Constraint")]
        [SerializeField]
        [Tooltip("Which part of the hand to move the solver towards.")]
        private SolverSafeZone safeZone = SolverSafeZone.UlnarSide;

        /// <summary>
        /// Which part of the hand to move the tracked object towards.
        /// </summary>
        public SolverSafeZone SafeZone
        {
            get { return safeZone; }
            set { safeZone = value; }
        }

        [SerializeField]
        [Tooltip("Additional offset to apply to the intersection point with the hand bounds along the intersection point normal.")]
        private float safeZoneBuffer = 0.1f;

        /// <summary>
        /// Additional offset to apply to the intersection point with the hand bounds.
        /// </summary>
        public float SafeZoneBuffer
        {
            get { return safeZoneBuffer; }
            set { safeZoneBuffer = value; }
        }

        [SerializeField]
        [Tooltip("Should the solver automatically switch to tracking the primary hand? The primary hand is the hand in view the longest or last active.")]
        private bool transitionBetweenHands = true;

        /// <summary>
        /// Should the solver automatically switch to tracking the primary hand? The primary hand is the hand in view the longest or last active.
        /// </summary>
        public bool TransitionBetweenHands
        {
            get { return transitionBetweenHands; }
            set { transitionBetweenHands = value; }
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
        /// TODO
        /// </summary>
        public enum SolverRotationBehavior
        {
            /// <summary>
            /// TODO
            /// </summary>
            None = 0,
            /// <summary>
            /// TODO
            /// </summary>
            LookAtMainCamera = 2,
            /// <summary>
            /// TODO
            /// </summary>
            LookAtTrackedObject = 3
        }

        [SerializeField]
        [Tooltip("TODO")]
        private SolverRotationBehavior rotationBehavior = SolverRotationBehavior.LookAtMainCamera;

        /// <summary>
        /// TODO
        /// </summary>
        public SolverRotationBehavior RotationBehavior
        {
            get { return rotationBehavior; }
            set { rotationBehavior = value; }
        }

        [SerializeField]
        [Tooltip("TODO")]
        private float handednessYawRotation = 20.0f;

        /// <summary>
        /// TODO
        /// </summary>
        public float HandednessYawRotation
        {
            get { return handednessYawRotation; }
            set
            {
                handednessYawRotation = value;
                yawRotation = Quaternion.Euler(0.0f, handednessYawRotation, 0.0f);
            }
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

        protected IMixedRealityHand trackedHand = null;
        protected List<IMixedRealityHand> handStack = new List<IMixedRealityHand>();
        protected HandBounds handBounds = null;

        private IMixedRealityInputSystem inputSystem = null;
        private Quaternion yawRotation = Quaternion.identity;
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

        protected override void OnEnable()
        {
            base.OnEnable();
            InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        }

        protected virtual void OnDisable()
        {
            InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        }

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            // Determine the new active hand.
            IMixedRealityHand newActivehand = null;

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

            // Update the goal position and rotation.
            GoalPosition = CalculateGoalPosition();
            GoalRotation = CalculateGoalRotation();

            if (trackedHand != null)
            {
                UpdateWorkingPositionToGoal();
                UpdateWorkingRotationToGoal();
            }
        }

        /// <summary>
        /// Determines if a hand meets the requirements for use with constraining the tracked object.
        /// </summary>
        /// <param name="hand">The hand to check against.</param>
        /// <returns>True if this hand should be used from tracking.</returns>
        protected virtual bool IsHandActive(IMixedRealityHand hand)
        {
            // If transitioning between hands is not allowed, make sure the TrackedObjectType matches the hand.
            if (!transitionBetweenHands)
            {
                TrackedObjectType trackedObjectType;

                if (HandednessToTrackedObjectType(hand.ControllerHandedness, out trackedObjectType))
                {
                    if (trackedObjectType != SolverHandler.TrackedObjectToReference)
                    {
                        return false;
                    }
                }
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
                Ray ray = CalculateSafeZoneRay(goalPosition, trackedHand.ControllerHandedness, safeZone);
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
                switch (SolverHandler.TrackedObjectToReference)
                {
                    case TrackedObjectType.HandJointLeft:
                    case TrackedObjectType.MotionControllerLeft:
                        {
                            goalRotation *= Quaternion.Inverse(yawRotation);
                        }
                        break;

                    case TrackedObjectType.HandJointRight:
                    case TrackedObjectType.MotionControllerRight:
                        {
                            goalRotation *= yawRotation;
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
        /// <returns></returns>
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
        /// Swaps out the currently tracked hand while triggered appropriate events.
        /// </summary>
        /// <param name="hand">Which hand to track now.</param>
        private void ChangeTrackedObjectType(IMixedRealityHand hand)
        {
            if (hand != null)
            {
                TrackedObjectType trackedObjectType;

                if (HandednessToTrackedObjectType(hand.ControllerHandedness, out trackedObjectType))
                {
                    if (SolverHandler.TrackedObjectToReference != trackedObjectType)
                    {
                        SolverHandler.TrackedObjectToReference = trackedObjectType;

                        // Move the currently tracked hand to the top of the stack.
                        handStack.Remove(hand);
                        handStack.Insert(0, hand);
                    }

                    if (trackedHand == null)
                    {
                        trackedHand = hand;
                        onHandActivate.Invoke();
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
                    Debug.LogWarning("Failed to change the tracked object type because an IMixedRealityHand could not be resolved to a TrackedObjectType.");
                }
            }
            else
            {
                if (trackedHand != null)
                {
                    StartCoroutine(ToggleCursor(true));
                    trackedHand = null;
                    onHandDeactivate.Invoke();
                }
            }
        }

        private static bool HandednessToTrackedObjectType(Handedness handedness, out TrackedObjectType trackedObjectType)
        {
            switch (handedness)
            {
                case Handedness.Left:
                    trackedObjectType = TrackedObjectType.HandJointLeft;
                    return true;

                case Handedness.Right:
                    trackedObjectType = TrackedObjectType.HandJointRight;
                    return true;
            }

            trackedObjectType = default(TrackedObjectType);
            return false;
        }

        private static Ray CalculateSafeZoneRay(Vector3 origin, Handedness handedness, SolverSafeZone handSafeZone)
        {
            Vector3 direction;
            var cameraForward = CameraCache.Main.transform.forward;

            switch (handSafeZone)
            {
                default:
                case SolverSafeZone.UlnarSide:
                    {
                        direction = Vector3.Cross(cameraForward, Vector3.up);

                        if (handedness == Handedness.Left)
                        {
                            direction = -direction;
                        }
                    }
                    break;

                case SolverSafeZone.RadialSide:
                    {
                        direction = -Vector3.Cross(cameraForward, Vector3.up);

                        if (handedness == Handedness.Left)
                        {
                            direction = -direction;
                        }
                    }
                    break;

                case SolverSafeZone.AboveFingerTips:
                    {
                        direction = Vector3.Cross(cameraForward, Vector3.right);
                    }
                    break;

                case SolverSafeZone.BelowWrist:
                    {
                        direction = -Vector3.Cross(cameraForward, Vector3.right);
                    }
                    break;
            }

            return new Ray(origin + direction, -direction);
        }

        #region MonoBehaviour Implementation

        protected override void OnValidate()
        {
            base.OnValidate();

            HandednessYawRotation = handednessYawRotation;
        }

        protected void Start()
        {
            handBounds = GetComponent<HandBounds>();

            // Initially no hands are tacked or active.
            trackedHand = null;
            onLastHandLost.Invoke();
            onHandDeactivate.Invoke();
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHand;

            if (hand != null && !handStack.Contains(hand))
            {
                if (handStack.Count == 0)
                {
                    onFirstHandDetected.Invoke();
                }

                handStack.Add(hand);
            }
        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHand;

            if (hand != null)
            {
                handStack.Remove(hand);

                if (handStack.Count == 0)
                {
                    onLastHandLost.Invoke();
                }
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation
    }
}
