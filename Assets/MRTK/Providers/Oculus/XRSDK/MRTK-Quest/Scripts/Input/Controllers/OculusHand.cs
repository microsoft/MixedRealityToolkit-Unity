//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;

#if OCULUSINTEGRATION_PRESENT
using static OVRSkeleton;

using Microsoft.MixedReality.Toolkit;
using UnityEngine;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Input
{
    /// <summary>
    /// Oculus Integration Asset package implementation of Oculus Quest articulated hands.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right },
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class OculusHand : BaseHand
    {
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Pose used by hand ray
        /// </summary>
        public MixedRealityPose HandPointerPose => currentPointerPose;

#if OCULUSINTEGRATION_PRESENT
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;
        private bool isIndexGrabbing = false;
        private bool isMiddleGrabbing = false;
        private OculusXRSDKDeviceManagerProfile settingsProfile;
        private MixedRealityHandTrackingProfile handTrackingProfile;
#endif

        /// <summary>
        /// Default constructor used by reflection for profiles
        /// </summary>
        public OculusHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness))
        { }

        public override void SetupDefaultInteractions()
        {
            AssignControllerMappings(DefaultInteractions);
        }

        #region IMixedRealityHand Implementation

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        /// <inheritdoc/>
        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            return jointPoses.TryGetValue(joint, out pose);
        }

        #endregion IMixedRealityHand Implementation

