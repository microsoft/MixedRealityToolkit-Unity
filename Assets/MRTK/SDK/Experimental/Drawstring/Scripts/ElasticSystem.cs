// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{

    /// <summary>
    /// Properties of the extent in which a damped
    /// harmonic oscillator is free to move.
    /// </summary>
    [Serializable]
    public struct ElasticExtentProperties<T>
    {
        /// <value>
        /// Represents the lower bound of the extent,
        /// specified as the norm of the n-dimensional extent
        /// </value>
        [SerializeField]
        public float minStretch;

        /// <value>
        /// Represents the upper bound of the extent,
        /// specified as the norm of the n-dimensional extent
        /// </value>
        [SerializeField]
        public float maxStretch;

        /// <value>
        /// Whether the system, when approaching the upper bound,
        /// will treat the upper bound as a snap point.
        /// </value>
        [SerializeField]
        public bool snapToMax;

        /// <value>
        /// Points inside the extent to which the system will snap.
        /// </value>
        [SerializeField]
        public T[] snapPoints;
    }

    /// <summary>
    /// Properties of the damped harmonic oscillator differential system.
    /// </summary>
    [Serializable]
    public struct ElasticProperties
    {
        /// <value>
        /// Mass of the simulated oscillator element
        /// </value>
        [SerializeField]
        public float mass;
        /// <value>
        /// Hand spring constant
        /// </value>
        [SerializeField]
        public float hand_k;
        /// <value>
        /// End cap spring constant
        /// </value>
        [SerializeField]
        public float end_k;
        /// <value>
        /// Snap point spring constant
        /// </value>
        [SerializeField]
        public float snap_k;
        /// <value>
        /// Extent at which snap points begin forcing the spring.
        /// </value>
        [SerializeField]
        public float snap_radius;
        /// <value>
        /// Drag/damper factor, proportional to velocity.
        /// </value>
        [SerializeField]
        public float drag;
    }

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
                                ElasticExtentProperties<T> extentInfo, ElasticProperties elasticProperties)
        {
            this.extentInfo = extentInfo;
            this.elasticProperties = elasticProperties;
            currentValue = initialValue;
            currentVelocity = initialVelocity;
        }

        /// <summary>
        /// Update the internal state of the damped harmonic oscillator, given the forcing/desired value.
        /// </summary>
        /// <param name="forcingValue">Input value, for example, a desired manipulation position.</param>
        /// <param name="deltaTime">Amount of time that has passed since the last update.</param>
        public abstract T ComputeIteration(T forcingValue, float deltaTime);

        /// <summary>
        /// Query the elastic system for the current instantaneous value
        /// </summary>
        /// <returns>Current value of the elastic system</returns>
        public T GetCurrentValue() => currentValue;
    }

    internal class LinearElasticSystem : ElasticSystem<float>
    {
        public LinearElasticSystem(float initialValue, float initialVelocity,
                                ElasticExtentProperties<float> extentInfo, ElasticProperties elasticProperties)
        : base(initialValue, initialVelocity, extentInfo, elasticProperties) { }


        /// <summary>
        /// Update the internal state of the damped harmonic oscillator, given the forcing/desired value.
        /// </summary>
        /// <param name="forcingValue">Input value, for example, a desired manipulation position.</param>
        /// <param name="deltaTime">Amount of time that has passed since the last update.</param>
        public override float ComputeIteration(float forcingValue, float deltaTime)
        {
            // F = -kx - (drag * v)
            var force = (forcingValue - currentValue) * elasticProperties.hand_k - elasticProperties.drag * currentVelocity;

            // Distance that the current stretch value is from the end limit.
            float distFromEnd = extentInfo.maxStretch - currentValue;

            // If we are extended beyond the end cap,
            // add one-sided force back to the center.
            if (currentValue > extentInfo.maxStretch)
            {
                force += distFromEnd * elasticProperties.end_k;
            }
            else
            {
                // Otherwise, add standard bidirectional magnetic/snapping force towards the end marker. (optional)
                if (extentInfo.snapToMax)
                {
                    force += (distFromEnd) * elasticProperties.end_k * (1.0f - Mathf.Clamp01(Mathf.Abs(distFromEnd / elasticProperties.snap_radius)));
                }
            }

            distFromEnd = extentInfo.minStretch - currentValue;
            if (currentValue < extentInfo.minStretch)
            {
                force += distFromEnd * elasticProperties.end_k;
            }
            else
            {
                // Otherwise, add standard bidirectional magnetic/snapping force towards the end marker. (optional)
                if (extentInfo.snapToMax)
                {
                    force += (distFromEnd) * elasticProperties.end_k * (1.0f - Mathf.Clamp01(Mathf.Abs(distFromEnd / elasticProperties.snap_radius)));
                }
            }

            // Iterate over each snapping point, and apply forces as necessary.
            foreach (float snappingPoint in extentInfo.snapPoints)
            {
                // Calculate distance from snapping point.
                var distFromSnappingPoint = snappingPoint - currentValue;

                // Snap force is calculated by multiplying the "-kx" factor by
                // a clamped distance factor. This results in an overall
                // hyperbolic profile to the force imparted by the snap point.
                force += (distFromSnappingPoint) * elasticProperties.snap_k
                          * (1.0f - Mathf.Clamp01(Mathf.Abs(distFromSnappingPoint / elasticProperties.snap_radius)));
            }

            // a = F/m
            var accel = force / elasticProperties.mass;

            // Integrate our acceleration over time.
            currentVelocity += accel * deltaTime;
            // Integrate our velocity over time.
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }
    }
}
