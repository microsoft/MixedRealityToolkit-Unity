// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's GameObject class
    /// </summary>
    public static class GameObjectExtensions
    {
        public static void SetChildrenActive(this GameObject root, bool isActive)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                root.transform.GetChild(i).gameObject.SetActive(false);
            }
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
                throw new ArgumentNullException(nameof(root), "Root transform can't be null.");
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
            if (root == null) { throw new ArgumentNullException(nameof(root)); }

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
            if (root == null) { throw new ArgumentNullException(nameof(root)); }
            if (cache == null) { throw new ArgumentNullException(nameof(cache)); }

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                int layer;
                if (!cache.TryGetValue(child.gameObject, out layer)) { continue; }
                child.gameObject.layer = layer;
                cache.Remove(child.gameObject);
            }
        }

        /// <summary>
        /// Determines whether or not a game object's layer is included in the specified layer mask.
        /// </summary>
        /// <param name="gameObject">The game object whose layer to test.</param>
        /// <param name="layerMask">The layer mask to test against.</param>
        /// <returns>True if <paramref name="gameObject"/>'s layer is included in <paramref name="layerMask"/>, false otherwise.</returns>
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            LayerMask gameObjectMask = 1 << gameObject.layer;
            return (gameObjectMask & layerMask) == gameObjectMask;
        }

        /// <summary>
        /// Apply the specified delegate to all objects in the hierarchy under a specified game object.
        /// </summary>
        /// <param name="root">Root game object of the hierarchy.</param>
        /// <param name="action">Delegate to apply.</param>
        public static void ApplyToHierarchy(this GameObject root, Action<GameObject> action)
        {
            action(root);
            Transform[] items = root.GetComponentsInChildren<Transform>();

            for (var i = 0; i < items.Length; i++)
            {
                action(items[i].gameObject);
            }
        }

        /// <summary>
        /// Find the first component of type <typeparamref name="T"/> in the ancestors of the specified game object.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="gameObject">Game object for which ancestors must be considered.</param>
        /// <param name="includeSelf">Indicates whether the specified game object should be included.</param>
        /// <returns>The component of type <typeparamref name="T"/>. Null if it none was found.</returns>
        public static T FindAncestorComponent<T>(this GameObject gameObject, bool includeSelf = true) where T : Component
        {
            return gameObject.transform.FindAncestorComponent<T>(includeSelf);
        }

        /// <summary>
        /// Perform an action on every component of type T that is on this GameObject
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        /// <param name="gameObject">this gameObject</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEachComponent<T>(this GameObject gameObject, Action<T> action)
        {
            foreach (T i in gameObject.GetComponents<T>())
            {
                action(i);
            }
        }

        /// <summary>
        /// Destroys gameobject appropriately depending if in edit or playmode
        /// </summary>
        /// <param name="gameObject">gameobject to destroy</param>
        /// <param name="t">time in seconds at which to destroy GameObject if applicable</param>
        public static void DestroyGameObject(GameObject gameObject, float t = 0.0f)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(gameObject, t);
            }
            else
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}
