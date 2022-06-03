// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

using GestureId = Microsoft.MixedReality.Toolkit.Input.GestureTypes.GestureId;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the Dialog UX component.
    /// </summary>
    public class DialogTests : BaseRuntimeInputTests
    {
        // UXComponents/Dialogs/Prefabs/Dialog_168x88mm.prefab
        private const string SmallDialogPrefabAssetGuid = "175cf7e8b8559f342806a0f7d7f3082a";
        private static readonly string SmallDialogPrefabAssetPath = AssetDatabase.GUIDToAssetPath(SmallDialogPrefabAssetGuid);

        private GameObject dialogGameObject;
        private Dialog dialogComponent;
        private DialogButtonType dialogResult;

        private const float DialogStabilizationTime = 0.2f;

        public override IEnumerator TearDown()
        {
            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
            yield return base.TearDown();
        }

        /// <summary>
        /// Tests the prefabs number of buttons
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogPrefabInitializations()
        {
            TestHand handRight = new TestHand(Handedness.Right);
            yield return handRight.Show(Vector3.zero);

            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.OK, true);
            yield return RuntimeTestUtilities.WaitForUpdates();
            // near distances determined by relevant properties on the dialog component
            float dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(DialogButtonType.OK, dialogComponent.Property.ButtonContexts[0].ButtonType);
            Assert.AreEqual("This is an example dialog", dialogComponent.Property.Message);
            Assert.AreEqual("Test Dialog", dialogComponent.Property.Title);
            Assert.IsTrue(dialogDistance > dialogComponent.FollowMinDistanceNear && dialogDistance < dialogComponent.FollowMaxDistanceNear);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // The dialog only supports displaying up to two options
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.YesNo, true);
            yield return RuntimeTestUtilities.WaitForUpdates();
            // near distances determined by relevant properties on the dialog component
            dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(DialogButtonType.Yes, dialogComponent.Property.ButtonContexts[0].ButtonType);
            Assert.AreEqual(DialogButtonType.No, dialogComponent.Property.ButtonContexts[1].ButtonType);
            Assert.IsTrue(dialogDistance > dialogComponent.FollowMinDistanceNear && dialogDistance < dialogComponent.FollowMaxDistanceNear);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
            yield return RuntimeTestUtilities.WaitForUpdates();

            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.YesNo, false);
            yield return RuntimeTestUtilities.WaitForUpdates();
            // near distances determined by relevant properties on the dialog component
            dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(DialogButtonType.Yes, dialogComponent.Property.ButtonContexts[0].ButtonType);
            Assert.AreEqual(DialogButtonType.No, dialogComponent.Property.ButtonContexts[1].ButtonType);
            Assert.IsTrue(dialogDistance > dialogComponent.FollowMinDistanceFar && dialogDistance < dialogComponent.FollowMaxDistanceFar);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
        }

        /// <summary>
        /// Tests the prefabs number of buttons
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogPrefabResults()
        {
            TestHand handRight = new TestHand(Handedness.Right);
            int handsteps = 60;

            // Testing near interactions
            yield return handRight.Show(new Vector3(0.0f, -0.06f, 0.3f));
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.OK, true);

            // Wait for the dialog to move to a stable position
            yield return new WaitForSeconds(DialogStabilizationTime);

            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(DialogState.WaitingForInput, dialogComponent.State);
            yield return handRight.MoveTo(new Vector3(0.0f, -0.06f, 0.5f), handsteps);
            yield return WaitForDialogClosedAndCheckResult(DialogButtonType.OK);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // The dialog only supports displaying up to two options
            yield return handRight.MoveTo(new Vector3(0.0f, -0.03f, 0.3f));
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.YesNo, true);

            // Wait for the dialog to move to a stable position
            yield return new WaitForSeconds(DialogStabilizationTime);

            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(DialogState.WaitingForInput, dialogComponent.State);
            yield return handRight.MoveTo(new Vector3(0.07f, -0.03f, 0.5f), handsteps);
            yield return WaitForDialogClosedAndCheckResult(DialogButtonType.No);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Testing far interactions
            // The dialog only supports displaying up to two options
            yield return handRight.MoveTo(new Vector3(0.0f, -0.03f, 0.3f));
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.YesNo, false);

            // Wait for the dialog to move to a stable position
            yield return new WaitForSeconds(DialogStabilizationTime);

            Assert.AreEqual(DialogState.WaitingForInput, dialogComponent.State);
            // moving the hand to an appropriate position to click on the dialog
            yield return handRight.MoveTo(new Vector3(0.04f, -0.14f, 0.3f), handsteps);
            yield return handRight.SetGesture(GestureId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return handRight.SetGesture(GestureId.Open);
            yield return WaitForDialogClosedAndCheckResult(DialogButtonType.Yes);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
        }

        /// <summary>
        /// Instantiates a dialog from the default prefab at position and rotation
        /// </summary>
        private void InstantiateFromPrefab(string title, string message, DialogButtonContext[] buttonContexts, bool isNearInteraction, Vector3? position = null, Quaternion? rotation = null)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SmallDialogPrefabAssetPath);

            dialogComponent = Dialog.InstantiateFromPrefab(prefab.GetComponent<Dialog>(), new DialogProperty(title, message, buttonContexts), isNearInteraction, true);
            Assert.IsNotNull(dialogComponent);

            dialogGameObject = dialogComponent.gameObject;
            Assert.IsNotNull(dialogGameObject);

            if (dialogComponent != null)
            {
                dialogComponent.OnClosed += OnClosedDialogEvent;
            }

            dialogGameObject.transform.SetPositionAndRotation(
                position ?? Vector3.forward,
                rotation ?? Quaternion.identity);
        }

        private void OnClosedDialogEvent(DialogProperty obj)
        {
            dialogResult = obj.ResultContext.ButtonType;
        }

        private IEnumerator WaitForDialogClosedAndCheckResult(DialogButtonType dialogButtonType)
        {
            while (dialogComponent.State != DialogState.Closed)
            {
                yield return null;
            }

            Assert.AreEqual(DialogState.Closed, dialogComponent.State);
            Assert.AreEqual(dialogButtonType, dialogResult);
        }
    }
}
