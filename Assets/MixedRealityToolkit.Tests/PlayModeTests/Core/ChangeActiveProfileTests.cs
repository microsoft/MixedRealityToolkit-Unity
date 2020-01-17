// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify that changing the active profile at runtime results in
    // the MRTK being correctly reconfigured.
    public class ChangeActiveProfileTests
    {
        //[SetUp]
        //public void SetUp()
        //{
        //    TestUtilities.InitializeMixedRealityToolkit(true);
        //    TestUtilities.PlayspaceToOriginLookingForward();

        //    // todo - load scene
        //}

        [TearDown]
        public void TearDown()
        {
            // todo - cleanup scene

            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private const string CameraInputDiagsProfile = "Assets/MixedRealityToolkit.Tests/PlayModeTests/Core/TestProfiles/CameraInputDiags.asset";

        [UnityTest]
        public IEnumerator VerifyServiceCount_ToFewer()
        {
            
            MixedRealityToolkitConfigurationProfile profile1 = TestUtilities.GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>();
            MixedRealityToolkitConfigurationProfile profile2 = LoadTestProfile(CameraInputDiagsProfile);

            yield return null;

            // Initialize the test case with profile 1
            InitializeTest(profile1);

            // Get count of registered services
            int count1 = MixedRealityServiceRegistry.GetAllServices().Count;

            // Switch to profile 2
            ChangeProfile(profile2);

            // Get count of registered services
            int count2 = MixedRealityServiceRegistry.GetAllServices().Count;

            // There should be fewer services registered
            Assert.IsTrue(count1 > count2);
        }

        [UnityTest]
        public IEnumerator VerifyServiceCount_ToMore()
        {
            MixedRealityToolkitConfigurationProfile profile1 = LoadTestProfile(CameraInputDiagsProfile);
            MixedRealityToolkitConfigurationProfile profile2 = TestUtilities.GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>();

            yield return null;

            // Initialize the test case with profile 1
            InitializeTest(profile1);

            // Get count of registered services
            int count1 = MixedRealityServiceRegistry.GetAllServices().Count;

            // Switch to profile 2
            ChangeProfile(profile2);

            // Get count of registered services
            int count2 = MixedRealityServiceRegistry.GetAllServices().Count;

            // There should be more services registered
            Assert.IsTrue(count1 < count2);
        }

        private void InitializeTest(MixedRealityToolkitConfigurationProfile profile)
        {
            TestUtilities.InitializeMixedRealityToolkit(profile);
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        private void ChangeProfile(MixedRealityToolkitConfigurationProfile newProfile)
        {
            MixedRealityToolkitConfigurationProfile oldProfile = MixedRealityToolkit.Instance.ActiveProfile;
            Debug.Log($"Switching active profile from {oldProfile.name} to {newProfile.name}");
            MixedRealityToolkit.Instance.ActiveProfile = newProfile;
        }

        private MixedRealityToolkitConfigurationProfile LoadTestProfile(string file)
        {
            return AssetDatabase.LoadAssetAtPath<MixedRealityToolkitConfigurationProfile>(file);
        }
    }
}

#endif // !WINDOWS_UWP