// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HoloToolkit.Unity
{
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
            StringBuilder stringBuilder = new StringBuilder();
            GetFullPath(stringBuilder, transform, delimiter, prefix);
            return stringBuilder.ToString();
        }

        private static void GetFullPath(StringBuilder stringBuilder, Transform transform, string delimiter, string prefix)
        {
            if (transform.parent == null)
            {
                stringBuilder.Append(prefix);
            } else
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
    }
}