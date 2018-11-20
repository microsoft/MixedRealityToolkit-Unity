// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers
{
    /// <summary>
    /// The interface contract for Mixed Reality spatial observers.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessObserver : IMixedRealityDataProvider
    {
        /// <summary>
        /// Is the observer running (actively accumulating spatial data)?
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Start the observer.
        /// </summary>
        void StartObserving();

        /// <summary>
        /// Stop the observer.
        /// </summary>
        void StopObserving();

        /// <summary>
        /// The collection of mesh <see cref="GameObject"/>s that have been observed.
        /// </summary>
        IReadOnlyDictionary<int, SpatialMeshObject> Meshes { get; }
    }
}
