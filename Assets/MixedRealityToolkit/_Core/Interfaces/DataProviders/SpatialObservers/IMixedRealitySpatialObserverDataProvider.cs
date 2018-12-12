// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers
{
    /// <summary>
    /// The Mixed Reality Spatial Observer Data Provider contract.
    /// </summary>
    public interface IMixedRealitySpatialObserverDataProvider : IMixedRealityDataProvider, IMixedRealityEventSource
    {
        /// <summary>
        /// The startup behavior of the hardware resource.
        /// </summary>
        AutoStartBehavior StartupBehavior { get; }

        /// <summary>
        /// Gets or sets the frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        float UpdateInterval { get; set; }

        /// <summary>
        /// The global physics layer to use when creating spatial objects for this observer.
        /// </summary>
        /// <remarks>
        /// Some implementations may override this.
        /// </remarks>
        int PhysicsLayer { get; }

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
    }
}
