// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class PlayModeTestUtilities
    {
        public static SimulatedHandData.HandJointDataGenerator GenerateHandPose(SimulatedHandPose.GestureId gesture, Handedness handedness, Vector3 screenPosition)
        {
            return (jointsOut) =>
            {
                SimulatedHandPose gesturePose = SimulatedHandPose.GetGesturePose(gesture);
                Quaternion rotation = Quaternion.identity;
                Vector3 position = CameraCache.Main.ScreenToWorldPoint(screenPosition);
                gesturePose.ComputeJointPositions(handedness, rotation, position, jointsOut);
            };
        }
    }
}
