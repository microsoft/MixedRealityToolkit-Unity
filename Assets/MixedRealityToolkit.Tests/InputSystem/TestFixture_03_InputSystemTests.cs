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
        [Test]
        public void Test01_CreateMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Create a Input System Profiles
            var inputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            inputSystemProfile.FocusProviderType = typeof(FocusProvider);
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
            MixedRealityToolkit.Instance.RegisterService<IMixedRealityInputSystem>(new MixedRealityInputSystem(MixedRealityToolkit.Instance, MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile, MixedRealityToolkit.Instance.MixedRealityPlayspace));

            // Tests
            Assert.IsNotEmpty(MixedRealityToolkit.Instance.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.Instance.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.Instance.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test02_TestGetMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

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
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(IMixedRealityInputSystem).Name} service.");
        }

        [Test]
        public void Test04_TestMixedRealityInputSystemExists()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

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