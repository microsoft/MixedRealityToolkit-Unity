// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Simulation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Tests to ensure the proper behavior of the interaction mode manager.
    /// </summary>
    public class InteractionModeManagerTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Tests that the proximity detector detects when to change the controllers interaction mode and properly toggles the associated interactors.
        /// Also checks that the proximity detector doesn't trigger hovers on other objects
        /// </summary>
        [UnityTest]
        public IEnumerator ProximityDetectorTest()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(1.0f, 0.1f, 1.0f);
            cube.transform.localScale = Vector3.one * 0.1f;
            cube.AddComponent<StatefulInteractable>();

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(0, 0, 0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            XRBaseController rightHandController = CachedLookup.RightHandController;
            Assert.IsTrue(rightHandController != null, "No controllers found for right hand.");

            // Magic number is tuned for a prox detector on the index tip with
            // a radius (collider) of 0.1. This is so that the prox detector should
            // overlap with the cube, but none of the interactors will.
            yield return rightHand.MoveTo(cube.transform.position + Vector3.back * 0.12f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(cube.GetComponent<StatefulInteractable>().isHovered,
                          "Interactable was hovered when it shouldn't have been. Was the radius of any of the interactors changed, or is a proximity detector firing hovers?");

            Assert.IsTrue(AnyProximityDetectorsTriggered(),
                           "The proximity detector should have detected the cube. Was the detector's radius changed, or is it broken?");

            InteractionMode currentMode = rightHandController.GetComponentInChildren<ProximityDetector>().ModeOnDetection;
            ValidateInteractionModeActive(rightHandController, currentMode);

            yield return null;
        }

        /// <summary>
        /// Tests the basic Interaction detector. The controller should enter one mode during hover, another during select, and fall back to the default mode during neither
        /// </summary>
        [UnityTest]
        public IEnumerator InteractionDetectorTest()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = InputTestUtilities.InFrontOfUser(1.5f);
            cube.transform.localScale = Vector3.one * 0.2f;
            cube.AddComponent<StatefulInteractable>();

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser());
            yield return RuntimeTestUtilities.WaitForUpdates();

            XRBaseController rightHandController = CachedLookup.RightHandController;
            Assert.IsTrue(rightHandController != null, "No controllers found for right hand.");

            // Moving the hand to a position where it's far ray is hovering over the cube
            yield return rightHand.AimAt(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            InteractionMode currentMode = rightHandController.GetComponentInChildren<MRTKRayInteractor>().GetComponent<InteractionDetector>().ModeOnHover;
            Assert.AreEqual(currentMode, rightHandController.GetComponentInChildren<MRTKRayInteractor>().GetComponent<InteractionDetector>().ModeOnDetection);
            ValidateInteractionModeActive(rightHandController, currentMode);

            yield return rightHand.SetHandshape(HandshapeTypes.HandshapeId.Grab);
            yield return RuntimeTestUtilities.WaitForUpdates();
            currentMode = rightHandController.GetComponentInChildren<MRTKRayInteractor>().GetComponent<InteractionDetector>().ModeOnSelect;
            Assert.AreEqual(currentMode, rightHandController.GetComponentInChildren<MRTKRayInteractor>().GetComponent<InteractionDetector>().ModeOnDetection);
            ValidateInteractionModeActive(rightHandController, currentMode);

            // move the hand far away and validate that we are in the default mode
            yield return rightHand.SetHandshape(HandshapeTypes.HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.MoveTo(cube.transform.position + new Vector3(3.0f,0,0));
            yield return RuntimeTestUtilities.WaitForUpdates();

            currentMode = InteractionModeManager.Instance.DefaultMode;
            ValidateInteractionModeActive(rightHandController, currentMode);
        }

        /// <summary>
        /// Tests that mode mediation works properly. The interaction mode with the higher priority should be the valid one which affects the controller.
        /// This test operates on the basic assumption that the priority order is FarRayHover < Near < GrabSelect
        /// </summary>
        [UnityTest]
        public IEnumerator ModeMediationTest()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = InputTestUtilities.InFrontOfUser(1.5f);
            cube.transform.localScale = Vector3.one * 0.2f;
            cube.AddComponent<StatefulInteractable>();
            yield return RuntimeTestUtilities.WaitForUpdates();

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser());
            yield return RuntimeTestUtilities.WaitForUpdates();

            XRBaseController rightHandController = CachedLookup.RightHandController;
            Assert.IsTrue(rightHandController != null, "No controllers found for right hand.");

            // Grab stabilization == ray stabilization
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, ControllerAnchorPoint.Grab);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Moving the hand to a position where it's far ray is hovering over the cube
            yield return rightHand.AimAt(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            InteractionMode farRayMode = rightHandController.GetComponentInChildren<MRTKRayInteractor>().GetComponent<InteractionDetector>().ModeOnHover;
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(farRayMode, rightHandController.GetComponentInChildren<MRTKRayInteractor>().GetComponent<InteractionDetector>().ModeOnDetection);
            ValidateInteractionModeActive(rightHandController, farRayMode);

            // Now move the hand in range for the proximity detector
            yield return rightHand.MoveTo(cube.transform.position - Vector3.forward * 0.09f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            InteractionMode nearMode = rightHandController.GetComponentInChildren<ProximityDetector>().ModeOnDetection;
            yield return RuntimeTestUtilities.WaitForUpdates();
            ValidateInteractionModeActive(rightHandController, nearMode);
            Assert.IsTrue(nearMode.priority > farRayMode.priority);

            // Finally move in for a grab
            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeTypes.HandshapeId.Grab);
            yield return RuntimeTestUtilities.WaitForUpdates();

            InteractionMode grabMode = rightHandController.GetComponentInChildren<GrabInteractor>().GetComponent<InteractionDetector>().ModeOnSelect;
            Assert.AreEqual(grabMode, rightHandController.GetComponentInChildren<GrabInteractor>().GetComponent<InteractionDetector>().ModeOnDetection);
            yield return RuntimeTestUtilities.WaitForUpdates();
            ValidateInteractionModeActive(rightHandController, grabMode);
            Assert.IsTrue(grabMode.priority > nearMode.priority);

            // Run it all in reverse and make sure the interaction stack is in order
            // Now move the hand in range for the proximity detector
            yield return rightHand.SetHandshape(HandshapeTypes.HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.MoveTo(cube.transform.position - Vector3.forward * 0.09f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            ValidateInteractionModeActive(rightHandController, nearMode);

            // Moving the hand to a position where it's far ray is hovering over the cube
            yield return rightHand.MoveTo(cube.transform.position + new Vector3(0.02f, -0.1f, -0.8f));
            yield return RuntimeTestUtilities.WaitForUpdates(frameCount:120);

            ValidateInteractionModeActive(rightHandController, farRayMode);
        }

        /// <summary>
        /// Validates that an interaction mode is active for the specified controller
        /// </summary>
        /// <param name="controller">The controller we are checking</param>
        /// <param name="currentMode">The interaction mode we expect to be active for the controller</param>
        private void ValidateInteractionModeActive(XRBaseController controller, InteractionMode currentMode)
        {
            // We construct the list of managed interactor types manually because we don't want to expose the internal controller mapping implementation to even internal use, since
            // we don't want any other class to be able to modify those collections without going through the Mode Manager or it's in-editor inspector.
            HashSet<System.Type> managedInteractorTypes = new HashSet<System.Type>(InteractionModeManager.Instance.PrioritizedInteractionModes.SelectMany(x => x.AssociatedTypes));
            HashSet<System.Type> activeInteractorTypes = InteractionModeManager.Instance.PrioritizedInteractionModes.Find(x => x.ModeName == currentMode.name).AssociatedTypes;

            // Ensure the prox detector has actually had the desired effect of enabling/disabling interactors.
            foreach (System.Type interactorType in managedInteractorTypes)
            {
                XRBaseInteractor interactor = controller.GetComponentInChildren(interactorType) as XRBaseControllerInteractor;
                if (interactor != null)
                {
                    Assert.AreEqual(activeInteractorTypes.Contains(interactorType), interactor.enabled);
                }
            }
        }

        // Returns true iff any of the ProximityDetectors in the scene are currently triggered.
        private bool AnyProximityDetectorsTriggered()
        {
            ProximityDetector[] detectors = Object.FindObjectsOfType<ProximityDetector>();
            foreach (var detector in detectors)
            {
                if (detector.IsModeDetected())
                {
                    return true;
                }
            }

            return false;
        }
    }
}

