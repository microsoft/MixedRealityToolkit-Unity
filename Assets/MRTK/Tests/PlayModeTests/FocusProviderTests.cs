// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class FocusProviderTests : BasePlayModeTests
    {
        /// <summary>
        /// Test that the gaze cursor behaves properly with articulated hand pointers.
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

            ArticulatedHandPose gesturePose = SimulatedArticulatedHandPoses.GetGesturePose(ArticulatedHandPose.GestureId.Open);
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
        /// Tests gaze cursor visibility when near interaction pointer is active and near grabbable.
        /// </summary>
        [UnityTest]
        public IEnumerator TestGazeCursorWhenHandsNearGrabbable()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create grabbable cube
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            cube.transform.localScale = Vector3.one * 0.35f;
            cube.transform.position = new Vector3(-0.2f, 0.3f, 4.2f);
            yield return null;

            // No hands, default cursor should be visible
            Assert.IsTrue(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible, "Head gaze cursor should be visible");

            // Hand up near grabbable
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward * 4.05f);
            yield return null;

            var grabPointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);

            // Grab pointer is near grabbable
            Assert.IsTrue(grabPointer.IsNearObject, "Grab pointer should be near a grabbable");

            // Head cursor invisible when grab pointer is near grabbable
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible, "Eye gaze cursor should not be visible");

            // Enabling eye tracking data
            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            inputSimulationService.EyeGazeSimulationMode = EyeGazeSimulationMode.CameraForwardAxis;
            yield return null;

            // Eye based gaze cursor should still be invisible
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible, "Eye gaze cursor should be visible");

            UnityEngine.Object.Destroy(cube);
        }

        /// <summary>
        /// Ensure that the gaze provider hit result is not null when looking at an object,
        /// even when the hand is up.
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

        [UnityTest]
        public IEnumerator TestGazeProviderTargetUnityUi()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // spawn 3d object 
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.forward;
            cube.transform.localScale = Vector3.one * 0.5f;
            yield return null;

            // make sure we get the 3d object as a gaze target
            Assert.AreEqual(CoreServices.InputSystem.GazeProvider.GazeTarget, cube, "GazeProvider target is not set to 3d object (cube)");

            // spawn 2d unity ui
            var canvas = UnityUiUtilities.CreateCanvas(0.002f);
            var toggle = UnityUiUtilities.CreateToggle(Color.gray, Color.blue, Color.green);
            toggle.transform.SetParent(canvas.transform, false);
            canvas.transform.position = Vector3.forward * 0.5f;
            yield return null;

            // check if gaze target has changed to unity ui
            Assert.AreEqual(CoreServices.InputSystem.GazeProvider.GazeTarget, toggle.gameObject, "GazeProvider target is not set to toggle unity ui");

            // destroy unity ui
            Object.DestroyImmediate(canvas.gameObject);
            yield return null;

            // make sure gaze is back at 3d object
            Assert.AreEqual(CoreServices.InputSystem.GazeProvider.GazeTarget, cube, "GazeProvider target is not set to 3d object (cube)");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestGazeProviderDestroyed()
        {
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // remove the gaze provider and it's components from the scene
            GazeProvider gazeProvider = CoreServices.InputSystem.GazeProvider.GameObjectReference.GetComponent<GazeProvider>();
            gazeProvider.GazePointer.BaseCursor.Destroy();
            DebugUtilities.LogVerbose("Application was playing, destroyed the gaze pointer's BaseCursor");
            UnityObjectExtensions.DestroyObject(gazeProvider);
            gazeProvider = null;

            // Ensure that the input system and it's related input sources are able to be reinitialized without issue.
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            PlayModeTestUtilities.GetInputSystem().Initialize();

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            yield return null;
        }

        /// <summary>
        /// Ensure FocusProvider's FocusDetails can be overridden.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOverrideFocusDetails()
        {
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

            var newFocusDetails = new Physics.FocusDetails
            {
                Object = cube,
                RayDistance = 10,
                Point = new Vector3(1, 2, 3)
            };
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
            cube.AddComponent<ObjectManipulator>(); // Add focus handler so focus can lock
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
            cube.AddComponent<ObjectManipulator>(); // Add focus handler so focus can lock
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

        /// <summary>
        /// Test focus provider returns results based on custom prioritized layer mask array
        /// </summary>
        [UnityTest]
        public IEnumerator TestPrioritizedLayerMask()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward, true);

            var shellHandRayPointer = hand.GetPointer<ShellHandRayPointer>();

            string spatialAwarenessLayerName = LayerMask.LayerToName(BaseSpatialObserver.DefaultSpatialAwarenessLayer);
            shellHandRayPointer.PrioritizedLayerMasksOverride = new LayerMask[] { LayerMask.GetMask(spatialAwarenessLayerName), LayerMask.GetMask("Default") };

            var pointerDirection = shellHandRayPointer.Rotation * Vector3.forward;

            var noPriorityCube = CreateTestCube(shellHandRayPointer.Position + pointerDirection * 2.0f);
            noPriorityCube.name = "NoPriorityCube";
            noPriorityCube.layer = 18; // assign to random layer

            var lowPriorityCube = CreateTestCube(shellHandRayPointer.Position + pointerDirection * 3.0f); // uses default layer
            lowPriorityCube.name = "LowPriorityCube";

            var highPriorityCube = CreateTestCube(shellHandRayPointer.Position + pointerDirection * 4.0f);
            highPriorityCube.layer = BaseSpatialObserver.DefaultSpatialAwarenessLayer;
            highPriorityCube.name = "HighPriorityCube";

            //
            // Test High Priority cube, although farthest, is selected as raycast target
            //
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.AreEqual(highPriorityCube, shellHandRayPointer.Result?.CurrentPointerTarget, $"{highPriorityCube.name} should be raycast target by shell hand ray pointer");

            //
            // With HighPriorityCube disabled, test Low Priority cube, although farther, is selected as raycast target
            //
            highPriorityCube.SetActive(false);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.AreEqual(lowPriorityCube, shellHandRayPointer.Result?.CurrentPointerTarget, $"{lowPriorityCube.name} should be raycast target by shell hand ray pointer");

            //
            // Test noPriorityCube still not selected in raycast
            //
            lowPriorityCube.SetActive(false);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsNull(shellHandRayPointer.Result?.CurrentPointerTarget, $"{noPriorityCube.name} should NOT be raycast target by shell hand ray pointer");
        }

        /// <summary>
        /// Ensures the focus provider runs its update loop properly without a gaze provider.
        /// Also tests that a gaze provider can successfully be cleaned up at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFocusProviderWithoutGaze()
        {
            IMixedRealityInputSystem inputSystem = PlayModeTestUtilities.GetInputSystem();
            yield return null;

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return null;

            // Put up a hand to ensure there's a second pointer, which will keep the FocusProvider UpdatePointers loop spinning
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);

            // Verify that the GazeProvider exists at the start
            Assert.IsTrue(inputSystem.GazeProvider as MonoBehaviour != null, "Gaze provider should exist at start");
            yield return null;

            // Destroy the GazeProvider
            Object.Destroy(inputSystem.GazeProvider as MonoBehaviour);
            yield return null;

            // Verify that the GazeProvider no longer exists
            Assert.IsTrue(inputSystem.GazeProvider as MonoBehaviour == null, "Gaze provider should no longer exist");
            yield return null;

            // Hide the hand for other tests
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);
        }

        private static GameObject CreateTestCube(Vector3 position, float scale = 0.2f)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<ObjectManipulator>(); // Add focus handler so focus can lock
            cube.transform.position = position;
            cube.transform.localScale = Vector3.one * scale;
            return cube;
        }
    }
}
#endif
