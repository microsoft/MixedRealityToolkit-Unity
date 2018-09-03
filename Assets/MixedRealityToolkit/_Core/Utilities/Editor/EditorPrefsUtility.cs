// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Editor
{
    /// <summary>
    /// Convenience class for setting Editor Preferences.
    /// </summary>
    public static class EditorPrefsUtility
    {
        /// <summary>
        /// Set the saved <see cref="string"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetEditorPref(string key, string value)
        {
            EditorPrefs.SetString($"{Application.productName}{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="bool"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetEditorPref(string key, bool value)
        {
            EditorPrefs.SetBool($"{Application.productName}{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="float"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetEditorPref(string key, float value)
        {
            EditorPrefs.SetFloat($"{Application.productName}{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="int"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetEditorPref(string key, int value)
        {
            EditorPrefs.SetInt($"{Application.productName}{key}", value);
        }

        /// <summary>
        /// Get the saved <see cref="string"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetEditorPref(string key, string defaultValue)
        {
            if (EditorPrefs.HasKey($"{Application.productName}{key}"))
            {
                return EditorPrefs.GetString($"{Application.productName}{key}");
            }

            EditorPrefs.SetString($"{Application.productName}{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="bool"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetEditorPref(string key, bool defaultValue)
        {
            if (EditorPrefs.HasKey($"{Application.productName}{key}"))
            {
                return EditorPrefs.GetBool($"{Application.productName}{key}");
            }

            EditorPrefs.SetBool($"{Application.productName}{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="float"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetEditorPref(string key, float defaultValue)
        {
            if (EditorPrefs.HasKey($"{Application.productName}{key}"))
            {
                return EditorPrefs.GetFloat($"{Application.productName}{key}");
            }

            EditorPrefs.SetFloat($"{Application.productName}{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="int"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetEditorPref(string key, int defaultValue)
        {
            if (EditorPrefs.HasKey($"{Application.productName}{key}"))
            {
                return EditorPrefs.GetInt($"{Application.productName}{key}");
            }

            EditorPrefs.SetInt($"{Application.productName}{key}", defaultValue);
            return defaultValue;
        }
    }
}
