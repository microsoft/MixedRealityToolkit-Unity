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
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify pointer state and pointer direction
    class PointerTests 
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        #region Tests

        /// <summary>
        /// Tests that sphere pointer grabs object when hand is insize a giant grabbable
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpherePointerInsideGrabbable()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spherePointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);
            Assert.IsNotNull(spherePointer, "Right hand does not have a sphere pointer");
            Assert.IsTrue(spherePointer.IsInteractionEnabled, "Sphere pointer should be enabled because it is near grabbable cube and visible, even if inside a giant cube.");
            GameObject.Destroy(cube);
        }

        /// <summary>
        /// Tests that sphere pointer behaves correctly when hand is near grabbable
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpherePointerNearGrabbable()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            cube.transform.position = Vector3.forward;
            cube.transform.localScale = Vector3.one * 0.1f;

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.forward);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spherePointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);
            Assert.IsNotNull(spherePointer, "Right hand does not have a sphere pointer");
            Assert.IsTrue(spherePointer.IsInteractionEnabled, "Sphere pointer should be enabled because it is near grabbable cube and visible.");
            
            // Move forward so that cube is no longer visible
            CameraCache.Main.transform.Translate(Vector3.up * 10);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsFalse(spherePointer.IsInteractionEnabled, "Sphere pointer should NOT be enabled because hand is near grabbable but the grabbable is not visible.");

            // Move camera back so that cube is visible again
            CameraCache.Main.transform.Translate(Vector3.up * -10f);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(spherePointer.IsInteractionEnabled, "Sphere pointer should be enabled because it is near grabbable cube and visible.");
            GameObject.Destroy(cube);
        }

        /// <summary>
        /// Tests that right after being instantiated, the pointer's direction 
        /// is in the same general direction as the forward direction of the camera
        /// </summary>
        [UnityTest]
        public IEnumerator TestPointerDirectionToCameraDirection()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Raise the hand
            var rightHand = new TestHand(Handedness.Right);

            // Set initial position and show hand
            Vector3 initialPos = new Vector3(0.01f, 0.1f, 0.5f);
            yield return rightHand.Show(initialPos);

            // Return first hand controller that is right and source type hand
            var handController = inputSystem.DetectedControllers.First(x => x.ControllerHandedness == Utilities.Handedness.Right && x.InputSource.SourceType == InputSourceType.Hand);
            Assert.IsNotNull(handController);

            // Get the line pointer from the hand controller
            var linePointer = handController.InputSource.Pointers.First(x => x is LinePointer);
            Assert.IsNotNull(linePointer);

            Vector3 linePointerOrigin = linePointer.Position;

            // Check that the line pointer origin is within half a centimeter of the initial position of the hand
            var distance = Vector3.Distance(initialPos, linePointerOrigin);
            Assert.LessOrEqual(distance, 0.005f);

            // Check that the angle between the line pointer ray and camera forward does not exceed 40 degrees
            float angle = Vector3.Angle(linePointer.Rays[0].Direction, CameraCache.Main.transform.forward);
            Assert.LessOrEqual(angle, 40.0f);
        }
        #endregion
    }
}
#endif
