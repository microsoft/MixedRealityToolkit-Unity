// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.Common.Extensions
{
    public static class EventSystemExtensions
    {
        private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();
        private static readonly RaycastResultComparer RaycastResultComparer = new RaycastResultComparer();

        /// <summary>
        /// Executes a raycast all and returns the closest element. Fixes the current issue with Unity's raycast sorting which does not 
        /// consider separate canvases.
        /// </summary>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        public static RaycastResult Raycast(this EventSystem eventSystem, PointerEventData pointerEventData, LayerMask[] layerMasks)
        {
            eventSystem.RaycastAll(pointerEventData, RaycastResults);
            return PrioritizeRaycastResult(layerMasks);
        }

        private static RaycastResult PrioritizeRaycastResult(LayerMask[] priority)
        {
            ComparableRaycastResult maxResult = default(ComparableRaycastResult);

            for (var i = 0; i < RaycastResults.Count; i++)
            {
                if (RaycastResults[i].gameObject == null) { continue; }

                var layerMaskIndex = RaycastResults[i].gameObject.layer.FindLayerListIndex(priority);
                if (layerMaskIndex == -1) { continue; }

                var result = new ComparableRaycastResult(RaycastResults[i], layerMaskIndex);

                if (maxResult.RaycastResult.module == null || RaycastResultComparer.Compare(maxResult, result) < 0)
                {
                    maxResult = result;
                }
            }

            return maxResult.RaycastResult;
        }
    }
}
