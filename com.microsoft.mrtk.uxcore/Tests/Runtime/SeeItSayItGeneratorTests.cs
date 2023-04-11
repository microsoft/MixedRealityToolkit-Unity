// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the See-It Say-It label generator
    /// </summary>
    public class SeeItSayItGeneratorTests : BaseRuntimeInputTests
    {
        [UnityTest]
        public IEnumerator TestNonCanvasLabel()
        {
            GameObject testButton = SetUpButton(true, false);
            yield return null;

#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            Assert.AreEqual(testButton.transform.childCount, 1, "Generated SeeItSayIt label.");
            Transform label = testButton.transform.GetChild(0);
            Assert.IsTrue(label.transform.localPosition.Equals(new Vector3(10f, -.504f, -.01f)), "Label is positioned correctly");
            Transform sublabel = label.transform.GetChild(0);
            Assert.IsTrue(!sublabel.gameObject.activeSelf, "Child objects are disabled");
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
            Assert.IsTrue(sublabel.anchoredPosition3D.Equals(new Vector3(10, -30, -10)), "Label is positioned correctly");
            Assert.IsTrue(!sublabel.gameObject.activeSelf, "Child objects are disabled.");
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
            //Create a pressable button to add SeeItSayItGenerator script to
            GameObject testButton = new GameObject("Button");
            PressableButton pressablebutton = testButton.AddComponent<PressableButton>();
            pressablebutton.AllowSelectByVoice = allowSelectByVoice;
            testButton.transform.localPosition = new Vector3(10f, 10f, 0f);

            //Create a label GameObject to generate 
            GameObject label = new GameObject("Label");
            GameObject subLabel = new GameObject("SubLabel");
            subLabel.transform.SetParent(label.transform, false);

            //For Canvas, set RectTransform
            if (isCanvas)
            {
                RectTransform buttonRectTransform = testButton.AddComponent<RectTransform>();
                buttonRectTransform.offsetMin = new Vector2(-30, -30);
                buttonRectTransform.offsetMax = new Vector2(30, 30);
                RectTransform labelRectTransform = label.AddComponent<RectTransform>();
                labelRectTransform.offsetMin = new Vector2(-10, -10);
                labelRectTransform.offsetMax = new Vector2(10, 10);
                subLabel.AddComponent<RectTransform>();
            }

            //Set up SeeItSayItGenerator script
            SeeItSayItGenerator generator = testButton.AddComponent<SeeItSayItGenerator>();
            generator.PositionControl = testButton.transform;
            generator.IsCanvas = isCanvas;
            generator.SeeItSayItPrefab = label;

            return testButton;
        }
    }
}
