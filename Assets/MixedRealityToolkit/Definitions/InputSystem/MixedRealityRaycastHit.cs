// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Including lightmap coord has a performance cost so we've disabled it by default.
// Uncomment the define to enable lightmap coords.
// #define MRTK_ENABLE_LIGHTMAPCOORD_IN_RAYCASTHIT

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The resulting hit information from an IMixedRealityRaycastProvider.
    /// </summary>
    public struct MixedRealityRaycastHit
    {
        public Vector3 point;
        public Vector3 normal;
        public Vector3 barycentricCoordinate;
        public float distance;
        public int triangleIndex;
        public Vector2 textureCoord;
        public Vector2 textureCoord2;
        public Transform transform;
        public Vector2 lightmapCoord;

        public MixedRealityRaycastHit(RaycastHit hitInfo)
        {
            point = hitInfo.point;
            normal = hitInfo.normal;
            barycentricCoordinate = hitInfo.barycentricCoordinate;
            distance = hitInfo.distance;
            triangleIndex = hitInfo.triangleIndex;
            textureCoord = hitInfo.textureCoord;
            textureCoord2 = hitInfo.textureCoord2;
            transform = hitInfo.transform;
#if MRTK_ENABLE_LIGHTMAPCOORD_IN_RAYCASTHIT
            // This null check causes a performance hit but it's preferable to an exception.
            lightmapCoord = hitInfo.transform != null ? hitInfo.lightmapCoord : Vector2.zero;
#else
            lightmapCoord = Vector2.zero;
#endif
        }
    }
}
