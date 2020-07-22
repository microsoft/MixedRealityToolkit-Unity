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
    public class LinearElasticSystem : ElasticSystem<float>
    {
        private float currentValue;
        private float currentVelocity;

        private LinearElasticExtent extent;
        private ElasticProperties elasticProperties;

        public LinearElasticSystem(float initialValue, float initialVelocity,
                                   LinearElasticExtent extentInfo,
                                   ElasticProperties elasticProperties)
        {
            currentValue = initialValue;
            currentVelocity = initialVelocity;
            this.extent = extentInfo;
            this.elasticProperties = elasticProperties;
        }

        /// <inheritdoc/>
        public override float ComputeIteration(float forcingValue, float deltaTime)
        {
            // F = -kx - (drag * v)
            var force = (forcingValue - currentValue) * elasticProperties.HandK - elasticProperties.Drag * currentVelocity;

            // Distance that the current stretch value is from the end limit.
            float distFromEnd = extent.MaxStretch - currentValue;

            // Add force to the "left" if we are beyond the max stretch.
            // (or, if enabled, add snapping force if we are near the endpoint)
            force -= ComputeEndForce(currentValue - extent.MaxStretch);

            // Add force to the "right" if we are beyond the max stretch.
            // (or, if enabled, add snapping force if we are near the endpoint)
            force += ComputeEndForce(extent.MinStretch - currentValue);

            // Iterate over each snapping point, and apply forces as necessary.
            foreach (float snappingPoint in extent.SnapPoints)
            {
                // Calculate distance from snapping point.
                var distFromSnappingPoint = snappingPoint - currentValue;
                force += ComputeSnapForce(distFromSnappingPoint, elasticProperties.SnapK, extent.SnapRadius);
            }

            // a = F/m
            var accel = force / elasticProperties.Mass;

            // Integrate our acceleration over time.
            currentVelocity += accel * deltaTime;
            // Integrate our velocity over time.
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }

        public override float GetCurrentValue() => currentValue;
        public override float GetCurrentVelocity() => currentVelocity;

        // Helper function to reduce force calculation copypasta.
        private float ComputeEndForce(float current)
        {
            // If we are extended beyond the end cap,
            // add one-sided force back to the center.
            if (current > 0)
            {
                return current * elasticProperties.EndK;
            }
            else
            {
                // Otherwise, add standard bidirectional magnetic/snapping force towards the end marker. (optional)
                return extent.SnapToEnds ? ComputeSnapForce(current, elasticProperties.EndK, extent.SnapRadius) : 0.0f;
            }
        }

        // Helper function to reduce force calculation copypasta.
        private float ComputeSnapForce(float distFromPoint, float k, float radius)
        {
            // Snap force is calculated by multiplying the "-kx" factor by
            // a clamped distance factor. This results in an overall
            // hyperbolic profile to the force imparted by the snap point.
            return (distFromPoint) * elasticProperties.SnapK * (1.0f - Mathf.Clamp01(Mathf.Abs(distFromPoint / radius)));
        }
    }
}