#if OCULUSINTEGRATION_PRESENT
        private ArticulatedHandDefinition handDefinition;
        private ArticulatedHandDefinition HandDefinition => handDefinition ?? (handDefinition = Definition as ArticulatedHandDefinition);

        public void InitializeHand(OVRHand ovrHand, OculusXRSDKDeviceManagerProfile deviceManagerSettings)
        {
            settingsProfile = deviceManagerSettings;
            handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile.HandTrackingProfile;
        }

        /// <inheritdoc/>
        public override bool IsInPointingPose => HandDefinition.IsInPointingPose;

        protected bool IsPinching { set; get; }

        // Pinch was also used as grab, we want to allow hand-curl grab not just pinch.
        // Determine pinch and grab separately
        protected bool IsGrabbing { set; get; }

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateController(OVRHand hand, OVRSkeleton ovrSkeleton, Transform trackingOrigin)
        {
            if (!Enabled || hand == null || ovrSkeleton == null)
            {
                return;
            }

            bool isTracked = UpdateHandData(hand, ovrSkeleton);
            IsPositionAvailable = IsRotationAvailable = isTracked;

            if (isTracked)
            {
                // Leverage Oculus Platform Hand Ray - instead of simulating it in a crummy way
                currentPointerPose.Position = trackingOrigin.TransformPoint(hand.PointerPose.position);

                Vector3 pointerForward = trackingOrigin.TransformDirection(hand.PointerPose.forward);
                Vector3 pointerUp = trackingOrigin.TransformDirection(hand.PointerPose.up);

                currentPointerPose.Rotation = Quaternion.LookRotation(pointerForward, pointerUp);

                currentGripPose = jointPoses[TrackedHandJoint.Palm];
                CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentGripPose);

                UpdateVelocity();
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        Interactions[i].PoseData = currentPointerPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPointerPose);
                        }
                        break;
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = currentGripPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentGripPose);
                        }
                        break;
                    case DeviceInputType.Select:
                        Interactions[i].BoolData = IsPinching || IsGrabbing;

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
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.GripPress:
                        Interactions[i].BoolData = IsPinching || IsGrabbing;

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
                        HandDefinition.UpdateCurrentIndexPose(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                        HandDefinition.UpdateCurrentTeleportPose(Interactions[i]);
                        break;
                }
            }
        }

        #region HandJoints
        protected readonly Dictionary<BoneId, TrackedHandJoint> boneJointMapping = new Dictionary<BoneId, TrackedHandJoint>()
        {
            { BoneId.Hand_Thumb1, TrackedHandJoint.ThumbMetacarpalJoint },
            { BoneId.Hand_Thumb2, TrackedHandJoint.ThumbProximalJoint },
            { BoneId.Hand_Thumb3, TrackedHandJoint.ThumbDistalJoint },
            { BoneId.Hand_ThumbTip, TrackedHandJoint.ThumbTip },
            { BoneId.Hand_Index1, TrackedHandJoint.IndexKnuckle },
            { BoneId.Hand_Index2, TrackedHandJoint.IndexMiddleJoint },
            { BoneId.Hand_Index3, TrackedHandJoint.IndexDistalJoint },
            { BoneId.Hand_IndexTip, TrackedHandJoint.IndexTip },
            { BoneId.Hand_Middle1, TrackedHandJoint.MiddleKnuckle },
            { BoneId.Hand_Middle2, TrackedHandJoint.MiddleMiddleJoint },
            { BoneId.Hand_Middle3, TrackedHandJoint.MiddleDistalJoint },
            { BoneId.Hand_MiddleTip, TrackedHandJoint.MiddleTip },
            { BoneId.Hand_Ring1, TrackedHandJoint.RingKnuckle },
            { BoneId.Hand_Ring2, TrackedHandJoint.RingMiddleJoint },
            { BoneId.Hand_Ring3, TrackedHandJoint.RingDistalJoint },
            { BoneId.Hand_RingTip, TrackedHandJoint.RingTip },
            { BoneId.Hand_Pinky1, TrackedHandJoint.PinkyKnuckle },
            { BoneId.Hand_Pinky2, TrackedHandJoint.PinkyMiddleJoint },
            { BoneId.Hand_Pinky3, TrackedHandJoint.PinkyDistalJoint },
            { BoneId.Hand_PinkyTip, TrackedHandJoint.PinkyTip },
            { BoneId.Hand_WristRoot, TrackedHandJoint.Wrist },
        };

        private float _lastHighConfidenceTime = 0f;
        protected bool UpdateHandData(OVRHand ovrHand, OVRSkeleton ovrSkeleton)
        {
            bool isTracked = ovrHand.IsTracked;
            if (ovrHand.HandConfidence == OVRHand.TrackingConfidence.High)
            {
                _lastHighConfidenceTime = Time.unscaledTime;
            }
            if (ovrHand.HandConfidence == OVRHand.TrackingConfidence.Low)
            {
                if (settingsProfile.MinimumHandConfidence == OVRHand.TrackingConfidence.High)
                {
                    isTracked = false;
                }
                else
                {
                    float lowConfidenceTime = Time.time - _lastHighConfidenceTime;
                    if (settingsProfile.LowConfidenceTimeThreshold > 0 &&
                        settingsProfile.LowConfidenceTimeThreshold < lowConfidenceTime)
                    {
                        isTracked = false;
                    }
                }
            }

            if (ControllerHandedness == Handedness.Left)
            {
                settingsProfile.CurrentLeftHandTrackingConfidence = ovrHand.HandConfidence;
            }
            else
            {
                settingsProfile.CurrentRightHandTrackingConfidence = ovrHand.HandConfidence;
            }

            if (ovrSkeleton != null)
            {
                var bones = ovrSkeleton.Bones;
                foreach (var bone in bones)
                {
                    UpdateBone(bone);
                }

                UpdatePalm();
            }

            HandDefinition?.UpdateHandJoints(jointPoses);

            // Note: After some testing, it seems when moving your hand fast, Oculus's pinch estimation data gets frozen, which leads to stuck pinches.
            // To counter this, we perform a distance check between thumb and index to determine if we should force the pinch to a false state.
            float pinchStrength = HandPoseUtils.CalculateIndexPinch(ControllerHandedness);
            if (pinchStrength == 0.0f)
            {
                IsPinching = false;
            }
            else
            {
                if (IsPinching)
                {
                    // If we are already pinching, we make the pinch a bit sticky
                    IsPinching = pinchStrength > 0.5f;
                }
                else
                {
                    // If not yet pinching, only consider pinching if finger confidence is high
                    IsPinching = pinchStrength > 0.85f && ovrHand.GetFingerConfidence(OVRHand.HandFinger.Index) == OVRHand.TrackingConfidence.High;
                }
            }

            isIndexGrabbing = HandPoseUtils.IsIndexGrabbing(ControllerHandedness);
            isMiddleGrabbing = HandPoseUtils.IsMiddleGrabbing(ControllerHandedness);


            // Pinch was also used as grab, we want to allow hand-curl grab not just pinch.
            // Determine pinch and grab separately
            if (isTracked)
            {
                IsGrabbing = isIndexGrabbing && isMiddleGrabbing;
            }

            return isTracked;
        }

        // 4 cm is the treshold for fingers being far apart.
        // 0.0016 is the square magnitude equivalent
        // Square magnitude is less expensive to perform than a distance check
        private const float IndexThumbSqrMagnitudeThreshold = 0.0016f;
        private float IndexThumbSqrMagnitude()
        {
            MixedRealityPose indexPose = MixedRealityPose.ZeroIdentity;
            TryGetJoint(TrackedHandJoint.IndexTip, out indexPose);

            MixedRealityPose thumbPose = MixedRealityPose.ZeroIdentity;
            TryGetJoint(TrackedHandJoint.ThumbTip, out thumbPose);

            Vector3 distanceVector = indexPose.Position - thumbPose.Position;
            return distanceVector.sqrMagnitude;
        }

        protected void UpdateBone(OVRBone bone)
        {
            var boneId = bone.Id;
            var boneTransform = bone.Transform;

            if (boneJointMapping.TryGetValue(boneId, out var joint))
            {
                Quaternion boneRotation = bone.Transform.rotation;

                // WARNING THIS CODE IS SUBJECT TO CHANGE WITH THE OCULUS SDK - This fix is a hack to fix broken and inconsistent rotations for hands
                if (ControllerHandedness == Handedness.Left)
                {
                    // Rotate palm 180 on X to flip up
                    boneRotation *= Quaternion.Euler(180f, 0f, 0f);

                    // Rotate palm 90 degrees on y to align x with right
                    boneRotation *= Quaternion.Euler(0f, -90, 0f);
                }
                else
                {
                    // Right Up direction is correct

                    // Rotate palm 90 degrees on y to align x with right
                    boneRotation *= Quaternion.Euler(0f, 90f, 0f);
                }

                UpdateJointPose(joint, boneTransform.position, boneRotation);
            }
        }

        protected void UpdatePalm()
        {
            bool hasMiddleKnuckle = TryGetJoint(TrackedHandJoint.MiddleKnuckle, out var middleKnucklePose);
            bool hasWrist = TryGetJoint(TrackedHandJoint.Wrist, out var wristPose);

            if (hasMiddleKnuckle && hasWrist)
            {
                Vector3 wristRootPosition = wristPose.Position;
                Vector3 middle3Position = middleKnucklePose.Position;

                Vector3 palmPosition = Vector3.Lerp(wristRootPosition, middle3Position, 0.5f);
                Quaternion palmRotation = wristPose.Rotation;

                UpdateJointPose(TrackedHandJoint.Palm, palmPosition, palmRotation);
            }
        }

        protected void UpdateJointPose(TrackedHandJoint joint, Vector3 position, Quaternion rotation)
        {
            // TODO Figure out kalman filter coefficients to get good quality smoothing
#if LATER
            if (joint == TrackedHandJoint.IndexTip)
            {
                jointPosition = indexTipFilter.Update(position);
            }
            else if (joint == TrackedHandJoint.Palm)
            {
                jointPosition = palmFilter.Update(position);
            }
#endif

            MixedRealityPose pose = new MixedRealityPose(position, rotation);
            if (!jointPoses.ContainsKey(joint))
            {
                jointPoses.Add(joint, pose);
            }
            else
            {
                jointPoses[joint] = pose;
            }
        }
        #endregion
#endif
    }
}
