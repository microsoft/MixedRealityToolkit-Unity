// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public class TestFixture_03_InputSystemTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        [Test]
        public void Test01_CreateMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Create a Input System Profiles
            var inputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            inputSystemProfile.FocusProviderType = typeof(FocusProvider);
            inputSystemProfile.RaycastProviderType = typeof(DefaultRaycastProvider);
            inputSystemProfile.InputActionsProfile = ScriptableObject.CreateInstance<MixedRealityInputActionsProfile>();
            inputSystemProfile.InputActionRulesProfile = ScriptableObject.CreateInstance<MixedRealityInputActionRulesProfile>();
            inputSystemProfile.PointerProfile = ScriptableObject.CreateInstance<MixedRealityPointerProfile>();
            inputSystemProfile.PointerProfile.GazeProviderType = typeof(GazeProvider);
            inputSystemProfile.GesturesProfile = ScriptableObject.CreateInstance<MixedRealityGesturesProfile>();
            inputSystemProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<MixedRealitySpeechCommandsProfile>();
            inputSystemProfile.ControllerVisualizationProfile = ScriptableObject.CreateInstance<MixedRealityControllerVisualizationProfile>();
            inputSystemProfile.ControllerMappingProfile = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfile>();

            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile = inputSystemProfile;

            // Add Input System
            bool didRegister = MixedRealityToolkit.Instance.RegisterService<IMixedRealityInputSystem>(new MixedRealityInputSystem(MixedRealityToolkit.Instance, MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile));

            // Tests
            Assert.IsTrue(didRegister);
            Assert.AreEqual(1, MixedRealityServiceRegistry.GetAllServices().Count);
            Assert.IsNotNull(MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>());
        }

        [Test]
        public void Test02_TestGetMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Retrieve Input System
            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);

            // Tests
            Assert.IsNotNull(inputSystem);
        }

        [Test]
        public void Test03_TestMixedRealityInputSystemDoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
        }

        [Test]
        public void Test04_TestMixedRealityInputSystemExists()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.Instance.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [Test]
        public void Test05_TestMixedRealityInputSystemDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Check for Input System
            var inputSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>();
            Assert.IsNotNull(inputSystem);

            var dataProviderAccess = (inputSystem as IMixedRealityDataProviderAccess);
            Assert.IsNotNull(dataProviderAccess);

            // Tests
            Assert.IsEmpty(dataProviderAccess.GetDataProviders());
        }

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.EditorCreateScenes();
        }
    }
}