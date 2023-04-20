// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using Microsoft.MixedReality.Toolkit.Input.Simulation;
using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Runtime.Tests
{
    /// <summary>
    /// Tests for SolverHandler
    /// </summary>
    public class SolverHandlerTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// This checks if the SolverHandler correctly switches to the active hand when tracking
        /// two interactors
        /// </summary>
        [UnityTest]
        public IEnumerator SolverHandlerInteractorSwitchesToActiveHand()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGaze();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();

            // Set it to track interactors
            solverHandler.TrackedTargetType = TrackedObjectType.Interactor;
            var lookup = GameObject.FindObjectOfType<ControllerLookup>();
            var leftInteractor = lookup.LeftHandController.GetComponentInChildren<MRTKRayInteractor>();
            var rightInteractor = lookup.RightHandController.GetComponentInChildren<MRTKRayInteractor>();
            solverHandler.LeftInteractor = leftInteractor;
            solverHandler.RightInteractor = rightInteractor;

            yield return RuntimeTestUtilities.WaitForUpdates();

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);
            var initialHandPosition = InputTestUtilities.InFrontOfUser(0.5f);

            yield return rightHand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if SolverHandler starts with target on right hand
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.RightInteractor.transform.position, $"Solver Handler started tracking incorrect hand");

            // Hide the right hand and make the left hand active at a new position
            yield return rightHand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();
            var secondHandPosition = new Vector3(-0.05f, -0.05f, 1f);
            yield return leftHand.Show(secondHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if the SolverHandler moves the target to the left hand
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.LeftInteractor.transform.position, $"Solver Handler did not switch to active hand");

            // Repeat the test, but hide the left hand this time
            yield return leftHand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();
            Vector3 finalPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, 0.05f, 0.5f));
            yield return rightHand.Show(finalPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if the SolverHandler moves the target back to the right hand
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.RightInteractor.transform.position, $"Solver Handler did not switch to final hand");
        }

        /// <summary>
        /// This checks if the SolverHandler starts tracking the preferred hand if both hands are view when tracking
        /// two interactors
        /// </summary>
        [UnityTest]
        public IEnumerator SolverHandlerInteractorPreferredHandedness()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGaze();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();

            yield return RuntimeTestUtilities.WaitForUpdates();
            // Set it to track interactors
            solverHandler.TrackedTargetType = TrackedObjectType.Interactor;
            var lookup = GameObject.FindObjectOfType<ControllerLookup>();
            var leftInteractor = lookup.LeftHandController.GetComponentInChildren<MRTKRayInteractor>();
            var rightInteractor = lookup.RightHandController.GetComponentInChildren<MRTKRayInteractor>();
            solverHandler.LeftInteractor = leftInteractor;
            solverHandler.RightInteractor = rightInteractor;

            // Set preferred tracked handedness to right
            solverHandler.PreferredTrackedHandedness = Handedness.Right;

            yield return RuntimeTestUtilities.WaitForUpdates();

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);
            var rightHandPos = new Vector3(-0.05f, -0.05f, 1f);
            var leftHandPos = new Vector3(0.05f, 0.05f, 1f);

            yield return rightHand.Show(rightHandPos);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return leftHand.Show(leftHandPos);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if SolverHandler tracks preferred hand if both are visible
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.RightInteractor.transform.position, $"Solver Handler not tracking preferred hand");
        }

        /// <summary>
        /// This checks if the SolverHandler keeps tracking the current active hand if another one comes
        /// in view when tracking two interactors
        /// </summary>
        [UnityTest]
        public IEnumerator SolverHandlerInteractorTracksInitialActiveHand()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGaze();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();

            // Set it to track interactors
            solverHandler.TrackedTargetType = TrackedObjectType.Interactor;
            var lookup = GameObject.FindObjectOfType<ControllerLookup>();
            var leftInteractor = lookup.LeftHandController.GetComponentInChildren<MRTKRayInteractor>();
            var rightInteractor = lookup.RightHandController.GetComponentInChildren<MRTKRayInteractor>();
            solverHandler.LeftInteractor = leftInteractor;
            solverHandler.RightInteractor = rightInteractor;

            yield return new WaitForFixedUpdate();
            yield return null;

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);
            var rightHandPos = new Vector3(-0.05f, -0.05f, 1f);
            var leftHandPos = new Vector3(0.05f, 0.05f, 1f);

            yield return rightHand.Show(rightHandPos);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if SolverHandler starts with target on right hand
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.RightInteractor.transform.position, $"Solver Handler started tracking incorrect hand");

            yield return leftHand.Show(leftHandPos);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check that the SolverHandler keeps tracking the right hand
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.RightInteractor.transform.position, $"Solver Handler switched to wrong active hand");
        }

        /// <summary>
        /// This checks if the SolverHandler moves with the active hand when tracking two interactors
        /// </summary>
        [UnityTest]
        public IEnumerator SolverHandlerInteractorMovesWithHand()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGaze();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();

            // Set it to track interactors
            solverHandler.TrackedTargetType = TrackedObjectType.Interactor;
            var lookup = GameObject.FindObjectOfType<ControllerLookup>();
            var leftInteractor = lookup.LeftHandController.GetComponentInChildren<MRTKRayInteractor>();
            var rightInteractor = lookup.RightHandController.GetComponentInChildren<MRTKRayInteractor>();
            solverHandler.LeftInteractor = leftInteractor;
            solverHandler.RightInteractor = rightInteractor;

            yield return new WaitForFixedUpdate();
            yield return null;

            TestHand rightHand = new TestHand(Handedness.Right);
            var initialHandPos = new Vector3(-0.05f, -0.05f, 1f);

            yield return rightHand.Show(initialHandPos);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if SolverHandler starts with target on right hand
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.RightInteractor.transform.position, $"Solver Handler started tracking incorrect hand");

            var finalHandPos = new Vector3(0.05f, 0.05f, 1f);
            yield return rightHand.MoveTo(finalHandPos);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check that the SolverHandler keeps tracking the right hand
            Assert.IsTrue(solverHandler.TransformTarget.position == solverHandler.RightInteractor.transform.position, $"Solver Handler did not follow hand");
        }

        /// <summary>
        /// This checks if the SolverHandler moves with head when tracking the head
        /// </summary>
        [UnityTest]
        public IEnumerator SolverHandlerHeadMovesWithHead()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGaze();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();

            // Set it to track head
            solverHandler.TrackedTargetType = TrackedObjectType.Head;

            yield return new WaitForFixedUpdate();
            yield return null;

            Camera.main.transform.position = new Vector3(0.1f, 0.1f, 0.1f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if SolverHandler starts at camera pose
            Assert.IsTrue(solverHandler.TransformTarget.position == Camera.main.transform.position, $"Solver Handler not tracking head");

            Camera.main.transform.position = new Vector3(1f, 1f, 1f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if SolverHandler starts at camera pose
            Assert.IsTrue(solverHandler.TransformTarget.position == Camera.main.transform.position, $"Solver Handler not moving with head");
        }

        /// <summary>
        /// This checks if the SolverHandler correctly applies additional offset and rotation
        /// </summary>
        [UnityTest]
        public IEnumerator SolverHandlerAppliesOffset()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGaze();

            // Set up GameObject with a SolverHandler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var solverHandler = testObject.AddComponent<SolverHandler>();

            // Set it to track head
            solverHandler.TrackedTargetType = TrackedObjectType.Head;

            // Apply additional offsets
            solverHandler.AdditionalOffset = Vector3.one;
            solverHandler.AdditionalRotation = new Vector3(30f, 30f, 30f);

            yield return new WaitForFixedUpdate();
            yield return null;

            Camera.main.transform.position = new Vector3(0.1f, 0.1f, 0.1f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Check if SolverHandler is at expected position and rotation
            Vector3 expectedPos = Camera.main.transform.position + solverHandler.AdditionalOffset;
            Quaternion expectedDir = Camera.main.transform.rotation*Quaternion.Euler(30f, 30f, 30f);
            Assert.IsTrue(solverHandler.TransformTarget.position == expectedPos, $"Solver Handler not applying additional offset");
            Assert.IsTrue(solverHandler.TransformTarget.rotation == expectedDir, $"Solver Handler not applying additional rotation");
        }
    }
}
