// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Physics.Distorters
{
    public class DistorterGravity : Distorter
    {
        [SerializeField]
        private Vector3 localCenterOfGravity;

        public Vector3 LocalCenterOfGravity
        {
            get { return localCenterOfGravity; }
            set { localCenterOfGravity = value; }
        }

        public Vector3 WorldCenterOfGravity
        {
            get
            {
                return transform.TransformPoint(localCenterOfGravity);
            }
            set
            {
                localCenterOfGravity = transform.InverseTransformPoint(value);
            }
        }

        [SerializeField]
        private Vector3 axisStrength = Vector3.one;

        public Vector3 AxisStrength
        {
            get { return axisStrength; }
            set { axisStrength = value; }
        }

        [Range(0f, 10f)]
        [SerializeField]
        private float radius = 0.5f;

        public float Radius
        {
            get { return radius; }
            set
            {
                if (value < 0f)
                {
                    radius = 0f;
                }
                else if (value > 10f)
                {
                    radius = 10f;
                }
                else
                {
                    radius = value;
                }
            }
        }

        [SerializeField]
        private AnimationCurve gravityStrength = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public AnimationCurve GravityStrength
        {
            get { return gravityStrength; }
            set { gravityStrength = value; }
        }

        /// <inheritdoc />
        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            Vector3 target = WorldCenterOfGravity;

            float normalizedDistance = 1f - Mathf.Clamp01(Vector3.Distance(point, target) / radius);

            strength *= gravityStrength.Evaluate(normalizedDistance);

            point.x = Mathf.Lerp(point.x, target.x, Mathf.Clamp01(strength * axisStrength.x));
            point.y = Mathf.Lerp(point.y, target.y, Mathf.Clamp01(strength * axisStrength.y));
            point.z = Mathf.Lerp(point.z, target.z, Mathf.Clamp01(strength * axisStrength.z));

            return point;
        }

        /// <inheritdoc />
        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            return point;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(WorldCenterOfGravity, 0.05f);
            Gizmos.DrawWireSphere(WorldCenterOfGravity, radius);
        }
    }
}