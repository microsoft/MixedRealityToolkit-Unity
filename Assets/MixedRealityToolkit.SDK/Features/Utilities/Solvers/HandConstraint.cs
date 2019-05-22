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
    /// Provides a solver that constrains the target to a region safe for hand interactions.
    /// </summary>
    [RequireComponent(typeof(InputSystemGlobalListener))]
    [RequireComponent(typeof(HandBounds))]
    public class HandConstraint : Solver, IMixedRealitySourceStateHandler
    {
        /// <summary>
        /// TODO
        /// </summary>
        public enum HandSafeZone
        {
            Hypothenar = 0,
            Thumb = 1,
            FingerTips = 2,
            Wrist = 3
        }

        [Header("Hand Constraint")]
        [SerializeField]
        [Tooltip("TODO")]
        private HandSafeZone handSafeZone = HandSafeZone.Hypothenar;

        [SerializeField]
        [Tooltip("TODO")]
        private float handSafeZoneExpansion = 0.1f;

        [SerializeField]
        [Tooltip("Should this solver automatically switch to tracking the primary hand? The primary hand is the hand in view the longest or last active.")]
        private bool transitionBetweenHands = true;

        [SerializeField]
        [Tooltip("TODO")]
        private bool hideHandCursorsOnActivate = true;

        [SerializeField]
        [Tooltip("TODO")]
        private UnityEvent onHandActivate = null;

        [SerializeField]
        [Tooltip("TODO")]
        private UnityEvent onHandDeactivate = null;

        private IMixedRealityHand trackedHand = null;
        private List<IMixedRealityHand> handStack = new List<IMixedRealityHand>();
        private HandBounds handBounds = null;

        public override void SolverUpdate()
        {
            var previousTrackedHand = trackedHand;
            IMixedRealityHand activehand = null;

            foreach (var hand in handStack)
            {
                if (IsHandActive(hand))
                {
                    activehand = hand;
                    break;
                }
            }

            ChangeTrackedObjectType(activehand);

            GoalPosition = CalculateGoalPosition();
            GoalRotation = SolverHandler.TransformTarget.rotation;

            if (previousTrackedHand == null && trackedHand != null)
            {
                SnapTo(GoalPosition, GoalRotation);
            }
            else
            {
                UpdateWorkingPositionToGoal();
                UpdateWorkingRotationToGoal();
            }
        }

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

            return true;
        }

        protected virtual Vector3 CalculateGoalPosition()
        {
            var goalPosition = SolverHandler.TransformTarget.position;
            Bounds trackedHandBounds;

            if (trackedHand != null &&
                handBounds.Bounds.TryGetValue(trackedHand.ControllerHandedness, out trackedHandBounds))
            {
                float distance;
                Ray ray = CalculateSafeZoneRay(goalPosition, trackedHand.ControllerHandedness, handSafeZone);
                trackedHandBounds.Expand(handSafeZoneExpansion);

                if (trackedHandBounds.IntersectRay(ray, out distance))
                {
                    goalPosition = ray.origin + ray.direction * distance;
                }
            }

            return goalPosition;
        }

        protected virtual IEnumerator ToggleCursor(bool visible, bool frameDelay = false)
        {
            if (hideHandCursorsOnActivate)
            {
                if (frameDelay)
                {
                    yield return null;
                }

                foreach (var pointer in trackedHand?.InputSource.Pointers)
                {
                    pointer?.BaseCursor?.SetVisibility(visible);
                }
            }
        }

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
                        ToggleCursor(true);
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
                    ToggleCursor(true);
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

        private static Ray CalculateSafeZoneRay(Vector3 origin, Handedness handedness, HandSafeZone handSafeZone)
        {
            Vector3 direction;

            switch (handSafeZone)
            {
                default:
                case HandSafeZone.Hypothenar:
                    {
                        direction = Vector3.Cross(CameraCache.Main.transform.forward, Vector3.up);

                        if (handedness == Handedness.Left)
                        {
                            direction = -direction;
                        }
                    }
                    break;

                case HandSafeZone.Thumb:
                    {
                        direction = -Vector3.Cross(CameraCache.Main.transform.forward, Vector3.up);

                        if (handedness == Handedness.Left)
                        {
                            direction = -direction;
                        }
                    }
                    break;

                case HandSafeZone.FingerTips:
                    {
                        direction = Vector3.Cross(CameraCache.Main.transform.forward, Vector3.right);
                    }
                    break;

                case HandSafeZone.Wrist:
                    {
                        direction = -Vector3.Cross(CameraCache.Main.transform.forward, Vector3.right);
                    }
                    break;
            }

            return new Ray(origin + direction, -direction);
        }

        #region MonoBehaviour Implementation

        protected void Start()
        {
            handBounds = GetComponent<HandBounds>();

            // Initially a hand is not active.
            trackedHand = null;
            onHandDeactivate.Invoke();
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHand;

            if (hand != null && !handStack.Contains(hand))
            {
                handStack.Add(hand);
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHand;

            if (hand != null)
            {
                handStack.Remove(hand);
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation
    }
}
