// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
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
            float angle = Vector3.Angle(linePointer.Rays[0].Direction, Camera.main.transform.forward);
            Assert.LessOrEqual(angle, 40.0f);
        }
        #endregion
    }
}
#endif
