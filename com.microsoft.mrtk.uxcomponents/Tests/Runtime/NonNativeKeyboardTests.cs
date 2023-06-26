// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using Microsoft.MixedReality.Toolkit.UX.Experimental;
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
		private KeyboardPreview keyboardPreview = null;

		[SetUp]
		public void Init()
		{
			testKeyboard = InstantiatePrefab(NonNativeKeyboardPath).GetComponent<NonNativeKeyboard>();
            testKeyboard.transform.position = InputTestUtilities.InFrontOfUser(distanceFromHead: 2.0f);
            keyboardPreview = testKeyboard.Preview;
            testKeyboard.Open();
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
            yield return TypeString("q");
			Assert.AreEqual("q", keyboardPreview.Text, "q", "Pressing key did not change text.");
		}

		[UnityTest]
		public IEnumerator TestNonnativeValueKeyShift()
		{
			NonNativeValueKey fKey = FindValueKey('f');
			Assert.IsNotNull(fKey, "Unable to find the 'f' key.");

			TMP_Text textMeshProText = fKey.gameObject.GetComponentInChildren<TMP_Text>();
			Assert.AreEqual(fKey.CurrentValue, "f", "Current value is set correctly");
			Assert.AreEqual(textMeshProText.text, "f", "TMP text is set correctly");

            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Shift);
			Assert.AreEqual(fKey.CurrentValue, "F", "Current value shifts correctly");
			Assert.AreEqual(textMeshProText.text, "F", "TMP text shifts correctly");

            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Shift);
            Assert.AreEqual(fKey.CurrentValue, "f", "Current value is un-shifts correctly");
			Assert.AreEqual(textMeshProText.text, "f", "TMP text un-shifts correctly");
		}

		[UnityTest]
		public IEnumerator TestNonnativeShiftFunctionKey()
		{
            yield return TypeString("m");
			Assert.AreEqual("m", keyboardPreview.Text, "Value should be lower case to start with.");
            yield return TypeString("M");
			Assert.AreEqual("mM", keyboardPreview.Text, "Value should be upper case immediately after clicking shift key.");
            yield return TypeString("m");
            Assert.AreEqual("mMm", keyboardPreview.Text, "Value should be lower case after shift key was consumed.");
		}

		[UnityTest]
		public IEnumerator TestNonnativeSpaceFunctionKey()
        {
            yield return TypeString("a");
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Space);
            yield return TypeString("b");

			Assert.AreEqual(keyboardPreview.Text, "a b", "The Space function key works.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeEnterFunctionKey()
		{
			testKeyboard.SubmitOnEnter = false;

            yield return TypeString("a");
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Enter);
            yield return TypeString("b");

			Assert.AreEqual(keyboardPreview.Text, "a\nb", "The Enter function key works.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeTabFunctionKey()
        {
            yield return TypeString("a");
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Symbol);
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Tab);
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Alpha);
            yield return TypeString("b");

            Assert.AreEqual(keyboardPreview.Text, "a\tb", "The Tab function key works.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeAlphaSymbolFunctionKeys()
		{
			GameObject alphaKeysSection = testKeyboard.AlphaKeysSection;
            GameObject defaultBottomSection = testKeyboard.DefaultBottomKeysSection;
            GameObject symbolKeysSection = testKeyboard.SymbolKeysSection;
            GameObject emailSection = testKeyboard.EmailBottomKeysSection;
            GameObject urlBottomSection = testKeyboard.UrlBottomKeysSection;

            Assert.IsTrue(alphaKeysSection.activeInHierarchy, "Alpha keys should be initially shown.");
            Assert.IsTrue(defaultBottomSection.activeInHierarchy, "Default bottom keys should be  initially shown.");
            Assert.IsFalse(symbolKeysSection.activeInHierarchy, "Symbol keys should be initially hidden.");
            Assert.IsFalse(emailSection.activeInHierarchy, "Email keys should be initially hidden.");
            Assert.IsFalse(urlBottomSection.activeInHierarchy, "Url bottom keys should be initially hidden.");

            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Symbol);
            Assert.IsFalse(alphaKeysSection.activeInHierarchy, "Alpha keys should be hidden.");
            Assert.IsTrue(symbolKeysSection.activeInHierarchy, "Symbol keys should be shown.");
            Assert.IsFalse(emailSection.activeInHierarchy, "Email keys should be hidden.");

            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Alpha);
            Assert.IsTrue(alphaKeysSection.activeInHierarchy, "Alpha keys should be shown.");
            Assert.IsFalse(symbolKeysSection.activeInHierarchy, "Symbol keys should be hidden.");
            Assert.IsFalse(emailSection.activeInHierarchy, "Email keys should be hidden.");

            yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativePreviousNextFunctionKeys()
		{
            yield return TypeString("ac");
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Previous);
            yield return TypeString("b");

			Assert.AreEqual("abc", keyboardPreview.Text, "The Previous function failed to work.");


            yield return TypeString("a");
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Next);
            yield return TypeString("a");

			Assert.AreEqual("abaca", keyboardPreview.Text, "The Next function failed to work");
		}

		[UnityTest]
		public IEnumerator TestNonnativeCloseFunctionKey()
		{
			NonNativeFunctionKey closeKey = FindFunctionKey(NonNativeFunctionKey.Function.Close);
			testKeyboard.ProcessFunctionKeyPress(closeKey);

			Assert.AreEqual(keyboardPreview.Text, "", "The input field is cleared.");
			Assert.IsTrue(!testKeyboard.gameObject.activeInHierarchy, "The keyboard is inactive.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeBackspaceFunctionKey()
        {
            yield return TypeString("a");
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Backspace);
            yield return TypeString("b");

			Assert.AreEqual(keyboardPreview.Text, "b", "The Tab function key works.");
			testKeyboard.Open();
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeKeyboardSubmitOnEnter()
		{
			testKeyboard.SubmitOnEnter = true;
			NonNativeFunctionKey enterKey = FindFunctionKey(NonNativeFunctionKey.Function.Enter);
			testKeyboard.ProcessFunctionKeyPress(enterKey);

			Assert.AreEqual(keyboardPreview.Text, "", "The input field is cleared.");
			Assert.IsTrue(!testKeyboard.gameObject.activeInHierarchy, "The keyboard is inactive.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestNonnativeKeyboardCloseOnInactivity()
		{
			testKeyboard.CloseOnInactivity = true;
			testKeyboard.CloseOnInactivityTime = .01f;

            yield return TypeString("a");
            yield return new WaitForSeconds(0.05f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.AreEqual(keyboardPreview.Text, "", "The input field is cleared.");
			Assert.IsTrue(!testKeyboard.gameObject.activeInHierarchy, "The keyboard is inactive.");
		}

		[UnityTest]
		public IEnumerator TestNonnativeValueKeyPressKeyWithHand()
		{
			TestHand hand = new TestHand(Handedness.Right);
			Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.05f, 0.3f)); // orient hand so far interaction ray will hit button
			yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();

            string text = "hello";
            yield return TypeStringWithHand(hand, text);
            Assert.AreEqual(text, keyboardPreview.Text, "Hand entered string did not work correctly.");
		}

        [UnityTest]
        public IEnumerator TestKeyboardPreviewWithLeftToRightPreviewText()
        {
            yield return SetupLeftToRightPreviewText();
            yield return TestKeyboardPreview();
        }

        [UnityTest]
        public IEnumerator TestKeyboardPreviewWithRightToLeftPreviewText()
        {
            yield return SetupRightToLeftPreviewText();
            yield return TestKeyboardPreview();
        }

        public IEnumerator TestKeyboardPreview()
        {
            var startLabelPosition = keyboardPreview.PreviewText.rectTransform.localPosition;
            var startCursorPosition = keyboardPreview.PreviewCaret.localPosition;

            var testString1 = "This is a test.";
            yield return TypeString(testString1);
            Assert.AreEqual(testString1, keyboardPreview.PreviewText.text, "Label text did not update.");

            var newLabelPosition1  = keyboardPreview.PreviewText.rectTransform.localPosition;
            Assert.IsTrue(newLabelPosition1 == startLabelPosition, "Label transform should not have moved, as nothing should have scrolled off.");

            var newCursorPosition1 = keyboardPreview.PreviewCaret.localPosition;
            TestCursorMovedForward(startCursorPosition, newCursorPosition1, "Cursor transform did not move forward while typing.");
            Assert.IsTrue(newCursorPosition1.y == startCursorPosition.y, "Cursor transform should not move up or down while typing.");

            var testString2 = " This is some more text which should cause the label to scroll over.";
            yield return TypeString(testString2);
            Assert.AreEqual(testString1 + testString2, keyboardPreview.PreviewText.text, "Label text did not update to include additional string.");

            var newLabelPosition2 = keyboardPreview.PreviewText.rectTransform.localPosition;
            TestLabelScrolledForward(startLabelPosition, newLabelPosition2, "Label transform did not move in the correct direction to expose the focused text.");
            Assert.IsTrue(newLabelPosition2.y == startLabelPosition.y, "Label transform should not have moved up or down yet.");

            var newCursorPosition2 = keyboardPreview.PreviewCaret.localPosition;
            TestCursorMovedForward(newCursorPosition1, newCursorPosition2, "Cursor transform did not move forward while typing.");
            Assert.IsTrue(newCursorPosition2.y == startCursorPosition.y, "Cursor transform should not move up or down while typing.");

            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Enter);
            Assert.AreEqual(testString1 + testString2 + '\n', keyboardPreview.PreviewText.text, "Label text did not update to include new line character.");

            var newLabelPosition3 = keyboardPreview.PreviewText.rectTransform.localPosition;
            Assert.IsTrue(newLabelPosition3.x == startLabelPosition.x, "Label transform should have reset in the horizontal direction with a new line character.");
            Assert.IsTrue(newLabelPosition3.y > startLabelPosition.y, "Label transform did not move up.");

            var newCursorPosition3 = keyboardPreview.PreviewCaret.localPosition;
            TestCursorHorizontalPositionsEqual(startCursorPosition, newCursorPosition3, "Cursor transform did not move back to the front with a new line character");
            Assert.IsTrue(newCursorPosition3.y == -newLabelPosition3.y, "Cursor transform should not move up or down while typing, when considering label movement. Note, the caret's parent should be the text label.");

            var testString3 = "Some more text.";
            yield return TypeString(testString3);
            Assert.AreEqual(testString1 + testString2 + '\n' + testString3, keyboardPreview.PreviewText.text, "Label text did not update to include third new string.");

            var newLabelPosition4 = keyboardPreview.PreviewText.rectTransform.localPosition;
            Assert.IsTrue(newLabelPosition4.x == newLabelPosition3.x, "Label transform should not have moved on the second line, as text hasn't scrolled off yet.");
            Assert.IsTrue(newLabelPosition4.y == newLabelPosition3.y, "Label transform should not have moved up or down yet.");

            var newCursorPosition4 = keyboardPreview.PreviewCaret.localPosition;
            TestCursorMovedForward(newCursorPosition3, newCursorPosition4, "Cursor transform did not move to the end of the string");
            Assert.IsTrue(newCursorPosition4.y == newCursorPosition3.y, "Cursor transform should not move up or down yet.");

            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Previous);
            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Previous);

            var newCursorPosition5 = keyboardPreview.PreviewCaret.localPosition;
            TestCursorMovedBackward(newCursorPosition4, newCursorPosition5, "Cursor transform did not move backward.");
            Assert.IsTrue(newCursorPosition5.y == newCursorPosition4.y, "Cursor transform should not move up or down yet.");

            yield return TypeFunctionKey(NonNativeFunctionKey.Function.Next);
            var newCursorPosition6 = keyboardPreview.PreviewCaret.localPosition;
            TestCursorMovedForward(newCursorPosition5, newCursorPosition6, "Cursor transform did not move forward.");
            Assert.IsTrue(newCursorPosition6.y == newCursorPosition5.y, "Cursor transform should not move up or down yet.");
            Assert.AreEqual(testString1 + testString2 + '\n' + testString3, keyboardPreview.PreviewText.text, "Label text should not have changed with cursor/caret movement.");

            var testString4 = " abc";
            var testString3withChanges = $"Some more text{testString4}.";
            yield return TypeString(testString4);
            Assert.AreEqual(testString1 + testString2 + '\n' + testString3withChanges, keyboardPreview.PreviewText.text, "Label text did not update correctly with mid-string additions.");
        }

        private void TestCursorMovedForward(Vector3 start, Vector3 end, string message)
        {
            if (IsRightToLeftPreviewText())
            {
                Assert.IsTrue(end.x < start.x, message);
            }
            else
            {
                Assert.IsTrue(end.x > start.x, message);
            }
        }

        private void TestCursorMovedBackward(Vector3 start, Vector3 end, string message)
        {
            if (IsRightToLeftPreviewText())
            {
                Assert.IsTrue(end.x > start.x, message);
            }
            else
            {
                Assert.IsTrue(end.x < start.x, message);
            }
        }

        private void TestCursorHorizontalPositionsEqual(Vector3 start, Vector3 end, string message)
        {
            // due to how cursor positions are calculated, the cursor may not end up at the exact position if move forward and then back.
            float acceptableDelta = 1.5f;
            float delta = Mathf.Abs(start.x - end.x);
            Assert.IsTrue(delta <= acceptableDelta, message);
        }

        private void TestLabelScrolledForward(Vector3 start, Vector3 end, string message)
        {
            if (IsRightToLeftPreviewText())
            {
                Assert.IsTrue(end.x > start.x, message);
            }
            else
            {
                Assert.IsTrue(end.x < start.x, message);
            }
        }

        private bool IsRightToLeftPreviewText()
        {
            return keyboardPreview.PreviewText.isRightToLeftText;
        }

        private IEnumerator SetupLeftToRightPreviewText()
        {
            keyboardPreview.PreviewText.isRightToLeftText = false;
            keyboardPreview.PreviewText.alignment = TextAlignmentOptions.Left;
            keyboardPreview.InvalidateLayout();
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        private IEnumerator SetupRightToLeftPreviewText()
        {
            keyboardPreview.PreviewText.isRightToLeftText = true;
            keyboardPreview.PreviewText.alignment = TextAlignmentOptions.Right;
            keyboardPreview.InvalidateLayout();
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        private IEnumerator TypeStringWithHand(TestHand hand, string text)
        {
            // transform's position is in upper left corner. Adjust slightly so hand ray points at the correct key
            Vector3 adjustments = new Vector3(10f, -10f, 0f);

            foreach (char c in text)
            {
                NonNativeValueKey key = FindValueKey(c);
                yield return hand.AimAt(key.transform.parent.TransformPoint(key.transform.localPosition + adjustments));
                yield return RuntimeTestUtilities.WaitForUpdates();
                yield return hand.SetHandshape(HandshapeId.Pinch);
                yield return RuntimeTestUtilities.WaitForUpdates();
                yield return hand.SetHandshape(HandshapeId.Open);
                yield return RuntimeTestUtilities.WaitForUpdates();
            }
        }

        private IEnumerator TypeString(string text)
        {
            foreach (char c in text)
            {
                if (c == ' ')
                {
                    yield return TypeFunctionKey(NonNativeFunctionKey.Function.Space);
                }
                else
                {
                    if (char.IsUpper(c))
                    {
                        yield return TypeFunctionKey(NonNativeFunctionKey.Function.Shift);
                    }

                    NonNativeKey key = FindValueKey(c);
                    Assert.IsNotNull(key, $"Could not find alpha key '{c}'");
                    key.GetComponent<Button>().onClick.Invoke();
                    yield return null;
                }
            }

            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        private IEnumerator TypeFunctionKey(NonNativeFunctionKey.Function value)
        {
            var key = FindFunctionKey(value);
            Assert.IsNotNull(key, $"Could not find function key '{value.ToString()}'");
            key.GetComponent<Button>().onClick.Invoke();
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

		private NonNativeValueKey FindValueKey(char value)
		{
			NonNativeValueKey[] keys = testKeyboard.gameObject.GetComponentsInChildren<NonNativeValueKey>(true);
			foreach (NonNativeValueKey key in keys)
			{
				if (key.isActiveAndEnabled &&
                    !string.IsNullOrEmpty(key.CurrentValue) &&
                    char.ToUpperInvariant(key.CurrentValue[0]) == char.ToUpperInvariant(value)) return key;
			}
			return null;
		}

		private NonNativeFunctionKey FindFunctionKey(NonNativeFunctionKey.Function function)
		{
			NonNativeFunctionKey[] keys = testKeyboard.gameObject.GetComponentsInChildren<NonNativeFunctionKey>(true);
			foreach (NonNativeFunctionKey key in keys)
			{
				if (key.isActiveAndEnabled && key.KeyFunction == function) return key;
			}
			return null;
		}

		private GameObject InstantiatePrefab(string prefabPath)
		{
			Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
			GameObject testGO = Object.Instantiate(pressableButtonPrefab) as GameObject;

			return testGO;
		}
	}
}
