// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Tests.Services;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Core
{
    public class TestFixture_01_MixedRealityToolkitTests
    {
        #region Service Locator Tests

        [Test]
        public void Test_01_InitializeMixedRealityToolkit()
        {
            TestUtilities.CreateScenes();
            MixedRealityToolkit mixedRealityToolkit = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
            MixedRealityToolkit.SetActiveInstance(mixedRealityToolkit);
            MixedRealityToolkit.ConfirmInitialized();

            // Tests
            GameObject gameObject = MixedRealityToolkit.Instance.gameObject;
            Assert.IsNotNull(gameObject);
        }

        [Test]
        public void Test_02_TestNoMixedRealityConfigurationFound()
        {
            TestUtilities.CreateScenes();
            MixedRealityToolkit mixedRealityToolkit = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
            MixedRealityToolkit.SetActiveInstance(mixedRealityToolkit);
            MixedRealityToolkit.ConfirmInitialized();

            MixedRealityToolkit.Instance.ActiveProfile = null;

            // Tests
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");
            Assert.IsFalse(MixedRealityToolkit.Instance.HasActiveProfile);
            Assert.IsNull(MixedRealityToolkit.Instance.ActiveProfile);
        }

        [Test]
        public void Test_03_CreateMixedRealityToolkit()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Tests
            Assert.AreEqual(0, MixedRealityToolkit.Instance.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        #endregion Service Locator Tests

        #region IMixedRealityDataprovider Tests

        [Test]
        public void Test_04_01_RegisterMixedRealityDataProvider()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test Data Provider 1", 10));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsNotNull(extensionService1);
        }

        [Test]
        public void Test_04_02_01_UnregisterMixedRealityDataProviderByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test Data Provider 1", 10));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.Instance.UnregisterService<ITestDataProvider1>();
            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.Instance.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
        }

        [Test]
        public void Test_04_02_02_UnregisterMixedRealityDataProviderByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test Data Provider 1", 10));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            // Retrieve service
            var dataProvider = MixedRealityToolkit.Instance.GetService<ITestDataProvider1>();

            // Validate
            Assert.IsNotNull(dataProvider);

            var success = MixedRealityToolkit.Instance.UnregisterService<ITestDataProvider1>(dataProvider.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.Instance.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
        }

        [Test]
        public void Test_04_03_RegisterMixedRealityDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test Data Provider 1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test Data Provider 2", 10));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityDataProvider>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_04_04_UnregisterMixedRealityDataProvidersByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test Data Provider 1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test Data Provider 2", 10));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityDataProvider>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestDataProvider1>();
            var extensionService2 = MixedRealityToolkit.Instance.GetService<ITestDataProvider2>();

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            var success1 = MixedRealityToolkit.Instance.UnregisterService<ITestDataProvider1>();
            var success2 = MixedRealityToolkit.Instance.UnregisterService<ITestDataProvider2>();

            // Validate non-existent service
            var isService1Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider1>();
            var isService2Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider2>();

            // Tests
            Assert.IsTrue(success1);
            Assert.IsTrue(success2);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.Instance.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider2).Name} service.");
        }

        [Test]
        public void Test_04_05_MixedRealityDataProviderDoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test data provider 1
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test Data Provider 1", 10));

            // Validate non-existent data provider
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider2>();

            // Tests
            Assert.IsFalse(isServiceRegistered);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider2).Name} service.");
        }

        [Test]
        public void Test_04_06_MixedRealityDataProviderDoesNotReturn()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            const string serviceName = "Test Data Provider";

            // Add test data provider
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, serviceName, 10));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.Instance.GetService<ITestExtensionService2>(serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_04_07_ValidateMixedRealityDataProviderName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            var testName1 = "Test04-07-1";
            var testName2 = "Test04-07-2";

            // Add test data providers
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, testName1, 10));
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, testName2, 10));

            // Retrieve
            var dataProvider1 = (TestDataProvider1)MixedRealityToolkit.Instance.GetService<ITestDataProvider1>(testName1);
            var dataProvider2 = (TestDataProvider2)MixedRealityToolkit.Instance.GetService<ITestDataProvider2>(testName2);

            // Tests
            Assert.AreEqual(testName1, dataProvider1.Name);
            Assert.AreEqual(testName2, dataProvider2.Name);
        }

        [Test]
        public void Test_04_08_GetMixedRealityDataProviderCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test data provider 1
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test04-08-1", 10));

            // Add test data provider 2
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test04-08-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test04-08-2.2", 10));

            // Retrieve all ITestDataProvider2 services
            var test2DataProviderServices = MixedRealityToolkit.Instance.GetServices<ITestDataProvider2>();

            // Tests
            Assert.AreEqual(2, test2DataProviderServices.Count);
        }

        [Test]
        public void Test_04_09_GetAllMixedRealityDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test16-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test16-1.2", 10));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test16-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test16-2.2", 10));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityDataProvider>();

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityDataprovider Tests

        #region IMixedRealityExtensionService Tests

        [Test]
        public void Test_05_01_RegisterMixedRealityExtensionService()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test ExtensionService 1",10, null));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_02_01_UnregisterMixedRealityExtensionServiceByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test ExtensionService 1", 10, null));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.Instance.UnregisterService<ITestExtensionService1>();

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.Instance.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
        }

        [Test]
        public void Test_05_02_02_UnregisterMixedRealityExtensionServiceByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test ExtensionService 1", 10, null));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.Instance.UnregisterService<ITestExtensionService1>(extensionService1.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.Instance.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
        }

        [Test]
        public void Test_05_03_RegisterMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test ExtensionService 1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test ExtensionService 2", 10, null));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityExtensionService>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_04_UnregisterMixedRealityExtensionServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test ExtensionService 1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test ExtensionService 2", 10, null));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityExtensionService>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestExtensionService1>();
            var extensionService2 = MixedRealityToolkit.Instance.GetService<ITestExtensionService2>();

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            var success1 = MixedRealityToolkit.Instance.UnregisterService<ITestExtensionService1>();
            var success2 = MixedRealityToolkit.Instance.UnregisterService<ITestExtensionService2>();

            // Validate non-existent service
            var isService1Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();
            var isService2Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            Assert.IsTrue(success1);
            Assert.IsTrue(success2);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.Instance.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService2).Name} service.");
        }

        [Test]
        public void Test_05_05_MixedRealityExtensionService2DoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test ExtensionService 1", 10, null));

            // Validate non-existent ExtensionService
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService2).Name} service.");
            Assert.IsFalse(isServiceRegistered);
        }

        [Test]
        public void Test_05_06_MixedRealityExtensionServiceDoesNotReturnByName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            const string serviceName = "Test ExtensionService 1";

            // Add test ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, serviceName, 10, null));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.Instance.GetService<ITestExtensionService2>(serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_05_07_ValidateExtensionServiceName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test14-1", 10, null));

            // Add test ExtensionService 2
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test14-2", 10, null));

            // Retrieve Test ExtensionService 2-2
            var extensionService2 = (TestExtensionService2)MixedRealityToolkit.Instance.GetService<ITestExtensionService2>("Test14-2");

            // ExtensionService 2-2 Tests
            Assert.AreEqual("Test14-2", extensionService2.Name);

            // Retrieve Test ExtensionService 2-1
            var extensionService1 = (TestExtensionService1)MixedRealityToolkit.Instance.GetService<ITestExtensionService1>("Test14-1");

            // ExtensionService 2-1 Tests
            Assert.AreEqual("Test14-1", extensionService1.Name);
        }

        [Test]
        public void Test_05_08_GetMixedRealityExtensionServiceCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test15-1", 10, null));

            // Add test ExtensionServices 2
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test15-2.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test15-2.2", 10, null));

            // Retrieve ExtensionService2
            var extensionServices = MixedRealityToolkit.Instance.GetServices<ITestExtensionService2>();

            // Tests
            Assert.AreEqual(2, extensionServices.Count);
        }

        [Test]
        public void Test_05_09_GetAllMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test16-1.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test16-1.2", 10, null));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test16-2.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test16-2.2", 10, null));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityExtensionService>();

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityExtensionService Tests

        [Test]
        public void Test_07_01_EnableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test07-01-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test07-01-1.2",10, null));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test07-01-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test07-01-2.2",10, null));

            // Enable all test services
            MixedRealityToolkit.Instance.EnableAllServicesByType(typeof(ITestService));

            // Tests
            var testServices = MixedRealityToolkit.Instance.GetServices<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsTrue((service as ITestService).IsEnabled);
            }
        }

        [Test]
        public void Test_07_02_DisableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(null, null, "Test07-01-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(null, "Test07-01-1.2",10, null));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider2>(new TestDataProvider2(null, null, "Test07-01-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(null, "Test07-01-2.2",10, null));

            // Enable all test services
            MixedRealityToolkit.Instance.EnableAllServicesByType(typeof(ITestService));

            // Get all services
            var testServices = MixedRealityToolkit.Instance.GetServices<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsTrue((service as ITestService).IsEnabled);
            }

            // Enable all test services
            MixedRealityToolkit.Instance.DisableAllServicesByType(typeof(ITestService));

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsFalse((service as ITestService).IsEnabled);
            }
        }

        #region Multiple Instances Tests

        [Test]
        public void Test_08_01_CreateMultipleInstances()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            MixedRealityToolkit secondInstance = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
            MixedRealityToolkit thirdInstance = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();

            Assert.AreNotEqual(secondInstance, MixedRealityToolkit.Instance);
            Assert.AreNotEqual(thirdInstance, MixedRealityToolkit.Instance);
            Assert.IsFalse(secondInstance.IsActiveInstance);
            Assert.IsFalse(thirdInstance.IsActiveInstance);
        }

        [Test]
        public void Test_08_02_SwitchBetweenActiveInstances()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            MixedRealityToolkit secondInstance = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
            MixedRealityToolkit thirdInstance = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();

            Assert.AreNotEqual(secondInstance, MixedRealityToolkit.Instance);
            Assert.AreNotEqual(thirdInstance, MixedRealityToolkit.Instance);
            Assert.IsFalse(secondInstance.IsActiveInstance);
            Assert.IsFalse(thirdInstance.IsActiveInstance);

            MixedRealityToolkit.SetActiveInstance(secondInstance);

            Assert.AreEqual(secondInstance, MixedRealityToolkit.Instance);
            Assert.IsTrue(secondInstance.IsActiveInstance);

            MixedRealityToolkit.SetActiveInstance(thirdInstance);

            Assert.AreEqual(thirdInstance, MixedRealityToolkit.Instance);
            Assert.IsTrue(thirdInstance.IsActiveInstance);
        }

        [Test]
        public void Test_08_03_DestroyActiveInstance()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            MixedRealityToolkit secondInstance = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();

            GameObject.DestroyImmediate(MixedRealityToolkit.Instance.gameObject);

            MixedRealityToolkit.SetActiveInstance(secondInstance);

            Assert.NotNull(MixedRealityToolkit.Instance);
            Assert.AreEqual(secondInstance, MixedRealityToolkit.Instance);
            Assert.IsTrue(secondInstance.IsActiveInstance);
        }

        [Test]
        public void Test_08_04_CreateMultipleInstancesInMultipleScenes()
        {
            TestUtilities.CreateScenes(3);

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                SceneManager.SetActiveScene(scene);
                MixedRealityToolkit newInstance = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();                
            }

            MixedRealityToolkit[] instances = GameObject.FindObjectsOfType<MixedRealityToolkit>();
            for (int i = 0; i < instances.Length; i++)
            {
                MixedRealityToolkit.SetActiveInstance(instances[i]);

                Assert.AreEqual(instances[i], MixedRealityToolkit.Instance);
                Assert.IsTrue(instances[i].IsActiveInstance);

                for (int j = 0; j < instances.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Assert.IsFalse(instances[j].IsActiveInstance);
                }
            }
        }

        #endregion

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.TearDownScenes();
        }
    }
}
