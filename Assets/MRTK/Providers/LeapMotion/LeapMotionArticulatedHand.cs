// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;

#if LEAPMOTIONCORE_PRESENT
using Leap.Unity.Attachments;
using Leap.Unity;
using Leap;
#endif

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right })]
    /// <summary>
    /// Class that represents one Leap Motion Articulated Hand.
    /// </summary>
    public class LeapMotionArticulatedHand : BaseHand
    {
        /// <summary>
        /// Constructor for a Leap Motion Articulated Hand
        /// </summary>
        /// <param name="trackingState">Tracking state for the controller</param>
        /// <param name="controllerHandedness">Handedness of this controller (Left or Right)</param>
        /// <param name="inputSource">The origin of user input for this controller</param>
        /// <param name="interactions">The controller interaction map between physical inputs and the logical representation in MRTK</param>
        public LeapMotionArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null) 
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            handDefinition = new ArticulatedHandDefinition(inputSource, controllerHandedness);
#if LEAPMOTIONCORE_PRESENT
            SetAttachmentHands();
#endif
        }

        private readonly ArticulatedHandDefinition handDefinition;

        // Set the interations for each hand to the Default interactions of the hand definition
        public override MixedRealityInteractionMapping[] DefaultInteractions => handDefinition?.DefaultInteractions;

        private static readonly ProfilerMarker TryGetJointPerfMarker = new ProfilerMarker("[MRTK] LeapMotionArticulatedHand.TryGetJoint");

#if LEAPMOTIONCORE_PRESENT

        /// <summary>
        /// If true, the current joint pose supports far interaction via the default controller ray.  
        /// </summary>
        public override bool IsInPointingPose => handDefinition.IsInPointingPose;

        /// <summary>
        /// If true, the hand is in air tap gesture, also called the pinch gesture.
        /// </summary>
        public bool IsPinching => handDefinition.IsPinching;

        protected LeapMotionDeviceManager LeapMotionDeviceManager => CoreServices.GetInputSystemDataProvider<LeapMotionDeviceManager>();

        // Array of TrackedHandJoint names
        protected static readonly TrackedHandJoint[] TrackedHandJointEnum = (TrackedHandJoint[])Enum.GetValues(typeof(TrackedHandJoint));

        // Joint poses of the MRTK hand based on the leap hand data
        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        protected AttachmentHands AttachmentHands => LeapMotionDeviceManager.LeapAttachmentHands;

        // The leap AttachmentHand contains the joint poses for the current leap hand in frame. There is one AttachmentHand, either 
        // left or right, associated with a LeapMotionArticulatedHand.
        private AttachmentHand attachmentHand = null;

        // Fully tracked leap motion hand data, used to retrieve the metacarpal joint poses.
        private Hand leapMotionMetacarpalHand = null;

        private List<TrackedHandJoint> metacarpals = new List<TrackedHandJoint>
        {
            TrackedHandJoint.ThumbMetacarpalJoint,
            TrackedHandJoint.IndexMetacarpal,
            TrackedHandJoint.MiddleMetacarpal,
            TrackedHandJoint.RingMetacarpal,
            TrackedHandJoint.PinkyMetacarpal
        };

        /// <summary>
        /// Set the Leap hands required for retrieving joint pose data.  A Leap AttachmentHand contain AttachmentPointFlags which are equivalent to 
        /// MRTK's TrackedHandJoint.  The Leap AttachmentHand contains all joint poses for a hand except the metacarpals.  The Leap Hand is 
        /// used to retrieve the metacarpal joint poses.
        /// </summary>
        private void SetAttachmentHands()
        {
            // Only get the attachment hands if the application is playing. The attachment hands are added at runtime and do not exist in edit mode.
            if (Application.isPlaying)
            {
                if (ControllerHandedness == Handedness.Left)
                {
                    attachmentHand = AttachmentHands.attachmentHands[0];
                }
                else
                {
                    attachmentHand = AttachmentHands.attachmentHands[1];
                }

                // Enable all attachment point flags in the leap hand. By default, only the wrist and the palm are enabled.
                foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                {
                    AttachmentHands.attachmentPoints |= ConvertMRTKJointToLeapJoint(joint);
                }

                // Set the fully tracked leapMotionMetacarpalHand with the correct handedness for metacarpal joint pose data.
                foreach (var hand in LeapMotionDeviceManager.CurrentHandsDetectedByLeap)
                {
                    if (ControllerHandedness == Handedness.Left && hand.IsLeft)
                    {
                        leapMotionMetacarpalHand = hand;
                    }
                    else if (ControllerHandedness == Handedness.Right && hand.IsRight)
                    {
                        leapMotionMetacarpalHand = hand;
                    }
                }
            }
        }
