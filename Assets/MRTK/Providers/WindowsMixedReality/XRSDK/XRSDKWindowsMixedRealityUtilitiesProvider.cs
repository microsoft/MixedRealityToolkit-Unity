// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using System;

#if WMR_ENABLED
using UnityEngine.XR.WindowsMR;
#endif // WMR_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// An implementation of <see cref="Toolkit.WindowsMixedReality.IWindowsMixedRealityUtilitiesProvider"/> for Unity's XR SDK pipeline.
    /// </summary>
    public class XRSDKWindowsMixedRealityUtilitiesProvider : IWindowsMixedRealityUtilitiesProvider
    {
        /// <inheritdoc />
        IntPtr IWindowsMixedRealityUtilitiesProvider.ISpatialCoordinateSystemPtr =>
#if WMR_ENABLED
            WindowsMREnvironment.OriginSpatialCoordinateSystem;
#else
            IntPtr.Zero;
#endif

        /// <summary>
        /// Currently unable to access HolographicFrame in XR SDK. Always returns IntPtr.Zero.
        /// </summary>
        IntPtr IWindowsMixedRealityUtilitiesProvider.IHolographicFramePtr => IntPtr.Zero;
    }
}
