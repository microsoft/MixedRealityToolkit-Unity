// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utility to save preferences that should be saved per project (i.e to source control) across MRTK. Effectively wraps a dictionary bag of objects to support all type access
    /// </summary>
    public class ProjectPreferences : ScriptableObject
    {
        private Dictionary<string, System.Object> DataBag = new Dictionary<string, System.Object>();

        private const string FILE_NAME = "ProjectPreferences.asset";
        private const string RELATIVE_FOLDER_PATH = "System/";
        private const MixedRealityToolkitModuleType MODULE_PATH = MixedRealityToolkitModuleType.Generated;
        private static ProjectPreferences _instance;
        private static ProjectPreferences Instance
        {
            get
            {
                if (_instance == null)
                {
                    string filePath = MixedRealityToolkitFiles.MapRelativeFilePath(MODULE_PATH, FILE_NAME);
                    if (string.IsNullOrEmpty(filePath))
                    {
                        // MapRelativeFilePath returned null, need to build ourselves
                        filePath = MixedRealityToolkitFiles.MapModulePath(MODULE_PATH) + "/" + FILE_NAME;

                        _instance = ScriptableObject.CreateInstance<ProjectPreferences>();
                        AssetDatabase.CreateAsset(_instance, filePath);
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        Debug.Log(filePath);
                        _instance = (ProjectPreferences)AssetDatabase.LoadAssetAtPath(filePath, typeof(ProjectPreferences));
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Save item of type T to preferences ScriptableObject with key given. Save all assets after execution
        /// </summary>
        /// <typeparam name="T">Type of object being saved</typeparam>
        /// <param name="key">Key to save object under in dictionary bag of preferences</param>
        /// <param name="item">Item object value to store</param>
        public static void Set<T>(string key, T item)
        {
            if (Instance.DataBag.ContainsKey(key))
            {
                Instance.DataBag[key] = item;
            }
            else
            {
                Instance.DataBag.Add(key, item);
            }

            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Gets the object stored with given key. If no item with key is found, create new entry with default value parameter
        /// </summary>
        /// <typeparam name="T">Type of object to get from preference store</typeparam>
        /// <param name="key">Key to use in search of dictionary bag of preferences</param>
        /// <param name="defaultVal">If no entry found, create new entry with this value</param>
        /// <returns>Returns object stored in dictionary bag of preferences at entry with given key</returns>
        public static T Get<T>(string key, T defaultVal)
        {
            if (Instance.DataBag.ContainsKey(key))
            {
                return (T)Instance.DataBag[key];
            }
            else
            {
                Set<T>(key, defaultVal);
                return defaultVal;
            }
        }
    }
}
