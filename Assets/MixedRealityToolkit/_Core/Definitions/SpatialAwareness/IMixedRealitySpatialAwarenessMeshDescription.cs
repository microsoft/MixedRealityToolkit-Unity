// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Describes the data required for an application to understand how to construct and place a mesh in the environment.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessMeshDescription : IMixedRealitySpatialAwarenessBaseDescription
    {
        /// <summary>
        /// The <see cref="Mesh"/> being described.
        /// </summary>
        Mesh Mesh { get; }
    }
}
