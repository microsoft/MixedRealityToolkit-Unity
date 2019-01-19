// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Physics
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