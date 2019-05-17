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
    public class HandConstraint : Solver, IMixedRealitySourceStateHandler
    {
        [Header("Hand Constraint")]
        [SerializeField]
        [Tooltip("TODO")]
        private bool billboard = true;

        [SerializeField]
        [Tooltip("TODO")]
        private bool transitionBetweenHands = true;

        [SerializeField]
        [Tooltip("TODO")]
        private bool activateOnPalmUp = true;

        [SerializeField]
        [Tooltip("TODO")]
        private UnityEvent onHandPresent;

        [SerializeField]
        [Tooltip("TODO")]
        private UnityEvent onHandNotPresent;

        private InputSystemGlobalListener.Implementation inputSystemGlobalListener = new InputSystemGlobalListener.Implementation();
        private List<IMixedRealityHand> activeHands = new List<IMixedRealityHand>();
        private bool handPresent = false;

        public override void SolverUpdate()
        {
            bool snap = false;

            if (transitionBetweenHands)
            {
                snap = TransitionBetweenHands();
            }

            GoalPosition = SolverHandler.TransformTarget.position;
            GoalRotation = billboard ? CameraCache.Main.transform.rotation : SolverHandler.TransformTarget.rotation;

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

        private bool TransitionBetweenHands()
        {
            IMixedRealityHand activeHand = null;

            foreach (var hand in activeHands)
            {
                if (HandIsValid(hand))
                {
                    activeHand = hand;
                    break;
                }
            }

            return ChangeTrackedObjectType(activeHand);
        }

        private bool HandIsValid(IMixedRealityHand hand)
        {
            if (activateOnPalmUp)
            {
                MixedRealityPose pose;

                if (hand.TryGetJoint(TrackedHandJoint.Palm, out pose))
                {
                    const float palmFacingThreshold = 0.3f;
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
                    SolverHandler.TrackedObjectToReference = trackedObjectType;

                    if (!handPresent)
                    {
                        onHandPresent.Invoke();
                        handPresent = true;

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
                if (handPresent)
                {
                    onHandNotPresent.Invoke();
                    handPresent = false;
                }
            }

            return false;
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

        protected override void OnEnable()
        {
            base.OnEnable();

            inputSystemGlobalListener.OnEnable(gameObject);
        }

        protected virtual async void Start()
        {
            await inputSystemGlobalListener.Start(gameObject);

            // Initially a hand is not present.
            onHandNotPresent.Invoke();
            handPresent = false;
        }

        protected virtual void OnDisable()
        {
            inputSystemGlobalListener.OnDisable(gameObject);
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHand;

            if (hand != null)
            {
                activeHands.Add(hand);
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHand;

            if (hand != null)
            {
                activeHands.Remove(hand);
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation
    }
}
