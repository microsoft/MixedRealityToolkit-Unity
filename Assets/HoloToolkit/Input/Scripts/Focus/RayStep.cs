using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    [Serializable]
    public struct RayStep
    {
        public RayStep(Vector3 origin, Vector3 terminus)
        {
            this.origin = origin;
            this.terminus = terminus;
            length = Vector3.Distance(origin, terminus);
            direction = (this.terminus - this.origin).normalized;
        }

        public Vector3 origin { get; private set; }
        public Vector3 terminus { get; private set; }
        public Vector3 direction { get; private set; }
        public float length { get; private set; }

        public Vector3 GetPoint(float distance)
        {
            return Vector3.MoveTowards(origin, terminus, distance);
        }

        public void UpdateRayStep(Vector3 origin, Vector3 terminus)
        {
            this.origin = origin;
            this.terminus = terminus;
            length = Vector3.Distance(origin, terminus);
            direction = (this.terminus - this.origin).normalized;
        }

        public void CopyRay (Ray ray, float rayLength)
        {
            length = rayLength;
            origin = ray.origin;
            direction = ray.direction;
            terminus = origin + (direction * length);
        }

        public static implicit operator Ray(RayStep r)
        {
            return new Ray(r.origin, r.direction);
        }
    }
}