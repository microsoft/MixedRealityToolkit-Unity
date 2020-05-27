// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// An implementation of <see cref="IWindowsMixedRealityUtilitiesProvider"/> for Unity's in-box XR pipeline.
    /// </summary>
    public class WindowsMixedRealityUtilitiesProvider : IWindowsMixedRealityUtilitiesProvider
    {
        /// <inheritdoc />
        IntPtr IWindowsMixedRealityUtilitiesProvider.ISpatialCoordinateSystemPtr =>
#if UNITY_WSA
            WorldManager.GetNativeISpatialCoordinateSystemPtr();
#else
            IntPtr.Zero;
#endif

        /// <inheritdoc />
        IntPtr IWindowsMixedRealityUtilitiesProvider.IHolographicFramePtr
        {
            get
            {
                IntPtr nativePtr = UnityEngine.XR.XRDevice.GetNativePtr();
                HolographicFrameNativeData hfd = Marshal.PtrToStructure<HolographicFrameNativeData>(nativePtr);
                return hfd.IHolographicFramePtr;
            }
        }
    }
}
