// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that constrains to hands with behaviors common to hand constrained UI.
    /// </summary>
    public class HandConstraint : Solver, IMixedRealitySourceStateHandler
    {
        [SerializeField]
        [Tooltip("TODO")]
        private bool billboard = true;

        [SerializeField]
        [Tooltip("TODO")]
        private bool transitionBetweenHands = true;

        [SerializeField]
        [Tooltip("TODO")]
        private bool displayOnPalmUp = true;

        private InputSystemGlobalListener.Implementation inputSystemGlobalListener = new InputSystemGlobalListener.Implementation();
        private List<IMixedRealityHand> activeHands = new List<IMixedRealityHand>();

        public override void SolverUpdate()
        {
            if (transitionBetweenHands)
            {
                TransitionBetweenHands();
            }

            GoalPosition = SolverHandler.TransformTarget.position;
            GoalRotation = billboard ? CameraCache.Main.transform.rotation : SolverHandler.TransformTarget.rotation;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        private void TransitionBetweenHands()
        {
            foreach (var hand in activeHands)
            {
                TrackedObjectType trackedObjectType;

                if (HandIsValid(hand) && HandednessToTrackedObjectType(hand.ControllerHandedness, out trackedObjectType))
                {
                    ChangeTrackedObjectType(trackedObjectType);
                    return;
                }
            }

            ChangeTrackedObjectType(null);
        }

        private bool HandIsValid(IMixedRealityHand hand)
        {
            if (displayOnPalmUp)
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

        private void ChangeTrackedObjectType(TrackedObjectType? trackedObjectType)
        {
            // TODO show/hide
            if (trackedObjectType != null)
            {
                SolverHandler.TrackedObjectToReference = trackedObjectType.Value;
            }
            else
            {

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

        protected override void OnEnable()
        {
            base.OnEnable();

            inputSystemGlobalListener.OnEnable(gameObject);
        }

        protected virtual async void Start()
        {
            await inputSystemGlobalListener.Start(gameObject);
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
