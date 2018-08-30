// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Describes the data required for an application to understand how to construct and place a planar surface in the environment.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessPlanarSurfaceDescription : IMixedRealitySpatialAwarenessBaseDescription
    {
        /// <summary>
        /// The axis aligned bounding box that contains the surface being described.
        /// </summary>
        Bounds BoundingBox { get; }

        /// <summary>
        /// The normal of the described surface.
        /// </summary>
        Vector3 Normal { get; }

        /// <summary>
        /// The semantic (ex: Floor) associated with the surface.
        /// </summary>
        MixedRealitySpatialAwarenessSurfaceTypes SurfaceType { get; }
    }
}
