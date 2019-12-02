// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    [Serializable]
    public struct RayStep
    {
        private static Vector3 dist;
        private static Vector3 dir;
        private static Vector3 pos;

        public RayStep(Vector3 origin, Vector3 terminus) : this()
        {
            Origin = origin;
            Terminus = terminus;

            dist.x = Terminus.x - Origin.x;
            dist.y = Terminus.y - Origin.y;
            dist.z = Terminus.z - Origin.z;
            Length = Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z));

            if (Length > 0)
            {
                dir.x = dist.x / Length;
                dir.y = dist.y / Length;
                dir.z = dist.z / Length;
            }
            else
            {
                dir = dist;
            }

            Direction = dir;

            epsilon = 0.01f;
        }

        public Vector3 Origin { get; private set; }
        public Vector3 Terminus { get; private set; }
        public Vector3 Direction { get; private set; }
        public float Length { get; private set; }

        private readonly float epsilon;

        public Vector3 GetPoint(float distance)
        {
            if (Length <= distance || Length == 0f)
                return Origin;

            pos.x = Origin.x + Direction.x * distance;
            pos.y = Origin.y + Direction.y * distance;
            pos.z = Origin.z + Direction.z * distance;

            return pos;
        }

        /// <summary>
        /// Update current raystep with new origin and terminus points. 
        /// Pass by ref to avoid unnecessary struct copy into function since values will be copied anyways locally
        /// </summary>
        /// <param name="origin">beginning of raystep origin</param>
        /// <param name="terminus">end of raystep</param>
        public void UpdateRayStep(ref Vector3 origin, ref Vector3 terminus)
        {
            Origin = origin;
            Terminus = terminus;

            dist.x = Terminus.x - Origin.x;
            dist.y = Terminus.y - Origin.y;
            dist.z = Terminus.z - Origin.z;
            Length = Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z));

            if (Length > 0)
            {
                dir.x = dist.x / Length;
                dir.y = dist.y / Length;
                dir.z = dist.z / Length;
            }
            else
            {
                dir = dist;
            }

            Direction = dir;
        }

        public void CopyRay(Ray ray, float rayLength)
        {
            Length = rayLength;
            Origin = ray.origin;
            Direction = ray.direction;

            pos.x = Origin.x + Direction.x * Length;
            pos.y = Origin.y + Direction.y * Length;
            pos.z = Origin.z + Direction.z * Length;

            Terminus = pos;
        }

        public bool Contains(Vector3 point)
        {
            dist.x = Origin.x - point.x;
            dist.y = Origin.y - point.y;
            dist.z = Origin.z - point.z;
            float sqrMagOriginPoint = (dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z);

            dist.x = point.x - Terminus.x;
            dist.y = point.y - Terminus.y;
            dist.z = point.z - Terminus.z;
            float sqrMagPointTerminus = (dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z);

            float sqrLength = Length * Length;
            float sqrEpsilon = epsilon * epsilon;

            return (sqrMagOriginPoint + sqrMagPointTerminus) - sqrLength > sqrEpsilon;
        }

        public static implicit operator Ray(RayStep r)
        {
            return new Ray(r.Origin, r.Direction);
        }

        #region static utility functions

        /// <summary>
        /// Returns a point along an array of RaySteps by distance
        /// </summary>
        public static Vector3 GetPointByDistance(RayStep[] steps, float distance)
        {
            Debug.Assert(steps != null);
            Debug.Assert(steps.Length > 0);

            float remainingDistance = 0;
            RayStep rayStep = GetStepByDistance(steps, distance, ref remainingDistance);
            if (remainingDistance > 0)
            {
                return Vector3.Lerp(rayStep.Origin, rayStep.Terminus, remainingDistance / rayStep.Length);
            }
            else
            {
                return rayStep.Terminus;
            }
        }

        /// <summary>
        /// Returns a RayStep along an array of RaySteps by distance
        /// </summary>
        public static RayStep GetStepByDistance(RayStep[] steps, float distance, ref float remainingDistance)
        {
            Debug.Assert(steps != null && steps.Length > 0);

            float traveledDistance = 0;
            float stepLength = 0;
            RayStep currentStep = new RayStep();


            foreach (var step in steps)
            {
                currentStep = step;
                stepLength = step.Length;

                if (distance > traveledDistance + stepLength)
                {
                    traveledDistance += stepLength;
                }
                else
                {
                    remainingDistance = Mathf.Clamp(distance - traveledDistance, 0f, stepLength);
                    return currentStep;
                }
            }

            remainingDistance = 0;
            return currentStep;
        }

        /// <summary>
        /// Returns a direction along an array of RaySteps by distance
        /// </summary>
        public static Vector3 GetDirectionByDistance(RayStep[] steps, float distance)
        {
            Debug.Assert(steps != null);
            Debug.Assert(steps.Length > 0);

            float traveledDistance = 0;
            return GetStepByDistance(steps, distance, ref traveledDistance).Direction;
        }

        #endregion
    }
}