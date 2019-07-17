// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SlateTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(panObject);
            GameObject.Destroy(panZoom);
            PlayModeTestUtilities.TearDown();
        }

        
        const string slatePrefabAssetPath = "Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Slate/Slate.prefab";

        GameObject panObject;
        HandInteractionPanZoom panZoom;

        /// <summary>
        /// Tests touch scrolling instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator Prefab_TouchScroll()
        {
            InstantiateFromPrefab(Vector3.forward);
            Vector2 totalPanDelta = Vector2.zero;
            panZoom.PanUpdated.AddListener((hpd) => totalPanDelta += hpd.PanDelta);

            TestHand h = new TestHand(Handedness.Right); ;
            yield return h.MoveTo(panObject.transform.position);
            yield return h.Move(new Vector3(0, -0.05f, 0), 10);

            Assert.AreEqual(totalPanDelta.y, 0.1f, "pan delta is not correct");

            yield return h.Hide();
            yield return new WaitForSeconds(0.5f);
        }

        /// <summary>
        /// Instantiates a slate from the default prefab at position, looking at the camera
        /// </summary>
        /// <returns></returns>
        private void InstantiateFromPrefab(Vector3 position)
        {
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(slatePrefabAssetPath, typeof(UnityEngine.Object));
            panObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
            Assert.IsNotNull(panObject);
            panObject.transform.position = position;
            // g.transform.LookAt(CameraCache.Main.transform.position);
            panZoom = panObject.GetComponentInChildren<HandInteractionPanZoom>();
            Assert.IsNotNull(panZoom);
        }

    }
}
#endif