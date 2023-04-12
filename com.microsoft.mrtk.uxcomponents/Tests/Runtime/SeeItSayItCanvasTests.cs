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
        //SeeItSayItLabel/SeeItSayItLabel-Canvas.prefab
        private const string SeeItSayItLabelGuid = "d9e84b5a8037fd946aa503a059fee93f";
        private static readonly string SeeItSayItLabelPath = AssetDatabase.GUIDToAssetPath(SeeItSayItLabelGuid);

        //Button/Prefabs/Empty Button.prefab
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
        public IEnumerator TestEmptyButtonSeeItSayItLabel()
        {
            GameObject testButton = InstantiatePrefab(EmptyButtonPath);
            yield return null;

            SeeItSayItLabelCreator labelGeneratorComponent = testButton.GetComponent<SeeItSayItLabelCreator>();
            Assert.IsNotNull(labelGeneratorComponent, "SeeItSayIt generator component exists on empty button prefab");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestLabelEnabledOnHover()
        {
            GameObject testButton = InstantiatePrefab(EmptyButtonPath);
            yield return null;

            //Ensure that there is a label 
            GameObject generatedLabel = GameObject.Find("SeeItSayItLabel-Canvas(Clone)");
            if (generatedLabel == null)
            {
                generatedLabel = InstantiateChildPrefab(SeeItSayItLabelPath, testButton.transform);
            }

            //and label child (the part that is enabled and disabled on hover)
            GameObject labelChild = null;
            if (generatedLabel.transform.childCount >= 1)
            {
                labelChild = generatedLabel.transform.GetChild(0).gameObject;
            }

            //No hover initially -- label should be disabled
            Assert.IsTrue(labelChild?.activeInHierarchy == false, "The label is disabled when the button is not hovered.");

            //Move hand to hover the object, wait for the animation to play
            yield return HoverButtonWithHand(testButton.transform.position);
            yield return RuntimeTestUtilities.WaitForFixedUpdates(frameCount: 50);
            Assert.IsTrue(labelChild?.activeInHierarchy == true, "The label is enabled when the button is hovered.");

            //Move hand away from the object
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
