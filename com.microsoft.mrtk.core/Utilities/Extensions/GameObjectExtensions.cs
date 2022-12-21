// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        /// <summary>
        /// Set all GameObject children active or inactive based on argument
        /// </summary>
        /// <param name="root">GameObject parent to traverse from</param>
        /// <param name="isActive">Indicates whether children GameObjects should be active or not</param>
        /// <remarks>
        /// Does not call SetActive on the top level GameObject, only its children
        /// </remarks>
        public static void SetChildrenActive(this GameObject root, bool isActive)
        {
            int count = root.transform.childCount;

            for (int i = 0; i < count; i++)
            {
                root.transform.GetChild(i).gameObject.SetActive(isActive);
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
            int count = items.Length;

            for (var i = 0; i < count; i++)
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
        public static void ForEachComponent<T>(this GameObject gameObject, Action<T> action) where T : Component
        {
            T[] components = gameObject.GetComponents<T>();
            int count = components.Length;

            for (int i = 0; i < count; i++)
            {
                action(components[i]);
            }
        }

        /// <summary>
        /// Checks if any MonoBehaviour on the given GameObject is using the RequireComponentAttribute requiring type T
        /// </summary>
        /// <remarks>Only functions when called within a UNITY_EDITOR context. Outside of UNITY_EDITOR, always returns false</remarks>
        /// <typeparam name="T">The potentially required component</typeparam>
        /// <param name="gameObject">the GameObject requiring the component</param>
        /// <param name="requiringTypes">A list of types that do require the component in question</param>
        /// <returns>true if <typeparamref name="T"/> appears in any RequireComponentAttribute, otherwise false </returns>
        public static bool IsComponentRequired<T>(this GameObject gameObject, out List<Type> requiringTypes) where T : Component
        {
            var genericType = typeof(T);
            requiringTypes = null;

#if UNITY_EDITOR
            MonoBehaviour[] monoBehaviours = gameObject.GetComponents<MonoBehaviour>();
            int count = monoBehaviours.Length;

            for (int i = 0; i < count; i++)
            {
                if (monoBehaviours[i] == null)
                {
                    continue;
                }

                var monoBehaviourType = monoBehaviours[i].GetType();
                var attributes = Attribute.GetCustomAttributes(monoBehaviourType);

                foreach (var attribute in attributes)
                {
                    if (attribute is RequireComponent requireComponentAttribute)
                    {
                        if (requireComponentAttribute.m_Type0 == genericType ||
                            requireComponentAttribute.m_Type1 == genericType ||
                            requireComponentAttribute.m_Type2 == genericType)
                        {
                            requiringTypes ??= new List<Type>();
                            requiringTypes.Add(monoBehaviourType);
                        }
                    }
                }
            }
#endif // UNITY_EDITOR

            return requiringTypes != null;
        }
    }
}
