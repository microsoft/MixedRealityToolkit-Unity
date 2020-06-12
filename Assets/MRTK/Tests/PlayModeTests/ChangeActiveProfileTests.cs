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

using Microsoft.MixedReality.Toolkit.Boundary;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify that changing the active profile at runtime results in
    // the MRTK being correctly reconfigured.
    public class ChangeActiveProfileTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private const string DefaultHoloLens2ProfileGuid = "7e7c962b9eb9dfa44993d5b2f2576752";
        private static readonly string DefaultHoloLens2ProfilePath = AssetDatabase.GUIDToAssetPath(DefaultHoloLens2ProfileGuid);

        private const string BoundaryOnlyProfileGuid = "1945e1d0f0513ea4da45f9296a206ab3";
        private static readonly string BoundaryOnlyProfilePath = AssetDatabase.GUIDToAssetPath(BoundaryOnlyProfileGuid);

        /// <summary>
        /// Test to verify that switching profiles results in appropriate service counts.
        /// </summary>
        [UnityTest]
        public IEnumerator VerifyServiceCount()
        {
            MixedRealityToolkitConfigurationProfile profile1 = TestUtilities.GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>();
            yield return null;

            // Initialize the test case with profile 1
            InitializeTest(profile1);

            // Get count of registered services
            IReadOnlyList<IMixedRealityService> services = MixedRealityServiceRegistry.GetAllServices();
            int count1 = services.Count;
            yield return null;

            // Switch to profile 2
            MixedRealityToolkitConfigurationProfile profile2 = LoadTestProfile(DefaultHoloLens2ProfilePath);
            ChangeProfile(profile2);
            yield return null;

            // Get count of registered services
            services = MixedRealityServiceRegistry.GetAllServices();
            int count2 = services.Count;

            // We specifically selected the test profiles to ensure that they load a different number of services.
            Assert.IsTrue(count1 != count2);
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

        private MixedRealityToolkitConfigurationProfile LoadTestProfile(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<MixedRealityToolkitConfigurationProfile>(assetPath);
        }
    }
}

#endif // !WINDOWS_UWP