// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public partial class BaseSpatialAwarenessObject : IMixedRealitySpatialAwarenessObject
    {
        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public GameObject GameObject { get; set; }

        /// <inheritdoc />
        public MeshRenderer Renderer { get; set; }

        /// <summary>
        /// The MeshFilter associated with this spatial object's renderer.
        /// </summary>
        public MeshFilter Filter { get; set; }

        /// <inheritdoc />
        public virtual void CleanObject()
        {
            // todo: consider if this should be virtual, and what params it should contain
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BaseSpatialAwarenessObject()
        {
        }
    }
}
