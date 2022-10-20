// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Tests for verifying the behavior of FuzzyGazeInteractor.
    /// </summary>
    public class FuzzyGazeInteractorTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Basic test of FuzzyGazeInteractor. Confirm a FuzzyGazeInteractor is active in the scene, and then
        /// make sure Interactable can be hovered even when not on the direct raycast from the interactor.
        /// </summary>
        [UnityTest]
        public IEnumerator BasicFuzzyGazeTest()
        {
            // Confirm a FuzzyGazeInteractor is active in the scene
            FuzzyGazeInteractor fuzzyGazeInteractor = Object.FindObjectOfType<FuzzyGazeInteractor>();
            Assert.IsNotNull(fuzzyGazeInteractor, "There is no active FuzzyGazeInteractor found in the scene.");

            // Instantiate two foregound cubes and one background cube for testing
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.AddComponent<StatefulInteractable>();
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.07f, 0.2f, 1));
            cube1.transform.localScale = Vector3.one * 0.1f;

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.AddComponent<StatefulInteractable>();
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.2f, 1));
            cube2.transform.localScale = Vector3.one * 0.1f;

            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.AddComponent<StatefulInteractable>();
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(1.6f);
            backgroundCube.transform.localScale = Vector3.one;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // No foreground cube should be hovered at their birth positions
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Move the cubes to bring them to the center on the y-axis
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.07f, 0, 1));
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0, 1));

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Cube 2 should now be hovered
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");

            // Move cube 2 back to its birth position
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.2f, 1));
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Background cube should now be hovered
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Move background cube further back
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(4f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Cube 1 should now be hovered
            Assert.IsTrue(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
        }

        /// <summary>
        /// Test the behavior of FuzzyGazeInteractor with different levels of precision and
        /// with performAdditionalRaycast on and off.
        /// </summary>
        [UnityTest]
        public IEnumerator FuzzyGazePrecisionTest()
        {
            // Confirm a FuzzyGazeInteractor is active in the scene and configure it for the test
            FuzzyGazeInteractor fuzzyGazeInteractor = Object.FindObjectOfType<FuzzyGazeInteractor>();
            Assert.IsNotNull(fuzzyGazeInteractor, "There is no active FuzzyGazeInteractor found in the scene.");
            fuzzyGazeInteractor.precision = 0;
            fuzzyGazeInteractor.performAdditionalRaycast = false;

            // Instantiate one foregound cubes and one background cube for testing
            GameObject foregroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            foregroundCube.AddComponent<StatefulInteractable>();
            foregroundCube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.241f, 0, 2));
            foregroundCube.transform.localScale = new Vector3(0.4f, 0.1f, 0.2f);

            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.AddComponent<StatefulInteractable>();
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.4f, 0, 2.4f));
            backgroundCube.transform.localScale = new Vector3(1.4f, 1.4f, 0.2f);
            backgroundCube.transform.localEulerAngles = new Vector3(0, -58, 0);

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Foreground cube should be hovered
            Assert.IsTrue(foregroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");

            // Increase the precision of the interactor
            fuzzyGazeInteractor.precision = FuzzyGazeInteractor.MaxPrecision;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Background cube should now be hovered
            Assert.IsFalse(foregroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Restore the precision to 0 but enable performAdditionalRaycast
            fuzzyGazeInteractor.precision = 0;
            fuzzyGazeInteractor.performAdditionalRaycast = true;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Background cube should still be hovered
            Assert.IsFalse(foregroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
        }
    }
}

