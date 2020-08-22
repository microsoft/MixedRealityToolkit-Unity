// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        /// Tests if an interface is null, taking potential UnityEngine.Object derived class implementers into account
        /// which require their overridden operators to be called
        /// </summary>
        /// <returns>True if either the managed or native object is null, false otherwise</returns>
        public static bool IsNull<T>(this T @interface) where T : class => @interface == null || @interface.Equals(null);

        /// <summary>
        /// Properly checks an interface for null and returns the MonoBehaviour implementing it
        /// </summary>
        /// <returns> True if the implementer of the interface is not a MonoBehaviour or the MonoBehaviour is null</returns>
        public static bool TryGetMonoBehaviour<T>(this T @interface, out MonoBehaviour monoBehaviour) where T : class => (monoBehaviour = @interface as MonoBehaviour) != null;
    }
}
