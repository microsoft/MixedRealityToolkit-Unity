// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Tests.EditMode.Services;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.SpatialAwarenessSystem
{
    public class SpatialAwarenessSystemTests
    {
        // Tests/EditModeTests/Services/TestProfiles/TestMixedRealitySpatialAwarenessSystemProfile.asset
        private const string TestSpatialAwarenessSystemProfileGuid = "757d72d70f9c56144b05346288682e00";
        private static readonly string TestSpatialAwarenessSystemProfilePath = AssetDatabase.GUIDToAssetPath(TestSpatialAwarenessSystemProfileGuid);

        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
            TestUtilities.EditorTearDownScenes();
        }

        [Test]
        public void TestGetSpatialAwarenessSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Tests
            Assert.IsNotNull(CoreServices.SpatialAwarenessSystem);
        }

        [Test]
        public void TestSpatialAwarenessSystemDoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Check for Spatial Awareness System
            Assert.IsFalse(MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealitySpatialAwarenessSystem>());
        }

        [Test]
        public void TestSpatialAwarenessSystemExists()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Check for Spatial Awareness System
            Assert.IsTrue(MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealitySpatialAwarenessSystem>());
        }

        [Test]
        public void TestDataProviderRegistration()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();
            MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile = AssetDatabase.LoadAssetAtPath<MixedRealitySpatialAwarenessSystemProfile>(TestSpatialAwarenessSystemProfilePath);

            var spatialAwarenessSystem = new MixedRealitySpatialAwarenessSystem(MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile);

            Assert.IsTrue(MixedRealityToolkit.Instance.RegisterService<IMixedRealitySpatialAwarenessSystem>(spatialAwarenessSystem));

            // Since EditMode, we have to auto-enable MRTK spatial awareness system ourselves
            MixedRealityToolkit.Instance.EnableAllServicesByType(typeof(IMixedRealitySpatialAwarenessSystem));

            Assert.AreEqual(1, MixedRealityServiceRegistry.GetAllServices().Count);
            Assert.IsNotNull(MixedRealityToolkit.Instance.GetService<IMixedRealitySpatialAwarenessSystem>());

            var dataProviderAccess = (spatialAwarenessSystem as IMixedRealityDataProviderAccess);
            Assert.IsNotNull(dataProviderAccess);

            var dataProvider = dataProviderAccess.GetDataProvider<TestSpatialAwarenessDataProvider>();
            Assert.IsNotNull(dataProvider);
            Assert.IsTrue(dataProvider.IsInitialized);
            Assert.IsTrue(dataProvider.IsEnabled);

            spatialAwarenessSystem.Disable();
            Assert.IsFalse(dataProvider.IsEnabled);

            spatialAwarenessSystem.Enable();
            // We still have reference to old dataprovider, check still disabled
            Assert.IsFalse(dataProvider.IsEnabled);

            // dataProvider has been unregistered in Disable and new one created by Enable.
            dataProvider = dataProviderAccess.GetDataProvider<TestSpatialAwarenessDataProvider>();
            Assert.IsNotNull(dataProvider);
            Assert.IsTrue(dataProvider.IsEnabled);

            spatialAwarenessSystem.Reset();
            LogAssert.Expect(LogType.Log, "TestDataProvider Reset");
            Assert.IsFalse(dataProvider.IsEnabled);

            // dataProvider has been unregistered and newly created in Reset
            dataProvider = dataProviderAccess.GetDataProvider<TestSpatialAwarenessDataProvider>();
            Assert.IsNotNull(dataProvider);
            Assert.IsTrue(dataProvider.IsInitialized);
        }
    }
}