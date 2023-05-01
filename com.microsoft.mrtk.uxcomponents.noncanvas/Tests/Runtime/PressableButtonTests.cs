// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;
using SpaceMode = Microsoft.MixedReality.Toolkit.UX.PressableButton.SpaceMode;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    public class PressableButtonTests : BaseRuntimeInputTests
    {
        // UXComponents/Buttons/Prefabs/PressableButton_32x32mm_IconAndText
        private const string PressableButtonGuid = "c25d8f2d22117bd40ad824684551fbde";
        private static readonly string PressableButtonPath = AssetDatabase.GUIDToAssetPath(PressableButtonGuid);

        private static readonly Dictionary<string, bool> PressableButtonTestPrefabs = new Dictionary<string, bool>
        {
            // Key is path. Value is whether or not it needs to be placed in a Canvas.
            { PressableButtonPath, false },
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

            // We don't get inheritdoc summaries here for some reason?
            var selectedness = buttonComponent.Selectedness();

            Assert.IsTrue(selectedness == 0.0f, "The button prefabs should start with 0 selectedness");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// This test reproduces P0 issue 4263 which caused null pointers when pressing buttons with poke interactors
        /// See https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4683
        /// </summary>
        [UnityTest]
        public IEnumerator PokeButtonTest([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            // Move the camera to origin looking at +z to more easily see the button.
            InputTestUtilities.InitializeCameraToOriginAndForward();

            testButton.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            bool buttonPressed = false;
            buttonComponent.selectEntered.AddListener((args) => { buttonPressed = true; });

            // Move the hand forward to press button, then off to the right
            yield return PressAndReleaseButtonWithHand(testButton.transform.position);

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

            testButton.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            bool buttonPressed = false;
            buttonComponent.selectEntered.AddListener((args) => { buttonPressed = true; });

            yield return null;

            // Test pressing button with hand when disabled
            testButton.SetActive(false);
            yield return PressAndReleaseButtonWithHand(testButton.transform.position);
            Assert.IsFalse(buttonPressed, "Button got pressed when component was disabled.");

            // Test pressing button with hand when enabled
            testButton.SetActive(true);
            yield return PressAndReleaseButtonWithHand(testButton.transform.position);
            Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");

            Object.Destroy(testButton);

            yield return null;
        }

        /// <summary>
        /// Test whether the button un-presses itself when disabled.
        /// </summary>
        [UnityTest]
        public IEnumerator DisableOnPress([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            testButton.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsNotNull(buttonComponent);

            buttonComponent.OnClicked.AddListener(() => { buttonComponent.enabled = false; });

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Press *through* the button
            yield return PressAndReleaseButtonWithHand(testButton.transform.position + Vector3.forward * 0.1f);

            Assert.IsFalse(buttonComponent.enabled, "Button did not get disabled on press");
            Assert.IsTrue(Mathf.Approximately(buttonComponent.Selectedness(), 0.0f), "Button did not un-press when disabled");

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

            testButton1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));
            testButton2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));

            // Move the camera to origin looking at +z to more easily see the button.
            InputTestUtilities.InitializeCameraToOriginAndForward();

            PressableButton buttonComponent = testButton1.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);
            Assert.IsTrue(buttonComponent.EnforceFrontPush, "Button default behavior should have enforce front push enabled");

            // Positioning the start push plane of the second button just before first button max push plane, creating a small overlap
            float distance = Mathf.Abs(buttonComponent.EndPushPlane) + Mathf.Abs(buttonComponent.StartPushPlane);
            distance = buttonComponent.DistanceSpaceMode == SpaceMode.Local ? distance * buttonComponent.LocalToWorldScale : distance;
            testButton2.transform.position += Vector3.forward * distance;

            bool buttonPressed = false;
            buttonComponent.selectEntered.AddListener((args) => { buttonPressed = true; });

            // Move the hand so the pointer passes through the two buttons
            yield return PressAndReleaseButtonWithHand(testButton2.transform.position);

            Assert.IsTrue(buttonPressed, "Button did not get pressed when a second button is nearby");

            Object.Destroy(testButton1);
            Object.Destroy(testButton2);

            yield return null;
        }


        /// <summary>
        /// Presses the button extremely quickly (to the point where there is no single frame where
        /// the finger actually overlaps the button!) This tests whether our poke interpolation
        /// heuristics are working correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator PressButtonFast([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            testButton.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);
            Assert.IsTrue(buttonComponent.EnforceFrontPush, "Button default behavior should have enforce front push enabled");

            bool buttonPressed = false;
            buttonComponent.selectEntered.AddListener((args) => { buttonPressed = true; });

            TestHand hand = new TestHand(Handedness.Right);

            // We have to start the finger near-ish the button, or else
            // the poke interactor won't even be enabled "in time". TODO: consider
            // possibly making poke interactions always enabled.
            Vector3 p1 = new Vector3(0, 0, -0.1f);
            Vector3 p2 = new Vector3(0, 0, 1f);

            yield return hand.Show(testButton.transform.position + p1);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(!buttonPressed, "Button should not be pressed yet");

            // Ony two steps! Very fast.
            yield return hand.MoveTo(testButton.transform.position + p2, 2);

            Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");

            Object.Destroy(testButton);

            yield return null;
        }

        /* TODO: A buttons visuals test which should be put in it's own category
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
            Debug.Log(initialOffset);

            // Press the button t
            yield return PressButtonWithHand(testButton.transform.position);

            // Rotate the button 90 degrees about the Y axis.
            testButton.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));

            var rotatedOffset = GetBackPlateToFrontPlateVector(buttonComponent);

            // Before rotating, the offset should be in the negative Z direction.  After rotating, it should be in the negative X direction.

            Assert.IsTrue(initialOffset.z < -0.007f);
            Assert.IsTrue(rotatedOffset.x < -0.007f);

            // Test that most of the magnitude of the offset is in the specified direction.  Give a large-ish tolerance.
            float tolerance = 0.00001f;

            Assert.AreEqual(initialOffset.magnitude, Mathf.Abs(initialOffset.z), tolerance);
            Assert.AreEqual(rotatedOffset.magnitude, Mathf.Abs(rotatedOffset.z), tolerance);

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }
        */

        /// <summary>
        /// This test verifies that buttons will trigger with far interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TriggerButtonFarInteraction([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            var objectToMoveAndScale = testButton.transform;

            objectToMoveAndScale.position = InputTestUtilities.InFrontOfUser(new Vector3(0f, 0.0f, 0.8f));
            objectToMoveAndScale.localScale *= 15f; // scale button up so it's easier to hit it with the far interaction pointer
            yield return RuntimeTestUtilities.WaitForUpdates();

            bool buttonTriggered = false;

            var onClickEvent = buttonComponent.OnClicked;

            onClickEvent.AddListener(() =>
            {
                buttonTriggered = true;
            });

            TestHand hand = new TestHand(Handedness.Right);
            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.05f, 0.3f)); // orient hand so far interaction ray will hit button
            yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(buttonTriggered, "Button did not get triggered with far interaction.");

            Object.Destroy(testButton);
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// This tests the release behavior of a button
        /// </summary>
        [UnityTest]
        public IEnumerator ReleaseButton([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            testButton.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            bool buttonPressed = false;
            buttonComponent.selectEntered.AddListener((_) => { buttonPressed = true; });

            bool buttonReleased = false;
            buttonComponent.selectExited.AddListener((_) => { buttonReleased = true; });

            bool buttonClicked = false;
            buttonComponent.OnClicked.AddListener(() => { buttonClicked = true; });

            // test release
            yield return PressAndReleaseButtonWithHand(testButton.transform.position);

            Assert.IsTrue(buttonPressed, $"Button did not get pressed when hand moved to press it.");
            Assert.IsTrue(buttonReleased, $"Button did not get released.");
            Assert.IsTrue(buttonClicked, $"Button did not get clicked.");

            buttonPressed = false;
            buttonReleased = false;
            buttonClicked = false;

            // test XY rolloff
            Assert.IsTrue(buttonComponent.RejectXYRolloff, "Default behavior of button should be to reject XY rolloff");
            Assert.IsFalse(buttonComponent.RejectZRolloff, "Default behavior of button should be to  *not* reject Z rolloff");

            yield return PressButtonWithHand(testButton.transform.position);
            yield return ReleaseButtonWithHand(testButton.transform.position, doRolloff: true);

            Assert.IsTrue(buttonPressed, $"Button did not get pressed when hand moved to press it.");
            Assert.IsTrue(buttonReleased, $"Button did not get released when hand exited the button.");
            Assert.IsFalse(buttonClicked, $"Button should not have fired OnClicked when the finger rolls off the edge.");

            buttonPressed = false;
            buttonReleased = false;
            buttonClicked = false;

            // test XY rolloff rejection disabled
            buttonComponent.RejectXYRolloff = false;

            yield return PressButtonWithHand(testButton.transform.position);
            yield return ReleaseButtonWithHand(testButton.transform.position, doRolloff: true);

            Assert.IsTrue(buttonPressed, $"Button did not get pressed when hand moved to press it.");
            Assert.IsTrue(buttonReleased, $"Button did not get released when hand exited the button.");
            Assert.IsTrue(buttonClicked, $"OnClicked should have been fired despite rolloff, because RejectXYRolloff = false.");

            buttonPressed = false;
            buttonReleased = false;
            buttonClicked = false;

            // test Z rolloff rejection = true;
            buttonComponent.RejectZRolloff = true;

            // Press *through* the button
            yield return PressAndReleaseButtonWithHand(testButton.transform.position + Vector3.forward * 0.1f);

            Assert.IsTrue(buttonPressed, $"Button did not get pressed when hand moved to press it.");
            Assert.IsTrue(buttonReleased, $"Button did not get released when hand exited the button.");
            Assert.IsFalse(buttonClicked, $"Button should not have fired OnClicked when the finger goes all the way through the button w/ RejectZRolloff = true.");

            buttonPressed = false;
            buttonReleased = false;
            buttonClicked = false;

            // test Z rolloff rejection = true;
            buttonComponent.RejectZRolloff = false;

            // Press *through* the button
            yield return PressAndReleaseButtonWithHand(testButton.transform.position + Vector3.forward * 0.1f);

            Assert.IsTrue(buttonPressed, $"Button did not get pressed when hand moved to press it.");
            Assert.IsTrue(buttonReleased, $"Button did not get released when hand exited the button.");
            Assert.IsTrue(buttonClicked, $"Button should have fired OnClicked when the finger goes all the way through the button w/ RejectZRolloff = false.");

            Object.Destroy(testButton);

            yield return null;
        }


        /// <summary>
        /// This tests the toggle behavior of a button
        /// </summary>
        [UnityTest]
        public IEnumerator ToggleButton([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);
            testButton.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1));

            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            // Set up button to be toggleable.
            buttonComponent.ToggleMode = StatefulInteractable.ToggleType.Toggle;

            // test toggle is false on init
            Assert.IsFalse(buttonComponent.IsToggled);

            // tests force toggle
            buttonComponent.ForceSetToggled(true);
            Assert.IsTrue(buttonComponent.IsToggled);
            buttonComponent.ForceSetToggled(false);
            Assert.IsFalse(buttonComponent.IsToggled);
            buttonComponent.ForceSetToggled(true);
            Assert.IsTrue(buttonComponent.IsToggled);

            // tests toggle after press
            buttonComponent.TriggerOnRelease = false;

            yield return PressButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(buttonComponent.IsToggled);

            yield return ReleaseButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(buttonComponent.IsToggled);

            yield return PressButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(buttonComponent.IsToggled);

            yield return ReleaseButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(buttonComponent.IsToggled);



            // tests toggle only after release
            buttonComponent.TriggerOnRelease = true;

            yield return PressButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(buttonComponent.IsToggled);

            yield return ReleaseButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(buttonComponent.IsToggled);

            yield return PressButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(buttonComponent.IsToggled);

            yield return ReleaseButtonWithHand(testButton.transform.position);

            // Wait a moment to allow the button's selectedness to lerp to the correct values
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(buttonComponent.IsToggled);


            // Cleanup
            Object.Destroy(testButton);

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
            button.StartPushPlane = 0.00003f;

            // get the buttons default values for the push planes
            float StartPushPlane = button.StartPushPlane;
            float EndPushPlane = button.EndPushPlane;
            // float pressDistance = button.PressDistance;
            // float releaseDistance = pressDistance - button.ReleaseDistanceDelta;

            Vector3 zeroPushDistanceWorld = button.GetWorldPositionAlongPushDirection(0.0f);

            Vector3 StartPushPlaneWorld = button.GetWorldPositionAlongPushDirection(StartPushPlane) - zeroPushDistanceWorld;
            Vector3 EndPushPlaneWorld = button.GetWorldPositionAlongPushDirection(EndPushPlane) - zeroPushDistanceWorld;
            // Vector3 pressDistanceWorld = button.GetWorldPositionAlongPushDirection(pressDistance) - zeroPushDistanceWorld;
            // Vector3 releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(releaseDistance) - zeroPushDistanceWorld;

            // scale the button in z direction
            // scaling the button while in world space shouldn't influence our button plane distances
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);

            Vector3 zeroPushDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(0.0f);

            Vector3 StartPushPlaneWorldScaled = button.GetWorldPositionAlongPushDirection(StartPushPlane) - zeroPushDistanceWorldScaled;
            Vector3 EndPushPlaneWorldScaled = button.GetWorldPositionAlongPushDirection(EndPushPlane) - zeroPushDistanceWorldScaled;
            // Vector3 pressDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(pressDistance) - zeroPushDistanceWorldScaled;
            // Vector3 releaseDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(releaseDistance) - zeroPushDistanceWorldScaled;

            // compare our distances
            Assert.IsTrue(StartPushPlaneWorld == StartPushPlaneWorldScaled, "Start Distance was modified while scaling button GameObject");
            Assert.IsTrue(EndPushPlaneWorld == EndPushPlaneWorldScaled, "Max Push Distance was modified while scaling button GameObject");
            // Assert.IsTrue(pressDistanceWorld == pressDistanceWorldScaled, "Press Distance was modified while scaling button GameObject");
            // Assert.IsTrue(releaseDistanceWorld == releaseDistanceWorldScaled, "Release Distance was modified while scaling button GameObject");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        // TODO: Consider adding PressDistance and ReleaseDistanceDelta configurations back to the PressableButton
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
            button.StartPushPlane = 0.00003f;

            // get the buttons default values for the push planes
            float StartPushPlaneLocal = button.StartPushPlane;
            float EndPushPlaneLocal = button.EndPushPlane;
            // float pressDistanceLocal = button.PressDistance;
            // float releaseDistanceLocal = pressDistanceLocal - button.ReleaseDistanceDelta;

            // get world space positions for local distances
            Vector3 StartPushPlaneWorldLocal = button.GetWorldPositionAlongPushDirection(StartPushPlaneLocal);
            Vector3 EndPushPlaneWorldLocal = button.GetWorldPositionAlongPushDirection(EndPushPlaneLocal);
            // Vector3 pressDistanceWorldLocal = button.GetWorldPositionAlongPushDirection(pressDistanceLocal);
            // Vector3 releaseDistanceWorldLocal = button.GetWorldPositionAlongPushDirection(releaseDistanceLocal);

            // switch to world space
            button.DistanceSpaceMode = PressableButton.SpaceMode.World;

            float StartPushPlane = button.StartPushPlane;
            float EndPushPlane = button.EndPushPlane;
            // float pressDistance = button.PressDistance;
            // float releaseDistance = pressDistance - button.ReleaseDistanceDelta;

            // check if distances have changed
            Assert.IsFalse(StartPushPlane == StartPushPlaneLocal, "Switching from world to local space distances didn't adjust the plane coords");
            Assert.IsFalse(EndPushPlane == EndPushPlaneLocal, "Switching from world to local space distances didn't adjust the plane coords");
            // Assert.IsFalse(pressDistance == pressDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");
            // Assert.IsFalse(releaseDistance == releaseDistanceLocal, "Switching from world to local space distances didn't adjust the plane coords");

            // get world space positions for local distances
            Vector3 StartPushPlaneWorld = button.GetWorldPositionAlongPushDirection(StartPushPlane);
            Vector3 EndPushPlaneWorld = button.GetWorldPositionAlongPushDirection(EndPushPlane);
            // Vector3 pressDistanceWorld = button.GetWorldPositionAlongPushDirection(pressDistance);
            // Vector3 releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(releaseDistance);

            // compare world space distances -> local and world space mode should return us the same world space positions
            Assert.IsTrue(StartPushPlaneWorld == StartPushPlaneWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            Assert.IsTrue(EndPushPlaneWorld == EndPushPlaneWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            // Assert.IsTrue(pressDistanceWorld == pressDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");
            // Assert.IsTrue(releaseDistanceWorld == releaseDistanceWorldLocal, "World and Local World positions don't match after switching pressable button distance mode");

            // switch back to local space
            button.DistanceSpaceMode = PressableButton.SpaceMode.Local;

            // distances must match up with original values
            Assert.IsTrue(StartPushPlaneLocal == button.StartPushPlane, "Conversion from local to world distances didn't return the correct world distances");
            Assert.IsTrue(EndPushPlaneLocal == button.EndPushPlane, "Conversion from local to world distances didn't return the correct world distances");
            // Assert.IsTrue(pressDistanceLocal == button.PressDistance, "Conversion from local to world distances didn't return the correct world distances");
            // float newReleaseDistance = button.PressDistance - button.ReleaseDistanceDelta;
            // Assert.IsTrue(releaseDistanceLocal == newReleaseDistance, "Conversion from local to world distances didn't return the correct world distances");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]

        // TODO: Consider adding PressDistance and ReleaseDistanceDelta configurations back to the PressableButton
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
            button.StartPushPlane = 0.00003f;

            float zeroPushDistanceWorld = button.GetWorldPositionAlongPushDirection(0.0f).z;

            // get the buttons default values for the push planes
            float StartPushPlaneWorld = button.GetWorldPositionAlongPushDirection(button.StartPushPlane).z - zeroPushDistanceWorld;
            float EndPushPlaneWorld = button.GetWorldPositionAlongPushDirection(button.EndPushPlane).z - zeroPushDistanceWorld;
            // float pressDistanceWorld = button.GetWorldPositionAlongPushDirection(button.PressDistance).z - zeroPushDistanceWorld;
            // float releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(button.PressDistance - button.ReleaseDistanceDelta).z - zeroPushDistanceWorld;

            // scale the button in z direction
            // scaling the button while in local space should keep plane distance ratios maintained
            Vector3 zScale = new Vector3(1.0f, 1.0f, 3.6f);
            testButton.transform.localScale = zScale;
            yield return null;

            float zeroPushDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(0.0f).z;

            float StartPushPlaneWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.StartPushPlane).z - zeroPushDistanceWorld_Scaled;
            float EndPushPlaneWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.EndPushPlane).z - zeroPushDistanceWorld_Scaled;
            // float pressDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.PressDistance).z - zeroPushDistanceWorld_Scaled;
            // float releaseDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.PressDistance - button.ReleaseDistanceDelta).z - zeroPushDistanceWorld_Scaled;

            float tolerance = 0.00000001f;

            Assert.AreEqual(StartPushPlaneWorld * zScale.z, StartPushPlaneWorld_Scaled, tolerance, "Start push distance plane did not scale correctly");
            Assert.AreEqual(EndPushPlaneWorld * zScale.z, EndPushPlaneWorld_Scaled, tolerance, "Max push distance plane did not scale correctly");
            // Assert.AreEqual(pressDistanceWorld * zScale.z, pressDistanceWorld_Scaled, tolerance, "Press distance plane did not scale correctly");
            // Assert.AreEqual(releaseDistanceWorld * zScale.z, releaseDistanceWorld_Scaled, tolerance), "Release distance plane did not scale correctly");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return null;
        }

        [UnityTest]

        // TODO: Consider adding PressDistance and ReleaseDistanceDelta configurations back to the PressableButton
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
            button.StartPushPlane = 0.00003f;

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

        /* TODO Introduce variants of tests which check that the buttons get pressed in low framerate vs high fr

        /* TODO: other tests which are not yet testable due to the current state of the button's features
        /// <summary>
        /// This test verifies that buttons will trigger with motion controller far interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TriggerButtonFarInteractionWithMotionController([ValueSource(nameof(PressableButtonsTestPrefabPaths))] string prefabFilename)
        {
            GameObject testButton = InstantiateDefaultPressableButton(prefabFilename);

            InputTestUtilities.InitializeCameraToOriginAndForward();

            Button buttonComponent = testButton.GetComponent<PressableButton>();

            Assert.IsTrue(buttonComponent != null, "Depending on button type, there should be either an Interactable or a UnityUI Button on the control");

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

        */
        #endregion Tests

        #region Private methods

        // TODO: Create tests and harnesses in general for buttons what are aligned with canvas elements
        private GameObject InstantiateDefaultPressableButton(string prefabPath)
        {
            Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            GameObject testButton = Object.Instantiate(pressableButtonPrefab) as GameObject;

            return testButton;
        }

        /// <summary>
        /// Move the hand forward to press button, then off to the right to release
        /// </summary>
        private IEnumerator PressAndReleaseButtonWithHand(Vector3 buttonPosition)
        {
            yield return PressButtonWithHand(buttonPosition);
            yield return ReleaseButtonWithHand(buttonPosition);
        }

        /// <summary>
        /// Move the hand forward to press button
        /// </summary>
        private IEnumerator PressButtonWithHand(Vector3 buttonPosition)
        {
            // Start the hand a good distance behind the button
            Vector3 p1 = new Vector3(0, 0, -0.2f);

            // Bring the hand so it fully depresses the button
            // This depth value is specifically coded for the size of the button prefab for these tests
            Vector3 p2 = new Vector3(0, 0, 0.013f);

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.MoveTo(buttonPosition + p1);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Slowly move the hand to touch the button
            yield return hand.MoveTo(buttonPosition + p2, 15);
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// Move the hand off to the right to release the button
        /// </summary>
        private IEnumerator ReleaseButtonWithHand(Vector3 buttonPosition, bool doRolloff = false)
        {
            Vector3 p3 = new Vector3(doRolloff ? 0.1f : 0.0f, 0, -0.05f);

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.MoveTo(buttonPosition + p3);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        #endregion Private methods
    }
}
