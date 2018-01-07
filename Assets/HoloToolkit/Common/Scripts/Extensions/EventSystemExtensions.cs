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
        private static readonly List<ComparableRaycastResult> ComparableRaycastResults = new List<ComparableRaycastResult>();
        private static readonly RaycastResultComparer RaycastResultComparer = new RaycastResultComparer();

        /// <summary>
        /// Executes a raycast all and returns the closest element. Fixes the current issue with Unity's raycast sorting which does not 
        /// consider separate canvases.
        /// </summary>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        public static RaycastResult Raycast(this EventSystem eventSystem, PointerEventData pointerEventData, LayerMask[] layerMasks)
        {
            RaycastResults.Clear();
            eventSystem.RaycastAll(pointerEventData, RaycastResults);
            return PrioritizeRaycastResult(layerMasks);
        }

        private static RaycastResult PrioritizeRaycastResult(LayerMask[] priority)
        {
            ComparableRaycastResults.Clear();
            for (var i = 0; i < RaycastResults.Count; i++)
            {
                if (RaycastResults[i].gameObject == null) { continue; }

                var layerMaskIndex = RaycastResults[i].gameObject.layer.FindLayerListIndex(priority);
                if (layerMaskIndex == -1) { continue; }

                ComparableRaycastResults.Add(new ComparableRaycastResult(RaycastResults[i], layerMaskIndex));
            }

            ComparableRaycastResult maxResult = default(ComparableRaycastResult);
            for (int i = 0; i < ComparableRaycastResults.Count; i++)
            {
                if (RaycastResultComparer.Compare(maxResult, ComparableRaycastResults[i]) < 0)
                {
                    maxResult = ComparableRaycastResults[i];
                }
            }

            return maxResult.RaycastResult;
        }
    }
}
