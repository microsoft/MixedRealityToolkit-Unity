// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Defines a set of IntPtr properties that are used by the static <see cref="WindowsMixedRealityUtilities"/>
    /// to provide access to specific underlying native objects relevant to Windows Mixed Reality.
    /// </summary>
    /// <remarks>
    /// <para>This is intended to be used to support both XR SDK and Unity's legacy XR pipeline, which provide
    /// different APIs to access these native objects.</para>
    /// </remarks>
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
