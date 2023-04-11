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
    /// Tests for the Canvas Dialog UX component.
    /// </summary>
    public class SeeItSayItTests : BaseRuntimeInputTests
    {
        //SeeItSayItLabel/SeeItSayItLabel-NonCanvas.prefab
        private const string SeeItSayItLabelGuid = "d9e84b5a8037fd946aa503a059fee93f";
        private static readonly string SeeItSayItLabelPath = AssetDatabase.GUIDToAssetPath(SeeItSayItLabelGuid);

        //Button/Prefabs/Empty Button.prefab
        private const string EmptyButtonGuid = "b85e005d231192249b7077b40a4d4e45";
        private static readonly string EmptyButtonPath = AssetDatabase.GUIDToAssetPath(EmptyButtonGuid);

        [UnityTest]
        public IEnumerator SeeItSayItLabelInstantiate()
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
        public IEnumerator EmptyButtonSeeItSayItLabel()
        {
            GameObject testButton = InstantiatePrefab(EmptyButtonPath);
            yield return null;
            SeeItSayItGenerator labelGeneratorComponent = testButton.GetComponent<SeeItSayItGenerator>();
            Assert.IsNotNull(labelGeneratorComponent, "SeeItSayIt generator component exists on empty button prefab");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator IsLabelChildEnabledOnHover()
        {
            GameObject testButton = InstantiatePrefab(EmptyButtonPath);
            yield return null;

            GameObject generatedLabel = GameObject.Find("SeeItSayItLabel-Canvas(Clone)");
            if (generatedLabel == null)
            {
                generatedLabel = InstantiateChildPrefab(SeeItSayItLabelPath, testButton.transform);
            }

            GameObject labelChild = null;
            if (generatedLabel.transform.childCount >= 1)
            {
                labelChild = generatedLabel.transform.GetChild(0).gameObject;
            }
            Assert.IsTrue(labelChild?.activeInHierarchy == false, "The label is disabled when the button is not hovered.");

            yield return HoverButtonWithHand(testButton.transform.position);

            for (int i = 0; i < 12; i++)
            {
                yield return 0;
            }

            Assert.IsTrue(labelChild?.activeInHierarchy == true, "The label is enabled when the button is hovered.");

            yield return ReleaseButtonWithHand(testButton.transform.position);
            yield return null;

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
        /// Move the hand forward to press button
        /// </summary>
        private IEnumerator HoverButtonWithHand(Vector3 buttonPosition)
        {
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return hand.MoveTo(buttonPosition);

            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// Move the hand off to the right to release the button
        /// </summary>
        private IEnumerator ReleaseButtonWithHand(Vector3 buttonPosition, bool doRolloff = false)
        {
            Vector3 p3 = new Vector3(doRolloff ? 0.1f : 0.0f, 0, -0.05f);

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.MoveTo(buttonPosition + p3);
            yield return hand.Hide();
        }
    }


}
