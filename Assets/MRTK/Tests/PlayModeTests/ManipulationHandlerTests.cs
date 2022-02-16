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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ManipulationHandlerTests : BasePlayModeTests
    {
        private readonly List<Action> cleanupAction = new List<Action>();

        public override IEnumerator TearDown()
        {
            cleanupAction.ForEach(f => f?.Invoke());
            yield return base.TearDown();
        }

        /// <summary>
        /// Test creating adding a ManipulationHandler to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
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

            // This particular test is sensitive to the number of steps that a hand is moving,
            // so it's set to 30 to override the default amount.
            int numSteps = 30;

            Vector3 handOffset = new Vector3(0, 0, 0.1f);
            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            Vector3 rightPosition = new Vector3(1f, 0f, 1f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHand(initialHandPosition, initialObjectPosition, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService, numSteps);
            yield return PlayModeTestUtilities.MoveHand(initialObjectPosition, rightPosition, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService, numSteps);

            yield return null;

            // test if the object was properly translated
            float maxError = 0.05f;
            Vector3 posDiff = testObject.transform.position - rightPosition;
            Assert.IsTrue(posDiff.magnitude <= maxError, "ManipulationHandler translate failed");

            // forcefully end manipulation and drag with hand back to original position - object shouldn't move with hand
            manipHandler.ForceEndManipulation();
            yield return PlayModeTestUtilities.MoveHand(rightPosition, initialObjectPosition, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService, numSteps);

            posDiff = testObject.transform.position - initialObjectPosition;
            Assert.IsTrue(posDiff.magnitude > maxError, "Manipulationhandler modified objects even though manipulation was forcefully ended.");
            posDiff = testObject.transform.position - rightPosition;
            Assert.IsTrue(posDiff.magnitude <= maxError, "Manipulated object didn't remain in place after forcefully ending manipulation");

            // move hand back to object
            yield return PlayModeTestUtilities.MoveHand(initialObjectPosition, rightPosition, ArticulatedHandPose.GestureId.Open, Handedness.Right, inputSimulationService, numSteps);

            // grab object again and move to original position
            yield return PlayModeTestUtilities.MoveHand(rightPosition, initialObjectPosition, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService, numSteps);

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
            // Note: don't wait for a physics update after releasing, because it would recompute
            // the velocity of the hand and make it deviate from the rigid body velocity!
            yield return hand.GrabAndThrowAt(rightPosition, false);

            // With simulated hand angular velocity would not be equal to 0, because of how simulation
            // moves hand when releasing the Pitch. Even though it doesn't directly follow from hand movement, there will always be some rotation.
            Assert.NotZero(rigidBody.angularVelocity.magnitude, "Manipulationhandler should apply angular velocity to rigidBody upon release.");
            Assert.AreEqual(hand.GetVelocity(), rigidBody.velocity, "Manipulationhandler should apply hand velocity to rigidBody upon release.");

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
                Vector3 initialGrabPoint = manipHandler.GetPointerGrabPoint(pointer.PointerId);
                Vector3 initialOffsetGrabToObjPivot = initialGrabPoint - testObject.transform.position;
                Vector3 initialGrabPointInObject = testObject.transform.InverseTransformPoint(initialGrabPoint);

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
                        // make sure that the offset between grab and object centre hasn't changed while rotating
                        Vector3 grabPoint = manipHandler.GetPointerGrabPoint(pointer.PointerId);
                        Vector3 offsetRotated = grabPoint - testObject.transform.position;
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

            // Hand pointing at middle of cube
            Vector3 initialHandPosition = new Vector3(0.044f, -0.1f, 0.45f);
            TestHand hand = new TestHand(Handedness.Right);

            // do this test for every one hand rotation mode
            foreach (ManipulationHandler.RotateInOneHandType type in Enum.GetValues(typeof(ManipulationHandler.RotateInOneHandType)))
            {
                // Some rotation modes move the object on grab, don't test those
                if (type == ManipulationHandler.RotateInOneHandType.MaintainOriginalRotation ||
                    type == ManipulationHandler.RotateInOneHandType.FaceAwayFromUser ||
                    type == ManipulationHandler.RotateInOneHandType.FaceUser)
                {
                    continue;
                }

                manipHandler.OneHandRotationModeFar = type;

                TestUtilities.PlayspaceToOriginLookingForward();

                yield return hand.Show(initialHandPosition);
                yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

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
        /// This tests that the gaze pointer can be used to directly invoke the manipulation logic via simulated pointer events, used
        /// for scenarios like voice-driven movement using the gaze pointer.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerGazePointerMove()
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

            TestUtilities.PlayspaceToOriginLookingForward();

            yield return new WaitForFixedUpdate();
            yield return null;

            var gazePointer = CoreServices.InputSystem.GazeProvider.GazePointer;
            MixedRealityPointerEventData pointerDownData = new MixedRealityPointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerDownData.Initialize(gazePointer, MixedRealityInputAction.None, Microsoft.MixedReality.Toolkit.Utilities.Handedness.None);

            manipHandler.OnPointerDown(pointerDownData);

            Vector3 cameraMovement = new Vector3(2.1f, 0.4f, 0.2f);
            CameraCache.Main.transform.position += cameraMovement;

            yield return null;

            MixedRealityPointerEventData pointerMoveData = new MixedRealityPointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerMoveData.Initialize(gazePointer, MixedRealityInputAction.None, Microsoft.MixedReality.Toolkit.Utilities.Handedness.None);
            manipHandler.OnPointerDragged(pointerMoveData);

            Vector3 expectedPosition = initialObjectPosition + cameraMovement;
            TestUtilities.AssertAboutEqual(manipHandler.HostTransform.position, expectedPosition, "Camera movement translates to object movement from pointer updates");

            MixedRealityPointerEventData pointerUpData = new MixedRealityPointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerUpData.Initialize(gazePointer, MixedRealityInputAction.None, Microsoft.MixedReality.Toolkit.Utilities.Handedness.None);
            manipHandler.OnPointerUp(pointerUpData);

            Vector3 cameraSecondMovement = new Vector3(-0.7f, 0.9f, 2.2f);
            CameraCache.Main.transform.position += cameraSecondMovement;

            yield return null;

            TestUtilities.AssertAboutEqual(manipHandler.HostTransform.position, expectedPosition, "Camera movement after releasing pointer does not continue to affect object");
        }

        /// <summary>
        /// This tests the one hand near rotation and applying different rotation constraints to the object.
        /// NOTE: This tests both LOCAL and WORLD SPACE rotation.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerOneHandRotateWithConstraint()
        {
            // set up cube with manipulation handler
            GameObject parentObject = new GameObject("Test Object Parent");

            // In case of error, this object won't be cleaned up, so clean up at the end of the test
            cleanupAction.Add(() => { if (parentObject != null) UnityEngine.Object.Destroy(parentObject); });

            GameObject testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.parent = parentObject.transform;

            // Rotate the parent object, as we differ when constraining on local vs world
            Quaternion initialQuaternion = Quaternion.Euler(30f, 30f, 30f);
            parentObject.transform.rotation = initialQuaternion;

            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            parentObject.transform.position = initialObjectPosition;

            ManipulationHandler manipHandler = testObject.AddComponent<ManipulationHandler>();
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

            /*********************************/
            /*** TEST WORLD SPACE ROTATION ***/
            /*********************************/

            // rotate without constraint
            manipHandler.ConstraintOnRotation = RotationConstraintType.None;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            float diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(testRotation, testRotation, testRotation) * initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "object didn't rotate with hand (world space)");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "object didn't rotate with hand (world space)");

            // rotate with x axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.XAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(testRotation, 0, 0) * initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on x axis did not lock axis correctly (world space)");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on x axis did not lock axis correctly (world space)");

            // rotate with y axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.YAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(0, testRotation, 0) * initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Y axis did not lock axis correctly (world space)");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Y axis did not lock axis correctly (world space)");

            // rotate with z axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.ZAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, Quaternion.Euler(0, 0, testRotation) * initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Z axis did not lock axis correctly (world space)");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.rotation, initialQuaternion);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Z axis did not lock axis correctly (world space)");

            /*********************************/
            /*** TEST LOCAL SPACE ROTATION ***/
            /*********************************/

            manipHandler.UseLocalSpaceForConstraint = true;
            // rotate with x axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.XAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.localRotation, Quaternion.Euler(testRotation, 0, 0));
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on x axis did not lock axis correctly (local space)");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.localRotation, Quaternion.identity);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on x axis did not lock axis correctly (local space)");

            // rotate with y axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.YAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.localRotation, Quaternion.Euler(0, testRotation, 0));
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Y axis did not lock axis correctly (local space)");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.localRotation, Quaternion.identity);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Y axis did not lock axis correctly (local space)");

            // rotate with z axis only
            manipHandler.ConstraintOnRotation = RotationConstraintType.ZAxisOnly;
            yield return hand.SetRotation(testQuaternion, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.localRotation, Quaternion.Euler(0, 0, testRotation));
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Z axis did not lock axis correctly (local space)");

            yield return hand.SetRotation(Quaternion.identity, numRotSteps);
            diffAngle = Quaternion.Angle(testObject.transform.localRotation, Quaternion.identity);
            Assert.IsTrue(Mathf.Approximately(diffAngle, 0.0f), "constraint on Z axis did not lock axis correctly (local space)");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return hand.Hide();

            UnityEngine.Object.Destroy(parentObject);
        }

        private class OriginOffsetTest
        {
            const int numSteps = 10;

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
            // Without this background object that contains a collider, Unity will rarely
            // return no colliders hit for raycasts and sphere casts, even if a ray / sphere is 
            // intersecting the collider. Causes tests to be unreliable.
            // It seems to only happen when one collider is in the scene.
            var backgroundObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundObject.transform.position = Vector3.forward * 10;
            backgroundObject.transform.localScale = new Vector3(100, 100, 1);
            var backgroundmaterial = new Material(StandardShaderUtility.MrtkStandardShader);
            backgroundmaterial.color = Color.green;
            backgroundObject.GetComponent<MeshRenderer>().material = backgroundmaterial;


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
            mesh.RecalculateBounds();
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
                TestUtilities.AssertAboutEqual(expectedData[i].pose.Rotation, actualData[i].pose.Rotation, $"Failed for rotation of object for {actualData[i].manipDescription}", 1.0f);
                TestUtilities.AssertAboutEqual(expectedData[i].scale, actualData[i].scale, $"Failed for scale of object for {actualData[i].manipDescription}");
            }
        }

        /// <summary>
        /// This tests the minimum and maximum scaling for manipulation.
        /// This test will scale a cube with two hand manipulation and ensure that
        /// maximum and minimum scales are not exceeded.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerMinMaxScale()
        {
            float initialScale = 0.2f;
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
            var scaleHandler = testObject.EnsureComponent<MinMaxScaleConstraint>();
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
            var oldHandSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

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

            // Hand position is not exactly the pointer position, this correction applies the delta
            // from the hand to the pointer.
            Vector3 correction = originalHandPosition - hand.GetPointer<GGVPointer>().Position;
            yield return hand.Move(correction, numHandSteps);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            // Rotate Head and readjust hand
            int numRotations = 10;
            for (int i = 0; i < numRotations; i++)
            {
                MixedRealityPlayspace.Transform.Rotate(Vector3.up, 180 / numRotations);
                correction = originalHandPosition - hand.GetPointer<GGVPointer>().Position;
                yield return hand.Move(correction, numHandSteps);
                yield return null;

                // Test Object hasn't moved
                TestUtilities.AssertAboutEqual(initialObjectPosition, testObject.transform.position, "Object moved while rotating head", 0.01f);
                TestUtilities.AssertAboutEqual(initialObjectRotation, testObject.transform.rotation, "Object rotated while rotating head", 0.25f);
            }

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldHandSimMode;
            yield return null;
        }

        /// <summary>
        /// For positionless input sources that use gaze, such as xbox controller, pointer position will
        /// be coincident with the head position. This was causing issues with manipulation handler, as
        /// the distance between the pointer and the head is taken as part of the move logic. 
        /// This test simulates a positionless input source by using GGV hands and setting the hand position
        /// to be the head position. It then ensures that there is no weird behaviour as a result of this.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerPositionlessController()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Switch to Gestures
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldHandSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            TestHand hand = new TestHand(Handedness.Right);
            const int numHandSteps = 1;

            float expectedDist = Vector3.Distance(testObject.transform.position, CameraCache.Main.transform.position);

            yield return hand.Show(CameraCache.Main.transform.position);
            yield return null;

            // Hand position is not exactly the pointer position, this correction applies the delta
            // from the hand to the pointer.
            Vector3 correction = CameraCache.Main.transform.position - hand.GetPointer<GGVPointer>().Position;
            yield return hand.Move(correction, numHandSteps);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.AreEqual(expectedDist, Vector3.Distance(testObject.transform.position, CameraCache.Main.transform.position), 0.02f);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // Apply correction delta again as we have changed hand pose
            correction = CameraCache.Main.transform.position - hand.GetPointer<GGVPointer>().Position;
            yield return hand.Move(correction, numHandSteps);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.AreEqual(expectedDist, Vector3.Distance(testObject.transform.position, CameraCache.Main.transform.position), 0.02f);

            Vector3 delta = new Vector3(1.0f, 0, 0);
            yield return hand.Move(delta);
            MixedRealityPlayspace.Transform.Translate(delta);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.AreEqual(expectedDist, Vector3.Distance(testObject.transform.position, CameraCache.Main.transform.position), 2.5f);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.AreEqual(expectedDist, Vector3.Distance(testObject.transform.position, CameraCache.Main.transform.position), 0.02f);

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldHandSimMode;
            yield return null;
        }

        /// <summary>
        /// This test first moves the hand a set amount along the x-axis, records its x distance from the hand, then moves
        /// it the same amount along the y-axis and records its y distance from the hand. Given no constraints on manipulation,
        /// we expect these values to be the same.
        /// This test was added as a change to pointer behaviour made GGV manipulation along the y-axis sluggish.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerMoveYAxisGGV()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Switch to Gestures
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldHandSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.OneHandRotationModeFar = ManipulationHandler.RotateInOneHandType.FaceAwayFromUser;

            TestHand hand = new TestHand(Handedness.Right);
            const int numHandSteps = 1;

            float xPos, yPosUp, yPosDown;

            // Grab the object
            yield return hand.Show(new Vector3(0, 0, 0.5f));
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            // Move the hand in a way that does not change the distance from the body ray
            float root2Over2 = Mathf.Cos(Mathf.PI / 4);
            Vector3 moveX = new Vector3(root2Over2, 0, root2Over2) * 0.5f;
            yield return hand.MoveTo(moveX, numHandSteps);
            yield return null;

            xPos = testObject.transform.position.x - moveX.x;

            Vector3 moveYUp = new Vector3(0, root2Over2, root2Over2) * 0.5f;
            yield return hand.MoveTo(moveYUp, numHandSteps);
            yield return null;

            yPosUp = testObject.transform.position.y - moveYUp.y;

            Vector3 moveYDown = new Vector3(0, -0.5f, 0.5f);
            yield return hand.MoveTo(moveYDown, numHandSteps);
            yield return null;

            yPosDown = testObject.transform.position.y - moveYDown.y;

            Assert.AreEqual(xPos, yPosUp, 0.02f);
            Assert.AreEqual(xPos, Mathf.Abs(yPosDown), 0.02f);

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldHandSimMode;
            yield return null;
        }

        /// <summary>
        /// Ensure that while manipulating an object, if that object gets
        /// deactivated, that manipulation no longer moves that object.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerDisableObject()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 originalObjectPos = Vector3.forward;
            testObject.transform.position = originalObjectPos;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            TestHand hand = new TestHand(Handedness.Right);
            const int numHandSteps = 1;

            // Move the object without disabling and assure it moves
            Vector3 originalHandPos = new Vector3(0.06f, -0.1f, 0.5f);
            yield return hand.Show(originalHandPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            yield return hand.Move(new Vector3(0.2f, 0, 0), numHandSteps);
            yield return null;

            Assert.AreNotEqual(originalObjectPos, testObject.transform.position);

            // Disable the object and move the hand and assure the object does not move
            yield return hand.MoveTo(originalHandPos, numHandSteps);
            yield return null;

            testObject.SetActive(false);
            yield return null;
            testObject.SetActive(true);

            yield return hand.Move(new Vector3(0.2f, 0, 0), numHandSteps);
            yield return null;

            TestUtilities.AssertAboutEqual(originalObjectPos, testObject.transform.position, "Object moved after it was disabled");
        }

        /// <summary>
        /// Test that OnManipulationStarted and OnManipulationEnded events call as expected
        /// for various One Handed Only.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerEventsOneHandedOnly()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.5f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = testObject.transform.rotation;
            testObject.transform.position = initialObjectPosition;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            int manipulationStartedCount = 0;
            int manipulationEndedCount = 0;
            manipHandler.OnManipulationStarted.AddListener((med) => manipulationStartedCount++);
            manipHandler.OnManipulationEnded.AddListener((med) => manipulationEndedCount++);

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            yield return rightHand.Show(new Vector3(0.1f, -0.1f, 0.5f));
            yield return leftHand.Show(new Vector3(-0.1f, -0.1f, 0.5f));
            yield return null;

            // One Handed
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneHandedOnly;

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(0, manipulationEndedCount);

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(0, manipulationEndedCount);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(1, manipulationEndedCount);

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(1, manipulationEndedCount);
        }

        /// <summary>
        /// Test that OnManipulationStarted and OnManipulationEnded events call as expected
        /// for Two Handed Only.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerEventsTwoHandedOnly()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.5f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = testObject.transform.rotation;
            testObject.transform.position = initialObjectPosition;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            int manipulationStartedCount = 0;
            int manipulationEndedCount = 0;
            manipHandler.OnManipulationStarted.AddListener((med) => manipulationStartedCount++);
            manipHandler.OnManipulationEnded.AddListener((med) => manipulationEndedCount++);

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            yield return rightHand.Show(new Vector3(0.1f, -0.1f, 0.5f));
            yield return leftHand.Show(new Vector3(-0.1f, -0.1f, 0.5f));
            yield return null;

            // Two Handed
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.TwoHandedOnly;

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(0, manipulationStartedCount);
            Assert.AreEqual(0, manipulationEndedCount);

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(0, manipulationEndedCount);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(1, manipulationEndedCount);

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(1, manipulationEndedCount);
        }

        /// <summary>
        /// Test that OnManipulationStarted and OnManipulationEnded events call as expected
        /// for One Handed and Two Handed.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerEventsBothHands()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.5f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = testObject.transform.rotation;
            testObject.transform.position = initialObjectPosition;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            int manipulationStartedCount = 0;
            int manipulationEndedCount = 0;
            manipHandler.OnManipulationStarted.AddListener((med) => manipulationStartedCount++);
            manipHandler.OnManipulationEnded.AddListener((med) => manipulationEndedCount++);

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            yield return rightHand.Show(new Vector3(0.1f, -0.1f, 0.5f));
            yield return leftHand.Show(new Vector3(-0.1f, -0.1f, 0.5f));
            yield return null;

            // One + Two Handed
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.OneAndTwoHanded;

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(0, manipulationEndedCount);

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(0, manipulationEndedCount);

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(0, manipulationEndedCount);

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.AreEqual(1, manipulationStartedCount);
            Assert.AreEqual(1, manipulationEndedCount);
        }

        /// <summary>
        /// Test that the grab centroid is calculated correctly while rotating
        /// the hands during a two-hand near interaction grab.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerTwoHandedCentroid()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.5f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = testObject.transform.rotation;
            testObject.transform.position = initialObjectPosition;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;

            // Add NearInteractionGrabbable to be able to grab the cube with articulated hands.
            testObject.AddComponent<NearInteractionGrabbable>();

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            yield return rightHand.Show(new Vector3(0.2f, -0.1f, 0.8f));
            yield return leftHand.Show(new Vector3(-0.2f, -0.1f, 0.8f));
            yield return null;

            // Two Handed
            manipHandler.ManipulationType = ManipulationHandler.HandMovementType.TwoHandedOnly;

            // Only testing move/rotate centroid position
            manipHandler.TwoHandedManipulationType = ManipulationHandler.TwoHandedManipulation.MoveRotate;

            // Only testing near manipulation
            manipHandler.AllowFarManipulation = false;

            int manipulationStartedCount = 0;
            int manipulationEndedCount = 0;
            manipHandler.OnManipulationStarted.AddListener((med) => manipulationStartedCount++);
            manipHandler.OnManipulationEnded.AddListener((med) => manipulationEndedCount++);

            // Grab the box.
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Should not have moved (yet!)
            TestUtilities.AssertAboutEqual(testObject.transform.position, initialObjectPosition, $"Object moved when it shouldn't have! Position: {testObject.transform.position:F5}", 0.00001f);

            // The ObjectManipulator should recognize that we've begun manipulation.
            Assert.IsTrue(manipulationStartedCount == 1);

            // Move both hands outwards; the object may be scaled but the position should remain the same.
            yield return rightHand.MoveTo(new Vector3(0.2f, -0.1f, 0.8f));
            yield return leftHand.MoveTo(new Vector3(-0.2f, -0.1f, 0.8f));

            // Should *still* not have moved!
            TestUtilities.AssertAboutEqual(testObject.transform.position, initialObjectPosition, $"Object moved when it shouldn't have! Position: {testObject.transform.position:F5}", 0.00001f);

            // Manipulation should not yet have ended.
            Assert.IsTrue(manipulationEndedCount == 0);

            // Get the grab points before we rotate the hands.
            var leftGrabPoint = manipHandler.GetPointerGrabPoint(leftHand.GetPointer<SpherePointer>().PointerId);
            var rightGrabPoint = manipHandler.GetPointerGrabPoint(rightHand.GetPointer<SpherePointer>().PointerId);
            var originalCentroid = (leftGrabPoint + rightGrabPoint) / 2.0f;

            // List of test conditions for test fuzzing.
            // (position, rotation)
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
                yield return MoveHandsAndCheckCentroid(testCondition.Item1, testCondition.Item2, leftHand, rightHand, manipHandler, initialObjectPosition, originalCentroid, testObject.transform);
            }

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
        }

        /// <summary>
        /// Helper function for ObjectManipulatorTwoHandedCentroid. Will mirror desired handRotation
        /// and handPosition across the two hands, and verify that the centroid was still respected
        /// by the manipulated object.
        /// </summary>
        private IEnumerator MoveHandsAndCheckCentroid(Vector3 handRotationEuler, Vector3 handPosition,
                                                      TestHand leftHand, TestHand rightHand,
                                                      ManipulationHandler mh,
                                                      Vector3 originalObjectPosition, Vector3 originalGrabCentroid,
                                                      Transform testObject)
        {
            // Rotate the hands.
            yield return rightHand.SetRotation(Quaternion.Euler(handRotationEuler.x, handRotationEuler.y, handRotationEuler.z));
            yield return leftHand.SetRotation(Quaternion.Euler(handRotationEuler.x, -handRotationEuler.y, -handRotationEuler.z));

            // Move the hands.
            yield return rightHand.MoveTo(new Vector3(handPosition.x, handPosition.y, handPosition.z));
            yield return leftHand.MoveTo(new Vector3(-handPosition.x, handPosition.y, handPosition.z));

            // Recalculate the new grab centroid.
            var leftGrabPoint = mh.GetPointerGrabPoint(leftHand.GetPointer<SpherePointer>().PointerId);
            var rightGrabPoint = mh.GetPointerGrabPoint(rightHand.GetPointer<SpherePointer>().PointerId);
            var centroid = (leftGrabPoint + rightGrabPoint) / 2.0f;

            // Compute delta between original grab centroid and the new centroid.
            var centroidDelta = centroid - originalGrabCentroid;

            // Ensure grab consistency.
            TestUtilities.AssertAboutEqual(testObject.transform.position, originalObjectPosition + centroidDelta,
                                           $"Object moved did not move according to the delta! Actual position: {testObject.transform.position:F5}, should be {originalObjectPosition + centroidDelta}", 0.00001f);
        }

        /// <summary>
        /// Ensure that a manipulated object has the same rotation as the hand
        /// when RotateAboutObjectCenter is used
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationHandlerRotateAboutObjectCenter()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = Vector3.forward;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.OneHandRotationModeFar = ManipulationHandler.RotateInOneHandType.RotateAboutObjectCenter;

            Quaternion rotateTo = Quaternion.Euler(45, 45, 45);

            TestHand hand = new TestHand(Handedness.Right);
            const int numHandSteps = 1;

            // Rotate the hand and test that the rotations are equal
            yield return hand.Show(new Vector3(0.06f, -0.1f, 0.5f));
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            TestUtilities.AssertAboutEqual(rotateTo, testObject.transform.rotation, "Object moved after it was disabled");

            // Rotate the hand back and test that the rotations still are equal
            yield return hand.SetRotation(Quaternion.identity, numHandSteps);
            yield return null;

            TestUtilities.AssertAboutEqual(Quaternion.identity, testObject.transform.rotation, "Object moved after it was disabled");
        }
    }
}
#endif