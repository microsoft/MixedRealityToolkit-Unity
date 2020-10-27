// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    public class QuaternionElasticSystem : IElasticSystem<Quaternion>
    {
        private Quaternion currentValue;
        private Quaternion currentVelocity;

        private QuaternionElasticExtent extent;
        private ElasticProperties elasticProperties;

        /// <summary>
        /// Default constructor; initializes the elastic system with the specified
        /// initial value, velocity, extent, and elastic properties.
        /// </summary>
        public QuaternionElasticSystem(Quaternion initialValue,
                                       Quaternion initialVelocity,
                                       QuaternionElasticExtent extentInfo,
                                       ElasticProperties elasticProperties)
        {
            currentValue = initialValue;
            currentVelocity = initialVelocity;
            this.extent = extentInfo;
            this.elasticProperties = elasticProperties;
        }

        /// <inheritdoc/>
        public Quaternion ComputeIteration(Quaternion forcingValue, float deltaTime)
        {
            // If the dot product is negative, we need to negate the forcing
            // quaternion so that the force is coherent/wraps correctly.
            if (Quaternion.Dot(forcingValue, currentValue) < 0)
            {
                forcingValue = new Quaternion(-forcingValue.x, -forcingValue.y, -forcingValue.z, -forcingValue.w);
            }

            // Displacement of the forcing value compared to the current state's value.
            Quaternion displacement = Quaternion.Inverse(currentValue) * forcingValue;

            // The force applied is an approximation of F=(kx - vd) in 4-space
            Quaternion force = Add(Scale(displacement, elasticProperties.HandK), Scale(currentVelocity, -elasticProperties.Drag));

            // Euler representation of the current elastic quaternion state.
            Vector3 eulers = currentValue.eulerAngles;
            
            foreach (var snapPoint in extent.SnapPoints)
            {
                // If we are set to repeat the snap points (i.e., tiling them across the sphere),
                // we use the nearest integer multiple of the snap point. If it is not repeated,
                // we use the snap point directly.
                Vector3 nearest = extent.RepeatSnapPoints ? GetNearest(eulers, snapPoint) : snapPoint;

                // Convert the nearest point to a quaternion representation.
                Quaternion nearestQuat = Quaternion.Euler(nearest.x, nearest.y, nearest.z);

                // If the dot product is negative, we need to negate the snap
                // quaternion so that the snap force is coherent/wraps correctly.
                if (Quaternion.Dot(nearestQuat, currentValue) < 0)
                {
                    nearestQuat = Scale(nearestQuat, -1.0f);
                }

                // Displacement from the current value to the snap quaternion.
                Quaternion snapDisplacement = Quaternion.Inverse(currentValue) * nearestQuat;

                // Angle of the snapping displacement, used to calculate the snapping magnitude.
                float snapAngle = Quaternion.Angle(currentValue, nearestQuat);

                // Strength of the snapping force, function of the snap angle and the configured
                // snapping radius.
                float snapFactor = ComputeSnapFactor(snapAngle, extent.SnapRadius);

                // Accumulate the force from every snap point.
                force = Add(force, Scale(snapDisplacement, elasticProperties.SnapK * snapFactor));
            }

            // Performing Euler integration in 4-space.

            // Acceleration = F/m
            Quaternion accel = Scale(force, (1 / elasticProperties.Mass));

            // v' = v + a * deltaT
            currentVelocity = Add(currentVelocity, Scale(accel, deltaTime));

            // x' = x + v' * deltaT
            currentValue *= Scale(currentVelocity, deltaTime).normalized;

            // As the current value is a quaternion, we must renormalize the quaternion
            // before using it as a representation of a rotation.
            currentValue = currentValue.normalized;

            return currentValue;
        }

        /// <inheritdoc/>
        public Quaternion GetCurrentValue() => currentValue;

        /// <inheritdoc/>
        public Quaternion GetCurrentVelocity() => currentVelocity;

        // Find the nearest integer multiple of the specified interval
        private Vector3 GetNearest(Vector3 target, Vector3 interval)
        {
            return new Vector3(GetNearest(target.x, interval.x), GetNearest(target.y, interval.y), GetNearest(target.z, interval.z));
        }

        // Find the nearest integer multiple of the specified interval
        private float GetNearest(float target, float interval)
        {
            return Mathf.Round(target / interval) * interval;
        }

        private float ComputeSnapFactor(float angleFromPoint, float radius)
        {
            // Snap force is calculated by multiplying the "-kx" factor by
            // a clamped distance factor. This results in an overall
            // hyperbolic profile to the force imparted by the snap point.
            return (1.0f - Mathf.Clamp01(Mathf.Abs(angleFromPoint / radius)));
        }

        // Elementwise quaternion addition.
        private Quaternion Add(Quaternion p, Quaternion q)
        {
            return new Quaternion(p.x + q.x, p.y + q.y, p.z + q.z, p.w + q.w);
        }

        // Elementwise quaternion scale.
        private Quaternion Scale(Quaternion p, float t)
        {
            return new Quaternion(p.x * t, p.y * t, p.z * t, p.w * t);
        }
    }
}
