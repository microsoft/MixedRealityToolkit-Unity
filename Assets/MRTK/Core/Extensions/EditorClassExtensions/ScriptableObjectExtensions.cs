// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Extensions for <see href="https://docs.unity3d.com/ScriptReference/ScriptableObject.html">ScriptableObject</see>s
    /// </summary>
    public static class ScriptableObjectExtensions
    {
        /// <summary>
        /// Creates, saves, and then opens a new asset for the target <see href="https://docs.unity3d.com/ScriptReference/ScriptableObject.html">ScriptableObject</see>.
        /// </summary>
        /// <param name="scriptableObject"><see href="https://docs.unity3d.com/ScriptReference/ScriptableObject.html">ScriptableObject</see> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="fileName">Optional filename for the new asset.</param>
        public static ScriptableObject CreateAsset(this ScriptableObject scriptableObject, string path = null, string fileName = null)
        {
            var name = string.IsNullOrEmpty(fileName) ? $"{scriptableObject.GetType().Name}" : fileName;

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }

            if (Path.GetExtension(path) != string.Empty)
            {
                var subtractedPath = path.Substring(path.LastIndexOf("/", StringComparison.Ordinal));
                path = path.Replace(subtractedPath, string.Empty);
            }

            if (!Directory.Exists(Path.GetFullPath(path)))
            {
                Directory.CreateDirectory(Path.GetFullPath(path));
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");

            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(scriptableObject);
            return scriptableObject;
        }

        /// <summary>
        /// Gets all the scriptable object instances in the project.
        /// </summary>
        /// <typeparam name="T">The Type of <see href="https://docs.unity3d.com/ScriptReference/ScriptableObject.html">ScriptableObject</see> you're wanting to find instances of.</typeparam>
        /// <returns>An Array of instances for the type.</returns>
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            // FindAssets uses tags check documentation for more info
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var instances = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                instances[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return instances;
        }

        /// <summary>
        /// Gets all the scriptable object instances in the project.
        /// </summary>
        /// <param name="assetType">The Type of <see href="https://docs.unity3d.com/ScriptReference/ScriptableObject.html">ScriptableObject</see> you're wanting to find instances of.</param>
        /// <returns>An Array of instances for the type.</returns>
        public static ScriptableObject[] GetAllInstances(Type assetType)
        {
            // FindAssets uses tags check documentation for more info
            string[] guids = AssetDatabase.FindAssets($"t:{assetType.Name}");
            var instances = new ScriptableObject[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                instances[i] = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }

            return instances;
        }
    }
}