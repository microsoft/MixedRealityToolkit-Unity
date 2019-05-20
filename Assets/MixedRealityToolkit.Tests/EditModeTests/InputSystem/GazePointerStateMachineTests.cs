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
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // After hand is raised, no pointer should show up;
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "After hand is raised, gaze pointer should go away");

            // After select called, pointer should show up again but only if no hands are up
            SpeechEventData data = new SpeechEventData(EventSystem.current);
            data.Initialize(new BaseGenericInputSource("test input source", new IMixedRealityPointer[0], InputSourceType.Voice ), 
                Utilities.RecognitionConfidenceLevel.High, 
                System.TimeSpan.MinValue, 
                System.DateTime.Now, 
                new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            gsm.OnSpeechKeywordRecognized(data);
            Assert.IsFalse(gsm.IsGazePointerActive, "After select is called but hands are up, gaze pointer should not show up");

            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            gsm.OnSpeechKeywordRecognized(data);
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            Assert.IsTrue(gsm.IsGazePointerActive, "When no hands present and select called, gaze pointer should show up");

            // Say select while gaze pointer is active, then raise hand. Gaze pointer should go away
            gsm.OnSpeechKeywordRecognized(data);
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "After select called with hands present, then hand up, gaze pointer should go away");
        }

        [Test]
        public void TestEyeGazeHandAndSpeechBehaviour()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // With the hand raised, eye gaze pointer should still exist because only far interaction causes the 
            // eye gaze pointer to go away.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "With near interaction, gaze pointer should continue to exist");

            // With far interaction active, eye gaze pointer should be hidden.
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "With far interaction, gaze pointer should go away");

            // Reset the state and validate that it goes back to being visible.
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible when no near or far pointers");

            // Saying "select" should have no impact on the state of eye gaze-based interactions.
            SpeechEventData data = new SpeechEventData(EventSystem.current);
            data.Initialize(new BaseGenericInputSource("test input source", new IMixedRealityPointer[0], InputSourceType.Voice),
                Utilities.RecognitionConfidenceLevel.High,
                System.TimeSpan.MinValue,
                System.DateTime.Now,
                new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            gsm.OnSpeechKeywordRecognized(data);
            Assert.IsTrue(gsm.IsGazePointerActive, "Saying 'select' should have no impact on eye gaze");

            // With far and near interaction active, eye gaze pointer should be hidden (because far interaction wins over
            // the eye gaze regardless of near interaction state).
            gsm.UpdateState(1 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "With far and near interaction, gaze pointer should go away");
        }

        [Test]
        public void TestEyeGazeToHeadGazeTransition()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // With the hand raised, eye gaze pointer should still exist because only far interaction causes the
            // eye gaze pointer to go away.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "With near interaction, gaze pointer should continue to exist");

            // With far interaction active, eye gaze pointer should be hidden.
            gsm.UpdateState(0 /*numNearPointersActive*/, 1 /*numFarPointersActive*/, true);
            Assert.IsFalse(gsm.IsGazePointerActive, "With far interaction, gaze pointer should go away");

            // Send a "select" command right now, to show that this cached select value doesn't affect the
            // state machine once eye gaze degrades into head gaze.
            SpeechEventData data = new SpeechEventData(EventSystem.current);
            data.Initialize(new BaseGenericInputSource("test input source", new IMixedRealityPointer[0], InputSourceType.Voice),
                Utilities.RecognitionConfidenceLevel.High,
                System.TimeSpan.MinValue,
                System.DateTime.Now,
                new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            gsm.OnSpeechKeywordRecognized(data);
            Assert.IsFalse(gsm.IsGazePointerActive, "Select should have no impact while eye gaze is active");

            // From this point on, we're simulating what happens when eye gaze degrades into head gaze.
            // Note that gaze pointer should still be hidden at this point despite no hands being visible
            // because "select" wasn't spoken after the degredation happened.
            // A user saying "select" 10 minutes before shouldn't have that "select" invocation carry over
            // 10 minutes later.
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "Gaze pointer should be inactive");

            // Saying select at this point should now show the eye gaze pointer.
            data = new SpeechEventData(EventSystem.current);
            data.Initialize(new BaseGenericInputSource("test input source", new IMixedRealityPointer[0], InputSourceType.Voice),
                Utilities.RecognitionConfidenceLevel.High,
                System.TimeSpan.MinValue,
                System.DateTime.Now,
                new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            gsm.OnSpeechKeywordRecognized(data);
            gsm.UpdateState(0 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be active");
        }

        [Test]
        public void TestHeadGazeToEyeGazeTransition()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // The eye pointer should go away because a hand was raised and head gaze is active.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, false);
            Assert.IsFalse(gsm.IsGazePointerActive, "With near interaction and head gaze, gaze pointer should be inactive");

            // After transitioning to eye gaze, the gaze pointer should now be active because near interaction
            // doesn't affect the visibility of eye-gaze style pointers.
            gsm.UpdateState(1 /*numNearPointersActive*/, 0 /*numFarPointersActive*/, true);
            Assert.IsTrue(gsm.IsGazePointerActive, "With near interaction and eye gaze, gaze pointer should be active");
        }
    }
}
