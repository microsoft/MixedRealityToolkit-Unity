// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class InputAnimationUtils
    {
        public static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public static void RecordKeyframe(InputAnimation animation, ulong frame)
        {
            var keyframe = new InputKeyframe();

            RecordInputHandData(Handedness.Left, keyframe.HandDataLeft);
            RecordInputHandData(Handedness.Right, keyframe.HandDataRight);
            if (CameraCache.Main)
            {
                keyframe.CameraPose = new MixedRealityPose(CameraCache.Main.transform.position, CameraCache.Main.transform.rotation);
            }

            animation.InsertKeyframe(keyframe, frame);
        }

        public static bool RecordKeyframeFiltered(InputAnimation animation, double time, float epsilonFrame, float epsilonJointPositions, float epsilonCameraPosition, float epsilonCameraRotation)
        {
            // Reject if keyframes exist too close to current time
            animation.FindKeyframeInterval(time, out InputKeyframe low, out double lowTime, out InputKeyframe high, out double highTime);
            if (low != null && Math.Abs(lowTime - time) <= epsilonFrame)
            {
                return false;
            }
            if (high != null && Math.Abs(highTime - time) <= epsilonFrame)
            {
                return false;
            }

            var keyframe = new InputKeyframe();
            RecordInputHandData(Handedness.Left, keyframe.HandDataLeft);
            RecordInputHandData(Handedness.Right, keyframe.HandDataRight);
            if (CameraCache.Main)
            {
                keyframe.CameraPose = new MixedRealityPose(CameraCache.Main.transform.position, CameraCache.Main.transform.rotation);
            }

            // Reject if data hasn't changed sufficiently
            bool dataChanged = false;
            if (low == null && high == null)
            {
                // First keyframe, always accept
                dataChanged = true;
            }
            if (low != null)
            {
                dataChanged |= IsHandDataChanged(low.HandDataLeft, keyframe.HandDataLeft, epsilonJointPositions);
                dataChanged |= IsHandDataChanged(low.HandDataRight, keyframe.HandDataRight, epsilonJointPositions);
                dataChanged |= IsCameraDataChanged(low.CameraPose, keyframe.CameraPose, epsilonCameraPosition, epsilonCameraRotation);
            }
            if (high != null)
            {
                dataChanged |= IsHandDataChanged(high.HandDataLeft, keyframe.HandDataLeft, epsilonJointPositions);
                dataChanged |= IsHandDataChanged(high.HandDataRight, keyframe.HandDataRight, epsilonJointPositions);
                dataChanged |= IsCameraDataChanged(high.CameraPose, keyframe.CameraPose, epsilonCameraPosition, epsilonCameraRotation);
            }

            if (!dataChanged)
            {
                return false;
            }

            animation.InsertKeyframe(keyframe, time);
            return true;
        }

        private static bool IsHandDataChanged(SimulatedHandData a, SimulatedHandData b, float epsilonJointPositions)
        {
            if (a.IsTracked != b.IsTracked || a.IsPinching != b.IsPinching)
            {
                return true;
            }

            if (a.IsTracked && b.IsTracked)
            {
                float sqrEpsilon = epsilonJointPositions * epsilonJointPositions;
                for (int i = 0; i < jointCount; ++i)
                {
                    if ((a.Joints[i] - b.Joints[i]).sqrMagnitude >= sqrEpsilon)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsCameraDataChanged(MixedRealityPose a, MixedRealityPose b, float epsilonPosition, float epsilonRotation)
        {
            float sqrEpsilonPosition = epsilonPosition * epsilonPosition;
            if ((a.Position - b.Position).sqrMagnitude >= epsilonPosition)
            {
                return true;
            }

            (Quaternion.Inverse(a.Rotation) * b.Rotation).ToAngleAxis(out float angle, out Vector3 axis);
            if (Mathf.Abs(angle) > epsilonRotation)
            {
                return true;
            }

            return false;
        }

        public static bool RecordInputHandData(Handedness handedness, SimulatedHandData handData)
        {
            var hand = HandJointUtils.FindHand(handedness);
            if (hand == null)
            {
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

            handData.Update(isTracked, isPinching,
                (Vector3[] jointPositions) =>
                {
                    for (int i = 0; i < jointCount; ++i)
                    {
                        hand.TryGetJoint((TrackedHandJoint)i, out MixedRealityPose jointPose);
                        jointPositions[i] = jointPose.Position;
                    }
                });

            return true;
        }

        public static void ApplyInputAnimation(InputAnimation animation, double time)
        {
            var inputSimService = MixedRealityToolkit.Instance.GetService<InputSimulationService>();
            if (inputSimService != null)
            {
                inputSimService.UserInputEnabled = false;

                animation.Interpolate(time, inputSimService.HandDataLeft, inputSimService.HandDataRight, out MixedRealityPose cameraPose);

                if (CameraCache.Main)
                {
                    CameraCache.Main.transform.SetPositionAndRotation(cameraPose.Position, cameraPose.Rotation);
                }
            }
        }
    }
}