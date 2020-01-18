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
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            // todo - cleanup scene

            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private const string CameraInputDiagsProfile = "Assets/MixedRealityToolkit.Tests/PlayModeTests/Core/TestProfiles/CameraInputDiags.asset";
        private const string BoundaryOnlyProfile = "Assets/MixedRealityToolkit.Tests/PlayModeTests/Core/TestProfiles/BoundaryOnly.asset";

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
            Debug.Log($"{profile1.name} loaded {count1} services.");
            foreach (IMixedRealityService s in services)
            {
                Debug.Log(s.Name);
            }

            yield return null;

            // Switch to profile 2
            MixedRealityToolkitConfigurationProfile profile2 = LoadTestProfile(CameraInputDiagsProfile);
            ChangeProfile(profile2);

            yield return null;

            // Get count of registered services
            services = MixedRealityServiceRegistry.GetAllServices();
            int count2 = services.Count;
            Debug.Log($"{profile2.name} loaded {count2} services.");
            foreach (IMixedRealityService s in services)
            {
                Debug.Log(s.Name);
            }

            // Compare the service counts
            Assert.IsTrue(count1 != count2);
        }

        [UnityTest]
        public IEnumerator VerifyServiceState()
        {
            MixedRealityToolkitConfigurationProfile profile1 = TestUtilities.GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>();

            yield return null;

            // Initialize the test case with profile 1
            InitializeTest(profile1);

            // Cache the interesting settings read from the initial boundary system instance
            IMixedRealityBoundarySystem boundarySystem1 = null;
            MixedRealityServiceRegistry.TryGetService<IMixedRealityBoundarySystem>(out boundarySystem1);

            yield return null;

            MixedRealityToolkitConfigurationProfile profile2 = LoadTestProfile(BoundaryOnlyProfile);

            // Switch to profile 2
            ChangeProfile(profile2);

            for (int i = 0; i < 300; i++)
            {
                yield return null;
            }

            // The custom boundary profile has been configured to match the default with the following fields being different
            // * Boundary height
            // * Floor plane physics layer
            // * Show tracked area
            // * Show boundary ceiling
            IMixedRealityBoundarySystem boundarySystem2 = null;
            MixedRealityServiceRegistry.TryGetService<IMixedRealityBoundarySystem>(out boundarySystem2);

            Debug.Log($"Boundary height: 1) {boundarySystem1.BoundaryHeight} 2) {boundarySystem2.BoundaryHeight}");
            Debug.Log($"Floor layer: 1) {boundarySystem1.FloorPhysicsLayer} 2) {boundarySystem2.FloorPhysicsLayer}");
            Debug.Log($"Show tracked area: 1) {boundarySystem1.ShowTrackedArea} 2) {boundarySystem2.ShowTrackedArea}");
            Debug.Log($"Show Ceiling: 1) {boundarySystem1.ShowBoundaryCeiling} 2) {boundarySystem2.ShowBoundaryCeiling}");

            // Check service settings to ensure it has properly reset
            Assert.IsTrue(boundarySystem1.BoundaryHeight != boundarySystem2.BoundaryHeight);
            Assert.IsTrue(boundarySystem1.FloorPhysicsLayer != boundarySystem2.FloorPhysicsLayer);
            Assert.IsTrue(boundarySystem1.ShowTrackedArea != boundarySystem2.ShowTrackedArea);
            Assert.IsTrue(boundarySystem1.ShowBoundaryCeiling != boundarySystem2.ShowBoundaryCeiling);
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