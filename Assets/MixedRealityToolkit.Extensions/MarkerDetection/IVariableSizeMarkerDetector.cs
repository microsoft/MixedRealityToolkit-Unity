// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection
{
    public interface IVariableSizeMarkerDetector : IMarkerDetector
    {
        /// <summary>
        /// Tries to obtain the physical size of a detected marker.
        /// </summary>
        /// <param name="markerId">id of detected marker</param>
        /// <param name="size">The physical size (in meters) of the detected marker</param>
        /// <returns>True if the size was located</returns>
        bool TryGetMarkerSize(int markerId, out float size);
    }
}
