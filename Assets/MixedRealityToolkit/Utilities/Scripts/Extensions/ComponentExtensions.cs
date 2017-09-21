// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Extensions methods for the Unity Component class.
    /// This also includes some component-related extensions for the GameObjet class.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Ensure that a component of type <typeparamref name="T"/> exists on the game object.
        /// If it doesn't exist, creates it.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <param name="gameObject">Game object on which component should be.</param>
        /// <returns>The component that was retrieved or created.</returns>
        public static T EnsureComponent<T>(this GameObject gameObject) where T : Component
        {
            T foundComponent = gameObject.GetComponent<T>();
            if (foundComponent == null)
            {
                return gameObject.AddComponent<T>();
            }

            return foundComponent;
        }

        /// <summary>
        /// Ensure that a component of type <typeparamref name="T"/> exists on the game object.
        /// If it doesn't exist, creates it.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <param name="component">A component on the game object for which a component of type <typeparamref name="T"/> should exist.</param>
        /// <returns>The component that was retrieved or created.</returns>
        public static T EnsureComponent<T>(this Component component) where T : Component
        {
            return EnsureComponent<T>(component.gameObject);
        }

        /// <summary>
        /// Apply the specified delegate to all objects in the hierarchy under a specified game object.
        /// </summary>
        /// <param name="root">Root game object of the hierarchy.</param>
        /// <param name="action">Delegate to apply.</param>
        public static void ApplyToHierarchy(this GameObject root, Action<GameObject> action)
        {
            action(root);
            foreach (var item in root.GetComponentsInChildren<Transform>())
            {
                action(item.gameObject);
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
        /// Find the first component of type <typeparamref name="T"/> in the ancestors of the game object of the specified component.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="component">Component for which its game object's ancestors must be considered.</param>
        /// <param name="includeSelf">Indicates whether the specified game object should be included.</param>
        /// <returns>The component of type <typeparamref name="T"/>. Null if it none was found.</returns>
        public static T FindAncestorComponent<T>(this Component component, bool includeSelf = true) where T : Component
        {
            return component.transform.FindAncestorComponent<T>(includeSelf);
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
        /// Perform an action on every component of type T that is on this GameObject
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        /// <param name="g">this gameObject</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEachComponent<T>(this GameObject g, Action<T> action)
        {
            foreach (T i in g.GetComponents<T>())
            {
                action(i);
            }
        }
    }
}