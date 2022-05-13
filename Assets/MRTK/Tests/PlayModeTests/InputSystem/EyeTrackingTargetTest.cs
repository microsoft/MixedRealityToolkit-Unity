// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    // tests for eyetracking
    public class EyeTrackingTargetTest : BasePlayModeTests
    {
        // Assets/MRTK/Examples/Demos/EyeTracking/General/Profiles/EyeTrackingDemoConfigurationProfile.asset
        private const string eyeTrackingConfigurationProfileGuid = "6615cacb3eaaa044f99b917186093aeb";

        private static readonly string eyeTrackingConfigurationProfilePath = AssetDatabase.GUIDToAssetPath(eyeTrackingConfigurationProfileGuid);

        // This method is called once before we enter play mode and execute any of the tests
        // do any kind of setup here that can't be done in playmode
        public override IEnumerator Setup()
        {
            var eyeTrackingProfile = AssetDatabase.LoadAssetAtPath(eyeTrackingConfigurationProfilePath, typeof(MixedRealityToolkitConfigurationProfile)) as MixedRealityToolkitConfigurationProfile;
            PlayModeTestUtilities.Setup(eyeTrackingProfile);
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;
        }

        #region Tests

        /// <summary>
        /// Skeleton for a new MRTK play mode test.
        /// </summary>
        [UnityTest]
        public IEnumerator TestEyeTrackingTarget()
        {
            string targetName = "eyetrackingTargetObject";

            var eyetrackingTargetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eyetrackingTargetObject.name = targetName;
            var eyeTrackingTargetComponent = eyetrackingTargetObject.AddComponent<EyeTrackingTarget>();
            eyetrackingTargetObject.transform.position = Vector3.forward;

            // We need to simulate an eye in the direction of the parent object
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            // We don't need any other input just eye gaze
            inputSimulationService.UserInputEnabled = false;
            inputSimulationService.EyeGazeSimulationMode = EyeGazeSimulationMode.CameraForwardAxis;

            yield return null;

            var isEyeGazeActive = InputRayUtils.TryGetEyeGazeRay(out var eyegazeRay);
            Assert.True(isEyeGazeActive);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.True(EyeTrackingTarget.LookedAtTarget != null);
            Assert.True(EyeTrackingTarget.LookedAtEyeTarget != null);
            Assert.True(EyeTrackingTarget.LookedAtEyeTarget.name == targetName);
        }
        #endregion
    }
}
#endif