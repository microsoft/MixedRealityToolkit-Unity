// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utility to save preferences that should be saved per project (i.e to source control) across MRTK. Supports primitive preferences bool, int, and float
    /// </summary>
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

        protected static string FilePath => MixedRealityToolkitFiles.MapRelativeFilePath(MODULE, DEFAULT_FILE_NAME);

        private const string DEFAULT_FILE_NAME = "ProjectPreferences.asset";
        private const MixedRealityToolkitModuleType MODULE = MixedRealityToolkitModuleType.Generated;
        private static ProjectPreferences _instance;
        private static ProjectPreferences Instance
        {
            get
            {
                if (_instance == null)
                {
                    string filePath = FilePath;
                    if (string.IsNullOrEmpty(filePath))
                    {
                        // MapRelativeFilePath returned null, need to build path ourselves
                        string modulePath = MixedRealityToolkitFiles.MapModulePath(MODULE);
                        if (!string.IsNullOrEmpty(modulePath))
                        {
                            filePath = Path.Combine(modulePath, DEFAULT_FILE_NAME);
                            _instance = CreateInstance<ProjectPreferences>();
                            AssetDatabase.CreateAsset(_instance, filePath);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    else
                    {
                        // Sometimes Unity has weird bug where asset file exists but Unity will not load it resulting in _instance = null. 
                        // Force refresh of asset database before we try to access our preferences file
                        AssetDatabase.Refresh();
                        _instance = (ProjectPreferences)AssetDatabase.LoadAssetAtPath(filePath, typeof(ProjectPreferences));
                    }
                }

                return _instance;
            }
        }

        #region Setters

        /// <summary>
        /// Save bool to preferences and save to ScriptableObject with key given.
        /// </summary>
        /// <remarks>
        /// If forceSave is true (default), then will call AssetDatabase.SaveAssets which saves all assets after execution
        /// </remarks>
        public static void Set(string key, bool value, bool forceSave = true) => Set(key, value, Instance != null ? Instance.boolPreferences : null, forceSave);

        /// <summary>
        /// Save float to preferences and save to ScriptableObject with key given.
        /// </summary>
        /// <remarks>
        /// If forceSave is true (default), then will call AssetDatabase.SaveAssets which saves all assets after execution
        /// </remarks>
        public static void Set(string key, float value, bool forceSave = true) => Set(key, value, Instance != null ? Instance.floatPreferences : null, forceSave);

        /// <summary>
        /// Save int to preferences and save to ScriptableObject with key given.
        /// </summary>
        /// <remarks>
        /// If forceSave is true (default), then will call AssetDatabase.SaveAssets which saves all assets after execution
        /// </remarks>
        public static void Set(string key, int value, bool forceSave = true) => Set(key, value, Instance != null ? Instance.intPreferences : null, forceSave);

        /// <summary>
        /// Save string to preferences and save to ScriptableObject with key given.
        /// </summary>
        /// <remarks>
        /// If forceSave is true (default), then will call AssetDatabase.SaveAssets which saves all assets after execution
        /// </remarks>
        public static void Set(string key, string value, bool forceSave = true) => Set(key, value, Instance != null ? Instance.stringPreferences : null, forceSave);

        #endregion

        #region Getters

        /// <summary>
        /// Get bool from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static bool Get(string key, bool defaultValue) => Get(key, defaultValue, Instance != null ? Instance.boolPreferences : null);

        /// <summary>
        /// Get float from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static float Get(string key, float defaultValue) => Get(key, defaultValue, Instance != null ? Instance.floatPreferences : null);

        /// <summary>
        /// Get int from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static int Get(string key, int defaultValue) => Get(key, defaultValue, Instance != null ? Instance.intPreferences : null);

        /// <summary>
        /// Get string from Project Preferences. If no entry found, then create new entry with provided defaultValue
        /// </summary>
        public static string Get(string key, string defaultValue) => Get(key, defaultValue, Instance != null ? Instance.stringPreferences : null);

        #endregion

        #region Remove

        /// <summary>
        /// Remove key item from preferences if applicable
        /// </summary>
        public static void RemoveBool(string key) => Remove(key, Instance != null ? Instance.boolPreferences : null);

        /// <summary>
        /// Remove key item from preferences if applicable
        /// </summary>
        public static void RemoveFloat(string key) => Remove(key, Instance != null ? Instance.floatPreferences : null);

        /// <summary>
        /// Remove key item from preferences if applicable
        /// </summary>
        public static void RemoveInt(string key) => Remove(key, Instance != null ? Instance.intPreferences : null);

        /// <summary>
        /// Remove key item from preferences if applicable
        /// </summary>
        public static void RemoveString(string key) => Remove(key, Instance != null ? Instance.stringPreferences : null);

        #endregion

        private static void Set<T>(string key, T item, SerializableDictionary<string, T> target, bool forceSave = true)
        {
            if (target == null)
            {
                return;
            }

            if (target.ContainsKey(key))
            {
                target[key] = item;
            }
            else
            {
                target.Add(key, item);
            }

            if (forceSave)
            {
                EditorUtility.SetDirty(Instance);
                AssetDatabase.SaveAssets();
            }
        }

        private static T Get<T>(string key, T defaultVal, SerializableDictionary<string, T> target)
        {
            if (target == null)
            {
                return default(T);
            }

            if (target.ContainsKey(key))
            {
                return target[key];
            }
            else
            {
                Set(key, defaultVal, target);
                return defaultVal;
            }
        }

        private static bool Remove<T>(string key, SerializableDictionary<string, T> target)
        {
            if (target != null)
            {
                return target.Remove(key);
            }

            return false;
        }
    }
}
