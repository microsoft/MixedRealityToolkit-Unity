// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utility to save preferences that should be saved per project (i.e to source control) across MRTK. Supports primitive preferences bool, int, and float
    /// </summary>
    [CreateAssetMenu(fileName = "ProjectPreferences", menuName = "Mixed Reality Toolkit/TestProjectPrefs", order = 1)]
    public class ProjectPreferences : ScriptableObject
    {
        // Dictionary is not Serializable by default and furthermore System.object is not Serializable
        // Thus, it is difficult to create a generic data bag. Instead we will create instances for each key preference types
        [System.Serializable]
        private class BoolPreferences : SerializableDictionary<string, bool> { }

        [System.Serializable]
        private class IntPreferences : SerializableDictionary<string, int> { }

        [System.Serializable]
        private class FloatPreferences : SerializableDictionary<string, float> { }

        [System.Serializable]
        private class StringPreferences : SerializableDictionary<string, string> { }

        [SerializeField]
        private BoolPreferences boolPreferences = new BoolPreferences();

        [SerializeField]
        private IntPreferences intPreferences = new IntPreferences();

        [SerializeField]
        private FloatPreferences floatPreferences = new FloatPreferences();

        [SerializeField]
        private StringPreferences stringPreferences = new StringPreferences();

        private const string FILE_NAME = "ProjectPreferences.asset";
        private const string RELATIVE_FOLDER_PATH = "System/";
        private const MixedRealityToolkitModuleType MODULE = MixedRealityToolkitModuleType.Generated;

        private static ProjectPreferences _instance;
        private static ProjectPreferences Instance
        {
            get
            {
                if (_instance == null)
                {
                    string filePath = MixedRealityToolkitFiles.MapRelativeFilePath(MODULE, FILE_NAME);
                    if (string.IsNullOrEmpty(filePath))
                    {
                        // MapRelativeFilePath returned null, need to build path ourselves
                        filePath = MixedRealityToolkitFiles.MapModulePath(MODULE) + "/" + FILE_NAME;

                        _instance = ScriptableObject.CreateInstance<ProjectPreferences>();
                        AssetDatabase.CreateAsset(_instance, filePath);
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        _instance = (ProjectPreferences)AssetDatabase.LoadAssetAtPath(filePath, typeof(ProjectPreferences));
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Save bool to preferences and save to ScriptableObject with key given. Calls AssetDatabase.SaveAssets which saves all assets after execution
        /// </summary>
        public static void Set(string key, bool value)
        {
            Set<bool>(key, value, Instance.boolPreferences);
        }

        /// <summary>
        /// Save float to preferences and save to ScriptableObject with key given. Calls AssetDatabase.SaveAssets which saves all assets after execution
        /// </summary>
        public static void Set(string key, float value)
        {
            Set<float>(key, value, Instance.floatPreferences);
        }

        /// <summary>
        /// Save int to preferences and save to ScriptableObject with key given. Calls AssetDatabase.SaveAssets which saves all assets after execution
        /// </summary>
        public static void Set(string key, int value)
        {
            Set<int>(key, value, Instance.intPreferences);
        }

        /// <summary>
        /// Save string to preferences and save to ScriptableObject with key given. Calls AssetDatabase.SaveAssets which saves all assets after execution
        /// </summary>
        public static void Set(string key, string value)
        {
            Set<string>(key, value, Instance.stringPreferences);
        }

        /// <summary>
        /// Get bool from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static bool Get(string key, bool defaultValue)
        {
            return Get<bool>(key, defaultValue, Instance.boolPreferences);
        }

        /// <summary>
        /// Get float from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static float Get(string key, float defaultValue)
        {
            return Get<float>(key, defaultValue, Instance.floatPreferences);
        }

        /// <summary>
        /// Get int from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static int Get(string key, int defaultValue)
        {
            return Get<int>(key, defaultValue, Instance.intPreferences);
        }

        /// <summary>
        /// Get string from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static string Get(string key, string defaultValue)
        {
            return Get<string>(key, defaultValue, Instance.stringPreferences);
        }

        private static void Set<T>(string key, T item, SerializableDictionary<string, T> target)
        {
            if (target.ContainsKey(key))
            {
                target[key] = item;
            }
            else
            {
                target.Add(key, item);
            }

            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
        }

        private static T Get<T>(string key, T defaultVal, SerializableDictionary<string, T> target)
        {
            if (target.ContainsKey(key))
            {
                return target[key];
            }
            else
            {
                Set<T>(key, defaultVal, target);
                return defaultVal;
            }
        }
    }
}
