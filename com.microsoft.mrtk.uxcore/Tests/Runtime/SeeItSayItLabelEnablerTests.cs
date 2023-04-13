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
            GameObject testButton = SetUpButton(true);
            yield return null;

#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            Transform label = testButton.transform.GetChild(0);
            Transform sublabel = label.transform.GetChild(0);
            Assert.IsTrue(label.gameObject.activeSelf, "Label is enabled");
            Assert.IsTrue(!sublabel.gameObject.activeSelf, "Child objects are disabled");
            TMP_Text text = label.gameObject.GetComponentInChildren<TMP_Text>(true);
            Assert.AreEqual(text.text, "Say 'test'", "Label text was set to voice command keyword.");
#else
            Assert.AreEqual(testButton.transform.childCount, 0, "Did not generate label because input or speech package is missing.");
#endif

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestVoiceCommandsUnavailable()
        {
            GameObject testButton = SetUpButton(false);
            yield return null;

            Assert.AreEqual(testButton.transform.childCount, 0, "Did not generate label because voice commands unavailable.");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        private GameObject SetUpButton(bool allowSelectByVoice)
        {
            // Create a PressableButton to add SeeItSayItLabelCreator script to
            GameObject testButton = new GameObject("Button");
            PressableButton pressablebutton = testButton.AddComponent<PressableButton>();
            pressablebutton.AllowSelectByVoice = allowSelectByVoice;
            pressablebutton.SpeechRecognitionKeyword = "test";

            // Create a label GameObject to generate 
            GameObject label = new GameObject("Label");
            label.transform.SetParent(testButton.transform, false);
            GameObject subLabel = new GameObject("SubLabel");
            subLabel.transform.SetParent(label.transform, false);
            subLabel.AddComponent<TextMeshProUGUI>();

            // Set up SeeItSayItCreatorLabel script
            SeeItSayItLabelEnabler enabler = testButton.AddComponent<SeeItSayItLabelEnabler>();
            enabler.SeeItSayItLabel = label;

            return testButton;
        }
    }
}
