// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;

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
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
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

#if LEAPMOTIONCORE_PRESENT

        protected LeapMotionDeviceManager LeapMotionDeviceManager => CoreServices.GetInputSystemDataProvider<LeapMotionDeviceManager>();

        protected AttachmentHands AttachmentHands => LeapMotionDeviceManager.attachmentHands;

        // Attachment hand needed for this hand, left or right hand
        private AttachmentHand attachmentHand = null;

        // Number of joints in an MRTK Tracked Hand
        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        // Joint poses of the MRTK hand based on the leap hand data
        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        private TrackedHandJoint[] metacarpals = new TrackedHandJoint[]
        {
            TrackedHandJoint.ThumbMetacarpalJoint,
            TrackedHandJoint.IndexMetacarpal,
            TrackedHandJoint.MiddleMetacarpal,
            TrackedHandJoint.RingMetacarpal,
            TrackedHandJoint.PinkyMetacarpal
        };

        /// <summary>
        /// If true, the current joint pose supports far interaction via the default controller ray.  
        /// </summary>
        public override bool IsInPointingPose => handDefinition.IsInPointingPose;

        /// <summary>
        /// If true, the hand is in air tap gesture, also called the pinch gesture
        /// </summary>
        public bool IsPinching => handDefinition.IsPinching;

        private void SetAttachmentHands()
        {
            // Only get the attachment hands if the application is playing. The attachment hands are added at runtime and do not exist in edit mode.
            if (Application.isPlaying)
            {
                // Get the attachment hand data based on controller handedness
                attachmentHand = Array.Find(AttachmentHands.attachmentHands, (hand => HandChiralityToHandedness(hand.chirality) == ControllerHandedness));

                // Enable all attachment point flags in the leap hand. By default, only the wrist and the palm are enabled.
                foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                {
                    AttachmentHands.attachmentPoints |= ConvertMRTKJointToLeapJoint(joint);
                }            
            }
        }
#endif
        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
#if LEAPMOTIONCORE_PRESENT
            if (attachmentHand != null && attachmentHand.isTracked)
            {
                // Is the current joint a metacarpal
                bool isMetacarpal = Array.Exists(metacarpals, (metacarpalJoint => metacarpalJoint == joint));

                // AttachmentPointFlags does not include metacarpals.  
                if (isMetacarpal) 
                {
                    MixedRealityPose metacarpalPose = GetMetacarpals(joint);

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
#if LEAPMOTIONCORE_PRESENT

        // Convert leap hand chirality to MRTK handedness
        private Handedness HandChiralityToHandedness(Chirality chirality)
        {
            return (chirality == Chirality.Left) ? Handedness.Left : Handedness.Right;
        }

        /// <summary>
        /// Get the pose of the metacarpal joints from the current frame of the LeapServiceProvider because the metacarpal joints 
        /// are not included in AttachmentPointFlags.  The metacarpal joints are those located directly above the wrist.
        /// </summary>
        /// <param name="metacarpalJoint"></param>
        /// <returns>The MixedRealityPose for the leap metacarpal joint</returns>
        private MixedRealityPose GetMetacarpals(TrackedHandJoint metacarpalJoint)
        {
            // Get the leap hand data in the current frame
            List<Hand> hands = LeapMotionDeviceManager.CurrentHandsDetectedByLeap;

            Hand leapHand;

            if (ControllerHandedness == Handedness.Left)
            {
                leapHand = hands.Find(hand => hand.IsLeft);
            }
            else
            {
                leapHand = hands.Find(hand => hand.IsRight);
            }

            int metacarpalIndex = Array.IndexOf(metacarpals, metacarpalJoint);

            // Get the pose of the metacarpal 
            Vector3 position = leapHand.Fingers[metacarpalIndex].bones[0].PrevJoint.ToVector3();
            Quaternion rotation = leapHand.Fingers[metacarpalIndex].bones[0].Rotation.ToQuaternion();

            return new MixedRealityPose(position, rotation);
        }

        /// <summary>
        /// Converts a TrackedHandJoint to a Leap AttachmentPointFlag. An AttachmentPointFlag is Leap's version of MRTK's TrackedHandJoint.
        /// </summary>
        /// <param name="joint">TrackedHandJoint to be mapped to a Leap AttachmentPointFlag</param>
        /// <returns></returns>
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
            for (int i = 0; i < jointCount; i++)
            {
                TrackedHandJoint handJoint = (TrackedHandJoint)i;
                TryGetJoint(handJoint, out MixedRealityPose pose);

                if (!jointPoses.ContainsKey(handJoint))
                {
                    jointPoses.Add(handJoint, pose);
                }
                else
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
                            // Update the index pose in hand definition 
                            handDefinition?.UpdateCurrentIndexPose(Interactions[i]);
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, indexPose);
                        }
                        break;
                }
            }
        }
#endif
    }

}
