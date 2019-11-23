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
        public uint VersionNumber;
        public uint MaxNumberOfCameras;
        public IntPtr ISpatialCoordinateSystemPtr; // Windows::Perception::Spatial::ISpatialCoordinateSystem
        public IntPtr IHolographicFramePtr; // Windows::Graphics::Holographic::IHolographicFrame 
        public IntPtr IHolographicCameraPtr; // Windows::Graphics::Holographic::IHolographicCamera
    }
}
