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

using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class PressableButtonTests : IPrebuildSetup
    {
        public void Setup()
        {
            PlayModeTestUtilities.EnsureTextMeshProEssentials();
        }

        #region Utilities
        private GameObject InstantiateDefaultPressableButton()
        {
            Object pressableButtonPrefab = AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2.prefab", typeof(Object));
            GameObject testButton = Object.Instantiate(pressableButtonPrefab) as GameObject;

            return testButton;
        }

        [SetUp]
        public void TestSetup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        #endregion

        #region Tests

        [UnityTest]
        public IEnumerator ButtonInstantiate()
        {
            GameObject testButton = InstantiateDefaultPressableButton();
            yield return null;
            PressableButton buttonComponent = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(buttonComponent);

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// This test reproduces P0 issue 4263 which caused null pointers when pressing buttons
        /// See https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4683
        /// </summary>
        [UnityTest]
        public IEnumerator PressButtonWithHand()
        {
            GameObject testButton = InstantiateDefaultPressableButton();

            // Move the camera to origin looking at +z to more easily see the button.
            TestUtilities.PlayspaceToOriginLookingForward();

            // For some reason, we would only get null pointers when the hand tries to click a button
            // at specific positions, hence the unusal z value.
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
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 30;
            Vector3 p1 = new Vector3(0, 0, 0.5f);
            Vector3 p2 = new Vector3(0, 0, 1.08f);
            Vector3 p3 = new Vector3(0.1f, 0, 1.08f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");

            Object.Destroy(testButton);

            yield return null;
        }

        /// <summary>
        /// This test reproduces P0 issue 4566 which didn't trigger a button with enabled backpressprotection 
        /// if hands were moving too fast in low framerate
        /// </summary>
        [UnityTest]
        public IEnumerator PressButtonFast()
        {
            GameObject testButton = InstantiateDefaultPressableButton();

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
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 2;
            Vector3 p1 = new Vector3(0, 0, -20.0f);
            Vector3 p2 = new Vector3(0, 0, 0.02f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");

            Object.Destroy(testButton);

            yield return null;
        }


        /// <summary>
        /// This test verifies that buttons will trigger with far interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TriggerButtonFarInteraction()
        {
            GameObject testButton = InstantiateDefaultPressableButton();

            TestUtilities.PlayspaceToOriginLookingForward();

            testButton.transform.position = new Vector3(0f, 0.3f, 0.8f);
            testButton.transform.localScale = Vector3.one * 15f; // scale button up so it's easier to hit it with the far interaction pointer
            yield return new WaitForFixedUpdate();
            yield return null;

            bool buttonTriggered = false;
            Interactable interactableComponent = testButton.GetComponent<Interactable>();
            Assert.IsNotNull(interactableComponent);
            interactableComponent.OnClick.AddListener(() =>
            {
                buttonTriggered = true;
            });

            TestHand hand = new TestHand(Handedness.Right);
            Vector3 initialHandPosition = new Vector3(0.0f, 0f, 0.3f); // orient hand so far interaction ray will hit button
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.IsTrue(buttonTriggered, "Button did not get triggered with far interaction.");

            Object.Destroy(testButton);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ScaleWorldDistances()
        {
            // instantiate scene and button
            GameObject testButton = InstantiateDefaultPressableButton();
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

            Vector3 startPushDistanceWorld = button.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorld = button.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorld = button.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(releaseDistance);

            // scale the button in z direction
            // scaling the button while in world space shouldn't influence our button plane distances
            testButton.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);

            Vector3 startPushDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(startPushDistance);
            Vector3 maxPushDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(maxPushDistance);
            Vector3 pressDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(pressDistance);
            Vector3 releaseDistanceWorldScaled = button.GetWorldPositionAlongPushDirection(releaseDistance);

            // compare our distances
            Assert.IsTrue(startPushDistanceWorld == startPushDistanceWorldScaled, "Start Distance was modified while scaling button gameobject");
            Assert.IsTrue(maxPushDistanceWorld == maxPushDistanceWorldScaled, "Max Push Distance was modified while scaling button gameobject");
            Assert.IsTrue(pressDistanceWorld == pressDistanceWorldScaled, "Press Distance was modified while scaling button gameobject");
            Assert.IsTrue(releaseDistanceWorld == releaseDistanceWorldScaled, "Release Distance was modified while scaling button gameobject");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator SwitchWorldToLocalDistanceMode()
        {
            // instantiate scene and button
            GameObject testButton = InstantiateDefaultPressableButton();
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
        public IEnumerator ScaleLocalDistances()
        {
            // instantiate scene and button
            GameObject testButton = InstantiateDefaultPressableButton();
            yield return null;

            PressableButton button = testButton.GetComponent<PressableButton>();
            Assert.IsNotNull(button);

            // check default value -> default must be using local space in order for the button to scale and function correctly
            Assert.IsTrue(button.DistanceSpaceMode == PressableButton.SpaceMode.Local);

            // make sure there's no scale on our button and non-zero to start push distance
            testButton.transform.localScale = Vector3.one;
            button.StartPushDistance = 0.00003f;

            // get the buttons default values for the push planes
            float startPushDistanceWorld = button.GetWorldPositionAlongPushDirection(button.StartPushDistance).z;
            float maxPushDistanceWorld = button.GetWorldPositionAlongPushDirection(button.MaxPushDistance).z;
            float pressDistanceWorld = button.GetWorldPositionAlongPushDirection(button.PressDistance).z;
            float releaseDistanceWorld = button.GetWorldPositionAlongPushDirection(button.PressDistance - button.ReleaseDistanceDelta).z;

            // scale the button in z direction
            // scaling the button while in local space should keep plane distance ratios mantained
            Vector3 zScale = new Vector3(1.0f, 1.0f, 3.6f);
            testButton.transform.localScale = zScale;
            yield return null;

            float startPushDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.StartPushDistance).z;
            float maxPushDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.MaxPushDistance).z;
            float pressDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.PressDistance).z;
            float releaseDistanceWorld_Scaled = button.GetWorldPositionAlongPushDirection(button.PressDistance - button.ReleaseDistanceDelta).z;

            Assert.IsTrue(Mathf.Approximately(startPushDistanceWorld * zScale.z, startPushDistanceWorld_Scaled), "Start push distance plane did not scale correctly");
            Assert.IsTrue(Mathf.Approximately(maxPushDistanceWorld * zScale.z, maxPushDistanceWorld_Scaled), "Max push distance plane did not scale correctly");
            Assert.IsTrue(Mathf.Approximately(pressDistanceWorld * zScale.z, pressDistanceWorld_Scaled), "Press distance plane did not scale correctly");
            Assert.IsTrue(Mathf.Approximately(releaseDistanceWorld * zScale.z, releaseDistanceWorld_Scaled), "Release distance plane did not scale correctly");

            Object.Destroy(testButton);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// This tests the release behavior of a button
        /// </summary>
        [UnityTest]
        public IEnumerator ReleaseButton()
        {
            GameObject testButton = InstantiateDefaultPressableButton();
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

            Vector3 startHand = new Vector3(0, 0, -0.008f);
            Vector3 inButtonOnPress = new Vector3(0, 0, 0.002f); // past press plane of mrtk pressablebutton prefab
            Vector3 rightOfButtonPress = new Vector3(1.0f, 0, 0.002f); // right of press plane, outside button
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
                
                Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");
                Assert.IsTrue(buttonReleased, "Button did not get released.");

                buttonPressed = false;
                buttonReleased = false;

                Assert.IsTrue(buttonComponent.ReleaseOnTouchEnd == true, "default behavior of button should be release on touch end");

                // test release on moving outside of button 
                yield return hand.Show(startHand);
                yield return hand.MoveTo(inButtonOnPress, numSteps);
                yield return hand.MoveTo(rightOfButtonPress, numSteps);
                yield return hand.Hide();
                
                Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");
                Assert.IsTrue(buttonReleased, "Button did not get released when hand exited the button.");

                buttonPressed = false;
                buttonReleased = false;

                buttonComponent.ReleaseOnTouchEnd = false;

                // test no release on moving outside of button when releaseOnTouchEnd is disabled
                yield return hand.Show(startHand);
                yield return hand.MoveTo(inButtonOnPress, numSteps);
                yield return hand.MoveTo(rightOfButtonPress, numSteps);
                yield return hand.Hide();

                Assert.IsTrue(buttonPressed, "Button did not get pressed when hand moved to press it.");
                Assert.IsFalse(buttonReleased, "Button did got released on exit even though releaseOnTouchEnd wasn't set");

                buttonPressed = false;
                buttonReleased = false;

                buttonComponent.ReleaseOnTouchEnd = true;
            }

            Object.Destroy(testButton);

            yield return null;
        }


        #endregion
    }
}


#endif
