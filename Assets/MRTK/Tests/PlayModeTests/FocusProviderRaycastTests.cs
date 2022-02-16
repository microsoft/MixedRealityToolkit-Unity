// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// This class is used to test that <see cref="Toolkit.Input.FocusProvider"/> raycasts are selecting the correct focus object.
    /// </summary>
    public class FocusProviderRaycastTests : BasePlayModeTests
    {
        private GameObject raycastTestPrefabInstance = null;
        private TestPointer pointer = null;
        IMixedRealityFocusProvider focusProvider = null;

        /// <summary>
        /// For each <see cref="FocusRaycastTestProxy="/> in the raycast test prefab, set the relevant values on the <see cref="TestPointer"/>,
        /// then wait for the <see cref="Toolkit.Input.FocusProvider.Update"/> and Assert that the <see cref="FocusRaycastTestProxy.ExpectedHitObject"/> matches
        /// the <see cref="TestPointer"/>'s <see cref="Toolkit.Input.IPointerResult.CurrentPointerTarget"/>.
        /// </summary>
        [UnityTest]
        public IEnumerator TestRaycastProxies()
        {
            Assert.IsNotNull(pointer, "FocusProvider is null!");

            foreach (var raycastTestProxy in raycastTestPrefabInstance.GetComponentsInChildren<FocusRaycastTestProxy>())
            {
                pointer.SetFromTestProxy(raycastTestProxy);
                yield return null;
                Assert.AreSame(raycastTestProxy.ExpectedHitObject, pointer.Result?.CurrentPointerTarget, "FAILED: " + raycastTestProxy.name);

                pointer.IsActive = false;
                yield return null;
                Assert.AreSame(null, pointer.Result?.CurrentPointerTarget, "Failed to clear pointer target after test.");
            }
        }

        public override IEnumerator Setup()
        {
            yield return base.Setup();

            focusProvider = PlayModeTestUtilities.GetInputSystem().FocusProvider;

            pointer = new TestPointer();
            focusProvider.RegisterPointer(pointer);

            GameObject raycastTestPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath("dc3b0cf66c0615e4c81d979f35d51eaa"));
            raycastTestPrefabInstance = Object.Instantiate(raycastTestPrefab);
            yield return null;
        }

        public override IEnumerator TearDown()
        {
            if (raycastTestPrefabInstance)
            {
                Object.DestroyImmediate(raycastTestPrefabInstance);
                raycastTestPrefabInstance = null;
            }

            focusProvider.UnregisterPointer(pointer);
            focusProvider = null;
            pointer = null;

            yield return base.TearDown();
        }
    }
}
#endif
