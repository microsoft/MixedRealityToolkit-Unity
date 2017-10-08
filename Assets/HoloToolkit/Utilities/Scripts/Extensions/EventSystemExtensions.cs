// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity
{
    public static class EventSystemExtensions
    {
        private static readonly List<RaycastResult> RaycastResultList = new List<RaycastResult>();
        private static readonly List<CanvasRaycastResult> CanvasRaycastResultList = new List<CanvasRaycastResult>();

        /// <summary>
        /// Executes a raycast all and returns the closest element. Fixes the current issue with Unitys raycast sorting which does not 
        /// consider separate canvases.
        /// </summary>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        public static RaycastResult Raycast(this EventSystem eventSystem, PointerEventData pointerEventData, IEnumerable<LayerMask> layerMasks)
        {
            RaycastResultList.Clear();
            eventSystem.RaycastAll(pointerEventData, RaycastResultList);
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

            CanvasRaycastResultList.Clear();
            for (var i = 0; i < RaycastResultList.Count; i++)
            {
                var candidate = RaycastResultList[i];
                if (!candidate.gameObject) { continue; }
                if (!candidate.gameObject.layer.IsInLayerMask(combinedLayerMask)) { continue; }
                CanvasRaycastResultList.Add(new CanvasRaycastResult(candidate));
            }
            CanvasRaycastResultList.Sort();
            return CanvasRaycastResultList.Count > 0 ? CanvasRaycastResultList[0].RaycastResult : default(RaycastResult);
        }
    }
}
