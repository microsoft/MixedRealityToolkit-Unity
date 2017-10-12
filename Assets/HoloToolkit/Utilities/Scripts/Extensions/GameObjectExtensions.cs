// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Extension methods for Unity's GameObject class
    /// </summary>
    public static class GameObjectExtensions
    {
        [Obsolete("Use the more extensive TransformExtensions.GetFullPath instead.")]
        public static string GetFullPath(this GameObject go)
        {
            return go.transform.GetFullPath("/", "");
        }

        /// <summary>
        /// Set the layer to the given object and the full hierarchy below it.
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="layer">The layer to apply</param>
        public static void SetLayerRecursively(this GameObject root, int layer)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root", "Root transform can't be null.");
            }

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Set the layer to the given object and the full hierarchy below it and cache the previous layers in the out parameter.
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="layer">The layer to apply</param>
        /// <param name="cache">The previously set layer for each object</param>
        public static void SetLayerRecursively(this GameObject root, int layer, out Dictionary<GameObject, int> cache)
        {
            if (root == null) { throw new ArgumentNullException("root"); }

            cache = new Dictionary<GameObject, int>();

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                cache[child.gameObject] = child.gameObject.layer;
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Reapplies previously cached hierarchy layers
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="cache">The previously set layer for each object</param>
        public static void ApplyLayerCacheRecursively(this GameObject root, Dictionary<GameObject, int> cache)
        {
            if (root == null) { throw new ArgumentNullException("root"); }
            if (cache == null) { throw new ArgumentNullException("cache"); }

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                int layer;
                if (!cache.TryGetValue(child.gameObject, out layer)) { continue; }
                child.gameObject.layer = layer;
                cache.Remove(child.gameObject);
            }
        }

        /// <summary>
        /// Gets the GameObject's root Parent object.
        /// </summary>
        /// <param name="child">The GameObject we're trying to find the root parent for.</param>
        /// <returns>The Root parent GameObject.</returns>
        public static GameObject GetParentRoot(this GameObject child)
        {
            if (child.transform.parent == null)
            {
                return child;
            }
            else
            {
                return GetParentRoot(child.transform.parent.gameObject);
            }
        }
    }
}
