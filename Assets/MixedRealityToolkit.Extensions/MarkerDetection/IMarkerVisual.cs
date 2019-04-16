// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// Set the physical size for displaying markers
        /// </summary>
        /// <param name="size">The physical size (in meters) that markers should be when shown</param>
        void SetMarkerSize(float size);
    }
}
