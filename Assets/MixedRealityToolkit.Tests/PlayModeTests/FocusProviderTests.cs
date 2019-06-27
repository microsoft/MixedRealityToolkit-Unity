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
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGazeCursorArticulated()
        {
            IMixedRealityInputSystem inputSystem;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);
            Assert.IsNotNull(inputSystem, "MixedRealityInputSystem is null!");

            yield return null;
            // Verify that the gaze cursor is visible at the start
            Assert.IsTrue(inputSystem.GazeProvider.GazePointer.IsInteractionEnabled, "Gaze cursor should be visible at start");

            // raise hand up -- gaze cursor should no longer be visible
            // disable user input
            InputSimulationService inputSimulationService = (inputSystem as IMixedRealityDataProviderAccess).GetDataProvider<InputSimulationService>();
            Assert.IsNotNull(inputSimulationService, "InputSimulationService is null!");

            inputSimulationService.UserInputEnabled = false;
            ArticulatedHandPose gesturePose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.Open);
            var handOpenPose = PlayModeTestUtilities.GenerateHandPose(ArticulatedHandPose.GestureId.Open, Handedness.Right, Vector3.forward * 0.1f);
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

        [SetUp]
        public void SetupMrtk()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }
    }
}
#endif