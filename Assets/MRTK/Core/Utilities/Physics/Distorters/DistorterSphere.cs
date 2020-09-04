// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// A Distorter that distorts points based on their distance and direction from the
    /// center of the sphere of size 2.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/DistorterSphere")]
    public class DistorterSphere : Distorter
    {
        public Vector3 SphereCenter
        {
            get
            {
                return transform.TransformPoint(sphereCenter);
            }
            set
            {
                sphereCenter = transform.InverseTransformPoint(value);
            }
        }

        [SerializeField]
        private Vector3 sphereCenter;

        [SerializeField]
        private float radius = 2f;

        /// <inheritdoc />
        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            Vector3 direction = (point - SphereCenter).normalized;
            return Vector3.Lerp(point, SphereCenter + (direction * radius), strength);
        }

        /// <inheritdoc />
        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            return Vector3.one;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(SphereCenter, radius);
        }
    }
}