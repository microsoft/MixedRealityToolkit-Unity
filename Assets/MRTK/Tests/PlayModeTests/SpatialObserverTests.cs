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

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Test class that validates observers start and stop based on right configuration in editor. 
    /// </summary>
    public class SpatialObserverTests : BasePlayModeTests
    {
        // Tests/PlayModeTests/TestProfiles/TestMixedRealitySpatialAwarenessSystemProfile.asset
        private const string TestSpatialAwarenessSystemProfileGuid = "c992bdb3ac45cd44d856b0198a3ee85d";
        private static readonly string TestSpatialAwarenessSystemProfilePath = AssetDatabase.GUIDToAssetPath(TestSpatialAwarenessSystemProfileGuid);

        // Tests/PlayModeTests/TestProfiles/TestMixedRealitySpatialAwarenessSystemProfile_ManualStart.asset
        private const string TestSpatialAwarenessSystemProfileGuid_ManualStart = "d38d502ad0bcb3447b106d550fd5a371";
        private static readonly string TestSpatialAwarenessSystemProfilePath_ManualStart = AssetDatabase.GUIDToAssetPath(TestSpatialAwarenessSystemProfileGuid_ManualStart);

        /// <summary>
        /// Test default case of Auto-Start SpatialObjectMeshObserver observer type and SpatialAwarenessSystem in editor
        /// Validates that system resumes (i.e auto-starts again) when disabled/enabled
        /// </summary>
        [UnityTest]
        public IEnumerator TestAutoStartObserver()
        {
            var mrtkProfile = CreateMRTKTestProfile(TestSpatialAwarenessSystemProfilePath);
            TestUtilities.InitializeMixedRealityToolkit(mrtkProfile);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spatialObserver = CoreServices.GetSpatialAwarenessSystemDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNotNull(spatialObserver, "No SpatialObjectMeshObserver data provider created or found");
            Assert.IsTrue(spatialObserver.IsRunning);
            Assert.IsNotEmpty(spatialObserver.Meshes);

            CoreServices.SpatialAwarenessSystem.Disable();
            spatialObserver = CoreServices.GetSpatialAwarenessSystemDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNull(spatialObserver);

            CoreServices.SpatialAwarenessSystem.Enable();
            spatialObserver = CoreServices.GetSpatialAwarenessSystemDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNotNull(spatialObserver, "No SpatialObjectMeshObserver data provider created or found");
            Assert.IsTrue(spatialObserver.IsRunning);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsNotEmpty(spatialObserver.Meshes);
        }

        /// <summary>
        /// Test case of Manual-Start SpatialObjectMeshObserver observer type and SpatialAwarenessSystem in editor
        /// Validates that system should not resume observers unless directly told by test
        /// </summary>
        [UnityTest]
        public IEnumerator TestManualStartObserver()
        {
            var mrtkProfile = CreateMRTKTestProfile(TestSpatialAwarenessSystemProfilePath_ManualStart);
            TestUtilities.InitializeMixedRealityToolkit(mrtkProfile);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spatialAwarenesssDataProvider = CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;
            var spatialObserver = spatialAwarenesssDataProvider.GetDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNotNull(spatialObserver, "No SpatialObjectMeshObserver data provider created or found");
            Assert.IsFalse(spatialObserver.IsRunning);
            Assert.IsEmpty(spatialObserver.Meshes);

            CoreServices.SpatialAwarenessSystem.Disable();
            spatialObserver = spatialAwarenesssDataProvider.GetDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNull(spatialObserver);

            CoreServices.SpatialAwarenessSystem.Enable();
            spatialObserver = spatialAwarenesssDataProvider.GetDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNotNull(spatialObserver, "No SpatialObjectMeshObserver data provider created or found");
            Assert.IsFalse(spatialObserver.IsRunning);
            Assert.IsEmpty(spatialObserver.Meshes);

            CoreServices.SpatialAwarenessSystem.ResumeObservers();
            Assert.IsTrue(spatialObserver.IsRunning);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsNotEmpty(spatialObserver.Meshes);

            CoreServices.SpatialAwarenessSystem.Disable();
            spatialObserver = spatialAwarenesssDataProvider.GetDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNull(spatialObserver);

            CoreServices.SpatialAwarenessSystem.Enable();
            spatialObserver = spatialAwarenesssDataProvider.GetDataProvider<SpatialObjectMeshObserver.SpatialObjectMeshObserver>();
            Assert.IsNotNull(spatialObserver, "No SpatialObjectMeshObserver data provider created or found");
            Assert.IsFalse(spatialObserver.IsRunning);
            Assert.IsEmpty(spatialObserver.Meshes);
        }

        /// <summary>
        /// Test that when the SpatialAwarenessSystem is disabled in the MRTK profile, no service is created
        /// </summary>
        [UnityTest]
        public IEnumerator TestDisableSpatialAwarenessSystem()
        {
            var mrtkProfile = CreateMRTKTestProfile(TestSpatialAwarenessSystemProfilePath);

            mrtkProfile.IsSpatialAwarenessSystemEnabled = false;

            TestUtilities.InitializeMixedRealityToolkit(mrtkProfile);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.IsNull(CoreServices.SpatialAwarenessSystem);
        }

        private static MixedRealityToolkitConfigurationProfile CreateMRTKTestProfile(string spatialAwarenessSystemPath)
        {
            var mrtkProfile = ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();

            mrtkProfile.SpatialAwarenessSystemSystemType = new SystemType(typeof(MixedRealitySpatialAwarenessSystem));
            mrtkProfile.IsSpatialAwarenessSystemEnabled = true;
            mrtkProfile.SpatialAwarenessSystemProfile = AssetDatabase.LoadAssetAtPath<MixedRealitySpatialAwarenessSystemProfile>(spatialAwarenessSystemPath);

            return mrtkProfile;
        }
    }
}
#endif
