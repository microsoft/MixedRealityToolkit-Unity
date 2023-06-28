using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using Microsoft.MixedReality.Toolkit.Core.Tests;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Runtime.Tests
{
    /// <summary>
    /// Tests for TapToPlace solver
    /// </summary>
    public class SolverTapToPlaceTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Verify TapToPlace can move an object to the end of the right hand ray.
        /// </summary>
        [UnityTest]
        public IEnumerator TapToPlaceFollowsRightHandRay()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGazeInteractor();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();
            var solver = testObject.AddComponent<TapToPlace>();

            // Disable smoothing so moving happens instantly. This makes testing positions easier.
            solver.Smoothing = false;

            // Set it to track interactors
            solverHandler.TrackedHandedness = Handedness.Both;
            solverHandler.TrackedTargetType = TrackedObjectType.Interactor;
            var lookup = GameObject.FindObjectOfType<ControllerLookup>();
            var leftInteractor = lookup.LeftHandController.GetComponentInChildren<MRTKRayInteractor>();
            var rightInteractor = lookup.RightHandController.GetComponentInChildren<MRTKRayInteractor>();
            solverHandler.LeftInteractor = leftInteractor;
            solverHandler.RightInteractor = rightInteractor;

            yield return RuntimeTestUtilities.WaitForUpdates();

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);
            var rightHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.05f, 1f));
            var leftHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, -0.05f, 1f));

            testObject.transform.position = InputTestUtilities.InFrontOfUser(3.0f);

            yield return rightHand.Show(rightHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.AimAt(testObject.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return leftHand.Show(leftHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return leftHand.AimAt(testObject.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if TapToPlace starts without being in "placement" mode.
            Assert.IsFalse(solver.IsBeingPlaced, "TapToPlace should have starting without being in placement mode.");

            // Start placement and move hand.
            solver.StartPlacement();
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if TapToPlace started.
            Assert.IsTrue(solver.IsBeingPlaced, "TapToPlace should have started.");
            var testObjectStartPosition = testObject.transform.position;

            // Aim hand and move object.
            yield return rightHand.AimAt(InputTestUtilities.InFrontOfUser(new Vector3(0.05f, 0.1f, 2.0f)));
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify shape moved to placement
            var testObjectPlacementPosition = testObject.transform.position;
            Assert.AreNotEqual(testObjectStartPosition, testObjectPlacementPosition, $"Game object did not move");

            // Wait for solvers double click prevention timeout
            yield return new WaitForSeconds(0.5f + 0.1f);

            // Clicking with opposite hand should stop movement
            yield return leftHand.Click();
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if TapToPlace stopped with pinch.
            Assert.IsFalse(solver.IsBeingPlaced, "TapToPlace should have stopped with left hand pinch.");

            // Aim hand
            yield return rightHand.AimAt(InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, -0.1f, 2.0f)));
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify shape did not moved 
            var testObjectFinalPosition = testObject.transform.position;
            Assert.AreEqual(testObjectPlacementPosition, testObjectFinalPosition, $"Game object should not have moved.");
        }

        /// <summary>
        /// Verify TapToPlace can move an object to the end of the left hand ray.
        /// </summary>
        [UnityTest]
        public IEnumerator TapToPlaceFollowsLeftHandRay()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGazeInteractor();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();
            var solver = testObject.AddComponent<TapToPlace>();

            // Disable smoothing so moving happens instantly. This makes testing positions easier.
            solver.Smoothing = false;

            // Set it to track interactors
            solverHandler.TrackedHandedness = Handedness.Both;
            solverHandler.TrackedTargetType = TrackedObjectType.Interactor;
            var lookup = GameObject.FindObjectOfType<ControllerLookup>();
            var leftInteractor = lookup.LeftHandController.GetComponentInChildren<MRTKRayInteractor>();
            var rightInteractor = lookup.RightHandController.GetComponentInChildren<MRTKRayInteractor>();
            solverHandler.LeftInteractor = leftInteractor;
            solverHandler.RightInteractor = rightInteractor;

            yield return RuntimeTestUtilities.WaitForUpdates();

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);
            var rightHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.05f, 1f));
            var leftHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, -0.05f, 1f));

            testObject.transform.position = InputTestUtilities.InFrontOfUser(3.0f);

            yield return leftHand.Show(leftHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return leftHand.AimAt(testObject.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.Show(rightHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.AimAt(testObject.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if TapToPlace starts without being in "placement" mode.
            Assert.IsFalse(solver.IsBeingPlaced, "TapToPlace should have starting without being in placement mode.");

            // Start placement and move hand.
            solver.StartPlacement();
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if TapToPlace started.
            Assert.IsTrue(solver.IsBeingPlaced, "TapToPlace should have started.");
            var testObjectStartPosition = testObject.transform.position;

            // Aim hand and move object.
            yield return leftHand.AimAt(InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.1f, 2.0f)));
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify shape moved to placement
            var testObjectPlacementPosition = testObject.transform.position;
            Assert.AreNotEqual(testObjectStartPosition, testObjectPlacementPosition, $"Game object did not move");

            // Wait for solvers double click prevention timeout
            yield return new WaitForSeconds(0.5f + 0.1f);

            // Clicking with opposite hand should stop movement
            yield return rightHand.Click();
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if TapToPlace stopped with pinch.
            Assert.IsFalse(solver.IsBeingPlaced, "TapToPlace should have stopped with left hand pinch.");

            // Aim hand
            yield return leftHand.AimAt(InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.1f, 2.0f)));
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify shape did not moved 
            var testObjectFinalPosition = testObject.transform.position;
            Assert.AreEqual(testObjectPlacementPosition, testObjectFinalPosition, $"Game object should not have moved.");
        }
    }
}
