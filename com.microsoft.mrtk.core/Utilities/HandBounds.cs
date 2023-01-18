// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Utility behavior to access the axis aligned bounds of an articulated hand
    /// (or the proxy visualizer of a motion controller).
    /// </summary>
    [AddComponentMenu("MRTK/Core/Hand Bounds")]
    public class HandBounds : MonoBehaviour
    {
        /// <summary>
        /// Accessor for the bounds associated with a handedness, calculated in global-axis-aligned space.
        /// </summary>
        public Dictionary<Handedness, Bounds> GlobalBounds { get; private set; } = new Dictionary<Handedness, Bounds>();

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
        private Dictionary<Handedness, Matrix4x4> boundsTransforms = new Dictionary<Handedness, Matrix4x4>();

        #region MonoBehaviour Implementation

        private void Update()
        {
            ComputeNewBounds(Handedness.Left);
            ComputeNewBounds(Handedness.Right);
        }

        private void OnDrawGizmos()
        {
            if (drawBoundsGizmo)
            {
                Gizmos.color = Color.yellow;
                foreach (var kvp in GlobalBounds)
                {
                    Gizmos.DrawWireCube(kvp.Value.center, kvp.Value.size);
                }
            }
            if (drawLocalBoundsGizmo)
            {
                Gizmos.color = Color.cyan;
                foreach (var kvp in LocalBounds)
                {
                    Gizmos.matrix = boundsTransforms[kvp.Key];
                    Gizmos.DrawWireCube(kvp.Value.center, kvp.Value.size);
                }
            }
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Fetches the hand joints from the hands aggregator subsystem and computes a local and global bounds.
        /// Currently requires hand joints, and will not work with motion controllers.
        /// </summary>
        private void ComputeNewBounds(Handedness hand)
        {
            if (XRSubsystemHelpers.HandsAggregator == null ||
                !XRSubsystemHelpers.HandsAggregator.TryGetEntireHand(hand == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand,
                                                out IReadOnlyList<HandJointPose> jointPoses))
            {
                GlobalBounds.Remove(hand);
                LocalBounds.Remove(hand);
                return;
            }
            else
            {
                HandJointPose palmPose = jointPoses[HandsUtils.ConvertToIndex(TrackedHandJoint.Palm)];
                Bounds newGlobalBounds = new Bounds(palmPose.Position, Vector3.zero);
                Bounds newLocalBounds = new Bounds(Vector3.zero, Vector3.zero);

                for (int i = 0; i < jointPoses.Count; i++)
                {
                    newGlobalBounds.Encapsulate(jointPoses[i].Position);
                    newLocalBounds.Encapsulate(Quaternion.Inverse(palmPose.Rotation) * (jointPoses[i].Position - palmPose.Position));
                }

                GlobalBounds[hand] = newGlobalBounds;
                LocalBounds[hand] = newLocalBounds;

                // We must normalize the quaternion before constructing the TRS matrix; non-unit-length quaternions
                // may be emitted from the palm-pose and they must be renormalized.
                boundsTransforms[hand] = Matrix4x4.TRS(palmPose.Position, palmPose.Rotation.normalized, Vector3.one);
            }
        }
    }
}
