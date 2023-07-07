// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the See-It Say-It label generator
    /// </summary>
    public class SeeItSayItLabelEnablerTests : BaseRuntimeInputTests
    {
        [UnityTest]
        public IEnumerator TestEnableAndSetLabel()
        {
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            SpeechInteractor interactor = Object.FindAnyObjectByType<SpeechInteractor>(FindObjectsInactive.Include);
            interactor.gameObject.SetActive(true);

            GameObject testButton = SetUpButton(true, Control.None);
            yield return null;
            if (Application.isBatchMode)
            {
                LogAssert.Expect(LogType.Exception, new Regex("Speech recognition is not supported on this machine"));
            }

            Transform label = testButton.transform.GetChild(0);

            Transform sublabel = label.transform.GetChild(0);
            Assert.IsTrue(label.gameObject.activeSelf, "Label is enabled");
            Assert.IsTrue(!sublabel.gameObject.activeSelf, "Child objects are disabled");
            TMP_Text text = label.gameObject.GetComponentInChildren<TMP_Text>(true);
            Assert.AreEqual(text.text, "Say 'test'", "Label text was set to voice command keyword.");
#else
            Assert.IsTrue(!label.gameObject.activeSelf, "Did not enable label because voice commands unavailable.");
#endif

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestVoiceCommandsUnavailable()
        {
            GameObject testButton = SetUpButton(false, Control.None);
            yield return null;

            Transform label = testButton.transform.GetChild(0);
            Assert.IsTrue(!label.gameObject.activeSelf, "Did not enable label because voice commands unavailable.");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPositionCanvasLabel()
        {
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            SpeechInteractor interactor = Object.FindAnyObjectByType<SpeechInteractor>(FindObjectsInactive.Include);
            interactor.gameObject.SetActive(true);

            GameObject testButton = SetUpButton(true, Control.Canvas);
            yield return null;
            if (Application.isBatchMode)
            {
                LogAssert.Expect(LogType.Exception, new Regex("Speech recognition is not supported on this machine"));
            }

            Transform label = testButton.transform.GetChild(0);
            RectTransform sublabel = label.transform.GetChild(0) as RectTransform;
            Assert.AreEqual(sublabel.anchoredPosition3D, new Vector3(10, -30, -10), "Label is positioned correctly");
#else
            Assert.IsTrue(!label.gameObject.activeSelf, "Did not enable label because voice commands unavailable.");
#endif

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPositionNonCanvasLabel()
        {
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            SpeechInteractor interactor = Object.FindAnyObjectByType<SpeechInteractor>(FindObjectsInactive.Include);
            interactor.gameObject.SetActive(true);

            GameObject testButton = SetUpButton(true, Control.NonCanvas);
            yield return null;
            if (Application.isBatchMode)
            {
                LogAssert.Expect(LogType.Exception, new Regex("Speech recognition is not supported on this machine"));
            }

            Transform label = testButton.transform.GetChild(0);
            Assert.AreEqual(label.transform.localPosition, new Vector3(10f, -.504f, -.004f), "Label is positioned correctly");
#else
            Assert.IsTrue(!label.gameObject.activeSelf, "Did not enable label because voice commands unavailable.");
#endif

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        private GameObject SetUpButton(bool allowSelectByVoice, Control control)
        {
            // Create a PressableButton to add SeeItSayItLabelCreator script to
            GameObject testButton = new GameObject("Button");
            PressableButton pressablebutton = testButton.AddComponent<PressableButton>();
            pressablebutton.AllowSelectByVoice = allowSelectByVoice;
            pressablebutton.SpeechRecognitionKeyword = "test";

            // Create a label GameObject to generate 
            GameObject label = new GameObject("Label");
            label.transform.SetParent(testButton.transform, false);
            label.SetActive(false);
            GameObject subLabel = new GameObject("SubLabel");
            subLabel.transform.SetParent(label.transform, false);
            subLabel.AddComponent<TextMeshProUGUI>();
            subLabel.SetActive(true);

            // Set positions as necessary to test Canvas and NonCanvas positioning
            Transform positionControl = null;
            switch (control)
            {
                case Control.Canvas:
                    RectTransform buttonRectTransform = testButton.AddComponent<RectTransform>();
                    buttonRectTransform.offsetMin = new Vector2(-30, -30);
                    buttonRectTransform.offsetMax = new Vector2(30, 30);
                    RectTransform labelRectTransform = label.AddComponent<RectTransform>();
                    labelRectTransform.offsetMin = new Vector2(-10, -10);
                    labelRectTransform.offsetMax = new Vector2(10, 10);
                    positionControl = buttonRectTransform;
                    break;
                case Control.NonCanvas:
                    testButton.transform.localPosition = new Vector3(10f, 10f, 0f);
                    positionControl = testButton.transform;
                    break;
                default:
                    break;
            }

            // Set up SeeItSayItCreatorLabel script
            SeeItSayItLabelEnabler enabler = testButton.AddComponent<SeeItSayItLabelEnabler>();
            enabler.SeeItSayItLabel = label;
            enabler.PositionControl = positionControl;

            return testButton;
        }

        private enum Control
        {
            None,
            Canvas,
            NonCanvas
        }
    }
}
