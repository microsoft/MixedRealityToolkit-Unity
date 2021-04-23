// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class PointerProfileTests
    {
        // Assets/MRTK/Examples/Demos/EyeTracking/General/Profiles/EyeTrackingDemoConfigurationProfile.asset
        private const string eyeTrackingConfigurationProfileGuid = "6615cacb3eaaa044f99b917186093aeb";

        private static readonly string eyeTrackingConfigurationProfilePath = AssetDatabase.GUIDToAssetPath(eyeTrackingConfigurationProfileGuid);

        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        // <summary>
        /// Verifies if eye tracking configuration is correctly applied at gaze provider initialization.
        /// </summary>
        [UnityTest]
        public IEnumerator TestGazeProviderEyeTrackingConfiguration()
        {
            // Default configuration profile should set head based gaze
            var eyeGazeProvider = CoreServices.InputSystem.GazeProvider as IMixedRealityEyeGazeProvider;

            Assert.IsFalse(eyeGazeProvider.IsEyeTrackingEnabled, "Use eye tracking should be set to false");

            // Eye tracking configuration profile should set eye based gaze
            var profile = AssetDatabase.LoadAssetAtPath(eyeTrackingConfigurationProfilePath, typeof(MixedRealityToolkitConfigurationProfile)) as MixedRealityToolkitConfigurationProfile;
            MixedRealityToolkit.Instance.ActiveProfile = profile;
            yield return null;
            yield return null;
            eyeGazeProvider = CoreServices.InputSystem.GazeProvider as IMixedRealityEyeGazeProvider;

            Assert.IsTrue(eyeGazeProvider.IsEyeTrackingEnabled, "Use eye tracking should be set to true");
        }
    }
}
