// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An XRController for binding to hand input. Able to support variable pinch
    /// select through the HandsAggregatorSubsystem.
    /// </summary>
    [AddComponentMenu("Scripts/Microsoft/MRTK/Input/Articulated Hand Controller")]
    public class ArticulatedHandController : ActionBasedController
    {
        #region Associated hand select values

        [SerializeField, Tooltip("The XRNode associated with this Hand Controller. Expected to be XRNode.LeftHand or XRNode.RightHand.")]
        private XRNode handNode;

        /// <summary>
        /// The XRNode associated with this Hand Controller.
        /// </summary>
        /// <remarks>Expected to be XRNode.LeftHand or XRNode.RightHand.</remarks>
        public XRNode HandNode => handNode;

        /// <summary>
        /// Is the hand ready to select? Typically, this
        /// represents whether the hand is in a pinching pose,
        /// within the FOV set by the aggregator config.
        /// </summary>
        public bool PinchSelectReady => (currentControllerState is ArticulatedHandControllerState handControllerState) ?
                                                handControllerState.PinchSelectReady : false;

        [Obsolete("Please use the selectInteractionState.value instead.")]
        public float PinchSelectProgress => currentControllerState.selectInteractionState.value;

        /// <summary>
        /// The worldspace pose of the pinch selection.
        /// </summary>
        public Pose PinchSelectPose => (currentControllerState is ArticulatedHandControllerState handControllerState) ?
                                                handControllerState.PinchPose : Pose.identity;

        #endregion Associated hand select values

        #region Properties

        private HandsAggregatorSubsystem handsAggregator;

        protected HandsAggregatorSubsystem HandsAggregator => handsAggregator;

        #endregion Properties

        // Awake() override to prevent the base class
        // from using the base controller state instead of our
        // derived state. TODO: Brought up with Unity, may be
        // resolved in future XRI update.
        protected override void Awake()
        {
            base.Awake();

            currentControllerState = new ArticulatedHandControllerState();
        }

        private static readonly ProfilerMarker UpdateTrackingInputPerfMarker =
            new ProfilerMarker("[MRTK] ArticulatedHandController.UpdateTrackingInput");

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            base.UpdateInput(controllerState);

            using (UpdateTrackingInputPerfMarker.Auto())
            {
                if (controllerState == null)
                    return;

                // Cast to expose hand state.
                ArticulatedHandControllerState handControllerState = controllerState as ArticulatedHandControllerState;

                Debug.Assert(handControllerState != null);

                // Get hands aggregator subsystem reference, if still null.
                // Should avoid per-frame allocs by only acquiring aggregator reference once.
                if (handsAggregator == null)
                {
                    handsAggregator = HandsUtils.GetSubsystem();
                }

                // If we still don't have an aggregator, then don't update selects.
                if (handsAggregator == null) { return; }

                bool gotPinchData = handsAggregator.TryGetPinchProgress(handNode, out bool isPinchReady, out bool isPinching, out float pinchAmount);

                // If we got pinch data, write it into our select interaction state.
                if (gotPinchData)
                {
                    controllerState.selectInteractionState.value = pinchAmount;
                }

                handControllerState.PinchSelectReady = isPinchReady;

                if (isPinching && handsAggregator.TryGetPinchingPoint(handNode, out HandJointPose pinchPose))
                {
                    handControllerState.PinchPose.position = pinchPose.Position;
                    handControllerState.PinchPose.rotation = pinchPose.Rotation;
                }
                else
                {
                    handControllerState.PinchPose.position = controllerState.position;
                    handControllerState.PinchPose.rotation = controllerState.rotation;
                }
            }
        }
    }
}
