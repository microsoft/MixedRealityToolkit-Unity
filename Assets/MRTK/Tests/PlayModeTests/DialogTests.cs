// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    public class DialogTests
    {
        // SDK/Features/UX/Prefabs/Slate/Slate.prefab
        private const string smallDialogPrefabAssetGuid = "8e686c90124b8e14cbf8093893109e9a";
        private static readonly string slatePrefabAssetPath = AssetDatabase.GUIDToAssetPath(smallDialogPrefabAssetGuid);

        private GameObject dialogGameObject;
        private Dialog dialogComponent;
        private DialogButtonType dialogResult;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            GameObject.Destroy(dialogGameObject);
            GameObject.Destroy(dialogComponent);
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        /// <summary>
        /// Tests the prefabs number of buttons
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogPrefabInitializations()
        {
            TestHand handRight = new TestHand(Handedness.Right);
            float dialogDistance;
            handRight.Show(Vector3.zero);

            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonType.OK, true);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            // near distances determined by the Dialog.Open() function
            dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(dialogComponent.Result.Buttons, DialogButtonType.OK);
            Assert.AreEqual(dialogComponent.Result.Message, "This is an example dialog");
            Assert.AreEqual(dialogComponent.Result.Title, "Test Dialog");
            Assert.AreEqual(dialogComponent.Result.Buttons, DialogButtonType.OK);
            Assert.IsTrue(dialogDistance > 0.5f && dialogDistance < 1.0f);

            GameObject.Destroy(dialogGameObject);
            GameObject.Destroy(dialogComponent);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // The dialog only supports displaying up to two options
            InstantiateFromPrefab("Test Dialog", "This is an example dialog",DialogButtonType.Yes | DialogButtonType.No, true);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            // near distances determined by the Dialog.Open() function
            dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(dialogComponent.Result.Buttons, DialogButtonType.Yes | DialogButtonType.No);
            Assert.IsTrue(dialogDistance > 0.5f && dialogDistance < 1.0f);

            GameObject.Destroy(dialogGameObject);
            GameObject.Destroy(dialogComponent);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonType.Yes | DialogButtonType.No, false);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            // near distances determined by the Dialog.Open() function
            dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(dialogComponent.Result.Buttons, DialogButtonType.Yes | DialogButtonType.No);
            Assert.IsTrue(dialogDistance > 1.0f && dialogDistance < 2.0f);

            GameObject.Destroy(dialogGameObject);
            GameObject.Destroy(dialogComponent);
        }

        /// <summary>
        /// Tests the prefabs number of buttons
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogPrefabResults()
        {
            TestHand handRight = new TestHand(Handedness.Right);
            int handsteps = 30;

            // Testing near interactions
            handRight.Show(Vector3.zero);
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonType.OK, true);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(dialogComponent.State == DialogState.WaitingForInput);
            yield return handRight.Move(new Vector3(0.0f, -0.1f, 1.0f), handsteps);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.AreEqual(dialogResult, DialogButtonType.OK);
            Assert.AreEqual(dialogComponent.State, DialogState.Closed);

            GameObject.Destroy(dialogGameObject);
            GameObject.Destroy(dialogComponent);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // The dialog only supports displaying up to two options
            yield return handRight.MoveTo(Vector3.zero);
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonType.Yes | DialogButtonType.No, true);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(dialogComponent.State == DialogState.WaitingForInput);
            yield return handRight.Move(new Vector3(0.1f, -0.1f, 1.0f), handsteps);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.AreEqual(dialogResult, DialogButtonType.No);
            Assert.AreEqual(dialogComponent.State, DialogState.Closed);

            GameObject.Destroy(dialogGameObject);
            GameObject.Destroy(dialogComponent);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Testing far interactions
            // The dialog only supports displaying up to two options
            yield return handRight.MoveTo(Vector3.zero);
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonType.Yes | DialogButtonType.No, false);
            // Wait for the dialog to move to a stable position
            yield return new WaitForSeconds(0.5f);
            Assert.IsTrue(dialogComponent.State == DialogState.WaitingForInput);
            // moving the hand to an appropriate position to click on the dialog
            yield return handRight.Move(new Vector3(-0.1f, -0.2f, 0.7f), handsteps);
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();


            Assert.AreEqual(dialogResult, DialogButtonType.Yes);
            Assert.AreEqual(dialogComponent.State, DialogState.Closed);

            GameObject.Destroy(dialogGameObject);
            GameObject.Destroy(dialogComponent);
        }

        /*
        /// <summary>
        /// Test hand ray scroll instantiated from prefab
        /// </summary>
        [UnityTest]
        public IEnumerator PrefabRayScroll()
        {
            InstantiateFromPrefab();
            Vector2 totalPanDelta = Vector2.zero;
            dialogComponent.PanUpdated.AddListener((hpd) => totalPanDelta += hpd.PanDelta);

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
            yield return handRight.MoveTo(dialogComponent.transform.position, 10);

            TestHand handLeft = new TestHand(Handedness.Left);
            yield return handLeft.Show(Vector3.zero);

            yield return handLeft.MoveTo(dialogComponent.transform.position + Vector3.right * -0.01f, 10);
            yield return handRight.Move(new Vector3(0.01f, 0f, 0f));
            yield return handLeft.Move(new Vector3(-0.01f, 0f, 0f));

            Assert.AreEqual(0.4, dialogComponent.CurrentScale, 0.1, "slate did not zoom in using two finger touch");

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

            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            TestHand handRight = new TestHand(Handedness.Right);
            yield return handRight.Show(new Vector3(0.0f, 0.0f, 0.6f));

            TestHand handLeft = new TestHand(Handedness.Left);
            yield return handLeft.Show(new Vector3(-0.1f, 0.0f, 0.6f));

            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return handLeft.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            yield return handRight.Move(new Vector3(0.005f, 0.0f, 0.0f), 10);
            yield return handLeft.Move(new Vector3(-0.005f, 0.0f, 0.0f), 10);

            Assert.AreEqual(0.5, dialogComponent.CurrentScale, 0.1, "slate did not zoom in using two ggv hands");

            yield return handRight.Hide();
            yield return handLeft.Hide();

            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
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
            yield return handRight.Show(dialogComponent.transform.position + Vector3.forward * -0.1f + Vector3.right * -0.3f);
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Left hand pinches slate
            TestHand handLeft = new TestHand(Handedness.Left);
            yield return handLeft.Show(dialogComponent.transform.position + Vector3.forward * 0.1f + Vector3.right * -0.3f);
            yield return handLeft.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Use both hands to zoom in
            yield return handRight.Move(new Vector3(0f, 0f, -0.1f), 5);
            yield return handLeft.Move(new Vector3(0f, 0f, 0.1f), 5);

            Assert.AreEqual(0.6, dialogComponent.CurrentScale, 0.1, "Rotated slate did not zoom in using near interaction");

            // Reset slate and hands configuration
            dialogComponent.Reset();
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return handLeft.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return null;

            // Both hands touch slate
            yield return handRight.MoveTo(dialogComponent.transform.position + Vector3.forward * -0.1f);
            yield return handLeft.MoveTo(dialogComponent.transform.position + Vector3.forward * 0.1f);
            yield return null;

            // Use both hands to zoom in
            yield return handRight.Move(new Vector3(0f, 0f, -0.1f), 5);
            yield return handLeft.Move(new Vector3(0f, 0f, 0.1f), 5);

            Assert.AreEqual(0.6, dialogComponent.CurrentScale, 0.1, "Rotated slate did not zoom in using far interaction");

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
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            Vector2 totalPanDelta = Vector2.zero;
            dialogComponent.PanUpdated.AddListener((hpd) => totalPanDelta += hpd.PanDelta);

            TestHand handRight = new TestHand(Handedness.Right);
            yield return handRight.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return handRight.Show(Vector3.zero);

            // Move requires a slower action (i.e. higher numSteps) in order to
            // achieve a reliable pan.
            yield return handRight.Move(new Vector3(0.0f, -0.1f, 0f), 30);

            Assert.AreEqual(expectedScroll, totalPanDelta.y, 0.1, "pan delta is not correct");

            yield return handRight.Hide();

            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }
        */

        /*
        private void InstantiateFromCode(Vector3? position = null, Quaternion? rotation = null)
        {
            dialogGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            dialogGameObject.EnsureComponent<BoxCollider>();
            dialogGameObject.transform.position = position != null ? (Vector3)position : Vector3.forward;
            dialogGameObject.transform.rotation = rotation != null ? (Quaternion)rotation : Quaternion.identity;
            dialogComponent = dialogGameObject.AddComponent<HandInteractionPanZoom>();
            dialogGameObject.AddComponent<NearInteractionTouchable>();
        }
        */

        /// <summary>
        /// Instantiates a slate from the default prefab at position and rotation
        /// </summary>
        private void InstantiateFromPrefab(string title, string message, DialogButtonType buttonType, bool isNearInteraction, Vector3? position = null, Quaternion? rotation = null)
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(slatePrefabAssetPath, typeof(GameObject));

            dialogComponent = Dialog.Open(prefab, buttonType, title, message, isNearInteraction);
            Assert.IsNotNull(dialogComponent);

            dialogGameObject = dialogComponent.gameObject;
            Assert.IsNotNull(dialogGameObject);

            if (dialogComponent != null)
            {
                dialogComponent.OnClosed += OnClosedDialogEvent;
            }

            dialogGameObject.transform.position = position != null ? (Vector3)position : Vector3.forward;
            dialogGameObject.transform.rotation = rotation != null ? (Quaternion)rotation : Quaternion.identity;
        }

        private void OnClosedDialogEvent(DialogResult obj)
        {
            dialogResult = obj.Result;
        }
    }
}
#endif
