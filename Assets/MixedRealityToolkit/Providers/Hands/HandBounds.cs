// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility behavior to access the axis aligned bounds of IMixedRealityHands.
    /// </summary>
    public class HandBounds : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
    {
        /// <summary>
        /// Accessor for the bounds associated with a handedness.
        /// </summary>
        public Dictionary<Handedness, Bounds> Bounds { get; private set; } = new Dictionary<Handedness, Bounds>();

        [SerializeField]
        [Tooltip("Should a gizmo be drawn to represent the hand bounds.")]
        private bool drawBoundsGizmo = false;

        /// <summary>
        /// Should a gizmo be drawn to represent the hand bounds.
        /// </summary>
        public bool DrawBoundsGizmo
        {
            get { return drawBoundsGizmo; }
            set { drawBoundsGizmo = value; }
        }

        private IMixedRealityInputSystem inputSystem = null;
        
        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }

                return inputSystem;
            }
        }

        #region MonoBehaviour Implementation

        private void OnEnable()
        {
            InputSystem?.Register(gameObject);
        }

        private void OnDisable()
        {
            InputSystem?.Unregister(gameObject);
        }

        private void OnDrawGizmos()
        {
            if (drawBoundsGizmo)
            {
                foreach (var kvp in Bounds)
                {
                    Gizmos.DrawWireCube(kvp.Value.center, kvp.Value.size);
                }
            }
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData)
        {
        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            var hand = eventData.Controller as IMixedRealityHand;

            if (hand != null)
            {
                Bounds.Remove(hand.ControllerHandedness);
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation

        #region IMixedRealityHandJointHandler Implementation

        /// <inheritdoc />
        public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            MixedRealityPose palmPose;

            if (eventData.InputData.TryGetValue(TrackedHandJoint.Palm, out palmPose))
            {
                var newBounds = new Bounds(palmPose.Position, Vector3.zero);

                foreach (var kvp in eventData.InputData)
                {
                    if (kvp.Key == TrackedHandJoint.None || 
                        kvp.Key == TrackedHandJoint.Palm)
                    {
                        continue;
                    }

                    newBounds.Encapsulate(kvp.Value.Position);
                }

                Bounds[eventData.Handedness] = newBounds;
            }
        }

        #endregion IMixedRealityHandJointHandler Implementation
    }
}
