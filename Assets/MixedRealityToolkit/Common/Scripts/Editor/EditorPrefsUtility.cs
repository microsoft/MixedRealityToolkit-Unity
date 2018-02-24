// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Common.EditorScript
{
    public static class EditorPrefsUtility
    {
        public static void SetEditorPref(string key, string value)
        {
            EditorPrefs.SetString(Application.productName + key, value);
        }

        public static void SetEditorPref(string key, bool value)
        {
            EditorPrefs.SetBool(Application.productName + key, value);
        }

        public static void SetEditorPref(string key, float value)
        {
            EditorPrefs.SetFloat(Application.productName + key, value);
        }

        public static void SetEditorPref(string key, int value)
        {
            EditorPrefs.SetInt(Application.productName + key, value);
        }

        public static string GetEditorPref(string key, string defaultValue)
        {
            if (EditorPrefs.HasKey(Application.productName + key))
            {
                return EditorPrefs.GetString(Application.productName + key);
            }

            EditorPrefs.SetString(Application.productName + key, defaultValue);
            return defaultValue;
        }

        public static bool GetEditorPref(string key, bool defaultValue)
        {
            if (EditorPrefs.HasKey(Application.productName + key))
            {
                return EditorPrefs.GetBool(Application.productName + key);
            }

            EditorPrefs.SetBool(Application.productName + key, defaultValue);
            return defaultValue;
        }

        public static float GetEditorPref(string key, float defaultValue)
        {
            if (EditorPrefs.HasKey(Application.productName + key))
            {
                return EditorPrefs.GetFloat(Application.productName + key);
            }

            EditorPrefs.SetFloat(Application.productName + key, defaultValue);
            return defaultValue;
        }

        public static int GetEditorPref(string key, int defaultValue)
        {
            if (EditorPrefs.HasKey(Application.productName + key))
            {
                return EditorPrefs.GetInt(Application.productName + key);
            }

            EditorPrefs.SetInt(Application.productName + key, defaultValue);
            return defaultValue;
        }
    }
}
