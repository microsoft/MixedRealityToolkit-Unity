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
        public void TestHandAndSpeechBehaviour()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            // Initial state: gaze pointer active
            var gsm = new GazePointerVisibilityStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive, "Gaze pointer should be visible on start");

            // After hand is raised, no pointer should show up;
            gsm.UpdateState(1, 0);
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

            gsm.UpdateState(0, 0);
            gsm.OnSpeechKeywordRecognized(data);
            gsm.UpdateState(0, 0);
            Assert.IsTrue(gsm.IsGazePointerActive, "When no hands present and select called, gaze pointer should show up");

            // Say select while gaze pointer is active, then raise hand. Gaze pointer should go away
            gsm.OnSpeechKeywordRecognized(data);
            gsm.UpdateState(1, 0);
            gsm.UpdateState(1, 0);
            Assert.IsFalse(gsm.IsGazePointerActive, "After select called with hands present, then hand up, gaze pointer should go away");
        }
    }
}
