// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ManipulationHandlerTests
    {
        /// <summary>
        /// Test creating adding a ManipulationHandler to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test01_ManipulationHandlerInstantiate()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            // Wait for two frames to make sure we don't get null pointer exception.
            yield return null;
            yield return null;

            GameObject.Destroy(testObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
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

        /// <summary>
        /// Test creating ManipulationHandler and receiving hover enter/exit events
        /// from gaze provider.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test02_ManipulationHandlerGazeHover()
        {

            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            int hoverEnterCount = 0;
            int hoverExitCount = 0;

            manipHandler.OnHoverEntered.AddListener((eventData) => hoverEnterCount++);
            manipHandler.OnHoverExited.AddListener((eventData) => hoverExitCount++);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(1, hoverEnterCount, $"ManipulationHandler did not receive hover enter event, count is {hoverEnterCount}");

            testObject.transform.Translate(Vector3.up);

            // First yield for physics. Second for normal frame step.
            // Without first one, second might happen before translation is applied.
            // Without second one services will not be stepped.
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(1, hoverExitCount, "ManipulationHandler did not receive hover exit event");

            testObject.transform.Translate(5 * Vector3.up);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.IsTrue(hoverExitCount == 1, "ManipulationHandler received the second hover event");

            GameObject.Destroy(testObject);

            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test creates an object with manipulationhandler verifies translation with articulated hand
        /// as well as forcefully ending of the manipulation
        /// from gaze provider.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerForceRelease()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            // add near interaction grabbable to be able to grab the cube with the siulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();

            yield return new WaitForFixedUpdate();
            yield return null;

            // grab the cube - move it to the right 
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 30;
            
            Vector3 handOffset = new Vector3(0, 0, 0.1f);
            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            Vector3 rightPosition = new Vector3(1f, 0f, 1f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, initialObjectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(initialObjectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);

            yield return null;

            // test if the object was properly translated
            float maxError = 0.05f;
            Vector3 posDiff = testObject.transform.position - rightPosition;
            Assert.IsTrue(posDiff.magnitude <= maxError, "ManipulationHandler translate failed");

            // forcefully end manipulation and drag with hand back to original position - object shouldn't move with hand
            manipHandler.ForceEndManipulation();
            yield return PlayModeTestUtilities.MoveHandFromTo(rightPosition, initialObjectPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);

            posDiff = testObject.transform.position - initialObjectPosition;
            Assert.IsTrue(posDiff.magnitude > maxError, "Manipulationhandler modified objects even though manipulation was forcefully ended.");
            posDiff = testObject.transform.position - rightPosition;
            Assert.IsTrue(posDiff.magnitude <= maxError, "Manipulated object didn't remain in place after forcefully ending manipulation");

            // move hand back to object
            yield return PlayModeTestUtilities.MoveHandFromTo(initialObjectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService);

            // grab object again and move to original position
            yield return PlayModeTestUtilities.MoveHandFromTo(rightPosition, initialObjectPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);

            // test if object was moved by manipulationhandler
            posDiff = testObject.transform.position - initialObjectPosition;
            Assert.IsTrue(posDiff.magnitude <= maxError, "ManipulationHandler translate failed on valid manipulation after ForceEndManipulation");

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            Object.Destroy(testObject);
        }

    }
}
#endif