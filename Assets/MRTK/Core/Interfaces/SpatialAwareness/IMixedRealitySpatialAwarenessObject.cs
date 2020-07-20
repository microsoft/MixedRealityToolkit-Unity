// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public interface IMixedRealitySpatialAwarenessObject
    {
        /// <summary>
        /// A unique ID identifying this spatial object.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The GameObject representing this spatial object in the scene.
        /// </summary>
        GameObject GameObject { get; set; }

        /// <summary>
        /// The MeshRenderer of this spatial object.
        /// </summary>
        MeshRenderer Renderer { get; set; }

        ///<summary>
        /// Cleans up this spatial object.
        /// </summary>
        void CleanObject();
    }
}