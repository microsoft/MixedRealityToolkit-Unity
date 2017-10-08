// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class LayerExtensions
    {
        /// <summary>
        /// Look through the layerMaskList and find the index in that list for which the supplied layer is part of
        /// </summary>
        /// <param name="layer">Layer to search for</param>
        /// <param name="layerMaskList">List of LayerMasks to search</param>
        /// <returns>LayerMaskList index, or -1 for not found</returns>
        public static int FindLayerListIndex(this int layer, LayerMask[] layerMaskList)
        {
            for (int i = 0; i < layerMaskList.Length; i++)
            {
                if (IsInLayerMask(layer, layerMaskList[i].value))
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool IsInLayerMask(this int layer, int layerMask)
        {
            return ((1 << layer) & layerMask) != 0;
        }

        public static int Combine(this IEnumerable<LayerMask> layerMaskList)
        {
            int combinedLayerMask = 0;
            foreach (var layer in layerMaskList)
            {
                combinedLayerMask = combinedLayerMask | layer.value;
            }
            return combinedLayerMask;
        }
    }
}
