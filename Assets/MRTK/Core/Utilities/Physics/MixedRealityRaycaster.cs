// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    public static class MixedRealityRaycaster
    {
        public static bool DebugEnabled = false;

        public const int MaxRaycastHitCount = 32;
        public const int MaxSphereCastHitCount = 32;
        private static readonly RaycastHit[] RaycastHits = new RaycastHit[MaxRaycastHitCount];
        private static readonly RaycastHit[] SphereCastHits = new RaycastHit[MaxSphereCastHitCount];

        /// <summary>
        /// Simple raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        public static bool RaycastSimplePhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out RaycastHit physicsHit)
        {
            return RaycastSimplePhysicsStep(step, step.Length, prioritizedLayerMasks, focusIndividualCompoundCollider, out physicsHit);
        }

        private static readonly ProfilerMarker RaycastSimplePhysicsStepPerfMarker = new ProfilerMarker("[MRTK] MixedRealityRaycaster.RaycastSimplePhysicsStep");

        /// <summary>
        /// Simple raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/> within a specified maximum distance.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        public static bool RaycastSimplePhysicsStep(RayStep step, float maxDistance, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out RaycastHit physicsHit)
        {
            using (RaycastSimplePhysicsStepPerfMarker.Auto())
            {
                Debug.Assert(maxDistance > 0, "Length must be longer than zero!");
                Debug.Assert(step.Direction != Vector3.zero, "Invalid step direction!");

                bool result = false;
                if (prioritizedLayerMasks.Length == 1)
                {
                    // If there is only one priority, don't prioritize
                    result = UnityEngine.Physics.Raycast(step.Origin, step.Direction, out physicsHit, maxDistance, prioritizedLayerMasks[0]);
                }
                else
                {
                    // Raycast across all layers and prioritize
                    int hitCount = UnityEngine.Physics.RaycastNonAlloc(step.Origin, step.Direction, RaycastHits, maxDistance, UnityEngine.Physics.AllLayers);
                    result = TryGetPrioritizedPhysicsHit(RaycastHits, hitCount, prioritizedLayerMasks, focusIndividualCompoundCollider, out physicsHit);
                }

                return result;
            }
        }

        private static readonly ProfilerMarker RaycastBoxPhysicsStepPerfMarker = new ProfilerMarker("[MRTK] MixedRealityRaycaster.RaycastBoxPhysicsStep");

        /// <summary>
        /// Box raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        public static bool RaycastBoxPhysicsStep(RayStep step, Vector3 extents, Vector3 targetPosition, Matrix4x4 matrix, float maxDistance, LayerMask[] prioritizedLayerMasks, int raysPerEdge, bool isOrthographic, bool focusIndividualCompoundCollider, out Vector3[] points, out Vector3[] normals, out bool[] hits)
        {
            using (RaycastBoxPhysicsStepPerfMarker.Auto())
            {
                if (Application.isEditor && DebugEnabled)
                {
                    Debug.DrawLine(step.Origin, step.Origin + step.Direction * 10.0f, Color.green);
                }

                extents /= (raysPerEdge - 1);

                int halfRaysPerEdge = (int)((raysPerEdge - 1) * 0.5f);
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
                        Vector3 offset = matrix.MultiplyVector(new Vector3(x * extents.x, y * extents.y, 0));

                        Vector3 origin = step.Origin;
                        Vector3 direction = (targetPosition + offset) - step.Origin;

                        if (isOrthographic)
                        {
                            origin += offset;
                            direction = step.Direction;
                        }

                        RaycastHit rayHit;
                        hits[index] = RaycastSimplePhysicsStep(new RayStep(origin, direction.normalized * maxDistance), prioritizedLayerMasks, focusIndividualCompoundCollider, out rayHit);

                        if (hits[index])
                        {
                            hitSomething = true;
                            points[index] = rayHit.point;
                            normals[index] = rayHit.normal;

                            if (Application.isEditor && DebugEnabled)
                            {
                                Debug.DrawLine(origin, points[index], Color.yellow);
                            }
                        }
                        else
                        {
                            if (Application.isEditor && DebugEnabled)
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

        /// <summary>
        /// Sphere raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        public static bool RaycastSpherePhysicsStep(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out RaycastHit physicsHit)
        {
            return RaycastSpherePhysicsStep(step, radius, step.Length, prioritizedLayerMasks, focusIndividualCompoundCollider, out physicsHit);
        }

        private static readonly ProfilerMarker RaycastSpherePhysicsStepPerfMarker = new ProfilerMarker("[MRTK] MixedRealityRaycaster.RaycastSpherePhysicsStep");

        /// <summary>
        /// Sphere raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/> within a specified maximum distance.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        public static bool RaycastSpherePhysicsStep(RayStep step, float radius, float maxDistance, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out RaycastHit physicsHit)
        {
            using (RaycastSpherePhysicsStepPerfMarker.Auto())
            {
                bool result = false;
                if (prioritizedLayerMasks.Length == 1)
                {
                    // If there is only one priority, don't prioritize
                    result = UnityEngine.Physics.SphereCast(step.Origin, radius, step.Direction, out physicsHit, maxDistance, prioritizedLayerMasks[0]);
                }
                else
                {
                    // Raycast across all layers and prioritize
                    int hitCount = UnityEngine.Physics.SphereCastNonAlloc(step.Origin, radius, step.Direction, SphereCastHits, maxDistance, UnityEngine.Physics.AllLayers);
                    result = TryGetPrioritizedPhysicsHit(SphereCastHits, hitCount, prioritizedLayerMasks, focusIndividualCompoundCollider, out physicsHit);
                }

                return result;
            }
        }

        /// <summary>
        /// Tries to get the prioritized physics raycast hit based on the prioritized layer masks.
        /// </summary>
        /// <remarks>Sorts all hit objects first by layerMask, then by distance.</remarks>
        /// <returns>The minimum distance hit within the first layer that has hits.</returns>
        public static bool TryGetPrioritizedPhysicsHit(
            RaycastHit[] hits,
            LayerMask[] priorityLayers,
            bool focusIndividualCompoundCollider,
            out RaycastHit raycastHit)
        {
            return TryGetPrioritizedPhysicsHit(
                hits,
                hits.Length,
                priorityLayers,
                focusIndividualCompoundCollider,
                out raycastHit);
        }

        private static readonly ProfilerMarker TryGetPrioritizedPhysicsHitPerfMarker = new ProfilerMarker("[MRTK] MixedRealityRaycaster.TryGetPrioritizedPhysicsHit");

        /// <summary>
        /// Tries to get the prioritized physics raycast hit based on the prioritized layer masks.
        /// </summary>
        /// <remarks>Sorts all hit objects first by layerMask, then by distance.</remarks>
        /// <returns>The minimum distance hit within the first layer that has hits.</returns>
        private static bool TryGetPrioritizedPhysicsHit(
            RaycastHit[] hits,
            int hitCount,
            LayerMask[] priorityLayers,
            bool focusIndividualCompoundCollider,
            out RaycastHit raycastHit)
        {
            using (TryGetPrioritizedPhysicsHitPerfMarker.Auto())
            {
                raycastHit = default(RaycastHit);

                if (hits.Length < hitCount)
                {
                    Debug.LogError("TryGetPrioritizedPhysicsHit: hitCount is larger than the hits array.");
                    return false;
                }

                if (hitCount == 0)
                {
                    return false;
                }

                for (int layerMaskIdx = 0; layerMaskIdx < priorityLayers.Length; layerMaskIdx++)
                {
                    RaycastHit? minHit = null;

                    for (int hitIdx = 0; hitIdx < hitCount; hitIdx++)
                    {
                        RaycastHit hit = hits[hitIdx];
                        GameObject targetGameObject = focusIndividualCompoundCollider ? hit.collider.gameObject : hit.transform.gameObject;

                        if (targetGameObject.layer.IsInLayerMask(priorityLayers[layerMaskIdx]) &&
                            (minHit == null || hit.distance < minHit.Value.distance))
                        {
                            minHit = hit;
                        }
                    }

                    if (minHit != null)
                    {
                        raycastHit = minHit.Value;
                        return true;
                    }
                }

                return false;
            }
        }

        private static readonly ProfilerMarker RaycastPlanePhysicsStepPerfMarker = new ProfilerMarker("[MRTK] MixedRealityRaycaster.RaycastSpherePhysicsStep");

        /// <summary>
        /// Intersection test of ray step with given plane.
        /// </summary>
        /// <returns>Whether the ray step intersects the ray step.</returns>
        public static bool RaycastPlanePhysicsStep(RayStep step, Plane plane, out Vector3 hitPoint)
        {
            using (RaycastPlanePhysicsStepPerfMarker.Auto())
            {
                if (plane.Raycast(step, out float intersectDistance))
                {
                    if (intersectDistance <= step.Length)
                    {
                        hitPoint = ((Ray)step).GetPoint(intersectDistance);
                        return true;
                    }
                }

                hitPoint = Vector3.zero;
                return false;
            }
        }
    }
}
