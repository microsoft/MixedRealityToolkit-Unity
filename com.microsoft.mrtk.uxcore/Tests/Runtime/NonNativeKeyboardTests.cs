// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        [SetUp]
        public void Init()
        {
            GameObject obj = new GameObject("Keyboard");
            obj.AddComponent<Canvas>();
            obj.SetActive(false);
            keyboard = obj.AddComponent<NonNativeKeyboard>();
            keyboard.Open();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(keyboard);
        }

        [UnityTest]
        public IEnumerator TestNonnativeValueKey()
        {
            NonNativeValueKey qkey = SetUpValueKey("q", true);
            yield return null;

            StatefulInteractable interactable = qkey.gameObject.GetComponentInChildren<StatefulInteractable>();
            interactable.OnClicked.Invoke();

            Assert.AreEqual(keyboard.Text.Substring(0,1), "q", "Pressing key changes InputField text.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNonnativeValueKeyShift()
        {
            NonNativeValueKey qkey = SetUpValueKey("q", false);
            NonNativeFunctionKey shiftkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
            yield return null;

            TMP_Text textMeshProText = qkey.gameObject.GetComponentInChildren<TMP_Text>();
            Assert.AreEqual(qkey.CurrentValue, "q", "Current value is set correctly");
            Assert.AreEqual(textMeshProText.text, "q", "TMP text is set correctly");
            keyboard.ProcessFunctionKeyPress(shiftkey);
            Assert.AreEqual(qkey.CurrentValue, "Q", "Current value shifts correctly");
            Assert.AreEqual(textMeshProText.text, "Q", "TMP text shifts correctly");
            keyboard.ProcessFunctionKeyPress(shiftkey);
            Assert.AreEqual(qkey.CurrentValue, "q", "Current value is unshifts correctly");
            Assert.AreEqual(textMeshProText.text, "q", "TMP text unshifts correctly");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNonnativeShiftFunctionKey()
        {
            NonNativeValueKey qkey = SetUpValueKey("q", true);
            NonNativeFunctionKey shiftkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
            yield return null;

            StatefulInteractable Qinteractable = qkey.gameObject.GetComponentInChildren<StatefulInteractable>();

            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "q", "Values not shifted to start with.");
            keyboard.ProcessFunctionKeyPress(shiftkey);
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQ", "The Shift function key works");
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQq", "Unshift works correctly");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNonnativeCapsLockFunctionKey()
        {
            NonNativeValueKey qkey = SetUpValueKey("q", true);
            NonNativeFunctionKey capslockkey = SetUpFunctionKey(NonNativeFunctionKey.Function.CapsLock);
            NonNativeFunctionKey shiftkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
            yield return null;

            StatefulInteractable Qinteractable = qkey.gameObject.GetComponentInChildren<StatefulInteractable>();

            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "q", "Values not shifted to start with.");
            keyboard.ProcessFunctionKeyPress(capslockkey);
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQ", "The CapsLock function key works");
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQQ", "The CapsLock state remains.");
            keyboard.ProcessFunctionKeyPress(shiftkey);
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "qQQq", "The CapsLock state clears properly on shift.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNonnativeSpaceFunctionKey()
        {
            PressFunctionKey(NonNativeFunctionKey.Function.Space);
            yield return null;
            Assert.AreEqual(keyboard.Text, "a b", "The Space function key works.");
        }

        [UnityTest]
        public IEnumerator TestNonnativeEnterFunctionKey()
        {
            PressFunctionKey(NonNativeFunctionKey.Function.Enter);
            yield return null;
            Assert.AreEqual(keyboard.Text, "a\nb", "The Enter function key works.");
        }

        [UnityTest]
        public IEnumerator TestNonnativeTabFunctionKey()
        {
            PressFunctionKey(NonNativeFunctionKey.Function.Tab);
            yield return null;
            Assert.AreEqual(keyboard.Text, "a\tb", "The Tab function key works.");
        }

        [UnityTest]
        public IEnumerator TestNonnativePreviousNextFunctionKeys()
        {
            NonNativeValueKey akey = SetUpValueKey("a", true);
            NonNativeValueKey bkey = SetUpValueKey("b", true);
            NonNativeValueKey ckey = SetUpValueKey("c", true);
            NonNativeFunctionKey prevkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Previous);
            NonNativeFunctionKey nextkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Next);
            yield return null;

            StatefulInteractable Ainteractable = akey.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable Binteractable = bkey.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable Cinteractable = ckey.gameObject.GetComponentInChildren<StatefulInteractable>();

            Ainteractable.OnClicked.Invoke();
            Cinteractable.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(prevkey);
            Binteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "abc", "The Previous function key works.");

            Ainteractable.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(nextkey);
            Ainteractable.OnClicked.Invoke();
            Assert.AreEqual(keyboard.Text, "abaca", "The Next function key works");

            yield return null;
        }

        private NonNativeValueKey SetUpValueKey(string value, bool statefulinteractable)
        {
            GameObject keyObj = new GameObject("ValueKey");
            keyObj.SetActive(false);
            keyObj.AddComponent<Button>();

            if(statefulinteractable)
            {
                keyObj.AddComponent<StatefulInteractable>();
            }

            NonNativeValueKey valuekey = keyObj.AddComponent<NonNativeValueKey>();
            valuekey.DefaultValue = value;
            valuekey.ShiftedValue = value.ToUpper();


            GameObject text = new GameObject("Text");
            text.transform.SetParent(valuekey.transform, false);
            text.AddComponent<TextMeshProUGUI>();

            keyObj.SetActive(true);

            return valuekey;
        }

        private NonNativeFunctionKey SetUpFunctionKey(NonNativeFunctionKey.Function function)
        {
            GameObject keyObj = new GameObject("ValueKey");
            keyObj.AddComponent<Button>();

            NonNativeFunctionKey functionkey = keyObj.AddComponent<NonNativeFunctionKey>();
            functionkey.KeyFunction = function;
            return functionkey;
        }

        private void PressFunctionKey(NonNativeFunctionKey.Function function)
        {
            NonNativeValueKey akey = SetUpValueKey("a", true);
            NonNativeValueKey bkey = SetUpValueKey("b", true);
            NonNativeFunctionKey functkey = SetUpFunctionKey(function);

            StatefulInteractable Ainteractable = akey.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable Binteractable = bkey.gameObject.GetComponentInChildren<StatefulInteractable>();
            Ainteractable.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(functkey);
            Binteractable.OnClicked.Invoke();
        }
    }
}
