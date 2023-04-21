// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Runtime.Tests
{
    /// <summary>
    /// Tests for verifying translation, rotation, and scale applied through attachTransform property of an Interactor.
    /// </summary>
    public class TransformViaInteractorTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Scale an ObjectManipulator by scaling the attachTransform property of an Interactor. Transform constraints
        /// applied to target object should be respected.
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleObjectTest()
        {
            const float DELTA = 0.01f;

            // Create cube with ObjectManipulator
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one;
            testObject.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1.0f));
            yield return RuntimeTestUtilities.WaitForUpdates();
            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.SmoothingFar = false; // by default scale changes have smoothing but disabling makes testing quicker

            // Confirm that our gaze is hovering on the cube
            int hoverEnterCount = 0;
            objectManipulator.hoverEntered.AddListener((eventData) => hoverEnterCount++);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.AreEqual(1, hoverEnterCount, $"ObjectManipulator did not receive hover enter event, count is {hoverEnterCount}");

            // Select the cube with a pinch
            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.0f, 0.3f));
            TestHand hand = new TestHand(Handedness.Right);

            yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();

            int selectCount = 0;
            objectManipulator.selectEntered.AddListener((eventData) => selectCount++);
            objectManipulator.selectExited.AddListener((eventData) => selectCount--);
            Assert.AreEqual(0, selectCount, $"ObjectManipulator is not selected, because we haven't pinched yet");

            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.AreEqual(1, selectCount, $"ObjectManipulator is selected");

            // Change the attachTransform of the selecting Interactor
            var rayInteractor = (MRTKRayInteractor)objectManipulator.firstInteractorSelecting;

            rayInteractor.attachTransform.localScale = Vector3.one * 20f;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                rayInteractor.attachTransform.localScale,
                testObject.transform.localScale,
                DELTA,
                "With no constraints, target should match attachTransform scale.");

            rayInteractor.attachTransform.localScale = new Vector3(0.2f, 11.11f, 2.5f);
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                rayInteractor.attachTransform.localScale,
                testObject.transform.localScale,
                DELTA,
                "With no constraints, target should match attachTransform scale.");

            rayInteractor.attachTransform.localScale = Vector3.one;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                rayInteractor.attachTransform.localScale,
                testObject.transform.localScale,
                DELTA,
                "With no constraints, target should match attachTransform scale.");

            GameObject.Destroy(testObject);

            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// Scale an ObjectManipulator by scaling the attachTransform property of an Interactor. Transform constraints
        /// applied to target object should be respected.
        /// </summary>
        [UnityTest]
        public IEnumerator NonUniformConstraintsScaleTest()
        {
            const float DELTA = 0.01f;

            // Create cube with ObjectManipulator
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var initialLocalScale = Vector3.one;
            testObject.transform.localScale = initialLocalScale;
            testObject.transform.position = new Vector3(0, 0, 1.0f);
            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.SmoothingFar = false; // by default scale changes have smoothing but disabling makes testing quicker

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Select the cube with a pinch
            Vector3 initialHandPosition = new Vector3(0.1f, 0.0f, 0.3f);
            TestHand hand = new TestHand(Handedness.Right);

            yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // non-uniform constraints with uniform scaling
            var scaleConstraint = testObject.AddComponent<MinMaxScaleConstraint>();
            scaleConstraint.MaximumScale = new Vector3(1f, 2f, 3f);

            // Change the attachTransform of the selecting Interactor
            var rayInteractor = (MRTKRayInteractor)objectManipulator.firstInteractorSelecting;
            rayInteractor.attachTransform.localScale = Vector3.one * 4f;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                scaleConstraint.MaximumScale,
                testObject.transform.localScale,
                DELTA,
                "Target should stay within constraint limits.");

            // Returning from a scale that was distorted because of constraints should
            // restore the original shape as long as it is part of the same gesture.
            // As a metaphor, think of a balloon inflated in a box until it becomes a cube
            // shape and then is deflated until it returns to balloon shape.
            rayInteractor.attachTransform.localScale = Vector3.one;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                rayInteractor.attachTransform.localScale,
                testObject.transform.localScale,
                DELTA,
                "Since we started at 1,1,1 as we are within the same gesture we will return to 1,1,1.");

            // non-uniform constraints with uniform scaling
            scaleConstraint.MaximumScale = new Vector3(1.3f, 1.2f, 1.0f);
            rayInteractor.attachTransform.localScale = Vector3.one * 2f;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                scaleConstraint.MaximumScale,
                testObject.transform.localScale,
                DELTA,
                "Target should stay within constraint limits.");

            // This time, we are going to release the selection and start a new
            // selection when the target it distorted by the constraints.

            yield return hand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // center the object if it moved due to scaling
            testObject.transform.position = new Vector3(0, 0, 1f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // reset attachTransform (many use cases will reset for each gesture)
            rayInteractor.attachTransform.localScale = Vector3.one;

            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Because the scale was already non-uniform when the gesture started,
            // it will stay non-uniform as it scales (unless distorted by additional
            // constraints).
            rayInteractor.attachTransform.localScale = Vector3.one * 0.5f;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                scaleConstraint.MaximumScale * 0.5f,
                testObject.transform.localScale,
                DELTA,
                "Target should stay within constraint limits.");

            GameObject.Destroy(testObject);

            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// Scale an ObjectManipulator by scaling the attachTransform property of an Interactor. Transform constraints
        /// applied to target object should be respected.
        /// </summary>
        [UnityTest]
        public IEnumerator NonUniformObjectScaleTest()
        {
            const float DELTA = 0.01f;

            // Create cube with ObjectManipulator
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var initialLocalScale = new Vector3(1.5f, 1f, 0.9f); // non-uniform scale
            testObject.transform.localScale = initialLocalScale;
            testObject.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0, 0, 1.0f));
            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.SmoothingFar = false; // by default scale changes have smoothing but disabling makes testing quicker
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Select the cube with a pinch
            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.3f, 0.0f, 0.3f));
            TestHand hand = new TestHand(Handedness.Right);

            yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Change the attachTransform of the selecting Interactor
            var rayInteractor = (MRTKRayInteractor)objectManipulator.firstInteractorSelecting;

            rayInteractor.attachTransform.localScale = Vector3.one * 2f;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                initialLocalScale * 2f, // object keeps aspect ratio
                testObject.transform.localScale,
                DELTA,
                "With no constraints, target should match attachTransform scale.");

            GameObject.Destroy(testObject);

            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        /// <summary>
        /// Scale an ObjectManipulator by scaling the attachTransform property of an Interactor. Transform constraints
        /// applied to target object should be respected.
        /// </summary>
        [UnityTest]
        public IEnumerator TargetWithNonDefaultStartingScaleTest()
        {
            const float DELTA = 0.01f;

            // Create cube with ObjectManipulator
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var initialLocalScale = Vector3.one;
            testObject.transform.localScale = initialLocalScale;
            testObject.transform.position = InputTestUtilities.InFrontOfUser(1.0f);
            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.SmoothingFar = false; // by default scale changes have smoothing but disabling makes testing quicker
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Select the cube with a pinch
            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.0f, 0.3f));
            TestHand hand = new TestHand(Handedness.Right);


            yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Get the interactor
            var rayInteractor = (MRTKRayInteractor)objectManipulator.firstInteractorSelecting;

            yield return hand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Scaling the attachTransform when there is no target
            rayInteractor.attachTransform.localScale = Vector3.one * 2f;
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Select cube again (attachTransform starts at 200%)
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Scaling attachTransform to one should scale down the cube
            rayInteractor.attachTransform.localScale = Vector3.one;
            yield return RuntimeTestUtilities.WaitForUpdates();
            AssertScalesEqual(
                Vector3.one * 0.5f,
                testObject.transform.localScale,
                DELTA,
                "With no constraints, target should match attachTransform scale.");

            GameObject.Destroy(testObject);

            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        public void AssertScalesEqual(Vector3 scale1, Vector3 scale2, float delta, String message)
        {
            Assert.AreEqual(scale1.x, scale2.x, delta, message);
            Assert.AreEqual(scale1.y, scale2.y, delta, message);
            Assert.AreEqual(scale1.z, scale2.z, delta, message);
        }
    }
}


