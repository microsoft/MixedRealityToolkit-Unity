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

        private static IInputSimulationService inputSimService = null;
        private static IInputSimulationService InputSimService => inputSimService ?? (inputSimService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>());

        public static void RecordKeyframe(InputAnimation animation, float time)
        {
            RecordInputHandData(animation, time, Handedness.Left);
            RecordInputHandData(animation, time, Handedness.Right);
            if (CameraCache.Main)
            {
                var cameraPose = new MixedRealityPose(CameraCache.Main.transform.position, CameraCache.Main.transform.rotation);
                animation.AddCameraPoseKeys(time, cameraPose);
            }
        }

        public static bool RecordInputHandData(InputAnimation animation, float time, Handedness handedness)
        {
            var hand = HandJointUtils.FindHand(handedness);
            if (hand == null)
            {
                animation.AddHandStateKeys(time, handedness, false, false);
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

            animation.AddHandStateKeys(time, handedness, isTracked, isPinching);

            if (isTracked)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    if (hand.TryGetJoint((TrackedHandJoint)i, out MixedRealityPose jointPose))
                    {
                        animation.AddHandJointKeys(time, handedness, (TrackedHandJoint)i, jointPose.Position);
                    }
                }
            }

            return true;
        }

        public static void ApplyInputAnimation(InputAnimation animation, float time)
        {
            if (InputSimService != null)
            {
                InputSimService.UserInputEnabled = false;

                animation.EvaluateHandData(time, Handedness.Left, InputSimService.HandDataLeft);
                animation.EvaluateHandData(time, Handedness.Right, InputSimService.HandDataRight);

                if (CameraCache.Main)
                {
                    MixedRealityPose cameraPose = animation.EvaluateCameraPose(time);
                    CameraCache.Main.transform.SetPositionAndRotation(cameraPose.Position, cameraPose.Rotation);
                }
            }
        }
    }
}