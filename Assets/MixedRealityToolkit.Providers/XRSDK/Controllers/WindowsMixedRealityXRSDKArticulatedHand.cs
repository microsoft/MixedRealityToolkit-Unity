// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    /// <summary>
    /// XR SDK implementation of the Windows Mixed Reality motion controllers.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right },
        "StandardAssets/Textures/MotionController")]
    public class WindowsMixedRealityXRSDKArticulatedHand : BaseWindowsMixedRealityXRSDKSource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityXRSDKArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress),
            new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger)
        };

        private static readonly HandFinger[] handFingers = Enum.GetValues(typeof(HandFinger)) as HandFinger[];
        private readonly List<Bone> fingerBones = new List<Bone>();
        private readonly Dictionary<TrackedHandJoint, MixedRealityPose> unityJointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        #region Update data functions

        /// <inheritdoc />
        public override void UpdateController(InputDevice inputDevice)
        {
            if (!Enabled) { return; }

            base.UpdateController(inputDevice);

            UpdateHandData(inputDevice);

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.IndexFinger:
                        UpdateIndexFingerData(Interactions[i]);
                        break;
                }
            }
        }

        /// <summary>
        /// Update the hand data from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateHandData(InputDevice inputDevice)
        {
            if (inputDevice.TryGetFeatureValue(CommonUsages.handData, out Hand hand))
            {
                foreach (HandFinger finger in handFingers)
                {
                    if (hand.TryGetFingerBones(finger, fingerBones))
                    {
                        for (int i = 0; i < fingerBones.Count; i++)
                        {
                            TrackedHandJoint trackedHandJoint = ConvertToTrackedHandJoint(finger, i);
                            Bone bone = fingerBones[i];

                            Vector3 position = Vector3.zero;
                            Quaternion rotation = Quaternion.identity;

                            if (bone.TryGetPosition(out position) || bone.TryGetRotation(out rotation))
                            {
                                // We want input sources to follow the Playspace, so fold in the playspace transform here to
                                // put the controller pose into world space.
                                position = MixedRealityPlayspace.TransformPoint(position);
                                rotation = MixedRealityPlayspace.Rotation * rotation;

                                unityJointPoses[trackedHandJoint] = new MixedRealityPose(position, rotation);
                            }
                        }
                    }
                }
            }

            CoreServices.InputSystem?.RaiseHandJointsUpdated(InputSource, ControllerHandedness, unityJointPoses);
        }

        private void UpdateIndexFingerData(MixedRealityInteractionMapping interactionMapping)
        {
            // Update the interaction data source
            interactionMapping.PoseData = unityJointPoses[TrackedHandJoint.IndexTip];

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
            }
        }

        private TrackedHandJoint ConvertToTrackedHandJoint(HandFinger finger, int index)
        {
            switch (finger)
            {
                case HandFinger.Thumb: return (index == 0) ? TrackedHandJoint.Wrist : TrackedHandJoint.ThumbMetacarpalJoint + index - 1;
                case HandFinger.Index: return TrackedHandJoint.IndexMetacarpal + index;
                case HandFinger.Middle: return TrackedHandJoint.MiddleMetacarpal + index;
                case HandFinger.Ring: return TrackedHandJoint.RingMetacarpal + index;
                case HandFinger.Pinky: return TrackedHandJoint.PinkyMetacarpal + index;
                default: return TrackedHandJoint.None;
            }
        }

        #endregion Update data functions
    }
}