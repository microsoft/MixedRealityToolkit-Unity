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
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static Microsoft.MixedReality.Toolkit.UI.PressableButton;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class PressableButtonTests : BasePlayModeTests, IPrebuildSetup
    {
        void IPrebuildSetup.Setup()
        {
            PlayModeTestUtilities.InstallTextMeshProEssentials();
        }

        #region Utilities

        // Tests/PlayModeTests/Prefabs/UnitTestCanvas.prefab
        private const string UnitTestCanvasPrefabGuid = "489163bd64fdf84418a1536cefbb4c80";
        private static readonly string UnitTestCanvasPrefabPath = AssetDatabase.GUIDToAssetPath(UnitTestCanvasPrefabGuid);

        // SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2UnityUI.prefab
        private const string PressableButtonHoloLens2UnityUIGuid = "2f626628bde0879488068de0e9f25f8d";
        private static readonly string PressableButtonHoloLens2UnityUIPath = AssetDatabase.GUIDToAssetPath(PressableButtonHoloLens2UnityUIGuid);

        private static readonly Dictionary<string, bool> PressableButtonTestPrefabs = new Dictionary<string, bool>
        {
            // Key is path. Value is whether or not it needs to be placed in a Canvas.
            { TestButtonUtilities.PressableHoloLens2PrefabPath, false },
            { PressableButtonHoloLens2UnityUIPath, true },
        };

        private static IEnumerable<string> PressableButtonsTestPrefabPaths
        {
            get
            {
                foreach (var prefabPath in PressableButtonTestPrefabs.Keys)
                {
                    yield return prefabPath;
                }
            }
        }

        private GameObject InstantiateDefaultPressableButton(string prefabPath)
        {
            Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            GameObject testButton = Object.Instantiate(pressableButtonPrefab) as GameObject;

            if (PressableButtonTestPrefabs[prefabPath])
            {
                // Need to place this test button in a Canvas. Instantiate the test canvas and place the button into it.
                var canvasPrefab = AssetDatabase.LoadAssetAtPath(UnitTestCanvasPrefabPath, typeof(Object));
                var canvasObject = (GameObject)Object.Instantiate(canvasPrefab);
                testButton.transform.SetParent(canvasObject.transform, worldPositionStays: false);
            }

            return testButton;
        }

        private static bool AreApproximatelyEqual(float f0, float f1, float tolerance)
        {
            return Mathf.Abs(f0 - f1) < tolerance;
        }

        /// <summary>
        /// Move the hand forward to press button, then off to the right
        /// </summary>
        private IEnumerator PressButtonWithHand()
        {
            Vector3 p1 = new Vector3(0, 0, 0.5f);
            Vector3 p2 = new Vector3(0, 0, 1.08f);
            Vector3 p3 = new Vector3(0.1f, 0, 1.08f);

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(p1);
            yield return hand.MoveTo(p2);
            yield return hand.MoveTo(p3);
        }

        #endregion

        #region Tests

        [UnityTest]
        public IEnumerator ButtonInstantiate([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            yield return null;
            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Some apps will instantiate a button, disable it while they do other setup, then enable it.  This caused a bug where the button front plate would be flattened against the button.
        /// This tests to confirm that this has not regressed.
        /// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6024
        /// </summary>
        [UnityTest]
        public IEnumerator ButtonInstantiateDisableThenEnableBeforeStart([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            // Disable then re-enable the button in the same frame as it was instantiated, so that Start() does not execute.
            testButton.SetActive(false);
            testButton.SetActive(true);

            yield return null;

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();

            var deltaPosition = GetBackPlateToFrontPlateVector(buttonComponent);

            Assert.IsTrue(deltaPosition.magnitude > 0.007f, "The button prefabs should all have their front plates at least 8mm away from the back plates.");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// There was an issue where rotating a button after Start() had executed resulted in the front plate going in the wrong direction.
        /// This tests that it has not regressed.
        /// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6025
        /// </summary>
        [UnityTest]
        public IEnumerator RotateButton([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            yield return null;

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            var initialOffset = GetBackPlateToFrontPlateVector(buttonComponent);

            // Rotate the button 90 degrees about the Y axis.
            testButton.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));

            yield return null;

            ForceInvoke_UpdateMovingVisualsPosition(buttonComponent);

            yield return null;

            var rotatedOffset = GetBackPlateToFrontPlateVector(buttonComponent);

            // Before rotating, the offset should be in the negative Z direction.  After rotating, it should be in the negative X direction.

            Assert.IsTrue(initialOffset.z < -0.007f);
            Assert.IsTrue(rotatedOffset.x < -0.007f);

            // Test that most of the magnitude of the offset is in the specified direction.  Give a large-ish tolerance.
            float tolerance = 0.00001f;
            Assert.IsTrue(AreApproximatelyEqual(initialOffset.magnitude, Mathf.Abs(initialOffset.z), tolerance));
            Assert.IsTrue(AreApproximatelyEqual(rotatedOffset.magnitude, Mathf.Abs(rotatedOffset.x), tolerance));

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// This test reproduces P0 issue 4263 which caused null pointers when pressing buttons
        /// See https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4683
        /// </summary>
        [UnityTest]
        public IEnumerator PressButtonWithHand([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            // Move the camera to origin looking at +z to more easily see the button.
            TestUtilities.PlayspaceToOriginLookingForward();

            // For some reason, we would only get null pointers when the hand tries to click a button
            // at specific positions, hence the unusual z value.
            testButton.transform.position = new Vector3(0, 0, 1.067121f);
            // The scale of the button was also unusual in the repro case
            testButton.transform.localScale = Vector3.one * 1.5f;

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            bool buttonPressed = false;
            buttonComponent.ButtonPressed.AddListener(() =>
            {
                buttonPressed = true;
            });

            // Move the hand forward to press button, then off to the right
            yield return PressButtonWithHand();

            Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");

            Object.Destroy(testButton);

            yield return null;
        }

        /// <summary>
        /// Test disabling the PressableButton GameObject and re-enabling
        /// </summary>
        [UnityTest]
        public IEnumerator DisablePressableButton([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            // Move the camera to origin looking at +z to more easily see the button.
            TestUtilities.PlayspaceToOriginLookingForward();

            // For some reason, we would only get null pointers when the hand tries to click a button
            // at specific positions, hence the unusual z value.
            testButton.transform.position = new Vector3(0, 0, 1.067121f);
            // The scale of the button was also unusual in the repro case
            testButton.transform.localScale = Vector3.one * 1.5f;

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            bool buttonPressed = false;
            buttonComponent.ButtonPressed.AddListener(() =>
            {
                buttonPressed = true;
            });

            yield return null;

            // Test pressing button with hand when disabled
            testButton.SetActive(false);
            yield return PressButtonWithHand();
            Assert.IsFalse(buttonPressed, "Button got pressed when component was disabled.");

            // Test pressing button with hand when enabled
            testButton.SetActive(true);
            yield return PressButtonWithHand();
            Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");

            Object.Destroy(testButton);

            yield return null;
        }


        /// <summary>
        /// This test reproduces P0 issue 4566 which didn't trigger a button with enabled back-press protection
        /// if hands were moving too fast in low framerate
        /// </summary>
        [UnityTest]
        public IEnumerator PressButtonFast([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            // Move the camera to origin looking at +z to more easily see the button.
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);
            Assert.IsTrue(buttonComponent.EnforceFrontPush, "Button default behavior should have enforce front push enabled");

            bool buttonPressed = false;
            buttonComponent.ButtonPressed.AddListener(() =>
            {
                buttonPressed = true;
            });

            // move the hand quickly from very far distance into the button and check if it was pressed
            TestHand hand = new TestHand(Handedness.Right);
            int numSteps = 2;
            Vector3 p1 = new Vector3(0, 0, -20.0f);
            Vector3 p2 = new Vector3(0, 0, 0.02f);

            yield return hand.Show(p1);
            yield return hand.MoveTo(p2, numSteps);

            Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");

            Object.Destroy(testButton);

            yield return null;
        }

        /// <summary>
        /// Tests if button pressed event is triggered when a second button is overlapping right behind the first button
        /// </summary>
        [UnityTest]
        public IEnumerator PressButtonWhenSecondButtonIsNearby([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton1 = InstantiateDefaultPressableButton(prefabFilename);
            GameObject testButton2 = InstantiateDefaultPressableButton(prefabFilename);

            // Move the camera to origin looking at +z to more easily see the button.
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton buttonComponent = testButton1.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);
            Assert.IsTrue(buttonComponent.EnforceFrontPush, "Button default behavior should have enforce front push enabled");

            // Positioning the start push plane of the second button just before first button max push plane, creating a small overlap
            float distance = Mathf.Abs(buttonComponent.MaxPushDistance) + Mathf.Abs(buttonComponent.StartPushDistance);
            distance = buttonComponent.DistanceSpaceMode == SpaceMode.Local ? distance * buttonComponent.LocalToWorldScale : distance;
            testButton2.transform.position += Vector3.forward * distance;

            bool buttonPressed = false;
            buttonComponent.ButtonPressed.AddListener(() =>
            {
                buttonPressed = true;
            });

            // Move the hand so the pointer passes through the two buttons 
            TestHand hand = new TestHand(Handedness.Right);
            float handReach = 0.1f;
            int numSteps = (int)Mathf.Ceil(handReach / (distance * 0.5f)); // Maximum hand speed in order to trigger touch started in the first button before the second button becomes the closest touchable to pointer

            yield return hand.Show(new Vector3(0, 0, -handReach / 2));
            yield return hand.Move(new Vector3(0, 0, handReach), numSteps);

            Assert.IsTrue(buttonPressed, "Button did not get pressed when a second button is nearby");

            Object.Destroy(testButton1);
            Object.Destroy(testButton2);

            yield return null;
        }

        /// <summary>
        /// This test verifies that buttons will trigger with far interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TriggerButtonFarInteraction([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            TestUtilities.PlayspaceToOriginLookingForward();

            Interactable interactableComponent = testButton.GetComponent<Interactable>();
            Button buttonComponent = testButton.GetComponent<Button>();

            Assert.IsTrue(interactableComponent != null || buttonComponent != null, "Depending on button type, there should be either an Interactable or a UnityUI Button on the control");

            var objectToMoveAndScale = testButton.transform;

            if (buttonComponent != null)
            {
                objectToMoveAndScale = testButton.transform.parent;
            }

            objectToMoveAndScale.position += new Vector3(0f, 0.3f, 0.8f);
            objectToMoveAndScale.localScale *= 15f; // scale button up so it's easier to hit it with the far interaction pointer
            yield return new WaitForFixedUpdate();
            yield return null;

            bool buttonTriggered = false;

            var onClickEvent = (interactableComponent != null) ? interactableComponent.OnClick : buttonComponent.onClick;

            onClickEvent.AddListener(() =>
            {
                buttonTriggered = true;
            });

            TestHand hand = new TestHand(Handedness.Right);
            Vector3 initialHandPosition = new Vector3(0.05f, -0.05f, 0.3f); // orient hand so far interaction ray will hit button
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.IsTrue(buttonTriggered, "Button did not get triggered with far interaction.");

            Object.Destroy(testButton);
            yield return null;
        }

        /// <summary>
        /// This test verifies that buttons will trigger with motion controller far interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TriggerButtonFarInteractionWithMotionController([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            TestUtilities.PlayspaceToOriginLookingForward();

            Interactable interactableComponent = testButton.GetComponent<Interactable>();
            Button buttonComponent = testButton.GetComponent<Button>();

            Assert.IsTrue(interactableComponent != null || buttonComponent != null, "Depending on button type, there should be either an Interactable or a UnityUI Button on the control");

            var objectToMoveAndScale = testButton.transform;

            if (buttonComponent != null)
            {
                objectToMoveAndScale = testButton.transform.parent;
            }

            objectToMoveAndScale.position += new Vector3(0f, 0.3f, 0.8f);
            objectToMoveAndScale.localScale *= 15f; // scale button up so it's easier to hit it with the far interaction pointer
            yield return new WaitForFixedUpdate();
            yield return null;

            bool buttonTriggered = false;

            var onClickEvent = (interactableComponent != null) ? interactableComponent.OnClick : buttonComponent.onClick;

            onClickEvent.AddListener(() =>
            {
                buttonTriggered = true;
            });

            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldHandSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;
            TestMotionController motionController = new TestMotionController(Handedness.Right);
            Vector3 initialmotionControllerPosition = new Vector3(0.05f, -0.05f, 0.3f); // orient hand so far interaction ray will hit button
            yield return motionController.Show(initialmotionControllerPosition);
            yield return motionController.Click();
            Assert.IsTrue(buttonTriggered, "Button did not get triggered with far interaction.");

            Object.Destroy(testButton);
            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldHandSimMode;
            yield return null;
        }

        [UnityTest]
        public IEnumerator ScaleWorldDistances([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            // instantiate scene and button
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            yield return null;

            PressableButton button = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(button);

            // check default value -> default must be using local space in order for the button to scale and function correctly
            Assert.IsTrue(button.DistanceSpaceMode == PressableButton.SpaceMode.Local);

            // Ensure uniform scale, non-zero start push distance, and world space distance
            testButton.transform.localScale = Vector3.one;
            button.DistanceSpaceMode = PressableButton.SpaceMode.World;
            button.StartPushDistance = 0.00003f;

            // get the buttons default values for the push planes
            float startPushDistance = button.StartPushDistance;
            float maxPushDistance = button.MaxPushDistance;
            float pressDistance = button.PressDistance;
            float releaseDistance = pressDistance - button.ReleaseDistanceDelta;

            Vector3 zeroPushDistanceWorld = button.GetWorldPositionAlongPushDirection(0.0f);

            Vector3 startPushDistanceWorld = button.GetWorldPositionAlongPushDirection(startPushDistance) - zeroPushDistanceWorld;
            Vector3 maxPushDistanceWorld = button.GetWorldPositionAlongPushDirection(maxPushDistance) - zeroPushDistanceWorld;
            Vector3 pressDistanceWorld = button.GetWorldPositionAlongPushDirection(pressDistance) - zeroPushDistanceWorld;
            Vector3 releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(releaseDistance) - zeroPushDistanceWorld;

            // scale the button in z direction
            // scaling the button while in world space shouldn't influence our button plane distances
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);

            Vector3 zeroPushDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(0.0f);

            Vector3 startPushDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(startPushDistance) - zeroPushDistanceWorldScaled;
            Vector3 maxPushDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(maxPushDistance) - zeroPushDistanceWorldScaled;
            Vector3 pressDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(pressDistance) - zeroPushDistanceWorldScaled;
            Vector3 releaseDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(releaseDistance) - zeroPushDistanceWorldScaled;

            // compare our distances
            Assert.IsTrue(startPushDistanceWorld == startPushDistanceWorldScaled, "Start Distance was modified while scaling button GameObject");
            Assert.IsTrue(maxPushDistanceWorld == maxPushDistanceWorldScaled, "Max Push Distance was modified while scaling button GameObject");
            Assert.IsTrue(pressDistanceWorld == pressDistanceWorldScaled, "Press Distance was modified while scaling button GameObject");
            Assert.IsTrue(releaseDistanceWorld == releaseDistanceWorldScaled, "Release Distance was modified while scaling button GameObject");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator SwitchWorldToLocalDistanceMode([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            // instantiate scene and button
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            yield return null;

            PressableButton button = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(button);

            // check default value -> default must be using local space in order for the button to scale and function correctly
            Assert.IsTrue(button.DistanceSpaceMode == PressableButton.SpaceMode.Local);

            // add scale to our button so we can compare world to local distances
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);
            button.StartPushDistance = 0.00003f;

            // get the buttons default values for the push planes
            float startPushDistanceLocal = button.StartPushDistance;
            float maxPushDistanceLocal = button.MaxPushDistance;
            float pressDistanceLocal = button.PressDistance;
            float releaseDistanceLocal = pressDistanceLocal - button.ReleaseDistanceDelta;

            // get world space positions for local distances
            Vector3 startPushDistanceWorldLocal = button.GetWorldPositionAlongPushDirection(startPushDistanceLocal);
            Vector3 maxPushDistanceWorldLocal = button.GetWorldPositionAlongPushDirection(maxPushDistanceLocal);
            Vector3 pressDistanceWorldLocal = button.GetWorldPositionAlongPushDirection(pressDistanceLocal);
            Vector3 releaseDistanceWorldLocal = button.GetWorldPositionAlongPushDirection(releaseDistanceLocal);

            // switch to world space
            button.DistanceSpaceMode = PressableButton.SpaceMode.World;

            float startPushDistance = button.StartPushDistance;
            float maxPushDistance = button.MaxPushDistance;
            float pressDistance = button.PressDistance;
            float releaseDistance = pressDistance - button.ReleaseDistanceDelta;

            // check if distances have changed
            Assert.IsFalse(startPushDistance == startPushDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");
            Assert.IsFalse(maxPushDistance == maxPushDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");
            Assert.IsFalse(pressDistance == pressDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");
            Assert.IsFalse(releaseDistance == releaseDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");

            // get world space positions for local distances
            Vector3 startPushDistanceWorld = button.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorld = button.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorld = button.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(releaseDistance);

            // compare world space distances -> local and world space mode should return us the same world space positions 
            Assert.IsTrue(startPushDistanceWorld == startPushDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            Assert.IsTrue(maxPushDistanceWorld == maxPushDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            Assert.IsTrue(pressDistanceWorld == pressDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            Assert.IsTrue(releaseDistanceWorld == releaseDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");

            // switch back to local space
            button.DistanceSpaceMode = PressableButton.SpaceMode.Local;

            // distances must match up with original values 
            Assert.IsTrue(startPushDistanceLocal == button.StartPushDistance, "Conversion from local to world distances didn't return the correct world distances");
            Assert.IsTrue(maxPushDistanceLocal == button.MaxPushDistance, "Conversion from local to world distances didn't return the correct world distances");
            Assert.IsTrue(pressDistanceLocal == button.PressDistance, "Conversion from local to world distances didn't return the correct world distances");
            float newReleaseDistance = button.PressDistance - button.ReleaseDistanceDelta;
            Assert.IsTrue(releaseDistanceLocal == newReleaseDistance, "Conversion from local to world distances didn't return the correct world distances");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator ScaleLocalDistances([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            // instantiate scene and button
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            yield return null;

            PressableButton button = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(button);

            // check default value -> default must be using local space in order for the button to scale and function correctly
            Assert.IsTrue(button.DistanceSpaceMode == PressableButton.SpaceMode.Local);

            // make sure there's no scale on our button and non-zero to start push distance
            testButton.transform.localScale = Vector3.one;
            button.StartPushDistance = 0.00003f;

            float zeroPushDistanceWorld = button.GetWorldPositionAlongPushDirection(0.0f).z;

            // get the buttons default values for the push planes
            float startPushDistanceWorld = button.GetWorldPositionAlongPushDirection(button.StartPushDistance).z - zeroPushDistanceWorld;
            float maxPushDistanceWorld = button.GetWorldPositionAlongPushDirection(button.MaxPushDistance).z - zeroPushDistanceWorld;
            float pressDistanceWorld = button.GetWorldPositionAlongPushDirection(button.PressDistance).z - zeroPushDistanceWorld;
            float releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(button.PressDistance - button.ReleaseDistanceDelta).z - zeroPushDistanceWorld;

            // scale the button in z direction
            // scaling the button while in local space should keep plane distance ratios maintained
            Vector3 zScale = new Vector3(1.0f, 1.0f, 3.6f);
            testButton.transform.localScale = zScale;
            yield return null;

            float zeroPushDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(0.0f).z;

            float startPushDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.StartPushDistance).z - zeroPushDistanceWorld_Scaled;
            float maxPushDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.MaxPushDistance).z - zeroPushDistanceWorld_Scaled;
            float pressDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.PressDistance).z - zeroPushDistanceWorld_Scaled;
            float releaseDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.PressDistance - button.ReleaseDistanceDelta).z - zeroPushDistanceWorld_Scaled;

            float tolerance = 0.00000001f;

            Assert.IsTrue(AreApproximatelyEqual(startPushDistanceWorld * zScale.z, startPushDistanceWorld_Scaled, tolerance), "Start push distance plane did not scale correctly");
            Assert.IsTrue(AreApproximatelyEqual(maxPushDistanceWorld * zScale.z, maxPushDistanceWorld_Scaled, tolerance), "Max push distance plane did not scale correctly");
            Assert.IsTrue(AreApproximatelyEqual(pressDistanceWorld * zScale.z, pressDistanceWorld_Scaled, tolerance), "Press distance plane did not scale correctly");
            Assert.IsTrue(AreApproximatelyEqual(releaseDistanceWorld * zScale.z, releaseDistanceWorld_Scaled, tolerance), "Release distance plane did not scale correctly");
            Object.Destroy(testButton);
            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestParentZeroScale([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            // instantiate scene and button
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            PressableButton button = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(button);

            // check default value -> default must be using local space in order for the button to scale and function correctly
            Assert.IsTrue(button.DistanceSpaceMode == PressableButton.SpaceMode.Local);

            // make sure there's no scale on our button and non-zero to start push distance
            testButton.transform.localScale = Vector3.one;
            button.StartPushDistance = 0.00003f;

            // Create an empty GameObject for our parent.
            GameObject emptyParent = new GameObject();
            emptyParent.transform.position = testButton.transform.position;
            // This should not cause any NaN errors, as per resolution to #7874
            emptyParent.transform.localScale = Vector3.zero;
            // Parent our button to the empty object.
            testButton.transform.SetParent(emptyParent.transform, false);
            yield return null;

            // Scale up the parent. Should not throw NaN exceptions.
            emptyParent.transform.localScale = Vector3.one;
            yield return null;

            Object.Destroy(testButton);
            Object.Destroy(emptyParent);
            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// This tests the release behavior of a button
        /// </summary>
        [UnityTest]
        public IEnumerator ReleaseButton([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            bool buttonPressed = false;
            buttonComponent.ButtonPressed.AddListener(() =>
            {
                buttonPressed = true;
            });

            bool buttonReleased = false;
            buttonComponent.ButtonReleased.AddListener(() =>
            {
                buttonReleased = true;
            });

            Vector3 startHand = new Vector3(0, 0, -0.0081f);
            Vector3 inButtonOnPress = new Vector3(0, 0, 0.002f); // past press plane of mrtk pressablebutton prefab
            Vector3 rightOfButtonPress = new Vector3(0.02f, 0, 0.002f); // right of press plane, outside button
            Vector3 inButtonOnRelease = new Vector3(0, 0, -0.0015f); // release plane of mrtk pressablebutton prefab
            TestHand hand = new TestHand(Handedness.Right);

            // test scenarios in normal and low framerate
            int[] stepVariations = { 30, 2 };
            for (int i = 0; i < stepVariations.Length; ++i)
            {
                int numSteps = stepVariations[i];

                // test release
                yield return hand.Show(startHand);
                yield return hand.MoveTo(inButtonOnPress, numSteps);
                yield return hand.MoveTo(inButtonOnRelease, numSteps);
                yield return hand.Hide();

                Assert.IsTrue(buttonPressed, $"A{i} - Button did not get pressed when hand moved to press it.");
                Assert.IsTrue(buttonReleased, $"A{i} - Button did not get released.");

                buttonPressed = false;
                buttonReleased = false;

                Assert.IsTrue(buttonComponent.ReleaseOnTouchEnd == true, "default behavior of button should be release on touch end");

                // test release on moving outside of button 
                yield return hand.Show(startHand);
                yield return hand.MoveTo(inButtonOnPress, numSteps);
                yield return hand.MoveTo(rightOfButtonPress, numSteps);
                yield return hand.Hide();

                Assert.IsTrue(buttonPressed, $"B{i} - Button did not get pressed when hand moved to press it.");
                Assert.IsTrue(buttonReleased, $"B{i} - Button did not get released when hand exited the button.");

                buttonPressed = false;
                buttonReleased = false;

                buttonComponent.ReleaseOnTouchEnd = false;

                // test no release on moving outside of button when releaseOnTouchEnd is disabled
                yield return hand.Show(startHand);
                yield return hand.MoveTo(inButtonOnPress, numSteps);
                yield return hand.MoveTo(rightOfButtonPress, numSteps);
                yield return hand.Hide();

                Assert.IsTrue(buttonPressed, $"C{i} - Button did not get pressed when hand moved to press it.");
                Assert.IsFalse(buttonReleased, $"C{i} - Button did got released on exit even though releaseOnTouchEnd wasn't set");

                buttonPressed = false;
                buttonReleased = false;

                buttonComponent.ReleaseOnTouchEnd = true;
            }

            Object.Destroy(testButton);

            yield return null;
        }

        private static Vector3 GetBackPlateToFrontPlateVector(PressableButton button)
        {
            var movingButtonVisualsTransform = GetPrivateMovingButtonVisuals(button).transform;
            var backPlateTransform = button.transform.Find("BackPlate");

            return movingButtonVisualsTransform.position - backPlateTransform.position;
        }

        private static GameObject GetPrivateMovingButtonVisuals(PressableButton button)
        {
            // Use reflection to get the private field that contains the front plate.
            var movingButtonVisualsField = typeof(PressableButton).GetField("movingButtonVisuals", BindingFlags.NonPublic | BindingFlags.Instance);
            return (GameObject)movingButtonVisualsField.GetValue(button);
        }

        private static void ForceInvoke_UpdateMovingVisualsPosition(PressableButton button)
        {
            // Use reflection to invoke a non-public method.
            var method = typeof(PressableButton).GetMethod("UpdateMovingVisualsPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(button, System.Array.Empty<object>());
        }

        /// <summary>
        /// Tests if Interactable is internally disabled, then the PhysicalPressEventRouter
        /// should not invoke events in Interactable.  Addresses issue 5833.
        /// </summary>
        [UnityTest]
        public IEnumerator CheckPhysicalPressableRouterEventsOnDisableInteractable()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            GameObject testButton = InstantiateDefaultPressableButton(TestButtonUtilities.PressableHoloLens2PrefabPath);
            testButton.transform.position = new Vector3(0, 0, 1);
            testButton.transform.localScale = Vector3.one * 1.5f;

            Interactable interactable = testButton.GetComponent<Interactable>();
            Assert.IsNotNull(interactable);

            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            yield return PressButtonWithHand();

            Assert.True(wasClicked, "Interactable is enabled, and should receive click events");
            wasClicked = false;

            // Disable Interactable, should not receive click events
            interactable.IsEnabled = false;

            yield return PressButtonWithHand();

            // Check if events were raised if interactable is not enabled
            Assert.False(wasClicked, "Interactable is disabled, we should not receive a click event");

            GameObject.Destroy(testButton);
        }

        #endregion
    }
}

#endif
