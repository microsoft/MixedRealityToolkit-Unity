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

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SolverTests : BasePlayModeTests
    {
        private const float DistanceThreshold = 1.5f;
        private const float HandDistanceThreshold = 0.5f;
        private const float SolverUpdateWaitTime = 1.0f; //seconds

        /// <summary>
        /// Internal class used to store data for setup
        /// </summary>
        protected class SetupData
        {
            public SolverHandler handler;
            public Solver solver;
            public GameObject target;
        }

        private List<SetupData> setupDataList = new List<SetupData>();

        [TearDown]
        public override void TearDown()
        {
            foreach (var setupData in setupDataList)
            {
                Object.Destroy(setupData?.target);
            }

            base.TearDown();
        }

        /// <summary>
        /// Test adding solver dynamically at runtime to gameobject
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestRuntimeInstantiation()
        {
            InstantiateTestSolver<Orbital>();

            yield return null;
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestTargetTypes()
        {
            Vector3 rightHandPos = Vector3.right * 50.0f;
            Vector3 customTransformPos = Vector3.up * 50.0f;

            var transformOverride = new GameObject("Override");
            transformOverride.transform.position = customTransformPos;

            var testObjects = InstantiateTestSolver<Orbital>();

            // Set solver handler to track hands
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            yield return TestHandSolver(testObjects.target, inputSimulationService, rightHandPos, Handedness.Right);

            // Test orbital around head
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;

            yield return WaitForFrames(2);

            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, Camera.main.transform.position), DistanceThreshold);

            // Test orbital around custom override
            testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
            testObjects.handler.TransformOverride = transformOverride.transform;

            yield return WaitForFrames(2);

            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, customTransformPos), DistanceThreshold);

            yield return WaitForFrames(2);
        }

        /// <summary>
        /// Tests solver handler's ability to switch hands
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestHandModality()
        {
            var testObjects = InstantiateTestSolver<Orbital>();

            // Set solver handler to track hands
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;

            // Set and save revelant positions
            Vector3 rightHandPos = Vector3.right * 20.0f;
            Vector3 leftHandPos = Vector3.right * -20.0f;

            yield return WaitForFrames(2);

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            yield return TestHandSolver(testObjects.target, inputSimulationService, rightHandPos, Handedness.Right);

            // Test orbital around left hand
            yield return TestHandSolver(testObjects.target, inputSimulationService, leftHandPos, Handedness.Left);

            // Test orbital with both hands visible
            yield return PlayModeTestUtilities.ShowHand(Handedness.Left, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, leftHandPos);
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, rightHandPos);

            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = testObjects.target.transform.position;
            Assert.LessOrEqual(Vector3.Distance(handOrbitalPos, leftHandPos), DistanceThreshold);
        }

        /// <summary>
        /// Test Surface Magnetism against "wall" and that attached object falls head direction
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestSurfaceMagnetism()
        {
            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Build wall to collide against
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(25.0f, 25.0f, 0.2f);
            wall.transform.Rotate(Vector3.up, 180.0f); // Rotate wall so forward faces camera
            wall.transform.position = Vector3.forward * 10.0f;

            yield return WaitForFrames(2);

            // Instantiate our test gameobject with solver. 
            // Set layer to ignore raycast so solver doesn't raycast itself (i.e BoxCollider)
            var testObjects = InstantiateTestSolver<SurfaceMagnetism>();
            testObjects.target.layer = LayerMask.NameToLayer("Ignore Raycast");
            SurfaceMagnetism surfaceMag = testObjects.solver as SurfaceMagnetism;

            var targetTransform = testObjects.target.transform;
            var cameraTransform = CameraCache.Main.transform;

            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is about on the wall straight ahead
            Assert.LessOrEqual(Vector3.Distance(targetTransform.position, wall.transform.position), DistanceThreshold);

            // Rotate the camera
            Vector3 cameraDir = Vector3.forward + Vector3.right;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(cameraDir);
            });

            // Calculate where our camera hits the wall
            RaycastHit hitInfo;
            Assert.IsTrue(UnityEngine.Physics.Raycast(Vector3.zero, cameraDir, out hitInfo), "Raycast from camera did not hit wall");

            // Let SurfaceMagnetism update
            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is on the wall with camera rotated
            Assert.LessOrEqual(Vector3.Distance(targetTransform.position, hitInfo.point), DistanceThreshold);

            // Default orientation mode is TrackedTarget, test object should be facing camera
            Assert.IsTrue(Mathf.Approximately(-1.0f, Vector3.Dot(targetTransform.forward.normalized, cameraTransform.forward.normalized)));

            // Change default orientation mode to surface normal
            surfaceMag.CurrentOrientationMode = SurfaceMagnetism.OrientationMode.SurfaceNormal;

            yield return WaitForFrames(2);

            // Test object should now be facing into the wall (i.e Z axis)
            Assert.IsTrue(Mathf.Approximately(1.0f, Vector3.Dot(targetTransform.forward.normalized, Vector3.forward)));
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestInBetween()
        {
            // Build "posts" to put solved object between
            var leftPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftPost.transform.position = Vector3.forward * 10.0f - Vector3.right * 10.0f;

            var rightPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightPost.transform.position = Vector3.forward * 10.0f + Vector3.right * 10.0f;

            // Instantiate our test gameobject with solver. 
            var testObjects = InstantiateTestSolver<InBetween>();

            testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
            testObjects.handler.TransformOverride = leftPost.transform;

            InBetween inBetween = testObjects.solver as InBetween;
            Assert.IsNotNull(inBetween, "Solver cast to InBetween is null");

            inBetween.SecondTrackedObjectType = TrackedObjectType.CustomOverride;
            inBetween.SecondTransformOverride = rightPost.transform;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 10.0f, "InBetween solver did not place object in middle of posts");

            inBetween.PartwayOffset = 0.0f;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, rightPost.transform.position, "InBetween solver did not move to the left post");
        }

        /// <summary>
        /// Test the HandConstraint to make sure it tracks hands correctly.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestHandConstraint()
        {
            // Instantiate our test gameobject with solver.
            var testObjects = InstantiateTestSolver<HandConstraint>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandness = Handedness.Both;

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraint solver did not start at the origin");

            // Add a right hand.
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);

            // Move the hand to 0, 0, 1 and ensure that the hand constraint followed.
            var handPosition = Vector3.forward;
            yield return rightHand.MoveTo(handPosition);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Make sure the solver is not in the same location as the hand because the solver should move to a hand safe zone.
            TestUtilities.AssertNotAboutEqual(testObjects.target.transform.position, handPosition, "HandConstraint solver is in the same location of the hand when it should be slightly offset from the hand.");

            // Make sure the solver is near the hand.
            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, handPosition), HandDistanceThreshold, "HandConstraint solver is not within {0} units of the hand", HandDistanceThreshold);

            // Hide the right hand and create a left hand.
            yield return rightHand.Hide();
            var leftHand = new TestHand(Handedness.Left);
            handPosition = Vector3.zero;
            yield return leftHand.Show(handPosition);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Make sure the solver is now near the other hand.
            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, handPosition), HandDistanceThreshold, "HandConstraint solver is not within {0} units of the hand", HandDistanceThreshold);
        }

        /// <summary>
        /// Test the Overlap solver and make sure it tracks the left simulated hand exactly
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestOverlap()
        {
            // Instantiate our test gameobject with solver.
            var testObjects = InstantiateTestSolver<Overlap>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            var targetTransform = testObjects.target.transform;

            TestUtilities.AssertAboutEqual(targetTransform.position, Vector3.zero, "Overlap not at original position");
            TestUtilities.AssertAboutEqual(targetTransform.rotation, Quaternion.identity, "Overlap not at original rotation");

            // Test that the solver flies to the position of the left hand
            var handPosition = Vector3.forward - Vector3.right;
            var handRotation = Quaternion.LookRotation(handPosition);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handPosition);
            yield return leftHand.SetRotation(handRotation);
            
            yield return WaitForFrames(2);
            var hand = PlayModeTestUtilities.GetInputSimulationService().GetHandDevice(Handedness.Left);
            Assert.IsNotNull(hand);
            Assert.IsTrue(hand.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose pose));

            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver is not at the same position as the left hand.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);

            // Make sure the solver did not move when hand was hidden
            yield return leftHand.Hide();
            yield return WaitForFrames(2);
            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver moved when the hand was hidden.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);
        }


        #region Test Helpers

        private IEnumerator TestHandSolver(GameObject target, InputSimulationService inputSimulationService, Vector3 handPos, Handedness hand)
        {
            yield return PlayModeTestUtilities.ShowHand(hand, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, handPos);

            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = target.transform.position;
            Assert.LessOrEqual(Vector3.Distance(handOrbitalPos, handPos), DistanceThreshold);

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            yield return WaitForFrames(2);
        }

        private SetupData InstantiateTestSolver<T>() where T: Solver
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);

            Solver solver = cube.AddComponent<T>();
            Assert.IsNotNull(solver, "AddComponent<T>() returned null");

            // Set Solver lerp times to 0 so we can process tests faster instead of waiting for transforms to update/apply
            solver.MoveLerpTime = 0.0f;
            solver.RotateLerpTime = 0.0f;
            solver.ScaleLerpTime = 0.0f;

            SolverHandler handler = cube.GetComponent<SolverHandler>();
            Assert.IsNotNull(handler, "GetComponent<SolverHandler>() returned null");

           var setupData =  new SetupData()
            {
                handler = handler,
                solver = solver,
                target = cube
            };

            setupDataList.Add(setupData);

            return setupData;
        }

        private IEnumerator WaitForFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }

#endregion
    }
}
#endif

