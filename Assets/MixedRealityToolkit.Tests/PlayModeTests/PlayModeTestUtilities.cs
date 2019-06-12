// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using NUnit.Framework;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using TMPro;
using UnityEditor;
#endif

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

        internal static IEnumerator MoveHandFromTo(Vector3 startPos, Vector3 endPos, int numSteps, ArticulatedHandPose.GestureId gestureId, Handedness handedness, InputSimulationService inputSimulationService)
        {
            Debug.Assert(handedness == Handedness.Right || handedness == Handedness.Left, "handedness must be either right or left");
            bool isPinching = gestureId == ArticulatedHandPose.GestureId.Grab || gestureId == ArticulatedHandPose.GestureId.Pinch || gestureId == ArticulatedHandPose.GestureId.PinchSteadyWrist;
            for (int i = 0; i < numSteps; i++)
            {
                float t = 1.0f / numSteps * i;
                Vector3 handPos = Vector3.Lerp(startPos, endPos, t);
                var handDataGenerator = GenerateHandPose(
                        gestureId,
                        handedness,
                        handPos);
                SimulatedHandData toUpdate = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
                inputSimulationService.HandDataRight.Update(true, isPinching, handDataGenerator);
                yield return null;
            }
        }

        internal static IEnumerator HideHand(Handedness handedness, InputSimulationService inputSimulationService)
        {
            SimulatedHandData toUpdate = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
            inputSimulationService.HandDataRight.Update(false, false, GenerateHandPose(ArticulatedHandPose.GestureId.Open, handedness, Vector3.zero));
            // Wait one frame for the hand to actually appear
            yield return null;
        }

        internal static IEnumerator ShowHand(Handedness handedness, InputSimulationService inputSimulationService)
        {
            SimulatedHandData toUpdate = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
            inputSimulationService.HandDataRight.Update(true, false, GenerateHandPose(ArticulatedHandPose.GestureId.Open, handedness, Vector3.zero));
            // Wait one frame for the hand to actually go away
            yield return null;
        }

        internal static void EnsureTextMeshProEssentials()
        {
#if UNITY_EDITOR
            // Special handling for TMP Settings and importing Essential Resources
            if (TMP_Settings.instance == null)
            {
                string packageFullPath = Path.GetFullPath("Packages/com.unity.textmeshpro");
                if (Directory.Exists(packageFullPath))
                {
                    AssetDatabase.ImportPackage(packageFullPath + "/Package Resources/TMP Essential Resources.unitypackage", false);
                }
            }
#endif
        }
    }
}
#endif