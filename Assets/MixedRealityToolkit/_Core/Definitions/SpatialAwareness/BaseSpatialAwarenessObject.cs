// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices
{

    public partial class BaseSpatialAwarenessObject : IMixedRealitySpatialAwarenessObject
    {
        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public GameObject GameObject { get; set; }

        /// <inheritdoc />
        public MeshRenderer Renderer { get; set; }

        /// <inheritdoc />
        public MeshFilter Filter { get; set; }

        public void CleanObject()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// constructor
        /// </summary>
        public BaseSpatialAwarenessObject()
        {
            //empty for now
        }

    }
}
