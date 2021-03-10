// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;

#if LEAPMOTIONCORE_PRESENT
using Leap;
using Leap.Unity;
using Leap.Unity.Attachments;
using System;
using Unity.Profiling;
using UnityEngine;
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
        public LeapMotionArticulatedHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness))
        { }

        // Joint poses of the MRTK hand based on the leap hand data
        private readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        #region IMixedRealityHand Implementation

        /// <inheritdoc/>
        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose) => jointPoses.TryGetValue(joint, out pose);

        #endregion IMixedRealityHand Implementation

#if LEAPMOTIONCORE_PRESENT
        private ArticulatedHandDefinition handDefinition;
        internal ArticulatedHandDefinition HandDefinition => handDefinition ?? (handDefinition = Definition as ArticulatedHandDefinition);

        /// <summary>
        /// If true, the current joint pose supports far interaction via the default controller ray.  
        /// </summary>
        public override bool IsInPointingPose => HandDefinition.IsInPointingPose;

        /// <summary>
        /// If true, the hand is in air tap gesture, also called the pinch gesture.
        /// </summary>
        public bool IsPinching => HandDefinition.IsPinching;

        // Array of TrackedHandJoint names
        private static readonly TrackedHandJoint[] TrackedHandJointEnum = (TrackedHandJoint[])Enum.GetValues(typeof(TrackedHandJoint));

        // The leap AttachmentHand contains the joint poses for the current leap hand in frame. There is one AttachmentHand, either 
        // left or right, associated with a LeapMotionArticulatedHand.
        private AttachmentHand attachmentHand = null;

        // The leap service provider contains the joint data for a hand.  The provider's CurrentFrame.Hands is used to retrieve 
        // metacarpal joint poses each frame.
        private LeapServiceProvider leapServiceProvider = null;

        private List<TrackedHandJoint> metacarpals = new List<TrackedHandJoint>
        {
            TrackedHandJoint.ThumbMetacarpalJoint,
            TrackedHandJoint.IndexMetacarpal,
            TrackedHandJoint.MiddleMetacarpal,
            TrackedHandJoint.RingMetacarpal,
            TrackedHandJoint.PinkyMetacarpal
        };

        /// <summary>
        /// Set the Leap hands required for retrieving joint pose data.  A Leap AttachmentHand contains AttachmentPointFlags which are equivalent to 
        /// MRTK's TrackedHandJoint.  The Leap AttachmentHand contains all joint poses for a hand except the metacarpals.  The Leap Hand is 
        /// used to retrieve the metacarpal joint poses.
        /// </summary>
        internal void SetAttachmentHands(AttachmentHand attachmentHandLeap, LeapServiceProvider leapMotionServiceProvider)
        {
            // Set the leap attachment hand with the corresponding handedness
            attachmentHand = attachmentHandLeap;

            // Cache a reference to the leap service provider which is used to gather the metacarpal joint data each frame
            leapServiceProvider = leapMotionServiceProvider;
        }

        /// <summary>
        /// Adds the joint poses calculated from the Leap Motion Controller to the jointPoses Dictionary.
        /// </summary>
        private void SetJointPoses()
        {
            foreach (TrackedHandJoint joint in TrackedHandJointEnum)
            {
                if (attachmentHand != null && attachmentHand.isTracked)
                {
                    // Is the current joint a metacarpal
                    bool isMetacarpal = metacarpals.Contains(joint);

                    // AttachmentPointFlags does not include metacarpals.  
                    if (isMetacarpal)
                    {
                        MixedRealityPose metacarpalPose = GetMetacarpalPose(joint);

                        jointPoses[joint] = metacarpalPose;
                    }
                    else
                    {
                        AttachmentPointFlags leapAttachmentFlag = ConvertMRTKJointToLeapJoint(joint);

                        // Get the pose of the leap joint
                        AttachmentPointBehaviour leapJoint = attachmentHand.GetBehaviourForPoint(leapAttachmentFlag);

                        // Set the pose calculated by the leap motion to a mixed reality pose
                        MixedRealityPose pose = new MixedRealityPose(leapJoint.transform.position, leapJoint.transform.rotation);

                        jointPoses[joint] = pose;
                    }
                }
                else
                {
                    jointPoses[joint] = MixedRealityPose.ZeroIdentity;
                }
            }
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

            // Get the joint poses of the hand each frame
            // A reference to the leap Hand cannot be cached and needs to be retrieved each frame
            List<Hand> leapHandsInCurrentFrame = leapServiceProvider.CurrentFrame.Hands;

            foreach (Hand hand in leapHandsInCurrentFrame)
            {
                if ((hand.IsLeft && ControllerHandedness == Handedness.Left) ||
                    (hand.IsRight && ControllerHandedness == Handedness.Right))
                {
                    // Leap Motion thumb metacarpal is stored at index 1
                    int boneIndex = (metacarpalJoint == TrackedHandJoint.ThumbMetacarpalJoint) ? 1 : 0;
                    Vector3 position = hand.Fingers[metacarpalIndex].bones[boneIndex].PrevJoint.ToVector3();
                    Quaternion rotation = hand.Fingers[metacarpalIndex].bones[boneIndex].Rotation.ToQuaternion();

                    return new MixedRealityPose(position, rotation);
                }
            }

            return MixedRealityPose.ZeroIdentity;
        }

        /// <summary>
        /// Converts a TrackedHandJoint to a Leap AttachmentPointFlag. An AttachmentPointFlag is Leap's version of MRTK's TrackedHandJoint.
        /// </summary>
        /// <param name="joint">TrackedHandJoint to be mapped to a Leap AttachmentPointFlag</param>
        /// <returns>Leap Motion AttachmentPointFlag pose</returns>
        static internal AttachmentPointFlags ConvertMRTKJointToLeapJoint(TrackedHandJoint joint)
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

        private static readonly ProfilerMarker UpdateStatePerfMarker = new ProfilerMarker("[MRTK] LeapMotionArticulatedHand.UpdateState");

        /// <summary>
        /// Updates the joint poses and interactions for the articulated hand.
        /// </summary>
        public void UpdateState()
        {
            using (UpdateStatePerfMarker.Auto())
            {
                // Get and set the joint poses provided by the Leap Motion Controller 
                SetJointPoses();

                // Update hand joints and raise event via handDefinition
                HandDefinition?.UpdateHandJoints(jointPoses);

                UpdateInteractions();

                UpdateVelocity();
            }
        }

        /// <summary>
        /// Updates the visibility of the hand ray and raises input system events based on joint pose data.
        /// </summary>
        protected void UpdateInteractions()
        {
            MixedRealityPose pointerPose = jointPoses[TrackedHandJoint.Palm];
            MixedRealityPose gripPose = jointPoses[TrackedHandJoint.Palm];
            MixedRealityPose indexPose = jointPoses[TrackedHandJoint.IndexTip];

            // Only update the hand ray if the hand is in pointing pose
            if (IsInPointingPose)
            {
                HandRay.Update(pointerPose.Position, GetPalmNormal(), CameraCache.Main.transform, ControllerHandedness);
                Ray ray = HandRay.Ray;

                pointerPose.Position = ray.origin;
                pointerPose.Rotation = Quaternion.LookRotation(ray.direction);
            }
        
            CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, gripPose);

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
                        HandDefinition?.UpdateCurrentIndexPose(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                        HandDefinition?.UpdateCurrentTeleportPose(Interactions[i]);
                        break;
                }
            }
        }
#endif
    }
}
