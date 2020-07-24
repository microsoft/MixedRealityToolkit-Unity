// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    public class QuaternionElasticSystem : ElasticSystem<Quaternion>
    {
        private Quaternion currentValue;
        private Quaternion currentVelocity;

        private QuaternionElasticExtent extent;
        private ElasticProperties elasticProperties;

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
        public override Quaternion ComputeIteration(Quaternion forcingValue, float deltaTime)
        {
            if (Quaternion.Dot(forcingValue, currentValue) < 0)
            {
                forcingValue = new Quaternion(-forcingValue.x, -forcingValue.y, -forcingValue.z, -forcingValue.w);
            }

            // For clarity and conciseness
            var k = elasticProperties.HandK;
            var d = elasticProperties.Drag;
            var m = elasticProperties.Mass;

            var displacement = Quaternion.Inverse(currentValue) * forcingValue;

            var force = Add(Scale(displacement, elasticProperties.HandK), Scale(currentVelocity, -d));

            var eulers = currentValue.eulerAngles;

            
            foreach (var interval in extent.SnapPoints )
            {
                var nearest = extent.RepeatSnapPoints ? GetNearest(eulers, interval) : interval;
                var nearestQuat = Quaternion.Euler(nearest.x, nearest.y, nearest.z);

                if (Quaternion.Dot(nearestQuat, currentValue) < 0)
                {
                    nearestQuat = Scale(nearestQuat, -1.0f);
                }
                var snapDisplacement = Quaternion.Inverse(currentValue) * nearestQuat;
                var snapAngle = Quaternion.Angle(currentValue, nearestQuat);

                var snapFactor = ComputeSnapFactor(snapAngle, extent.SnapRadius);

                force = Add(force, Scale(snapDisplacement, elasticProperties.SnapK * snapFactor));
            }

            var accel = Scale(force, (1 / m));
            currentVelocity = Add(currentVelocity, Scale(accel, deltaTime));
            currentValue = currentValue * Scale(currentVelocity, deltaTime).normalized;
            currentValue = currentValue.normalized;

            return currentValue;
        }

        /// <inheritdoc/>
        public override Quaternion GetCurrentValue() => currentValue;

        /// <inheritdoc/>
        public override Quaternion GetCurrentVelocity() => currentVelocity;

        private Vector3 GetNearest(Vector3 target, Vector3 interval)
        {
            return new Vector3(GetNearest(target.x, interval.x), GetNearest(target.y, interval.y), GetNearest(target.z, interval.z));
        }

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

        private Quaternion Add(Quaternion p, Quaternion q)
        {
            return new Quaternion(p.x + q.x, p.y + q.y, p.z + q.z, p.w + q.w);
        }

        private Quaternion Scale(Quaternion p, float t)
        {
            return new Quaternion(p.x * t, p.y * t, p.z * t, p.w * t);
        }
    }
    internal static class QuaternionExtras
    {
        internal static Quaternion Lerp(Quaternion p, Quaternion q, float t, bool shortWay)
        {
            if (shortWay)
            {
                float dot = Quaternion.Dot(p, q);
                if (dot < 0.0f)
                    return Lerp(ScalarMultiply(p, -1.0f), q, t, true);
            }

            Quaternion r = Quaternion.identity;
            r.x = p.x * (1f - t) + q.x * (t);
            r.y = p.y * (1f - t) + q.y * (t);
            r.z = p.z * (1f - t) + q.z * (t);
            r.w = p.w * (1f - t) + q.w * (t);
            return r;
        }

        internal static Quaternion Slerp(Quaternion p, Quaternion q, float t, bool shortWay)
        {
            float dot = Quaternion.Dot(p, q);
            if (shortWay)
            {
                if (dot < 0.0f)
                    return Slerp(ScalarMultiply(p, -1.0f), q, t, true);
            }

            float angle = Mathf.Acos(dot);
            Quaternion first = ScalarMultiply(p, Mathf.Sin((1f - t) * angle));
            Quaternion second = ScalarMultiply(q, Mathf.Sin((t) * angle));
            float division = 1f / Mathf.Sin(angle);
            return ScalarMultiply(Add(first, second), division);
        }


        internal static Quaternion ScalarMultiply(Quaternion input, float scalar)
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }

        internal static Quaternion Add(Quaternion p, Quaternion q)
        {
            return new Quaternion(p.x + q.x, p.y + q.y, p.z + q.z, p.w + q.w);
        }
    }
}
