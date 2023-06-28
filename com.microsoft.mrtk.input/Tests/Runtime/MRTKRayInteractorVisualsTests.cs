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
    /// Tests for verifying the behavior of visuals related to the MRTKRayInteractor
    /// </summary>
    public class MRTKRayInteractorVisualsTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Ensure that far ray interactor visuals are set active/inactive appropriately.
        /// </summary>
        [UnityTest]
        public IEnumerator ReticleAndLineVisualActiveTest()
        {
            // Because many of our visual scripts rely on OnBeforeRender, exit early if this test
            // is being run in batchmode (which does not rendering)
            if (Application.isBatchMode)
            {
                Debug.Log("Skipping test ReticleAndLineVisualActiveTest, as it does not work in batch mode settings");
                yield break;
            }

            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGazeInteractor();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = InputTestUtilities.InFrontOfUser(1f);
            testObject.transform.position = initialObjectPosition;
            testObject.AddComponent<StatefulInteractable>();

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check that our components are enabled
            var lineVisual = CachedLookup.RightHandController.GetComponentInChildren<MRTKLineVisual>();
            var reticleVisual = CachedLookup.RightHandController.GetComponentInChildren<MRTKRayReticleVisual>();
            Assert.IsTrue(lineVisual.enabled);
            Assert.IsTrue(reticleVisual.enabled);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check that the ray is active and the reticle is not
            Assert.IsTrue(lineVisual.GetComponentInChildren<LineRenderer>().enabled);
            Assert.IsFalse(reticleVisual.Reticle.activeSelf);

            Vector3 hoverPosition = InputTestUtilities.InFrontOfUser(0.6f);
            Quaternion hoverRotation = Quaternion.identity;

            yield return hand.MoveTo(hoverPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.RotateTo(hoverRotation);
            yield return RuntimeTestUtilities.WaitForUpdates();

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
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.RotateTo(Quaternion.identity);
            yield return RuntimeTestUtilities.WaitForUpdates();

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

