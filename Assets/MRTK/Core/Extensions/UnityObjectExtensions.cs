// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's Object class
    /// </summary>
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Enable Unity objects to skip "DontDestroyOnLoad" when editor isn't playing so test runner passes.
        /// </summary>
        public static void DontDestroyOnLoad(this Object target)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
#endif
                Object.DontDestroyOnLoad(target);
        }

        /// <summary>
        /// Destroys a Unity object appropriately depending if running in edit or play mode.
        /// </summary>
        /// <param name="obj">Unity object to destroy</param>
        /// <param name="t">Time in seconds at which to destroy the object, if applicable.</param>
        public static void DestroyObject(Object obj, float t = 0.0f)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj, t);
            }
            else
            {
            #if UNITY_EDITOR
                // Must use DestroyImmediate in edit mode but it is not allowed when called from 
                // trigger/contact, animation event callbacks or OnValidate. Must use Destroy instead.
                // Delay call to counter this issue in editor
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    Object.DestroyImmediate(obj);
                };
            #else
                Object.DestroyImmediate(obj);
            #endif
            }
        }

        /// <summary>
        /// Tests if the Unity object is null. Checks both the managed object and the underly Unity-managed native object
        /// </summary>
        /// <returns>True if either the managed or native object is null, false otherwise</returns>
        public static bool IsNull(Object obj)
        {
            return obj == null || obj.Equals(null);
        }
    }
}
