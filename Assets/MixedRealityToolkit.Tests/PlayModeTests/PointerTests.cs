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
        /// <returns></returns>
        /// 
        [UnityTest]
        public IEnumerator TestPointerDirectionMatchesCameraDirectionFirstFrame()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Raise the hand
            var rightHand = new TestHand(Handedness.Right);

            //Position 1 show hand
            Vector3 initialPos = new Vector3(0.01f, 0.1f, 0.5f);
            yield return rightHand.Show(initialPos);

            //return first hand controller that is right and source type hand
            var handController = inputSystem.DetectedControllers.First(x => x.ControllerHandedness == Utilities.Handedness.Right && x.InputSource.SourceType == InputSourceType.Hand);
            Assert.IsNotNull(handController);

            // Get the line pointer from the hand controller
            var linePointer = handController.InputSource.Pointers.First(x => x is LinePointer);
            Assert.IsNotNull(linePointer);

            // Take dot product of camera forward and line pointer
            float dot = Vector3.Dot(linePointer.Rays[0].Direction, Camera.main.transform.forward);

            // Making sure the pointer is pointing in the same general direction as the camera
            // A dot of 1 means the pointer and camera forward point in the exact same direction 
            // but greater than 0.5 is same general direction
            Assert.GreaterOrEqual(dot, 0.5f);

            // Position 2
            yield return new WaitForSeconds(1);
            yield return rightHand.MoveTo(new Vector3(-1.0f, 0, 2.0f));

            float dot2 = Vector3.Dot(linePointer.Rays[0].Direction, Camera.main.transform.forward);
            Assert.GreaterOrEqual(dot2, 0.5f);

            // Position 3 
            yield return new WaitForSeconds(1);
            yield return rightHand.MoveTo(new Vector3(1.0f, -1.0f, 2.0f));

            float dot3 = Vector3.Dot(linePointer.Rays[0].Direction, Camera.main.transform.forward);
            Assert.GreaterOrEqual(dot3, 0.5f);

            // Position 4 
            yield return new WaitForSeconds(1);
            yield return rightHand.MoveTo(new Vector3(1.0f, 1.0f, 2.0f));

            float dot4 = Vector3.Dot(linePointer.Rays[0].Direction, Camera.main.transform.forward);
            Assert.GreaterOrEqual(dot4, 0.5f);
        }
        #endregion
    }
}
#endif
