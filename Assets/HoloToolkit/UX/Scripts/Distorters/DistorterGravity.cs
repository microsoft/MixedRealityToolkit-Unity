// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class DistorterGravity : Distorter
    {
        [SerializeField]
        private Vector3 localCenterOfGravity;

        [SerializeField]
        private Vector3 axisStrength = Vector3.one;

        [Range(0f, 10f)]
        [SerializeField]
        private float radius = 0.5f;

        [SerializeField]
        private AnimationCurve gravityStrength = AnimationCurve.EaseInOut(0, 0, 1, 1);

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