// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public static class EditorLayerExtensions
    {
        private static SerializedProperty tagManagerLayers = null;

        /// <summary>
        /// The current layers defined in the Tag Manager.
        /// </summary>
        public static UnityEditor.SerializedProperty TagManagerLayers
        {
            get
            {
                if (tagManagerLayers == null)
                {
                    InitializeTagManager();
                }

                return tagManagerLayers;
            }
        }

        private static void InitializeTagManager()
        {
            Object[] tagAssets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if ((tagAssets == null) || (tagAssets.Length == 0))
            {
                Debug.LogError("Failed to load TagManager!");
                return;
            }

            var tagsManager = new UnityEditor.SerializedObject(tagAssets);
            tagManagerLayers = tagsManager.FindProperty("layers");

            Debug.Assert(tagManagerLayers != null);
        }

        /// <summary>
        /// Attempts to set the layer in Project Settings Tag Manager.
        /// </summary>
        /// <param name="layerId">The layer Id to attempt to set the layer on.</param>
        /// <param name="layerName">The layer name to attempt to set the layer on.</param>
        /// <returns>
        /// True if the specified layerId was newly configured, false otherwise.
        /// </returns>
        public static bool SetupLayer(int layerId, string layerName)
        {
            SerializedProperty layer = TagManagerLayers.GetArrayElementAtIndex(layerId);

            if (!string.IsNullOrEmpty(layer.stringValue))
            {
                // layer already set.
                return false;
            }

            layer.stringValue = layerName;
            layer.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            return true;
        }

        /// <summary>
        /// Attempts to remove the layer from the Project Settings Tag Manager.
        /// </summary>
        public static void RemoveLayer(string layerName)
        {
            for (int i = 0; i < TagManagerLayers.arraySize; i++)
            {
                var layer = TagManagerLayers.GetArrayElementAtIndex(i);

                if (layer.stringValue == layerName)
                {
                    layer.stringValue = string.Empty;
                    layer.serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    break;
                }
            }
        }
    }
}