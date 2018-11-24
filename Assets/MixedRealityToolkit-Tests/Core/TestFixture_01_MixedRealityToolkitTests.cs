// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Tests.Services;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Core
{
    public class TestFixture_01_MixedRealityToolkitTests
    {
        [Test]
        public void Test_01_InitializeMixedRealityToolkit()
        {
            TestUtilities.CleanupScene();

            TestUtilities.InitializeMixedRealityToolkit();
            GameObject gameObject = GameObject.Find(nameof(MixedRealityToolkit));
            Assert.AreEqual(nameof(MixedRealityToolkit), gameObject.name);
        }

        [Test]
        public void Test_02_TestNoMixedRealityConfigurationFound()
        {
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");

            MixedRealityToolkit.Instance.ActiveProfile = null;

            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);
            Assert.IsNull(MixedRealityToolkit.Instance.ActiveProfile);

            TestUtilities.CleanupScene();
        }

        [Test]
        public void Test_03_CreateMixedRealityToolkit()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Create Test Configuration
            Assert.AreEqual(0, MixedRealityToolkit.Instance.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        #region IMixedRealityService Tests

        #endregion IMixedRealityService Tests

        #region IMixedRealityExtensionService Tests

        [Test]
        public void RegisterMixedRealityExtensionService()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService1));

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsNotNull(extensionService1);
        }

        [Test]
        public void RegisterMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test ExtensionService 2", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(IFailService), new TestFailService("Fail Service", 10));
            LogAssert.Expect(LogType.Error, $"Unable to register {typeof(IFailService)}. Concrete type does not implement IMixedRealityExtensionService or IMixedRealityDataProvider.");

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityExtensionService));

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void MixedRealityExtensionService2DoesNotReturn()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService2), "Test2");
            LogAssert.Expect(LogType.Error, "Unable to find Test2 service.");
        }

        [Test]
        public void MixedRealityExtensionService2DoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));

            // Validate non-existent ExtensionService
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            Assert.IsFalse(isServiceRegistered);
        }

        [Test]
        public void ValidateExtensionServiceName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test14-1", 10));

            // Add test ExtensionService 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test14-2", 10));

            // Retrieve Test ExtensionService 2-2
            TestExtensionService2 extensionService2 = (TestExtensionService2)MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService2), "Test14-2");

            // ExtensionService 2-2 Tests
            Assert.AreEqual("Test14-2", extensionService2.Name);

            // Retrieve Test ExtensionService 2-1
            TestExtensionService1 extensionService1 = (TestExtensionService1)MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService1), "Test14-1");

            // ExtensionService 2-1 Tests
            Assert.AreEqual("Test14-1", extensionService1.Name);
        }

        [Test]
        public void GetMixedRealityExtensionServiceCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test15-1", 10));

            // Add test ExtensionServices 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test15-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test15-2.2", 10));

            // Retrieve ExtensionService2
            var extensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(ITestExtensionService2));

            // Tests
            Assert.AreEqual(2, extensionServices.Count);
        }

        [Test]
        public void GetAllMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test16-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test16-1.2", 10));

            // Add test ExtensionServices 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test16-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test16-2.2", 10));

            // Retrieve ExtensionService1
            var allExtensionServices = MixedRealityToolkit.Instance.RegisteredMixedRealityServices;

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityExtensionService Tests

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }
    }
}
