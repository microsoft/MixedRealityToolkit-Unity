#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Utility class to provide helpful functions for applying input in tests
    /// </summary>
    public static class TestInputUtilities
    {
        /// <summary>
        /// Fires speech command to default input system with given string and input action type.
        /// If no input source is provided, then the first detected input source is used in conjunction with the raised event.
        /// </summary>
        public static IEnumerator ExecuteSpeechCommand(string voiceCommand, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource = null)
        {
            if (inputSource == null)
            {
                // Find an input source to associate with the input event (doesn't matter which one)
                IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
                Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");
                inputSource = defaultInputSource;
            }

            // Raise a voice select input event, then wait for transition to take place
            // Wait for at least one frame explicitly to ensure the input goes through
            SpeechCommands commands = new SpeechCommands(voiceCommand, KeyCode.None, inputAction);
            CoreServices.InputSystem.RaiseSpeechCommandRecognized(inputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(100), System.DateTime.Now, commands);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
        }

        /// <summary>
        /// Fires a global input click event by given input source and for given input action type. 
        /// If testExec is not null, the function will execute in between the input down and input up raised events.
        /// The testExec function parameter is useful for executing tests and asserts in between raised events.
        /// </summary>
        public static IEnumerator ExecuteGlobalClick(IMixedRealityInputSource defaultInputSource,
                    MixedRealityInputAction inputAction,
                    Func<IEnumerator> testExec = null)
        {
            // Raise a select down input event, then wait for transition to take place
            // Wait for at least one frame explicitly to ensure the input goes through
            CoreServices.InputSystem.RaiseOnInputDown(defaultInputSource, Handedness.Right, inputAction);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            if (testExec != null)
            {
                yield return testExec();
            }

            // Raise a select up input event, then wait for transition to take place
            CoreServices.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.Right, inputAction);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
        }

    }
}
#endif