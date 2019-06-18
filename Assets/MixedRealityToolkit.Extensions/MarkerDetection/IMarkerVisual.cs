// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection
{
    /// <summary>
    /// Interface that should be implemented by any class that displays markers for detection
    /// </summary>
    public interface IMarkerVisual
    {
        /// <summary>
        /// Shows the specified marker
        /// </summary>
        /// <param name="id">Id of the marker to show</param>
        void ShowMarker(int id);

        /// <summary>
        /// Hides any shown markers
        /// </summary>
        void HideMarker();

        /// <summary>
        /// Tries to set the physical size for displaying markers
        /// </summary>
        /// <param name="size">The physical size (in meters) that markers should be when shown</param>
        /// <returns>Returns true if the marker can be set to the provided size, otherwise false.</returns>
        bool TrySetMarkerSize(float size);

        /// <summary>
        /// Tries to obtain the maximum marker id supported by this marker visual.
        /// </summary>
        /// <param name="supportedIds">Output maximum marker id</param>
        /// <returns>Returns true if the maximum marker id can be found, otherwise false.</returns>
        bool TryGetMaxSupportedMarkerId(out int markerId);
    }
}
