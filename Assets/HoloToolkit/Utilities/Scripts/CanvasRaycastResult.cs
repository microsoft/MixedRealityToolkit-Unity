// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity
{
    public struct CanvasRaycastResult : IComparable<CanvasRaycastResult>
    {
        public static CanvasRaycastResult Empty = new CanvasRaycastResult(new RaycastResult());

        public readonly RaycastResult RaycastResult;
        public readonly Canvas Canvas;

        public CanvasRaycastResult(RaycastResult raycastResult)
        {
            RaycastResult = raycastResult;
            Canvas = raycastResult.gameObject ? raycastResult.gameObject.GetComponentInParent<Canvas>() : null;
        }

        public int CompareTo(CanvasRaycastResult other)
        {
            var result = CompareRaycastsByCanvasDepth(this, other);
            if (result != 0) { return result; }
            return CompareRaycastsByDistance(this, other);
        }

        private static int CompareRaycastsByCanvasDepth(CanvasRaycastResult left, CanvasRaycastResult right)
        {
            if (left.Canvas != null && right.Canvas != null && left.Canvas.rootCanvas == right.Canvas.rootCanvas)
            {
                return right.RaycastResult.depth.CompareTo(left.RaycastResult.depth);
            }
            return 0;
        }

        private static int CompareRaycastsByDistance(CanvasRaycastResult left, CanvasRaycastResult right)
        {
            return left.RaycastResult.distance.CompareTo(right.RaycastResult.distance);
        }
    }
}
