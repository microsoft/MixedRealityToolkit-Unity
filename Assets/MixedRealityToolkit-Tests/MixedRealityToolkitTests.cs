// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using NUnit.Framework;
using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class MixedRealityToolkitTests
    {
        public static void InitializeMixedRealityToolkit()
        {
            MixedRealityToolkit.ConfirmInitialized();
        }

        [Test]
        public void Test01_InitializeMixedRealityToolkit()
        {
            CleanupScene();

            InitializeMixedRealityToolkit();
            GameObject gameObject = GameObject.Find(nameof(MixedRealityToolkit));
            Assert.AreEqual(nameof(MixedRealityToolkit), gameObject.name);
        }

        [Test]
        public void Test02_TestNoMixedRealityConfigurationFound()
        {
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");

            MixedRealityToolkit.Instance.ActiveProfile = null;

            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);
            Assert.IsNull(MixedRealityToolkit.Instance.ActiveProfile);

            CleanupScene();
        }

        [Test]
        public void Test03_CreateMixedRealityOrchestrator()
        {
            InitializeMixedRealityToolkitScene();

            // Create Test Configuration
            Assert.AreEqual(0, MixedRealityToolkit.Instance.ActiveProfile.ActiveServices.Count);
            Assert.AreEqual(0, MixedRealityToolkit.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test04_CreateMixedRealityInputSystem()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityToolkit.Instance.ActiveProfile.ActiveServices);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.ActiveProfile.ActiveServices.Count);
            Assert.AreEqual(0, MixedRealityToolkit.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test05_TestGetMixedRealityInputSystem()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Retrieve Input System
            var inputSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(inputSystem);
        }

        [Test]
        public void Test06_TestMixedRealityInputSystemDoesNotExist()
        {
            InitializeMixedRealityToolkitScene();

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
        }


        [Test]
        public void Test07_TestMixedRealityInputSystemExists()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [Test]
        public void Test08_CreateMixedRealityComponent()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1());

            // Tests
            Assert.AreEqual(1, MixedRealityToolkit.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test09_TestMixedRealityComponentExists()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1());

            // Retrieve Component1
            var component1 = MixedRealityToolkit.Instance.GetService(typeof(ITestComponent1));

            // Tests
            Assert.IsNotNull(component1);
        }

        [Test]
        public void Test10_TestMixedRealityComponents()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1());
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2());
            MixedRealityToolkit.Instance.RegisterService(typeof(IFailComponent), new TestFailComponent());
            LogAssert.Expect(LogType.Error, $"Unable to register {typeof(IFailComponent)}. Concrete type does not implement the IMixedRealityExtensionService implementation.");

            // Retrieve all registered IMixedRealityExtensionServices
            var components = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealityExtensionService));

            // Tests
            Assert.AreEqual(2, components.Count);
        }

        [Test]
        public void Test11_TestMixedRealityComponent2DoesNotReturn()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1());

            // Validate non-existent component
            MixedRealityToolkit.Instance.GetService(typeof(ITestComponent2), "Test2");
            LogAssert.Expect(LogType.Error, "Unable to find Test2 Manager.");
        }

        [Test]
        public void Test12_TestMixedRealityComponent2DoesNotExist()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1());

            // Validate non-existent component
            var component2 = MixedRealityToolkit.Instance.IsServiceRegistered<ITestComponent2>();

            // Tests
            Assert.IsFalse(component2);
        }

        [Test]
        public void Test13_CreateMixedRealityComponentNameWithInput()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Add test component 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1());

            //Add test component 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2 { Name = "Test2-1" });

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityToolkit.Instance.ActiveProfile.ActiveServices);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.ActiveProfile.ActiveServices.Count);
            Assert.AreEqual(2, MixedRealityToolkit.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test14_ValidateComponentNameWithInput()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1 { Name = "Test14-1" });

            // Add test component 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2 { Name = "Test14-2" });

            // Retrieve Test component 2-2
            TestComponent2 component2 = (TestComponent2)MixedRealityToolkit.Instance.GetService(typeof(ITestComponent2), "Test14-2");

            // Component 2-2 Tests
            Assert.IsNotNull(component2.InputSystem);
            Assert.AreEqual("Test14-2", component2.Name);

            // Retrieve Test component 2-1
            TestComponent1 component1 = (TestComponent1)MixedRealityToolkit.Instance.GetService(typeof(ITestComponent1), "Test14-1");

            // Component 2-1 Tests
            Assert.IsNotNull(component1.InputSystem);
            Assert.AreEqual("Test14-1", component1.Name);
        }

        [Test]
        public void Test15_GetMixedRealityComponentsCollection()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1 { Name = "Test15-1" });

            // Add test components 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2 { Name = "Test15-2.1" });
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2 { Name = "Test15-2.2" });

            // Retrieve Component2
            var components = MixedRealityToolkit.Instance.GetActiveServices(typeof(ITestComponent2));

            // Tests
            Assert.AreEqual(2, components.Count);
        }

        [Test]
        public void Test16_GetAllMixedRealityComponents()
        {
            InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1 { Name = "Test16-1.1" });
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent1), new TestComponent1 { Name = "Test16-1.2" });

            // Add test components 2
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2 { Name = "Test16-2.1" });
            MixedRealityToolkit.Instance.RegisterService(typeof(ITestComponent2), new TestComponent2 { Name = "Test16-2.2" });

            // Retrieve Component1
            var allComponents = MixedRealityToolkit.Instance.MixedRealityComponents;

            // Tests
            Assert.AreEqual(4, allComponents.Count);
        }

        [Test]
        public void Test17_CleanupMixedRealityOrchestrator()
        {
            CleanupScene();
        }

        #region Helper Functions

        private static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        private static void InitializeMixedRealityToolkitScene()
        {
            // Setup
            CleanupScene();
            InitializeMixedRealityToolkit();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            var configuration = ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();
            MixedRealityToolkit.Instance.ActiveProfile = configuration;
            Assert.NotNull(MixedRealityToolkit.Instance.ActiveProfile);
        }

        #endregion Helper Functions
    }

    #region Test Components

    public interface ITestComponent1 : IMixedRealityExtensionService { }

    public interface ITestComponent2 : IMixedRealityExtensionService { }

    internal class TestComponent1 : BaseService, ITestComponent1
    {
        public IMixedRealityInputSystem InputSystem = null;

        /// <summary>
        /// The initialize function is used to setup the service once created.
        /// This method is called once all services have been registered in the Mixed Reality Toolkit.
        /// </summary>
        public override void Initialize()
        {
            InputSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>();
        }
    }

    internal class TestComponent2 : BaseService, ITestComponent2
    {
        public IMixedRealityInputSystem InputSystem = null;

        /// <summary>
        /// The initialize function is used to setup the service once created.
        /// This method is called once all services have been registered in the Mixed Reality Toolkit.
        /// </summary>
        public override void Initialize()
        {
            InputSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>();
        }
    }

    internal interface IFailComponent : IMixedRealityService { }

    internal class TestFailComponent : BaseService, IFailComponent { }

    #endregion Test Components
}