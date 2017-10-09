// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity
{
    public static class EventSystemExtensions
    {
        private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();
        private static readonly RaycastResultComparer RaycastResultComparer = new RaycastResultComparer();

        /// <summary>
        /// Executes a raycast all and returns the closest element. Fixes the current issue with Unitys raycast sorting which does not 
        /// consider separate canvases.
        /// </summary>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        public static RaycastResult Raycast(this EventSystem eventSystem, PointerEventData pointerEventData, IEnumerable<LayerMask> layerMasks)
        {
            RaycastResults.Clear();
            eventSystem.RaycastAll(pointerEventData, RaycastResults);
            return FindClosestRaycastHitInLayerMasks(layerMasks);
        }

        /// <summary>
        /// Find the closest raycast hit in the list of RaycastResults that is also included in the LayerMask list.  
        /// </summary>
        /// <param name="layerMaskList">List of layers to support</param>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        private static RaycastResult FindClosestRaycastHitInLayerMasks(IEnumerable<LayerMask> layerMaskList)
        {
            int combinedLayerMask = layerMaskList.Combine();

            for (var i = RaycastResults.Count - 1; i >= 0; i--)
            {
                var candidate = RaycastResults[i];
                if (candidate.gameObject && candidate.gameObject.layer.IsInLayerMask(combinedLayerMask)) continue;
                RaycastResults.Remove(candidate);
            }
            ;
            return RaycastResults.Count > 0 ? GetClosestRaycast() : default(RaycastResult);
        }

        private static RaycastResult GetClosestRaycast()
        {
            var max = RaycastResults[0];

            for (var i = 1; i < RaycastResults.Count; i++)
            {
                if (RaycastResultComparer.Compare(max, RaycastResults[i]) > 0)
                {
                    max = RaycastResults[i];
                }
            }

            return max;
        }

    }
}
