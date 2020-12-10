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
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    class UserInputSimulationTest
    {
        GameObject cube;
        Interactable interactable;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            PlayModeTestUtilities.Setup();

            // Explicitly enable user input to test in editor behavior.
            InputSimulationService iss = PlayModeTestUtilities.GetInputSimulationService();
            Assert.IsNotNull(iss, "InputSimulationService is null!");
            iss.UserInputEnabled = true;

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0, 0, 2);
            cube.transform.localScale = new Vector3(.2f, .2f, .2f);

            interactable = cube.AddComponent<Interactable>();

            KeyInputSystem.StartKeyInputStimulation();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            KeyInputSystem.StopKeyInputSimulation();
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        [UnityTest]
        public IEnumerator InputSimulationHandsFreeInteraction()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // start click on the cube
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // release the click on the cube
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Check to see that the cube was clicked on
            Assert.True(wasClicked);
        }

        [UnityTest]
        public IEnumerator InputSimulationArticulatedHandNearGrabbable()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // No hands, default cursor should be visible
            Assert.IsTrue(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible, "Head gaze cursor should be visible");

            // Begin right hand manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.ToggleRightControllerKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure right hand is tracked
            Assert.True(iss.HandDataRight.IsTracked);

            TestHand hand = new TestHand(Handedness.Right);

            // Head cursor invisible when hand is tracked
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible, "Eye gaze cursor should not be visible");
            // Hand ray visible
            var handRayPointer = hand.GetPointer<ShellHandRayPointer>();
            Assert.True(handRayPointer.IsActive, "Hand ray not active");

            // Create grabbable cube
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            cube.transform.localScale = Vector3.one * 0.3f;
            cube.transform.position = new Vector3(-0.2f, 0.2f, 0.6f);
            yield return null;

            // Grab pointer is near grabbable
            var grabPointer = hand.GetPointer<SpherePointer>();
            Assert.IsTrue(grabPointer.isActiveAndEnabled, "grab pointer is enabled");
            Assert.IsTrue(grabPointer.IsNearObject, "Grab pointer should be near a grabbable");

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Head cursor invisible when grab pointer is near grabbable
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible, "Eye gaze cursor should not be visible");
            // Hand ray invisible when grab pointer is near grabbable
            Assert.True(!handRayPointer.IsActive, "Hand ray not active");
        }

        [UnityTest]
        public IEnumerator InputSimulationArticulatedHandGesture()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // Check that gaze cursor is initially visible
            Assert.True(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Begin right hand manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure right hand is tracked
            Assert.True(iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);

            // Make sure gaze cursor is not visible
            Assert.True(!CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // start click
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure correct hand is pinching
            Assert.True(iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);

            // release the click
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure hands are not pinching anymore
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);

            // End right hand manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand hide timeout to hide the hands
            yield return new WaitForSeconds(iss.InputSimulationProfile.ControllerHideTimeout + 0.1f);

            // Make sure right hand is not tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);

            // Check that gaze cursor is visible
            Assert.True(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Repeat with left hand
            // Begin left hand manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure left hand is tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(iss.HandDataLeft.IsTracked);

            // Make sure gaze cursor is not visible
            Assert.True(!CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // start click
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure correct hand is pinching
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(iss.HandDataLeft.IsPinching);

            // release the click
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure hands are not pinching anymore
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);


            // End left hand manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand hide timeout to hide the hands
            yield return new WaitForSeconds(iss.InputSimulationProfile.ControllerHideTimeout + 0.1f);

            // Make sure left hand is not tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);

            // Check that gaze cursor is visible
            Assert.True(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Lastly with 2 hands
            // Begin hand manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure hands are tracked
            Assert.True(iss.HandDataRight.IsTracked);
            Assert.True(iss.HandDataLeft.IsTracked);

            // Make sure gaze cursor is not visible
            Assert.True(!CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // start click
            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand to animate
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return new WaitForSeconds(1.0f / iss.InputSimulationProfile.HandGestureAnimationSpeed + 0.1f);

            // Make sure hands are pinching
            Assert.True(iss.HandDataRight.IsPinching);
            Assert.True(iss.HandDataLeft.IsPinching);

            // release the click
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure hands are not pinching anymore
            Assert.True(!iss.HandDataRight.IsPinching);
            Assert.True(!iss.HandDataLeft.IsPinching);


            // End hand manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the hand hide timeout to hide the hands
            yield return new WaitForSeconds(iss.InputSimulationProfile.ControllerHideTimeout + 0.1f);


            // Make sure hands are not tracked
            Assert.True(!iss.HandDataRight.IsTracked);
            Assert.True(!iss.HandDataLeft.IsTracked);

            // Check that gaze cursor is visible
            Assert.True(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);
        }

        [UnityTest]
        public IEnumerator InputSimulationRightMotionControllerButtonState()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            // Switch to motion controller
            var oldHandSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // Check that gaze cursor is initially visible
            Assert.True(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Begin right motion controller manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure right motion controller is tracked
            Assert.True(iss.MotionControllerDataRight.IsTracked);
            Assert.True(!iss.MotionControllerDataLeft.IsTracked);

            // Make sure gaze cursor is not visible
            Assert.True(!CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // press trigger
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is selecting
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not selecting anymore
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);

            // press grab
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is grabbing
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not grabbing anymore
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);

            // press menu
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is pressing menu
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not pressing menu anymore
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);

            // press all three buttons
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is pressing three buttons
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not pressing three buttons anymore
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            

            // End right motion controller manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the motion controller hide timeout to hide the motion controllers
            yield return new WaitForSeconds(iss.InputSimulationProfile.ControllerHideTimeout + 0.1f);

            // Make sure right motion controller is not tracked
            Assert.True(!iss.MotionControllerDataRight.IsTracked);
            Assert.True(!iss.MotionControllerDataLeft.IsTracked);


            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldHandSimMode;
            yield return null;
        }

        [UnityTest]
        public IEnumerator InputSimulationLeftMotionControllerButtonState()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            // Switch to motion controller
            var oldHandSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // Check that gaze cursor is initially visible
            Assert.True(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Begin left motion controller manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure left motion controller is tracked
            Assert.True(iss.MotionControllerDataLeft.IsTracked);
            Assert.True(!iss.MotionControllerDataRight.IsTracked);

            // Make sure gaze cursor is not visible
            Assert.True(!CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // press trigger
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is selecting
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not selecting anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);

            // press grab
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is grabbing
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not grabbing anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);

            // press menu
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is pressing menu
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not pressing menu anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);

            // press all three buttons
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure correct motion controller is pressing three buttons
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not pressing three buttons anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);


            // End left motion controller manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the motion controller hide timeout to hide the motion controllers
            yield return new WaitForSeconds(iss.InputSimulationProfile.ControllerHideTimeout + 0.1f);

            // Make sure left motion controller is not tracked
            Assert.True(!iss.MotionControllerDataLeft.IsTracked);
            Assert.True(!iss.MotionControllerDataRight.IsTracked);


            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldHandSimMode;
            yield return null;
        }

        [UnityTest]
        public IEnumerator InputSimulationBothMotionControllerButtonState()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            // Switch to motion controller
            var oldHandSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            // Check that gaze cursor is initially visible
            Assert.True(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Begin both motion controller manipulation
            KeyInputSystem.PressKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure both motion controllers are tracked
            Assert.True(iss.MotionControllerDataLeft.IsTracked);
            Assert.True(iss.MotionControllerDataRight.IsTracked);

            // Make sure gaze cursor is not visible
            Assert.True(!CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // press trigger
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure both motion controllers are selecting
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsSelecting);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not selecting anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);

            // press grab
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure both motion controllers are grabbing
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsGrabbing);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not grabbing anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);

            // press menu
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure both motion controllers are pressing menu
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsPressingMenu);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not pressing menu anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);

            // press all three buttons
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            KeyInputSystem.PressKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure both motion controllers are pressing three buttons
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(iss.MotionControllerDataRight.ButtonState.IsPressingMenu);

            // release the button
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerTriggerKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerGrabKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.MotionControllerMenuKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // Make sure motion controllers are not pressing three buttons anymore
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsSelecting);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsGrabbing);
            Assert.True(!iss.MotionControllerDataLeft.ButtonState.IsPressingMenu);
            Assert.True(!iss.MotionControllerDataRight.ButtonState.IsPressingMenu);


            // End both motion controller manipulation
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.LeftControllerManipulationKey);
            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.RightControllerManipulationKey);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            // Wait for the motion controller hide timeout to hide the motion controllers
            yield return new WaitForSeconds(iss.InputSimulationProfile.ControllerHideTimeout + 0.1f);

            // Make sure both motion controllers are not tracked
            Assert.True(!iss.MotionControllerDataLeft.IsTracked);
            Assert.True(!iss.MotionControllerDataRight.IsTracked);


            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldHandSimMode;
            yield return null;
        }
    }
}
#endif
