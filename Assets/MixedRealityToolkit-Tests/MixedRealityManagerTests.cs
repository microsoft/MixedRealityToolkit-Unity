// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class MixedRealityManagerTests
    {
        public static void CreateMixedRealityManager()
        {
            // Create The MR Manager
            GameObject go = GameObject.Find("Mixed Reality Manager");
            if (!go)
            {
                go = new GameObject("Mixed Reality Manager");
            }
            go.AddComponent<MixedRealityManager>();
        }

        [Test]
        public void Test01_CreateMixedRealityManager()
        {
            // Create The MR Manager
            CreateMixedRealityManager();
            GameObject go = GameObject.Find("Mixed Reality Manager");
            Assert.AreEqual("Mixed Reality Manager", go.name);
        }


        [Test]
        public void Test02_TestNoMixedRealityConfigurationFound()
        {
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Manager");
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);
        }

        [Test]
        public void Test03_CreateMixedRealityManager()
        {
            //Test the Manager
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);
            Assert.IsNotNull(MixedRealityManager.Instance);

            // Create Test Configuration
            Assert.IsEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.IsEmpty(MixedRealityManager.Instance.MixedRealityComponents);
        }

        [Test]
        public void Test04_CreateMixedRealityInputManager()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Tests
            Assert.IsNotNull(MixedRealityManager.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.AreEqual(MixedRealityManager.Instance.ActiveProfile.ActiveManagers.Count, 1);
            Assert.IsEmpty(MixedRealityManager.Instance.MixedRealityComponents);

        }

        [Test]
        public void Test05_TestGetMixedRealityInputManager()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Retrieve Input System
            var inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(inputSystem);
        }

        [Test]
        public void Test06_TestMixedRealityInputManagerDoesNotExist()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            //Check for Input System
            var inputSystemExists = MixedRealityManager.Instance.ManagerExists<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
        }


        [Test]
        public void Test07_TestMixedRealityInputManagerExists()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Check for Input System
            var inputSystemExists = MixedRealityManager.Instance.ManagerExists<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [Test]
        public void Test08_CreateMixedRealityComponent()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            var component = new TestComponentManager1();

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), component);

            // Tests
            Assert.IsNotNull(MixedRealityManager.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.AreEqual(MixedRealityManager.Instance.MixedRealityComponents.Count, 1);
        }

        [Test]
        public void Test09_TestMixedRealityComponentExists()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Retrieve Component1
            var component1 = MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager1));

            // Tests
            Assert.IsNotNull(component1);
        }

        [Test]
        public void Test10_TestMixedRealityComponents()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Retrieve Component1
            var components = MixedRealityManager.Instance.GetManagers(typeof(ITestComponentManager1));

            // Tests
            Assert.AreEqual(components.Count(), 3);
        }

        [Test]
        public void Test11_TestMixedRealityComponent2DoesNotReturn()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Validate non-existent component
            var component2 = (TestComponentManager2)MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager2), "Test2");

            // Tests
            Assert.IsNull(component2);
        }

        [Test]
        public void Test12_TestMixedRealityComponent2DoesNotExist()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            //Add test component 1
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Validate non-existent component
            var component2 = MixedRealityManager.Instance.ManagerExists<ITestComponentManager2>();

            // Tests
            Assert.IsFalse(component2);
        }

        [Test]
        public void Test13_CreateMixedRealityComponentNameWithInput()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Add test component 1
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Add test component 2
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test2-1" });

            // Tests
            Assert.IsNotNull(MixedRealityManager.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.AreEqual(MixedRealityManager.Instance.ActiveProfile.ActiveManagers.Count, 1);
            Assert.AreEqual(MixedRealityManager.Instance.MixedRealityComponents.Count, 7);
        }

        [Test]
        public void Test14_ValidateComponentNameWithInput()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Add test component 1
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Add test component 2
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test2-2" });

            //Retrieve Test component 2-2
            TestComponentManager2 component2_2 = (TestComponentManager2)MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager2), "Test2-2");

            // Component 2-2 Tests
            Assert.IsNotNull(component2_2.InputSystem);
            Assert.AreEqual(component2_2.Name, "Test2-2");

            //Retrieve Test component 2-1
            TestComponentManager2 component2_1 = (TestComponentManager2)MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager2), "Test2-1");

            // Component 2-1 Tests
            Assert.IsNotNull(component2_1.InputSystem);
            Assert.AreEqual(component2_1.Name, "Test2-1");

        }

        [Test]
        public void Test15_GetMixedRealityComponentsCollection()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            //Retrieve Component1
            var components = MixedRealityManager.Instance.GetManagers(typeof(ITestComponentManager2));

            // Tests
            Assert.AreEqual(components.Count(), 2);
        }

        [Test]
        public void Test16_GetAllMixedRealityComponents()
        {
            // Create The MR Manager & Test configuration
            CreateMixedRealityConfiguration(MixedRealityManager.Instance);

            //Retrieve Component1
            var allComponents = MixedRealityManager.Instance.MixedRealityComponents;

            // Tests
            Assert.AreEqual(allComponents.Count, 9);
        }

        #region Helper Functions

        private static void CreateMixedRealityConfiguration(MixedRealityManager mixedRealityManager)
        {
            var mrConfiguration = ScriptableObject.CreateInstance<MixedRealityConfigurationProfile>();
            mixedRealityManager.ActiveProfile = mrConfiguration;
        }

        #endregion
    }

    #region Test Components

    public interface ITestComponentManager1 : IMixedRealityManager { }

    public interface ITestComponentManager2 : IMixedRealityManager { }


    internal class TestComponentManager1 : BaseManager, ITestComponentManager1
    {
        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        public override void Initialize()
        {
            // TODO Initialize stuff 
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        public override void Update()
        {
            // TODO Update stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        public override void Reset()
        {
            // TODO React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// </summary>
        public override void Destroy()
        {
            // TODO Destroy stuff 
        }
    }

    internal class TestComponentManager2 : BaseManager, ITestComponentManager2
    {
        public IMixedRealityInputSystem InputSystem = null;

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        public override void Initialize()
        {
            // TODO Initialize stuff 
            InputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        public override void Update()
        {
            // TODO Update stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        public override void Reset()
        {
            // TODO React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// </summary>
        public override void Destroy()
        {
            // TODO Destroy stuff 
        }
    }
    #endregion
}