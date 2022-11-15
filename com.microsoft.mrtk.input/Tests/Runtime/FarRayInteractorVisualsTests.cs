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
    public class FarRayInteractorVisualsTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Basic test of FuzzyGazeInteractor. Confirm a FuzzyGazeInteractor is active in the scene, and then
        /// make sure Interactable can be hovered even when not on the direct raycast from the interactor.
        /// </summary>
        [UnityTest]
        public IEnumerator ReticleAndLineVisualActiveTest()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGaze();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = InputTestUtilities.InFrontOfUser(1f);
            testObject.transform.position = initialObjectPosition;
            testObject.AddComponent<StatefulInteractable>();

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            Vector3 hoverPosition = InputTestUtilities.InFrontOfUser(0.6f);
            Quaternion hoverRotation = Quaternion.identity;

            // Check that our components are enabled
            var lineVisual = CachedLookup.RightHandController.GetComponentInChildren<MRTKLineVisual>();
            var reticleVisual = CachedLookup.RightHandController.GetComponentInChildren<MRTKRayReticleVisual>();
            Assert.IsTrue(lineVisual.enabled);
            Assert.IsTrue(reticleVisual.enabled);

            // Check that the ray is active and the reticle is not
            Assert.IsTrue(lineVisual.GetComponentInChildren<LineRenderer>().enabled);
            Assert.IsFalse(reticleVisual.Reticle.activeSelf);

            yield return hand.MoveTo(hoverPosition);
            yield return hand.RotateTo(hoverRotation);

            // Check that both are active
            Assert.IsTrue(lineVisual.GetComponentInChildren<LineRenderer>().enabled);
            Assert.IsTrue(reticleVisual.Reticle.activeSelf);

            // disable the components and check that all visuals are disabled
            lineVisual.enabled = false;
            reticleVisual.enabled = false;
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check that both are disabled
            Assert.IsFalse(lineVisual.GetComponentInChildren<LineRenderer>().enabled);
            Assert.IsFalse(reticleVisual.Reticle.activeSelf);

            // Make sure they are still disabled after moving the hand back to the inital position
            yield return hand.MoveTo(Vector3.zero);
            yield return hand.RotateTo(Quaternion.identity);

            // Check that both are disabled
            Assert.IsFalse(lineVisual.GetComponentInChildren<LineRenderer>().enabled);
            Assert.IsFalse(reticleVisual.Reticle.activeSelf);

            // Make sure we are back in the correct visibility state after reactivating the visuals
            lineVisual.enabled = true;
            reticleVisual.enabled = true;
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(lineVisual.GetComponentInChildren<LineRenderer>().enabled);
            Assert.IsFalse(reticleVisual.Reticle.activeSelf);
        }
    }
}

