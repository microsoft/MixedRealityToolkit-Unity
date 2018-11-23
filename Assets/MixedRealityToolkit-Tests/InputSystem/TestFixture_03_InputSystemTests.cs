// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public class TestFixture_03_InputSystemTests
    {
        [Test]
        public void Test01_CreateMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityToolkit.Instance.ActiveProfile.ActiveServices);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.ActiveProfile.ActiveServices.Count);
            Assert.AreEqual(0, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test02_TestGetMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Retrieve Input System
            var inputSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(inputSystem);
        }

        [Test]
        public void Test03_TestMixedRealityInputSystemDoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
        }

        [Test]
        public void Test04_TestMixedRealityInputSystemExists()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.RegisterService(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }
    }
}