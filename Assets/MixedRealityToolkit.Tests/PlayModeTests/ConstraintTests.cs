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

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ConstraintTests
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
        /// Tests that the MoveAxisConstraint works for various axes.
        /// This test uses world space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainMovementAxisWorldSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;

            var constraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = false;

            yield return new WaitForFixedUpdate();
            yield return null;

            float moveAmount = 1.5f;
            const int numHandSteps = 1;

            // Hand pointing at middle of cube
            Vector3 initialHandPosition = new Vector3(0.044f, -0.1f, 0.45f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnMovement = AxisFlags.XAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreNotEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreNotEqual(initialObjectPosition.z, testObject.transform.position.z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnMovement = AxisFlags.YAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreNotEqual(initialObjectPosition.z, testObject.transform.position.z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnMovement = AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreNotEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreEqual(initialObjectPosition.z, testObject.transform.position.z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreNotEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreEqual(initialObjectPosition.z, testObject.transform.position.z);
        }

        /// <summary>
        /// Tests that the MoveAxisConstraint works for various axes.
        /// This test uses local space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainMovementAxisLocalSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            Quaternion initialObjectRotation = Quaternion.Euler(30, 30, 30);
            testObject.transform.rotation = initialObjectRotation;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;

            var constraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = true;

            yield return new WaitForFixedUpdate();
            yield return null;

            float moveAmount = 1.5f;
            const int numHandSteps = 1;
            Quaternion inverse = Quaternion.Inverse(initialObjectRotation);

            // Hand pointing at middle of cube
            Vector3 initialHandPosition = new Vector3(0.044f, -0.1f, 0.45f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnMovement = AxisFlags.XAxis;

            yield return hand.Move((initialObjectRotation * Vector3.one) * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x, 0.01f);
            Assert.AreNotEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y);
            Assert.AreNotEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnMovement = AxisFlags.YAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x);
            Assert.AreEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y, 0.01f);
            Assert.AreNotEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnMovement = AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x);
            Assert.AreNotEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y);
            Assert.AreEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z, 0.01f);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x, 0.01f);
            Assert.AreNotEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y);
            Assert.AreEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z, 0.01f);
        }

        /// <summary>
        /// Tests that the RotationAxisConstraint works for various axes.
        /// This test uses world space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainRotationAxisWorldSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            var constraint = manipHandler.EnsureComponent<RotationAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = false;

            yield return new WaitForFixedUpdate();
            yield return null;
            
            const int numHandSteps = 1;

            // Hand pointing at middle of cube
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(new Vector3(0.044f, -0.1f, 0.45f));
            var rotateTo = Quaternion.Euler(45, 45, 45);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnRotation = AxisFlags.XAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.y);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.z);

            yield return hand.SetRotation(Quaternion.identity, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnRotation = AxisFlags.YAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.x);
            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.y, 0.01f);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.z);

            yield return hand.SetRotation(Quaternion.identity, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnRotation = AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.x);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.y);
            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.z, 0.01f);

            yield return hand.SetRotation(Quaternion.identity, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.y);
            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.z, 0.01f);
        }

        /// <summary>
        /// Tests that the RotationAxisConstraint works for various axes.
        /// This test uses local space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainRotationAxisLocalSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = Quaternion.Euler(-30, -30, -30);
            testObject.transform.rotation = initialObjectRotation;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            var constraint = manipHandler.EnsureComponent<RotationAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = true;

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;
            Quaternion inverse = Quaternion.Inverse(initialObjectRotation);

            // Hand pointing at middle of cube
            Vector3 initialHandPosition = new Vector3(0.044f, -0.1f, 0.45f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);
            var rotateTo = Quaternion.Euler(45, 45, 45);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnRotation = AxisFlags.XAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z);

            yield return hand.SetRotation(initialObjectRotation, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnRotation = AxisFlags.YAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x);
            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y, 0.01f);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z);

            yield return hand.SetRotation(initialObjectRotation, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnRotation = AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y);
            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z, 0.01f);

            yield return hand.SetRotation(initialObjectRotation, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y);
            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z, 0.01f);
        }

        /// <summary>
        /// This tests the minimum and maximum scaling for manipulation.
        /// This test will scale a cube with two hand manipulation and ensure that
        /// maximum and minimum scales are not exceeded.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainScaleMinMax()
        {
            float initialScale = 0.2f;
            float minScale = 0.5f;
            float maxScale = 2f;

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * initialScale;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.TwoHanded;
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
        /// Tests that the FixedDistanceConstraint keeps the manipulated object
        /// at a fixed distance from the constraint object (camera)
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainMovementFixedDistance()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            var constraint = manipHandler.EnsureComponent<FixedDistanceConstraint>();
            constraint.ConstraintTransform = CameraCache.Main.transform;

            float originalDist = (CameraCache.Main.transform.position - testObject.transform.position).magnitude;

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;

            // Hand pointing at middle of cube
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(new Vector3(0.044f, -0.1f, 0.45f));

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Move and test that still same distance from head
            yield return hand.Move(Vector3.one * 0.5f, numHandSteps);
            yield return null;

            Assert.AreEqual(originalDist, (CameraCache.Main.transform.position - testObject.transform.position).magnitude, 0.001f);

            yield return hand.Move(Vector3.one * -1f, numHandSteps);
            yield return null;

            Assert.AreEqual(originalDist, (CameraCache.Main.transform.position - testObject.transform.position).magnitude, 0.001f);
        }

        /// <summary>
        /// Tests that the MaintainApparentSizeConstraint maintains the angle between opposite
        /// corners on the cube
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainScaleApparentSize()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            // add an xy move constraint so that the object's position does not change on screen
            var moveConstraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            moveConstraint.UseLocalSpaceForConstraint = false;
            moveConstraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.YAxis;

            var constraint = manipHandler.EnsureComponent<MaintainApparentSizeConstraint>();

            Vector3 topLeft = testObject.transform.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f));
            Vector3 bottomRight = testObject.transform.TransformPoint(new Vector3(0.5f, -0.5f, -0.5f));
            float originalAngle = Vector3.Angle(topLeft - CameraCache.Main.transform.position, bottomRight - CameraCache.Main.transform.position);

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;

            // Hand pointing at middle of cube
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(new Vector3(0.044f, -0.1f, 0.45f));

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Move and test that still same distance from head
            yield return hand.Move(Vector3.forward * 0.5f, numHandSteps);
            yield return null;

            Vector3 newtopLeft = testObject.transform.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f));
            Vector3 newBottomRight = testObject.transform.TransformPoint(new Vector3(0.5f, -0.5f, -0.5f));
            float newAngle = Vector3.Angle(newtopLeft - CameraCache.Main.transform.position, newBottomRight - CameraCache.Main.transform.position);

            Assert.AreEqual(originalAngle, newAngle, 0.05f);
        }

        /// <summary>
        /// Tests FixedRotationToUserConstraint MaintainRotationToUser should only align with user / camera 
        /// on x / y and not apply rotations in z
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainRotationFixToUser()
        {
            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;
            manipHandler.OneHandRotationModeNear = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

            var rotConstraint = manipHandler.EnsureComponent<FixedRotationToUserConstraint>();
            rotConstraint.TargetTransform = manipHandler.HostTransform;

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
        /// Tests that different constraints can apply for one handed and two handed manipulation
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainByNumberOfHands()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 originalPosition = Vector3.forward;
            testObject.transform.position = originalPosition;
            Quaternion originalRotation = Quaternion.identity;
            testObject.transform.rotation = originalRotation;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            // add an xyz rotate constraint for one handed so we can only move
            var rotateConstraint = manipHandler.EnsureComponent<RotationAxisConstraint>();
            rotateConstraint.UseLocalSpaceForConstraint = false;
            rotateConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.YAxis | AxisFlags.ZAxis;
            rotateConstraint.HandType = ManipulationHandFlags.OneHanded;

            // add an xyz move constraint for two handed so we can only rotate
            var moveConstraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            moveConstraint.UseLocalSpaceForConstraint = false;
            moveConstraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.YAxis | AxisFlags.ZAxis;
            moveConstraint.HandType = ManipulationHandFlags.TwoHanded;

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 10;
            
            TestHand leftHand = new TestHand(Handedness.Left);
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return leftHand.Show(new Vector3(-0.05f, -0.1f, 0.45f));
            yield return rightHand.Show(new Vector3(0.05f, -0.1f, 0.45f));

            // rotate and move left hand
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return leftHand.SetRotation(Quaternion.Euler(45, 45, 45), numHandSteps);
            yield return null;

            TestUtilities.AssertAboutEqual(originalRotation, testObject.transform.rotation, "Rotation should be equal for one handed interaction");

            yield return leftHand.Move((Vector3.left + Vector3.up) * 0.2f, numHandSteps);
            yield return null;

            TestUtilities.AssertNotAboutEqual(originalPosition, testObject.transform.position, "Position should not be equal for one handed interaction");

            // return hand to original pose
            yield return leftHand.SetRotation(Quaternion.identity, numHandSteps);
            yield return leftHand.Move((Vector3.right + Vector3.down) * 0.2f, numHandSteps);
            yield return null;

            // grab with both hands and move/rotate
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return leftHand.Move((Vector3.left + Vector3.up) * 0.2f, numHandSteps);
            yield return null;

            TestUtilities.AssertNotAboutEqual(originalRotation, testObject.transform.rotation, "Rotation should not be equal for two handed interaction");
            TestUtilities.AssertAboutEqual(originalPosition, testObject.transform.position, "Position should be equal for two handed interaction");
        }

        /// <summary>
        /// Tests that different constraints can apply for near and far manipulation
        /// </summary>
        [UnityTest]
        public IEnumerator CombinedConstraintFarNear()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 originalPosition = Vector3.forward;
            testObject.transform.position = originalPosition;
            Quaternion originalRotation = Quaternion.identity;
            testObject.transform.rotation = originalRotation;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();

            // add an xyz rotate constraint for one handed so we can only move
            var rotateConstraint = manipHandler.EnsureComponent<RotationAxisConstraint>();
            rotateConstraint.UseLocalSpaceForConstraint = false;
            rotateConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.YAxis | AxisFlags.ZAxis;
            rotateConstraint.ProximityType = ManipulationProximityFlags.Near;

            // add an xyz move constraint for two handed so we can only rotate
            var moveConstraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            moveConstraint.UseLocalSpaceForConstraint = false;
            moveConstraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.YAxis | AxisFlags.ZAxis;
            moveConstraint.ProximityType = ManipulationProximityFlags.Far;

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;

            TestHand leftHand = new TestHand(Handedness.Left);
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return leftHand.Show(new Vector3(-0.05f, 0, 1));
            yield return rightHand.Show(new Vector3(0.05f, 0, 1));
            yield return null;

            // near interaction
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return leftHand.Move((Vector3.left + Vector3.up) * 0.2f, numHandSteps);
            yield return null;

            TestUtilities.AssertAboutEqual(originalRotation, testObject.transform.rotation, "Rotation should be equal for near interaction");
            TestUtilities.AssertNotAboutEqual(originalPosition, testObject.transform.position, "Position should not be equal for near interaction");

            // far interaction
            yield return leftHand.Move((Vector3.right + Vector3.down) * 0.2f, numHandSteps);
            yield return null;

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return null;

            yield return leftHand.MoveTo(new Vector3(-0.05f, -0.1f, 0.45f), numHandSteps);
            yield return rightHand.MoveTo(new Vector3(0.05f, -0.1f, 0.45f), numHandSteps);
            yield return null;

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return leftHand.Move((Vector3.left + Vector3.up) * 0.2f, numHandSteps);
            yield return null;

            TestUtilities.AssertNotAboutEqual(originalRotation, testObject.transform.rotation, "Rotation should not be equal for far interaction");
            TestUtilities.AssertAboutEqual(originalPosition, testObject.transform.position, "Position should be equal for far interaction");
        }

        /// <summary>
        /// Tests that FaceUserConstraint updates the rotation to face the user
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainRotationFaceUser()
        {
            TestUtilities.PlayspaceToOriginLookingForward();
            
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = Vector3.forward;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;
            manipHandler.OneHandRotationModeNear = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

            var rotConstraint = manipHandler.EnsureComponent<FaceUserConstraint>();
            rotConstraint.TargetTransform = manipHandler.HostTransform;
            rotConstraint.FaceAway = false;

            // Face user first
            const int numHandSteps = 10;
            TestHand hand = new TestHand(Handedness.Right);

            yield return hand.Show(new Vector3(0.05f, -0.1f, 0.45f));
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            Vector3 cameraToObject() => testObject.transform.position - CameraCache.Main.transform.position;
            Vector3 checkCollinear() => Vector3.Cross(testObject.transform.forward, cameraToObject());
            float checkNegative() => Vector3.Dot(testObject.transform.forward, cameraToObject());

            TestUtilities.AssertAboutEqual(checkCollinear(), Vector3.zero, "Object not facing camera", 0.002f);
            Assert.Less(checkNegative(), 0, "Object facing away");

            // Move the hand
            yield return hand.Move(new Vector3(0.2f, 0.2f, 0), numHandSteps);
            yield return null;

            Assert.AreNotEqual(Vector3.forward, testObject.transform.position); // ensure the object moved
            TestUtilities.AssertAboutEqual(checkCollinear(), Vector3.zero, "Object not facing camera", 0.002f);
            Assert.Less(checkNegative(), 0, "Object facing away");

            // Face away from user
            rotConstraint.FaceAway = true;
            yield return hand.Move(new Vector3(-0.2f, -0.2f, 0), numHandSteps);
            yield return null;

            TestUtilities.AssertAboutEqual(checkCollinear(), Vector3.zero, "Object not facing camera", 0.002f);
            Assert.Greater(checkNegative(), 0, "Object facing away");

            // Move the hand
            yield return hand.Move(new Vector3(0.2f, 0.2f, 0), numHandSteps);
            yield return null;

            Assert.AreNotEqual(Vector3.forward, testObject.transform.position); // ensure the object moved
            TestUtilities.AssertAboutEqual(checkCollinear(), Vector3.zero, "Object not facing camera", 0.002f);
            Assert.Greater(checkNegative(), 0, "Object facing away");
        }

        /// <summary>
        /// Tests that FixedRotationToWorldConstraint maintains the original rotation of the manipulated object
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainRotationFixToWorld()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = Vector3.forward;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ManipulationHandFlags.OneHanded;
            manipHandler.OneHandRotationModeNear = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

            var rotConstraint = manipHandler.EnsureComponent<FixedRotationToWorldConstraint>();
            rotConstraint.TargetTransform = manipHandler.HostTransform;

            // Face user first
            const int numHandSteps = 1;
            TestHand hand = new TestHand(Handedness.Right);

            yield return hand.Show(new Vector3(0.05f, -0.1f, 0.45f));
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;

            // rotate the hand
            yield return hand.SetRotation(Quaternion.Euler(45, 45, 45), numHandSteps);
            yield return null;

            Assert.AreEqual(Quaternion.identity, testObject.transform.rotation);

            // move the hand
            yield return hand.Move(Vector3.right * 0.2f, numHandSteps);
            yield return null;

            Assert.AreEqual(Quaternion.identity, testObject.transform.rotation);

            // rotate the head
            CameraCache.Main.transform.Rotate(new Vector3(0, 10, 0));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(Quaternion.identity, testObject.transform.rotation);
        }
    }
}
#endif