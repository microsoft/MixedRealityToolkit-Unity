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

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SlateTests
    {
        // SDK/Features/UX/Prefabs/Slate/Slate.prefab
        private const string slatePrefabAssetGuid = "937ce507dd7ee334ba569554e24adbdd";
        private static readonly string slatePrefabAssetPath = AssetDatabase.GUIDToAssetPath(slatePrefabAssetGuid);

        private GameObject panObject;
        private HandInteractionPanZoom panZoom;

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            PlayModeTestUtilities.PushHandSimulationProfile();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(panObject);
            GameObject.Destroy(panZoom);
            PlayModeTestUtilities.PopHandSimulationProfile();
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Tests touch scrolling instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator PrefabTouchScroll()
        {
            InstantiateFromPrefab();
            Vector2 totalPanDelta = Vector2.zero;
            panZoom.PanUpdated.AddListener((hpd) => totalPanDelta += hpd.PanDelta);

            TestHand handRight = new TestHand(Handedness.Right); ;
            yield return handRight.MoveTo(panObject.transform.position);
            yield return handRight.Move(new Vector3(0, -0.05f, 0), 10);

            Assert.AreEqual(0.1, totalPanDelta.y, 0.05, "pan delta is not correct");

            yield return handRight.Hide();
        }

        /// <summary>
        /// Test hand ray scroll instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator PrefabRayScroll()
        {
            InstantiateFromPrefab();
            Vector2 totalPanDelta = Vector2.zero;
            panZoom.PanUpdated.AddListener((hpd) => totalPanDelta += hpd.PanDelta);

            TestHand handRight = new TestHand(Handedness.Right);
            Vector3 screenPoint = CameraCache.Main.ViewportToScreenPoint(new Vector3(0.5f, 0.25f, 0.5f));
            yield return handRight.Show(CameraCache.Main.ScreenToWorldPoint(screenPoint));

            Assert.True(PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out Vector3 hitPointStart));

            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return handRight.Move(new Vector3(0, -0.05f, 0), 10);

            Assert.True(PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out Vector3 hitPointEnd));

            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Open);

            TestUtilities.AssertNotAboutEqual(hitPointStart, hitPointEnd, "ray should not stick on slate scrolling");
            Assert.AreEqual(0.1, totalPanDelta.y, 0.05, "pan delta is not correct");

            yield return handRight.Hide();
        }

        /// <summary>
        /// Test touch zooming instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator PrefabTouchZoom()
        {
            InstantiateFromPrefab();

            TestHand handRight = new TestHand(Handedness.Right);
            yield return handRight.Show(Vector3.zero);
            yield return handRight.MoveTo(panZoom.transform.position, 10);

            TestHand handLeft = new TestHand(Handedness.Left);
            yield return handLeft.Show(Vector3.zero);

            yield return handLeft.MoveTo(panZoom.transform.position + Vector3.right * -0.01f, 10);
            yield return handRight.Move(new Vector3(0.01f, 0f, 0f));
            yield return handLeft.Move(new Vector3(-0.01f, 0f, 0f));

            Assert.AreEqual(0.4, panZoom.CurrentScale, 0.1, "slate did not zoom in using two finger touch");

            yield return handRight.Hide();
            yield return handLeft.Hide();
        }

        /// <summary>
        /// Test ggv (gaze, gesture, and voice) zooming instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator PrefabGGVZoom()
        {
            InstantiateFromPrefab();

            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            TestHand handRight = new TestHand(Handedness.Right);
            yield return handRight.Show(new Vector3(0.0f, 0.0f, 0.6f));

            TestHand handLeft = new TestHand(Handedness.Left);
            yield return handLeft.Show(new Vector3(-0.1f, 0.0f, 0.6f));

            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return handLeft.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            yield return handRight.Move(new Vector3(0.005f, 0.0f, 0.0f), 10);
            yield return handLeft.Move(new Vector3(-0.005f, 0.0f, 0.0f), 10);

            Assert.AreEqual(0.5, panZoom.CurrentScale, 0.1, "slate did not zoom in using two ggv hands");

            yield return handRight.Hide();
            yield return handLeft.Hide();
        }

        /// <summary>
        /// Test zooming in using far and near interaction on a slate that is rotated 90 degrees around up vector.
        /// This test guarantees that the z component of the hand or controller position is being considered on the zooming logic.
        /// </summary>
        [UnityTest]
        public IEnumerator ZoomRotatedSlate()
        {
            // Configuring camera and hands to interact with rotated slate
            InstantiateFromPrefab(Vector3.right, Quaternion.LookRotation(Vector3.right));
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 90));
            yield return null;

            // Right hand pinches slate
            TestHand handRight = new TestHand(Handedness.Right);
            yield return handRight.Show(panZoom.transform.position + Vector3.forward * -0.1f + Vector3.right * -0.3f);
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Left hand pinches slate
            TestHand handLeft = new TestHand(Handedness.Left);
            yield return handLeft.Show(panZoom.transform.position + Vector3.forward * 0.1f + Vector3.right * -0.3f);
            yield return handLeft.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Use both hands to zoom in
            yield return handRight.Move(new Vector3(0f, 0f, -0.1f), 5);
            yield return handLeft.Move(new Vector3(0f, 0f, 0.1f), 5);

            Assert.AreEqual(0.6, panZoom.CurrentScale, 0.1, "Rotated slate did not zoom in using near interaction");

            // Reset slate and hands configuration
            panZoom.Reset();
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return handLeft.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return null;

            // Both hands touch slate
            yield return handRight.MoveTo(panZoom.transform.position + Vector3.forward * -0.1f);
            yield return handLeft.MoveTo(panZoom.transform.position + Vector3.forward * 0.1f);
            yield return null;

            // Use both hands to zoom in
            yield return handRight.Move(new Vector3(0f, 0f, -0.1f), 5);
            yield return handLeft.Move(new Vector3(0f, 0f, 0.1f), 5);

            Assert.AreEqual(0.6, panZoom.CurrentScale, 0.1, "Rotated slate did not zoom in using far interaction");

            yield return handRight.Hide();
            yield return handLeft.Hide();
        }

        /// <summary>
        /// Test ggv scroll instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator PrefabGGVScroll()
        {
            InstantiateFromPrefab();
            yield return RunGGVScrollTest(0.25f);
        }

        /// <summary>
        /// Test hand ray scroll instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator InstantiateGGVScroll()
        {
            InstantiateFromCode();
            yield return RunGGVScrollTest(0.08f);
        }

        /// <summary>
        /// Scroll a slate using GGV and ensure that the slate scrolls
        /// expected amount. Assumes panZoom has already been created.
        /// </summary>
        /// <param name="expectedScroll">The amount panZoom is expected to scroll</param>
        private IEnumerator RunGGVScrollTest(float expectedScroll)
        {
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            Vector2 totalPanDelta = Vector2.zero;
            panZoom.PanUpdated.AddListener((hpd) => totalPanDelta += hpd.PanDelta);

            TestHand handRight = new TestHand(Handedness.Right);
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return handRight.Show(Vector3.zero);

            // Move requires a slower action (i.e. higher numSteps) in order to
            // achieve a reliable pan.
            yield return handRight.Move(new Vector3(0.0f, -0.1f, 0f), 30);

            Assert.AreEqual(expectedScroll, totalPanDelta.y, 0.1, "pan delta is not correct");

            yield return handRight.Hide();
        }

        private void InstantiateFromCode(Vector3? position = null, Quaternion? rotation = null)
        {
            panObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            panObject.EnsureComponent<BoxCollider>();
            panObject.transform.position = position != null ? (Vector3)position : Vector3.forward;
            panObject.transform.rotation = rotation != null ? (Quaternion)rotation : Quaternion.identity;
            panZoom = panObject.AddComponent<HandInteractionPanZoom>();
            panObject.AddComponent<NearInteractionTouchable>();

        }

        /// <summary>
        /// Instantiates a slate from the default prefab at position and rotation
        /// </summary>
        private void InstantiateFromPrefab(Vector3? position = null, Quaternion? rotation = null)
        {
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(slatePrefabAssetPath, typeof(UnityEngine.Object));
            panObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
            Assert.IsNotNull(panObject);
            panObject.transform.position = position != null ? (Vector3)position : Vector3.forward;
            panObject.transform.rotation = rotation != null ? (Quaternion)rotation : Quaternion.identity;
            panZoom = panObject.GetComponentInChildren<HandInteractionPanZoom>();
            Assert.IsNotNull(panZoom);
        }
    }
}
#endif
