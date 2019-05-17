// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
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
        [Tooltip("TODO")]
        private bool transitionBetweenHands = true;

        [SerializeField]
        [Tooltip("TODO")]
        private bool activateOnPalmUp = true;

        [SerializeField]
        [Tooltip("TODO")]
        private float palmFacingThreshold = 0.3f;

        [SerializeField]
        [Tooltip("TODO")]
        private UnityEvent onHandActivate;

        [SerializeField]
        [Tooltip("TODO")]
        private UnityEvent onHandDeactivate;

        private bool handActive = false;
        private List<IMixedRealityHand> handStack = new List<IMixedRealityHand>();

        public override void SolverUpdate()
        {
            bool snap = HandActivationUpdate();

            GoalPosition = CalculateGoalPosition();
            GoalRotation = SolverHandler.TransformTarget.rotation;

            if (snap)
            {
                SnapTo(GoalPosition, GoalRotation);
            }
            else
            {
                UpdateWorkingPositionToGoal();
                UpdateWorkingRotationToGoal();
            }
        }

        private bool HandActivationUpdate()
        {
            IMixedRealityHand activeHand = null;

            foreach (var hand in handStack)
            {
                if (IsHandActive(hand))
                {
                    activeHand = hand;
                    break;
                }
            }

            return ChangeTrackedObjectType(activeHand);
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

            if (activateOnPalmUp)
            {
                MixedRealityPose pose;

                if (hand.TryGetJoint(TrackedHandJoint.Palm, out pose))
                {
                    return Vector3.Dot(pose.Up, CameraCache.Main.transform.forward) > palmFacingThreshold;
                }
            }

            return true;
        }

        private bool ChangeTrackedObjectType(IMixedRealityHand hand)
        {
            if (hand != null)
            {
                TrackedObjectType trackedObjectType;

                if (HandednessToTrackedObjectType(hand.ControllerHandedness, out trackedObjectType))
                {
                    if (SolverHandler.TrackedObjectToReference != trackedObjectType)
                    {
                        SolverHandler.TrackedObjectToReference = trackedObjectType;
                        SolverHandler.AdditionalOffset = -SolverHandler.AdditionalOffset;

                        // Move the currently tracked hand to the top of the stack.
                        handStack.Remove(hand);
                        handStack.Insert(0, hand);
                    }

                    if (!handActive)
                    {
                        onHandActivate.Invoke();
                        handActive = true;

                        return true;
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to change the tracked object type because an IMixedRealityHand could not be resolved to a TrackedObjectType.");
                }
            }
            else
            {
                if (handActive)
                {
                    onHandDeactivate.Invoke();
                    handActive = false;
                }
            }

            return false;
        }

        private Vector3 CalculateGoalPosition()
        {
            // TODO, maintain distance from hand.
            return SolverHandler.TransformTarget.position;
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
            onHandDeactivate.Invoke();
            handActive = false;
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
