// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    public interface IWindowsMixedRealitySceneUnderstanding
    {
        /// <summary>
        /// Returns a texture where 1 represents a valid placement position on a Quad
        /// </summary>
        /// <param name="quad"></param>
        /// <param name="textureWidth"></param>
        /// <param name="textureHeight"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        bool TryGetPlaneValidationMask(SpatialAwarenessSceneObject.Quad quad, ushort textureWidth, ushort textureHeight, out byte[] mask);

        /// <summary>
        /// Returns the best position given the size of something for a Quad
        /// </summary>
        /// <param name="quad"></param>
        /// <param name="objExtents"></param>
        /// <param name="placementPosOnPlane"></param>
        /// <returns></returns>
        bool TryGetBestPlacementPosition(SpatialAwarenessSceneObject.Quad quad, Vector2 objExtents, out Vector2 placementPosOnPlane);
    }
}
