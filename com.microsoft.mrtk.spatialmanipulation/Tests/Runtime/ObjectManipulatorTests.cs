// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using Microsoft.MixedReality.Toolkit.Input.Simulation;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Runtime.Tests
{
    /// <summary>
    /// Tests for ObjectManipulator
    /// </summary>
    public class ObjectManipulatorTests : BaseRuntimeInputTests
    {

        /// <summary>
        /// Verifies that creating an ObjectManipulator at runtime properly
        /// respects the various interactor filtering/interaction type rules.
        /// </summary>
        [UnityTest]
        public IEnumerator TestObjManipInteractorRules()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<ObjectManipulator>();
            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.1f, 1));
            cube.transform.localScale = Vector3.one * 0.2f;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify that a ConstraintManager was automatically added.
            Assert.IsTrue(cube.GetComponent<ConstraintManager>() != null, "Runtime-spawned ObjManip didn't also spawn ConstraintManager");

            ObjectManipulator objManip = cube.GetComponent<ObjectManipulator>();

            Assert.IsTrue(objManip.AllowedInteractionTypes == (InteractionFlags.Near | InteractionFlags.Ray | InteractionFlags.Gaze | InteractionFlags.Generic),
                "ObjManip started out with incorrect AllowedInteractionTypes");

            Assert.IsTrue(objManip.AllowedManipulations == (TransformFlags.Move | TransformFlags.Rotate | TransformFlags.Scale),
                "ObjManip started out with incorrect AllowedManipulations");

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objManip.IsPokeHovered, "ObjManip shouldn't get IsPokeHovered");
            Assert.IsTrue(objManip.IsGrabHovered, "ObjManip didn't report IsGrabHovered");

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(objManip.IsGrabSelected, "ObjManip didn't report IsGrabSelected");
            Assert.IsFalse(objManip.IsPokeSelected, "ObjManip was PokeSelected. Should not be possible.");

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objManip.isSelected, "ObjManip didn't de-select.");

            objManip.AllowedInteractionTypes = InteractionFlags.Ray | InteractionFlags.Gaze;
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objManip.IsGrabSelected, "ObjManip was still grab selected after removing Near from AllowedInteractionTypes.");
            Assert.IsFalse(objManip.IsPokeSelected, "ObjManip was PokeSelected. Should not be possible.");

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // We don't have full gaze support in sim yet, so this is an approximation.
            // Set cube's position to straight ahead.
            cube.transform.position = InputTestUtilities.InFrontOfUser(1.0f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Put hand out in front, in-FOV, but not too close to cube as to
            // disable the far interactors.
            yield return rightHand.MoveTo(InputTestUtilities.InFrontOfUser(new Vector3(0.2f, 0, 0.5f)));
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(objManip.IsGazePinchHovered,
                "ObjManip didn't report IsGazePinchHovered");

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(objManip.IsGazePinchSelected,
                "ObjManip didn't report IsGazePinchSelected");

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objManip.IsGazePinchSelected,
                "ObjManip was still GazePinchSelected after un-pinching.");

            objManip.AllowedInteractionTypes = InteractionFlags.Ray;
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objManip.IsGazePinchSelected,
                "ObjManip was still GazePinchSelected after removing Near from AllowedInteractionTypes.");
        }

        /// <summary>
        /// Verifies that an ObjectManipulator created at runtime has proper smoothing characteristics.
        /// </summary>
        [UnityTest]
        public IEnumerator TestObjManipSmoothingDrift()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<ObjectManipulator>();
            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.1f, 1));
            cube.transform.localScale = Vector3.one * 0.2f;

            yield return RuntimeTestUtilities.WaitForUpdates();

            ObjectManipulator objManip = cube.GetComponent<ObjectManipulator>();
            objManip.SmoothingNear = false;

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.MoveTo(cube.transform.position);
            yield return new WaitForSeconds(0.1f);

            Assert.IsFalse(objManip.IsPokeHovered, "ObjManip shouldn't get IsPokeHovered");
            Assert.IsTrue(objManip.IsGrabHovered, "ObjManip didn't report IsGrabHovered");

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(objManip.IsGrabSelected, "ObjManip didn't report IsGrabSelected");

            // Enable smoothing for near interaction.
            objManip.SmoothingNear = true;

            // Move the hand to the right.
            Vector3 originalPosition = cube.transform.position;
            Vector3 attachTransform = objManip.firstInteractorSelecting.GetAttachTransform(objManip).position;
            Vector3 originalAttachOffset = attachTransform - originalPosition;

            Vector3 newPosition = originalPosition + Vector3.right * 1.5f;
            yield return rightHand.MoveTo(newPosition);
            yield return new WaitForSeconds(0.1f);

            // Smoothing should mean that the cube has lagged behind the hand.
            attachTransform = objManip.firstInteractorSelecting.GetAttachTransform(objManip).position;
            Vector3 attachOffset = attachTransform - cube.transform.position;
            Assert.IsTrue((attachOffset - originalAttachOffset).magnitude > 0.1f,
                "Smoothing didn't seem to work. Current attachTransform offset should be different than the original, indicating lag.");

            // Wait long enough for the object to catch up.
            yield return new WaitForSeconds(6.0f);
            attachTransform = objManip.firstInteractorSelecting.GetAttachTransform(objManip).position;
            attachOffset = attachTransform - cube.transform.position;
            Assert.IsTrue((attachOffset - originalAttachOffset).magnitude < 0.001f,
                "Cube didn't catch up with the hand after waiting for a bit. Magnitude: " + (attachOffset - originalAttachOffset).magnitude.ToString("F4"));

            // Disable smoothing, to check that it properly sticks to the hand once disabled.
            objManip.SmoothingNear = false;

            newPosition = originalPosition - Vector3.right * 1.5f;
            yield return rightHand.MoveTo(newPosition);
            yield return new WaitForSeconds(0.1f);

            // Immediately check to make sure the cube matches our grab exactly.
            attachTransform = objManip.firstInteractorSelecting.GetAttachTransform(objManip).position;
            attachOffset = attachTransform - cube.transform.position;
            Assert.IsTrue((attachOffset - originalAttachOffset).magnitude < 0.001f,
                "Cube didn't match hand exactly after setting SmoothingNear to false.");
        }

        /// <summary>
        /// Test creating adding a ObjectManipulator to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorInstantiate()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            // Wait for two frames to make sure we don't get null pointer exception.
            yield return null;
            yield return null;

            GameObject.Destroy(testObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test creating ObjectManipulator and receiving hover enter/exit events
        /// from gaze provider.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorGazeHover()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            int hoverEnterCount = 0;
            int hoverExitCount = 0;

            objectManipulator.hoverEntered.AddListener((eventData) => hoverEnterCount++);
            objectManipulator.hoverExited.AddListener((eventData) => hoverExitCount++);

            testObject.transform.position = InputTestUtilities.InFrontOfUser(1.0f);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(1, hoverEnterCount, $"ObjectManipulator did not receive hover enter event, count is {hoverEnterCount}");

            testObject.transform.Translate(Vector3.up);

            // First yield for physics. Second for normal frame step.
            // Without first one, second might happen before translation is applied.
            // Without second one services will not be stepped.
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(1, hoverExitCount, "ObjectManipulator did not receive hover exit event");

            testObject.transform.Translate(5 * Vector3.up);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.IsTrue(hoverExitCount == 1, "ObjectManipulator received the second hover event");

            GameObject.Destroy(testObject);

            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return null;
        }

        #region One Handed Manipulation Tests

        /// <summary>
        /// This tests the one hand near movement while camera (character) is moving around.
        /// The test will check the offset between object pivot and grab point and make sure we're not drifting
        /// out of the object on pointer rotation - this test should be the same in all rotation setups
        /// This test also has a sanity check to ensure behavior is still the same for objects of different scale
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorOneHandMoveNear()
        {
            // Set the anchor point to be the grab position
            InputTestUtilities.SetHandAnchorPoint(Handedness.Left, Input.Simulation.ControllerAnchorPoint.Grab);
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, Input.Simulation.ControllerAnchorPoint.Grab);

            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGazeInteractor();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Vector3 initialObjectPosition = InputTestUtilities.InFrontOfUser(1f);
            testObject.transform.position = initialObjectPosition;
            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = testObject.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;

            yield return RuntimeTestUtilities.WaitForUpdates();

            const int numCircleSteps = 10;

            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(0.5f);
            Vector3 initialGrabPosition = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, -0.05f, 1f)); // grab around the left bottom corner of the cube
            Quaternion initialGrabRotation = Quaternion.identity;
            TestHand hand = new TestHand(Handedness.Right);

            Vector3[] objectScales = new Vector3[] { Vector3.one * 0.2f, new Vector3(0.2f, 0.4f, 0.3f) };

            foreach (var objectScale in objectScales)
            {
                testObject.transform.localScale = objectScale;
                yield return RuntimeTestUtilities.WaitForUpdates();

                // do this test for every one hand rotation mode
                foreach (ObjectManipulator.RotateAnchorType rotationAnchorType in Enum.GetValues(typeof(ObjectManipulator.RotateAnchorType)))
                {
                    objectManipulator.RotationAnchorNear = rotationAnchorType;

                    InputTestUtilities.InitializeCameraToOriginAndForward();

                    yield return hand.Show(initialHandPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.MoveTo(initialGrabPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.RotateTo(initialGrabRotation);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.SetHandshape(HandshapeId.Pinch);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    Assert.IsTrue(objectManipulator.IsGrabSelected, $"ObjectManipulator didn't get grabbed on pinch!");

                    // Ensure the object didn't move after pinching if using object centered rotation
                    // Todo: Re-enable when grab-anchoring no longer has a frame delay. (Will require
                    // updates to synthetic hands subsystem!)
                    // Vector3 initialPosition = testObject.transform.position;
                    // if (rotationAnchorType == ObjectManipulator.RotateAnchorType.RotateAboutObjectCenter)
                    // {
                    //     TestUtilities.AssertAboutEqual(testObject.transform.position, initialPosition, "object shifted during pinch", 0.01f);
                    // }

                    // save relative pos grab point to object
                    // The firstInteractorSelecting is the one that is currently grabbing the object
                    Vector3 initialGrabPoint = objectManipulator.firstInteractorSelecting.GetAttachTransform(objectManipulator).position;
                    Vector3 initialGrabPointInObject = testObject.transform.InverseTransformPoint(initialGrabPoint);
                    Vector3 initialGrabOffset = initialGrabPoint - testObject.transform.position;

                    // full circle
                    const int degreeStep = 360 / numCircleSteps;

                    // rotating the pointer in a circle around "the user"
                    for (int i = 1; i <= numCircleSteps; ++i)
                    {
                        // rotate main camera (user)
                        Vector3 rotationDelta = degreeStep * Vector3.up;
                        InputTestUtilities.RotateCamera(rotationDelta);
                        yield return RuntimeTestUtilities.WaitForUpdates();

                        // move hand with the camera
                        Vector3 newHandPosition = Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialGrabPosition;
                        yield return hand.MoveTo(newHandPosition);
                        yield return RuntimeTestUtilities.WaitForUpdates();
                        yield return hand.RotateTo(Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialGrabRotation);
                        yield return RuntimeTestUtilities.WaitForUpdates();

                        if (rotationAnchorType == ObjectManipulator.RotateAnchorType.RotateAboutObjectCenter)
                        {
                            // make sure that the offset between grab and object centre hasn't changed while rotating
                            Vector3 grabPoint = objectManipulator.firstInteractorSelecting.GetAttachTransform(objectManipulator).position;
                            Vector3 offsetRotated = grabPoint - testObject.transform.position;
                            TestUtilities.AssertAboutEqual(offsetRotated, initialGrabOffset, $"Object offset changed during rotation using {rotationAnchorType}");
                        }
                        else
                        {
                            // make sure that grab point has not changed relative to the object while rotating
                            Vector3 grabPoint = objectManipulator.firstInteractorSelecting.GetAttachTransform(objectManipulator).position;
                            Vector3 grabPointRotated = testObject.transform.InverseTransformPoint(grabPoint);
                            TestUtilities.AssertAboutEqual(grabPointRotated, initialGrabPointInObject, $"Grab point on object changed during rotation using {rotationAnchorType}");
                        }
                    }

                    // Move the object forward and back
                    yield return hand.MoveTo(initialGrabPosition + Vector3.forward * 0.4f);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    // make sure that the offset between grab and object centre hasn't changed while rotating
                    Vector3 currentGrabPoint = objectManipulator.firstInteractorSelecting.GetAttachTransform(objectManipulator).position;
                    Vector3 currentOffset = currentGrabPoint - testObject.transform.position;
                    TestUtilities.AssertAboutEqual(currentOffset, initialGrabOffset, $"Object offset changed during move forward");

                    yield return hand.MoveTo(initialGrabPosition + Vector3.back * 0.4f);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    // make sure that the offset between grab and object centre hasn't changed while moving
                    currentGrabPoint = objectManipulator.firstInteractorSelecting.GetAttachTransform(objectManipulator).position;
                    currentOffset = currentGrabPoint - testObject.transform.position;
                    TestUtilities.AssertAboutEqual(currentOffset, initialGrabOffset, $"Object offset changed during move backward");

                    yield return hand.MoveTo(initialGrabPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    yield return hand.SetHandshape(HandshapeId.Open);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.Hide();
                    yield return RuntimeTestUtilities.WaitForUpdates();
                }
            }
        }

        /// <summary>
        /// This tests the one hand far movement while camera (character) is moving around.
        /// The test will check the offset between object pivot and grab point and make sure we're not drifting
        /// out of the object on pointer rotation - this test is the same for all objects that won't change
        /// their orientation to camera while camera / pointer rotates as this will modify the far interaction grab point
        /// This test also has a sanity check to ensure behavior is still the same for objects of different scale
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorOneHandMoveFar()
        {
            // Disable gaze interactions for this unit test;
            InputTestUtilities.DisableGazeInteractor();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.3f;
            Vector3 initialObjectPosition = InputTestUtilities.InFrontOfUser(1f);
            testObject.transform.position = initialObjectPosition;
            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = testObject.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;

            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return new WaitForFixedUpdate();
            yield return null;

            const int numCircleSteps = 10;

            // Hand pointing at the cube
            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(0.6f);
            Quaternion initialHandRotation = Quaternion.identity;
            TestHand hand = new TestHand(Handedness.Right);

            Vector3[] objectScales = new Vector3[] { Vector3.one * 0.2f, new Vector3(0.2f, 0.4f, 0.3f) };
            foreach (var objectScale in objectScales)
            {
                // do this test for every one hand rotation mode
                foreach (ObjectManipulator.RotateAnchorType rotationAnchorType in Enum.GetValues(typeof(ObjectManipulator.RotateAnchorType)))
                {
                    objectManipulator.RotationAnchorFar = rotationAnchorType;

                    InputTestUtilities.InitializeCameraToOriginAndForward();
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    yield return hand.Show(initialHandPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.RotateTo(initialHandRotation);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    Vector3 initialPosition = testObject.transform.position;
                    yield return hand.SetHandshape(HandshapeId.Pinch);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    Assert.IsTrue(objectManipulator.isSelected, "ObjectManipulator wasn't selected!");

                    // Ensure the object didn't move after pinching if using object centered rotation
                    if (rotationAnchorType == ObjectManipulator.RotateAnchorType.RotateAboutObjectCenter)
                    {
                        TestUtilities.AssertAboutEqual(initialPosition, testObject.transform.position, "object shifted during pinch", 0.005f);
                    }

                    // we do this because even though the interactor's position doesn't shift during the pinch
                    // what the hand considers to be the 'controller position' seems to shift slightly when performing the pinch gesture
                    yield return hand.MoveTo(initialHandPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.RotateTo(initialHandRotation);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    // save relative pos grab point to object
                    // The firstInteractorSelecting is the one that is currently grabbing the object
                    Vector3 initialGrabPoint = objectManipulator.firstInteractorSelecting.GetAttachTransform(objectManipulator).position;
                    Vector3 initialGrabPointInObject = testObject.transform.InverseTransformPoint(initialGrabPoint);
                    Vector3 initialGrabOffset = objectManipulator.firstInteractorSelecting.transform.position - testObject.transform.position;

                    // full circle
                    const int degreeStep = 360 / numCircleSteps;

                    // rotating the pointer in a circle around "the user"
                    for (int i = 1; i <= numCircleSteps; ++i)
                    {
                        // rotate main camera (user)
                        Vector3 rotationDelta = degreeStep * Vector3.up;
                        InputTestUtilities.RotateCamera(rotationDelta);
                        yield return RuntimeTestUtilities.WaitForUpdates();

                        // move hand with the camera
                        Vector3 newHandPosition = Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialHandPosition;
                        yield return hand.MoveTo(newHandPosition);
                        yield return RuntimeTestUtilities.WaitForUpdates();
                        yield return hand.RotateTo(Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialHandRotation);
                        yield return RuntimeTestUtilities.WaitForUpdates();

                        if (rotationAnchorType == ObjectManipulator.RotateAnchorType.RotateAboutObjectCenter)
                        {
                            // We can't guarantee that the attach transform stays locked in the same place due to the object itself rotating
                            // Just check that it's rotation matches that of the hand
                            Quaternion objectRotation = objectManipulator.transform.rotation;
                            TestUtilities.AssertAboutEqual(Quaternion.AngleAxis(degreeStep * i, Vector3.up), objectRotation, $"Rotation incorrect using {rotationAnchorType}");

                            // Also check that the object stays approximately infront of the hand
                            Assert.IsTrue(objectManipulator.firstInteractorSelecting.transform.InverseTransformPoint(objectManipulator.transform.position).z > 0);
                        }
                        else
                        {
                            // make sure that the grab point has not changed relative to the object while rotating
                            Vector3 grabPoint = objectManipulator.firstInteractorSelecting.GetAttachTransform(objectManipulator).position;
                            Vector3 grabPointRotated = testObject.transform.InverseTransformPoint(grabPoint);
                            TestUtilities.AssertAboutEqual(grabPointRotated, initialGrabPointInObject, $"Grab point on object changed during rotation using {rotationAnchorType}");
                        }
                    }

                    // Move the object forward and back
                    yield return hand.MoveTo(initialHandPosition + Vector3.forward);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    // make sure that the offset between grab and object centre has grown
                    Vector3 currentInteractorPosition = objectManipulator.firstInteractorSelecting.transform.position;
                    Vector3 currentOffset = currentInteractorPosition - testObject.transform.position;
                    Assert.IsTrue(currentOffset.magnitude > initialGrabOffset.magnitude, $"Object did not move farther away when moving forward while doing gaze manipulation");

                    yield return hand.MoveTo(initialHandPosition + Vector3.back * 0.2f);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    // make sure that the offset between grab and object centre has shrunk as it moves closer to the camera
                    currentInteractorPosition = objectManipulator.firstInteractorSelecting.transform.position;
                    currentOffset = currentInteractorPosition - testObject.transform.position;
                    Assert.IsTrue(currentOffset.magnitude < initialGrabOffset.magnitude, $"Object did not move closer when moving backwards while doing gaze manipulation");

                    yield return hand.MoveTo(initialHandPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.RotateTo(initialHandRotation);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    yield return hand.SetHandshape(HandshapeId.Open);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.Hide();
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    // There seems to be some sort of deficiency with the object manipulator where the object does not return exactly to it's original position
                    // TODO: Fix or log a bug
                    // TestUtilities.AssertAboutEqual(testObject.transform.position, initialObjectPosition, "object has shifted significantly");
                    testObject.transform.position = initialObjectPosition;
                }
            }
        }


        // <summary>
        /// This tests that the gaze pointer can be used to directly invoke the manipulation logic via simulated pointer events, used
        /// for scenarios like voice-driven movement using the gaze pointer.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorOneHandMoveGaze()
        {
            // Enable gaze interactions for this unit test;
            InputTestUtilities.EnableGazeInteractor();

            // Set up cube with ObjectManipulator
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Vector3 initialObjectPosition = InputTestUtilities.InFrontOfUser(1f);
            testObject.transform.position = initialObjectPosition;
            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = testObject.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;

            yield return RuntimeTestUtilities.WaitForUpdates();

            const int numCircleSteps = 10;

            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(0.5f); // Hand hovers in the center of the fov, but the hand ray misses the cube
            Quaternion initialHandRotation = Quaternion.identity;
            TestHand hand = new TestHand(Handedness.Right);

            Vector3[] objectScales = new Vector3[] { Vector3.one * 0.1f, new Vector3(0.1f, 0.05f, 0.08f) };

            foreach (var objectScale in objectScales)
            {
                testObject.transform.localScale = objectScale;
                yield return RuntimeTestUtilities.WaitForUpdates();

                // do this test for every one hand rotation mode
                foreach (ObjectManipulator.RotateAnchorType rotationAnchorType in Enum.GetValues(typeof(ObjectManipulator.RotateAnchorType)))
                {
                    objectManipulator.RotationAnchorNear = rotationAnchorType;

                    InputTestUtilities.InitializeCameraToOriginAndForward();

                    yield return hand.Show(initialHandPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.RotateTo(initialHandRotation);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    Vector3 initialPosition = testObject.transform.position;

                    yield return hand.SetHandshape(HandshapeId.Pinch);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    // Ensure the object didn't move after pinching if using object centered rotation
                    if (rotationAnchorType == ObjectManipulator.RotateAnchorType.RotateAboutObjectCenter)
                    {
                        TestUtilities.AssertAboutEqual(initialPosition, testObject.transform.position, "object shifted during pinch", 0.01f);
                    }
                    // we do this because even though the interactor's position doesn't shift during the pinch
                    // what the hand considers to be the 'controller position' seems to shift slightly when performing the pinch gesture
                    yield return hand.MoveTo(initialHandPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.RotateTo(initialHandRotation);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    // save relative pos grab point to object
                    // The firstInteractorSelecting is the one that is currently grabbing the object
                    Vector3 initialGrabPoint = objectManipulator.firstInteractorSelecting.transform.position;
                    Vector3 initialGrabPointInObject = testObject.transform.InverseTransformPoint(initialGrabPoint);
                    Vector3 initialGrabOffset = objectManipulator.firstInteractorSelecting.transform.position - testObject.transform.position;

                    // full circle
                    const int degreeStep = 360 / numCircleSteps;

                    // rotating the pointer in a circle around "the user"
                    for (int i = 1; i <= numCircleSteps; ++i)
                    {
                        // rotate main camera (user)
                        Vector3 rotationDelta = degreeStep * Vector3.up;
                        InputTestUtilities.RotateCamera(rotationDelta);
                        yield return RuntimeTestUtilities.WaitForUpdates();

                        // move hand with the camera
                        Vector3 newHandPosition = Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialHandPosition;
                        yield return hand.MoveTo(newHandPosition);
                        yield return RuntimeTestUtilities.WaitForUpdates();
                        yield return hand.RotateTo(Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialHandRotation);
                        yield return RuntimeTestUtilities.WaitForUpdates();

                        // The exact position where a gaze pinched object ends up as it's manipulated by the hand is a bit unclear at the moment
                        // For now, use the following rough check to see that the object is rotating and is staying in front of the interactor

                        // We can't guarantee that the attach transform stays locked in the same place due to the object itself rotating
                        // Just check that it's rotation matches that of the hand
                        Quaternion objectRotation = objectManipulator.transform.rotation;
                        TestUtilities.AssertAboutEqual(Quaternion.AngleAxis(degreeStep * i, Vector3.up).normalized, objectRotation.normalized, $"Rotation incorrect using {rotationAnchorType}");

                        // Also check that the object stays approximately in front of the hand
                        Assert.IsTrue(objectManipulator.firstInteractorSelecting.transform.InverseTransformPoint(objectManipulator.transform.position).z > 0);
                    }

                    // Move the object forward and back
                    yield return hand.MoveTo(initialHandPosition + Vector3.forward);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    // make sure that the offset between grab and object centre hasn't changed while rotating
                    Vector3 currentGrabPoint = objectManipulator.firstInteractorSelecting.transform.position;
                    Vector3 currentOffset = currentGrabPoint - testObject.transform.position;
                    Assert.IsTrue(currentOffset.magnitude > initialGrabOffset.magnitude, $"Object did not move farther away when moving forward while doing gaze manipulation");

                    yield return hand.MoveTo(initialHandPosition + Vector3.back * 0.2f);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    // make sure that the offset between grab and object centre hasn't changed while rotating
                    currentGrabPoint = objectManipulator.firstInteractorSelecting.transform.position;
                    currentOffset = currentGrabPoint - testObject.transform.position;
                    Assert.IsTrue(currentOffset.magnitude < initialGrabOffset.magnitude, $"Object did not move closer when moving backwards while doing gaze manipulation");

                    yield return hand.MoveTo(initialHandPosition);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.RotateTo(initialHandRotation);
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    yield return hand.SetHandshape(HandshapeId.Open);
                    yield return RuntimeTestUtilities.WaitForUpdates();
                    yield return hand.Hide();
                    yield return RuntimeTestUtilities.WaitForUpdates();

                    // There seems to be some sort of deficiency with the object manipulator where the object does not return exactly to it's original position
                    // TODO: Fix or log a bug
                    // TestUtilities.AssertAboutEqual(testObject.transform.position, initialObjectPosition, "object has shifted significantly");
                    testObject.transform.position = initialObjectPosition;
                }
            }
        }

        /// <summary>
        /// Verifies that changing the ObjectManipulator's target transform at runtime works as expected.
        /// (GH#10889)
        /// </summary>
        [UnityTest]
        public IEnumerator TestObjManipTargetChange()
        {
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ObjectManipulator objmanip1 = cube1.AddComponent<ObjectManipulator>();
            objmanip1.SmoothingNear = false;
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.1f, 0.1f, 1));
            cube1.transform.localScale = Vector3.one * 0.2f;
            yield return RuntimeTestUtilities.WaitForUpdates();

            // First cube gets a FaceUserConstraint
            cube1.AddComponent<FaceUserConstraint>();

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ObjectManipulator objmanip2 = cube2.AddComponent<ObjectManipulator>();
            objmanip2.SmoothingNear = false;
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.5f, 0.1f, 1));
            yield return RuntimeTestUtilities.WaitForUpdates();
            cube2.transform.localScale = Vector3.one * 0.2f;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify that a ConstraintManager was automatically added.
            Assert.IsTrue(cube1.GetComponent<ConstraintManager>() != null, "Runtime-spawned ObjManip didn't also spawn ConstraintManager");
            Assert.IsTrue(cube2.GetComponent<ConstraintManager>() != null, "Runtime-spawned ObjManip didn't also spawn ConstraintManager");

            // Assert that HostTransform defaults to the object's transform.
            Assert.IsTrue(objmanip1.HostTransform == cube1.transform, "ObjManip's HostTransform didn't default to the object itself!");
            Assert.IsTrue(objmanip2.HostTransform == cube2.transform, "ObjManip's HostTransform didn't default to the object itself!");

            var rightHand = new TestHand(Handedness.Right);
            InputTestUtilities.SetHandAnchorPoint(Handedness.Left, ControllerAnchorPoint.Grab);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.MoveTo(cube1.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objmanip1.IsPokeHovered, "ObjManip shouldn't get IsPokeHovered");
            Assert.IsTrue(objmanip1.IsGrabHovered, "ObjManip didn't report IsGrabHovered");

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(objmanip1.IsGrabSelected, "ObjManip didn't report IsGrabSelected");
            Assert.IsFalse(objmanip1.IsPokeSelected, "ObjManip was PokeSelected. Should not be possible.");

            yield return rightHand.Move(Vector3.left * 0.5f); // Move the hand, cube should stay facing user.
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(cube1.transform.forward.CloseEnoughTo(-(cube1.transform.position - Camera.main.transform.position).normalized), "Cube1 didn't stay facing user!");

            yield return rightHand.SetHandshape(HandshapeId.Open);

            // Reassign the HostTransform to cube2.
            objmanip1.HostTransform = cube2.transform;
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Vector3 cube1Pos = cube1.transform.position;
            Vector3 cube2Pos = cube2.transform.position;

            yield return rightHand.Move(Vector3.right * 0.5f); // Move hand. Cube1 should stay in place, cube2 should move.

            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(cube1.transform.position.CloseEnoughTo(cube1Pos), "Cube1 moved when it shouldn't have!");
            Assert.IsTrue(!cube2.transform.position.CloseEnoughTo(cube2Pos), "Cube2 didn't move when it should have!");
            
            // Cube2 should be facing the user.
            Assert.IsTrue(cube2.transform.forward.CloseEnoughTo(-(cube2.transform.position - Camera.main.transform.position).normalized), "Cube2 didn't stay facing user!");

        }

        #endregion

        #region Two Handed Manipulation Tests

        // This test is not yet working due to some confusion as to how the centroid math works with the current object manipulator

        /*
        /// <summary>
        /// Test that the grab centroid is calculated correctly while rotating
        /// the hands during a two-hand near interaction grab.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorTwoHandedCentroid()
        {
            InputTestUtilities.DisableGazeInteractor();

            InputTestUtilities.InitializeCameraToOriginAndForward();

            // Set up cube with ObjectManipulator
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.5f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = testObject.transform.rotation;
            testObject.transform.position = initialObjectPosition;

            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = testObject.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;
            // Configuring for two-handed interaction
            objectManipulator.selectMode = UnityEngine.XR.Interaction.Toolkit.InteractableSelectMode.Multiple;

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            yield return rightHand.Show(Vector3.zero);
            yield return leftHand.Show(Vector3.zero);

            yield return rightHand.MoveTo(new Vector3(0.1f, -0.1f, 0.8f));
            yield return leftHand.MoveTo(new Vector3(-0.1f, -0.1f, 0.8f));
            yield return null;

            // Only testing move/rotate centroid position
            objectManipulator.AllowedManipulations = TransformFlags.Move | TransformFlags.Scale;

            // Only testing near manipulation
            objectManipulator.AllowedInteractionTypes = InteractionFlags.Near;

            int manipulationStartedCount = 0;
            int manipulationEndedCount = 0;
            objectManipulator.selectEntered.AddListener((med) => manipulationStartedCount++);
            objectManipulator.selectExited.AddListener((med) => manipulationEndedCount++);

            // Grab the box.
            yield return rightHand.SetGesture(GestureId.Pinch);
            yield return leftHand.SetGesture(GestureId.Pinch);

            // Previously we checked that we didn't move after two pinches, however, due to the hand position shifting slighting on pinch, this is not applicable
            // TODO, address in the future?
            // Should not have moved (yet!)
            // TestUtilities.AssertAboutEqual(testObject.transform.position, initialObjectPosition, $"Object moved when it shouldn't have! Position: {testObject.transform.position:F5}", 0.00001f);

            // The ObjectManipulator should recognize that we've begun manipulation.
            Assert.IsTrue(manipulationStartedCount > 0);

            yield return RuntimeTestUtilities.WaitForEnterKey();

            // Move both hands outwards; the object may be scaled but the position should remain the same.
            yield return rightHand.MoveTo(new Vector3(0.2f, -0.1f, 0.8f));
            yield return leftHand.MoveTo(new Vector3(-0.2f, -0.1f, 0.8f));


            yield return RuntimeTestUtilities.WaitForEnterKey();

            // Should *still* not have moved!
            // TestUtilities.AssertAboutEqual(testObject.transform.position, initialObjectPosition, $"Object moved when it shouldn't have! Position: {testObject.transform.position:F5}", 0.00001f);

            // Manipulation should not yet have ended.
            Assert.IsTrue(manipulationEndedCount == 0);

            // Get the grab points before we rotate the hands.
            // the left and right grab interactors should be the only interactors allowed to select this object manipulator
            var leftGrabPoint = objectManipulator.interactorsSelecting[0].transform.position;
            var rightGrabPoint = objectManipulator.interactorsSelecting[1].transform.position;
            var originalCentroid = (leftGrabPoint + rightGrabPoint) / 2.0f;

            // List of test conditions for test fuzzing.
            // Uses ValueTuple with layout (position, rotation)
            List<(Vector3, Vector3)> testConditions = new List<(Vector3, Vector3)>
            {
                (new Vector3(0, 90, 0), new Vector3(0.2f, -0.1f, 0.8f)),
                (new Vector3(25, 30, 45), new Vector3(0.3f, -0.2f, 0.7f)),
                (new Vector3(75, 140, 0), new Vector3(0.1f, -0.1f, 0.8f)),
                (new Vector3(10, 90, 20), new Vector3(0.5f, -0.2f, 0.5f)),
                (new Vector3(45, 110, 0), new Vector3(0.3f, -0.1f, 0.8f))
            };

            // Fuzz test.
            foreach (var testCondition in testConditions)
            {
                yield return MoveHandsAndCheckCentroid(testCondition.Item1, testCondition.Item2, leftHand, rightHand, objectManipulator, initialObjectPosition, originalCentroid, testObject.transform);
            }

            yield return rightHand.SetGesture(GestureId.Open);
            yield return leftHand.SetGesture(GestureId.Open);
        }

        /// <summary>
        /// Helper function for ObjectManipulatorTwoHandedCentroid. Will mirror desired handRotation
        /// and handPosition across the two hands, and verify that the centroid was still respected
        /// by the manipulated object.
        /// </summary>
        private IEnumerator MoveHandsAndCheckCentroid(Vector3 handRotationEuler, Vector3 handPosition,
                TestHand leftHand, TestHand rightHand,
                ObjectManipulator om,
                Vector3 originalObjectPosition, Vector3 originalGrabCentroid,
                Transform testObject)
        {
            // Rotate the hands.
            yield return rightHand.RotateTo(Quaternion.Euler(handRotationEuler.x, handRotationEuler.y, handRotationEuler.z));
            yield return leftHand.RotateTo(Quaternion.Euler(handRotationEuler.x, -handRotationEuler.y, -handRotationEuler.z));

            // Move the hands.
            yield return rightHand.MoveTo(new Vector3(handPosition.x, handPosition.y, handPosition.z));
            yield return leftHand.MoveTo(new Vector3(-handPosition.x, handPosition.y, handPosition.z));

            // Recalculate the new grab centroid.
            var leftGrabPoint = om.interactorsSelecting[0].transform.position;
            var rightGrabPoint = om.interactorsSelecting[0].transform.position;
            var centroid = (leftGrabPoint + rightGrabPoint) / 2.0f;

            // Compute delta between original grab centroid and the new centroid.
            var centroidDelta = centroid - originalGrabCentroid;

            // Ensure grab consistency.
            TestUtilities.AssertAboutEqual(testObject.transform.position, originalObjectPosition + centroidDelta,
                                           $"Object moved did not move according to the delta! Actual position: {testObject.transform.position:F5}, should be {originalObjectPosition + centroidDelta}", 0.00001f);
        }
        */

        #endregion

        #region Physics Interaction Tests

        /// <summary>
        /// Test that objects with both ObjectManipulator and Rigidbody respond
        /// correctly to static colliders.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorStaticCollision()
        {
            InputTestUtilities.InitializeCameraToOriginAndForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.5f;
            testObject.transform.position = InputTestUtilities.InFrontOfUser(1f);

            var rigidbody = testObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;

            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = testObject.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;

            var collisionListener = testObject.AddComponent<TestCollisionListener>();

            // set up static cube to collide with
            var backgroundObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundObject.transform.localScale = Vector3.one;
            backgroundObject.transform.position = InputTestUtilities.InFrontOfUser(2f);
            backgroundObject.GetComponent<MeshRenderer>().material.color = Color.green;

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(testObject.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Grab the cube and move towards the collider
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return hand.Move(Vector3.forward * 3f);
            yield return RuntimeTestUtilities.WaitForFixedUpdates();

            Assert.Less(testObject.transform.position.z, backgroundObject.transform.position.z);
            Assert.AreEqual(1, collisionListener.CollisionCount);
        }

        /// <summary>
        /// Test that objects with both ObjectManipulator and Rigidbody respond
        /// correctly to rigidbody colliders.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectManipulatorRigidbodyCollision()
        {
            InputTestUtilities.InitializeCameraToOriginAndForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.5f;
            testObject.transform.position = InputTestUtilities.InFrontOfUser(1f);

            var rigidbody = testObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;

            var objectManipulator = testObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = testObject.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;

            var collisionListener = testObject.AddComponent<TestCollisionListener>();

            // set up static cube to collide with
            var backgroundObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundObject.transform.localScale = Vector3.one;
            backgroundObject.transform.position = InputTestUtilities.InFrontOfUser(2f);
            backgroundObject.GetComponent<MeshRenderer>().material.color = Color.green;
            var backgroundRigidbody = backgroundObject.AddComponent<Rigidbody>();
            backgroundRigidbody.useGravity = false;

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(testObject.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Grab the cube and move towards the collider
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return hand.Move(Vector3.forward * 3f);
            yield return RuntimeTestUtilities.WaitForFixedUpdates();

            Assert.AreNotEqual(Vector3.zero, backgroundRigidbody.velocity);
            Assert.AreEqual(1, collisionListener.CollisionCount);
        }

        class TestCollisionListener : MonoBehaviour
        {
            public int CollisionCount { get; private set; }

            private void OnCollisionEnter(Collision collision)
            {
                CollisionCount++;
            }
        }

        #endregion

        /************** To be added in the future *****************
       /// <summary>
       /// Test validates throw behavior on manipulation handler. Box with disabled gravity should travel a
       /// certain distance when being released from grab during hand movement. Specifically for near interactions, 
       /// where we expect the thrown object to match the controllers velocities.
       /// </summary>
       [UnityTest]
       public IEnumerator ObjectManipulatorNearThrow()
       {
           // set up cube with manipulation handler
           var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
           testObject.transform.localScale = Vector3.one * 0.2f;
           Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
           testObject.transform.position = initialObjectPosition;

           var rigidBody = testObject.AddComponent<Rigidbody>();
           rigidBody.useGravity = false;

           var objectManipulator = testObject.AddComponent<ObjectManipulator>();
           objectManipulator.HostTransform = testObject.transform;
           objectManipulator.SmoothingFar = false;
           objectManipulator.SmoothingNear = false;

           yield return new WaitForFixedUpdate();
           yield return null;

           TestHand hand = new TestHand(Handedness.Right);

           Vector3 handOffset = new Vector3(0, 0, 0.1f);
           Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
           Vector3 rightPosition = new Vector3(1f, 0f, 1f);

           yield return hand.Show(initialHandPosition);
           yield return hand.MoveTo(initialObjectPosition);
           yield return RuntimeInputTestUtils.WaitForEnterKey();


           // Note: don't wait for a physics update after releasing, because it would recompute
           // the velocity of the hand and make it deviate from the rigid body velocity!
           yield return hand.GrabAndThrowAt(rightPosition, false);

           // yield return RuntimeInputTestUtils.WaitForEnterKey();


           // With simulated hand angular velocity would not be equal to 0, because of how simulation
           // moves hand when releasing the Pitch. Even though it doesn't directly follow from hand movement, there will always be some rotation.
           // Assert.NotZero(rigidBody.angularVelocity.magnitude, "ObjectManipulator should apply angular velocity to rigidBody upon release.");
           Assert.AreEqual(hand.GetVelocity(), rigidBody.velocity, "ObjectManipulator should apply hand velocity to rigidBody upon release.");

           // This is just for debugging purposes, so object's movement after release can be seen.
           yield return hand.MoveTo(initialHandPosition);
           yield return hand.Hide();

           GameObject.Destroy(testObject);
           yield return null;
       }


       /// <summary>
       /// Test validates throw behavior on manipulation handler. Box with disabled gravity should travel a
       /// certain distance when being released from grab during hand movement. Specifically for far interactions, 
       /// where we expect the thrown object to maintain it's velocities after being thrown
       /// </summary>
       [UnityTest]
       public IEnumerator ObjectManipulatorFarThrow()
       {
           // set up cube with manipulation handler
           var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
           testObject.transform.localScale = Vector3.one * 0.2f;
           Vector3 initialObjectPosition = new Vector3(-0.637f, -0.679f, 1.13f);
           testObject.transform.position = initialObjectPosition;

           var rigidBody = testObject.AddComponent<Rigidbody>();
           rigidBody.useGravity = false;

           var objectManipulator = testObject.AddComponent<ObjectManipulator>();
           objectManipulator.HostTransform = testObject.transform;
           objectManipulator.SmoothingFar = false;
           objectManipulator.SmoothingNear = false;

           yield return new WaitForFixedUpdate();
           yield return null;

           TestHand hand = new TestHand(Handedness.Right);

           Vector3 handOffset = new Vector3(0, 0, 0.1f);
           Vector3 initialHandPosition = Vector3.zero;
           Vector3 rightPosition = new Vector3(1f, 0f, 1f);

           yield return hand.Show(initialHandPosition);
           yield return RuntimeInputTestUtils.WaitForEnterKey();
           // Note: don't wait for a physics update after releasing, because it would recompute
           // the velocity of the hand and make it deviate from the rigid body velocity!
           yield return hand.GrabAndThrowAt(rightPosition, false);

           // With simulated hand angular velocity would not be equal to 0, because of how simulation
           // moves hand when releasing the Pitch. Even though it doesn't directly follow from hand movement, there will always be some rotation.
           Assert.Zero(rigidBody.angularVelocity.magnitude, "Object should have maintained its angular velocity of zero being released.");

           // Assert.IsTrue(rigidBody.velocity != hand.GetVelocity() && rigidBody.velocity.magnitude > 0.0f, "ObjectManipulator should not dampen it's velocity to match the hand's upon release.");
           yield return RuntimeInputTestUtils.WaitForEnterKey();
           // This is just for debugging purposes, so object's movement after release can be seen.
           yield return hand.MoveTo(initialHandPosition);
           yield return hand.Hide();

           GameObject.Destroy(testObject);
           yield return null;
       }
        */
    }
}

