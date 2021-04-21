// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
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
        /// Accessor for the bounds associated with a handedness, calculated in global-axis-aligned space.
        /// </summary>
        public Dictionary<Handedness, Bounds> Bounds { get; private set; } = new Dictionary<Handedness, Bounds>();

        /// <summary>
        /// Accessor for the bounds associated with a handedness, calculated in local hand-space, locally axis aligned.
        /// </summary>
        public Dictionary<Handedness, Bounds> LocalBounds { get; private set; } = new Dictionary<Handedness, Bounds>();

        [SerializeField]
        [Tooltip("Should a gizmo be drawn to represent the hand bounds.")]
        private bool drawBoundsGizmo = false;

        /// <summary>
        /// Should a gizmo be drawn to represent the hand bounds.
        /// </summary>
        public bool DrawBoundsGizmo
        {
            get => drawBoundsGizmo;
            set => drawBoundsGizmo = value;
        }

        [SerializeField]
        [Tooltip("Should a gizmo be drawn to represent the locally-calculated hand bounds.")]
        private bool drawLocalBoundsGizmo = false;

        /// <summary>
        /// Should a gizmo be drawn to represent the locally-calculated hand bounds.
        /// </summary>
        public bool DrawLocalBoundsGizmo
        {
            get => drawLocalBoundsGizmo;
            set => drawLocalBoundsGizmo = value;
        }

        /// <summary>
        /// Mapping between controller handedness and associated hand transforms.
        /// Used to transform the debug gizmos when rendering the hand AABBs.
        /// </summary>
        private Dictionary<Handedness, Matrix4x4> BoundsTransforms = new Dictionary<Handedness, Matrix4x4>();

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
                Gizmos.color = Color.yellow;
                foreach (var kvp in Bounds)
                {
                    Gizmos.DrawWireCube(kvp.Value.center, kvp.Value.size);
                }
            }
            if (drawLocalBoundsGizmo)
            {
                Gizmos.color = Color.cyan;
                foreach (var kvp in LocalBounds)
                {
                    Gizmos.matrix = BoundsTransforms[kvp.Key];
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
                        // Bounds calculated in proxy-space will have an origin of zero, but bounds
                        // calculated in global space will have an origin centered on the proxy transform.
                        var newGlobalBounds = new Bounds(proxy.transform.position, Vector3.zero);
                        var newLocalBounds = new Bounds(Vector3.zero, Vector3.zero);
                        var boundsPoints = new List<Vector3>();
                        BoundsExtensions.GetRenderBoundsPoints(proxy, boundsPoints, 0);

                        foreach (var point in boundsPoints)
                        {
                            newGlobalBounds.Encapsulate(point);
                            // Local hand-space bounds are encapsulated using proxy-space point coordinates
                            newLocalBounds.Encapsulate(proxy.transform.InverseTransformPoint(point));
                        }

                        Bounds[hand.ControllerHandedness] = newGlobalBounds;
                        LocalBounds[hand.ControllerHandedness] = newLocalBounds;
                        BoundsTransforms[hand.ControllerHandedness] = proxy.transform.localToWorldMatrix;
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
                LocalBounds.Remove(hand.ControllerHandedness);
                BoundsTransforms.Remove(hand.ControllerHandedness);
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
                var newGlobalBounds = new Bounds(palmPose.Position, Vector3.zero);
                var newLocalBounds = new Bounds(Vector3.zero, Vector3.zero);

                foreach (var kvp in eventData.InputData)
                {
                    if (kvp.Key == TrackedHandJoint.None ||
                        kvp.Key == TrackedHandJoint.Palm)
                    {
                        continue;
                    }

                    newGlobalBounds.Encapsulate(kvp.Value.Position);
                    newLocalBounds.Encapsulate(Quaternion.Inverse(palmPose.Rotation) * (kvp.Value.Position - palmPose.Position));
                }

                Bounds[eventData.Handedness] = newGlobalBounds;
                LocalBounds[eventData.Handedness] = newLocalBounds;

                // We must normalize the quaternion before constructing the TRS matrix; non-unit-length quaternions
                // may be emitted from the palm-pose and they must be renormalized.
                BoundsTransforms[eventData.Handedness] = Matrix4x4.TRS(palmPose.Position, palmPose.Rotation.normalized, Vector3.one);
            }
        }

        #endregion IMixedRealityHandJointHandler Implementation
    }
}
