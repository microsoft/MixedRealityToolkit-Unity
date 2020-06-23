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

using Assert = UnityEngine.Assertions.Assert;
using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Experimental
{
    /// <summary>
    /// Tests for runtime behavior of the ElasticSystem.
    /// </summary>
    public class LinearElasticSystemTests
    {
        #region Utilities

        // Some arbitrary, yet reasonable, values.
        ElasticExtentProperties<float> extentProperties = new ElasticExtentProperties<float>
        {
            MinStretch = 0.0f,
            MaxStretch = 10.0f,
            SnapToEnds = false,
            SnapPoints = new float[0],
        };

        // Some arbitrary, yet reasonable, values.
        ElasticProperties elasticProperties = new ElasticProperties
        {
            Mass = 0.1f,
            HandK = 4.0f,
            EndK = 2.0f,
            SnapK = 1.0f,
            SnapRadius = 1.0f,
            Drag = 0.2f
        };

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            PlayModeTestUtilities.TearDown();
        }
        #endregion

        /// <summary>
        /// Tests the sign of the force applied when stretched beyond the endpoints.
        /// </summary>
        [UnityTest]
        public IEnumerator EndcapForceSign()
        {
            LinearElasticSystem les = new LinearElasticSystem(0.0f, 0.0f, extentProperties, elasticProperties);

            // Goal position for the elastic system to stretch towards
            var goalValue = 15.0f;

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simlulates independently from Unity's frame system.
            for (int i = 0; i < 50; i++){
                les.ComputeIteration(goalValue, 0.1f);
            }

            // Get the equilibrium value from the system.
            var equilibrium = les.GetCurrentValue();
            Debug.Assert(equilibrium < goalValue, $"Stretching beyond max limit should result in equilibrium value less than goal value, equilibrium: {equilibrium}");

            // Compute one small iteration, covering 50 milliseconds.
            var newValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(newValue < equilibrium, $"Elastic system should have contracted towards endpoint when released, actual value: {newValue}, equilibrium: {equilibrium}");
            Debug.Assert(les.GetCurrentVelocity() < 0.0f, $"Elastic system should now have negative velocity, actual velocity: {les.GetCurrentVelocity()}");

            // Compute one more small iteration (50 milliseconds)
            var secondNewValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(secondNewValue < equilibrium, $"Elastic system should have contracted towards endpoint when released, actual value: {secondNewValue}, equilibrium: {equilibrium}");
            Debug.Assert(secondNewValue < newValue, $"Elastic system should have contracted further towards endpoint, new value: {secondNewValue}, last value: {newValue}");
            Debug.Assert(les.GetCurrentVelocity() < 0.0f, $"Elastic system should still have negative velocity, actual velocity: {les.GetCurrentVelocity()}");

            // Now, we test pulling the elastic negative, and performing similar checks.
            goalValue = -5.0f;

            // Let the elastic system come to an equilibrium
            for (int i = 0; i < 50; i++)
            {
                les.ComputeIteration(goalValue, 0.1f);
            }

            // Get the equilibrium value from the system.
            equilibrium = les.GetCurrentValue();
            Debug.Assert(equilibrium > goalValue, $"Stretching beyond minimum limit should result in equilibrium value greater than goal value, equilibrium: {equilibrium}");

            // Compute one small iteration, covering 50 milliseconds.
            newValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(newValue > equilibrium, $"Elastic system should have contracted towards endpoint when released, actual value: {newValue}, equilibrium: {equilibrium}");
            Debug.Assert(les.GetCurrentVelocity() > 0.0f, $"Elastic system should now have positive velocity, actual velocity: {les.GetCurrentVelocity()}");

            // Compute one more small iteration (50 milliseconds)
            secondNewValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(secondNewValue > equilibrium, $"Elastic system should have contracted towards endpoint when released, actual value: {secondNewValue}, equilibrium: {equilibrium}");
            Debug.Assert(secondNewValue > newValue, $"Elastic system should have contracted further towards endpoint, new value: {secondNewValue}, last value: {newValue}");
            Debug.Assert(les.GetCurrentVelocity() > 0.0f, $"Elastic system should still have positive velocity, actual velocity: {les.GetCurrentVelocity()}");

            yield return null;
        }

        /// <summary>
        /// Tests that snap forces at the endpoint behave correctly according to the SnapToEnd property.
        /// </summary>
        [UnityTest]
        public IEnumerator EndpointSnapping()
        {
            // Default extent properties have SnapToEnd set to false.
            LinearElasticSystem les = new LinearElasticSystem(0.0f, 0.0f, extentProperties, elasticProperties);

            // Goal position for the elastic system to seek.
            // We will let the system settle to right next to the endpoint.
            var goalValue = 9.5f;

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simlulates independently from Unity's frame system.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be basically equal to the goal value, given that endpoint snapping is disabled.
            var equilibrium = les.GetCurrentValue();
            Assert.AreApproximatelyEqual(goalValue, les.GetCurrentValue(), $"Equilibrium should be roughly equal to goal value. Goal: {goalValue}");

            // Compute one small iteration, covering 50 milliseconds.
            var newValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have stayed still.
            Assert.AreApproximatelyEqual(newValue, equilibrium, $"Elastic system should have stayed mostly still when released, actual value: {newValue}, equilibrium: {equilibrium}");
            Assert.AreApproximatelyEqual(les.GetCurrentVelocity(), 0.0f, $"Elastic system should have zero velocity, actual velocity: {les.GetCurrentVelocity()}");

            // Copy the extent properties, but now we enable end snapping.
            var newExtentProperties = extentProperties;
            newExtentProperties.SnapToEnds = true;

            // Create new system.
            les = new LinearElasticSystem(0.0f, 0.0f, newExtentProperties, elasticProperties);

            // Again, right next to the endpoint.
            goalValue = 9.5f;

            // Let the elastic system come to an equilibrium.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should now be slightly bigger than the goal value, due to endpoint snapping.
            equilibrium = les.GetCurrentValue();
            Debug.Assert(equilibrium > goalValue, $"Equilibrium should be slightly greater than goal value. Goal: {goalValue}, Current: {equilibrium}");
            Debug.Assert(equilibrium < 10.0f, $"Equilibrium should still be less than the endpoint value");

            // Compute one small iteration, covering 50 milliseconds.
            newValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have moved towards the endpoint
            Debug.Assert(newValue > equilibrium, $"Elastic system should snap towards endpoint, actual value: {newValue}, equilibrium: {equilibrium}");
            Debug.Assert(les.GetCurrentVelocity() > 0.0f, $"Elastic system should have positive velocity, actual velocity: {les.GetCurrentVelocity()}");

            yield return null;
        }
    }
}
#endif