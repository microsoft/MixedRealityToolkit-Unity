// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic hand joint visualizer that draws an instanced mesh on each hand joint.
    /// </summary>
    [AddComponentMenu("Scripts/Microsoft/MRTK/Hands/Controller Visualizer")]
    public class ControllerVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this controller is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this controller is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        private HandsAggregatorSubsystem handsSubsystem;

        public GameObject ControllerRenderer;

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            handsSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();

            // Ensure hand is not visible until we can update position first time.
            ControllerRenderer.SetActive(false);
        }

        protected void OnDisable()
        {
            // Disable the rigged hand renderer when this component is disabled
            ControllerRenderer.SetActive(false);
        }

        private void LateUpdate()
        {
            if (handsSubsystem == null)
            {
                return;
            }

            // Query all joints in the hand.
            // If we don't get any joints, then allow the controller to render
            if (!handsSubsystem.TryGetEntireHand(handNode, out IReadOnlyList<HandJointPose> joints))
            {
                ControllerRenderer.SetActive(true);
                return;
            }
        }
    }
}
