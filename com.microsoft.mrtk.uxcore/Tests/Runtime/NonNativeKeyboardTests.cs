// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using TMPro;
using UnityEditor;
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

            GameObject inputObj = new GameObject("InputField");
            inputObj.transform.SetParent(obj.transform, false);
            MRTKTMPInputField inputfield = inputObj.AddComponent<MRTKTMPInputField>();

            GameObject textObj = new GameObject("InputText");
            textObj.transform.SetParent(inputObj.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

            keyboard = obj.AddComponent<NonNativeKeyboard>();
            keyboard.InputField = inputfield;
            inputfield.textComponent = text;

            obj.SetActive(true);
            inputfield.textComponent.ForceMeshUpdate(true);

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

            MRTKTMPInputField inputfield = keyboard.gameObject.GetComponentInChildren<MRTKTMPInputField>();
            Assert.AreEqual(inputfield.textComponent.text.Substring(0,1), "q", "Pressing key changes InputField text.");

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

            MRTKTMPInputField inputfield = keyboard.gameObject.GetComponentInChildren<MRTKTMPInputField>();
            StatefulInteractable Qinteractable = qkey.gameObject.GetComponentInChildren<StatefulInteractable>();

            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "q", "Values not shifted to start with.");
            keyboard.ProcessFunctionKeyPress(shiftkey);
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "qQ", "The Shift function key works");
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "qQq", "Unshift works correctly");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNonnativeCapsLockFunctionKey()
        {
            NonNativeValueKey qkey = SetUpValueKey("q", true);
            NonNativeFunctionKey capslockkey = SetUpFunctionKey(NonNativeFunctionKey.Function.CapsLock);
            NonNativeFunctionKey shiftkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
            yield return null;

            MRTKTMPInputField inputfield = keyboard.gameObject.GetComponentInChildren<MRTKTMPInputField>();
            StatefulInteractable Qinteractable = qkey.gameObject.GetComponentInChildren<StatefulInteractable>();

            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "q", "Values not shifted to start with.");
            keyboard.ProcessFunctionKeyPress(capslockkey);
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "qQ", "The CapsLock function key works");
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "qQQ", "The CapsLock state remains.");
            keyboard.ProcessFunctionKeyPress(shiftkey);
            Qinteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "qQQq", "The CapsLock state clears properly on shift.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNonnativeSpaceFunctionKey()
        {
            MRTKTMPInputField inputfield = PressFunctionKey(NonNativeFunctionKey.Function.Space);
            yield return null;

            Assert.AreEqual(inputfield.text, "a b", "The Space function key works.");
        }

        [UnityTest]
        public IEnumerator TestNonnativeEnterFunctionKey()
        {
            MRTKTMPInputField inputfield = PressFunctionKey(NonNativeFunctionKey.Function.Enter);
            yield return null;

            Assert.AreEqual(inputfield.text, "a\nb", "The Enter function key works.");
        }

        [UnityTest]
        public IEnumerator TestNonnativeTabFunctionKey()
        {
            MRTKTMPInputField inputfield = PressFunctionKey(NonNativeFunctionKey.Function.Tab);
            yield return null;

            Assert.AreEqual(inputfield.text, "a\tb", "The Tab function key works.");
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

            MRTKTMPInputField inputfield = keyboard.gameObject.GetComponentInChildren<MRTKTMPInputField>();
            StatefulInteractable Ainteractable = akey.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable Binteractable = bkey.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable Cinteractable = ckey.gameObject.GetComponentInChildren<StatefulInteractable>();

            Ainteractable.OnClicked.Invoke();
            Cinteractable.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(prevkey);
            Binteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "abc", "The Previous function key works.");

            Ainteractable.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(nextkey);
            Ainteractable.OnClicked.Invoke();
            Assert.AreEqual(inputfield.text, "abaca", "The Next function key works");

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

        private MRTKTMPInputField PressFunctionKey(NonNativeFunctionKey.Function function)
        {
            NonNativeValueKey akey = SetUpValueKey("a", true);
            NonNativeValueKey bkey = SetUpValueKey("b", true);
            NonNativeFunctionKey functkey = SetUpFunctionKey(function);

            StatefulInteractable Ainteractable = akey.gameObject.GetComponentInChildren<StatefulInteractable>();
            StatefulInteractable Binteractable = bkey.gameObject.GetComponentInChildren<StatefulInteractable>();
            Ainteractable.OnClicked.Invoke();
            keyboard.ProcessFunctionKeyPress(functkey);
            Binteractable.OnClicked.Invoke();

            return keyboard.gameObject.GetComponentInChildren<MRTKTMPInputField>();
        }
    }
}