// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class InputAnimationRecordingUtils
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Record a keyframe at the given time for the main camera and tracked input devices.
        /// </summary>
        public static void RecordKeyframe(InputAnimation animation, float time, MixedRealityInputRecordingProfile profile)
        {
            RecordInputHandData(animation, time, Handedness.Left, profile);
            RecordInputHandData(animation, time, Handedness.Right, profile);
            if (CameraCache.Main)
            {
                var cameraPose = new MixedRealityPose(CameraCache.Main.transform.position, CameraCache.Main.transform.rotation);
                animation.AddCameraPoseKey(time, cameraPose, profile.CameraPositionThreshold, profile.CameraRotationThreshold);
            }
        }

        /// <summary>
        /// Record a keyframe at the given time for a hand with the given handedness it is tracked.
        /// </summary>
        public static bool RecordInputHandData(InputAnimation animation, float time, Handedness handedness, MixedRealityInputRecordingProfile profile)
        {
            var hand = HandJointUtils.FindHand(handedness);
            if (hand == null)
            {
                animation.AddHandStateKey(time, handedness, false, false);
                return false;
            }

            bool isTracked = (hand.TrackingState == TrackingState.Tracked);

            // Extract extra information from current interactions
            bool isPinching = false;
            for (int i = 0; i < hand.Interactions?.Length; i++)
            {
                var interaction = hand.Interactions[i];
                switch (interaction.InputType)
                {
                    case DeviceInputType.Select:
                        isPinching = interaction.BoolData;
                        break;
                }
            }

            animation.AddHandStateKey(time, handedness, isTracked, isPinching);

            if (isTracked)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    if (hand.TryGetJoint((TrackedHandJoint)i, out MixedRealityPose jointPose))
                    {
                        animation.AddHandJointKey(time, handedness, (TrackedHandJoint)i, jointPose, profile.JointPositionThreshold);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Serialize an animation curve as binary data.
        /// </summary>
        public static void SerializeAnimationCurve(BinaryWriter writer, AnimationCurve curve)
        {
            writer.Write((int)curve.preWrapMode);
            writer.Write((int)curve.postWrapMode);

            writer.Write(curve.length);
            for (int i = 0; i < curve.length; ++i)
            {
                var keyframe = curve.keys[i];
                writer.Write(keyframe.time);
                writer.Write(keyframe.value);
                writer.Write(keyframe.inTangent);
                writer.Write(keyframe.outTangent);
                writer.Write(keyframe.inWeight);
                writer.Write(keyframe.outWeight);
                writer.Write((int)keyframe.weightedMode);
            }
        }

        /// <summary>
        /// Deserialize an animation curve from binary data.
        /// </summary>
        public static void DeserializeAnimationCurve(BinaryReader reader, AnimationCurve curve)
        {
            curve.preWrapMode = (WrapMode)reader.ReadInt32();
            curve.postWrapMode = (WrapMode)reader.ReadInt32();

            int keyframeCount = reader.ReadInt32();

            Keyframe[] keys = new Keyframe[keyframeCount];
            for (int i = 0; i < keyframeCount; ++i)
            {
                keys[i].time = reader.ReadSingle();
                keys[i].value = reader.ReadSingle();
                keys[i].inTangent = reader.ReadSingle();
                keys[i].outTangent = reader.ReadSingle();
                keys[i].inWeight = reader.ReadSingle();
                keys[i].outWeight = reader.ReadSingle();
                keys[i].weightedMode = (WeightedMode)reader.ReadInt32();
            }

            curve.keys = keys;
        }
    }
}