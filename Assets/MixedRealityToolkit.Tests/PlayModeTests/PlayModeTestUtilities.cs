// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using NUnit.Framework;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class PlayModeTestUtilities
    {
        public static SimulatedHandData.HandJointDataGenerator GenerateHandPose(ArticulatedHandPose.GestureId gesture, Handedness handedness, Vector3 worldPosition)
        {
            return (jointsOut) =>
            {
                ArticulatedHandPose gesturePose = ArticulatedHandPose.GetGesturePose(gesture);
                Quaternion rotation = Quaternion.identity;
                gesturePose.ComputeJointPoses(handedness, rotation, worldPosition, jointsOut);
            };
        }

        public static IMixedRealityInputSystem GetInputSystem()
        {
            IMixedRealityInputSystem inputSystem;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);
            Assert.IsNotNull(inputSystem, "MixedRealityInputSystem is null!");
            return inputSystem;
        }

        public static InputSimulationService GetInputSimulationService()
        {
            IMixedRealityInputSystem inputSystem = GetInputSystem();
            InputSimulationService inputSimulationService = (inputSystem as IMixedRealityDataProviderAccess).GetDataProvider<InputSimulationService>();
            Assert.IsNotNull(inputSimulationService, "InputSimulationService is null!");
            inputSimulationService.UserInputEnabled = false;

            return inputSimulationService;
        }

        internal static IEnumerator MoveHandFromTo(InputSimulationService inputSimulationService, Vector3 startPos, Vector3 endPos, int numSteps, ArticulatedHandPose.GestureId gestureId, Handedness handedness)
        {
            for (int i = 0; i < numSteps; i++)
            {
                float t = 1.0f / numSteps * i;
                Vector3 handPos = Vector3.Lerp(startPos, endPos, t);
                var handDataGenerator = GenerateHandPose(
                        gestureId,
                        handedness,
                        handPos);
                Debug.Assert(handedness == Handedness.Right || handedness == Handedness.Left, "handedness must be either right or left");
                SimulatedHandData toUpdate = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
                bool isPinching = gestureId == ArticulatedHandPose.GestureId.Grab || gestureId == ArticulatedHandPose.GestureId.Pinch || gestureId == ArticulatedHandPose.GestureId.PinchSteadyWrist;
                inputSimulationService.HandDataRight.Update(true, false, handDataGenerator);
                yield return null;
            }
        }

        internal static IEnumerator HideHand(InputSimulationService inputSimulationService, Handedness handedness)
        {
            SimulatedHandData toUpdate = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
            inputSimulationService.HandDataRight.Update(false, false, GenerateHandPose(ArticulatedHandPose.GestureId.Open, handedness, Vector3.zero));
            // Wait one frame for the hand to actually go away
            yield return null;
        }
    }
}
