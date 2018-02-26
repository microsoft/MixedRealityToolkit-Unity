// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class DistorterSimplex : Distorter
    {
        public float ScaleMultiplier = 10f;
        public float SpeedMultiplier = 1f;
        public float StrengthMultiplier = 0.5f;
        public Vector3 AxisStrength = Vector3.one;
        public Vector3 AxisSpeed = Vector3.one;
        public Vector3 AxisOffset = Vector3.zero;
        public float ScaleDistort = 2f;
        public bool UniformScaleDistort = true;

        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            Vector3 scaledPoint = (point * ScaleMultiplier) + AxisOffset;

            point.x = (float)(point.x + (noise.Evaluate(scaledPoint.x, scaledPoint.y, scaledPoint.z, Time.unscaledTime * AxisSpeed.x)) * AxisStrength.x * StrengthMultiplier);
            point.y = (float)(point.y + (noise.Evaluate(scaledPoint.x, scaledPoint.y, scaledPoint.z, Time.unscaledTime * AxisSpeed.y)) * AxisStrength.y * StrengthMultiplier);
            point.z = (float)(point.z + (noise.Evaluate(scaledPoint.x, scaledPoint.y, scaledPoint.z, Time.unscaledTime * AxisSpeed.z)) * AxisStrength.z * StrengthMultiplier);
            return point;
        }

        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            if (UniformScaleDistort)
            {
                float scale = (float)(noise.Evaluate(point.x, point.y, point.z, Time.unscaledTime));
                return Vector3.one  + (Vector3.one * (scale * ScaleDistort));
            }
            else
            {
                point = DistortPointInternal(point, strength);
                return Vector3.Lerp (Vector3.one, Vector3.Scale(Vector3.one, Vector3.one + (point * ScaleDistort)), strength);
            }
        }

        private FastSimplexNoise noise = new FastSimplexNoise();
    }
}