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

using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ManipulationHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Test creating adding a ManipulationHandler to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerInstantiate()
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

        /// <summary>
        /// Test creating ManipulationHandler and receiving hover enter/exit events
        /// from gaze provider.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerGazeHover()
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

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
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

            GameObject.Destroy(testObject);
        }

        /// <summary>
        /// Tests MaintainRotationToUser mode of ManipulationHandler (OneHandedOnly)
        /// MaintainRotationToUser should only align with user / camera on x / y and not apply rotations in z
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerRotateWithUser()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneHandedOnly;
            manipHandler.OneHandRotationModeNear = ManipulationHandler.RotateInOneHandType.MaintainRotationToUser;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();
            yield return new WaitForFixedUpdate();
            yield return null;


            Vector3 initialGrabPosition = new Vector3(-0.1f, -0.1f, 1f); // grab the left bottom corner of the cube 
            TestHand hand = new TestHand(Handedness.Right);
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return hand.Show(initialGrabPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            Quaternion originalObjRotation = testObject.transform.rotation;

            CameraCache.Main.transform.Rotate(new Vector3(10, 0, 0));
            yield return new WaitForFixedUpdate();
            yield return null;

            Quaternion rotatedOriginal = originalObjRotation * Quaternion.Euler(10, 0, 0);
            // check if x rotation was applied to object
            TestUtilities.AssertAboutEqual(testObject.transform.rotation.eulerAngles, rotatedOriginal.eulerAngles, "Object wasn't rotated with camera");

            CameraCache.Main.transform.Rotate(new Vector3(-10, 0, 0));
            CameraCache.Main.transform.Rotate(new Vector3(0, 10, 0));

            yield return new WaitForFixedUpdate();
            yield return null;

            // check if y rotation was applied to object
            rotatedOriginal = originalObjRotation * Quaternion.Euler(0, 10, 0);
            TestUtilities.AssertAboutEqual(testObject.transform.rotation.eulerAngles, rotatedOriginal.eulerAngles, "Object wasn't rotated with camera");

            CameraCache.Main.transform.Rotate(new Vector3(0, -10, 0));
            CameraCache.Main.transform.Rotate(new Vector3(0, 0, 10));
            yield return new WaitForFixedUpdate();
            yield return null;

            // check if z rotation wasn't applied to object
            rotatedOriginal = originalObjRotation * Quaternion.Euler(0, 0, 10);
            TestUtilities.AssertNotAboutEqual(testObject.transform.rotation.eulerAngles, rotatedOriginal.eulerAngles, "Object rolled with camera");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return hand.Hide();

        }

        /// <summary>
        /// Test validates throw behavior on manipulation handler. Box with disabled gravity should travel a 
        /// certain distance when being released from grab during hand movement
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerThrow()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;

            var rigidBody = testObject.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();

            yield return new WaitForFixedUpdate();
            yield return null;

            TestHand hand = new TestHand(Handedness.Right);

            // grab the cube - move it to the right 
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            Vector3 handOffset = new Vector3(0, 0, 0.1f);
            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            Vector3 rightPosition = new Vector3(1f, 0f, 1f);

            yield return hand.Show(initialHandPosition);
            yield return hand.MoveTo(initialObjectPosition);
            yield return hand.GrabAndThrowAt(rightPosition);

            // With simulated hand angular velocity would not be equal to 0, because of how simulation
            // moves hand when releasing the Pitch. Even though it doesn't directly follow from hand movement, there will always be some rotation.
            Assert.NotZero(rigidBody.angularVelocity.magnitude, "Manipulationhandler should apply angular velocity to rigidBody upon release.");
            Assert.AreEqual(rigidBody.velocity, hand.GetVelocity(), "Manipulationhandler should apply hand velocity to rigidBody upon release.");

            // This is just for debugging purposes, so object's movement after release can be seen.
            yield return hand.MoveTo(initialHandPosition);
            yield return hand.Hide();

            GameObject.Destroy(testObject);
            yield return null;
        }


        /// <summary>
        /// This tests the one hand near movement while camera (character) is moving around.
        /// The test will check the offset between object pivot and grab point and make sure we're not drifting
        /// out of the object on pointer rotation - this test should be the same in all rotation setups
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerOneHandMoveNear()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneHandedOnly;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numCircleSteps = 10;
            const int numHandSteps = 3;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            Vector3 initialGrabPosition = new Vector3(-0.1f, -0.1f, 1f); // grab the left bottom corner of the cube 
            TestHand hand = new TestHand(Handedness.Right);

            // do this test for every one hand rotation mode
            foreach (ManipulationHandler.RotateInOneHandType type in Enum.GetValues(typeof(ManipulationHandler.RotateInOneHandType)))
            {
                manipHandler.OneHandRotationModeNear = type;

                TestUtilities.PlayspaceToOriginLookingForward();

                yield return hand.Show(initialHandPosition);
                var pointer = hand.GetPointer<SpherePointer>();
                Assert.IsNotNull(pointer);

                yield return hand.MoveTo(initialGrabPosition, numHandSteps);
                yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

                // save relative pos grab point to object
                Vector3 initialOffsetGrabToObjPivot = pointer.Position - testObject.transform.position;
                Vector3 initialGrabPointInObject = testObject.transform.InverseTransformPoint(manipHandler.GetPointerGrabPoint(pointer.PointerId));

                // full circle
                const int degreeStep = 360 / numCircleSteps;

                // rotating the pointer in a circle around "the user" 
                for (int i = 1; i <= numCircleSteps; ++i)
                {
                    // rotate main camera (user)
                    MixedRealityPlayspace.PerformTransformation(
                    p =>
                    {
                        p.position = MixedRealityPlayspace.Position;
                        Vector3 rotatedFwd = Quaternion.AngleAxis(degreeStep * i, Vector3.up) * Vector3.forward;
                        p.LookAt(rotatedFwd);
                    });

                    yield return null;

                    // move hand with the camera
                    Vector3 newHandPosition = Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialGrabPosition;
                    yield return hand.MoveTo(newHandPosition, numHandSteps);

                    if (type == ManipulationHandler.RotateInOneHandType.RotateAboutObjectCenter)
                    {
                        // make sure that the offset between hand and object centre hasn't changed while rotating
                        Vector3 offsetRotated = pointer.Position - testObject.transform.position;
                        TestUtilities.AssertAboutEqual(offsetRotated, initialOffsetGrabToObjPivot, $"Object offset changed during rotation using {type}");
                    }
                    else
                    {
                        // make sure that the offset between grab point and object pivot hasn't changed while rotating
                        Vector3 grabPoint = manipHandler.GetPointerGrabPoint(pointer.PointerId);
                        Vector3 cornerRotated = testObject.transform.TransformPoint(initialGrabPointInObject);
                        TestUtilities.AssertAboutEqual(cornerRotated, grabPoint, $"Grab point on object changed during rotation using {type}");
                    }
                }

                yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
                yield return hand.Hide();

            }

        }

        /// <summary>
        /// This tests the one hand far movement while camera (character) is moving around.
        /// The test will check the offset between object pivot and grab point and make sure we're not drifting
        /// out of the object on pointer rotation - this test is the same for all objects that won't change 
        /// their orientation to camera while camera / pointer rotates as this will modify the far interaction grab point
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerOneHandMoveFar()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneHandedOnly;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numCircleSteps = 10;
            const int numHandSteps = 3;

            Vector3 initialHandPosition = new Vector3(0.04f, -0.18f, 0.3f); // grab point on the lower center part of the cube
            
            TestHand hand = new TestHand(Handedness.Right);     

            // do this test for every one hand rotation mode
            foreach (ManipulationHandler.RotateInOneHandType type in Enum.GetValues(typeof(ManipulationHandler.RotateInOneHandType)))
            {
                // TODO: grab point is moving in this test and has to be covered by a different test
                if (type == ManipulationHandler.RotateInOneHandType.MaintainOriginalRotation)
                {
                    continue;
                }         

                manipHandler.OneHandRotationModeFar = type;

                TestUtilities.PlayspaceToOriginLookingForward();

                yield return hand.Show(initialHandPosition);
                yield return null;
               
                // pinch and let go of the object again to make sure that any rotation adjustment we're doing is applied 
                // at the beginning of our test and doesn't interfere with our grab position on the cubes surface while we're moving around
                yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
                yield return new WaitForFixedUpdate();
                yield return null;

                yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
                yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);


                // save relative pos grab point to object - for far interaction we need to check the grab point where the pointer ray hits the manipulated object
                InputSimulationService simulationService = PlayModeTestUtilities.GetInputSimulationService();
                IMixedRealityController[] inputControllers = simulationService.GetActiveControllers();
                // assume hand is first controller and pointer for this test
                IMixedRealityController handController = inputControllers[0];
                IMixedRealityPointer handPointer = handController.InputSource.Pointers[0];
                Vector3 initialGrabPosition = handPointer.Result.Details.Point;
                Vector3 initialOffsetGrabToObjPivot = MixedRealityPlayspace.InverseTransformPoint(initialGrabPosition) - MixedRealityPlayspace.InverseTransformPoint(testObject.transform.position);

                // full circle
                const int degreeStep = 360 / numCircleSteps;

                // rotating the pointer in a circle around "the user" 
                for (int i = 1; i <= numCircleSteps; ++i)
                {

                    // rotate main camera (user)
                    MixedRealityPlayspace.PerformTransformation(
                    p =>
                    {
                        p.position = MixedRealityPlayspace.Position;
                        Vector3 rotatedFwd = Quaternion.AngleAxis(degreeStep * i, Vector3.up) * Vector3.forward;
                        p.LookAt(rotatedFwd);
                    });

                    yield return null;

                    // move hand with the camera
                    Vector3 newHandPosition = Quaternion.AngleAxis(degreeStep * i, Vector3.up) * initialHandPosition;
                    yield return hand.MoveTo(newHandPosition, numHandSteps);
                    yield return new WaitForFixedUpdate();
                    yield return null;


                    // make sure that the offset between grab point and object pivot hasn't changed while rotating
                    Vector3 newGrabPosition = handPointer.Result.Details.Point;
                    Vector3 offsetRotated = MixedRealityPlayspace.InverseTransformPoint(newGrabPosition) - MixedRealityPlayspace.InverseTransformPoint(testObject.transform.position);
                    TestUtilities.AssertAboutEqual(offsetRotated, initialOffsetGrabToObjPivot, "Grab point on object changed during rotation");
                }

                yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
                yield return hand.Hide();

            }

        }


        /// <summary>
        /// This tests the one hand near rotation and applying different rotation constraints to the object.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerOneHandRotateWithConstraint()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneHandedOnly;
            manipHandler.OneHandRotationModeFar = ManipulationHandler.RotateInOneHandType.RotateAboutGrabPoint;
            manipHandler.ReleaseBehavior = 0;
            manipHandler.AllowFarManipulation = false;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();

            yield return new WaitForFixedUpdate();
            yield return null;

            TestHand hand = new TestHand(Handedness.Right);

            yield return hand.Show(initialObjectPosition);
            yield return null;

            // grab the object
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForFixedUpdate();
            yield return null;

            float testRotation = 45;
            const int numRotSteps = 10;
            Quaternion testQuaternion = Quaternion.Euler(testRotation, testRotation, testRotation);

            // rotate without constraint
            manipHandler.ConstraintOnRotation = RotationConstraintType.None;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            float diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(testRotation, testRotation, testRotation));
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "object didn't rotate with hand");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.identity);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "object didn't rotate with hand");

            // rotate with x axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.XAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(testRotation, 0, 0));
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on x axis did not lock axis correctly");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.identity);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on x axis did not lock axis correctly");

            // rotate with y axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.YAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(0, testRotation, 0));
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Y axis did not lock axis correctly");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.identity);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Y axis did not lock axis correctly");

            // rotate with z axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.ZAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(0, 0, testRotation));
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Z axis did not lock axis correctly");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.identity);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Z axis did not lock axis correctly");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return hand.Hide();
        }

        private class OriginOffsetTest
        {
            const int numSteps = 1;

            public struct TestData
            {
                // transform data
                public MixedRealityPose pose;
                public Vector3 scale;

                // used to print where error occurred
                public string manipDescription;
            }

            public List<TestData> data = new List<TestData>();

            /// <summary>
            /// Manipulates the given testObject in a number of ways and records the output here
            /// </summary>
            /// <param name="testObject">An unrotated primitive cube at (0, 0, 1) with scale (0.2, 0.2, 0,2)</param>
            public IEnumerator RecordTransformValues(GameObject testObject)
            {
                TestUtilities.PlayspaceToOriginLookingForward();

                float testRotation = 45;
                Quaternion testQuaternion = Quaternion.Euler(testRotation, testRotation, testRotation);

                Vector3 leftHandNearPos = new Vector3(-0.1f, 0, 1);
                Vector3 rightHandNearPos = new Vector3(0.1f, 0, 1);
                Vector3 leftHandFarPos = new Vector3(-0.06f, -0.1f, 0.5f);
                Vector3 rightHandFarPos = new Vector3(0.06f, -0.1f, 0.5f);
                TestHand leftHand = new TestHand(Handedness.Left);
                TestHand rightHand = new TestHand(Handedness.Right);

                // One hand rotate near
                yield return rightHand.MoveTo(rightHandNearPos, numSteps);
                yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
                yield return rightHand.SetRotation(testQuaternion, numSteps);
                RecordTransform(testObject.transform, "one hand rotate near");

                // Two hand rotate/scale near
                yield return rightHand.SetRotation(Quaternion.identity, numSteps);
                yield return leftHand.MoveTo(leftHandNearPos, numSteps);
                yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
                yield return rightHand.Move(new Vector3(0.2f, 0.2f, 0), numSteps);
                yield return leftHand.Move(new Vector3(-0.2f, -0.2f, 0), numSteps);
                RecordTransform(testObject.transform, "two hand rotate/scale near");

                // Two hand rotate/scale far
                yield return rightHand.MoveTo(rightHandNearPos, numSteps);
                yield return leftHand.MoveTo(leftHandNearPos, numSteps);
                yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
                yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
                yield return rightHand.MoveTo(rightHandFarPos, numSteps);
                yield return leftHand.MoveTo(leftHandFarPos, numSteps);
                yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
                yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
                yield return rightHand.Move(new Vector3(0.2f, 0.2f, 0), numSteps);
                yield return leftHand.Move(new Vector3(-0.2f, -0.2f, 0), numSteps);
                RecordTransform(testObject.transform, "two hand rotate/scale far");

                // One hand rotate near
                yield return rightHand.MoveTo(rightHandFarPos, numSteps);
                yield return leftHand.MoveTo(leftHandFarPos, numSteps);
                yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
                yield return leftHand.Hide();

                MixedRealityPlayspace.PerformTransformation(
                p =>
                {
                    p.position = MixedRealityPlayspace.Position;
                    Vector3 rotatedFwd = Quaternion.AngleAxis(testRotation, Vector3.up) * Vector3.forward;
                    p.LookAt(rotatedFwd);
                });
                yield return null;
                
                Vector3 newHandPosition = Quaternion.AngleAxis(testRotation, Vector3.up) * rightHandFarPos;
                yield return rightHand.MoveTo(newHandPosition, numSteps);
                RecordTransform(testObject.transform, "one hand rotate far");

                yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
                yield return rightHand.Hide();
            }

            private void RecordTransform(Transform transform, string description)
            {
                data.Add(new TestData
                {
                    pose = new MixedRealityPose(transform.position, transform.rotation),
                    scale = transform.localScale,
                    manipDescription = description
                });
            }
        }

        /// <summary>
        /// This test records the poses and scales of an object after various forms of manipulation,
        /// once when the object origin is at the mesh centre and again when the origin is offset from the mesh.
        /// The test then compares these poses and scales in order to ensure that they are about equal. 
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerOriginOffset()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneAndTwoHanded;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();

            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = Vector3.forward;

            // Collect data for unmodified cube
            OriginOffsetTest expectedTest = new OriginOffsetTest();
            yield return expectedTest.RecordTransformValues(testObject);
            yield return null;

            // Modify cube mesh so origin is offset from centre
            Vector3 offset = Vector3.one * 5;

            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.rotation = Quaternion.identity;
            var mesh = testObject.GetComponent<MeshFilter>().mesh;
            var vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += offset;
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            testObject.GetComponent<BoxCollider>().center = offset;

            testObject.transform.position = Vector3.forward - testObject.transform.TransformVector(offset);

            // Collect data for modified cube
            OriginOffsetTest actualTest = new OriginOffsetTest();
            yield return actualTest.RecordTransformValues(testObject);
            yield return null;

            // Test that the results of both tests are equal
            var expectedData = expectedTest.data;
            var actualData = actualTest.data;
            Assert.AreEqual(expectedData.Count, actualData.Count);

            for (int i = 0; i < expectedData.Count; i++)
            {
                Vector3 transformedOffset = actualData[i].pose.Rotation * Vector3.Scale(offset, actualData[i].scale);
                TestUtilities.AssertAboutEqual(expectedData[i].pose.Position, actualData[i].pose.Position + transformedOffset, $"Failed for position of object for {actualData[i].manipDescription}");
                TestUtilities.AssertAboutEqual(expectedData[i].pose.Rotation, actualData[i].pose.Rotation, $"Failed for rotation of object for {actualData[i].manipDescription}");
                TestUtilities.AssertAboutEqual(expectedData[i].scale, actualData[i].scale, $"Failed for scale of object for {actualData[i].manipDescription}");
            }
        }

        /// <summary>
        /// This tests the minimum and maximum scaling for manipulation.
        /// This test will scale a cube with two hand manipulation and ensure that
        /// maximum and minimum scales are not exceeded.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ManipulationHandlerMinMaxScale()
        {
            float initialScale =  0.2f;
            float minScale = 0.5f;
            float maxScale = 2f;

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * initialScale;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneAndTwoHanded;

            var scaleHandler = testObject.EnsureComponent<TransformScaleHandler>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();
            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            Vector3 leftGrabPosition = new Vector3(-0.1f, -0.1f, 1f); // grab the bottom left corner of the cube 
            Vector3 rightGrabPosition = new Vector3(0.1f, -0.1f, 1f); // grab the bottom right corner of the cube 
            TestHand leftHand = new TestHand(Handedness.Left);
            TestHand rightHand = new TestHand(Handedness.Right);

            // Hands grab object at initial positions
            yield return leftHand.Show(initialHandPosition);
            yield return leftHand.MoveTo(leftGrabPosition, numHandSteps);
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            yield return rightHand.Show(initialHandPosition);
            yield return rightHand.MoveTo(rightGrabPosition, numHandSteps);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // No change to scale yet
            Assert.AreEqual(Vector3.one * initialScale, testObject.transform.localScale);

            // Move hands beyond max scale limit
            yield return leftHand.MoveTo(new Vector3(-scaleHandler.ScaleMaximum, 0, 0) + leftGrabPosition, numHandSteps);
            yield return rightHand.MoveTo(new Vector3(scaleHandler.ScaleMaximum, 0, 0) + rightGrabPosition, numHandSteps);

            // Assert scale at max
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMaximum, testObject.transform.localScale);

            // Move hands beyond min scale limit
            yield return leftHand.MoveTo(new Vector3(scaleHandler.ScaleMinimum, 0, 0) + leftGrabPosition, numHandSteps);
            yield return rightHand.MoveTo(new Vector3(-scaleHandler.ScaleMinimum, 0, 0) + rightGrabPosition, numHandSteps);

            // Assert scale at min
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMinimum, testObject.transform.localScale);
        }

        /// <summary>
        /// This test rotates the head without moving the hand.
        /// This test is set up to test using the Gestures input simulation mode as this is
        /// where we observed issues with this.
        /// If the head rotates, without moving the hand, the grabbed object should not move.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerRotateHeadGGV()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Switch to Gestures
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldIsp = iss.InputSimulationProfile;
            var isp = ScriptableObject.CreateInstance<MixedRealityInputSimulationProfile>();
            isp.HandSimulationMode = HandSimulationMode.Gestures;
            iss.InputSimulationProfile = isp;

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = testObject.transform.rotation;
            testObject.transform.position = initialObjectPosition;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            
            Vector3 originalHandPosition = new Vector3(0, 0, 0.5f);
            TestHand hand = new TestHand(Handedness.Right);
            const int numHandSteps = 1;

            // Grab cube
            yield return hand.Show(originalHandPosition);
            yield return null;
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Rotate Head and readjust hand
            int numRotations = 10;
            for (int i = 0; i < numRotations; i++)
            {
                MixedRealityPlayspace.Transform.Rotate(Vector3.up, 180 / numRotations);
                yield return hand.MoveTo(originalHandPosition, numHandSteps);
                yield return null;

                // Test Object hasn't moved
                TestUtilities.AssertAboutEqual(initialObjectPosition, testObject.transform.position, "Object moved while rotating head");
                TestUtilities.AssertAboutEqual(initialObjectRotation, testObject.transform.rotation, "Object rotated while rotating head", 0.25f);
            }

            // Restore the input simulation profile
            iss.InputSimulationProfile = oldIsp;
            yield return null;
        }
    }
}
#endif