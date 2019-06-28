// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class PinchSliderTests
    {
        const string defaultPinchSliderPrefabPath = "Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Sliders/PinchSlider.prefab";

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Tests that a slider component can be added at runtime.
        /// at runtime.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestAddInteractableAtRuntime()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;
            Transform sliderThumbRoot;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Quaternion.identity, out pinchSliderObject, out slider, out sliderThumbRoot);
            yield return PlayModeTestUtilities.WaitForEnterKey();

            // clean up
            GameObject.Destroy(pinchSliderObject);
            yield return null;
        }
        /// <summary>
        /// Generates an interactable from primitives and assigns a select action.
        /// </summary>
        /// <param name="pinchSliderObject"></param>
        /// <param name="slider"></param>
        /// <param name="sliderThumbRoot"></param>
        /// <param name="selectActionDescription"></param>
        private void AssembleSlider(Vector3 position, Quaternion rotation, out GameObject pinchSliderObject, out PinchSlider slider, out Transform sliderThumbRoot)
        {
            // Assemble an interactable out of a set of primitives
            // This will be the slider root
            pinchSliderObject = new GameObject();
            pinchSliderObject.name = "PinchSliderRoot";

            // Make the slider track
            var sliderTrack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(sliderTrack.GetComponent<BoxCollider>());
            sliderTrack.transform.position = Vector3.zero;
            sliderTrack.transform.localScale = new Vector3(1f, .01f, .01f);
            sliderTrack.transform.parent = pinchSliderObject.transform;

            // Make the thumb root
            var thumbRoot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            thumbRoot.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            thumbRoot.transform.parent = pinchSliderObject.transform;
            sliderThumbRoot = thumbRoot.transform;

            slider = pinchSliderObject.AddComponent<PinchSlider>();
            slider.ThumbRoot = thumbRoot;

            pinchSliderObject.transform.position = position;
            pinchSliderObject.transform.localRotation = rotation;
        }

        /// <summary>
        /// Instantiates the default interactable button.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="sliderObject"></param>
        /// <param name="pinchSlider"></param>
        /// <param name="pinchSliderRoot"></param>
        private void InstantiateDefaultSliderPrefab(Vector3 position, Vector3 rotation, out GameObject sliderObject, out PinchSlider pinchSlider, out Transform pinchSliderRoot)
        {
            // Load interactable prefab
            Object sliderPrefab = AssetDatabase.LoadAssetAtPath(defaultPinchSliderPrefabPath, typeof(Object));
            sliderObject = Object.Instantiate(sliderPrefab) as GameObject;
            pinchSlider = sliderObject.GetComponent<PinchSlider>();
            Assert.IsNotNull(pinchSlider);

            // Find the target object for the interactable transformation
            pinchSliderRoot = pinchSlider.ThumbRoot.transform;

            // Move the object into position
            sliderObject.transform.position = position;
            sliderObject.transform.eulerAngles = rotation;
        }
    }
}
#endif