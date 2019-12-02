// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// A Distorter that distorts points based on their distance and direction from the center of the
    /// bulge point.
    /// </summary>
    /// <remarks>
    /// The characteristics of the distortion are also heavily controlled by the BulgeFalloff
    /// property, which should contain key frames that cover the [0, 1] time range.
    /// </remarks>
    [AddComponentMenu("Scripts/MRTK/Core/DistorterBulge")]
    public class DistorterBulge : Distorter
    {
        [SerializeField]
        private Vector3 bulgeLocalCenter = Vector3.zero;

        public Vector3 BulgeLocalCenter
        {
            get { return bulgeLocalCenter; }
            set { bulgeLocalCenter = value; }
        }

        public Vector3 BulgeWorldCenter
        {
            get
            {
                return transform.TransformPoint(bulgeLocalCenter);
            }
            set
            {
                bulgeLocalCenter = transform.InverseTransformPoint(value);
            }
        }

        [SerializeField]
        private AnimationCurve bulgeFalloff = new AnimationCurve();

        public AnimationCurve BulgeFalloff
        {
            get { return bulgeFalloff; }
            set { bulgeFalloff = value; }
        }

        [SerializeField]
        private float bulgeRadius = 1f;

        public float BulgeRadius
        {
            get { return bulgeRadius; }
            set { bulgeRadius = value < 0f ? 0f : value; }
        }

        [SerializeField]
        private float scaleDistort = 2f;

        public float ScaleDistort
        {
            get { return scaleDistort; }
            set { scaleDistort = value; }
        }

        [SerializeField]
        private float bulgeStrength = 1f;

        public float BulgeStrength
        {
            get { return bulgeStrength; }
            set { bulgeStrength = value; }
        }

        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            float distanceToCenter = Vector3.Distance(point, BulgeWorldCenter);

            if (distanceToCenter < bulgeRadius)
            {
                float distortion = (1f - (bulgeFalloff.Evaluate(distanceToCenter / bulgeRadius))) * bulgeStrength;
                Vector3 direction = (point - BulgeWorldCenter).normalized;
                point = point + (direction * distortion * bulgeStrength);
            }

            return point;
        }

        /// <inheritdoc />
        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            float distanceToCenter = Vector3.Distance(point, BulgeWorldCenter);

            if (distanceToCenter < bulgeRadius)
            {
                float distortion = (1f - (bulgeFalloff.Evaluate(distanceToCenter / bulgeRadius))) * bulgeStrength;
                return Vector3.one + (Vector3.one * distortion * scaleDistort);
            }

            return Vector3.one;
        }

        private void OnDrawGizmos()
        {
            Vector3 bulgePoint = transform.TransformPoint(bulgeLocalCenter);
            Color gColor = Color.red;
            gColor.a = 0.5f;
            Gizmos.color = gColor;
            Gizmos.DrawWireSphere(bulgePoint, bulgeRadius);
            const int steps = 8;

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
