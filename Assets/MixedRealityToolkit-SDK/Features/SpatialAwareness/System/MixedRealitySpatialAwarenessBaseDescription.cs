// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem
{
    /// <summary>
    /// Class poviding the default implementation of the <see cref="IMixedRealitySpatialAwarenessBaseDescription"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessBaseDescription : IMixedRealitySpatialAwarenessBaseDescription
    {
        /// <inheritdoc />
        public Vector3 Position
        { get; private set; }

        public MixedRealitySpatialAwarenessBaseDescription(Vector3 position)
        {
            // todo
        }
    }
}
