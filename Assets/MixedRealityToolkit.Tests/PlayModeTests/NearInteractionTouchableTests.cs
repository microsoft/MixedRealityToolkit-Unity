// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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

        private InputSimulationService inputSim;
        IMixedRealityInputSystem inputSystem;

        [SetUp]
        public void SetupMrtk()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            inputSim = PlayModeTestUtilities.GetInputSimulationService();
            Assert.NotNull(inputSim);

            Assert.IsTrue(MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem));
            Assert.NotNull(inputSystem);
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private const int numSteps = 30;

        // Scale larger than bounds vector to test bounds checks
        private float objectScale = 0.4f;
        private Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
        private Vector3 objectPosition = new Vector3(0f, 0f, 1f);
        private Vector3 rightPosition = new Vector3(1f, 0f, 1f);
        private Vector3 backPosition = new Vector3(0f, 0f, 2f);
        private Vector3 outOfBoundsOffset = new Vector3(0.15f, 0f, 0f);
        private Vector3 touchNormal = new Vector3(0, 0, -1);

        private T CreateTouchable<T>() where T : BaseNearInteractionTouchable
        {
            // Set up cube with touchable
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * objectScale;
            testObject.transform.position = objectPosition;

            var touchable = testObject.AddComponent<T>();

            // Register the game object to receive events
            inputSystem.Register(testObject);

            return touchable;
        }

        /// <summary>
        /// Test creates an object with NearInteractionTouchable
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test02_NearInteractionTouchable()
        {
            var touchable = CreateTouchable<NearInteractionTouchable>();
            touchable.SetLocalForward(touchNormal);
            touchable.Bounds = new Vector2(0.5f, 0.5f);

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            using (var catcher = TouchEventCatcher.Create())
            {
                // Touch started and completed when entering and exiting
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(0, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);

                // Touch started and completed when entering and exiting behind the plane
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, backPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                // No touch when moving at behind the plane
                yield return PlayModeTestUtilities.MoveHandFromTo(backPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                // No touch when moving outside the bounds
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition + outOfBoundsOffset, objectPosition + outOfBoundsOffset, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition + outOfBoundsOffset, rightPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);
            }

            UnityEngine.Object.Destroy(touchable.gameObject);
        }

        /// <summary>
        /// Test creates an object with NearInteractionTouchableUnboundedPlane
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test03_NearInteractionTouchableUnboundedPlane()
        {
            var touchable = CreateTouchable<NearInteractionTouchableUnboundedPlane>();
            touchable.SetLocalNormal(touchNormal);

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            using (var catcher = TouchEventCatcher.Create())
            {
                // Touch started and completed when entering and exiting
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(0, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);

                // Touch started and completed when entering and exiting behind the plane
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, backPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                // No touch when moving at behind the plane
                yield return PlayModeTestUtilities.MoveHandFromTo(backPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                // Touch when moving off-center
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition + outOfBoundsOffset, objectPosition + outOfBoundsOffset, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition + outOfBoundsOffset, rightPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(3, catcher.EventsStarted);
                Assert.AreEqual(3, catcher.EventsCompleted);

                yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSim);
                Assert.AreEqual(3, catcher.EventsStarted);
                Assert.AreEqual(3, catcher.EventsCompleted);
            }

            UnityEngine.Object.Destroy(touchable.gameObject);
        }

        /// <summary>
        /// Test creates an object with NearInteractionTouchableVolume
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test04_NearInteractionTouchableVolume()
        {
            var touchable = CreateTouchable<NearInteractionTouchableVolume>();

            yield return new WaitForFixedUpdate();
            yield return null;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSim);

            using (var catcher = TouchEventCatcher.Create())
            {
                // Touch started and completed when entering and exiting the collider
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition, objectPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(0, catcher.EventsCompleted);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);

                // No touch when moving outside the collider
                yield return PlayModeTestUtilities.MoveHandFromTo(backPosition, rightPosition, numSteps, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSim);
                Assert.AreEqual(1, catcher.EventsStarted);
                Assert.AreEqual(1, catcher.EventsCompleted);

                // Touch when moving off-center
                yield return PlayModeTestUtilities.MoveHandFromTo(initialHandPosition + outOfBoundsOffset, objectPosition + outOfBoundsOffset, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                yield return PlayModeTestUtilities.MoveHandFromTo(objectPosition + outOfBoundsOffset, rightPosition, numSteps, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);

                yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSim);
                Assert.AreEqual(2, catcher.EventsStarted);
                Assert.AreEqual(2, catcher.EventsCompleted);
            }

            UnityEngine.Object.Destroy(touchable.gameObject);
        }

    }
}
#endif