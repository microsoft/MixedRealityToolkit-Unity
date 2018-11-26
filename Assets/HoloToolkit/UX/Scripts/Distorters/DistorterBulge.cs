// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class DistorterBulge : Distorter
    {
        public Vector3 BulgeCenter
        {
            get
            {
                return transform.TransformPoint(bulgeCenter);
            }
            set
            {
                bulgeCenter = transform.InverseTransformPoint(value);
            }
        }

        [SerializeField]
        private Vector3 bulgeCenter = Vector3.zero;
        [SerializeField]
        private AnimationCurve bulgeFalloff = new AnimationCurve();
        [SerializeField]
        private float bulgeRadius = 1f;
        [SerializeField]
        private float scaleDistort = 2f;
        [SerializeField]
        private float bulgeStrength = 1f;

        protected override Vector3 DistortPointInternal (Vector3 point, float strength)
        {
            float distanceToCenter = Vector3.Distance(point, BulgeCenter);
            if (distanceToCenter < bulgeRadius)
            {
                float distortion = (1f - (bulgeFalloff.Evaluate(distanceToCenter / bulgeRadius))) * bulgeStrength;
                Vector3 direction = (point - BulgeCenter).normalized;
                point = point + (direction * distortion * bulgeStrength);
            }
            return point;
        }

        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            float distanceToCenter = Vector3.Distance(point, BulgeCenter);
            if (distanceToCenter < bulgeRadius)
            {
                float distortion = (1f - (bulgeFalloff.Evaluate(distanceToCenter / bulgeRadius))) * bulgeStrength;
                return Vector3.one + (Vector3.one * distortion * scaleDistort);
            }
            return Vector3.one;
        }

        private void OnDrawGizmos()
        {
            Vector3 bulgePoint = transform.TransformPoint(bulgeCenter);
            Color gColor = Color.red;
            gColor.a = 0.5f;
            Gizmos.color = gColor;
            Gizmos.DrawWireSphere(bulgePoint, bulgeRadius);
            int steps = 8;
            for (int i = 0; i < steps; i++)
            {
                float normalizedStep = (1f / steps) * i;
                gColor.a = (1f - bulgeFalloff.Evaluate(normalizedStep)) * 0.5f;
                Gizmos.color = gColor;
                Gizmos.DrawSphere(bulgePoint, bulgeRadius * bulgeFalloff.Evaluate(normalizedStep));
            }
        }
    }
}
