// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.Utilities
{
    public static class RaycastHelper
    {
        public static bool DebugEnabled = false;

        public delegate bool RaycastFunc(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, out RaycastResultHelper result);

        public static bool First(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, out RaycastResultHelper result)
        {
            result = default(RaycastResultHelper);

            RaycastHit hit;
            bool hitSomething = false;

            // Check to see if the ray cast hits any of the requested Unity layers
            if (layerMask != 0 &&
                UnityEngine.Physics.Raycast(origin, direction, out hit, distance, layerMask))
            {
                result = new RaycastResultHelper(hit, layerMask);

                hitSomething = true;
            }

            return hitSomething;
        }

        public static bool SphereFirst(Vector3 origin, Vector3 direction, float radius, float distance, LayerMask layerMask, out RaycastResultHelper result)
        {
            result = default(RaycastResultHelper);

            RaycastHit hit;
            bool hitSomething = false;

            // Check to see if the ray cast hits any of the requested Unity layers
            if (layerMask != 0 &&
                UnityEngine.Physics.SphereCast(origin, radius, direction, out hit, distance, layerMask))
            {
                result = new RaycastResultHelper(hit, layerMask);

                hitSomething = true;
            }

            return hitSomething;
        }

        public static List<RaycastResultHelper> All(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask)
        {
            return All(origin, direction, distance, layerMask, null);
        }

        public static List<RaycastResultHelper> All(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, List<Collider> movedColliders)
        {
            // Check to see if the ray cast hits any of the requested Unity layers
            List<RaycastResultHelper> results = null;
            if (layerMask != 0)
            {
                var raycastHits = UnityEngine.Physics.RaycastAll(origin, direction, distance, layerMask);
                if (raycastHits != null)
                {
                    results = new List<RaycastResultHelper>(raycastHits.Length);
                    foreach (var raycastHit in raycastHits)
                    {
                        results.Add(new RaycastResultHelper(raycastHit, layerMask));
                    }
                }
            }

            // If we have colliders that have moved, then remove them from the results list and redo the raycast.
            if (movedColliders != null)
            {
                RaycastHit hitInfo;
                Ray ray = new Ray(origin, direction);
                foreach (var collider in movedColliders)
                {
                    if ((collider.gameObject.layer & layerMask) != 0)
                    {
                        int colliderIndex = results.FindIndex(x => x.Collider == collider);

                        if (collider.Raycast(ray, out hitInfo, distance))
                        {
                            if (colliderIndex >= 0)
                            {
                                results[colliderIndex] = new RaycastResultHelper(hitInfo, layerMask);
                            }
                            else
                            {
                                results.Add(new RaycastResultHelper(hitInfo, layerMask));
                            }
                        }
                        else if (colliderIndex >= 0)
                        {
                            results.RemoveAt(colliderIndex);
                        }
                    }
                }
            }

            // Unity doesn't return hit results in any particular order, so we need to sort them to closest first.
            if (results != null)
            {
                results.Sort((x, y) => x.Distance < y.Distance ? -1 : 1);
            }

            return results;
        }

        public static Vector3 GetBoxColliderExtents(BoxCollider boxCollider)
        {
            return boxCollider.size;
        }

        // raysPerEdge should be odd
        public static bool CastBoxExtents(Vector3 extents, Vector3 targetPosition, Matrix4x4 trs, Ray ray, float maxDistance, LayerMask surface, RaycastFunc raycastFunc, int raysPerEdge, bool ortho, out Vector3[] points, out Vector3[] normals, out bool[] hits)
        {
            bool debugEnabled = DebugEnabled;
            if (debugEnabled)
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10.0f, Color.green);
            }

            extents /= (raysPerEdge - 1);

            int halfRaysPerEdge = (raysPerEdge - 1) / 2;
            int numRays = raysPerEdge * raysPerEdge;

            bool hitSomething = false;

            points = new Vector3[numRays];
            normals = new Vector3[numRays];
            hits = new bool[numRays];

            int index = 0;

            for (int x = -halfRaysPerEdge; x <= halfRaysPerEdge; x += 1)
            {
                for (int y = -halfRaysPerEdge; y <= halfRaysPerEdge; y += 1)
                {
                    Vector3 offset = trs.MultiplyVector(new Vector3(x * extents.x, y * extents.y, 0));

                    Vector3 origin = ray.origin;
                    Vector3 direction = (targetPosition + offset) - ray.origin;

                    if (ortho)
                    {
                        origin += offset;
                        direction = ray.direction;
                    }

                    RaycastResultHelper rayHit;
                    hits[index] = raycastFunc(origin, direction, maxDistance, surface, out rayHit);

                    if (hits[index])
                    {
                        hitSomething = true;
                        points[index] = rayHit.Point;
                        normals[index] = rayHit.Normal;

                        if (debugEnabled)
                        {
                            Debug.DrawLine(origin, points[index], Color.yellow);
                        }
                    }
                    else
                    {
                        if (debugEnabled)
                        {
                            Debug.DrawLine(origin, origin + direction * 3.0f, Color.gray);
                        }
                    }

                    index++;
                }
            }

            return hitSomething;
        }
    }
}
