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
    /// Tests for the Canvas See-It Say-It label.
    /// </summary>
    public class SeeItSayItCanvasTests : BaseRuntimeInputTests
    {
        // SeeItSayItLabel/SeeItSayItLabel-Canvas.prefab
        private const string SeeItSayItLabelGuid = "d9e84b5a8037fd946aa503a059fee93f";
        private static readonly string SeeItSayItLabelPath = AssetDatabase.GUIDToAssetPath(SeeItSayItLabelGuid);

        // Button/Prefabs/Empty Button.prefab
        private const string EmptyButtonGuid = "b85e005d231192249b7077b40a4d4e45";
        private static readonly string EmptyButtonPath = AssetDatabase.GUIDToAssetPath(EmptyButtonGuid);

        [UnityTest]
        public IEnumerator TestSeeItSayItLabelInstantiate()
        {
            GameObject testLabel = InstantiatePrefab(SeeItSayItLabelPath);
            yield return null;

            StateVisualizer labelStateVisualizerComponent = testLabel.GetComponent<StateVisualizer>();
            Assert.IsNotNull(labelStateVisualizerComponent, "State visualizer component exists on label");
            Assert.AreEqual(testLabel.transform.childCount, 1, "Label has one child GameObject");

            Object.Destroy(testLabel);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEmptyButtonSeeItSayItLabelEnabler()
        {
            GameObject testButton = InstantiatePrefab(EmptyButtonPath);
            yield return null;

            SeeItSayItLabelEnabler labelEnablerComponent = testButton.GetComponent<SeeItSayItLabelEnabler>();
            Assert.IsNotNull(labelEnablerComponent, "SeeItSayIt enabler component exists on empty button prefab");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestLabelEnabledOnHover()
        {
            GameObject testButton = InstantiatePrefab(EmptyButtonPath);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Ensure that there is a label 
            GameObject generatedLabel = GameObject.Find("SeeItSayItLabel-Canvas");
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

            yield return RuntimeTestUtilities.WaitForUpdates();
            // No hover initially -- label should be disabled
            Assert.IsTrue(labelChild?.activeInHierarchy == false, "The label is disabled when the button is not hovered.");
            // Move hand to hover the object, wait for the animation to play
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.one);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.MoveTo(testButton.transform.position);
            // Label show animation takes 1.0 seconds.  Wait for it to finish.
            yield return new WaitForSecondsRealtime(1.25f);
            Assert.IsTrue(labelChild?.activeInHierarchy == true, $"The label is enabled when the button is hovered.");

            // Move hand away from the object
            yield return hand.MoveTo(testButton.transform.position + new Vector3(0.5f, 0, -0.05f));
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.Hide();
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
    }
}
