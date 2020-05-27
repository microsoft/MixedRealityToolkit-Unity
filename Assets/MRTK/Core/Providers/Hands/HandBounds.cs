// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility behavior to access the axis aligned bounds of IMixedRealityHands (or the proxy visualizer of IMixedRealityControllers).
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/HandBounds")]
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

        #region MonoBehaviour Implementation

        private void OnEnable()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
        }

        private void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
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
            var hand = eventData.Controller;

            if (hand != null)
            {
                // If a hand does not contain joints, OnHandJointsUpdated will not be called the bounds should
                // be calculated based on the proxy visuals.
                bool handContainsJoints = (hand as IMixedRealityHand) != null;

                if (!handContainsJoints)
                {
                    var proxy = hand.Visualizer?.GameObjectProxy;

                    if (proxy != null)
                    {
                        var newBounds = new Bounds(proxy.transform.position, Vector3.zero);
                        var boundsPoints = new List<Vector3>();
                        BoundsExtensions.GetRenderBoundsPoints(proxy, boundsPoints, 0);

                        foreach (var point in boundsPoints)
                        {
                            newBounds.Encapsulate(point);
                        }

                        Bounds[hand.ControllerHandedness] = newBounds;
                    }
                }
            }
        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            var hand = eventData.Controller;

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
