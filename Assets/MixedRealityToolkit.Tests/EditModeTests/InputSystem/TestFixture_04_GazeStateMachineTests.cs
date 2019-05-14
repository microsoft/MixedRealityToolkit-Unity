// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    class TestFixture_04_GazeStateMachineTests
    {
        [Test]
        public void Test01_TestHandAndSpeechBehavior()
        {
            // Initial state: gaze pointer active
            var gsm = new GazePointerStateMachine();
            Assert.IsTrue(gsm.IsGazePointerActive);

            // After hand is raised, no pointer should show up;
            gsm.UpdateState(1, 0);
            Assert.IsFalse(gsm.IsGazePointerActive);

            // After select called, pointer should show up again but only if no hands are up
            SpeechEventData data = new SpeechEventData(EventSystem.current);
            data.Initialize(new BaseGenericInputSource("test input source"), Utilities.RecognitionConfidenceLevel.High, System.TimeSpan.MinValue, System.DateTime.Now, new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            gsm.OnSpeechKeywordRecognized(data);
            Assert.IsFalse(gsm.IsGazePointerActive);

            gsm.UpdateState(0, 0);
            gsm.OnSpeechKeywordRecognized(data);
            Assert.IsTrue(gsm.IsGazePointerActive);
        }
    }
}