#endif

        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            using (TryGetJointPerfMarker.Auto())
            {
#if LEAPMOTIONCORE_PRESENT
                if (attachmentHand != null && attachmentHand.isTracked)
                {
                    // Is the current joint a metacarpal
                    bool isMetacarpal = metacarpals.Contains(joint);

                    // AttachmentPointFlags does not include metacarpals.  
                    if (isMetacarpal)
                    {
                        MixedRealityPose metacarpalPose = GetMetacarpalPose(joint);

                        pose = metacarpalPose;
                        return true;
                    }

                    AttachmentPointFlags leapAttachmentFlag = ConvertMRTKJointToLeapJoint(joint);

                    // Get the pose of the leap joint
                    AttachmentPointBehaviour leapJoint = attachmentHand.GetBehaviourForPoint(leapAttachmentFlag);

                    // Set the pose calculated by the leap motion to a mixed reality pose
                    pose = new MixedRealityPose(leapJoint.transform.position, leapJoint.transform.rotation);
                    return true;
                }
#endif
                pose = MixedRealityPose.ZeroIdentity;
                return false;
            }
        }

#if LEAPMOTIONCORE_PRESENT

        /// <summary>
        /// Convert leap hand chirality to MRTK handedness
        /// </summary>
        /// <param name="chirality">Leap hand Chirality (Left or Right)</param>
        /// <returns>MRTK Handedness given Leap Hand Chirality</returns>
        public static Handedness HandChiralityToHandedness(Chirality chirality)
        {
            return (chirality == Chirality.Left) ? Handedness.Left : Handedness.Right;
        }

        /// <summary>
        /// Get the pose of the metacarpal joints from the current frame of the LeapServiceProvider because the metacarpal joints 
        /// are not included in AttachmentPointFlags.  The metacarpal joints are those located directly above the wrist.
        /// </summary>
        /// <param name="metacarpalJoint">A metacarpal TrackedHandJoint</param>
        /// <returns>The MixedRealityPose for the leap metacarpal joint</returns>
        private MixedRealityPose GetMetacarpalPose(TrackedHandJoint metacarpalJoint)
        {
            int metacarpalIndex = metacarpals.IndexOf(metacarpalJoint);

            // Get the pose of the metacarpal 
            Vector3 position = leapMotionMetacarpalHand.Fingers[metacarpalIndex].bones[0].PrevJoint.ToVector3();
            Quaternion rotation = leapMotionMetacarpalHand.Fingers[metacarpalIndex].bones[0].Rotation.ToQuaternion();

            return new MixedRealityPose(position, rotation);
        }

        /// <summary>
        /// Converts a TrackedHandJoint to a Leap AttachmentPointFlag. An AttachmentPointFlag is Leap's version of MRTK's TrackedHandJoint.
        /// </summary>
        /// <param name="joint">TrackedHandJoint to be mapped to a Leap AttachmentPointFlag</param>
        /// <returns>Leap Motion AttachmentPointFlag pose</returns>
        private AttachmentPointFlags ConvertMRTKJointToLeapJoint(TrackedHandJoint joint)
        {
            switch (joint)
            {
                case TrackedHandJoint.Palm: return AttachmentPointFlags.Palm;
                case TrackedHandJoint.Wrist: return AttachmentPointFlags.Wrist;

                case TrackedHandJoint.ThumbProximalJoint: return AttachmentPointFlags.ThumbProximalJoint;
                case TrackedHandJoint.ThumbDistalJoint: return AttachmentPointFlags.ThumbDistalJoint;
                case TrackedHandJoint.ThumbTip: return AttachmentPointFlags.ThumbTip;

                case TrackedHandJoint.IndexKnuckle: return AttachmentPointFlags.IndexKnuckle;
                case TrackedHandJoint.IndexMiddleJoint: return AttachmentPointFlags.IndexMiddleJoint;
                case TrackedHandJoint.IndexDistalJoint: return AttachmentPointFlags.IndexDistalJoint;
                case TrackedHandJoint.IndexTip: return AttachmentPointFlags.IndexTip;

                case TrackedHandJoint.MiddleKnuckle: return AttachmentPointFlags.MiddleKnuckle;
                case TrackedHandJoint.MiddleMiddleJoint: return AttachmentPointFlags.MiddleMiddleJoint;
                case TrackedHandJoint.MiddleDistalJoint: return AttachmentPointFlags.MiddleDistalJoint;
                case TrackedHandJoint.MiddleTip: return AttachmentPointFlags.MiddleTip;

                case TrackedHandJoint.RingKnuckle: return AttachmentPointFlags.RingKnuckle;
                case TrackedHandJoint.RingMiddleJoint: return AttachmentPointFlags.RingMiddleJoint;
                case TrackedHandJoint.RingDistalJoint: return AttachmentPointFlags.RingDistalJoint;
                case TrackedHandJoint.RingTip: return AttachmentPointFlags.RingTip;

                case TrackedHandJoint.PinkyKnuckle: return AttachmentPointFlags.PinkyKnuckle;
                case TrackedHandJoint.PinkyMiddleJoint: return AttachmentPointFlags.PinkyMiddleJoint;
                case TrackedHandJoint.PinkyDistalJoint: return AttachmentPointFlags.PinkyDistalJoint;
                case TrackedHandJoint.PinkyTip: return AttachmentPointFlags.PinkyTip;

                // Metacarpals are not included in AttachmentPointFlags
                default: return AttachmentPointFlags.Wrist;
            }
        }

        public void UpdateState()
        {
            // Populate the jointPoses 
            foreach (TrackedHandJoint handJoint in TrackedHandJointEnum)
            {
                if (TryGetJoint(handJoint, out MixedRealityPose pose))
                {
                    jointPoses[handJoint] = pose;
                }
            }

            // Update hand joints and raise event via handDefinition
            handDefinition?.UpdateHandJoints(jointPoses);

            UpdateInteractions();

            UpdateVelocity();
        }

        protected void UpdateInteractions()
        {
            MixedRealityPose pointerPose = jointPoses[TrackedHandJoint.Palm];
            MixedRealityPose gripPose = jointPoses[TrackedHandJoint.Palm];
            MixedRealityPose indexPose = jointPoses[TrackedHandJoint.IndexTip];

            // Only update the hand ray if the hand is in pointing pose
            if (handDefinition.IsInPointingPose)
            {
                HandRay.Update(pointerPose.Position, GetPalmNormal(), CameraCache.Main.transform, ControllerHandedness);
                Ray ray = HandRay.Ray;

                pointerPose.Position = ray.origin;
                pointerPose.Rotation = Quaternion.LookRotation(ray.direction);
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        Interactions[i].PoseData = pointerPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, pointerPose);
                        }
                        break;
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = gripPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, gripPose);
                        }
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.TriggerPress:
                        Interactions[i].BoolData = IsPinching;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.IndexFinger:
                        Interactions[i].PoseData = indexPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, indexPose);
                        }
                        break;
                }
            }
        }
#endif
    }

}
