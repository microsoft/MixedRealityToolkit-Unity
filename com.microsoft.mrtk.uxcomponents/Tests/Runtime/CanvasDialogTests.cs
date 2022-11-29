// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the Canvas Dialog UX component.
    /// </summary>
    public class CanvasDialogTests : BaseRuntimeInputTests
    {
        // A dummy interactor used to test basic selection logic.
        private class TestInteractor : XRBaseInteractor { }

        // UXComponents/Dialog/CanvasDialog.prefab
        private const string CanvasDialogPrefabGUID = "cca6164bb2744884a92a100266f5f3aa";
        private static readonly string CanvasDialogPrefabAssetPath = AssetDatabase.GUIDToAssetPath(CanvasDialogPrefabGUID);

        private DialogSpawner spawner;
        private TestInteractor testInteractor;

        private const float DialogOpenTime = 0.2f;
        private const float DialogCloseTime = 0.5f;

        public override IEnumerator Setup()
        {
            yield return base.Setup();

            var spawnerObj = new GameObject("DialogSpawner");
            spawner = spawnerObj.AddComponent<DialogSpawner>();
            spawner.DialogPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CanvasDialogPrefabAssetPath);

            var interactorObj = new GameObject("TestInteractor");
            testInteractor = interactorObj.AddComponent<TestInteractor>();
        }
        
        public override IEnumerator TearDown()
        {
            UnityEngine.Object.Destroy(spawner.gameObject);
            UnityEngine.Object.Destroy(testInteractor.gameObject);
            yield return base.TearDown();
        }

        /// <summary>
        /// Tests whether the dialog can be spawned with no content other than the header.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogHeaderOnly()
        {
            IDialog dialog = spawner.Get()
                .SetHeader("This is a test header.")
                .Show();

            // Let's dig in and find the text object that corresponds to the header.
            // It should have the text properly set.
            // If we only set the header, it should be the only text object in the entire layout.
            TMP_Text textObject = dialog.VisibleRoot.GetComponentInChildren<TMP_Text>(false);

            Assert.IsNotNull(textObject, "No text object was found at all in the dialog after setting the header.");
            Assert.AreEqual("This is a test header.", textObject.text, "The header object did not have the expected text.");

            yield return null;
        }

        /// <summary>
        /// Tests whether the dialog can be spawned with no content other than the body.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogBodyOnly()
        {
            IDialog dialog = spawner.Get()
                .SetBody("This is a test body.")
                .Show();

            // Let's dig in and find the text object that corresponds to the body.
            // It should have the text properly set.
            // If we only set the header, it should be the only text object in the entire layout.
            TMP_Text textObject = dialog.VisibleRoot.GetComponentInChildren<TMP_Text>(false);

            Assert.IsNotNull(textObject, "No text object was found at all in the dialog after setting the body.");
            Assert.AreEqual("This is a test body.", textObject.text, "The body object did not have the expected text.");

            yield return null;
        }

        /// <summary>
        /// Tests whether the dialog can be spawned with one button, and one button only.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogSingleButton()
        {
            bool optionSelected = false;
            bool wasDismissed = false;

            IDialog dialog = spawner.Get()
                .SetHeader("This is a test header.")
                .SetBody("This is a test body.")
                .SetNeutral("OK", ( args ) => { optionSelected = true; })
                .Show();

            dialog.OnDismissed.AddListener(( args ) => { wasDismissed = true; });

            // How many buttons got spawned?
            PressableButton[] buttons = dialog.VisibleRoot.GetComponentsInChildren<PressableButton>(false);
            Assert.AreEqual(1, buttons.Length, "One and only one button should be present in the layout.");

            // Select the option, test the result.
            testInteractor.StartManualInteraction(buttons[0] as IXRSelectInteractable);
            yield return null;
            testInteractor.EndManualInteraction();

            Assert.IsTrue(optionSelected, "The dialog option was not selected.");

            // Wait for the dialog to close.
            yield return new WaitForSeconds(DialogCloseTime);

            Assert.IsTrue(wasDismissed, "The dialog was not dismissed.");

            // Dialog should be gone.
            Assert.IsFalse(dialog.VisibleRoot.activeInHierarchy, "Dialog shouldn't be active after dismissal.");
        }

        /// <summary>
        /// Tests whether the dialog correctly reports which option was selected. Also tests
        /// dialogs using all three selection types.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDialogOptions(
            [Values(
                DialogButtonType.Negative,
                DialogButtonType.Positive,
                DialogButtonType.Neutral)] DialogButtonType buttonToTest,
            [Values] bool pickWrong)
        {
            bool optionSelected = false;

            IDialog dialog = spawner.Get()
                .SetHeader("This is a test header.")
                .SetBody("This is a test body.")
                .SetNegative("No", ( args ) => optionSelected = (buttonToTest == DialogButtonType.Negative))
                .SetPositive("Yes", ( args ) => optionSelected = (buttonToTest == DialogButtonType.Positive))
                .SetNeutral("OK", ( args ) => optionSelected = (buttonToTest == DialogButtonType.Neutral))
                .Show();

            // How many buttons got spawned?
            PressableButton[] buttons = dialog.VisibleRoot.GetComponentsInChildren<PressableButton>(false);
            Assert.AreEqual(3, buttons.Length, "Three buttons should be present in the layout.");

            // We set them in the same order as the enum values,
            // so we can fetch them in hierarchy order as well.
            int buttonIdx = (int)buttonToTest;

            // Should we pick the "wrong" button? If so, pick the next one.
            buttonIdx = pickWrong ? (buttonIdx + 1) % buttons.Length : buttonIdx;

            // Select the option, test the result.
            testInteractor.StartManualInteraction(buttons[buttonIdx] as IXRSelectInteractable);
            yield return null;
            testInteractor.EndManualInteraction();

            if (pickWrong)
            {
                Assert.IsFalse(optionSelected, $"The dialog option ({buttonToTest}) was selected, but it shouldn't have been.");
            }
            else
            {
                Assert.IsTrue(optionSelected, $"The dialog option ({buttonToTest}) was not selected, but it should have been.");
            }
        }

        /// <summary>
        /// Tests whether the dialog can be spawned with one button, and one button only.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMultiDialogPolicy()
        {
            bool wasDismissed = false;

            IDialog dialog = spawner.Get()
                .SetHeader("This is a test header.")
                .SetBody("This is a test body.")
                .SetNeutral("OK", ( args ) => {  })
                .Show();

            dialog.OnDismissed.AddListener(( args ) => { wasDismissed = true; });

            IDialog anotherDialog = spawner.Get(spawnPolicy: DialogSpawner.Policy.DismissExisting)
                .SetHeader("This is a test header.")
                .SetBody("This is a test body.")
                .SetNeutral("OK", ( args ) => {  })
                .Show();

            // Wait for the dialog to close.
            yield return new WaitForSeconds(DialogCloseTime);

            Assert.IsTrue(wasDismissed, "The previous dialog was not dismissed.");

            IDialog thirdDialog = spawner.Get(spawnPolicy: DialogSpawner.Policy.AbortIfExisting);

            Assert.IsNull(thirdDialog, "The third dialog should not have been spawned.");

            yield return null;
        }
    }
}
