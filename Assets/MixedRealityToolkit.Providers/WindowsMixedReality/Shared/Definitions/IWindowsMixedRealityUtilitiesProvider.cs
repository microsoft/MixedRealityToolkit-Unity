// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    public interface IWindowsMixedRealityUtilitiesProvider
    {
        /// <summary>
        /// The current native root <see href="https://docs.microsoft.com/uwp/api/windows.perception.spatial.spatialcoordinatesystem">ISpatialCoordinateSystem</see>.
        /// </summary>
        IntPtr ISpatialCoordinateSystemPtr { get; }

        /// <summary>
        /// The current native <see href="https://docs.microsoft.com/uwp/api/Windows.Graphics.Holographic.HolographicFrame">IHolographicFrame</see>.
        /// </summary>
        IntPtr IHolographicFramePtr { get; }
    }
}
