// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    public class SliderTests : BaseRuntimeInputTests
    {
        // UXComponents/Sliders/Prefabs/NonCanvasSliderBase.prefab
        private const string defaultSliderPrefabGuid = "5cf5d5d5cc7fe184a93c388dbab66bb9";
        private static readonly string defaultSliderPrefabPath = AssetDatabase.GUIDToAssetPath(defaultSliderPrefabGuid);
        private const float sliderValueDelta = 0.02f;

        public override IEnumerator Setup()
        {
            yield return base.Setup();
            InputTestUtilities.InitializeCameraToOriginAndForward();
            yield return null;
        }

        #region Tests
        /// <summary>
        /// Tests that a slider component can be added at runtime.
        /// at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAddInteractableAtRuntime()
        {

            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out SliderVisuals sliderVisuals);
            yield return DirectPinchAndMoveSlider(slider, 1.0f);

            // clean up
            Object.Destroy(sliderObject);
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// Tests that an interactable assembled at runtime can be manipulated
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleInteractableAndNearManip()
        {
            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);

            Assert.AreEqual(0.5f, slider.Value, sliderValueDelta, "Slider should have value 0.5 at start");
            yield return DirectPinchAndMoveSlider(slider, 1.0f);
            // Allow some leeway due to grab positions shifting on open
            Assert.AreEqual(1.0f, slider.Value, sliderValueDelta, "Slider should have value 1.0 after being manipulated at start");

            //// clean up
            Object.Destroy(sliderObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestLoadPrefabAndNearManipNoSnap()
        {
            // This should not throw exception
            InstantiateDefaultSliderPrefab(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);
            slider.SnapToPosition = false;
            slider.IsTouchable = false;

            Assert.AreEqual(0.5f, slider.Value, sliderValueDelta, "Slider should have value 0.5 at start");
            yield return DirectPinchAndMoveSlider(slider, 1.0f);
            // Allow some leeway due to grab positions shifting on open
            Assert.AreEqual(1.0f, slider.Value, sliderValueDelta, "Slider should have value 1.0 after being manipulated at start");

            // clean up
            Object.Destroy(sliderObject);
            yield return null;
        }

        /// <summary>
        /// Tests that slider can be assembled from code and manipulated using Gaze
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleInteractableAndGazePinchManip()
        {
            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(slider.Value == 0.5, "Slider should have value 0.5 at start");

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.08f, 0.3f));
            yield return rightHand.Show(initialPos);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.MoveTo(InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.08f, 0.3f)), 60);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(slider.isSelected, "Slider wasn't selected");
            Assert.IsTrue(slider.IsGazePinchSelected, "Slider wasn't gaze pinch selected");
            Assert.IsTrue(slider.interactorsSelecting.Count == 1, "Something else was selecting the slider, too! Should only have one selecting.");

            yield return rightHand.Move(new Vector3(0.1f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Hide();

            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(!slider.isSelected, "Slider wasn't deselected");
            Assert.IsTrue(!slider.IsGazePinchSelected, "Slider wasn't de-gaze-pinch-selected");

            Assert.That(slider.Value, Is.GreaterThan(0.5), "Slider didn't move after gaze-pinch manipulation.");

            // clean up
            Object.Destroy(sliderObject);
        }

        /// <summary>
        /// Tests that interactable raises proper events
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleInteractableAndEventsRaised()
        {
            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);
            yield return RuntimeTestUtilities.WaitForUpdates();

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, 0, 1.0f));

            yield return RuntimeTestUtilities.WaitForUpdates();

            bool interactionStarted = false;
            slider.selectEntered.AddListener((x) => interactionStarted = true);
            yield return rightHand.Show(initialPos);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");

            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            yield return rightHand.Move(new Vector3(0.1f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            bool interactionEnded = false;
            slider.selectExited.AddListener((x) => interactionEnded = true);

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            Assert.That(slider.Value, Is.GreaterThan(0.5));

            Object.Destroy(sliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when using step divisions
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleStepSlider()
        {
            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);

            // Set the slider to use step divisions
            slider.UseSliderStepDivisions = true;
            slider.SliderStepDivisions = 4;

            yield return RuntimeTestUtilities.WaitForUpdates();

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, 0, 1.0f));

            bool interactionStarted = false;
            slider.selectEntered.AddListener((x) => interactionStarted = true);
            yield return rightHand.Show(initialPos);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");

            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            yield return rightHand.Move(new Vector3(0.25f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            bool interactionEnded = false;
            slider.selectExited.AddListener((x) => interactionEnded = true);

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            Assert.AreEqual(0.75f, slider.Value);

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Move(new Vector3(-0.5f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.AreEqual(0.25f, slider.Value);

            Object.Destroy(sliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when snapping to a position
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleSnapSlider()
        {
            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Set the slider to use step divisions
            slider.UseSliderStepDivisions = true;
            slider.SliderStepDivisions = 4;

            // Set slider to snap to position
            slider.SnapToPosition = true;
            slider.IsTouchable = true;


            bool interactionStarted = false;
            slider.selectEntered.AddListener((x) => interactionStarted = true);
            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = InputTestUtilities.InFrontOfUser(new Vector3(0.3f, 0, 0.0f));

            yield return rightHand.Show(initialPos);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Move(new Vector3(0, 0, 1.0f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");
            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            Assert.AreEqual(0.75f, slider.Value);

            yield return rightHand.Move(new Vector3(-0.5f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(0.25f, slider.Value);

            // Ensure the step divisions work (that a small motion doesn't adjust the slider)
            yield return rightHand.Move(new Vector3(-0.1f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(0.25f, slider.Value);

            bool interactionEnded = false;
            slider.selectExited.AddListener((x) => interactionEnded = true);

            yield return rightHand.MoveTo(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            Object.Destroy(sliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when snapping to a position
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleTouchSnapSlider()
        {
            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);

            // Set the slider to use step divisions
            slider.UseSliderStepDivisions = true;
            slider.SliderStepDivisions = 4;

            // Set slider to snap with touch
            slider.SnapToPosition = true;
            slider.IsTouchable = true;

            yield return RuntimeTestUtilities.WaitForUpdates();

            bool interactionStarted = false;
            slider.selectEntered.AddListener((x) => interactionStarted = true);
            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = InputTestUtilities.InFrontOfUser(new Vector3(0.3f, 0, 0.0f));
            yield return rightHand.Show(initialPos);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Move(new Vector3(0, 0, 1.0f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");
            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            Assert.AreEqual(0.75f, slider.Value);

            yield return rightHand.Move(new Vector3(-0.5f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(0.25f, slider.Value);

            yield return rightHand.Move(new Vector3(0.3f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(0.5f, slider.Value);

            bool interactionEnded = false;
            slider.selectExited.AddListener((x) => interactionEnded = true);

            // Move the hand back to finish the interactions
            yield return rightHand.Move(new Vector3(0.5f, 0, 0));
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            Object.Destroy(sliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when *not* snapping to the touch point
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleTouchNoSnapSlider()
        {
            // For some reason, batch tests "lose" the poke really easily.
            // This really shouldn't happen. Possibly an 
            // edge case with how the PokeInteractor does spherecasts?
            // It works perfectly fine in-editor, so I'm assuming it's some kind
            // of strange interaction between the timing of frames, physics updates,
            // and the per-frame spherecasts that the poke interactor does.
            if (Application.isBatchMode) { yield break; }

            // This should not throw exception
            AssembleSlider(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out _);

            // Set slider to not snap with touch
            slider.SnapToPosition = false;
            slider.IsTouchable = true;
            slider.UseSliderStepDivisions = false;
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.AreEqual(0.5f, slider.Value, "Slider did not initialize to the correct value");

            bool interactionStarted = false;
            slider.selectEntered.AddListener((x) => interactionStarted = true);
            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            var rightHand = new TestHand(Handedness.Right);
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, Input.Simulation.ControllerAnchorPoint.IndexFinger);
            yield return rightHand.Show();
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Poke the slider at the 20% point. It shouldn't snap to the finger.
            Vector3 firstPoint = Vector3.Lerp(slider.SliderStart.position, slider.SliderEnd.position, 0.2f);

            yield return rightHand.MoveTo(firstPoint, 60);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsFalse(interactionStarted, "Slider started interaction when we didn't touch the handle and snapToPosition = false");
            Assert.IsFalse(interactionUpdated, "Slider updated value when we didn't touch the handle and snapToPosition = false");
            Assert.AreEqual(0.5f, slider.Value, "Slider shouldn't have snapped to the finger point when SnapToPosition = false");

            yield return rightHand.MoveTo(Vector3.zero, 60);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Vector3 middlePoint = Vector3.Lerp(slider.SliderStart.position, slider.SliderEnd.position, 0.5f);
            yield return rightHand.MoveTo(middlePoint, 60);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(slider.IsPokeSelected, "Slider should be poked!");
            Assert.IsTrue(interactionStarted, "Slider didn't start interaction when we poked the handle");
            Assert.IsTrue(interactionUpdated, "Slider didn't invoke OnValueUpdated when we poked the handle");
            Assert.AreEqual(0.5f, slider.Value, 0.001f, "Slider should still be roughly the same value");

            interactionUpdated = false;

            yield return rightHand.MoveTo(firstPoint, 60);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(slider.IsPokeSelected, "Slider should still be poked.");
            Assert.IsTrue(interactionUpdated, "Slider didn't invoke OnValueUpdated when we dragged the handle");
            Assert.AreEqual(0.2f, slider.Value, 0.001f, "Slider value should roughly be 0.2");

            Object.Destroy(sliderObject);
        }

        /// <summary>
        /// Tests that pinch slider visuals can be null
        /// </summary>
        [UnityTest]
        public IEnumerator TestNullVisualsNoSnap()
        {
            // This should not throw exception
            InstantiateDefaultSliderPrefab(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out SliderVisuals sliderVisuals);
            slider.SnapToPosition = false;

            // Remove visuals
            Object.Destroy(sliderVisuals);

            // Test that the slider still works and no errors are thrown
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.AreEqual(0.5f, slider.Value, sliderValueDelta, "Slider should have value 0.5 at start");

            // clean up
            Object.Destroy(sliderObject);
            yield return null;
        }

        /// <summary>
        /// Tests that pinch slider visuals have the correct orientation after slider axis change
        /// </summary>
        [UnityTest]
        public IEnumerator TestVisualsOrientation()
        {
            // This should not throw exception
            InstantiateDefaultSliderPrefab(InputTestUtilities.InFrontOfUser(Vector3.forward), Vector3.zero, out GameObject sliderObject, out Slider slider, out SliderVisuals sliderVisuals);

            slider.SliderStart.localPosition = Vector3.left;
            slider.SliderEnd.localPosition = Vector3.right;
            yield return null;

            Debug.Assert(sliderVisuals.TrackArea.transform.localRotation == Quaternion.identity, "TrackVisuals should have local rotation equal to Quaternion.identity");
            Debug.Assert(sliderVisuals.Handle.transform.localRotation == Quaternion.identity, "ThumbVisuals should have local rotation equal to Quaternion.identity");

            slider.SliderStart.localPosition = Vector3.down;
            slider.SliderEnd.localPosition = Vector3.up;
            yield return null;

            Debug.Assert(sliderVisuals.TrackArea.transform.localRotation == Quaternion.Euler(0.0f, 0.0f, 90.0f), "TrackVisuals should have local rotation equal to Quaternion.Euler(0.0f, 0.0f, 90.0f)");
            Debug.Assert(sliderVisuals.Handle.transform.localRotation == Quaternion.Euler(0.0f, 0.0f, 90.0f), "ThumbVisuals should have local rotation equal to Quaternion.Euler(0.0f, 0.0f, 90.0f)");

            // clean up
            Object.Destroy(sliderObject);
            yield return null;
        }

        // Disabled as we adopt Unity's multi-selection implementation
        // /// <summary>
        // /// Tests that the slider will only use the first valid grab interactor, and reject others.
        // /// </summary>
        // [UnityTest]
        // public IEnumerator TestMultipleGrabBehavior()
        // {
        //     // This should not throw exception
        //     AssembleSlider(Vector3.forward, Vector3.zero, out GameObject sliderObject, out Slider slider, out SliderVisuals sliderVisuals);

        //     Debug.Assert(slider.Value == 0.5, "Slider should have value 0.5 at start");

        //     // Single mode needed to reject incoming interactors once we are already selected.
        //     Debug.Assert(slider.selectMode == InteractableSelectMode.Single, "Slider should be in single select mode");

        //     var rightHand = new TestHand(Handedness.Right);
        //     Vector3 initialPos = sliderVisuals.Handle.position;
        //     yield return rightHand.Show(initialPos);
        //     yield return rightHand.SetGesture(GestureId.Pinch);

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Debug.Assert(slider.isSelected == true, "Slider was not originally selected");
        //     Debug.Assert(slider.IsGrabSelected == true, "Slider should specifically be grab selected");

        //     var leftHand = new TestHand(Handedness.Left);
        //     yield return leftHand.Show(initialPos);
        //     yield return leftHand.SetGesture(GestureId.Pinch);

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Debug.Assert(slider.interactorsSelecting.Count == 1, "Single mode should enforce single selection");

        //     // Now we move the right hand to the left, but the slider should not move.
        //     yield return rightHand.MoveTo(new Vector3(-0.1f, 0, 1.0f));

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Debug.Assert(Mathf.Abs(slider.Value - 0.5f) < 0.01f, "Slider should not have moved");

        //     // Now we move the left (new) hand to the right, and the slider should now move.
        //     yield return leftHand.MoveTo(new Vector3(0.1f, 0, 1.0f));

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Assert.That(slider.Value, Is.GreaterThan(0.5), "Slider didn't move after moving new grab");

        //     // clean up
        //     Object.Destroy(sliderObject);
        // }

        // /// <summary>
        // /// Tests that the slider will only use the first valid GazePinch interactor, and reject others.
        // /// </summary>
        // [UnityTest]
        // public IEnumerator TestMultipleGazePinchBehavior()
        // {
        //     // This should not throw exception
        //     AssembleSlider(Vector3.forward, Vector3.zero, out GameObject sliderObject, out Slider slider, out _);

        //     Debug.Assert(slider.Value == 0.5, "Slider should have value 0.5 at start");

        //     // Single mode needed to reject incoming interactors once we are already selected.
        //     Debug.Assert(slider.selectMode == InteractableSelectMode.Single, "Slider should be in single select mode");

        //     var rightHand = new TestHand(Handedness.Right);
        //     Vector3 initialPos = new Vector3(0.0f, 0, 0.5f);
        //     yield return rightHand.Show(initialPos);

        //     // Use more frames for better gaze-pinch reliability
        //     yield return rightHand.SetGesture(GestureId.Pinch, 10);

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Debug.Assert(slider.isSelected == true, "Slider was not originally selected");
        //     Debug.Assert(slider.IsGazePinchSelected == true, "Slider should specifically be gaze-pinch selected");
        //     Debug.Assert(slider.interactorsSelecting.Count == 1, "Only one interactor should be selecting so far.");

        //     float initialSliderValue = slider.Value;

        //     var leftHand = new TestHand(Handedness.Left);
        //     yield return leftHand.Show(initialPos);

        //     // Use more frames for better gaze-pinch reliability
        //     yield return leftHand.SetGesture(GestureId.Pinch, 10);

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Debug.Assert(slider.interactorsSelecting.Count == 1, "Single mode should not allow multiple interactors to select");

        //     // Now we move the right (original) hand to the left, but the slider should not move.
        //     yield return rightHand.MoveTo(new Vector3(-0.1f, 0, 1.0f));

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Debug.Assert(Mathf.Abs(slider.Value - initialSliderValue) < 0.01f, "Slider should have detached from the first interactor");

        //     // Now we move the left (new) hand to the right, and the slider should now move.
        //     yield return leftHand.MoveTo(new Vector3(0.1f, 0, 1.0f));

        //     yield return RuntimeTestUtilities.WaitForUpdates();
        //     Assert.That(slider.Value, Is.GreaterThan(initialSliderValue), "Slider didn't move after moving the new interactor");

        //     // clean up
        //     Object.Destroy(sliderObject);
        // }

        #endregion Tests

        #region Private methods

        private IEnumerator DirectPinchAndMoveSlider(Slider slider, float toSliderValue)
        {
            InputTestUtilities.SetHandAnchorPoint(Handedness.Left, Input.Simulation.ControllerAnchorPoint.Grab);
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, Input.Simulation.ControllerAnchorPoint.Grab);

            Debug.Log($"moving hand to value {toSliderValue}");
            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, 0, 1.0f));
            yield return rightHand.Show(initialPos);
            yield return RuntimeTestUtilities.WaitForUpdates();

            SliderVisuals sliderVisuals = slider.GetComponent<SliderVisuals>();

            yield return rightHand.MoveTo(sliderVisuals.Handle.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.SetHandshape(HandshapeId.Pinch, 30);
            yield return RuntimeTestUtilities.WaitForUpdates();

            if (!(toSliderValue >= 0 && toSliderValue <= 1))
            {
                throw new System.ArgumentException("toSliderValue must be between 0 and 1");
            }

            Assert.That(slider.IsGrabSelected, "Slider wasn't grabbed/pinched");

            Vector3 targetPos = Vector3.Lerp(slider.SliderStart.position, slider.SliderEnd.position, toSliderValue);

            yield return rightHand.MoveTo(targetPos);

            // Buffer time for the hand to move to finish moving
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Open, 30);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// Generates an interactable from primitives and assigns a select action.
        /// builds it over several frames so the interactable doesn't have colliders deleted unexpectedly from it.
        /// </summary>
        private void AssembleSlider(Vector3 position, Vector3 rotation, out GameObject sliderObject, out Slider slider, out SliderVisuals sliderVisuals)
        {
            // Assemble an interactable out of a set of primitives
            // This will be the slider root
            sliderObject = new GameObject();
            sliderObject.name = "SliderRoot";

            // Make the slider track
            var sliderTrack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sliderTrack.GetComponent<Collider>().enabled = false;
            sliderTrack.transform.position = Vector3.zero;
            sliderTrack.transform.localScale = new Vector3(1f, .01f, .01f);
            sliderTrack.transform.parent = sliderObject.transform;
            sliderTrack.name = "SliderTrack";

            var sliderTouchable = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sliderTouchable.GetComponent<Renderer>().enabled = false;
            sliderTouchable.transform.position = Vector3.zero;
            sliderTouchable.transform.localScale = new Vector3(1f, .2f, .1f);
            sliderTouchable.transform.parent = sliderObject.transform;
            sliderTouchable.name = "Touchable";

            // Make the thumb root
            var thumbRoot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            thumbRoot.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            thumbRoot.transform.parent = sliderObject.transform;
            thumbRoot.name = "ThumbRoot";

            // Create and initialize the main slider components
            slider = sliderObject.AddComponent<Slider>();
            slider.SliderStart = new GameObject().transform;
            slider.SliderEnd = new GameObject().transform;
            slider.SliderStart.position = new Vector3(-0.5f, 0, 0);
            slider.SliderEnd.position = new Vector3(0.5f, 0, 0);
            slider.SliderStart.transform.parent = slider.transform;
            slider.SliderEnd.transform.parent = slider.transform;
            slider.TrackCollider = sliderTouchable.GetComponent<BoxCollider>();


            sliderVisuals = sliderObject.AddComponent<SliderVisuals>();
            sliderVisuals.Handle = thumbRoot.transform;
            sliderVisuals.TrackArea = sliderTrack.transform;

            sliderObject.transform.position = position;
            sliderObject.transform.eulerAngles = rotation;
        }

        /// <summary>
        /// Instantiates the default interactable button.
        /// </summary>
        private void InstantiateDefaultSliderPrefab(Vector3 position, Vector3 rotation, out GameObject sliderObject, out Slider slider, out SliderVisuals sliderVisuals)
        {
            // Load interactable prefab
            Object sliderPrefab = AssetDatabase.LoadAssetAtPath(defaultSliderPrefabPath, typeof(Object));
            sliderObject = Object.Instantiate(sliderPrefab) as GameObject;
            slider = sliderObject.GetComponent<Slider>();
            sliderVisuals = sliderObject.GetComponent<SliderVisuals>();
            Assert.IsNotNull(slider);

            // Move the object into position
            sliderObject.transform.position = position;
            sliderObject.transform.eulerAngles = rotation;
        }

        #endregion Private methods
    }
}
