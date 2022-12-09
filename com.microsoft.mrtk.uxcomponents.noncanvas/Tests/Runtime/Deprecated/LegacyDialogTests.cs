// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

using ObsoleteAttribute = System.ObsoleteAttribute;
using DialogButtonType = Microsoft.MixedReality.Toolkit.UX.Deprecated.DialogButtonType;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated.Runtime.Tests
{
    /// <summary>
    /// Tests for the Static Dialog UX component.
    /// </summary>
    [Obsolete("Tests for obsolete Dialog component. Will be removed in a future release.")]
    public class LegacyDialogTests : BaseRuntimeInputTests
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
            UnityEngine.Object.Destroy(dialogGameObject);
            UnityEngine.Object.Destroy(dialogComponent);
            yield return base.TearDown();
        }

        /// <summary>
        /// Tests the prefabs number of buttons
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogPrefabInitializations()
        {
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.OK, true);
            yield return RuntimeTestUtilities.WaitForUpdates();
            // near distances determined by relevant properties on the dialog component
            float dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(DialogButtonType.OK, dialogComponent.Property.ButtonContexts[0].ButtonType);
            Assert.AreEqual("This is an example dialog", dialogComponent.Property.Message);
            Assert.AreEqual("Test Dialog", dialogComponent.Property.Title);

            // TODO: Reinstate when properly-scaling Dialogs are reintroduced.
            // Assert.IsTrue(dialogDistance > dialogComponent.FollowMinDistanceNear && dialogDistance < dialogComponent.FollowMaxDistanceNear);

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

            // TODO: Reinstate when properly-scaling Dialogs are reintroduced.
            // Assert.IsTrue(dialogDistance > dialogComponent.FollowMinDistanceNear && dialogDistance < dialogComponent.FollowMaxDistanceNear);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
            yield return RuntimeTestUtilities.WaitForUpdates();

            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.YesNo, false);
            yield return RuntimeTestUtilities.WaitForUpdates();
            // near distances determined by relevant properties on the dialog component
            dialogDistance = dialogGameObject.transform.position.magnitude;
            Assert.AreEqual(DialogButtonType.Yes, dialogComponent.Property.ButtonContexts[0].ButtonType);
            Assert.AreEqual(DialogButtonType.No, dialogComponent.Property.ButtonContexts[1].ButtonType);

            // TODO: Reinstate when properly-scaling Dialogs are reintroduced.
            // Assert.IsTrue(dialogDistance > dialogComponent.FollowMinDistanceFar && dialogDistance < dialogComponent.FollowMaxDistanceFar);

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
            yield return handRight.Show(Vector3.zero);

            // Testing near interactions
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.OK, true);

            // Wait for the dialog to move to a stable position
            yield return new WaitForSeconds(DialogStabilizationTime);

            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(DialogState.WaitingForInput, dialogComponent.State);

            // Make sure the OK button works.
            yield return PokeAndCheckResult(handRight, DialogButtonType.OK);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // The dialog only supports displaying up to two options
            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.YesNo, true);

            // Wait for the dialog to move to a stable position
            yield return new WaitForSeconds(DialogStabilizationTime);

            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(DialogState.WaitingForInput, dialogComponent.State);

            // Makes sure the No button works.
            yield return PokeAndCheckResult(handRight, DialogButtonType.No);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);

            InstantiateFromPrefab("Test Dialog", "This is an example dialog", DialogButtonHelpers.YesNo, true);

            // Wait for the dialog to move to a stable position
            yield return new WaitForSeconds(DialogStabilizationTime);

            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(DialogState.WaitingForInput, dialogComponent.State);

            // Makes sure the Yes button works.
            yield return PokeAndCheckResult(handRight, DialogButtonType.Yes);

            Object.Destroy(dialogGameObject);
            Object.Destroy(dialogComponent);

            // TODO: Re-introduce far-interaction tests for Dialogs once properly-scaling dialogs are reintroduced.
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

        private IEnumerator PokeAndCheckResult(TestHand hand, DialogButtonType dialogButtonType)
        {
            // Go find the button so we know where to poke with the finger.
            foreach (DialogButton button in dialogComponent.GetComponentsInChildren<DialogButton>())
            {
                if (button.ButtonContext.ButtonType == dialogButtonType)
                {
                    yield return hand.MoveTo(button.transform.position - button.transform.forward * 0.05f);
                    yield return hand.MoveTo(button.transform.position + button.transform.forward * 0.05f);
                    yield return WaitForDialogClosedAndCheckResult(dialogButtonType);
                    break;
                }
            }
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
