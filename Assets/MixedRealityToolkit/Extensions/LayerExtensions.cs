// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's LayerMask struct
    /// </summary>
    public static class LayerExtensions
    {
        /// <summary>
        /// The Invalid Layer Id.
        /// </summary>
        public const int InvalidLayerId = -1;

        /// <summary>
        /// Look through the layerMaskList and find the index in that list for which the supplied layer is part of
        /// </summary>
        /// <param name="layer">Layer to search for</param>
        /// <param name="layerMasks">List of LayerMasks to search</param>
        /// <returns>LayerMaskList index, or -1 for not found</returns>
        public static int FindLayerListIndex(this int layer, LayerMask[] layerMasks)
        {
            var i = 0;
            for (int j = 0; j < layerMasks.Length; j++)
            {
                if (layer.IsInLayerMask(layerMasks[i]))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Checks whether a layer is in a layer mask
        /// </summary>
        /// <returns>True if the layer mask contains the layer</returns>
        public static bool IsInLayerMask(this int layer, int layerMask)
        {
            return ((1 << layer) & layerMask) != 0;
        }

        /// <summary>
        /// Combines provided layers into a single layer mask.
        /// </summary>
        /// <returns>The combined layer mask</returns>
        public static int Combine(this LayerMask[] layerMaskList)
        {
            int combinedLayerMask = 0;
            for (int i = 0; i < layerMaskList.Length; i++)
            {
                combinedLayerMask = combinedLayerMask | layerMaskList[i].value;
            }
            return combinedLayerMask;
        }

        /// <summary>
        /// Transform layer id to <see href="https://docs.unity3d.com/ScriptReference/LayerMask.html">LayerMask</see>
        /// </summary>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public static LayerMask ToMask(int layerId)
        {
            return 1 << layerId;
        }

        /// <summary>
        /// Gets a valid layer id using the layer name.
        /// </summary>
        /// <param name="cache">The cached layer id.</param>
        /// <param name="layerName">The name of the layer to look for if the cache is unset.</param>
        /// <returns>The layer id.</returns>
        public static int GetLayerId(ref int cache, string layerName)
        {
            if (cache == InvalidLayerId)
            {
                cache = LayerMask.NameToLayer(layerName);
            }

            return cache;
        }

#if UNITY_EDITOR

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
        public static bool  SetupLayer(int layerId, string layerName)
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
        /// <param name="layerName"></param>
        /// <returns></returns>
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

#endif // UNITY_EDITOR
    }
}