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
    class EyeTrackingTargetTest
    {
        // Assets/MRTK/Examples/Demos/EyeTracking/General/Profiles/EyeTrackingDemoConfigurationProfile.asset
        private const string eyeTrackingConfigurationProfileGuid = "6615cacb3eaaa044f99b917186093aeb";

        private static readonly string eyeTrackingConfigurationProfilePath = AssetDatabase.GUIDToAssetPath(eyeTrackingConfigurationProfileGuid);

        private GameObject target;

        // This method is called once before we enter play mode and execute any of the tests
        // do any kind of setup here that can't be done in playmode
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        // Destroy the scene - this method is called after each test listed below has completed
        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        #region Tests

        /// <summary>
        /// Skeleton for a new MRTK play mode test.
        /// </summary>
        [UnityTest]
        public IEnumerator TestEyeTrackingTargetColliderOnParent()
        {
            // Eye tracking configuration profile should set eye based gaze
            var profile = AssetDatabase.LoadAssetAtPath(eyeTrackingConfigurationProfilePath, typeof(MixedRealityToolkitConfigurationProfile)) as MixedRealityToolkitConfigurationProfile;
            MixedRealityToolkit.Instance.ResetConfiguration(profile);

            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Build a target to collide against
            var parentOfTargetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            parentOfTargetObject.name = "ParentOfTargetObject";
            parentOfTargetObject.AddComponent<BoxCollider>();
            parentOfTargetObject.transform.localScale = new Vector3(25.0f, 25.0f, 0.2f);
            parentOfTargetObject.transform.Rotate(Vector3.up, 180.0f); // Rotate parentOfTargetObject so forward faces camera
            parentOfTargetObject.transform.position = Vector3.forward * 10.0f;

            var childOfTargetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            childOfTargetObject.name = "childOfTargetObject";
            var eyeTrackingTargetComponent = childOfTargetObject.AddComponent<EyeTrackingTarget>();
            childOfTargetObject.transform.localScale = new Vector3(20.0f, 20.0f, 0.1f);
            childOfTargetObject.transform.SetParent(parentOfTargetObject.transform);

            //We need to simulate an eye in the direction of the parent object
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            //We don't need any other input just eye gaze
            inputSimulationService.UserInputEnabled = false;
            inputSimulationService.EyeGazeSimulationMode = EyeGazeSimulationMode.CameraForwardAxis;

            yield return null;

            var isEyeGazeActive = InputRayUtils.TryGetEyeGazeRay(out var eyegazeRay);
            Assert.True(isEyeGazeActive);
            yield return null;
            Assert.True(EyeTrackingTarget.LookedAtTarget != null);
            Assert.True(EyeTrackingTarget.LookedAtEyeTarget.name == "childOfTargetObject");
        }
        #endregion
    }
}
#endif