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

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class FocusProviderTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// </summary>
        [UnityTest]
        public IEnumerator TestGazeCursorArticulated()
        {
            IMixedRealityInputSystem inputSystem = PlayModeTestUtilities.GetInputSystem();
            yield return null;

            // Verify that the gaze cursor is visible at the start
            Assert.IsTrue(inputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should be visible at start");

            // raise hand up -- gaze cursor should no longer be visible
            // disable user input
            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            inputSimulationService.UserInputEnabled = false;

            ArticulatedHandPose gesturePose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Open);
            var handOpenPose = PlayModeTestUtilities.GenerateHandPose(ArticulatedHandPose.GestureId.Open, Handedness.Right, Vector3.forward * 0.1f, Quaternion.identity);
            inputSimulationService.HandDataRight.Update(true, false, handOpenPose);
            yield return null;

            // Gaze cursor should not be visible
            Assert.IsFalse(inputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should not be visible when one articulated hand is up");
            inputSimulationService.HandDataRight.Update(false, false, handOpenPose);
            yield return null;

            // Say "select" to make gaze cursor active again
            // Really we need to tear down the scene and create it again but MRTK doesn't support that yet
            var gazeInputSource = inputSystem.DetectedInputSources.Where(x => x.SourceName.Equals("Gaze")).First();
            inputSystem.RaiseSpeechCommandRecognized(gazeInputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(), System.DateTime.Now, new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            yield return null;
            Assert.IsTrue(inputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should be visible after select command");
        }

        /// <summary>
        /// Ensure that the gaze provider hit result is not null when looking at an object,
        /// even when the hand is up
        /// </summary>
        [UnityTest]
        public IEnumerator TestGazeProviderTargetNotNull()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.forward;

            yield return null;
            yield return null;

            Assert.NotNull(CoreServices.InputSystem.GazeProvider.GazeTarget, "GazeProvider target is null when looking at an object");

            TestHand h = new TestHand(Handedness.Right);
            yield return h.Show(Vector3.forward * 0.2f);
            yield return null;

            Assert.NotNull(CoreServices.InputSystem.GazeProvider.GazeTarget, "GazeProvider target is null when looking at an object with hand raised");
        }

        /// <summary>
        /// Ensure FocusProvider's FocusDetails can be overridden.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOverrideFocusDetails()
        {
            PlayModeTestUtilities.Setup();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yield return null;

            var focusProvider = PlayModeTestUtilities.GetInputSystem().FocusProvider;

            var pointer = new TestPointer();
            Assert.IsFalse(focusProvider.TryGetFocusDetails(pointer, out var focusDetails));
            Assert.IsFalse(focusProvider.TryOverrideFocusDetails(pointer, new Physics.FocusDetails()));

            focusProvider.RegisterPointer(pointer);
            yield return null;

            Assert.IsTrue(focusProvider.TryGetFocusDetails(pointer, out focusDetails));
            Assert.IsNull(focusDetails.Object);

            var newFocusDetails = new Physics.FocusDetails();
            newFocusDetails.Object = cube;
            newFocusDetails.RayDistance = 10;
            newFocusDetails.Point = new Vector3(1, 2, 3);
            Assert.IsTrue(focusProvider.TryOverrideFocusDetails(pointer, newFocusDetails));

            Assert.IsTrue(focusProvider.TryGetFocusDetails(pointer, out focusDetails));
            Assert.AreEqual(newFocusDetails.Object, focusDetails.Object);
            Assert.AreEqual(newFocusDetails.RayDistance, focusDetails.RayDistance);
            Assert.AreEqual(newFocusDetails.Point, focusDetails.Point);
            Assert.AreEqual(newFocusDetails.Object, focusProvider.GetFocusedObject(pointer));
        }

        /// <summary>
        /// Ensure that focused object is set to null when we disable the currently focused object
        /// </summary>
        [UnityTest]
        public IEnumerator TestDisableFocusObject()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.right;
            cube.transform.localScale = Vector3.one * 0.2f;

            const int numHandSteps = 1;

            // No initial focus
            cube.transform.position = Vector3.forward;
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward * 0.5f);
            yield return null;

            var handRayPointer = hand.GetPointer<ShellHandRayPointer>();

            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);

            // Focus on cube
            yield return hand.MoveTo(new Vector3(0.06f, -0.1f, 0.5f), numHandSteps);
            yield return null;

            Assert.AreEqual(cube, handRayPointer.Result.CurrentPointerTarget);

            // Deactivate the cube
            cube.SetActive(false);
            yield return null;

            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);

            // Reactivate the cube
            cube.SetActive(true);
            yield return null;

            Assert.AreEqual(cube, handRayPointer.Result.CurrentPointerTarget);
        }

        /// <summary>
        /// Ensure that focused object is set to null, and that focus lock is reset when we disable 
        /// the currently focus-locked (aka grabbed) object
        /// </summary>
        [UnityTest]
        public IEnumerator TestDisableFocusLockedObject()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<ManipulationHandler>(); // Add focus handler so focus can lock
            cube.transform.position = Vector3.right;
            cube.transform.localScale = Vector3.one * 0.2f;

            const int numHandSteps = 1;

            // No initial focus
            cube.transform.position = Vector3.forward;
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward * 0.5f);
            yield return null;

            var handRayPointer = hand.GetPointer<ShellHandRayPointer>();

            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);

            // Focus lock cube
            yield return hand.MoveTo(new Vector3(0.06f, -0.1f, 0.5f), numHandSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            Assert.IsTrue(handRayPointer.IsFocusLocked);
            Assert.AreEqual(cube, handRayPointer.Result.CurrentPointerTarget);

            // Deactivate the cube
            cube.SetActive(false);
            yield return null;

            Assert.IsFalse(handRayPointer.IsFocusLocked);
            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);

            // Reactivate the cube
            cube.SetActive(true);
            yield return null;

            Assert.IsFalse(handRayPointer.IsFocusLocked);
            Assert.AreEqual(cube, handRayPointer.Result.CurrentPointerTarget);
        }

        /// <summary>
        /// Ensure that focused object is set to null when we destroy the currently focused object
        /// </summary>
        [UnityTest]
        public IEnumerator TestDestroyFocusObject()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.right;
            cube.transform.localScale = Vector3.one * 0.2f;

            const int numHandSteps = 1;

            // No initial focus
            cube.transform.position = Vector3.forward;
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward * 0.5f);
            yield return null;

            var handRayPointer = hand.GetPointer<ShellHandRayPointer>();

            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);

            // Focus on cube
            yield return hand.MoveTo(new Vector3(0.06f, -0.1f, 0.5f), numHandSteps);
            yield return null;

            Assert.AreEqual(cube, handRayPointer.Result.CurrentPointerTarget);

            // Destroy the cube
            Object.DestroyImmediate(cube);
            yield return null;

            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);
            // Verify that CurrentPointer is not still referencing the destroyed GameObject
            Assert.IsTrue(ReferenceEquals(handRayPointer.Result.CurrentPointerTarget, null));
        }

        /// <summary>
        /// Ensure that focused object is set to null, and that focus lock is reset when we destroy 
        /// the currently focus-locked (aka grabbed) object
        /// </summary>
        [UnityTest]
        public IEnumerator TestDestroyFocusLockedObject()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<ManipulationHandler>(); // Add focus handler so focus can lock
            cube.transform.position = Vector3.right;
            cube.transform.localScale = Vector3.one * 0.2f;

            const int numHandSteps = 1;

            // No initial focus
            cube.transform.position = Vector3.forward;
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward * 0.5f);
            yield return null;

            var handRayPointer = hand.GetPointer<ShellHandRayPointer>();

            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);

            // Focus lock cube
            yield return hand.MoveTo(new Vector3(0.06f, -0.1f, 0.5f), numHandSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            Assert.IsTrue(handRayPointer.IsFocusLocked);
            Assert.AreEqual(cube, handRayPointer.Result.CurrentPointerTarget);

            // Destroy the cube
            Object.DestroyImmediate(cube);
            yield return null;

            Assert.IsFalse(handRayPointer.IsFocusLocked);
            Assert.IsNull(handRayPointer.Result.CurrentPointerTarget);
            // Verify that CurrentPointer is not still referencing the destroyed GameObject
            Assert.IsTrue(ReferenceEquals(handRayPointer.Result.CurrentPointerTarget, null));
        }
    }
}
#endif
