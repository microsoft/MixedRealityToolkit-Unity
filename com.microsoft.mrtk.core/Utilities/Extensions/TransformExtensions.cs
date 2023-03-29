// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's Transform class
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// An extension method that will get you the full path to an object.
        /// </summary>
        /// <param name="transform">The transform you wish a full path to.</param>
        /// <param name="delimiter">The delimiter with which each object is delimited in the string.</param>
        /// <param name="prefix">Prefix with which the full path to the object should start.</param>
        /// <returns>A delimited string that is the full path to the game object in the hierarchy.</returns>
        public static string GetFullPath(this Transform transform, string delimiter = ".", string prefix = "/")
        {
            var stringBuilder = new StringBuilder();
            GetFullPath(stringBuilder, transform, delimiter, prefix);
            return stringBuilder.ToString();
        }

        private static void GetFullPath(StringBuilder stringBuilder, Transform transform, string delimiter, string prefix)
        {
            if (transform.parent == null)
            {
                stringBuilder.Append(prefix);
            }
            else
            {
                GetFullPath(stringBuilder, transform.parent, delimiter, prefix);
                stringBuilder.Append(delimiter);
            }

            stringBuilder.Append(transform.name);
        }

        /// <summary>
        /// Enumerates all children in the hierarchy starting at the root object.
        /// </summary>
        /// <param name="root">Start point of the traversion set</param>
        public static IEnumerable<Transform> EnumerateHierarchy(this Transform root)
        {
            if (root == null) { throw new ArgumentNullException("root"); }
            return root.EnumerateHierarchyCore(new List<Transform>(0));
        }

        /// <summary>
        /// Enumerates all children in the hierarchy starting at the root object except for the branches in ignore.
        /// </summary>
        /// <param name="root">Start point of the traversion set</param>
        /// <param name="ignore">Transforms and all its children to be ignored</param>
        public static IEnumerable<Transform> EnumerateHierarchy(this Transform root, ICollection<Transform> ignore)
        {
            if (root == null) { throw new ArgumentNullException("root"); }
            if (ignore == null)
            {
                throw new ArgumentNullException("ignore", "Ignore collection can't be null, use EnumerateHierarchy(root) instead.");
            }
            return root.EnumerateHierarchyCore(ignore);
        }

        /// <summary>
        /// Enumerates all children in the hierarchy starting at the root object except for the branches in ignore.
        /// </summary>
        /// <param name="root">Start point of the traversion set</param>
        /// <param name="ignore">Transforms and all its children to be ignored</param>
        private static IEnumerable<Transform> EnumerateHierarchyCore(this Transform root, ICollection<Transform> ignore)
        {
            var transformQueue = new Queue<Transform>();
            transformQueue.Enqueue(root);

            while (transformQueue.Count > 0)
            {
                var parentTransform = transformQueue.Dequeue();

                if (!parentTransform || ignore.Contains(parentTransform)) { continue; }

                for (var i = 0; i < parentTransform.childCount; i++)
                {
                    transformQueue.Enqueue(parentTransform.GetChild(i));
                }

                yield return parentTransform;
            }
        }

        /// <summary>
        /// Calculates the bounds of all the colliders attached to this GameObject and all its children
        /// </summary>
        /// <param name="transform">Transform of root GameObject the colliders are attached to </param>
        /// <returns>The total bounds of all colliders attached to this GameObject. 
        /// If no colliders attached, returns a bounds of center and extents 0</returns>
        public static Bounds GetColliderBounds(this Transform transform)
        {
            Collider[] colliders = transform.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0) { return new Bounds(); }

            Bounds bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }
            return bounds;
        }

        /// <summary>
        /// Checks if the provided transforms are child/parent related.
        /// </summary>
        /// <returns>True if either transform is the parent of the other or if they are the same</returns>
        public static bool IsParentOrChildOf(this Transform transform1, Transform transform2)
        {
            return transform1.IsChildOf(transform2) || transform2.IsChildOf(transform1);
        }

        /// <summary>
        /// Find the first component of type <typeparamref name="T"/> in the ancestors of the specified transform.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="startTransform">Transform for which ancestors must be considered.</param>
        /// <param name="includeSelf">Indicates whether the specified transform should be included.</param>
        /// <returns>The component of type <typeparamref name="T"/>. Null if it none was found.</returns>
        public static T FindAncestorComponent<T>(this Transform startTransform, bool includeSelf = true) where T : Component
        {
            foreach (Transform transform in startTransform.EnumerateAncestors(includeSelf))
            {
                T component = transform.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// Enumerates the ancestors of the specified transform.
        /// </summary>
        /// <param name="startTransform">Transform for which ancestors must be returned.</param>
        /// <param name="includeSelf">Indicates whether the specified transform should be included.</param>
        /// <returns>An enumeration of all ancestor transforms of the specified start transform.</returns>
        public static IEnumerable<Transform> EnumerateAncestors(this Transform startTransform, bool includeSelf)
        {
            if (!includeSelf)
            {
                startTransform = startTransform.parent;
            }

            for (Transform transform = startTransform; transform != null; transform = transform.parent)
            {
                yield return transform;
            }
        }

        /// <summary>
        /// Transforms the size from local to world.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="localSize">The local size.</param>
        /// <returns>World size.</returns>
        public static Vector3 TransformSize(this Transform transform, Vector3 localSize)
        {
            Vector3 transformedSize = new Vector3(localSize.x, localSize.y, localSize.z);

            Transform t = transform;
            do
            {
                transformedSize.x *= t.localScale.x;
                transformedSize.y *= t.localScale.y;
                transformedSize.z *= t.localScale.z;
                t = t.parent;
            }
            while (t != null);

            return transformedSize;
        }

        /// <summary>
        /// Transforms the size from world to local.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="worldSize">The world size</param>
        /// <returns>World size.</returns>
        public static Vector3 InverseTransformSize(this Transform transform, Vector3 worldSize)
        {
            Vector3 transformedSize = new Vector3(worldSize.x, worldSize.y, worldSize.z);

            Transform t = transform;
            do
            {
                transformedSize.x /= t.localScale.x;
                transformedSize.y /= t.localScale.y;
                transformedSize.z /= t.localScale.z;
                t = t.parent;
            }
            while (t != null);

            return transformedSize;
        }

        /// <summary>
        /// Gets the hierarchical depth of the Transform from its root. Returns -1 if the transform is the root.
        /// </summary>
        /// <param name="t">The transform to get the depth for.</param>
        public static int GetDepth(this Transform t)
        {
            int depth = -1;

            Transform root = t.transform.root;
            if (root == t.transform)
            {
                return depth;
            }

            TryGetDepth(t, root, ref depth);

            return depth;
        }

        /// <summary>
        /// Tries to get the hierarchical depth of the Transform from the specified parent. This method is recursive.
        /// </summary>
        /// <param name="target">The transform to get the depth for</param>
        /// <param name="parent">The starting transform to look for the target transform in</param>
        /// <param name="depth">The depth of the target transform</param>
        /// <returns>'true' if the depth could be retrieved, or 'false' because the transform is a root transform.</returns>
        public static bool TryGetDepth(Transform target, Transform parent, ref int depth)
        {
            foreach (Transform child in parent)
            {
                depth++;
                if (child == target.transform || TryGetDepth(target, child, ref depth))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Walk hierarchy looking for named transform
        /// </summary>
        /// <param name="t">root transform to start searching from</param>
        /// <param name="name">name to look for</param>
        /// <returns>returns found transform or null if none found</returns>
        public static Transform GetChildRecursive(Transform t, string name)
        {
            int numChildren = t.childCount;
            for (int ii = 0; ii < numChildren; ++ii)
            {
                Transform child = t.GetChild(ii);
                if (child.name == name)
                {
                    return child;
                }
                Transform foundIt = GetChildRecursive(child, name);
                if (foundIt != null)
                {
                    return foundIt;
                }
            }
            return null;
        }
    }
}
