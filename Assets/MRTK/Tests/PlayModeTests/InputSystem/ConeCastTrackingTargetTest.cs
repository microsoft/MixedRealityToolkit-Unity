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
    public class ConeCastTrackingTargetTest : BasePlayModeTests
    {
        // Assets/MRTK/Examples/Demos/LookAndPinch/General/Profiles/LookAndPinchDemoConfigurationProfile.asset
        private const string eyeTrackingConfigurationProfileGuid = "4aad8e2f64fa9274e8bcc7714040f599";

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

            // Create another target cone cast will hit and will prefer
            var eyetrackingTargetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eyetrackingTargetObject.name = targetName;
            var eyeTrackingTargetComponent = eyetrackingTargetObject.AddComponent<EyeTrackingTarget>();
            eyetrackingTargetObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            eyetrackingTargetObject.transform.position = Vector3.forward + (Vector3.right * 0.05f);

            // We need to simulate an eye in the direction of the parent object
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            // We don't need any other input just eye gaze
            inputSimulationService.UserInputEnabled = false;
            inputSimulationService.EyeGazeSimulationMode = EyeGazeSimulationMode.CameraForwardAxis;

            yield return null;

            var isEyeGazeActive = InputRayUtils.TryGetEyeGazeRay(out var eyegazeRay);
            Assert.True(isEyeGazeActive);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.True(CoreServices.InputSystem?.EyeGazeProvider.GazeTarget == eyetrackingTargetObject);
            Assert.True(EyeTrackingTarget.LookedAtEyeTarget.name == targetName);
        }

        /// <summary>
        /// Skeleton for a new MRTK play mode test.
        /// </summary>
        [UnityTest]
        public IEnumerator TestEyeTrackingTargetMiss()
        {
            string targetName = "eyetrackingTargetObject";

            // Create another target cone cast will not hit 
            var eyetrackingTargetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eyetrackingTargetObject.name = targetName;
            var eyeTrackingTargetComponent = eyetrackingTargetObject.AddComponent<EyeTrackingTarget>();
            eyetrackingTargetObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            eyetrackingTargetObject.transform.position = Vector3.forward + (Vector3.right * 0.1f);

            // We need to simulate an eye in the direction of the parent object
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            // We don't need any other input just eye gaze
            inputSimulationService.UserInputEnabled = false;
            inputSimulationService.EyeGazeSimulationMode = EyeGazeSimulationMode.CameraForwardAxis;

            yield return null;

            var isEyeGazeActive = InputRayUtils.TryGetEyeGazeRay(out var eyegazeRay);
            Assert.True(isEyeGazeActive);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.True(CoreServices.InputSystem?.EyeGazeProvider.GazeTarget == null);
        }

        /// <summary>
        /// Skeleton for a new MRTK play mode test.
        /// </summary>
        [UnityTest]
        public IEnumerator TestEyeTrackingTargetMultipleTargets()
        {
            string targetNameHit = "eyetrackingTargetObject-Hit";
            string targetNameMiss = "eyetrackingTargetObject-Miss";

            // Create another target cone cast will hit and will prefer
            var eyetrackingTargetObjectHit = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eyetrackingTargetObjectHit.name = targetNameHit;
            _ = eyetrackingTargetObjectHit.AddComponent<EyeTrackingTarget>();
            eyetrackingTargetObjectHit.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            eyetrackingTargetObjectHit.transform.position = Vector3.forward + (Vector3.right * 0.01f);

            // Create another target cone cast will hit but not prefer
            var eyetrackingTargetObjectMiss = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eyetrackingTargetObjectMiss.name = targetNameMiss;
            _ = eyetrackingTargetObjectMiss.AddComponent<EyeTrackingTarget>();
            eyetrackingTargetObjectMiss.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            eyetrackingTargetObjectMiss.transform.position = (Vector3.forward * 2) + (Vector3.right * 0.03f);

            // We need to simulate an eye in the direction of the parent object
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            // We don't need any other input just eye gaze
            inputSimulationService.UserInputEnabled = false;
            inputSimulationService.EyeGazeSimulationMode = EyeGazeSimulationMode.CameraForwardAxis;

            yield return null;

            var isEyeGazeActive = InputRayUtils.TryGetEyeGazeRay(out var eyegazeRay);
            Assert.True(isEyeGazeActive);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.True(CoreServices.InputSystem?.EyeGazeProvider.GazeTarget == eyetrackingTargetObjectHit);
            Assert.True(EyeTrackingTarget.LookedAtEyeTarget.name == targetNameHit);
            Assert.True(CoreServices.InputSystem?.EyeGazeProvider.GazeTarget != eyetrackingTargetObjectMiss);
            Assert.True(EyeTrackingTarget.LookedAtEyeTarget.name != targetNameMiss);
        }
        #endregion
    }
}
#endif