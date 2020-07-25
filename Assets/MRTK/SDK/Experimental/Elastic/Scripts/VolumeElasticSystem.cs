// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    public class VolumeElasticSystem : IElasticSystem<Vector3>
    {
        // Internal system state.
        private Vector3 currentValue;
        private Vector3 currentVelocity;

        // Configuration of extent and spring properties.
        private VolumeElasticExtent extent;
        private ElasticProperties elasticProperties;

        /// <summary>
        /// Default constructor; initializes the elastic system with the specified
        /// initial value, velocity, extent, and elastic properties.
        /// </summary>
        public VolumeElasticSystem(Vector3 initialValue, Vector3 initialVelocity,
                                   VolumeElasticExtent extentInfo,
                                   ElasticProperties elasticProperties)
        {
            currentValue = initialValue;
            currentVelocity = initialVelocity;
            this.extent = extentInfo;
            this.elasticProperties = elasticProperties;
        }

        /// <inheritdoc/>
        public Vector3 ComputeIteration(Vector3 forcingValue, float deltaTime)
        {
            // F = -kx - (drag * v)
            Vector3 force = (forcingValue - currentValue) * elasticProperties.HandK - elasticProperties.Drag * currentVelocity;

            // Iterate over each snapping point, and apply forces as necessary.
            foreach (Vector3 snapPoint in extent.SnapPoints)
            {
                Vector3 closestPoint = extent.RepeatSnapPoints ? FindNearest(currentValue, snapPoint) : snapPoint;
                // Calculate distance from snapping point.
                Vector3 distFromSnappingPoint = closestPoint - currentValue;
                force += ComputeSnapForce(distFromSnappingPoint, elasticProperties.SnapK, extent.SnapRadius);
            }

            // Check if our current value is within the specified bounds.
            // If outside, we will apply the end-force (if the bounds are enabled)
            if (!extent.StretchBounds.Contains(currentValue) && extent.UseBounds)
            {
                Vector3 closestPoint = extent.StretchBounds.ClosestPoint(currentValue);
                Vector3 displacementFromEdge = closestPoint - currentValue;

                // Apply the force (F = kx)
                force += displacementFromEdge * elasticProperties.EndK;
            }

            // a = F/m
            Vector3 accel = force / elasticProperties.Mass;

            // Integrate our acceleration over time.
            currentVelocity += accel * deltaTime;
            // Integrate our velocity over time.
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }

        public Vector3 GetCurrentValue() => currentValue;
        public Vector3 GetCurrentVelocity() => currentVelocity;

        // Find the nearest snapping point to the given value, on a repeated
        // snapping interval.
        private Vector3 FindNearest(Vector3 value, Vector3 interval)
        {
            Debug.Assert(interval != Vector3.zero, "Zero vector used for repeating snapping interval; divide by zero!");
            return new Vector3(Mathf.Round(value.x / interval.x) * interval.x,
                               Mathf.Round(value.y / interval.y) * interval.y,
                               Mathf.Round(value.z / interval.z) * interval.z);
        }

        // Helper function to reduce force calculation copypasta.
        private Vector3 ComputeSnapForce(Vector3 distFromPoint, float k, float radius)
        {
            // Snap force is calculated by multiplying the "-kx" factor by
            // a clamped distance factor. This results in an overall
            // hyperbolic profile to the force imparted by the snap point.
            return (distFromPoint) * elasticProperties.SnapK * (1.0f - Mathf.Clamp01(Mathf.Abs(distFromPoint.magnitude / radius)));
        }
    }
}
