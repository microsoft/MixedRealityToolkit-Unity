// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
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
    [AddComponentMenu("MRTK/Input/Articulated Hand Controller")]
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

        private bool pinchedLastFrame = false;

        // For pointing pose polyfill only.
        // Remove once we have reliable cross-vendor aim pose from hands.
        private HandRay handRay;

        // Awake() override to prevent the base class
        // from using the base controller state instead of our
        // derived state. TODO: Brought up with Unity, may be
        // resolved in future XRI update.
        protected override void Awake()
        {
            base.Awake();

            // For pointing pose polyfill.
            handRay = new HandRay();

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

                    // Workaround for missing select actions on devices without interaction profiles
                    // for hands, such as Varjo and Quest. Should be removed once we have universal
                    // hand interaction profile(s) across vendors.
                    if ((selectAction.action?.controls.Count ?? 0) == 0)
                    {
                        // Debounced.
                        bool isPinched = pinchAmount >= (pinchedLastFrame ? 0.9f : 1.0f);

                        controllerState.selectInteractionState.active = isPinched;
                        controllerState.selectInteractionState.activatedThisFrame = isPinched && !pinchedLastFrame;
                        controllerState.selectInteractionState.deactivatedThisFrame = !isPinched && pinchedLastFrame;

                        pinchedLastFrame = isPinched;
                    }
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

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            base.UpdateTrackingInput(controllerState);

            // Workaround for missing aim/pointing-pose on devices without interaction profiles
            // for hands, such as Varjo and Quest. Should be removed once we have universal
            // hand interaction profile(s) across vendors.
            if (handsAggregator != null && (positionAction.action?.controls.Count ?? 0) == 0)
            {
                if (!handsAggregator.TryGetJoint(TrackedHandJoint.IndexProximal, HandNode, out HandJointPose knuckle))
                {
                    return;
                }
                if (!handsAggregator.TryGetJoint(TrackedHandJoint.Palm, HandNode, out HandJointPose palm))
                {
                    return;
                }

                // Tick the hand ray generator function. Uses index knuckle for position.
                handRay.Update(PlayspaceUtilities.ReferenceTransform.TransformPoint(knuckle.Position),
                               PlayspaceUtilities.ReferenceTransform.TransformVector(-palm.Up),
                               CameraCache.Main.transform,
                               HandNode.ToHandedness());
                
                Ray ray = handRay.Ray;
                controllerState.position = ray.origin;
                controllerState.rotation = Quaternion.LookRotation(ray.direction, PlayspaceUtilities.ReferenceTransform.TransformVector(palm.Up));
            }
        }
    }
}
