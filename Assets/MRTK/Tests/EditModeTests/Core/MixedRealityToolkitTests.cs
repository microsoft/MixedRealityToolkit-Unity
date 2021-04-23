// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Tests.EditMode.Services;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Core
{
    public class MixedRealityToolkitTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
            TestUtilities.EditorTearDownScenes();
        }

        #region Service Locator Tests

        [Test]
        public void TestInitializeMixedRealityToolkit()
        {
            TestUtilities.EditorCreateScenes();
            MixedRealityToolkit mixedRealityToolkit = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
            MixedRealityToolkit.SetActiveInstance(mixedRealityToolkit);
            MixedRealityToolkit.ConfirmInitialized();

            // Tests
            GameObject gameObject = MixedRealityToolkit.Instance.gameObject;
            Assert.IsNotNull(gameObject);
        }

        [Test]
        public void TestNoMixedRealityConfigurationFound()
        {
            TestUtilities.EditorCreateScenes();
            MixedRealityToolkit mixedRealityToolkit = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
            MixedRealityToolkit.SetActiveInstance(mixedRealityToolkit);
            MixedRealityToolkit.ConfirmInitialized();

            MixedRealityToolkit.Instance.ActiveProfile = null;

            // Tests
            LogAssert.Expect(LogType.Warning, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");
            Assert.IsFalse(MixedRealityToolkit.Instance.HasActiveProfile);
            Assert.IsNull(MixedRealityToolkit.Instance.ActiveProfile);
        }

        [Test]
        public void TestCreateMixedRealityToolkit()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.Instance.GetServices<IMixedRealityService>());
        }

        #endregion Service Locator Tests

        #region IMixedRealityExtensionService Tests

        [Test]
        public void TestRegisterMixedRealityExtensionService()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1", 10, null));

            // Retrieve ITestExtensionService1
            Assert.IsNotNull(MixedRealityToolkit.Instance.GetService<IMixedRealityExtensionService>());
            Assert.IsNotNull(MixedRealityToolkit.Instance.GetService<ITestExtensionService1>());
            Assert.IsNotNull(MixedRealityToolkit.Instance.GetService<TestExtensionService1>());
            Assert.IsNotNull(MixedRealityToolkit.Instance.GetService<BaseExtensionService>());
            Assert.AreEqual(1, MixedRealityServiceRegistry.GetAllServices().Count);
        }

        [Test]
        public void TestUnregisterMixedRealityExtensionServiceByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1", 10, null));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.AreEqual(1, MixedRealityServiceRegistry.GetAllServices().Count);

            var success = MixedRealityToolkit.Instance.UnregisterService<ITestExtensionService1>();

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityServiceRegistry.GetAllServices());
        }

        [Test]
        public void TestUnregisterMixedRealityExtensionServiceByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Register ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1", 10, null));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.AreEqual(1, MixedRealityServiceRegistry.GetAllServices().Count);

            var success = MixedRealityToolkit.Instance.UnregisterService<ITestExtensionService1>(extensionService1.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityServiceRegistry.GetAllServices());
        }

        [Test]
        public void TestRegisterMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test ExtensionService 2", 10, null));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityExtensionService>();
            var serviceCount = MixedRealityServiceRegistry.GetAllServices().Count;

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.AreEqual(2, serviceCount);
            Assert.AreEqual(extensionServices.Count, serviceCount);
        }

        [Test]
        public void TestUnregisterMixedRealityExtensionServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test ExtensionService 2", 10, null));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityExtensionService>();
            var serviceCount = MixedRealityServiceRegistry.GetAllServices().Count;

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.AreEqual(2, serviceCount);
            Assert.AreEqual(extensionServices.Count, serviceCount);

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
            Assert.IsEmpty(MixedRealityServiceRegistry.GetAllServices());
        }

        [Test]
        public void TestMixedRealityExtensionService2DoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1", 10, null));

            // Validate non-existent ExtensionService
            var isServiceRegistered = MixedRealityToolkit.Instance.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            Assert.IsFalse(isServiceRegistered);
        }

        [Test]
        public void TestMixedRealityExtensionServiceDoesNotReturnByName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            const string serviceName = "Test ExtensionService 1";

            // Add test ITestExtensionService1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(serviceName, 10, null));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.Instance.GetService<ITestExtensionService2>(serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void TestValidateExtensionServiceName()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService 1
            string service1Name = "Test14-1";
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1(service1Name, 10, null));

            // Add test ExtensionService 2
            string service2Name = "Test14-2";
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2(service2Name, 10, null));

            // Retrieve Test ExtensionService 2-2
            var extensionService2 = MixedRealityToolkit.Instance.GetService<ITestExtensionService2>(service2Name);
            Assert.AreEqual(service2Name, extensionService2.Name);
            Assert.IsNotNull(MixedRealityToolkit.Instance.GetService<IMixedRealityExtensionService>(service2Name));

            // Retrieve Test ExtensionService 2-1
            var extensionService1 = MixedRealityToolkit.Instance.GetService<ITestExtensionService1>(service1Name);
            Assert.AreEqual(service1Name, extensionService1.Name);
        }

        [Test]
        public void TestGetMixedRealityExtensionServiceCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test ExtensionService 1
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test15-1", 10, null));

            // Add test ExtensionServices 2
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test15-2.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test15-2.2", 10, null));

            // Retrieve ExtensionService2
            var extensionServices = MixedRealityToolkit.Instance.GetServices<ITestExtensionService2>();

            // Tests
            Assert.AreEqual(2, extensionServices.Count);
        }

        [Test]
        public void TestGetAllMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Add test 1 services
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test16-1.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test16-1.2", 10, null));

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test16-2.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test16-2.2", 10, null));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.Instance.GetServices<IMixedRealityExtensionService>();

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityExtensionService Tests

        [Test]
        public void TestEnableServicesByType()
        {
            // Use the default profile, since we need an input system for this test.
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Add test 1 services
            TestExtensionService1 service1 = new TestExtensionService1("Test07-01-1.2", 10, null);
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(service1, "Test07-01-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(service1);

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService<ITestInputDataProvider>(new TestInputDataProvider(CoreServices.InputSystem, "Test07-01-2.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test07-01-2.2", 10, null));

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
        public void TestDisableServicesByType()
        {
            // Use the default profile, since we need an input system for this test.
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Add test 1 services
            TestExtensionService1 service1 = new TestExtensionService1("Test07-01-1.2", 10, null);
            MixedRealityToolkit.Instance.RegisterService<ITestDataProvider1>(new TestDataProvider1(service1, "Test07-01-1.1", 10));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(service1);

            // Add test 2 services
            MixedRealityToolkit.Instance.RegisterService<ITestInputDataProvider>(new TestInputDataProvider(CoreServices.InputSystem, "Test07-01-2.1", 10, null));
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test07-01-2.2", 10, null));

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

        /// <summary>
        /// This test validates that even when services are inserted in non-priority order
        /// (i.e. 20 -> 30 -> 10), the services are returned in ascending priority order
        /// when GetAllServices is called (i.e. 10, 20, 30).
        /// </summary>
        [Test]
        public void TestGetAllServicesAscendingOrder()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            ITestExtensionService1 service1 = new TestExtensionService1("Service1", 20, null);
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(service1);

            ITestExtensionService2 service2 = new TestExtensionService2("Service2", 30, null);
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(service2);

            ITestExtensionService3 service3 = new TestExtensionService3("Service3", 10, null);
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService3>(service3);

            // The order should be service3, service1, service2 because:
            // service3 priority = 10
            // service1 priority = 20
            // service2 priority = 30
            CollectionAssert.AreEqual(
                new List<IMixedRealityService>() { service3, service1, service2 },
                MixedRealityServiceRegistry.GetAllServices());
        }

        /// <summary>
        /// Similar to TestGetAllServicesAscendingOrder, except one of the services is then
        /// removed, and this validates that the remaining services are still sorted correctly.
        /// </summary>
        [Test]
        public void TestGetAllServicesAscendingOrderAfterRemoval()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            ITestExtensionService3 service3 = new TestExtensionService3("Service3", 10, null);
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService3>(service3);

            ITestExtensionService1 service1 = new TestExtensionService1("Service1", 20, null);
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService1>(service1);

            ITestExtensionService2 service2 = new TestExtensionService2("Service2", 30, null);
            MixedRealityToolkit.Instance.RegisterService<ITestExtensionService2>(service2);

            MixedRealityToolkit.Instance.UnregisterService<ITestExtensionService2>();

            // The order should be service3, service1 because:
            // service3 priority = 10
            // service1 priority = 20
            CollectionAssert.AreEqual(
                new List<IMixedRealityService>() { service3, service1 },
                MixedRealityServiceRegistry.GetAllServices());
        }

        #region Multiple Instances Tests

        [Test]
        public void TestCreateMultipleInstances()
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
        public void TestSwitchBetweenActiveInstances()
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
        public void TestDestroyActiveInstance()
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
        public void TestCreateMultipleInstancesInMultipleScenes()
        {
            TestUtilities.EditorCreateScenes(3);

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                SceneManager.SetActiveScene(scene);
                _ = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
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
    }
}
