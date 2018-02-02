// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Common.Extensions
{
    public static class LayerExtensions
    {
        private const int InvalidLayer = -1;

        #region Local layers
        private static int defaultLayer = InvalidLayer;
        private static int surfaceLayer = InvalidLayer;
        private static int interactionLayer = InvalidLayer;
        private static int activationLayer = InvalidLayer;
        #endregion

        public static int Default
        {
            get
            {
                return GetLayerNumber(ref defaultLayer, "Default");
            }
        }

        public static int Surface
        {
            get
            {
                return GetLayerNumber(ref surfaceLayer, "SR");
            }
        }
        public static int Interaction
        {
            get
            {
                return GetLayerNumber(ref interactionLayer, "Interaction");
            }
        }

        public static int Activation
        {
            get
            {
                return GetLayerNumber(ref activationLayer, "Activation");
            }
        }

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

        public static LayerMask ToMask(int layer)
        {
            return 1 << layer;
        }

        private static int GetLayerNumber(ref int cache, string layerName)
        {
            if (cache == InvalidLayer)
            {
                cache = LayerMask.NameToLayer(layerName);
            }
            return cache;
        }
    }
}
