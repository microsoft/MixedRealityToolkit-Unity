// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Represents a raycast that is a portion of a longer raycast.
    /// </summary>
    [Serializable]
    public struct RayStep
    {
        // Re-use static space to avoid additional allocation
        private static Vector3 dist;
        private static Vector3 dir;
        private static Vector3 pos;


        /// <summary>
        /// Initializes a new instance of the <see cref="RayStep"/> struct.
        /// </summary>
        /// <param name="origin">The origin position of the raycast step.</param>
        /// <param name="terminus">The end position of the raycast step.</param>
        public RayStep(Vector3 origin, Vector3 terminus) : this()
        {
            UpdateRayStep(in origin, in terminus);

            epsilon = 0.01f;
        }

        /// <summary>
        /// Get the origin position of the raycast step.
        /// </summary>
        public Vector3 Origin { get; private set; }

        /// <summary>
        /// Get the end position of the raycast step.
        /// </summary>
        public Vector3 Terminus { get; private set; }
        
        /// <summary>
        /// Get the direction of the raycast step. This direction will be a normalized vector from the origin to the terminus.
        /// </summary>
        public Vector3 Direction { get; private set; }

        /// <summary>
        /// The length or magnitude of the raycast step. This is the distance from the origin to the terminus.
        /// </summary>
        public float Length { get; private set; }

        private readonly float epsilon;

        /// <summary>
        /// Get a point along the raycast, at a specified distance from the origin.
        /// </summary>
        /// <param name="distance">The returned point will be at this distance from the origin.</param>
        /// <returns>
        /// A new point that is at the specified distance from the origin.
        /// </returns>
        public Vector3 GetPoint(float distance)
        {
            if (Length <= distance || Length == 0f)
            {
                return Origin;
            }

            pos.x = Origin.x + Direction.x * distance;
            pos.y = Origin.y + Direction.y * distance;
            pos.z = Origin.z + Direction.z * distance;

            return pos;
        }

        /// <summary>
        /// Update the ray step with new origin and terminus points. 
        /// </summary>
        /// <remarks>
        /// Vectors are passed by reference to avoid unnecessary struct copies. The input values will be copied locally.
        /// </remarks>
        /// <param name="origin">The origin position of the ray step.</param>
        /// <param name="terminus">The end position of the ray step.</param>
        public void UpdateRayStep(in Vector3 origin, in Vector3 terminus)
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

        /// <summary>
        /// Copy the given ray structure to this raycast step, along with the specified length.
        /// </summary>
        /// <param name="length">
        /// The new length for this raycast step. The length or magnitude of the raycast step. 
        /// This is the distance from the ray's origin to the terminus.
        /// </param>
        public void CopyRay(Ray ray, float length)
        {
            Length = length;
            Origin = ray.origin;
            Direction = ray.direction;

            pos.x = Origin.x + Direction.x * Length;
            pos.y = Origin.y + Direction.y * Length;
            pos.z = Origin.z + Direction.z * Length;

            Terminus = pos;
        }

        /// <summary>
        /// Test if the raycast step contain the specified point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>
        /// True if the point is contained along the raycast step. Otherwise false is returned.
        /// </returns>
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

        /// <summary>
        /// Create a copy of the given raycast step.
        /// </summary>
        /// <param name="rayStep">The raycast step to copy.</param>
        public static implicit operator Ray(RayStep rayStep)
        {
            return new Ray(rayStep.Origin, rayStep.Direction);
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
            RayStep currentStep = new RayStep();

            foreach (var step in steps)
            {
                currentStep = step;
                float stepLength = step.Length;

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
