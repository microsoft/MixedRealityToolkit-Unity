// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    public static class ConeCastUtility
    {
        public struct ConeCastHit
        {
            public ConeCastHit(RaycastHit hit, float distance, float angle)
            {
                raycastHit = hit;
                hitDistance = distance;
                hitAngle = angle;
            }

            public RaycastHit raycastHit;
            public float hitDistance;
            public float hitAngle;
        }

        static List<ConeCastHit> coneCastHitList = new List<ConeCastHit>();

        /// <summary>
        /// Function casts a sphere along a ray and checks if the hitpoint is within the angle of the cone and returns detailed information.
        /// </summary>
        /// <param name="origin">The vertex of the cone and the at the start of the sweep.</param>
        /// <param name="direction">The direction into which to sweep the sphere..</param>
        /// <param name="maxRadius">The radius of the sweep.</param>
        /// <param name="maxDistance">The max length of the cast.</param>
        /// <param name="coneAngle">The angle used to define the cone.</param>
        /// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
        /// <returns>An array of structs that contain RaycastHit, distance to hit, and the angle of all the objects that were hit.</returns>
        public static ConeCastHit[] ConeCastAll(Vector3 origin, Vector3 direction, float maxRadius, float maxDistance, float coneAngle, LayerMask layerMask)
        {
            coneCastHitList.Clear();

            RaycastHit[] sphereCastHits = UnityEngine.Physics.SphereCastAll(origin - new Vector3(0, 0, maxRadius), maxRadius, direction, maxDistance, layerMask);

            if (sphereCastHits.Length == 0)
            {
                return Array.Empty<ConeCastHit>();
            }

            for (int i = 0; i < sphereCastHits.Length; i++)
            {
                Vector3 hitPoint = sphereCastHits[i].point;
                Vector3 directionToHit = hitPoint - origin;
                float angleToHit = Vector3.Angle(direction, directionToHit);

                if (angleToHit < coneAngle)
                {
                    coneCastHitList.Add(new ConeCastHit(sphereCastHits[i], directionToHit.magnitude, angleToHit));
                }
            }

            return coneCastHitList.ToArray();
        }

        private static RaycastHit[] sphereCastHits = null;
        private static int sphereCastMaxHitCount = 10;
        private const int sphereCastLimit = 100;

        /// <summary>
        /// Function casts a sphere along a ray and checks if the hitpoint is within the angle of the cone and returns the best target determined by the weights provided.
        /// </summary>
        /// <param name="origin">The vertex of the cone and the at the start of the sweep.</param>
        /// <param name="direction">The direction into which to sweep the sphere..</param>
        /// <param name="maxRadius">The radius of the sweep.</param>
        /// <param name="maxDistance">The max length of the cast.</param>
        /// <param name="coneAngle">The angle used to define the cone.</param>
        /// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
        /// <param name="distanceWeight">The importance of distance between the hitpoint and the origin in selecting the best target.</param>
        /// <param name="angleWeight">The importance of angle between the hitpoint and the origin in selecting the best target.</param>
        /// <param name="distanceToCenterWeight">The importance of distance between the hitpoint and the center of the object in selecting the best target.</param>
        /// <param name="angleToCenterWeight">The importance of angle between the hitpoint and the center of the object in selecting the best target.</param>
        /// <returns>The RaycastHit of the best object.</returns>
        public static RaycastHit ConeCastBest(Vector3 origin, Vector3 direction, float maxRadius, float maxDistance, float coneAngle, LayerMask layerMask, float distanceWeight, float angleWeight, float distanceToCenterWeight, float angleToCenterWeight)
        {
            if (sphereCastHits == null || sphereCastHits.Length < sphereCastMaxHitCount)
            {
                sphereCastHits = new RaycastHit[sphereCastMaxHitCount];
            }

            var hitCount = UnityEngine.Physics.SphereCastNonAlloc(origin - (direction * maxRadius), maxRadius, direction, sphereCastHits, maxDistance, layerMask, QueryTriggerInteraction.Ignore);

            // Algorithm: double the max hit count if there are too many results, up to a certain limit
            if (hitCount >= sphereCastMaxHitCount && sphereCastMaxHitCount < sphereCastLimit)
            {
                // There might be more hits we didn't get, grow the array and try again next time
                // Note that this frame, the results might be imprecise.
                sphereCastMaxHitCount = Math.Min(sphereCastLimit, sphereCastMaxHitCount * 2);
            }

            RaycastHit hitGameobject = new RaycastHit();
            float score = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = sphereCastHits[i];

                Vector3 hitPoint = hit.point;
                Vector3 directionToHit = hitPoint - origin;
                float angleToHit = Vector3.Angle(direction, directionToHit);
                Vector3 hitDistance = hit.collider.transform.position - hitPoint;
                Vector3 directionToCenter = hit.collider.transform.position - origin;
                float angleToCenter = Vector3.Angle(direction, directionToCenter);

                // Additional work to see if there is a better point slightly further ahead on the direction line. This is only allowed if the collider isn't a mesh collider.
                if (hit.collider.GetType() != typeof(MeshCollider))
                {
                    Vector3 pointFurtherAlongGazePath = (maxRadius * 0.5f * direction.normalized) + FindNearestPointOnLine(origin, direction, hitPoint);
                    Vector3 closestPointToPointFurtherAlongGazePath = hit.collider.ClosestPoint(pointFurtherAlongGazePath);
                    Vector3 directionToSecondaryPoint = closestPointToPointFurtherAlongGazePath - origin;
                    float angleToSecondaryPoint = Vector3.Angle(direction, directionToSecondaryPoint);

                    if (angleToSecondaryPoint < angleToHit)
                    {
                        hitPoint = closestPointToPointFurtherAlongGazePath;
                        directionToHit = directionToSecondaryPoint;
                        angleToHit = angleToSecondaryPoint;
                        hitDistance = hit.collider.transform.position - hitPoint;
                    }
                }

                if (angleToHit < coneAngle)
                {
                    float distanceScore = distanceWeight == 0 ? 0.0f : (distanceWeight * directionToHit.magnitude);
                    float angleScore = angleWeight == 0 ? 0.0f : (angleWeight * angleToHit);
                    float centerScore = distanceToCenterWeight == 0 ? 0.0f : (distanceToCenterWeight * hitDistance.magnitude);
                    float centerAngleScore = angleToCenterWeight == 0 ? 0.0f : (angleToCenterWeight * angleToCenter);
                    float newScore = distanceScore + angleScore + centerScore + centerAngleScore;

                    if (newScore < score)
                    {
                        score = newScore;
                        hitGameobject = hit;
                    }
                }
            }

            return hitGameobject;
        }

        private static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
        {
            direction.Normalize();
            Vector3 lhs = point - origin;

            float dotP = Vector3.Dot(lhs, direction);
            return origin + direction * dotP;
        }
    }
}