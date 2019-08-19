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

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class FocusProviderTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGazeCursorArticulated()
        {
            IMixedRealityInputSystem inputSystem = PlayModeTestUtilities.GetInputSystem();
            yield return null;

            // Verify that the gaze cursor is visible at the start
            Assert.IsTrue(inputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should be visible at start");

            // raise hand up -- gaze cursor should no longer be visible
            // disable user input
            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            inputSimulationService.UserInputEnabled = false;

            ArticulatedHandPose gesturePose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Open);
            var handOpenPose = PlayModeTestUtilities.GenerateHandPose(ArticulatedHandPose.GestureId.Open, Handedness.Right, Vector3.forward * 0.1f, Quaternion.identity);
            inputSimulationService.HandDataRight.Update(true, false, handOpenPose);
            yield return null;

            // Gaze cursor should not be visible
            Assert.IsFalse(inputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should not be visible when one articulated hand is up");
            inputSimulationService.HandDataRight.Update(false, false, handOpenPose);
            yield return null;

            // Say "select" to make gaze cursor active again
            // Really we need to tear down the scene and create it again but MRTK doesn't support that yet
            var gazeInputSource = inputSystem.DetectedInputSources.Where(x => x.SourceName.Equals("Gaze")).First();
            inputSystem.RaiseSpeechCommandRecognized(gazeInputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(), System.DateTime.Now, new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
            yield return null;
            Assert.IsTrue(inputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should be visible after select command");
        }

        /// <summary>
        /// Ensure that the gaze provider hit result is not null when looking at an object,
        /// even when the hand is up
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGazeProviderTargetNotNull()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.forward;

            yield return null;

            Assert.NotNull(CoreServices.InputSystem.GazeProvider.GazeTarget, "GazeProvider target is null when looking at an object");

            TestHand h = new TestHand(Handedness.Right);
            yield return h.Show(Vector3.forward * 0.2f);
            yield return null;

            Assert.NotNull(CoreServices.InputSystem.GazeProvider.GazeTarget, "GazeProvider target is null when looking at an object with hand raised");
        }
    }
}
#endif
