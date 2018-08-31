// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem
{
    /// <summary>
    /// Class poviding the default implementation of the <see cref="IMixedRealitySpatialAwarenessPlanarSurfaceDescription"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessPlanarSurfaceDescription : MixedRealitySpatialAwarenessBaseDescription, IMixedRealitySpatialAwarenessPlanarSurfaceDescription
    {
        /// <inheritdoc />
        public Bounds BoundingBox
        { get; private set; }

        /// <inheritdoc />
        public Vector3 Normal
        { get; private set; }

        /// <inheritdoc />
        public MixedRealitySpatialAwarenessSurfaceTypes SurfaceType
        { get; private set; }

        public MixedRealitySpatialAwarenessPlanarSurfaceDescription(
            Vector3 position,
            Bounds boundingBox,
            Vector3 normal,
            MixedRealitySpatialAwarenessSurfaceTypes surfaceType) : base(position)
        {
            BoundingBox = boundingBox;
            Normal = normal;
            SurfaceType = surfaceType;
        }
    }
}