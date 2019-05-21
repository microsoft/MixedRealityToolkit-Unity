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
    /// Provides a solver that constrains to hands with behaviors common to hand constrained UI.
    /// </summary>
    [RequireComponent(typeof(InputSystemGlobalListener))]
    public class HandConstraint : Solver, IMixedRealitySourceStateHandler
    {
        [Header("Hand Constraint")]
        [SerializeField]
        [Tooltip("Should this solver automatically switch to tracking the primary hand? The primary hand is the hand in view the longest or last active.")]
        private bool transitionBetweenHands = true;

        [SerializeField]
        [Tooltip("How much to offset the hand along the view's right vector when the hand is completely vertical (interpolates to handHorizontalOffset)")]
        private float handVerticalOffset = -0.1f;

        [SerializeField]
        [Tooltip("How much to offset the hand along the view's right vector when the hand is completely horizontal (interpolates to handVerticalOffset)")]
        private float handHorizontalOffset = -0.2f;

        [SerializeField]
        [Tooltip("When true changes the sign of the vertical and horizontal offset when hands automatically transition.")]
        private bool negateOffsetsOnHandTransition = true;

        [SerializeField]
        [Tooltip("When true calls onHandActivate/onHandDeactivate only when the hand satisfies the jointFacingThreshold.")]
        private bool activateOnJointUp = true;

        [SerializeField]
        [Tooltip("TODO")]
        private float jointFacingThreshold = 0.3f;

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

        public override void SolverUpdate()
        {
            var previousTrackedHand = trackedHand;
            HandActivationUpdate();

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

        private void HandActivationUpdate()
        {
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
        }

        private bool IsHandActive(IMixedRealityHand hand)
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

            if (activateOnJointUp)
            {
                MixedRealityPose pose;

                if (hand.TryGetJoint(SolverHandler.TrackedHandJoint, out pose))
                {
                    return Vector3.Dot(pose.Up, CameraCache.Main.transform.forward) > jointFacingThreshold;
                }
            }

            return true;
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

                        if (negateOffsetsOnHandTransition)
                        {
                            handVerticalOffset = -handVerticalOffset;
                            handHorizontalOffset = -handHorizontalOffset;
                        }

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

        private Vector3 CalculateGoalPosition()
        {
            var offsetWeight = 0.0f;
            var pose = new MixedRealityPose();

            if (trackedHand?.TryGetJoint(SolverHandler.TrackedHandJoint, out pose) != null)
            {
                offsetWeight = Mathf.Abs(Vector3.Dot(pose.Forward, CameraCache.Main.transform.up));
            }

            var viewRight = Vector3.Cross(CameraCache.Main.transform.forward, Vector3.up);
            return SolverHandler.TransformTarget.position + viewRight * Mathf.Lerp(handHorizontalOffset, handVerticalOffset, offsetWeight);
        }

        private IEnumerator ToggleCursor(bool visible, bool frameDelay = false)
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

        #region MonoBehaviour Implementation

        protected void Start()
        {
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
