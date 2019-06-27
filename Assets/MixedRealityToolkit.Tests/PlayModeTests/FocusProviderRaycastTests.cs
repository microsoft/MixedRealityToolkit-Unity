// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class FocusProviderRaycastTests
    {
        private GameObject raycastTestPrefabInstance = null;
        private TestPointer pointer = null;
        IMixedRealityFocusProvider focusProvider = null;

        [UnityTest]
        public IEnumerator TestRaycastProxies()
        {
            Assert.IsNotNull(pointer, "FocusProvider is null!");

            foreach (var raycastTestProxy in raycastTestPrefabInstance.GetComponentsInChildren<FocusRaycastTestProxy>())
            {
                pointer.SetFromTestProxy(raycastTestProxy);
                yield return null;
                yield return null;
                Assert.AreSame(raycastTestProxy.ExpectedHitObject, pointer.Result?.CurrentPointerTarget, "FAILED: " + raycastTestProxy.name);
            }
        }

        [SetUp]
        public void SetupMrtk()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            MixedRealityServiceRegistry.TryGetService(out focusProvider);

            pointer = new TestPointer();
            focusProvider.RegisterPointer(pointer);
            
            GameObject raycastTestPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine("Assets", "MixedRealityToolkit.Tests", "PlayModeTests", "Prefabs", "FocusProviderRaycastTest.prefab"));
            raycastTestPrefabInstance = Object.Instantiate(raycastTestPrefab);
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            if (raycastTestPrefabInstance)
            {
                Object.DestroyImmediate(raycastTestPrefabInstance);
                raycastTestPrefabInstance = null;
            }

            focusProvider.UnregisterPointer(pointer);
            focusProvider = null;
            pointer = null;

            TestUtilities.ShutdownMixedRealityToolkit();
        }
    }
}
#endif
