// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
	/// <summary>
	/// Tests for the Canvas Non-Native keyboard prefab. 
	/// </summary>
	public class NonNativeKeyboardTests : BaseRuntimeInputTests
	{
		// Keyboard/NonNativeKeyboard.prefab
		private const string NonNativeKeyboardGuid = "74b589d1efab94a4cb70e4b5c22783f8";
		private static readonly string NonNativeKeyboardPath = AssetDatabase.GUIDToAssetPath(NonNativeKeyboardGuid);

		private NonNativeKeyboard testKeyboard = null;
		private MRTKTMPInputField inputfield = null;

		[SetUp]
		public void Init()
		{
			testKeyboard = InstantiatePrefab(NonNativeKeyboardPath).GetComponent<NonNativeKeyboard>();
			testKeyboard.Open();
			inputfield = testKeyboard.gameObject.GetComponentInChildren<MRTKTMPInputField>();
		}

		[TearDown]
		public void Teardown()
		{
			Object.Destroy(testKeyboard);
			// Wait for a frame to give Unity a change to actually destroy the object
		}

		[UnityTest]
		public IEnumerator TestNonNativeKeyboardInstantiate()
		{
			Assert.IsNotNull(testKeyboard, "NonNativeKeyboard component exists on prefab");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeValueKey()
		{
			NonNativeValueKey qkey = SetUpValueKey("q");
			Assert.IsNotNull(qkey, "q value key exists on prefab");

			Button interactable = qkey.gameObject.GetComponent<Button>();
			interactable.onClick.Invoke();
			Assert.AreEqual(inputfield.text, "q", "Pressing key changes InputField text.");

			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeValueKeyShift()
		{
			NonNativeValueKey fkey = SetUpValueKey("f");
			NonNativeFunctionKey shiftkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
			Assert.IsNotNull(fkey, "f value key exists on prefab");
			Assert.IsNotNull(shiftkey, "Shift key exists on prefab");

			TMP_Text textMeshProText = fkey.gameObject.GetComponentInChildren<TMP_Text>();
			Assert.AreEqual(fkey.CurrentValue, "f", "Current value is set correctly");
			Assert.AreEqual(textMeshProText.text, "f", "TMP text is set correctly");
			testKeyboard.ProcessFunctionKeyPress(shiftkey);
			Assert.AreEqual(fkey.CurrentValue, "F", "Current value shifts correctly");
			Assert.AreEqual(textMeshProText.text, "F", "TMP text shifts correctly");
			testKeyboard.ProcessFunctionKeyPress(shiftkey);
			Assert.AreEqual(fkey.CurrentValue, "f", "Current value is unshifts correctly");
			Assert.AreEqual(textMeshProText.text, "f", "TMP text unshifts correctly");

			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeShiftFunctionKey()
		{
			NonNativeValueKey mkey = SetUpValueKey("m");
			NonNativeFunctionKey shiftkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Shift);
			Assert.IsNotNull(mkey, "M value key exists on prefab");
			Assert.IsNotNull(shiftkey, "Shift key exists on prefab");

			Button interactable = mkey.gameObject.GetComponent<Button>();
			interactable.onClick.Invoke();
			Assert.AreEqual(inputfield.text, "m", "Values not shifted to start with.");
			testKeyboard.ProcessFunctionKeyPress(shiftkey);
			interactable.onClick.Invoke();
			Assert.AreEqual(inputfield.text, "mM", "The Shift function key works");
			interactable.onClick.Invoke();
			Assert.AreEqual(inputfield.text, "mMm", "Unshift works correctly");

			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeSpaceFunctionKey()
		{
			NonNativeFunctionKey spacekey = SetUpFunctionKey(NonNativeFunctionKey.Function.Space);
			PressFunctionKey(spacekey);

			Assert.AreEqual(inputfield.text, "a b", "The Space function key works.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeEnterFunctionKey()
		{
			testKeyboard.SubmitOnEnter = false;
			NonNativeFunctionKey enterkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Enter);
			PressFunctionKey(enterkey);

			Assert.AreEqual(inputfield.text, "a\nb", "The Enter function key works.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeTabFunctionKey()
		{
			NonNativeFunctionKey tabkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Tab);
			PressFunctionKey(tabkey);

			Assert.AreEqual(inputfield.text, "a\tb", "The Tab function key works.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeAlphaSymbolFunctionKeys()
		{
			GameObject alphaKeysSection = testKeyboard.alphaKeysSection;
			GameObject symbolKeysSection = testKeyboard.symbolKeysSection;
			NonNativeFunctionKey abckey = SetUpFunctionKey(NonNativeFunctionKey.Function.Alpha);
			NonNativeFunctionKey symbolkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Symbol);;

			Assert.IsTrue(alphaKeysSection.activeSelf, "Alpha keys initially presented.");
			Assert.IsTrue(!symbolKeysSection.activeSelf, "Symbol keys initially hidden.");
			testKeyboard.ProcessFunctionKeyPress(abckey);
			Assert.IsTrue(alphaKeysSection.activeSelf, "Alpha keys stay presented.");
			Assert.IsTrue(!symbolKeysSection.activeSelf, "Symbol keys stay hidden.");
			testKeyboard.ProcessFunctionKeyPress(symbolkey);
			Assert.IsTrue(!alphaKeysSection.activeSelf, "Alpha keys hidden.");
			Assert.IsTrue(symbolKeysSection.activeSelf, "Symbol keys presented.");
			testKeyboard.ProcessFunctionKeyPress(symbolkey);
			Assert.IsTrue(!alphaKeysSection.activeSelf, "Alpha keys stay hidden.");
			Assert.IsTrue(symbolKeysSection.activeSelf, "Symbol keys stay presented.");
			testKeyboard.ProcessFunctionKeyPress(abckey);
			Assert.IsTrue(alphaKeysSection.activeSelf, "Alpha keys presented.");
			Assert.IsTrue(!symbolKeysSection.activeSelf, "Symbol keys hidden.");

			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativePreviousNextFunctionKeys()
		{
			NonNativeValueKey akey = SetUpValueKey("a");
			NonNativeValueKey bkey = SetUpValueKey("b");
			NonNativeValueKey ckey = SetUpValueKey("c");
			NonNativeFunctionKey prevkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Previous);
			NonNativeFunctionKey nextkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Next);

			Button Ainteractable = akey.gameObject.GetComponent<Button>();
			Button Binteractable = bkey.gameObject.GetComponent<Button>();
			Button Cinteractable = ckey.gameObject.GetComponent<Button>();

			Ainteractable.onClick.Invoke();
			Cinteractable.onClick.Invoke();
			testKeyboard.ProcessFunctionKeyPress(prevkey);
			Binteractable.onClick.Invoke();
			Assert.AreEqual(inputfield.text, "abc", "The Previous function key works.");

			Ainteractable.onClick.Invoke();
			testKeyboard.ProcessFunctionKeyPress(nextkey);
			Ainteractable.onClick.Invoke();
			Assert.AreEqual(inputfield.text, "abaca", "The Next function key works");

			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeCloseFunctionKey()
		{
			NonNativeFunctionKey closekey = SetUpFunctionKey(NonNativeFunctionKey.Function.Close);
			testKeyboard.ProcessFunctionKeyPress(closekey);

			Assert.AreEqual(inputfield.text, "", "The input field is cleared.");
			Assert.IsTrue(!testKeyboard.gameObject.activeInHierarchy, "The keyboard is inactive.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeBackspaceFunctionKey()
		{
			NonNativeFunctionKey backkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Backspace);
			PressFunctionKey(backkey);

			Assert.AreEqual(inputfield.text, "b", "The Tab function key works.");
			testKeyboard.Open();
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeKeyboardSubmitOnEnter()
		{
			testKeyboard.SubmitOnEnter = true;
			NonNativeFunctionKey enterkey = SetUpFunctionKey(NonNativeFunctionKey.Function.Enter);
			testKeyboard.ProcessFunctionKeyPress(enterkey);

			Assert.AreEqual(inputfield.text, "", "The input field is cleared.");
			Assert.IsTrue(!testKeyboard.gameObject.activeInHierarchy, "The keyboard is inactive.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeKeyboardCloseOnInactivity()
		{
			testKeyboard.CloseOnInactivity = true;
			testKeyboard.CloseOnInactivityTime = .01f;

			NonNativeValueKey akey = SetUpValueKey("a");
			Button Ainteractable = akey.gameObject.GetComponent<Button>();
			Ainteractable.onClick.Invoke();


			yield return new WaitForSeconds(0.05f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.AreEqual(inputfield.text, "", "The input field is cleared.");
			Assert.IsTrue(!testKeyboard.gameObject.activeInHierarchy, "The keyboard is inactive.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeValueKeyPressKeyWithHand()
		{
			NonNativeValueKey nkey = SetUpValueKey("n");
			nkey.transform.position = new Vector3(0f, 1.6f, .69f);
			yield return RuntimeTestUtilities.WaitForUpdates();

			TestHand hand = new TestHand(Handedness.Right);
			Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.05f, 0.3f)); // orient hand so far interaction ray will hit button
			yield return hand.Show(initialHandPosition);
			yield return RuntimeTestUtilities.WaitForUpdates();
			yield return hand.SetHandshape(HandshapeId.Pinch);
			yield return RuntimeTestUtilities.WaitForUpdates();
			yield return hand.SetHandshape(HandshapeId.Open);
			yield return RuntimeTestUtilities.WaitForUpdates();

			yield return RuntimeTestUtilities.WaitForUpdates();

			Assert.AreEqual(inputfield.text, "n", "Pressing key changes InputField text.");
			yield return null;
		}

		private NonNativeValueKey SetUpValueKey(string value)
		{
			NonNativeValueKey[] keys = testKeyboard.gameObject.GetComponentsInChildren<NonNativeValueKey>(true);
			foreach (NonNativeValueKey key in keys)
			{
				if (key.CurrentValue?.Equals(value) == true) return key;
			}
			return null;
		}

		private NonNativeFunctionKey SetUpFunctionKey(NonNativeFunctionKey.Function function)
		{
			NonNativeFunctionKey[] keys = testKeyboard.gameObject.GetComponentsInChildren<NonNativeFunctionKey>(true);
			foreach (NonNativeFunctionKey key in keys)
			{
				if (key.KeyFunction == function) return key;
			}
			return null;
		}

		private void PressFunctionKey(NonNativeFunctionKey function)
		{
			NonNativeValueKey akey = SetUpValueKey("a");
			NonNativeValueKey bkey = SetUpValueKey("b");

			Button Ainteractable = akey.gameObject.GetComponent<Button>();
			Button Binteractable = bkey.gameObject.GetComponent<Button>();
			Ainteractable.onClick.Invoke();
			testKeyboard.ProcessFunctionKeyPress(function);
			Binteractable.onClick.Invoke();
		}

		private GameObject InstantiatePrefab(string prefabPath)
		{
			Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
			GameObject testGO = Object.Instantiate(pressableButtonPrefab) as GameObject;

			return testGO;
		}
	}
}
