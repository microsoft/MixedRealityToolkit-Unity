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

        //SeeItSayItLabel/SeeItSayItLabel-NonCanvas.prefab
        private const string SeeItSayItLabelGuid = "d9e84b5a8037fd946aa503a059fee93f";
        private static readonly string SeeItSayItLabelPath = AssetDatabase.GUIDToAssetPath(SeeItSayItLabelGuid);


        [UnityTest]
        public IEnumerator TestGenerateWhenVoiceCommandsAvailable()
        {
            GameObject testButton = SetUpButton(true);
            yield return null;
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
            Assert.AreEqual(testButton.transform.childCount, 1, "Generated SeeItSayIt label.");
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
            GameObject testButton = SetUpButton(false);
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

        private GameObject SetUpButton(bool allowSelectByVoice)
        {
            GameObject testButton = new GameObject("Button");
            PressableButton pressablebutton = testButton.AddComponent<PressableButton>();
            pressablebutton.AllowSelectByVoice = allowSelectByVoice;
            SeeItSayItGenerator generator = testButton.AddComponent<SeeItSayItGenerator>();
            generator.PositionControl = testButton.transform;
            generator.SeeItSayItPrefab = InstantiatePrefab(SeeItSayItLabelPath);
            generator.IsCanvas = true;
            return testButton;
        }
    }
}
