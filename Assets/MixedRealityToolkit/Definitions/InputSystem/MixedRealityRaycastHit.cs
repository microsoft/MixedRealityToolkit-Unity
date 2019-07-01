// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

            try
            {
                lightmapCoord = hitInfo.lightmapCoord;
            }
            catch (Exception)
            {
                // Accessing lightmap coord appears to throw a NullReferenceException in some cases, probably when lightmaps are not used.
                // Catch this, and just leave as default value.
                lightmapCoord = Vector2.zero;
            }
        }
    }
}
