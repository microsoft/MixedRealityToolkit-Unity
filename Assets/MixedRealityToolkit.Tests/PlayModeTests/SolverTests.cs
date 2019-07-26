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
        private const float HandOrbitalDistanceThreshold = 2.5f;

        protected class SetupData
        {
            public SolverHandler handler;
            public Solver solver;
            public GameObject target;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestRuntimeInstantiation()
        {
            InstantiateTestSolver<Orbital>();

            yield return null;
        }

        /// <summary>
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

            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, Vector3.zero), HandOrbitalDistanceThreshold);

            // Test orbital around custom override
            testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
            testObjects.handler.SetTransformOverride(transformOverride.transform);

            yield return new WaitForSeconds(3.0f);

            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, customTransformPos), HandOrbitalDistanceThreshold);

            yield return null;
        }

        /// <summary>
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

        private IEnumerator TestHandSolver(GameObject target, InputSimulationService inputSimulationService, Vector3 handPos, Handedness hand)
        {
            yield return PlayModeTestUtilities.ShowHand(hand, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, handPos);

            // Give time for cube to float to hand
            yield return new WaitForSeconds(3.0f);

            Vector3 handOrbitalPos = target.transform.position;
            Assert.LessOrEqual(Vector3.Distance(handOrbitalPos, handPos), HandOrbitalDistanceThreshold);

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            yield return new WaitForSeconds(3.0f);
        }

        private SetupData InstantiateTestSolver<T>() where T: Solver
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

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
    }
}
#endif

