// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Extensions
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
        /// Calculates the bounds of all the colliders attached to this GameObject and all it's children
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
    }
}