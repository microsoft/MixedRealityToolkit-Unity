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
        public bool PinchSelectReady => (currentControllerState is ArticulatedHandControllerState handControllerState) && handControllerState.PinchSelectReady;

        #endregion Associated hand select values

        #region Properties
        protected HandsAggregatorSubsystem HandsAggregator => handsAggregator ??= HandsUtils.GetSubsystem();
        private HandsAggregatorSubsystem handsAggregator;

        #endregion Properties

        private bool pinchedLastFrame = false;

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

                // If we still don't have an aggregator, then don't update selects.
                if (HandsAggregator == null) { return; }

                bool gotPinchData = HandsAggregator.TryGetPinchProgress(handNode, out bool isPinchReady, out bool isPinching, out float pinchAmount);

                // If we got pinch data, write it into our select interaction state.
                if (gotPinchData)
                {
                    controllerState.selectInteractionState.value = pinchAmount;

                    // Workaround for missing select actions on devices without interaction profiles
                    // for hands, such as Varjo and Quest. Should be removed once we have universal
                    // hand interaction profile(s) across vendors.
                    if (!selectAction.action.HasAnyControls())
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
            }
        }

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            base.UpdateTrackingInput(controllerState);

            // In case the position input action is not provided, we will try to polyfill it with the device position.
            // Should be removed once we have universal hand interaction profile(s) across vendors.

            if (!positionAction.action.HasAnyControls() && TryGetPolyfillDevicePose(out Pose devicePose))
            {
                controllerState.position = devicePose.position;
                controllerState.rotation = devicePose.rotation;
                
                // Polyfill the tracking state, too.
                controllerState.inputTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;
            }
        }


        private static readonly Quaternion rightPalmOffset = Quaternion.Inverse(new Quaternion(Mathf.Sqrt(0.125f), Mathf.Sqrt(0.125f), -Mathf.Sqrt(1.5f) / 2.0f, Mathf.Sqrt(1.5f) / 2.0f));
        private static readonly Quaternion leftPalmOffset = Quaternion.Inverse(new Quaternion(Mathf.Sqrt(0.125f), -Mathf.Sqrt(0.125f), Mathf.Sqrt(1.5f) / 2.0f, Mathf.Sqrt(1.5f) / 2.0f));

        // Workaround for missing device pose on devices without interaction profiles
        // for hands, such as Varjo and Quest. Should be removed once we have universal
        // hand interaction profile(s) across vendors.
        private bool TryGetPolyfillDevicePose(out Pose devicePose)
        {
            bool poseRetrieved = false;
            Handedness handedness = HandNode.ToHandedness();

            // palmPose retrieved in global space.
            if (HandsAggregator != null && HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, HandNode, out HandJointPose palmPose))
            {
                // XRControllers work in OpenXR scene-origin-space, so we need to transform
                // our global palm pose back into scene-origin-space.
                devicePose = PlayspaceUtilities.InverseTransformPose(palmPose.Pose);

                switch (handedness)
                {
                    case Handedness.Left:
                        devicePose.rotation *= leftPalmOffset;
                        poseRetrieved = true;
                        break;
                    case Handedness.Right:
                        devicePose.rotation *= rightPalmOffset;
                        poseRetrieved = true;
                        break;
                    default:
                        Debug.LogError("No polyfill available for device with handedness " + handedness);
                        devicePose = Pose.identity;
                        poseRetrieved = false;
                        break;
                };
            }
            else
            {
                devicePose = Pose.identity;
            }

            return poseRetrieved;
        }
    }
}
