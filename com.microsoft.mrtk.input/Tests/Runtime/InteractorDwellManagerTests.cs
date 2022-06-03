// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Tests for verifying the dwell behavior of interactors.
    /// </summary>
    public class InteractorDwellManagerTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Test far ray dwell on a StatefulInteractable.
        /// </summary>
        [UnityTest]
        public IEnumerator FarRayDwellTest()
        {
            // Instantiate a cube and attach the StatefulInteractable component
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StatefulInteractable interactable = cube.AddComponent<StatefulInteractable>();
            cube.transform.position = new Vector3(0, 0, 1);
            cube.transform.localScale = Vector3.one * 0.1f;

            // Configure the StatefulInteractable component to use far ray dwell
            const float dwellTime = 1f;
            interactable.UseFarDwell = true;
            interactable.FarDwellTime = dwellTime;
            interactable.UseGazeDwell = false;

            // Confirm the presence of DwellManager
            InteractorDwellManager dwellManager = GameObject.Find("MRTK RightHand Controller").transform.Find("Far Ray").GetComponent<InteractorDwellManager>();
            Assert.IsNotNull(dwellManager, "InteractorDwellManager does not exist on the MRTK RightHand Controller/Far Ray GameObject.");

            // Show the hand and confirm the interactable is being hovered but not selected yet
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(0, 0, 0.5f), true);
            Assert.IsTrue(interactable.IsRayHovered,
                          "StatefulInteractable did not get RayHovered.");
            Assert.IsFalse(interactable.isSelected,
                          "StatefulInteractable gets Selected too early.");

            // Wait for the dwell time to pass and confirm the interactable is selected
            yield return new WaitForSeconds(dwellTime);
            Assert.IsTrue(interactable.isSelected,
                          "StatefulInteractable did not get Selected.");

            // Wait for the dwell trigger time to pass and confirm the interactable is now not selected
            yield return new WaitForSeconds(dwellManager.DwellTriggerTime);
            Assert.IsFalse(interactable.isSelected,
                          "StatefulInteractable is still selected after DwellTriggerTime has passed.");

            yield return null;
        }

        /// <summary>
        /// Test gaze dwell on a StatefulInteractable.
        /// </summary>
        [UnityTest]
        public IEnumerator GazeDwellTest()
        {
            // Instantiate a cube and attach the StatefulInteractable component
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StatefulInteractable interactable = cube.AddComponent<StatefulInteractable>();
            cube.transform.position = new Vector3(0, 0, 1);
            cube.transform.localScale = Vector3.one * 0.1f;

            // Configure the StatefulInteractable component to use gaze dwell
            const float dwellTime = 1f;
            interactable.UseFarDwell = false;
            interactable.FarDwellTime = dwellTime;
            interactable.UseGazeDwell = true;

            // Confirm the presence of DwellManager
            InteractorDwellManager dwellManager = GameObject.Find("GazeInteractor").GetComponent<InteractorDwellManager>();
            Assert.IsNotNull(dwellManager, "InteractorDwellManager does not exist on the GazeInteractor GameObject.");
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            // Show the hand and confirm the interactable is being hovered but not selected yet
            Assert.IsTrue(interactable.IsGazeHovered,
                          "StatefulInteractable did not get GazeHovered.");
            Assert.IsFalse(interactable.isSelected,
                          "StatefulInteractable gets Selected too early.");

            // Wait for the dwell time to pass and confirm the interactable is selected
            yield return new WaitForSeconds(dwellTime);
            Assert.IsTrue(interactable.isSelected,
                          "StatefulInteractable did not get Selected.");

            // Wait for the dwell trigger time to pass and confirm the interactable is now not selected
            yield return new WaitForSeconds(dwellManager.DwellTriggerTime);
            Assert.IsFalse(interactable.isSelected,
                          "StatefulInteractable is still selected after DwellTriggerTime has passed.");

            yield return null;
        }
    }
}

