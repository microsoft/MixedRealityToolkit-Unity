// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessBaseDescription : IEqualityComparer
    {
        /// <summary>
        /// The position, in the environment, at which the described object should be placed.
        /// </summary>
        Vector3 Position { get; }
    }
}
