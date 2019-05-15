// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// A Distorter that randomly distorts points.
    /// </summary>
    public class DistorterSimplex : Distorter
    {
        private readonly FastSimplexNoise noise = new FastSimplexNoise();

        [SerializeField]
        private float scaleMultiplier = 2f;

        public float ScaleMultiplier
        {
            get { return scaleMultiplier; }
            set { scaleMultiplier = value; }
        }

        [SerializeField]
        private float strengthMultiplier = 0.25f;

        public float StrengthMultiplier
        {
            get { return strengthMultiplier; }
            set { strengthMultiplier = value; }
        }

        [SerializeField]
        private Vector3 axisStrength = Vector3.one;

        public Vector3 AxisStrength
        {
            get { return axisStrength; }
            set { axisStrength = value; }
        }

        [SerializeField]
        private Vector3 axisSpeed = Vector3.one;

        public Vector3 AxisSpeed
        {
            get { return axisSpeed; }
            set { axisSpeed = value; }
        }

        [SerializeField]
        private Vector3 axisOffset = Vector3.zero;

        public Vector3 AxisOffset
        {
            get { return axisOffset; }
            set { axisOffset = value; }
        }

        [SerializeField]
        private float scaleDistort = 2f;

        public float ScaleDistort
        {
            get { return scaleDistort; }
            set { scaleDistort = value; }
        }

        [SerializeField]
        private bool uniformScaleDistort = true;

        public bool UniformScaleDistort
        {
            get { return uniformScaleDistort; }
            set { uniformScaleDistort = value; }
        }

        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            Vector3 scaledPoint = (point * scaleMultiplier) + axisOffset;
            point.x += (float)((noise.Evaluate(scaledPoint.x, Time.unscaledTime * axisSpeed.x)) * axisStrength.x * strengthMultiplier * strength);
            point.y += (float)((noise.Evaluate(scaledPoint.x + scaledPoint.y, Time.unscaledTime * axisSpeed.y)) * axisStrength.y * strengthMultiplier * strength);
            point.z += (float)((noise.Evaluate(scaledPoint.x + scaledPoint.y + scaledPoint.z, Time.unscaledTime * axisSpeed.z)) * axisStrength.z * strengthMultiplier * strength);
            return point;
        }

        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            if (uniformScaleDistort)
            {
                var scale = (float)(noise.Evaluate(point.x, point.y, point.z, Time.unscaledTime));
                return Vector3.one + (Vector3.one * (scale * scaleDistort));
            }

            point = DistortPointInternal(point, strength);
            return Vector3.Lerp(Vector3.one, Vector3.Scale(Vector3.one, Vector3.one + (point * scaleDistort)), strength);
        }
    }
}