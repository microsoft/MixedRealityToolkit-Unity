// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity
{
    public class RaycastResultComparer : IComparer<RaycastResult>
    {
        private static readonly List<Func<RaycastResult, RaycastResult, int>> Comparers = new List<Func<RaycastResult, RaycastResult, int>>
        {
            CompareRaycastsBySortingLayer,
            CompareRaycastsBySortingOrder,
            CompareRaycastsByCanvasDepth,
            CompareRaycastsByDistance,
        };

        public int Compare(RaycastResult left, RaycastResult right)
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

        private static int CompareRaycastsBySortingOrder(RaycastResult left, RaycastResult right)
        {
            //Higher is better
            return right.sortingOrder.CompareTo(left.sortingOrder);
        }

        private static int CompareRaycastsBySortingLayer(RaycastResult left, RaycastResult right)
        {
            //Higher is better
            return right.sortingLayer.CompareTo(left.sortingLayer);
        }

        private static int CompareRaycastsByCanvasDepth(RaycastResult left, RaycastResult right)
        {
            //Module is the graphic raycaster on the canvases.
            if (left.module.transform.IsParentOrChildOf(right.module.transform))
            {
                //Higher is better
                return right.depth.CompareTo(left.depth);
            }
            return 0;
        }

        private static int CompareRaycastsByDistance(RaycastResult left, RaycastResult right)
        {
            //Lower is better
            return left.distance.CompareTo(right.distance);
        }
    }
}
