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

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    class SimulatedUserinputTest
    {
        private GameObject cube;

        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();

            // Explicitly enable user input to test in editor behavior.
            InputSimulationService inputSimulationService = CoreServices.GetInputSystemDataProvider<InputSimulationService>();
            Assert.IsNotNull(inputSimulationService, "InputSimulationService is null!");
            inputSimulationService.UserInputEnabled = true;


            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0, 0, 2);
            cube.transform.localScale = new Vector3(.2f, .2f, .2f);

            KeyInputSystem.StartKeyInputStimulation();
        }

        [TearDown]
        public void TearDown()
        {
            KeyInputSystem.StopKeyInputSimulation();
            PlayModeTestUtilities.TearDown();
        }
        
        [UnityTest]
        public IEnumerator HandsFreeInteractionTest()
        {
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;

            cube.AddComponent<NearInteractionGrabbable>();
            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.PlayspaceToOriginLookingForward();
            yield return new WaitForFixedUpdate();
            yield return null;

            KeyInputSystem.PressKey(iss.InputSimulationProfile.InteractionButton);
            yield return new WaitForFixedUpdate();
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return new WaitForFixedUpdate();
            yield return null;


            // full circle
            const int numCircleSteps = 10;
            const int degreeStep = 360 / numCircleSteps;

            // rotating the camera in a circle
            for (int i = 1; i <= numCircleSteps; ++i)
            {
                // rotate main camera (user)
                CameraCache.Main.transform.Rotate(new Vector3(degreeStep, 0, 0));

                yield return null;
                
                // make sure that the cube moved
            }

            KeyInputSystem.ReleaseKey(iss.InputSimulationProfile.InteractionButton);
            yield return null;
            KeyInputSystem.AdvanceSimulation();
            yield return null;

            // make sure that the cube is back in original position


            CameraCache.Main.transform.Rotate(new Vector3(degreeStep, 0, 0));

            // make sure that the cube is no longer grabbed

        }
    }
}
#endif