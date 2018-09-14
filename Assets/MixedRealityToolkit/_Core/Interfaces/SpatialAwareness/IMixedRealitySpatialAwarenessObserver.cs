// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessObserver : IMixedRealityComponent // todo , IMixedRealityEventSystem, IMixedRealityEventSource
    {
        /// <summary>
        /// Is the observer running (actively accumulating spatial data)?
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// The descriptions of the observed spatial meshes.
        /// </summary>
        Dictionary<uint, IMixedRealitySpatialAwarenessMeshDescription> MeshDescriptions { get; }

        /// <summary>
        /// The descriptions of the observed planar surfaces.
        /// </summary>
        Dictionary<uint, IMixedRealitySpatialAwarenessPlanarSurfaceDescription> SurfaceDescriptions { get; }

        /// <summary>
        /// Start the observer.
        /// </summary>
        void StartObserving();

        /// <summary>
        /// Stop the observer.
        /// </summary>
        void StopObserving();
    }
}
