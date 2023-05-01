// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Static class that contains various useful methods for computing the total bounds of a collection of objects.
    /// </summary>
    public static class BoundsCalculator
    {
        /// <summary>
        /// This enum defines what volume type the bound calculation depends on and its priority.
        /// </summary>
        public enum BoundsCalculationMethod
        {
            /// <summary>
            /// Used Renderers for the bounds calculation and Colliders as a fallback
            /// </summary>
            RendererOverCollider = 0,

            /// <summary>
            /// Used Colliders for the bounds calculation and Renderers as a fallback
            /// </summary>
            ColliderOverRenderer,

            /// <summary>
            /// Omits Renderers and uses Colliders for the bounds calculation exclusively
            /// </summary>
            ColliderOnly,

            /// <summary>
            /// Omits Colliders and uses Renderers for the bounds calculation exclusively
            /// </summary>
            RendererOnly,
        }

        // Private scratchpad to reduce allocs.
        private static List<Vector3> totalBoundsCorners = new List<Vector3>(8);

        // Private scratchpad to reduce allocs.
        private static List<Transform> childTransforms = new List<Transform>();

        // Private scratchpad to reduce allocs.
        private static Vector3[] cornersToWorld = new Vector3[8];

        /// <summary>
        /// Compute Bounds, localized to <paramref name="root"/>, of all children of <paramref name="root"/>, excluding <paramref name="exclude"/>.
        /// </summary>
        /// <remarks>
        /// This is quite expensive, so call sparingly! This traverses the entire hierarchy and does quite a lot of work on each node.
        /// </remarks>
        /// <param name="root">The root transform under which this method will traverse + calculate composite bounds.</param>
        /// <param name="exclude">The transform to exclude from the bounds calculation. Only valid if a direct child of <paramref name="root"/>.</param>
        /// <param name="boundsCalculationMethod">Method to use to calculate bounds.</param>
        /// <param name="containsCanvas">
        /// Set to true if the bounds calculation finds a RectTransform. If true, it is recommended to re-run
        /// this bounds calculation once more after a single frame delay, to make sure the computed layout sizing is taken into account.
        /// </param>
        /// <param name="includeInactiveObjects">Should objects that are currently inactive be included in the bounds calculation?</param>
        /// <param name="abortOnCanvas">Should we early-out if we find a canvas element? Will still set <paramref name="containsCanvas"/> = true.</param>
        internal static Bounds CalculateBounds(Transform root, Transform target, Transform exclude, out bool containsCanvas,
                                               BoundsCalculationMethod boundsCalculationMethod = BoundsCalculationMethod.RendererOverCollider,
                                               bool includeInactiveObjects = false,
                                               bool abortOnCanvas = false)
        {
            totalBoundsCorners.Clear();
            childTransforms.Clear();
            containsCanvas = false;
            target.GetComponentsInChildren<Transform>(includeInactiveObjects, childTransforms);

            // Iterate transforms and collect bound volumes
            foreach (Transform childTransform in childTransforms)
            {
                // Reject if child of exclude 
                if (childTransform.IsChildOf(exclude)) { continue; }

                containsCanvas |= childTransform is RectTransform;
                if (containsCanvas && abortOnCanvas) { break; }

                ExtractBoundsCorners(childTransform, boundsCalculationMethod);
            }

            if (totalBoundsCorners.Count == 0)
            {
                return new Bounds();
            }

            Bounds finalBounds = new Bounds(root.InverseTransformPoint(totalBoundsCorners[0]), Vector3.zero);

            for (int i = 1; i < totalBoundsCorners.Count; i++)
            {
                finalBounds.Encapsulate(root.InverseTransformPoint(totalBoundsCorners[i]));
            }

            return finalBounds;
        }

        /// <summary>
        /// Compute the flattening vector for a bounds of size <paramref name="size"/>.
        /// <returns>
        /// Returns a unit vector along the direction of the smallest component of <paramref name="size"/>.
        /// </returns>
        /// <remarks>
        /// Returns Vector3.forward if all components are approximately equal.
        /// </remarks>
        /// <param name="size">The size of the bounds to compute the flatten vector for.</param>
        public static Vector3 CalculateFlattenVector(Vector3 size)
        {
            if (size.x < size.y && size.x < size.z)
            {
                return Vector3.right;
            }
            else if (size.y < size.x && size.y < size.z)
            {
                return Vector3.up;
            }
            else
            {
                return Vector3.forward;
            }
        }

        private static void ExtractBoundsCorners(Transform childTransform, BoundsCalculationMethod boundsCalculationMethod)
        {
            KeyValuePair<Transform, Collider> colliderByTransform = default;
            KeyValuePair<Transform, Bounds> rendererBoundsByTransform = default;

            if (boundsCalculationMethod != BoundsCalculationMethod.RendererOnly)
            {
                if (childTransform.TryGetComponent(out Collider collider))
                {
                    colliderByTransform = new KeyValuePair<Transform, Collider>(childTransform, collider);
                }
                else
                {
                    colliderByTransform = new KeyValuePair<Transform, Collider>();
                }
            }

            if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
            {
                MeshFilter meshFilter = childTransform.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>(childTransform, meshFilter.sharedMesh.bounds);
                }
                else if (childTransform is RectTransform rt)
                {
                    rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>(childTransform, new Bounds(rt.rect.center, new Vector3(rt.rect.width, rt.rect.height, 0.1f)));
                }
                else
                {
                    rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>();
                }
            }

            // Encapsulate the collider bounds if criteria match
            if (boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly ||
                boundsCalculationMethod == BoundsCalculationMethod.ColliderOverRenderer)
            {
                if (AddColliderBoundsCornersToTarget(colliderByTransform) && boundsCalculationMethod == BoundsCalculationMethod.ColliderOverRenderer ||
                    boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly) { return; }
            }

            // Encapsulate the renderer bounds if criteria match
            if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
            {
                if (AddRendererBoundsCornersToTarget(rendererBoundsByTransform) && boundsCalculationMethod == BoundsCalculationMethod.RendererOverCollider ||
                    boundsCalculationMethod == BoundsCalculationMethod.RendererOnly) { return; }
            }

            // Do the collider for the one case that we chose RendererOverCollider and did not find a renderer
            AddColliderBoundsCornersToTarget(colliderByTransform);
        }

        private static bool AddRendererBoundsCornersToTarget(KeyValuePair<Transform, Bounds> rendererBoundsByTarget)
        {
            if (rendererBoundsByTarget.Key == null) { return false; }

            rendererBoundsByTarget.Value.GetCornerPositions(rendererBoundsByTarget.Key, ref cornersToWorld);
            totalBoundsCorners.AddRange(cornersToWorld);
            return true;
        }

        private static bool AddColliderBoundsCornersToTarget(KeyValuePair<Transform, Collider> colliderByTransform)
        {
            if (colliderByTransform.Key != null)
            {
                BoundsExtensions.GetColliderBoundsPoints(colliderByTransform.Value, totalBoundsCorners, 0);
            }

            return colliderByTransform.Key != null;
        }
    }
}
