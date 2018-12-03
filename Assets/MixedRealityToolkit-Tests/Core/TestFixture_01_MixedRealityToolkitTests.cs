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
        #region Service Locator Tests

        [Test]
        public void Test_01_InitializeMixedRealityToolkit()
        {
            TestUtilities.CleanupScene();
            MixedRealityToolkit.ConfirmInitialized();

            // Tests
            GameObject gameObject = GameObject.Find(nameof(MixedRealityToolkit));
            Assert.AreEqual(nameof(MixedRealityToolkit), gameObject.name);
        }

        [Test]
        public void Test_02_TestNoMixedRealityConfigurationFound()
        {
            TestUtilities.CleanupScene();
            MixedRealityToolkit.ConfirmInitialized();

            MixedRealityToolkit.Instance.ActiveProfile = null;

            // Tests
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);
            Assert.IsNull(MixedRealityToolkit.Instance.ActiveProfile);
        }

        [Test]
        public void Test_03_CreateMixedRealityToolkit()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Tests
            Assert.AreEqual(0, MixedRealityToolkit.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        #endregion Service Locator Tests

        #region IMixedRealityDataprovider Tests

        [Test]
        public void Test_04_01_RegisterMixedRealityDataProvider()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test Data Provider 1", 10));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider1));

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsNotNull(extensionService1);
        }

        [Test]
        public void Test_04_02_01_UnregisterMixedRealityDataProviderByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test Data Provider 1", 10));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider1));

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.Instance.UnregisterService(typeof(ITestDataProvider1));

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
        }

        [Test]
        public void Test_04_02_02_UnregisterMixedRealityDataProviderByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test Data Provider 1", 10));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider1));

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve service
            var dataProvider = MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider1));

            // Validate
            Assert.IsNotNull(dataProvider);

            var success = MixedRealityToolkit.UnregisterService(typeof(ITestDataProvider1), dataProvider.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
        }

        [Test]
        public void Test_04_03_RegisterMixedRealityDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test Data Provider 1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestDataProvider2("Test Data Provider 2", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestFailService), new TestFailService());
            LogAssert.Expect(LogType.Error, $"{typeof(ITestFailService).Name} does not implement {typeof(IMixedRealityService).Name}.");

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityDataProvider));

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_04_04_UnregisterMixedRealityDataProvidersByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test Data Provider 1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestDataProvider2("Test Data Provider 2", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestFailService), new TestFailService());
            LogAssert.Expect(LogType.Error, $"{typeof(ITestFailService).Name} does not implement {typeof(IMixedRealityService).Name}.");

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityDataProvider));

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider1));
            var extensionService2 = MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider2));

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            var success1 = MixedRealityToolkit.Instance.UnregisterService(typeof(ITestDataProvider1));
            var success2 = MixedRealityToolkit.Instance.UnregisterService(typeof(ITestDataProvider2));

            // Validate non-existent service
            var isService1Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider1>();
            var isService2Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider2>();

            // Tests
            Assert.IsTrue(success1);
            Assert.IsTrue(success2);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider2).Name} service.");
        }

        [Test]
        public void Test_04_05_MixedRealityDataProviderDoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test data provider 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test Data Provider 1", 10));

            // Validate non-existent data provider
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestDataProvider2>();

            // Tests
            Assert.IsFalse(isServiceRegistered);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider2).Name} service.");
        }

        [Test]
        public void Test_04_06_MixedRealityDataProviderDoesNotReturn()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            const string serviceName = "Test Data Provider";

            // Add test test data provider
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1(serviceName, 10));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService2), serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_04_07_ValidateMixedRealityDataProviderName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            var testName1 = "Test04-07-1";
            var testName2 = "Test04-07-2";

            // Add test data providers
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1(testName1, 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestDataProvider2(testName2, 10));

            // Retrieve 
            var dataProvider1 = (TestDataProvider1)MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider1), testName1);
            var dataProvider2 = (TestDataProvider2)MixedRealityToolkit.Instance.GetService(typeof(ITestDataProvider2), testName2);

            // Tests
            Assert.AreEqual(testName1, dataProvider1.Name);
            Assert.AreEqual(testName2, dataProvider2.Name);
        }

        [Test]
        public void Test_04_08_GetMixedRealityDataProviderCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test data provider 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestExtensionService1("Test04-08-1", 10));

            // Add test data provider 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestExtensionService2("Test04-08-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestExtensionService2("Test04-08-2.2", 10));

            // Retrieve all ITestDataProvider2 services
            var test2DataProviderServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(ITestDataProvider2));

            // Tests
            Assert.AreEqual(2, test2DataProviderServices.Count);
        }

        [Test]
        public void Test_04_09_GetAllMixedRealityDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestExtensionService1("Test16-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestExtensionService1("Test16-1.2", 10));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestExtensionService2("Test16-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestExtensionService2("Test16-2.2", 10));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityExtensionService));

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityDataprovider Tests

        #region IMixedRealityExtensionService Tests

        [Test]
        public void Test_05_01_RegisterMixedRealityExtensionService()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService1));

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_02_01_UnregisterMixedRealityExtensionServiceByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService1));

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.Instance.UnregisterService(typeof(ITestExtensionService1));

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
        }

        [Test]
        public void Test_05_02_02_UnregisterMixedRealityExtensionServiceByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService1));

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.UnregisterService(typeof(ITestExtensionService1), extensionService1.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
        }

        [Test]
        public void Test_05_03_RegisterMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test ExtensionService 2", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestFailService), new TestFailService());
            LogAssert.Expect(LogType.Error, $"{typeof(ITestFailService).Name} does not implement {typeof(IMixedRealityService).Name}.");

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityExtensionService));

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_04_UnregisterMixedRealityExtensionServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test ExtensionService 2", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestFailService), new TestFailService());
            LogAssert.Expect(LogType.Error, $"{typeof(ITestFailService).Name} does not implement {typeof(IMixedRealityService).Name}.");

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityExtensionService));

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService1));
            var extensionService2 = MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService2));

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            var success1 = MixedRealityToolkit.Instance.UnregisterService(typeof(ITestExtensionService1));
            var success2 = MixedRealityToolkit.Instance.UnregisterService(typeof(ITestExtensionService2));

            // Validate non-existent service
            var isService1Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();
            var isService2Registered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            Assert.IsTrue(success1);
            Assert.IsTrue(success2);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService2).Name} service.");
        }

        [Test]
        public void Test_05_05_MixedRealityExtensionService2DoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test ExtensionService 1", 10));

            // Validate non-existent ExtensionService
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService2).Name} service.");
            Assert.IsFalse(isServiceRegistered);
        }

        [Test]
        public void Test_05_06_MixedRealityExtensionServiceDoesNotReturnByName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            const string serviceName = "Test ExtensionService 1";

            // Add test ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1(serviceName, 10));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService2), serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_05_07_ValidateExtensionServiceName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test14-1", 10));

            // Add test ExtensionService 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test14-2", 10));

            // Retrieve Test ExtensionService 2-2
            var extensionService2 = (TestExtensionService2)MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService2), "Test14-2");

            // ExtensionService 2-2 Tests
            Assert.AreEqual("Test14-2", extensionService2.Name);

            // Retrieve Test ExtensionService 2-1
            var extensionService1 = (TestExtensionService1)MixedRealityToolkit.Instance.GetService(typeof(ITestExtensionService1), "Test14-1");

            // ExtensionService 2-1 Tests
            Assert.AreEqual("Test14-1", extensionService1.Name);
        }

        [Test]
        public void Test_05_08_GetMixedRealityExtensionServiceCollectionByInterface()
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
        public void Test_05_09_GetAllMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test16-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test16-1.2", 10));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test16-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test16-2.2", 10));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityExtensionService));

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityExtensionService Tests

        [Test]
        public void Test_07_01_EnableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test07-01-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test07-01-1.2", 10));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestDataProvider2("Test07-01-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test07-01-2.2", 10));

            // Enable all test services
            MixedRealityToolkit.EnableAllServicesByType(typeof(ITestService));

            // Tests
            var testServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(ITestService));

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsTrue((service as ITestService).IsEnabled);
            }
        }

        [Test]
        public void Test_07_02_DisableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider1), new TestDataProvider1("Test07-01-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService1), new TestExtensionService1("Test07-01-1.2", 10));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestDataProvider2), new TestDataProvider2("Test07-01-2.1", 10));
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestExtensionService2), new TestExtensionService2("Test07-01-2.2", 10));

            // Enable all test services
            MixedRealityToolkit.EnableAllServicesByType(typeof(ITestService));

            // Get all services
            var testServices = MixedRealityToolkit.Instance.GetActiveServices(typeof(ITestService));

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsTrue((service as ITestService).IsEnabled);
            }

            // Enable all test services
            MixedRealityToolkit.DisableAllServicesByType(typeof(ITestService));

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsFalse((service as ITestService).IsEnabled);
            }
        }

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }
    }
}
