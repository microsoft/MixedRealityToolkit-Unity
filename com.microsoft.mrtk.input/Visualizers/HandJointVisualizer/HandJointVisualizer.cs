// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic hand joint visualizer that draws an instanced mesh on each hand joint.
    /// This visualizer is mostly recommended for debugging purposes; try the
    /// the <see cref="RiggedHandMeshVisualizer"/> for a more visually pleasing
    /// hand visualization.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Visualizers/Hand Joint Visualizer")]
    public class HandJointVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this hand is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        [SerializeField]
        [Tooltip("Joint visualization mesh.")]
        private Mesh jointMesh;

        /// <summary> Joint visualization mesh. </summary>
        public Mesh JointMesh { get => jointMesh; set => jointMesh = value; }

        [SerializeField]
        [Tooltip("Joint visualization material.")]
        private Material jointMaterial;

        /// <summary> Joint visualization material. </summary>
        public Material JointMaterial { get => jointMaterial; set => jointMaterial = value; }

        private HandsAggregatorSubsystem handsSubsystem;

        // Transformation matrix for each joint.
        private List<Matrix4x4> jointMatrices = new List<Matrix4x4>();

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            handsSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();

            if (handsSubsystem == null)
            {
                StartCoroutine(EnableWhenSubsystemAvailable());
            }
            else
            {
                for (int i = 0; i < (int)TrackedHandJoint.TotalJoints; i++)
                {
                    jointMatrices.Add(new Matrix4x4());
                }
            }
        }

        void OnDrawGizmos()
        {
            if (!enabled) { return; }
            
            // Query all joints in the hand.
            if (handsSubsystem == null || !handsSubsystem.TryGetEntireHand(handNode, out IReadOnlyList<HandJointPose> joints))
            {
                return;
            }

            for (int i = 0; i < joints.Count; i++)
            {
                HandJointPose joint = joints[i];
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(joint.Position, joint.Forward * 0.01f);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(joint.Position, joint.Right * 0.01f);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(joint.Position, joint.Up * 0.01f);
            }
        }

        /// <summary>
        /// Coroutine to wait until subsystem becomes available.
        /// </summary>
        private IEnumerator EnableWhenSubsystemAvailable()
        {
            yield return new WaitUntil(() => XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>() != null);
            OnEnable();
        }

        private void Update()
        {
            // Query all joints in the hand.
            if (handsSubsystem == null || !handsSubsystem.TryGetEntireHand(handNode, out IReadOnlyList<HandJointPose> joints))
            {
                return;
            }

            RenderJoints(joints);
        }

        private void RenderJoints(IReadOnlyList<HandJointPose> joints)
        {
            for (int i = 0; i < joints.Count; i++)
            {
                // Skip joints with uninitialized quaternions.
                // This is temporary; eventually the HandsSubsystem will
                // be robust enough to never give us broken joints.
                if (joints[i].Rotation.Equals(new Quaternion(0, 0, 0, 0)))
                {
                    continue;
                }

                // Fill the matrices list with TRSs from the joint poses.
                jointMatrices[i] = Matrix4x4.TRS(joints[i].Position, joints[i].Rotation.normalized, Vector3.one * joints[i].Radius);
            }

            // Draw the joints.
            Graphics.DrawMeshInstanced(jointMesh, 0, jointMaterial, jointMatrices);
        }
    }
}
