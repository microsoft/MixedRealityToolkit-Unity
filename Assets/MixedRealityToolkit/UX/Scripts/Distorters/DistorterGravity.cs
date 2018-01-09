// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class DistorterGravity : Distorter
    {
        public Vector3 WorldCenterOfGravity
        {
            get
            {
                return transform.TransformPoint(LocalCenterOfGravity);
            }
            set
            {
                LocalCenterOfGravity = transform.InverseTransformPoint(value);
            }
        }

        public Vector3 LocalCenterOfGravity;
        public Vector3 AxisStrength = Vector3.one;
        [Range(0f, 10f)]
        public float Radius = 0.5f;
        public AnimationCurve GravityStrength = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            Vector3 target = WorldCenterOfGravity;

            float normalizedDistance = 1f - Mathf.Clamp01 (Vector3.Distance(point, target) / Radius);

            strength *= GravityStrength.Evaluate (normalizedDistance);

            point.x = Mathf.Lerp(point.x, target.x, Mathf.Clamp01(strength * AxisStrength.x));
            point.y = Mathf.Lerp(point.y, target.y, Mathf.Clamp01(strength * AxisStrength.y));
            point.z = Mathf.Lerp(point.z, target.z, Mathf.Clamp01(strength * AxisStrength.z));

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
            Gizmos.DrawWireSphere(WorldCenterOfGravity, Radius);
        }
    }
}