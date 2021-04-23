// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    class GazePointerStateMachineTests
    {
        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        [Test]
        public void TestHeadGazeHandAndSpeechBehaviour()
        {
            TestGazeHandAndSpeechBehaviour(false);
        }

        [Test]
        public void TestEyeGazeHandAndSpeechBehaviour()
        {
            TestGazeHandAndSpeechBehaviour(true);
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
        public void TestEyeGazeToHeadGazeTransition()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // With one hand is raised and being in near touch mode, the eye gaze pointer should be disabled.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "When hands are in near interaction mode, the eye gaze pointer should be disabled");

            // With far interaction active, the eye gaze pointer should be hidden.
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "When hand rays are active, the eye gaze pointer should be disabled");

            // Saying "select" should have no impact on the state of eye gaze pointer. 
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

            // The eye pointer should go away because a hand was raised and eye gaze is inactive defaulting back to head gaze.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "With near interaction and head gaze, gaze pointer should be inactive");

            // After transitioning to eye gaze, the gaze pointer should still be inactive because there is at least one hand 
            // in near interaction mode.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "With near interaction and eye gaze, gaze pointer should be inactive");

            // With far interaction active, the eye gaze pointer should be hidden.
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "When hand rays are active, the eye gaze pointer should be disabled");

            // With no other near or far pointer active, the eye gaze pointer should still be inactive.
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "Eye gaze pointer should be inactive");

            // Saying select at this point should now show the eye gaze pointer.
            FireSelectKeyword(gsm);
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be active");
        }

        private void TestGazeHandAndSpeechBehaviour(bool isEyeGazeValid)
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            // Note that in this section, the numFarPointersActive == 1 to simulate the far pointer
            // of the gaze pointer itself.

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // After hand is raised, no pointer should show up;
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, isEyeGazeValid);
            Assert.IsFalse(gsm.IsGazePointerActive, "After hand is raised, gaze pointer should go away");

            // After select called, pointer should show up again but only if no hands are up
            FireSelectKeyword(gsm);
            Assert.IsFalse(gsm.IsGazePointerActive, "After select is called but hands are up, gaze pointer should not show up");

            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, isEyeGazeValid);
            FireSelectKeyword(gsm);
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, isEyeGazeValid);
            Assert.IsTrue(gsm.IsGazePointerActive, "When no hands present and select called, gaze pointer should show up");

            // Say select while gaze pointer is active, then raise hand. Gaze pointer should go away
            FireSelectKeyword(gsm);
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, isEyeGazeValid);
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, isEyeGazeValid);
            Assert.IsFalse(gsm.IsGazePointerActive, "After select called with hands present, then hand up, gaze pointer should go away");

            // Simulate a scene with just the gaze ray to reset the state such that
            // the gaze pointer is active.
            FireSelectKeyword(gsm);
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, isEyeGazeValid);
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible with just the gaze pointer in the scene");

            // Simulate the addition of a far hand ray - the gaze pointer should be hidden now.
            gsm.UpdateState(0 /*numNearPointersActive*/, 2 /*numFarPointersActive*/, 0 /*numFarPointersWithoutCursorActive*/, isEyeGazeValid);
            Assert.IsFalse(gsm.IsGazePointerActive, "Gaze pointer should be hidden in the presence of another far pointer");
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
