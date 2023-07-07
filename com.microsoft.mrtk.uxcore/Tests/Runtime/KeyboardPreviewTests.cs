// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the NonNative keyboard
    /// </summary>
    public class KeyboardPreviewTests : BaseRuntimeInputTests
    {
        private KeyboardPreview keyboardPreview = null;

        [SetUp]
        public void Init()
        {
            GameObject obj = new GameObject("KeyboardPreview");
            obj.AddComponent<Canvas>();
            obj.SetActive(false);
            keyboardPreview = obj.AddComponent<KeyboardPreview>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(keyboardPreview);
        }

        [UnityTest]
        public IEnumerator TestCreation()
        {
            Assert.IsNotNull(keyboardPreview, "KeyboardPreview was not created.");
            yield return Initialize();
            Assert.IsNotNull(keyboardPreview, "KeyboardPreview was destroyed unexpectedly.");
        }

        [UnityTest]
        public IEnumerator TestWithNoLabel()
        {
            yield return Initialize();

            int defaultCaretIndex = 0;
            string defaultText = string.Empty;
            Assert.AreEqual(defaultText, keyboardPreview.Text, "Keyboard Preview text should be an empty string at the start.");
            Assert.AreEqual(defaultCaretIndex, keyboardPreview.CaretIndex, "Keyboard Preview caret index should be zero.");

            string text1 = "Sample Text";
            keyboardPreview.Text = text1;
            Assert.AreEqual(text1, keyboardPreview.Text, "Keyboard Preview text was not applied correctly.");
            Assert.AreEqual(defaultCaretIndex, keyboardPreview.CaretIndex, "Keyboard Preview caret index should not have changed with text change.");

            int caretIndex1 = 2;
            keyboardPreview.CaretIndex = caretIndex1;
            Assert.AreEqual(text1, keyboardPreview.Text, "Keyboard Preview text should not have changed with caret index change.");
            Assert.AreEqual(caretIndex1, keyboardPreview.CaretIndex, "Keyboard Preview caret index did not change.");

            int caretIndex3TooBig = text1.Length + 1;
            keyboardPreview.CaretIndex = caretIndex3TooBig;
            Assert.AreEqual(text1, keyboardPreview.Text, "Keyboard Preview text should not have changed with caret index change.");
            Assert.AreEqual(text1.Length, keyboardPreview.CaretIndex, "Keyboard Preview caret index did not get clamped to 0 - Text.length when assigned value was too big.");

            int caretIndex3TooSmall = -1;
            keyboardPreview.CaretIndex = caretIndex3TooSmall;
            Assert.AreEqual(text1, keyboardPreview.Text, "Keyboard Preview text should not have changed with caret index change.");
            Assert.AreEqual(0, keyboardPreview.CaretIndex, "Keyboard Preview caret index did not get clamped to 0 - Text.length when assigned value was too small.");

            int caretIndex2 = text1.Length;
            keyboardPreview.CaretIndex = caretIndex2;
            Assert.AreEqual(text1, keyboardPreview.Text, "Keyboard Preview text should not have changed with caret index change.");
            Assert.AreEqual(caretIndex2, keyboardPreview.CaretIndex, "Keyboard Preview caret index did not change to end of string.");

            string text2 = "A different text string";
            keyboardPreview.Text = text2;
            Assert.AreEqual(text2, keyboardPreview.Text, "Keyboard Preview text was not applied correctly the second time.");
            Assert.AreEqual(caretIndex2, keyboardPreview.CaretIndex, "Keyboard Preview caret index should have changed.");

            string text3Small = "Small";
            keyboardPreview.Text = text3Small;
            Assert.AreEqual(text3Small, keyboardPreview.Text, "Keyboard Preview text was not applied correctly the third time.");
            Assert.AreEqual(text3Small.Length, keyboardPreview.CaretIndex, "Keyboard Preview caret index did not get clamped to 0 - Text.length when assigned text was made smaller.");
        }

        private IEnumerator Initialize()
        {
            keyboardPreview.gameObject.SetActive(false);
            yield return null;
        }
    }
}
