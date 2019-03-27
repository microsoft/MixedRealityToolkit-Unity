// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection
{
    /// <summary>
    /// Delegate called when markers have been detected by an <see cref="IMarkerDetector"/>
    /// </summary>
    /// <param name="markers">Dictionary of <see cref="Marker"/>s that have been detected</param>
    public delegate void MarkersUpdatedHandler(Dictionary<int, Marker> markers);

    /// <summary>
    /// Interface that should be implemented by any class that detects markers
    /// </summary>
    public interface IMarkerDetector
    {
        /// <summary>
        /// Event called when any observed markers have been updated
        /// </summary>
        event MarkersUpdatedHandler MarkersUpdated;

        /// <summary>
        /// Starts marker detection
        /// </summary>
        void StartDetecting();

        /// <summary>
        /// Stops marker detection
        /// </summary>
        void StopDetecting();

        /// <summary>
        /// Set the physical size for markers being detected
        /// </summary>
        /// <param name="size">The physical size (in meters) of markers being detected</param>
        void SetMarkerSize(float size);
    }
}
