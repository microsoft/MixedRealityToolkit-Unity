// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class InteractableTests
    {
        /// <summary>
        /// Tests that an interactable component can be added to a GameObject
        /// at runtime.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestAddInteractableAtRuntime()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // This should not throw an exception
            var interactable = cube.AddComponent<Interactable>();

            // clean up
            GameObject.Destroy(cube);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestInstantiateAndInteractWithPrefab()
        {
            // instantiate scene and interactable
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            Object interactablePrefab = AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.Examples/Demos/UX/Interactables/Prefabs/Model_PushButton.prefab", typeof(Object));
            GameObject testInteractable = Object.Instantiate(interactablePrefab) as GameObject;
            Interactable interactable = testInteractable.GetComponent<Interactable>();
            Assert.IsNotNull(interactable);

            // Find the target object for the interactable transformation
            Transform translateTargetObject = testInteractable.transform.Find("Cylinder");
            Assert.IsNotNull(translateTargetObject, "Object 'Cylinder' could not be found under example object Model_PushButton.");

            // Move the interactable into its press position
            testInteractable.transform.localPosition = new Vector3(0.025f, 0.05f, 0.5f);
            testInteractable.transform.eulerAngles = new Vector3(-90f, 0f, 0f);

            // Move the camera to origin looking at +z to more easily see the button.
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            Vector3 targetStartPosition = translateTargetObject.localPosition;
            
            // Move the hand forward to intersect the interactable
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 32;
            Vector3 p1 = new Vector3(0.0f, 0f, 0f);
            Vector3 p2 = new Vector3(0.05f, 0f, 0.51f);
            Vector3 p3 = new Vector3(0.0f, 0f, 0.0f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            
            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Scale target object was not translated by action.");

            // Move the hand back
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Scale target object was not translated back by action.");

            Object.Destroy(testInteractable);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }
    }
}
#endif