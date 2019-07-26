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
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SolverTests : BasePlayModeTests
    {
        private const float DistanceThreshold = 2.5f;

        /// <summary>
        /// Internal class used to store data for setup
        /// </summary>
        protected class SetupData
        {
            public SolverHandler handler;
            public Solver solver;
            public GameObject target;
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

            yield return new WaitForSeconds(3.0f);

            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, Vector3.zero), DistanceThreshold);

            // Test orbital around custom override
            testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
            testObjects.handler.SetTransformOverride(transformOverride.transform);

            yield return new WaitForSeconds(3.0f);

            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, customTransformPos), DistanceThreshold);

            yield return null;
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

            yield return null;

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            yield return TestHandSolver(testObjects.target, inputSimulationService, rightHandPos, Handedness.Right);

            // Test orbital around left hand
            yield return TestHandSolver(testObjects.target, inputSimulationService, leftHandPos, Handedness.Left);
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
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
            wall.transform.position = Vector3.forward * 10.0f;

            yield return null;

            // Instantiate our test gameobject with solver. 
            // Set layer to ignore raycast so solver doesn't raycast itself (i.e BoxCollider)
            var testObjects = InstantiateTestSolver<SurfaceMagnetism>();
            testObjects.target.layer = LayerMask.NameToLayer("Ignore Raycast");

            yield return new WaitForSeconds(1.0f);

            // Confirm that the surfacemagnetic cube is about on the wall straight ahead
            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, wall.transform.position), DistanceThreshold);

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
            yield return new WaitForSeconds(1.0f);

            // Confirm that the surfacemagnetic cube is on the wall with camera rotated
            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, hitInfo.point), DistanceThreshold);
        }

#region Test Helpers

        private IEnumerator TestHandSolver(GameObject target, InputSimulationService inputSimulationService, Vector3 handPos, Handedness hand)
        {
            yield return PlayModeTestUtilities.ShowHand(hand, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, handPos);

            // Give time for cube to float to hand
            yield return new WaitForSeconds(3.0f);

            Vector3 handOrbitalPos = target.transform.position;
            Assert.LessOrEqual(Vector3.Distance(handOrbitalPos, handPos), DistanceThreshold);

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            yield return new WaitForSeconds(3.0f);
        }

        private SetupData InstantiateTestSolver<T>() where T: Solver
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);

            Solver solver = cube.AddComponent<T>();
            Assert.IsNotNull(solver, "AddComponent<T>() returned null");

            SolverHandler handler = cube.GetComponent<SolverHandler>();
            Assert.IsNotNull(handler, "GetComponent<SolverHandler>() returned null");

            return new SetupData()
            {
                handler = handler,
                solver = solver,
                target = cube
            };
        }
#endregion
    }
}
#endif

