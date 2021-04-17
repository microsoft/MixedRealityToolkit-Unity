// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// The interface for defining an on-demand spatial observer which enables on demand updating
    /// </summary>
    public interface IMixedRealityOnDemandObserver : IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Whether the observer updates its observations automatically on interval.
        /// </summary>
        /// <remarks>
        /// When false, call <see cref="UpdateOnDemand()"/> to manually update an observer when needed.
        /// </remarks>
        bool AutoUpdate { get; set; }

        /// <summary>
        /// Whether the observer updates once after initialization (regardless whether <see cref="AutoUpdate"/> is true).
        /// </summary>
        bool UpdateOnceInitialized { get; set; }

        /// <summary>
        /// Delay in seconds before the observer starts to update automatically for the first time after initialization
        /// </summary>
        /// <remarks>
        /// Only applies when <see cref="AutoUpdate"/> is set to true.
        /// </remarks>
        float FirstAutoUpdateDelay { get; set; }

        /// <summary>
        /// Tells the observer to update the observations.
        /// </summary>
        /// <remarks>
        /// Regardless of <see cref="AutoUpdate"/>, calling this method will force the observer to update.
        /// </remarks>
        void UpdateOnDemand();
    }
}
