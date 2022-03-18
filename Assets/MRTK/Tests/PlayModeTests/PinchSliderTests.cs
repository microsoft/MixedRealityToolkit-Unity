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
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class PinchSliderTests : BasePlayModeTests
    {
        // SDK/Features/UX/Prefabs/Sliders/PinchSlider.prefab
        private const string defaultPinchSliderPrefabGuid = "1093263a89abe47499cccf7dcb08effb";
        private static readonly string defaultPinchSliderPrefabPath = AssetDatabase.GUIDToAssetPath(defaultPinchSliderPrefabGuid);

        public override IEnumerator Setup()
        {
            yield return base.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
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
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            // clean up
            GameObject.Destroy(pinchSliderObject);
            yield return null;
        }

        /// <summary>
        /// Tests that an interactable assembled at runtime can be manipulated
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleInteractableAndNearManip()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            Assert.AreEqual(0.5f, slider.SliderValue, 1.0e-5f, "Slider should have value 0.5 at start");
            yield return DirectPinchAndMoveSlider(slider, 1.0f);
            Assert.AreEqual(1.0f, slider.SliderValue, 1.0e-5f, "Slider should have value 1.0 after being manipulated at start");

            // clean up
            GameObject.Destroy(pinchSliderObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestLoadPrefabAndNearManipNoSnap()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            InstantiateDefaultSliderPrefab(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);
            slider.SnapToPosition = false;

            Assert.AreEqual(0.5f, slider.SliderValue, 1.0e-5f, "Slider should have value 0.5 at start");
            yield return DirectPinchAndMoveSlider(slider, 1.0f);
            Assert.AreEqual(1.0f, slider.SliderValue, 1.0e-5f, "Slider should have value 1.0 after being manipulated at start");

            // clean up
            GameObject.Destroy(pinchSliderObject);
            yield return null;
        }

        /// <summary>
        /// Tests that slider can be assembled from code and manipulated using GGV
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleInteractableAndFarManip()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            Debug.Assert(slider.SliderValue == 0.5, "Slider should have value 0.5 at start");

            // Set up ggv simulation
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = new Vector3(0.05f, 0, 1.0f);
            yield return rightHand.Show(initialPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            yield return rightHand.Move(new Vector3(0.1f, 0, 0));
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Hide();

            Assert.That(slider.SliderValue, Is.GreaterThan(0.5));

            // clean up
            GameObject.Destroy(pinchSliderObject);
            iss.ControllerSimulationMode = oldSimMode;
        }

        /// <summary>
        /// Tests that interactable raises proper events
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleInteractableAndEventsRaised()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = new Vector3(0.05f, 0, 1.0f);

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            bool interactionStarted = false;
            slider.OnInteractionStarted.AddListener((x) => interactionStarted = true);
            yield return rightHand.Show(initialPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");

            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            yield return rightHand.Move(new Vector3(0.1f, 0, 0));

            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            bool interactionEnded = false;
            slider.OnInteractionEnded.AddListener((x) => interactionEnded = true);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Hide();

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            Assert.That(slider.SliderValue, Is.GreaterThan(0.5));

            GameObject.Destroy(pinchSliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when using step divisions
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleStepPinchSlider()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            // Set the slider to use step divisions
            slider.UseSliderStepDivisions = true;
            slider.SliderStepDivisions = 4;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = new Vector3(0.05f, 0, 1.0f);

            bool interactionStarted = false;
            slider.OnInteractionStarted.AddListener((x) => interactionStarted = true);
            yield return rightHand.Show(initialPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");

            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            yield return rightHand.Move(new Vector3(0.3f, 0, 0));

            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            bool interactionEnded = false;
            slider.OnInteractionEnded.AddListener((x) => interactionEnded = true);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            Assert.AreEqual(0.75f, slider.SliderValue);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.Move(new Vector3(-0.6f, 0, 0));
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            yield return rightHand.Hide();

            Assert.AreEqual(0.25f, slider.SliderValue);

            GameObject.Destroy(pinchSliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when snapping to a position
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleSnapSlider()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            // Set the slider to use step divisions
            slider.UseSliderStepDivisions = true;
            slider.SliderStepDivisions = 4;

            // Set slider to snap without touch
            slider.SnapToPosition = true;
            slider.IsTouchable = false;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            bool interactionStarted = false;
            slider.OnInteractionStarted.AddListener((x) => interactionStarted = true);
            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = new Vector3(0.3f, 0, 0.0f);
            yield return rightHand.Show(initialPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Move(new Vector3(0, 0, 1.0f));
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.IsFalse(interactionStarted, "Slider raised interaction started early.");
            Assert.IsFalse(interactionUpdated, "Slider raised SliderUpdated event early.");

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");
            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            Assert.AreEqual(0.75f, slider.SliderValue);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Move(new Vector3(-0.6f, 0, 0));
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(0.25f, slider.SliderValue);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Move(new Vector3(0.3f, 0, 0));
            Assert.AreEqual(0.25f, slider.SliderValue);

            bool interactionEnded = false;
            slider.OnInteractionEnded.AddListener((x) => interactionEnded = true);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Hide();

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            GameObject.Destroy(pinchSliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when snapping to a position
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleTouchSnapSlider()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            // Set the slider to use step divisions
            slider.UseSliderStepDivisions = true;
            slider.SliderStepDivisions = 4;

            // Set slider to snap with touch
            slider.SnapToPosition = true;
            slider.IsTouchable = true;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            bool interactionStarted = false;
            slider.OnInteractionStarted.AddListener((x) => interactionStarted = true);
            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = new Vector3(0.3f, 0, 0.0f);
            yield return rightHand.Show(initialPos);
            yield return rightHand.Move(new Vector3(0, 0, 1.0f));

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");
            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            Assert.AreEqual(0.75f, slider.SliderValue);

            yield return rightHand.Move(new Vector3(-0.6f, 0, 0));
            Assert.AreEqual(0.25f, slider.SliderValue);

            yield return rightHand.Move(new Vector3(0.3f, 0, 0));
            Assert.AreEqual(0.5f, slider.SliderValue);

            bool interactionEnded = false;
            slider.OnInteractionEnded.AddListener((x) => interactionEnded = true);

            yield return rightHand.Hide();

            Assert.IsTrue(interactionEnded, "Slider did not raise interaction ended.");

            GameObject.Destroy(pinchSliderObject);
        }

        /// <summary>
        /// Tests that interactable has the correct values when snapping to a position
        /// </summary>
        [UnityTest]
        public IEnumerator TestAssembleTouchNoSnapSlider()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            AssembleSlider(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            // Set the slider to use step divisions
            slider.UseSliderStepDivisions = true;
            slider.SliderStepDivisions = 100;

            // Set slider to not snap with touch
            slider.SnapToPosition = false;
            slider.IsTouchable = true;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            bool interactionStarted = false;
            slider.OnInteractionStarted.AddListener((x) => interactionStarted = true);
            bool interactionUpdated = false;
            slider.OnValueUpdated.AddListener((x) => interactionUpdated = true);

            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = new Vector3(0.0f, 0, 1.00f);

            yield return rightHand.Show(initialPos);

            yield return rightHand.Move(new Vector3(0.3f, 0, 0), 60);
            Assert.IsTrue(interactionStarted, "Slider did not raise interaction started.");
            Assert.IsTrue(interactionUpdated, "Slider did not raise SliderUpdated event.");

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            yield return rightHand.Move(new Vector3(-0.6f, 0, 0), 60);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            yield return rightHand.Hide();

            Assert.AreEqual(0.20f, slider.SliderValue, 0.005f);

            GameObject.Destroy(pinchSliderObject);
        }

        /// <summary>
        /// Tests that pinch slider visuals can be null
        /// </summary>
        [UnityTest]
        public IEnumerator TestNullVisualsNoSnap()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            InstantiateDefaultSliderPrefab(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);
            slider.SnapToPosition = false;

            // Remove references to visuals
            slider.TrackVisuals = null;
            slider.TickMarks = null;
            slider.ThumbVisuals = null;

            // Test that the slider still works
            Assert.AreEqual(0.5f, slider.SliderValue, 1.0e-5f, "Slider should have value 0.5 at start");
            yield return DirectPinchAndMoveSlider(slider, 1.0f);
            Assert.AreEqual(1.0f, slider.SliderValue, 1.0e-5f, "Slider should have value 1.0 after being manipulated at start");

            // clean up
            GameObject.Destroy(pinchSliderObject);
            yield return null;
        }

        /// <summary>
        /// Tests that pinch slider visuals have the correct orientation after slider axis change
        /// </summary>
        [UnityTest]
        public IEnumerator TestVisualsOrientation()
        {
            GameObject pinchSliderObject;
            PinchSlider slider;

            // This should not throw exception
            InstantiateDefaultSliderPrefab(Vector3.forward, Vector3.zero, out pinchSliderObject, out slider);

            var tickMarks = slider.TickMarks;
            var trackVisuals = slider.TrackVisuals;
            var thumbVisuals = slider.ThumbVisuals;

            slider.CurrentSliderAxis = SliderAxis.XAxis;

            yield return null;

            if (trackVisuals)
            {
                Debug.Assert(trackVisuals.transform.localRotation == Quaternion.identity, "TrackVisuals should have local rotation equal to Quaternion.identity");
            }

            if (tickMarks)
            {
                Debug.Assert(tickMarks.transform.localRotation == Quaternion.identity, "TickMarks should have local rotation equal to Quaternion.identity");
                Debug.Assert(tickMarks.GetComponent<GridObjectCollection>().Layout == LayoutOrder.Horizontal, "TickMarks GridObjectCollection Layout should be Horizontal");
            }

            if (thumbVisuals)
            {
                Debug.Assert(thumbVisuals.transform.localRotation == Quaternion.identity, "ThumbVisuals should have local rotation equal to Quaternion.identity");
            }

            slider.CurrentSliderAxis = SliderAxis.YAxis;

            yield return null;

            if (trackVisuals)
            {
                Debug.Assert(trackVisuals.transform.localRotation == Quaternion.Euler(0.0f, 0.0f, 90.0f), "TrackVisuals should have local rotation equal to Quaternion.Euler(0.0f, 0.0f, 90.0f)");
            }

            if (tickMarks)
            {
                Debug.Assert(tickMarks.transform.localRotation == Quaternion.identity, "TickMarks should have local rotation equal to Quaternion.identity");
                Debug.Assert(tickMarks.GetComponent<GridObjectCollection>().Layout == LayoutOrder.Vertical, "TickMarks GridObjectCollection Layout should be Vertical");
            }

            if (thumbVisuals)
            {
                Debug.Assert(thumbVisuals.transform.localRotation == Quaternion.Euler(0.0f, 0.0f, 90.0f), "ThumbVisuals should have local rotation equal to Quaternion.Euler(0.0f, 0.0f, 90.0f)");
            }

            slider.CurrentSliderAxis = SliderAxis.ZAxis;

            yield return null;

            if (trackVisuals)
            {
                Debug.Assert(trackVisuals.transform.localRotation == Quaternion.Euler(0.0f, 90.0f, 0.0f), "TrackVisuals should have local rotation equal to Quaternion.Euler(0.0f, 90.0f, 0.0f)");
            }

            if (tickMarks)
            {
                Debug.Assert(tickMarks.transform.localRotation == Quaternion.Euler(0.0f, 90.0f, 0.0f), "TickMarks should have local rotation equal to Quaternion.Euler(0.0f, 90.0f, 0.0f)");
                Debug.Assert(tickMarks.GetComponent<GridObjectCollection>().Layout == LayoutOrder.Horizontal, "TickMarks GridObjectCollection Layout should be Horizontal");
            }

            if (thumbVisuals)
            {
                Debug.Assert(thumbVisuals.transform.localRotation == Quaternion.Euler(0.0f, 90.0f, 0.0f), "ThumbVisuals should have local rotation equal to Quaternion.Euler(0.0f, 90.0f, 0.0f)");
            }

            // clean up
            GameObject.Destroy(pinchSliderObject);
            yield return null;
        }

        #endregion Tests

        #region Private methods

        private IEnumerator DirectPinchAndMoveSlider(PinchSlider slider, float toSliderValue)
        {
            Debug.Log($"moving hand to value {toSliderValue}");
            var rightHand = new TestHand(Handedness.Right);
            Vector3 initialPos = new Vector3(0.05f, 0, 1.0f);
            yield return rightHand.Show(initialPos);
            yield return rightHand.MoveTo(slider.ThumbRoot.transform.position);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            if (!(toSliderValue >= 0 && toSliderValue <= 1))
            {
                throw new System.ArgumentException("toSliderValue must be between 0 and 1");
            }

            yield return rightHand.MoveTo(Vector3.Lerp(slider.SliderStartPosition, slider.SliderEndPosition, toSliderValue));
            // Buffer time for the hand to move to finish moving
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Hide();
        }

        /// <summary>
        /// Generates an interactable from primitives and assigns a select action.
        /// </summary>
        private void AssembleSlider(Vector3 position, Vector3 rotation, out GameObject pinchSliderObject, out PinchSlider slider)
        {
            // Assemble an interactable out of a set of primitives
            // This will be the slider root
            pinchSliderObject = new GameObject();
            pinchSliderObject.name = "PinchSliderRoot";

            // Make the slider track
            var sliderTrack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(sliderTrack.GetComponent<BoxCollider>());
            sliderTrack.transform.position = Vector3.zero;
            sliderTrack.transform.localScale = new Vector3(1f, .01f, .01f);
            sliderTrack.transform.parent = pinchSliderObject.transform;

            var sliderTouchable = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sliderTouchable.GetComponent<Renderer>().enabled = false;
            sliderTouchable.transform.position = Vector3.zero;
            sliderTouchable.transform.localScale = new Vector3(1f, .2f, .1f);
            sliderTouchable.AddComponent<NearInteractionGrabbable>();
            sliderTouchable.AddComponent<NearInteractionTouchable>();
            sliderTouchable.transform.parent = pinchSliderObject.transform;


            // Make the thumb root
            var thumbRoot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            thumbRoot.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            thumbRoot.AddComponent<NearInteractionGrabbable>();
            thumbRoot.AddComponent<NearInteractionTouchable>();
            thumbRoot.transform.parent = pinchSliderObject.transform;


            // Create and initialize the collider
            slider = pinchSliderObject.AddComponent<PinchSlider>();
            slider.ThumbRoot = thumbRoot;
            slider.ThumbCollider = thumbRoot.GetComponent<Collider>();
            slider.TouchCollider = sliderTouchable.GetComponent<Collider>();

            pinchSliderObject.transform.position = position;
            pinchSliderObject.transform.eulerAngles = rotation;
        }

        /// <summary>
        /// Instantiates the default interactable button.
        /// </summary>
        private void InstantiateDefaultSliderPrefab(Vector3 position, Vector3 rotation, out GameObject sliderObject, out PinchSlider pinchSlider)
        {
            // Load interactable prefab
            Object sliderPrefab = AssetDatabase.LoadAssetAtPath(defaultPinchSliderPrefabPath, typeof(Object));
            sliderObject = Object.Instantiate(sliderPrefab) as GameObject;
            pinchSlider = sliderObject.GetComponent<PinchSlider>();
            Assert.IsNotNull(pinchSlider);

            // Move the object into position
            sliderObject.transform.position = position;
            sliderObject.transform.eulerAngles = rotation;
        }
        #endregion Private methods
    }
}
#endif