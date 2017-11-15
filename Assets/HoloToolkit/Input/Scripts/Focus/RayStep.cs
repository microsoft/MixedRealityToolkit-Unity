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

        #region static utility functions

        /// <summary>
        /// Returns a point along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 GetPointByDistance(RayStep[] steps, float distance)
        {
            Debug.Assert(steps != null);
            Debug.Assert(steps.Length > 0);

            Vector3 point = Vector3.zero;
            float remainingDistance = distance;

            for (int i = 0; i < steps.Length; i++)
            {
                if (remainingDistance > steps[i].length)
                {
                    remainingDistance -= steps[i].length;
                }
                else
                {
                    point = Vector3.Lerp(steps[i].origin, steps[i].terminus, remainingDistance / steps[i].length);
                    remainingDistance = 0;
                    break;
                }
            }

            if (remainingDistance > 0)
            {
                // If we reach the end and still have distance left, set the point to the terminus of the last step
                point = steps[steps.Length - 1].terminus;
            }

            return point;
        }

        /// <summary>
        /// Returns a RayStep along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static RayStep GetStepByDistance(RayStep[] steps, float distance)
        {
            Debug.Assert(steps != null);
            Debug.Assert(steps.Length > 0);

            RayStep step = new RayStep();
            float remainingDistance = distance;

            for (int i = 0; i < steps.Length; i++)
            {
                if (remainingDistance > steps[i].length)
                {
                    remainingDistance -= steps[i].length;
                }
                else
                {
                    step = steps[i];
                    remainingDistance = 0;
                    break;
                }
            }
            
            if (remainingDistance > 0)
            {
                // If we reach the end and still have distance left, return the last step
                step = steps[steps.Length - 1];
            }

            return step;
        }

        /// <summary>
        /// Returns a direction along an array of RaySteps by distance
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 GetDirectionByDistance(RayStep[] steps, float distance)
        {
            Debug.Assert(steps != null);
            Debug.Assert(steps.Length > 0);

            return GetStepByDistance(steps, distance).direction;
        }

        #endregion
    }
}