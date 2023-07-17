// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for tests. While nice to have, this documentation is not required.
#pragma warning disable CS1591

using System.Collections;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using Microsoft.MixedReality.Toolkit.UX.Experimental;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the NonNative keyboard
    /// </summary>
    public class NonNativeKeyboardTests : BaseRuntimeInputTests
    {
        private NonNativeKeyboard keyboard = null;

        /// <summary>
        /// Initialize the non-native keyboard tests by creating a game object with a <see cref="NonNativeKeyboard"/> component,
        /// and then opening this component.
        /// </summary>
        [SetUp]
        public void Init()
        {
            GameObject obj = new GameObject("Keyboard");
            obj.AddComponent<Canvas>();
            obj.SetActive(false);
            keyboard = obj.AddComponent<NonNativeKeyboard>();
            keyboard.Open();
        }

        /// <summary>
        /// Clean-up the non-native keyboard tests by destroying the game object with the <see cref="NonNativeKeyboard"/> component.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            Object.Destroy(keyboard);
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> component correctly handles an alpha key value.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativeValueKey()
        {
            NonNativeValueKey keyQ = SetUpValueKey("q", true);
            yield return null;

            StatefulInteractable interactable = keyQ.gameObject.GetComponentInChildren<StatefulInteractable>();
            interactable.OnClicked.Invoke();

            Assert.AreEqual(keyboard.Text.Substring(0,1), "q", "Pressing key changes InputField text.");

            yield return null;
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> component correctly handles an alpha key value with a shift
        /// key modifier.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativeValueKeyShift()
        {
            NonNativeValueKey keyQ = SetUpValueKey("q", false);
            NonNativeFunctionKey keyShift = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
            yield return null;

            TMP_Text textMeshProText = keyQ.gameObject.GetComponentInChildren<TMP_Text>();
            Assert.AreEqual(keyQ.CurrentValue, "q", "Current value is set correctly");
            Assert.AreEqual(textMeshProText.text, "q", "TMP text is set correctly");
            keyboard.ProcessFunctionKeyPress(keyShift);
            Assert.AreEqual(keyQ.CurrentValue, "Q", "Current value shifts correctly");
            Assert.AreEqual(textMeshProText.text, "Q", "TMP text shifts correctly");
            keyboard.ProcessFunctionKeyPress(keyShift);
            Assert.AreEqual(keyQ.CurrentValue, "q", "Current value should handle releasing the shift keys correctly");
            Assert.AreEqual(textMeshProText.text, "q", "TMP text should handle releasing the shift keys correctly");

            yield return null;
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> text property correctly handles an alpha key value with a shift
        /// key modifier, and then without the modifier.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativeShiftFunctionKey()
        {
            NonNativeValueKey keyQ = SetUpValueKey("q", true);
            NonNativeFunctionKey keyShift = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
            yield return null;

            StatefulInteractable interactableQ = keyQ.gameObject.GetComponentInChildren<StatefulInteractable>();

            interactableQ.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "q", "Values not shifted to start with.");
            keyboard.ProcessFunctionKeyPress(keyShift);
            interactableQ.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQ", "The Shift function key works");
            interactableQ.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQq", "Releasing the shift key should work correctly");
            yield return null;
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> text property correctly handles an alpha key value with the caps
        /// lock key modifier.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativeCapsLockFunctionKey()
        {
            NonNativeValueKey keyQ = SetUpValueKey("q", true);
            NonNativeFunctionKey keyCapsLock = SetUpFunctionKey(NonNativeFunctionKey.Function.CapsLock);
            NonNativeFunctionKey keyShift = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
            yield return null;

            StatefulInteractable interactableQ = keyQ.gameObject.GetComponentInChildren<StatefulInteractable>();

            interactableQ.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "q", "Values not shifted to start with.");
            keyboard.ProcessFunctionKeyPress(keyCapsLock);
            interactableQ.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQ", "The CapsLock function key works");
            interactableQ.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQQ", "The CapsLock state remains.");
            keyboard.ProcessFunctionKeyPress(keyShift);
            interactableQ.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQQq", "The CapsLock state clears properly on shift.");

            yield return null;
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> text property correctly handles the space key.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativeSpaceFunctionKey()
        {
            PressFunctionKey(NonNativeFunctionKey.Function.Space);
            yield return null;
            Assert.AreEqual(keyboard.Text, "a b", "The Space function key works.");
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> text property correctly handles the enter key.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativeEnterFunctionKey()
        {
            PressFunctionKey(NonNativeFunctionKey.Function.Enter);
            yield return null;
            Assert.AreEqual(keyboard.Text, "a\nb", "The Enter function key works.");
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> text property correctly handles the tab key.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativeTabFunctionKey()
        {
            PressFunctionKey(NonNativeFunctionKey.Function.Tab);
            yield return null;
            Assert.AreEqual(keyboard.Text, "a\tb", "The Tab function key works.");
        }

        /// <summary>
        /// Test that the <see cref="NonNativeKeyboard"/> text property correctly handles the previous and next keys.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonnativePreviousNextFunctionKeys()
        {
            NonNativeValueKey keyA = SetUpValueKey("a", true);
            NonNativeValueKey keyB = SetUpValueKey("b", true);
            NonNativeValueKey keyC = SetUpValueKey("c", true);
            NonNativeFunctionKey prevKey = SetUpFunctionKey(NonNativeFunctionKey.Function.Previous);
            NonNativeFunctionKey nextKey = SetUpFunctionKey(NonNativeFunctionKey.Function.Next);
            yield return null;

            StatefulInteractable interactableA = keyA.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable interactableB = keyB.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable interactableC = keyC.gameObject.GetComponentInChildren<StatefulInteractable>();

            interactableA.OnClicked.Invoke();
            interactableC.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(prevKey);
            interactableB.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "abc", "The Previous function key works.");

            interactableA.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(nextKey);
            interactableA.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "abaca", "The Next function key works");

            yield return null;
        }

        private NonNativeValueKey SetUpValueKey(string value, bool statefulInteractable)
        {
            GameObject keyObj = new GameObject("ValueKey");
            keyObj.SetActive(false);
            keyObj.AddComponent<Button>();

            if(statefulInteractable)
            {
                keyObj.AddComponent<StatefulInteractable>();
            }

            NonNativeValueKey valueKey = keyObj.AddComponent<NonNativeValueKey>();
            valueKey.DefaultValue = value;
            valueKey.ShiftedValue = value.ToUpper();


            GameObject text = new GameObject("Text");
            text.transform.SetParent(valueKey.transform, false);
            text.AddComponent<TextMeshProUGUI>();

            keyObj.SetActive(true);

            return valueKey;
        }

        private NonNativeFunctionKey SetUpFunctionKey(NonNativeFunctionKey.Function function)
        {
            GameObject keyObj = new GameObject("ValueKey");
            keyObj.AddComponent<Button>();

            NonNativeFunctionKey functionKey = keyObj.AddComponent<NonNativeFunctionKey>();
            functionKey.KeyFunction = function;
            return functionKey;
        }

        private void PressFunctionKey(NonNativeFunctionKey.Function function)
        {
            NonNativeValueKey keyA = SetUpValueKey("a", true);
            NonNativeValueKey keyB = SetUpValueKey("b", true);
            NonNativeFunctionKey functionKey = SetUpFunctionKey(function);

            StatefulInteractable interactableA = keyA.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable interactableB = keyB.gameObject.GetComponentInChildren<StatefulInteractable>();
            interactableA.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(functionKey);
            interactableB.OnClicked.Invoke();
        }
    }
}
#pragma warning restore CS1591