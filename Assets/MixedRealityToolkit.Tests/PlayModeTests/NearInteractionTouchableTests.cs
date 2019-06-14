// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#define DEBUGGINGREMOVEME

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

#if DEBUGGINGREMOVEME
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class NearInteractionTouchableTests
    {
        /// <summary>
        /// Test creating adding a NearInteractionTouchable to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test01_ManipulationHandlerInstantiate()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var touchable = testObject.AddComponent<NearInteractionTouchable>();
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
        /// Test creates an object with NearInteractionTouchable
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test02_NearInteractionTouchableTouching()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;

            // Register the game object to receive events
            Assert.IsTrue(MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out IMixedRealityInputSystem inputSystem));
            inputSystem.Register(testObject);

            var touchable = testObject.AddComponent<NearInteractionTouchable>();
            touchable.SetLocalForward(new Vector3(0, 0, -1));
            touchable.Bounds = new Vector2(1, 1);

            yield return new WaitForFixedUpdate();
            yield return null;

            // grab the cube - move it to the right 
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 30;

            Vector3 handOffset = new Vector3(0, 0, 0.1f);
            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            Vector3 rightPosition = new Vector3(1f, 0f, 1f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);

            using (var catcher = TouchEventCatcher.Create())
            {
                // EditorApplication.isPaused = true;
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, initialObjectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService);
                Assert.AreEqual(catcher.EventsStarted, 1);
                Assert.AreEqual(catcher.EventsCompleted, 0);

                yield return PlayModeTestUtilities.MoveHandFromTo(initialObjectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);
                Assert.AreEqual(catcher.EventsStarted, 1);
                Assert.AreEqual(catcher.EventsCompleted, 1);

                yield return PlayModeTestUtilities.MoveHandFromTo(rightPosition, initialObjectPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);
                Assert.AreEqual(catcher.EventsStarted, 2);
                Assert.AreEqual(catcher.EventsCompleted, 1);

                yield return PlayModeTestUtilities.MoveHandFromTo(initialObjectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService);
                Assert.AreEqual(catcher.EventsStarted, 2);
                Assert.AreEqual(catcher.EventsCompleted, 2);

                yield return PlayModeTestUtilities.MoveHandFromTo(rightPosition, initialObjectPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);
                Assert.AreEqual(catcher.EventsStarted, 3);
                Assert.AreEqual(catcher.EventsCompleted, 2);

                yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);
                Assert.AreEqual(catcher.EventsStarted, 3);
                Assert.AreEqual(catcher.EventsCompleted, 3);
            }

            UnityEngine.Object.Destroy(testObject);
        }

    }
}
#endif