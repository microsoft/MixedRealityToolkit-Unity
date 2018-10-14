// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class MixedRealityManagerTests
    {
        public static void CreateMixedRealityManager()
        {
            // Create The MR Manager
            MixedRealityOrchestrator.ConfirmInitialized();
        }

        [Test]
        public void Test01_CreateMixedRealityManager()
        {
            CleanupScene();

            // Create The MR Manager
            CreateMixedRealityManager();
            GameObject manager = GameObject.Find(nameof(MixedRealityOrchestrator));
            Assert.AreEqual(nameof(MixedRealityOrchestrator), manager.name);
        }

        [Test]
        public void Test02_TestNoMixedRealityConfigurationFound()
        {
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Orchestrator");

            MixedRealityOrchestrator.Instance.ActiveProfile = null;

            Assert.IsFalse(MixedRealityOrchestrator.HasActiveProfile);
            Assert.IsNull(MixedRealityOrchestrator.Instance.ActiveProfile);

            CleanupScene();
        }

        [Test]
        public void Test03_CreateMixedRealityManager()
        {
            InitializeMixedRealityManager();

            // Create Test Configuration
            Assert.AreEqual(0, MixedRealityOrchestrator.Instance.ActiveProfile.ActiveServices.Count);
            Assert.AreEqual(3, MixedRealityOrchestrator.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test04_CreateMixedRealityInputManager()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Tests
            Assert.IsNotNull(MixedRealityOrchestrator.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityOrchestrator.Instance.ActiveProfile.ActiveServices);
            Assert.AreEqual(1, MixedRealityOrchestrator.Instance.ActiveProfile.ActiveServices.Count);
            Assert.AreEqual(3, MixedRealityOrchestrator.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test05_TestGetMixedRealityInputManager()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Retrieve Input System
            var inputSystem = MixedRealityOrchestrator.Instance.GetService<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(inputSystem);
        }

        [Test]
        public void Test06_TestMixedRealityInputManagerDoesNotExist()
        {
            InitializeMixedRealityManager();

            // Check for Input System
            var inputSystemExists = MixedRealityOrchestrator.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
        }


        [Test]
        public void Test07_TestMixedRealityInputManagerExists()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Check for Input System
            var inputSystemExists = MixedRealityOrchestrator.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [Test]
        public void Test08_CreateMixedRealityComponent()
        {
            InitializeMixedRealityManager();

            var component = new TestComponentManager1();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), component);

            // Tests
            Assert.AreEqual(4, MixedRealityOrchestrator.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test09_TestMixedRealityComponentExists()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1());

            // Retrieve Component1
            var component1 = MixedRealityOrchestrator.Instance.GetService(typeof(ITestComponentManager1));

            // Tests
            Assert.IsNotNull(component1);
        }

        [Test]
        public void Test10_TestMixedRealityComponents()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1());
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1());
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1());

            // Retrieve Component1
            var components = MixedRealityOrchestrator.Instance.GetActiveServices(typeof(ITestComponentManager1));

            // Tests
            Assert.AreEqual(3, components.Count());
        }

        [Test]
        public void Test11_TestMixedRealityComponent2DoesNotReturn()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1());

            try
            {
                // Validate non-existent component
                MixedRealityOrchestrator.Instance.GetService(typeof(ITestComponentManager2), "Test2");
                Assert.Fail();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Test]
        public void Test12_TestMixedRealityComponent2DoesNotExist()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1());

            // Validate non-existent component
            var component2 = MixedRealityOrchestrator.Instance.IsServiceRegistered<ITestComponentManager2>();

            // Tests
            Assert.IsFalse(component2);
        }

        [Test]
        public void Test13_CreateMixedRealityComponentNameWithInput()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Add test component 1
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1());

            //Add test component 2
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test2-1" });

            // Tests
            Assert.IsNotNull(MixedRealityOrchestrator.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityOrchestrator.Instance.ActiveProfile.ActiveServices);
            Assert.AreEqual(1, MixedRealityOrchestrator.Instance.ActiveProfile.ActiveServices.Count);
            Assert.AreEqual(5, MixedRealityOrchestrator.Instance.MixedRealityComponents.Count);
        }

        [Test]
        public void Test14_ValidateComponentNameWithInput()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1 { Name = "Test14-1" });

            // Add test component 2
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test14-2" });

            // Retrieve Test component 2-2
            TestComponentManager2 component2 = (TestComponentManager2)MixedRealityOrchestrator.Instance.GetService(typeof(ITestComponentManager2), "Test14-2");

            // Component 2-2 Tests
            Assert.IsNotNull(component2.InputSystem);
            Assert.AreEqual("Test14-2", component2.Name);

            // Retrieve Test component 2-1
            TestComponentManager1 component1 = (TestComponentManager1)MixedRealityOrchestrator.Instance.GetService(typeof(ITestComponentManager1), "Test14-1");

            // Component 2-1 Tests
            Assert.IsNotNull(component1.InputSystem);
            Assert.AreEqual("Test14-1", component1.Name);
        }

        [Test]
        public void Test15_GetMixedRealityComponentsCollection()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1 { Name = "Test15-1" });

            // Add test components 2
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test15-2.1" });
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test15-2.2" });

            // Retrieve Component2
            var components = MixedRealityOrchestrator.Instance.GetActiveServices(typeof(ITestComponentManager2));

            // Tests
            Assert.AreEqual(2, components.Count());
        }

        [Test]
        public void Test16_GetAllMixedRealityComponents()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityOrchestrator.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Add test component 1
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1 { Name = "Test16-1.1" });
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager1), new TestComponentManager1 { Name = "Test16-1.2" });

            // Add test components 2
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test16-2.1" });
            MixedRealityOrchestrator.Instance.RegisterService(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test16-2.2" });

            // Retrieve Component1
            var allComponents = MixedRealityOrchestrator.Instance.MixedRealityComponents;

            // Tests
            Assert.AreEqual(7, allComponents.Count);
        }

        [Test]
        public void Test17_CleanupMixedRealityManager()
        {
            CleanupScene();
        }

        #region Helper Functions

        private static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        private static void InitializeMixedRealityManager()
        {
            CleanupScene();
            CreateMixedRealityManager();

            // Test the Manager
            Assert.IsNotNull(MixedRealityOrchestrator.Instance);
            var configuration = ScriptableObject.CreateInstance<MixedRealityConfigurationProfile>();
            MixedRealityOrchestrator.Instance.ActiveProfile = configuration;
            Assert.NotNull(MixedRealityOrchestrator.Instance.ActiveProfile);
        }

        #endregion Helper Functions
    }

    #region Test Components

    public interface ITestComponentManager1 : IMixedRealityService { }

    public interface ITestComponentManager2 : IMixedRealityService { }

    internal class TestComponentManager1 : BaseService, ITestComponentManager1
    {
        public IMixedRealityInputSystem InputSystem = null;

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Orchestrator.
        /// </summary>
        public override void Initialize()
        {
            InputSystem = MixedRealityOrchestrator.Instance.GetService<IMixedRealityInputSystem>();
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        public override void Update()
        {
            // TODO Update stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Orchestrator is replaced
        /// </summary>
        public override void Reset()
        {
            // TODO React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Orchestrator is destroyed
        /// </summary>
        public override void Destroy()
        {
            // TODO Destroy stuff 
        }
    }

    internal class TestComponentManager2 : BaseService, ITestComponentManager2
    {
        public IMixedRealityInputSystem InputSystem = null;

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Orchestrator.
        /// </summary>
        public override void Initialize()
        {
            InputSystem = MixedRealityOrchestrator.Instance.GetService<IMixedRealityInputSystem>();
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        public override void Update()
        {
            // TODO Update stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Orchestrator is replaced
        /// </summary>
        public override void Reset()
        {
            // TODO React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Orchestrator is destroyed
        /// </summary>
        public override void Destroy()
        {
            // TODO Destroy stuff 
        }
    }

    #endregion Test Components
}