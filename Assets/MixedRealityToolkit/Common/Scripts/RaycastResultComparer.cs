// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.Common
{
    public struct ComparableRaycastResult
    {
        public readonly int LayerMaskIndex;
        public readonly RaycastResult RaycastResult;

        public ComparableRaycastResult(RaycastResult raycastResult, int layerMaskIndex = 0)
        {
            RaycastResult = raycastResult;
            LayerMaskIndex = layerMaskIndex;
        }
    }

    public class RaycastResultComparer : IComparer<ComparableRaycastResult>
    {
        private static readonly List<Func<ComparableRaycastResult, ComparableRaycastResult, int>> Comparers = new List<Func<ComparableRaycastResult, ComparableRaycastResult, int>>
        {
            CompareRaycastsByLayerMaskPrioritization,
            CompareRaycastsBySortingLayer,
            CompareRaycastsBySortingOrder,
            CompareRaycastsByCanvasDepth,
            CompareRaycastsByDistance,
        };

        public int Compare(ComparableRaycastResult left, ComparableRaycastResult right)
        {
            for (var i = 0; i < Comparers.Count; i++)
            {
                var result = Comparers[i](left, right);
                if (result != 0)
                {
                    return result;
                }
            }
            return 0;
        }

        private static int CompareRaycastsByLayerMaskPrioritization(ComparableRaycastResult left, ComparableRaycastResult right)
        {
            //Lower is better, -1 is not relevant
            return right.LayerMaskIndex.CompareTo(left.LayerMaskIndex);
        }

        private static int CompareRaycastsBySortingLayer(ComparableRaycastResult left, ComparableRaycastResult right)
        {
            //Higher is better
            return left.RaycastResult.sortingLayer.CompareTo(right.RaycastResult.sortingLayer);
        }

        private static int CompareRaycastsBySortingOrder(ComparableRaycastResult left, ComparableRaycastResult right)
        {
            //Higher is better
            return left.RaycastResult.sortingOrder.CompareTo(right.RaycastResult.sortingOrder);
        }

        private static int CompareRaycastsByCanvasDepth(ComparableRaycastResult left, ComparableRaycastResult right)
        {
            //Module is the graphic raycaster on the canvases.
            if (left.RaycastResult.module.transform.IsParentOrChildOf(right.RaycastResult.module.transform))
            {
                //Higher is better
                return left.RaycastResult.depth.CompareTo(right.RaycastResult.depth);
            }
            return 0;
        }

        private static int CompareRaycastsByDistance(ComparableRaycastResult left, ComparableRaycastResult right)
        {
            //Lower is better
            return right.RaycastResult.distance.CompareTo(left.RaycastResult.distance);
        }
    }
}
