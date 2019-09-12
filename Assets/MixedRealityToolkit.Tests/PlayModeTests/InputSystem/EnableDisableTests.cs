// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    /// <summary>
    /// Tests that verify that various services of the MRTK support being disabled
    /// and then re-enabled.
    /// </summary>
    class EnableDisableTests
    {
        private PointerHandler pointerHandler;

        // This cube is created in test setup, and is a .2x.2.x2 sized
        // cube located at (0, 0, 2).
        private GameObject cube;

        /// <summary>
        /// Validates that it's possible to disable and then re-enable the input system,
        /// and that the gaze subsystem is inactive when the input system is disabled
        /// and working again once the input system is re-enabled.
        /// </summary>
        [UnityTest]
        public IEnumerator TestGazeInputSystemEnableDisable()
        {
            // Assert that the initial state of the cube is one that hasn't had any relevant
            // focus events
            Assert.AreEqual(0, pointerHandler.state.numFocusEnter);

            // The input system is enabled by default, so we first disable it.
            MixedRealityServiceRegistry.DisableService<IMixedRealityInputSystem>();
            yield return null;

            CameraCache.Main.transform.LookAt(cube.transform);
            yield return null;

            // At this point, looking at the cube would normally have triggered 
            // a numFocusEnter increment, but because the input system is disabled
            // this shouldn't do anything. In addition, the GazeProvider should still
            // be valid (i.e. non-null) but its target shouldn't have updated.
            Assert.AreEqual(0, pointerHandler.state.numFocusEnter);
            Assert.IsNotNull(CoreServices.InputSystem.GazeProvider);
            Assert.IsNull(CoreServices.InputSystem.GazeProvider.GazeTarget);

            // Reset the camera to look forward, and then enable the input system and
            // validate that this all works when the input system has been enabled.
            CameraCache.Main.transform.LookAt(Vector3.forward);
            yield return null;

            MixedRealityServiceRegistry.EnableService<IMixedRealityInputSystem>();
            yield return null;

            CameraCache.Main.transform.LookAt(cube.transform);
            yield return null;

            Assert.AreEqual(1, pointerHandler.state.numFocusEnter);
            Assert.AreEqual(cube, CoreServices.InputSystem.GazeProvider.GazeTarget);
            yield return null;
        }

        /// <summary>
        /// Validates that it's possible to disable and then re-enable the input system,
        /// and that the hands don't trigger a focus event when the input system is
        /// disabled.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandsInputSystemEnableDisable()
        {
            // This test mainly uses near interaction in verifying focus changes,
            // so we need to make sure that the cube is marked as supporting near
            // interactions
            cube.AddComponent<NearInteractionGrabbable>();

            // Assert that the initial state of the cube is one that hasn't had any relevant
            // focus events
            Assert.AreEqual(0, pointerHandler.state.numFocusEnter);

            // Show the hand not around the cube and make sure that it isn't triggering
            // focus.
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(0, pointerHandler.state.numFocusEnter);

            // The input system is enabled by default, so we first disable it.
            MixedRealityServiceRegistry.DisableService<IMixedRealityInputSystem>();
            yield return null;

            // Because the input system is disabled, we can't move the hand to where
            // the cube is - however, we CAN move the cube to where the hand was,
            // and then validate that there no focus enter event.
            cube.transform.position = Vector3.zero;
            Assert.AreEqual(0, pointerHandler.state.numFocusEnter);

            // Move the cube back to where it was originally, so that we can
            // repeat the steps above and show that with the input system
            // re-enabled, everything works as expected.
            cube.transform.position = new Vector3(0, 0, 2);

            MixedRealityServiceRegistry.EnableService<IMixedRealityInputSystem>();
            yield return null;

            // Show the right hand where the cube is, and verify this causes
            // a focus event.
            rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(cube.transform.position);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(1, pointerHandler.state.numFocusEnter);
        }

        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();
            CameraCache.Main.transform.LookAt(Vector3.forward);

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0, 0, 2);
            cube.transform.localScale = new Vector3(.2f, .2f, .2f);

            var collider = cube.GetComponentInChildren<Collider>();
            pointerHandler = collider.gameObject.AddComponent<PointerHandler>();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
            GameObject.Destroy(cube);
        }
    }
}
#endif