// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem
{
    /// <summary>
    /// Class poviding the default implementation of the <see cref="IMixedRealitySpatialAwarenessMeshDescription"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessMeshDescription : MixedRealitySpatialAwarenessBaseDescription, IMixedRealitySpatialAwarenessMeshDescription
    {
        /// <inheritdoc />
        public Mesh Mesh
        {get; private set;}

        public MixedRealitySpatialAwarenessMeshDescription(Vector3 position, Mesh mesh) : base(position)
        {
            Mesh = mesh;
        }
    }
}
