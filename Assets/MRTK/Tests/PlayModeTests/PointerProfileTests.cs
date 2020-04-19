// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class PointerProfileTests
    {
        // Assets/MRTK/Examples/Demos/EyeTracking/General/Profiles/EyeTrackingDemoConfigurationProfile.asset
        private const string eyeTrackingConfigurationProfileGuid = "6615cacb3eaaa044f99b917186093aeb";

        private static readonly string eyeTrackingConfigurationProfilePath = AssetDatabase.GUIDToAssetPath(eyeTrackingConfigurationProfileGuid);
        
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }
        
        // <summary>
        /// Verifies if eye tracking configuration is correctly applied at gaze provider initialization.
        /// </summary>
        [Test]
        public void TestGazeProviderEyeTrackingConfiguration()
        {
            // Default configuration profile should set head based gaze
            var eyeGazeProvider = CoreServices.InputSystem.GazeProvider as IMixedRealityEyeGazeProvider;

            Assert.IsFalse(eyeGazeProvider.IsEyeTrackingEnabled, "Use eye tracking should be set to false");

            // Eye tracking configuration profile should set eye based gaze
            var profile = AssetDatabase.LoadAssetAtPath(eyeTrackingConfigurationProfilePath, typeof(MixedRealityToolkitConfigurationProfile)) as MixedRealityToolkitConfigurationProfile;
            MixedRealityToolkit.Instance.ResetConfiguration(profile);

            Assert.IsTrue(eyeGazeProvider.IsEyeTrackingEnabled, "Use eye tracking should be set to true");
        }       
    }
}
