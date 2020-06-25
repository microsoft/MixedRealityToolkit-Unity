// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Physics
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
}