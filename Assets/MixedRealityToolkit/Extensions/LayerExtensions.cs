// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

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
    }
}