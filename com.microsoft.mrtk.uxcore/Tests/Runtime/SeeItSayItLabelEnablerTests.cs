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
        public IEnumerator TestNonCanvasLabel()
        {
            GameObject testButton = SetUpButton(true, false);
            yield return null;

#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            Assert.AreEqual(testButton.transform.childCount, 1, "Generated SeeItSayIt label.");
            Transform label = testButton.transform.GetChild(0);
            Assert.AreEqual(label.transform.localPosition, new Vector3(10f, -.504f, -.004f), "Label is positioned correctly");
            Transform sublabel = label.transform.GetChild(0);
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
        public IEnumerator TestCanvasLabel()
        {
            GameObject testButton = SetUpButton(true, true);
            yield return null;

#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            Assert.AreEqual(testButton.transform.childCount, 1, "Generated SeeItSayIt label.");
            RectTransform sublabel = testButton.transform.GetChild(0).GetChild(0) as RectTransform;
            Assert.AreEqual(sublabel.anchoredPosition3D, new Vector3(10, -30, -10), "Label is positioned correctly");
            Assert.IsTrue(!sublabel.gameObject.activeSelf, "Child objects are disabled.");
            TMP_Text text = sublabel.gameObject.GetComponentInChildren<TMP_Text>(true);
            Assert.AreEqual(text.text, "Say 'test'", "Label text was set to voice command keyword.");
#else
            Assert.AreEqual(testButton.transform.childCount, 0, "Did not generate label because input or speech package is missing.");
#endif

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestGenerateWhenVoiceCommandsUnavailable()
        {
            GameObject testButton = SetUpButton(false, false);
            yield return null;

            Assert.AreEqual(testButton.transform.childCount, 0, "Did not generate label because voice commands unavailable.");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        private GameObject InstantiatePrefab(string prefabPath)
        {
            Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            GameObject testGO = Object.Instantiate(pressableButtonPrefab) as GameObject;

            return testGO;
        }

        private GameObject SetUpButton(bool allowSelectByVoice, bool isCanvas)
        {
            // Create a PressableButton to add SeeItSayItLabelCreator script to
            GameObject testButton = new GameObject("Button");
            PressableButton pressablebutton = testButton.AddComponent<PressableButton>();
            pressablebutton.AllowSelectByVoice = allowSelectByVoice;
            pressablebutton.SpeechRecognitionKeyword = "test";
            testButton.transform.localPosition = new Vector3(10f, 10f, 0f);

            // Create a label GameObject to generate 
            GameObject label = new GameObject("Label");
            GameObject subLabel = new GameObject("SubLabel");
            subLabel.transform.SetParent(label.transform, false);
            subLabel.AddComponent<TextMeshProUGUI>();

            // Set up SeeItSayItCreatorLabel script
            SeeItSayItLabelEnabler generator = testButton.AddComponent<SeeItSayItLabelEnabler>();
            generator.SeeItSayItLabel = label;

            return testButton;
        }
    }
}
