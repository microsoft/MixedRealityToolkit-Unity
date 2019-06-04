// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    class GazePointerStateMachineTests
    {
        [Test]
        public void TestHeadGazeHandAndSpeechBehaviour()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Note that in this section, the numFarPointersActive == 1 to simulate the far pointer
            // of the gaze pointer itself.

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Head gaze pointer should be visible on start");

            // After hand is raised, no pointer should show up;
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "After hand is raised, head gaze pointer should go away");

            // After select called, pointer should show up again but only if no hands are up
            FireSelectKeyword(gsm);
            Assert.IsFalse(gsm.IsGazePointerActive, "After select is called but hands are up, head gaze pointer should not show up");

            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            FireSelectKeyword(gsm);
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsTrue(gsm.IsGazePointerActive, "When no hands present and select called, head gaze pointer should show up");

            // Say select while gaze pointer is active, then raise hand. Gaze pointer should go away
            FireSelectKeyword(gsm);
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "After select called with hands present, then hand up, head gaze pointer should go away");

            // Simulate a scene with just the head gaze ray to reset the state such that
            // the head gaze pointer is active.
            FireSelectKeyword(gsm);
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsTrue(gsm.IsGazePointerActive, "Head gaze pointer should be visible with just the gaze pointer in the scene");

            // Simulate the addition of a far hand ray - the head gaze pointer should be hidden now.
            gsm.UpdateState(0 /*numNearPointersActive*/, 2 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "Head gaze pointer should be hidden in the presence of another far pointer");
        }

        [Test]
        /// <summary>
        /// Tests scenarios when the hands are in HoloLens 1 mode (GGV behavior).
        /// GGV stands for gaze, gesture, voice.
        /// </summary>
        public void TestHeadGazeHoloLens1GGV()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Head gaze pointer should be visible on start");

            // Note that in these tests numFarPointersWithoutCursorActive is 1 here. The GGV pointer is a pointer that does not have
            // a base cursor associated with it, there is one of these for each hand.


            // When a hand is raised in HoloLens 1 there will be no near pointers. Gaze cursor should stay on
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 2 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsTrue(gsm.IsGazePointerActive, "After hand is raised, head gaze pointer should not go away");

            // Saying "select" should have no impact on the state of HoloLens 1 interactions.
            FireSelectKeyword(gsm);
            Assert.IsTrue(gsm.IsGazePointerActive, "Saying 'select' should have no impact on HoloLens 1");
        }

        [Test]
        /// <summary>
        /// Tests scenarios when we have articulated hands (HoloLens 2), but the hands
        /// are using the GGV pointers / we are emulating HoloLens 1 behavior
        /// </summary>
        public void TestHeadGazeGGVArticulatedHands()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Head gaze pointer should be visible on start");

            // Note that in these tests numFarPointersWithoutCursorActive is 1 here. The GGV pointer is a pointer that does not have
            // a base cursor associated with it, there is one of these for each hand.

            // When a hand is raised there will be a frame when the near pointer is active. Cursor should go away
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 2 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "After hand is raised, head gaze pointer should go away");

            // Shortly after the near pointer for the hand will be disabled if there is nothing nearby
            // The gaze cursor should now appear
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 2 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsTrue(gsm.IsGazePointerActive, "If hand is not near anything, the gaze cursor should show up again (gaze cursor disappears when hand is near something)");

            // If a near pointer appears again, the gaze cursor should go away
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 2 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "If hand goes near a grabbable, the gaze cursor should disappear");


            // Saying "select" should have no impact on the state of interactions.
            FireSelectKeyword(gsm);
            Assert.IsFalse(gsm.IsGazePointerActive, "Saying 'select' should have no impact on GGV articulated hands");
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 2 /*numFarPointersWithoutCursorActive*/, false);
            FireSelectKeyword(gsm);
            Assert.IsTrue(gsm.IsGazePointerActive, "Saying 'select' should have no impact on GGV articulated hands");

        }

        [Test]
        public void TestEyeGazeHandAndSpeechBehaviour()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Eye gaze pointer should be visible on start");

            // With the hand raised, eye gaze pointer should still exist because only far interaction causes the 
            // eye gaze pointer to go away.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "With near interaction, eye gaze pointer should continue to exist");

            // With far interaction active, eye gaze pointer should be hidden.
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "With far interaction, eye gaze pointer should go away");

            // Reset the state and validate that it goes back to being visible.
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "Eye gaze pointer should be visible when no near or far pointers");

            // Saying "select" should have no impact on the state of eye gaze-based interactions.
            FireSelectKeyword(gsm);
            Assert.IsTrue(gsm.IsGazePointerActive, "Saying 'select' should have no impact on eye gaze");

            // With far and near interaction active, eye gaze pointer should be hidden (because far interaction wins over
            // the eye gaze regardless of near interaction state).
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "With far and near interaction, gaze pointer should go away");
        }

        [Test]
        public void TestEyeGazeToHeadGazeTransition()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // With the hand raised, eye gaze pointer should still exist because only far interaction causes the
            // eye gaze pointer to go away.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "With near interaction, gaze pointer should continue to exist");

            // With far interaction active, eye gaze pointer should be hidden.
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "With far interaction, gaze pointer should go away");

            // Send a "select" command right now, to show that this cached select value doesn't affect the
            // state machine once eye gaze degrades into head gaze.
            FireSelectKeyword(gsm);
            Assert.IsFalse(gsm.IsGazePointerActive, "Select should have no impact while eye gaze is active");

            // From this point on, we're simulating what happens when eye gaze degrades into head gaze.
            // Note that gaze pointer should still be hidden at this point despite no hands being visible
            // because "select" wasn't spoken after the degradation happened.
            // A user saying "select" 10 minutes before shouldn't have that "select" invocation carry over
            // 10 minutes later.
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "Gaze pointer should be inactive");

            // Saying select at this point should now show the eye gaze pointer.
            FireSelectKeyword(gsm);
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be active");
        }

        [Test]
        public void TestHeadGazeToEyeGazeTransition()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // The eye pointer should go away because a hand was raised and head gaze is active.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "With near interaction and head gaze, gaze pointer should be inactive");

            // After transitioning to eye gaze, the gaze pointer should now be active because near interaction
            // doesn't affect the visibility of eye-gaze style pointers.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "With near interaction and eye gaze, gaze pointer should be active");
        }

        private static void FireSelectKeyword(GazePointerVisibilityStateMachine gsm)
        {
            SpeechEventData data = new SpeechEventData(EventSystem.current);
            data.Initialize(new BaseGenericInputSource("test input source", new IMixedRealityPointer[0], InputSourceType.Voice),
                Utilities.RecognitionConfidenceLevel.High,
                System.TimeSpan.MinValue,
                System.DateTime.Now,
                new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            gsm.OnSpeechKeywordRecognized(data);
        }
    }
}
