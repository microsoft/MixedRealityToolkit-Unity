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
    /// Tests for the Non-Canvas See-It Say-It label.
    /// </summary>
    public class SeeItSayItNonCanvasTests : BaseRuntimeInputTests
    {
        // SeeItSayItLabel/SeeItSayItLabel-NonCanvas.prefab
        private const string SeeItSayItLabelGuid = "6f685b60890c0884289dcc35603d03c2";
        private static readonly string SeeItSayItLabelPath = AssetDatabase.GUIDToAssetPath(SeeItSayItLabelGuid);

        // Button/128x32/PressableButton_128x32mm_TextOnly.prefab
        private const string PressableButtonGuid = "72dfeb9ecf5ad884b87eff8bc5b49276";
        private static readonly string PressableButtonPath = AssetDatabase.GUIDToAssetPath(PressableButtonGuid);

        [UnityTest]
        public IEnumerator TestSeeItSayItLabelInstantiate()
        {
            GameObject testLabel = InstantiatePrefab(SeeItSayItLabelPath);
            yield return null;

            StateVisualizer labelStateVisualizerComponent = testLabel.GetComponent<StateVisualizer>();
            Assert.IsNotNull(labelStateVisualizerComponent, "State visualizer component exists on label");
            Assert.AreEqual(testLabel.transform.childCount, 2, "Label has two child GameObjects");

            Object.Destroy(testLabel);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPressableButtonSeeItSayItLabelEnabler()
        {
            GameObject testButton = InstantiatePrefab(PressableButtonPath);
            yield return null;

            SeeItSayItLabelEnabler labelEnablerComponent = testButton.GetComponent<SeeItSayItLabelEnabler>();
            Assert.IsNotNull(labelEnablerComponent, "SeeItSayIt generator component exists on pressable button prefab");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestLabelEnabledOnHover()
        {
            GameObject testButton = InstantiatePrefab(PressableButtonPath);
            yield return null;

            // Ensure that there is a label 
            GameObject generatedLabel = GameObject.Find("SeeItSayItLabel-NonCanvas");
            if (generatedLabel == null)
            {
                generatedLabel = InstantiateChildPrefab(SeeItSayItLabelPath, testButton.transform);
            }

            // and label child (the part that is enabled and disabled on hover)
            GameObject labelChild = null;
            if (generatedLabel.transform.childCount >= 1)
            {
                labelChild = generatedLabel.transform.GetChild(0).gameObject;
                labelChild.SetActive(false);
            }

            // No hover initially -- label should be disabled
            Assert.IsTrue(labelChild?.activeInHierarchy == false, "The label is disabled when the button is not hovered.");

            // Move hand to hover the object, wait for the animation to play
            yield return HoverButtonWithHand(testButton.transform.position);
            yield return RuntimeTestUtilities.WaitForFixedUpdates(frameCount: 50);
            Assert.IsTrue(labelChild?.activeInHierarchy == true, "The label is enabled when the button is hovered.");

            // Move hand away from the object
            yield return ReleaseButtonWithHand(testButton.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(labelChild?.activeInHierarchy == false, "The label is disabled when the button is not hovered.");

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

        private GameObject InstantiateChildPrefab(string prefabPath, Transform parent)
        {
            Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            GameObject testGO = Object.Instantiate(pressableButtonPrefab, parent) as GameObject;

            return testGO;
        }

        /// <summary>
        /// Move the hand forward to the button
        /// </summary>
        private IEnumerator HoverButtonWithHand(Vector3 buttonPosition)
        {
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return hand.MoveTo(buttonPosition, 15);
        }

        /// <summary>
        /// Move the hand away from the button
        /// </summary>
        private IEnumerator ReleaseButtonWithHand(Vector3 buttonPosition)
        {
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.MoveTo(buttonPosition + new Vector3(0.0f, 0, -0.05f));
            yield return hand.Hide();
        }
    }
}
