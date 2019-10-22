// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Convenience class for setting Editor Preferences with <see href="https://docs.unity3d.com/ScriptReference/Application-productName.html">Application.productName</see> as key prefix.
    /// </summary>
    public static class EditorPreferences
    {
        /// <summary>
        /// Set the saved <see cref="string"/> from to <see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static void Set(string key, string value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetString($"{Application.productName}_{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="bool"/> from to <see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static void Set(string key, bool value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetBool($"{Application.productName}_{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="float"/> from the <see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static void Set(string key, float value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetFloat($"{Application.productName}_{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="int"/> from the<see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static void Set(string key, int value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetInt($"{Application.productName}_{key}", value);
        }

        /// <summary>
        /// Get the saved <see cref="string"/> from the<see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static string Get(string key, string defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{Application.productName}_{key}"))
            {
                return EditorPrefs.GetString($"{Application.productName}_{key}");
            }

            EditorPrefs.SetString($"{Application.productName}_{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="bool"/> from the <see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static bool Get(string key, bool defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{Application.productName}_{key}"))
            {
                return EditorPrefs.GetBool($"{Application.productName}_{key}");
            }

            EditorPrefs.SetBool($"{Application.productName}_{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="float"/> from the <see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static float Get(string key, float defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{Application.productName}_{key}"))
            {
                return EditorPrefs.GetFloat($"{Application.productName}_{key}");
            }

            EditorPrefs.SetFloat($"{Application.productName}_{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="int"/> from the <see href="https://docs.unity3d.com/ScriptReference/EditorPrefs.html">EditorPrefs</see>.
        /// </summary>
        public static int Get(string key, int defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{Application.productName}_{key}"))
            {
                return EditorPrefs.GetInt($"{Application.productName}_{key}");
            }

            EditorPrefs.SetInt($"{Application.productName}_{key}", defaultValue);
            return defaultValue;
        }
    }
}
