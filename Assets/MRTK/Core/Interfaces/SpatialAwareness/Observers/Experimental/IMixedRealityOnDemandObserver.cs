// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// The interface for defining an <see cref="IMixedRealityOnDemandObserver"/> which enables on demand updating
    /// </summary>
    public interface IMixedRealityOnDemandObserver : IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Gets or sets a value indicating whether the observer can autoupdate on interval.
        /// </summary>
        /// <remarks>
        /// When false, calling UpdateOnDemand() is the ONLY way to update an observer.
        /// </remarks>
        bool AutoUpdate { get; set; }

        /// <summary>
        /// Observer will update once after initialization then require manual update thereafter. Uses <see cref="FirstUpdateDelay"/> to determine when.
        /// </summary>
        bool UpdateOnceOnLoad { get; set; }

        /// <summary>
        /// Delay in seconds before the observer will update automatically, once.
        /// </summary>
        float FirstUpdateDelay { get; set; }

        /// <summary>
        /// Tells the observer to run a cycle
        /// </summary>
        /// <remarks>
        /// regardless of AutoUpdate, calling this method will force the observer to update.
        /// When AutoUpdate is false, this is the ONLY way to update an observer.
        /// </remarks>
        void UpdateOnDemand();
    }
}