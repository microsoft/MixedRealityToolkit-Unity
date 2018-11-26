// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public static class TestUtilities
    {
        public static void InitializeMixedRealityToolkit()
        {
            MixedRealityToolkit.ConfirmInitialized();
        }

        public static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        public static void InitializeMixedRealityToolkitScene()
        {
            // Setup
            CleanupScene();
            Assert.IsTrue(!MixedRealityToolkit.IsInitialized);
            InitializeMixedRealityToolkit();

            // Tests
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            var configuration = ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();
            MixedRealityToolkit.Instance.ActiveProfile = configuration;
            Assert.NotNull(MixedRealityToolkit.Instance.ActiveProfile);
        }
    }
}