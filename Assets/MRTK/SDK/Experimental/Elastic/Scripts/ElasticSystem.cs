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
    /// <summary>
    /// Represents a damped harmonic oscillator over an
    /// N-dimensional vector space, specified by generic type T.
    /// 
    /// This extensibility allows not just for 1, 2, and 3-D springs, but
    /// allows for 4-dimensional quaternion springs.
    /// </summary>
    internal abstract class ElasticSystem<T>
    {
        protected ElasticExtentProperties<T> extentInfo;
        protected ElasticProperties elasticProperties;

        protected T currentValue;
        protected T currentVelocity;

        public ElasticSystem(T initialValue, T initialVelocity,
                             ElasticExtentProperties<T> extentInfo,
                             ElasticProperties elasticProperties)
        {
            this.extentInfo = extentInfo;
            this.elasticProperties = elasticProperties;
            currentValue = initialValue;
            currentVelocity = initialVelocity;
        }

        /// <summary>
        /// Update the internal state of the damped harmonic oscillator,
        /// given the forcing/desired value, returning the new value.
        /// </summary>
        /// <param name="forcingValue">Forcing function, for example, a desired manipulation position.
        /// See https://en.wikipedia.org/wiki/Forcing_function_(differential_equations). It is a non-time-dependent
        /// input function to a differential equation; in our situation, it is the "input position" to the spring.</param>
        /// <param name="deltaTime">Amount of time that has passed since the last update.</param>
        /// <returns>The new value of the system.</returns>
        public abstract T ComputeIteration(T forcingValue, float deltaTime);

        /// <summary>
        /// Query the elastic system for the current instantaneous value
        /// </summary>
        /// <returns>Current value of the elastic system</returns>
        public T GetCurrentValue() => currentValue;

        /// <summary>
        /// Query the elastic system for the current instantaneous velocity
        /// </summary>
        /// <returns>Current value of the elastic system</returns>
        public T GetCurrentVelocity() => currentVelocity;
    }

    internal class LinearElasticSystem : ElasticSystem<float>
    {
        public LinearElasticSystem(float initialValue, float initialVelocity,
                                   ElasticExtentProperties<float> extentInfo,
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
        public override float ComputeIteration(float forcingValue, float deltaTime)
        {
            // F = -kx - (drag * v)
            var force = (forcingValue - currentValue) * elasticProperties.HandK - elasticProperties.Drag * currentVelocity;

            // Distance that the current stretch value is from the end limit.
            float distFromEnd = extentInfo.MaxStretch - currentValue;

            // Add force to the "left" if we are beyond the max stretch.
            // (or, if enabled, add snapping force if we are near the endpoint)
            force -= computeEndForce(currentValue - extentInfo.MaxStretch);

            // Add force to the "right" if we are beyond the max stretch.
            // (or, if enabled, add snapping force if we are near the endpoint)
            force += computeEndForce(extentInfo.MinStretch - currentValue);

            // Iterate over each snapping point, and apply forces as necessary.
            foreach (float snappingPoint in extentInfo.SnapPoints)
            {
                // Calculate distance from snapping point.
                var distFromSnappingPoint = snappingPoint - currentValue;
                force += computeSnapForce(distFromSnappingPoint, elasticProperties.SnapK, elasticProperties.SnapRadius);
            }

            // a = F/m
            var accel = force / elasticProperties.Mass;

            // Integrate our acceleration over time.
            currentVelocity += accel * deltaTime;
            // Integrate our velocity over time.
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }

        // Helper function to reduce force calculation copypasta.
        private float computeEndForce(float current)
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
                return extentInfo.SnapToEnds ? computeSnapForce(current, elasticProperties.EndK, elasticProperties.SnapRadius) : 0.0f;
            }
        }

        // Helper function to reduce force calculation copypasta.
        private float computeSnapForce(float distFromPoint, float k, float radius)
        {
            // Snap force is calculated by multiplying the "-kx" factor by
            // a clamped distance factor. This results in an overall
            // hyperbolic profile to the force imparted by the snap point.
            return (distFromPoint) * elasticProperties.SnapK * (1.0f - Mathf.Clamp01(Mathf.Abs(distFromPoint / radius)));
        }
    }
}
