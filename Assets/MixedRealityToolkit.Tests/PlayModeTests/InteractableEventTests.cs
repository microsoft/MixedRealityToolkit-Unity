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

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class InteractableEventTests
    {
        GameObject cube;
        Interactable interactable;

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(1, 0, 1);

            interactable = cube.AddComponent<Interactable>();
            Assert.NotNull(interactable, "Failed to add interactable to cube");
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(cube);
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Tests that an Interactable component can be added to a GameObject
        /// at runtime, and an OnClick event handler can be added
        /// </summary>
        [UnityTest]
        public IEnumerator TestClickEvents()
        {
            PlayModeTestUtilities.PushHandSimulationProfile();
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            var testHand = new TestHand(Handedness.Right);
            yield return testHand.Show(Vector3.zero);

            CameraCache.Main.transform.LookAt(interactable.transform);
            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return testHand.Hide();

            Assert.True(wasClicked);

            PlayModeTestUtilities.PopHandSimulationProfile();
        }

        [UnityTest]
        public IEnumerator TestGrabEvents()
        {
            var grabReceiver = interactable.AddReceiver<InteractableOnGrabReceiver>();
            interactable.gameObject.AddComponent<NearInteractionGrabbable>();
            bool didGrab = false;
            grabReceiver.OnGrab.AddListener(() => didGrab = true);
            bool didRelease = false;
            grabReceiver.OnRelease.AddListener(() => didRelease = true);

            var testHand = new TestHand(Handedness.Right);
            yield return testHand.Show(interactable.transform.position);
            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return testHand.Hide();
            Assert.True(didGrab, "Did not receive grab event");
            Assert.True(didRelease, "Did not receive release event");
            GameObject.Destroy(cube);
            yield return null;
        }

        public IEnumerator TestHoldEvents()
        {
            // Hold
            var holdReceiver = interactable.AddReceiver<InteractableOnHoldReceiver>();
            bool didHold = false;
            holdReceiver.OnHold.AddListener(() => didHold = true);

            var testHand = new TestHand(Handedness.Right);
            yield return testHand.Show(interactable.transform.position);
            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForSeconds(holdReceiver.HoldTime);
            yield return testHand.Hide();
            Assert.True(didHold, "Did not receive hold event");
            GameObject.Destroy(cube);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPressEvents()
        {
            var testHand = new TestHand(Handedness.Right);
            interactable.gameObject.AddComponent<NearInteractionGrabbable>();
            var pressReceiver = interactable.AddReceiver<InteractableOnPressReceiver>();
            bool didPress = false;
            pressReceiver.OnPress.AddListener(() => didPress = true);
            yield return testHand.Show(interactable.transform.position);
            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return testHand.Hide();
            Assert.True(didPress, "did not receive press event");
        }

        [UnityTest]
        public IEnumerator TestToggleEvents()
        {
            PlayModeTestUtilities.PushHandSimulationProfile();
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            var toggleReceiver = interactable.AddReceiver<InteractableOnToggleReceiver>();
            interactable.transform.position = Vector3.forward * 2f;
            interactable.NumOfDimensions = 2;
            interactable.CanSelect = true;
            interactable.CanDeselect = true;
            bool didSelect = false;
            bool didUnselect = false;
            toggleReceiver.OnSelect.AddListener(() => didSelect = true);
            toggleReceiver.OnDeselect.AddListener(() => didUnselect = true);

            var testHand = new TestHand(Handedness.Right);
            yield return testHand.Show(Vector3.forward);

            CameraCache.Main.transform.LookAt(interactable.transform.position);

            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return testHand.Click();
            yield return testHand.Click();
            yield return testHand.Hide();
            Assert.True(didSelect, "Toggle select did not fire");
            Assert.True(didUnselect, "Toggle unselect did not fire");

            PlayModeTestUtilities.PopHandSimulationProfile();
        }


        [UnityTest]
        public IEnumerator TestTouchEvents()
        {
            interactable.gameObject.AddComponent<NearInteractionTouchableVolume>();
            var touchReceiver = interactable.AddReceiver<InteractableOnTouchReceiver>();
            bool didTouch = false;
            bool didUntouch = false;
            touchReceiver.OnTouchStart.AddListener(() => didTouch = true);
            touchReceiver.OnTouchEnd.AddListener(() => didUntouch = true);

            var testHand = new TestHand(Handedness.Right);
            yield return testHand.Show(Vector3.forward);
            yield return testHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return testHand.MoveTo(interactable.transform.position);
            yield return testHand.MoveTo(Vector3.forward);
            yield return testHand.Hide();
            Assert.True(didTouch, "Did not receive touch event");
            Assert.True(didUntouch, "Did not receive touch end event");
        }

        /// <summary>
        /// Tests that an interactable component can be added to a GameObject
        /// at runtime, and all receivers that extend ReceiverBase
        /// and have event handlers can easily be added, and work
        /// </summary>
        [UnityTest]
        public IEnumerator TestFocusEvents()
        {
            var onFocusReceiver = interactable.AddReceiver<InteractableOnFocusReceiver>();

            bool didHover = false;
            bool didUnHover = false;
            onFocusReceiver.OnFocusOn.AddListener(() => didHover = true);
            onFocusReceiver.OnFocusOff.AddListener(() => didUnHover = true);
            CameraCache.Main.transform.LookAt(interactable.transform);
            yield return null;
            Assert.True(didHover, "Interactable did not receive hover event");
            Assert.False(didUnHover, "Interactable shouldn't have received un-hover event, but it did.");
            CameraCache.Main.transform.LookAt(Vector3.forward);
            yield return null;
            Assert.True(didUnHover, "Interactable did not receive un-hover event");

            yield return null;
        }
    }
}
#endif
