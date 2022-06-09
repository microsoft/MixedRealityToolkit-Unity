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
    [AddComponentMenu("Scripts/Microsoft/MRTK/Hands/VisualizerMediator")]
    public class VisualizerMediator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this controller is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this controller is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        private HandsAggregatorSubsystem handsSubsystem;

        public HandJointVisualizer vis1;

        public RiggedHandMeshVisualizer vis2;

        public ControllerVisualizer vis3;

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            handsSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
        }

        private float timeout = 0.7f;
        private float timeoutCounter = 0.0f;

        public bool handsActive;

        private void LateUpdate()
        {
            if (handsSubsystem == null)
            {
                return;
            }

            // Query all joints in the hand.
            if (!handsSubsystem.TryGetEntireHand(handNode, out IReadOnlyList<HandJointPose> joints))
            {
                if(handsActive)
                {
                    timeoutCounter += Time.deltaTime;
                    if(timeoutCounter > timeout)
                    {
                        handsActive = false;
                    }
                }
                else
                {
                    timeoutCounter = 0.0f;
                }
            }
            else
            {
                if (!handsActive)
                {
                    timeoutCounter += Time.deltaTime;
                    if (timeoutCounter > timeout)
                    {
                        handsActive = true;
                    }
                }
                else
                {
                    timeoutCounter = 0.0f;
                }
            }

            if(handsActive)
            {
                vis1.enabled = true;
                vis2.enabled = true;
                vis3.enabled = false;
            }
            else
            {
                vis1.enabled = false;
                vis2.enabled = false;
                vis3.enabled = true;
            }
        }
    }
}
