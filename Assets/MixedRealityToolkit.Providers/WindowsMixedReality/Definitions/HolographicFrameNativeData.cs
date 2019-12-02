// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// A representation of Windows Mixed Reality native data, provided as an IntPtr from Unity's UnityEngine.XR.XRDevice.GetNativePtr().
    /// </summary>
    /// <remarks>See <see href="https://docs.microsoft.com/en-us/windows/mixed-reality/unity-xrdevice-advanced"/> for more info.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct HolographicFrameNativeData
    {
        /// <summary>
        /// The version number of this native data.
        /// </summary>
        public uint VersionNumber;

        /// <summary>
        /// The number of cameras present in the IHolographicCameraPtr array.
        /// </summary>
        public uint MaxNumberOfCameras;

        /// <summary>
        /// The current native root <see href="https://docs.microsoft.com/uwp/api/windows.perception.spatial.spatialcoordinatesystem">ISpatialCoordinateSystem</see>).
        /// </summary>
        public IntPtr ISpatialCoordinateSystemPtr;
        
        /// <summary>
        /// The current native <see href="https://docs.microsoft.com/uwp/api/Windows.Graphics.Holographic.HolographicFrame">IHolographicFrame</see>).
        /// </summary>
        public IntPtr IHolographicFramePtr;

        /// <summary>
        /// An array of IntPtr (to <see href="https://docs.microsoft.com/uwp/api/Windows.Graphics.Holographic.HolographicCamera">IHolographicCamera</see>) marshaled as UnmanagedType.ByValArray with a length equal to MaxNumberOfCameras.
        /// </summary>
        public IntPtr IHolographicCameraPtr;
    }
}
