// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public interface IMixedRealitySpatialAwarenessSurfacePlaneObserver: IMixedRealitySpatialAwarenessObserver
    {
        SpatialAwarenessSurfaceTypes SurfaceTypes { get; set; }

        int PhysicsLayer { get; set; }
    }
}
