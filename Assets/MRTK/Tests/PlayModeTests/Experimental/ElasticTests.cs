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

using Assert = UnityEngine.Assertions.Assert;
using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Experimental
{
    /// <summary>
    /// Tests for runtime behavior of the ElasticSystem.
    /// </summary>
    public class ElasticSystemTests
    {
        #region Utilities

        // Some arbitrary, yet reasonable, values.
        LinearElasticExtent linearExtent = new LinearElasticExtent
        {
            MinStretch = 0.0f,
            MaxStretch = 10.0f,
            SnapToEnds = false,
            SnapPoints = new float[0],
            SnapRadius = 1.0f
        };

        // Some arbitrary, yet reasonable, values.
        VolumeElasticExtent volumeExtent = new VolumeElasticExtent
        {
            StretchBounds = new Bounds(Vector3.zero, Vector3.one),
            UseBounds = true,
            SnapPoints = new Vector3[0],
            SnapRadius = 1.0f
        };

        // Some arbitrary, yet reasonable, values.
        QuaternionElasticExtent quatExtent = new QuaternionElasticExtent
        {
            SnapPoints = new Vector3[0],
            SnapRadius = 45.0f
        };

        // Some arbitrary, yet reasonable, values.
        ElasticProperties elasticProperties = new ElasticProperties
        {
            Mass = 0.1f,
            HandK = 4.0f,
            EndK = 2.0f,
            SnapK = 1.0f,
            Drag = 0.2f
        };

        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
            yield return null;
        }
        #endregion

        /// <summary>
        /// Tests the sign of the force applied when stretched beyond the endpoints.
        /// </summary>
        [UnityTest]
        public IEnumerator LinearEndcapForceSign()
        {
            LinearElasticSystem les = new LinearElasticSystem(0.0f, 0.0f, linearExtent, elasticProperties);

            // Goal position for the elastic system to stretch towards
            var goalValue = 15.0f;

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simulates independently from Unity's frame system.
            for (int i = 0; i < 50; i++){
                les.ComputeIteration(goalValue, 0.1f);
            }

            // Get the equilibrium value from the system.
            var equilibrium = les.GetCurrentValue();
            Debug.Assert(
                equilibrium < goalValue,
                $"Stretching beyond max limit should result in equilibrium value less than goal value, equilibrium: {equilibrium}"
            );

            // Compute one small iteration, covering 50 milliseconds.
            var newValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                newValue < equilibrium,
                $"Elastic system should have contracted towards endpoint when released, actual value: {newValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                les.GetCurrentVelocity() < 0.0f,
                $"Elastic system should now have negative velocity, actual velocity: {les.GetCurrentVelocity()}"
            );

            // Compute one more small iteration (50 milliseconds)
            var secondNewValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                secondNewValue < equilibrium,
                $"Elastic system should have contracted towards endpoint when released, actual value: {secondNewValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                secondNewValue < newValue,
                $"Elastic system should have contracted further towards endpoint, new value: {secondNewValue}, last value: {newValue}"
            );
            Debug.Assert(
                les.GetCurrentVelocity() < 0.0f,
                $"Elastic system should still have negative velocity, actual velocity: {les.GetCurrentVelocity()}"
            );

            // Now, we test pulling the elastic negative, and performing similar checks.
            goalValue = -5.0f;

            // Let the elastic system come to an equilibrium
            for (int i = 0; i < 50; i++)
            {
                les.ComputeIteration(goalValue, 0.1f);
            }

            // Get the equilibrium value from the system.
            equilibrium = les.GetCurrentValue();
            Debug.Assert(
                equilibrium > goalValue,
                $"Stretching beyond minimum limit should result in equilibrium value greater than goal value, equilibrium: {equilibrium}"
            );

            // Compute one small iteration, covering 50 milliseconds.
            newValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                newValue > equilibrium,
                $"Elastic system should have contracted towards endpoint when released, actual value: {newValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                les.GetCurrentVelocity() > 0.0f,
                $"Elastic system should now have positive velocity, actual velocity: {les.GetCurrentVelocity()}"
            );

            // Compute one more small iteration (50 milliseconds)
            secondNewValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                secondNewValue > equilibrium,
                $"Elastic system should have contracted towards endpoint when released, actual value: {secondNewValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                secondNewValue > newValue,
                $"Elastic system should have contracted further towards endpoint, new value: {secondNewValue}, last value: {newValue}"
            );
            Debug.Assert(
                les.GetCurrentVelocity() > 0.0f,
                $"Elastic system should still have positive velocity, actual velocity: {les.GetCurrentVelocity()}"
            );

            yield return null;
        }

        /// <summary>
        /// Tests that snap forces at the endpoint behave correctly according to the SnapToEnd property.
        /// </summary>
        [UnityTest]
        public IEnumerator LinearEndpointSnapping()
        {
            // Default extent properties have SnapToEnd set to false.
            LinearElasticSystem les = new LinearElasticSystem(0.0f, 0.0f, linearExtent, elasticProperties);

            // Goal position for the elastic system to seek.
            // We will let the system settle to right next to the endpoint.
            var goalValue = 9.5f;

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simulates independently from Unity's frame system.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be basically equal to the goal value, given that endpoint snapping is disabled.
            var equilibrium = les.GetCurrentValue();
            Assert.AreApproximatelyEqual(
                goalValue,
                les.GetCurrentValue(),
                $"Equilibrium should be roughly equal to goal value. Goal: {goalValue}"
            );

            // Compute one small iteration, covering 50 milliseconds.
            var newValue = les.ComputeIteration(equilibrium, 0.05f);

            // The system should have stayed still.
            Assert.AreApproximatelyEqual(
                newValue,
                equilibrium,
                $"Elastic system should have stayed mostly still when released, actual value: {newValue}, equilibrium: {equilibrium}"
            );
            Assert.AreApproximatelyEqual(
                les.GetCurrentVelocity(),
                0.0f,
                $"Elastic system should have zero velocity, actual velocity: {les.GetCurrentVelocity()}"
            );

            // Copy the extent properties, but now we enable end snapping.
            var newExtentProperties = linearExtent;
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

        /// <summary>
        /// Tests that snap forces at the snap points behave correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator LinearSnapPointSnapping()
        {
            // Make a copy of the default linear elastic extent.
            var snappingExtent = linearExtent;

            // Set some decent snap points
            snappingExtent.SnapPoints = new float[] { 2.0f, 5.0f };

            // Construct our system.
            LinearElasticSystem les = new LinearElasticSystem(0.0f, 0.0f, snappingExtent, elasticProperties);

            // Goal position for the elastic system to seek.
            // We will let the system settle to right next to one of the snapping point.
            var goalValue = 4.5f;

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simulates independently from Unity's frame system.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be near the goal value, but slightly pulled towards the snapping point.
            var equilibrium = les.GetCurrentValue();

            Debug.Assert(equilibrium > goalValue, $"Equilibrium should be slightly greater than goal value. Goal: {goalValue}, Current: {equilibrium}");
            Debug.Assert(equilibrium < 5.0f, $"Equilibrium should still be less than the snapping value");

            // Move the goal value to slightly more than the other snapping point (2.0)
            goalValue = 2.5f;

            // Let the elastic system come to an equilibrium.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be near the goal value, but slightly pulled towards the snapping point.
            equilibrium = les.GetCurrentValue();

            Debug.Assert(equilibrium < goalValue, $"Equilibrium should be slightly less than goal value. Goal: {goalValue}, Current: {equilibrium}");
            Debug.Assert(equilibrium > 2.0f, $"Equilibrium should still be greater than the snapping value");

            yield return null;
        }

        /// <summary>
        /// Tests the direction of the force applied when stretched beyond the bounds.
        /// </summary>
        [UnityTest]
        public IEnumerator VolumeBoundsForce()
        {
            // VolumeExtent is configured with bounds centered at (0,0,0), with size (1,1,1)
            VolumeElasticSystem ves = new VolumeElasticSystem(Vector3.zero, Vector3.zero, volumeExtent, elasticProperties);

            // Goal position for the elastic system to stretch towards.
            // Will be beyond the configured bounds!
            var goalValue = 15.0f * Vector3.one;

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simulates independently from Unity's frame system.
            for (int i = 0; i < 50; i++)
            {
                ves.ComputeIteration(goalValue, 0.1f);
            }

            // Get the equilibrium value from the system.
            var equilibrium = ves.GetCurrentValue();
            Debug.Assert(
                SignedVectorLessThan(equilibrium, goalValue),
                $"Stretching beyond max limit should result in equilibrium value less than goal value, equilibrium: {equilibrium}"
            );

            // Compute one small iteration, covering 50 milliseconds.
            var newValue = ves.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                SignedVectorLessThan(equilibrium, goalValue),
                $"Elastic system should have contracted towards endpoint when released, actual value: {newValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                SignedVectorLessThan(ves.GetCurrentVelocity(), Vector3.zero),
                $"Elastic system should now have negative velocity, actual velocity: {ves.GetCurrentVelocity()}"
            );

            // Compute one more small iteration (50 milliseconds)
            var secondNewValue = ves.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                secondNewValue.magnitude < equilibrium.magnitude,
                $"Elastic system should have contracted towards endpoint when released, actual value: {secondNewValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                secondNewValue.magnitude < newValue.magnitude,
                $"Elastic system should have contracted further towards endpoint, new value: {secondNewValue}, last value: {newValue}"
            );
            Debug.Assert(
                ves.GetCurrentVelocity().x < 0.0f &&
                ves.GetCurrentVelocity().y < 0.0f &&
                ves.GetCurrentVelocity().z < 0.0f,
                $"Elastic system should still have negative velocity, actual velocity: {ves.GetCurrentVelocity()}"
            );

            // Now, we test pulling the elastic negative, and performing similar checks.
            goalValue = -5.0f * Vector3.one;

            // Let the elastic system come to an equilibrium
            for (int i = 0; i < 50; i++)
            {
                ves.ComputeIteration(goalValue, 0.1f);
            }

            // Get the equilibrium value from the system.
            equilibrium = ves.GetCurrentValue();
            Debug.Assert(
                SignedVectorGreaterThan(equilibrium, goalValue),
                $"Stretching beyond minimum limit should result in equilibrium value greater than goal value, equilibrium: {equilibrium}"
            );

            // Compute one small iteration, covering 50 milliseconds.
            newValue = ves.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                SignedVectorGreaterThan(newValue, equilibrium),
                $"Elastic system should have contracted towards endpoint when released, actual value: {newValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                SignedVectorGreaterThan(ves.GetCurrentVelocity(), Vector3.zero),
                $"Elastic system should now have positive velocity, actual velocity: {ves.GetCurrentVelocity()}"
            );

            // Compute one more small iteration (50 milliseconds)
            secondNewValue = ves.ComputeIteration(equilibrium, 0.05f);

            // The system should have shrunk back towards the endpoint.
            Debug.Assert(
                SignedVectorGreaterThan(secondNewValue, equilibrium),
                $"Elastic system should have contracted towards endpoint when released, actual value: {secondNewValue}, equilibrium: {equilibrium}"
            );
            Debug.Assert(
                SignedVectorGreaterThan(secondNewValue, newValue),
                $"Elastic system should have contracted further towards endpoint, new value: {secondNewValue}, last value: {newValue}"
            );
            Debug.Assert
                (SignedVectorGreaterThan(ves.GetCurrentVelocity(), Vector3.zero),
                $"Elastic system should still have positive velocity, actual velocity: {ves.GetCurrentVelocity()}"
            );

            yield return null;
        }

        /// <summary>
        /// Tests that snap forces at the snap points behave correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator VolumeSnapPointSnapping()
        {
            // Make a copy of the default volume elastic extent.
            var snappingExtent = volumeExtent;

            // Set some decent snap points
            snappingExtent.SnapPoints = new Vector3[]
            {
                new Vector3(-0.5f,-0.5f,-0.5f),
                new Vector3(0.4f, 0.4f, 0.4f)
            };

            snappingExtent.SnapRadius = 0.5f;

            // Construct our system.
            VolumeElasticSystem les = new VolumeElasticSystem(Vector3.zero, Vector3.zero, snappingExtent, elasticProperties);

            // Goal position for the elastic system to seek.
            // We will let the system settle to right next to one of the snapping point.
            var goalValue = new Vector3(0.3f, 0.3f, 0.3f);

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simulates independently from Unity's frame system.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be near the goal value, but slightly pulled towards the snapping point.
            var equilibrium = les.GetCurrentValue();

            Debug.Assert(
                SignedVectorGreaterThan(equilibrium, goalValue),
                $"Equilibrium should be slightly greater than goal value. Goal: {goalValue}, Current: {equilibrium}"
            );
            Debug.Assert(
                SignedVectorLessThan(equilibrium, snappingExtent.SnapPoints[1]),
                $"Equilibrium should still be less than the snapping value"
            );

            // Move the goal value to next to the other snapping point (-0.5,-0.5,-0.5)
            goalValue = new Vector3(-0.4f, -0.4f, -0.4f);

            // Let the elastic system come to an equilibrium.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be near the goal value, but slightly pulled towards the snapping point.
            equilibrium = les.GetCurrentValue();

            Debug.Assert(
                SignedVectorLessThan(equilibrium, goalValue),
                $"Equilibrium should be slightly less than goal value. Goal: {goalValue}, Current: {equilibrium}"
            );
            Debug.Assert(
                SignedVectorGreaterThan(equilibrium, snappingExtent.SnapPoints[0]),
                $"Equilibrium should still be greater than the snapping value"
            );

            yield return null;
        }

        /// <summary>
        /// Tests that the repeating snap intervals behave correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator VolumeIntervalSnapping()
        {
            // Make a copy of the default volume elastic extent.
            var snappingExtent = volumeExtent;

            // Set some decent snap points
            snappingExtent.SnapPoints = new Vector3[]
            {
                new Vector3(0.2f, 0.2f, 0.2f)
            };

            // Expand the bounds a bit
            snappingExtent.StretchBounds = new Bounds(Vector3.zero, 2.0f * Vector3.one);

            // Set a good snap radius.
            snappingExtent.SnapRadius = 0.1f;

            // Enable interval snapping.
            snappingExtent.RepeatSnapPoints = true;

            // Construct our system.
            VolumeElasticSystem les = new VolumeElasticSystem(Vector3.zero, Vector3.zero, snappingExtent, elasticProperties);

            // Loop over a range of negative and positive integer multiples of the snap interval.
            for (int j = -3; j < 6; j++)
            {
                // Goal position for the elastic system to seek.
                // We will let the system settle to right next to an integer multiple of the snap point.
                var goalValue = (snappingExtent.SnapPoints[0] * j) - (new Vector3(0.03f, 0.03f, 0.03f));

                // Let the elastic system come to an equilibrium.
                // No need for yielding for new frames because the elastic system
                // simulates independently from Unity's frame system.
                for (int i = 0; i < 1000; i++)
                {
                    les.ComputeIteration(goalValue, 0.05f);
                }

                // Get the equilibrium value from the system.
                // It should be near the goal value, but slightly pulled towards the snapping point.
                var equilibrium = les.GetCurrentValue();
                Debug.Assert(
                    SignedVectorGreaterThan(equilibrium, goalValue),
                    $"Equilibrium should be slightly greater than goal value. Goal: {goalValue.ToString("F4")}, Current: {equilibrium.ToString("F4")}"
                );
                Debug.Assert(
                    SignedVectorLessThan(equilibrium, snappingExtent.SnapPoints[0] * j),
                    $"Equilibrium should still be less than the snapping value. Equilibrium"
                );
            }

            yield return null;
        }

        /// <summary>
        /// Tests that snap forces at the snap points behave correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator QuaternionSnapPointSnapping()
        {
            // Make a copy of the default volume elastic extent.
            var snappingExtent = quatExtent;

            // Set some decent snap points
            snappingExtent.SnapPoints = new Vector3[]
            {
                new Vector3(45.0f, 45.0f, 45.0f),
                new Vector3(-45.0f, -45.0f, -45.0f)
            };

            // Construct our system.
            QuaternionElasticSystem les = new QuaternionElasticSystem(Quaternion.identity, Quaternion.identity, snappingExtent, elasticProperties);

            // Goal position for the elastic system to seek.
            // We will let the system settle to right next to one of the snapping point.
            var goalValue = Quaternion.Euler(new Vector3(40, 40, 40));

            // Let the elastic system come to an equilibrium.
            // No need for yielding for new frames because the elastic system
            // simulates independently from Unity's frame system.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be near the goal value, but slightly pulled towards the snapping point.
            var equilibrium = les.GetCurrentValue();

            Debug.Assert(
                SignedVectorGreaterThan(equilibrium.eulerAngles, goalValue.eulerAngles),
                $"Equilibrium should be slightly greater than goal value. Goal: {goalValue.eulerAngles:F4}, Current: {equilibrium.eulerAngles:F4}"
            );
            Debug.Assert(
                SignedVectorLessThan(equilibrium.eulerAngles, snappingExtent.SnapPoints[0]),
                $"Equilibrium should still be less than the snapping value"
            );

            // Move the goal value to next to the other snapping point (-45,-45,-45)
            goalValue = Quaternion.Euler(new Vector3(-35, -35, -35));

            // Let the elastic system come to an equilibrium.
            for (int i = 0; i < 1000; i++)
            {
                les.ComputeIteration(goalValue, 0.05f);
            }

            // Get the equilibrium value from the system.
            // It should be near the goal value, but slightly pulled towards the snapping point.
            equilibrium = les.GetCurrentValue();

            Debug.Assert(
                SignedVectorLessThan(equilibrium.eulerAngles, goalValue.eulerAngles),
                $"Equilibrium should be slightly less than goal value. Goal: {goalValue.eulerAngles:F4}, Current: {equilibrium.eulerAngles:F4}"
            );
            Debug.Assert(
                SignedVectorGreaterThan(equilibrium.eulerAngles, snappingExtent.SnapPoints[0]),
                $"Equilibrium should still be greater than the snapping value"
            );

            yield return null;
        }

        /// <summary>
        /// Tests that the repeating snap intervals behave correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator QuaternionIntervalSnapping()
        {
            // Make a copy of the default quaternion elastic extent.
            var snappingExtent = quatExtent;

            // Set some decent snap points
            snappingExtent.SnapPoints = new Vector3[]
            {
                new Vector3(10,10,10)
            };
            // Set a good snap radius.
            snappingExtent.SnapRadius = 5f;

            // Enable interval snapping.
            snappingExtent.RepeatSnapPoints = true;

            // Construct our system.
            QuaternionElasticSystem les = new QuaternionElasticSystem(Quaternion.identity, Quaternion.identity, snappingExtent, elasticProperties);

            // Loop over a range of negative and positive integer multiples of the snap interval.
            for (int j = -3; j < 6; j++)
            {
                // Goal position for the elastic system to seek.
                // We will let the system settle to right next to an integer multiple of the snap point.
                var goalValue = Quaternion.Euler((snappingExtent.SnapPoints[0] * j) - (new Vector3(0.03f, 0.03f, 0.03f)));

                // Let the elastic system come to an equilibrium.
                // No need for yielding for new frames because the elastic system
                // simulates independently from Unity's frame system.
                for (int i = 0; i < 1000; i++)
                {
                    les.ComputeIteration(goalValue, 0.05f);
                }

                // Get the equilibrium value from the system.
                // It should be near the goal value, but slightly pulled towards the snapping point.
                var equilibrium = les.GetCurrentValue();
                Debug.Assert(
                    SignedVectorGreaterThan(equilibrium.eulerAngles, goalValue.eulerAngles),
                    $"Equilibrium should be slightly greater than goal value. Goal: {goalValue:F4}, Current: {equilibrium.eulerAngles:F4}"
                );
                Debug.Assert(
                    SignedVectorLessThan(equilibrium.eulerAngles, Vector3.one * 360.0f + snappingExtent.SnapPoints[0] * j),
                    $"Equilibrium should still be less than the snapping value."
                );
            }

            yield return null;
        }


        /// <summary>
        /// Returns true if every component of a is greater than every component of b.
        /// </summary>
        private bool SignedVectorGreaterThan(Vector3 a, Vector3 b)
        {
            return a.x > b.x && a.y > b.y && a.z > b.z;
        }

        /// <summary>
        /// Returns true if every component of a is less than every component of b.
        /// </summary>
        private bool SignedVectorLessThan(Vector3 a, Vector3 b)
        {
            return !SignedVectorGreaterThan(a, b) && (a != b);
        }
    }
}
#endif