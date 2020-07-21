// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    internal class Interval3DElasticSystem : ElasticSystem<Vector3>
    {
        public Interval3DElasticSystem(Vector3 initialValue, Vector3 initialVelocity,
                                   ElasticExtentProperties<Vector3> extentInfo,
                                   ElasticProperties elasticProperties)
                                   : base(initialValue, initialVelocity,
                                          extentInfo, elasticProperties) { }

        /// <summary>
        /// Update the internal state of the damped harmonic oscillator, given the forcing/desired value.
        /// </summary>
        /// <param name="forcingValue">Forcing function, for example, a desired manipulation position.
        /// See https://en.wikipedia.org/wiki/Forcing_function_(differential_equations). It is a non-time-dependent
        /// input function to a differential equation; in our situation, it is the "input position" to the spring.</param>
        /// <param name="deltaTime">Amount of time that has passed since the last update.</param>
        public override Vector3 ComputeIteration(Vector3 forcingValue, float deltaTime)
        {
            // F = -kx - (drag * v)
            var force = (forcingValue - currentValue) * elasticProperties.HandK - elasticProperties.Drag * currentVelocity;



            // Iterate over each snapping point, and apply forces as necessary.
            foreach (Vector3 interval in extentInfo.SnapPoints)
            {
                Vector3 closestPoint = FindNearest(currentValue, interval);
                // Calculate distance from snapping point.
                var distFromSnappingPoint = closestPoint - currentValue;
                force += computeSnapForce(distFromSnappingPoint, elasticProperties.SnapK, extentInfo.SnapRadius);
            }

            // a = F/m
            var accel = force / elasticProperties.Mass;

            // Integrate our acceleration over time.
            currentVelocity += accel * deltaTime;
            // Integrate our velocity over time.
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }

        private Vector3 FindNearest(Vector3 value, Vector3 interval)
        {
            return new Vector3(Mathf.Round(value.x / interval.x) * interval.x,
                               Mathf.Round(value.y / interval.y) * interval.y,
                               Mathf.Round(value.z / interval.z) * interval.z);
        }

        // Helper function to reduce force calculation copypasta.
        private Vector3 computeSnapForce(Vector3 distFromPoint, float k, float radius)
        {
            // Snap force is calculated by multiplying the "-kx" factor by
            // a clamped distance factor. This results in an overall
            // hyperbolic profile to the force imparted by the snap point.
            return (distFromPoint) * elasticProperties.SnapK * (1.0f - Mathf.Clamp01(Mathf.Abs(distFromPoint.magnitude / radius)));
        }
    }
}
