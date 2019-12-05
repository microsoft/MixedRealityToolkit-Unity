// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.WSA;
#if WINDOWS_UWP
using Windows.Perception.Spatial;
#if DOTNETWINRT_PRESENT
using Microsoft.Windows.Graphics.Holographic;
#else
using Windows.Graphics.Holographic;
#endif
#elif DOTNETWINRT_PRESENT
using Microsoft.Windows.Graphics.Holographic;
using Microsoft.Windows.Perception.Spatial;
#endif
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    public static class WindowsMixedRealityUtilities
    {
#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
#if ENABLE_DOTNET
        [DllImport("DotNetNativeWorkaround.dll", EntryPoint = "MarshalIInspectable")]
        private static extern void GetSpatialCoordinateSystem(IntPtr nativePtr, out SpatialCoordinateSystem coordinateSystem);

        /// <summary>
        /// Helps marshal WinRT IInspectable objects that have been passed to managed code as an IntPtr.
        /// </summary>
        /// <remarks>
        /// On .NET Native, IInspectable pointers cannot be marshaled from native to managed code using Marshal.GetObjectForIUnknown.
        /// This class calls into a native method that specifically marshals the type as a specific WinRT interface, which
        /// is supported by the marshaller on both .NET Core and .NET Native.
        /// Please see https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSystem/HandTracking.html#net-native for more info.
        /// </remarks>
        private static SpatialCoordinateSystem GetSpatialCoordinateSystem(IntPtr nativePtr)
        {
            try
            {
                GetSpatialCoordinateSystem(nativePtr, out SpatialCoordinateSystem coordinateSystem);
                return coordinateSystem;
            }
            catch
            {
                UnityEngine.Debug.LogError("Call to the DotNetNativeWorkaround plug-in failed. The plug-in is required for correct behavior when using .NET Native compilation");
                return Marshal.GetObjectForIUnknown(nativePtr) as SpatialCoordinateSystem;
            }
        }
#endif //ENABLE_DOTNET

        /// <summary>
        /// Access the underlying native spatial coordinate system.
        /// </summary>
        /// <remarks>
        /// Changing the state of the native objects received via this API may cause unpredictable
        /// behaviour and rendering artifacts, especially if Unity also reasons about that same state.
        /// </remarks>
        public static SpatialCoordinateSystem SpatialCoordinateSystem
        {
            get
            {
#if ENABLE_DOTNET
                return spatialCoordinateSystem ?? (spatialCoordinateSystem = GetSpatialCoordinateSystem(WorldManager.GetNativeISpatialCoordinateSystemPtr()));
#elif WINDOWS_UWP
                return spatialCoordinateSystem ?? (spatialCoordinateSystem = Marshal.GetObjectForIUnknown(WorldManager.GetNativeISpatialCoordinateSystemPtr()) as SpatialCoordinateSystem);
#elif DOTNETWINRT_PRESENT
                var spatialCoordinateSystemPtr = WorldManager.GetNativeISpatialCoordinateSystemPtr();
                if (spatialCoordinateSystem == null && spatialCoordinateSystemPtr != IntPtr.Zero)
                {
                    spatialCoordinateSystem = SpatialCoordinateSystem.FromNativePtr(WorldManager.GetNativeISpatialCoordinateSystemPtr());
                }
                return spatialCoordinateSystem;
#endif
            }
        }

        /// <summary>
        /// Access the underlying native current holographic frame.
        /// </summary>
        /// <remarks>
        /// Changing the state of the native objects received via this API may cause unpredictable
        /// behavior and rendering artifacts, especially if Unity also reasons about that same state.
        /// </remarks>
        public static HolographicFrame CurrentHolographicFrame
        {
            get
            {
#if DOTNETWINRT_PRESENT
                IntPtr nativePtr = UnityEngine.XR.XRDevice.GetNativePtr();
                HolographicFrameNativeData hfd = Marshal.PtrToStructure<HolographicFrameNativeData>(nativePtr);
                return HolographicFrame.FromNativePtr(hfd.IHolographicFramePtr);
#elif WINDOWS_UWP
                IntPtr nativePtr = UnityEngine.XR.XRDevice.GetNativePtr();
                HolographicFrameNativeData hfd = Marshal.PtrToStructure<HolographicFrameNativeData>(nativePtr);
                return Marshal.GetObjectForIUnknown(hfd.IHolographicFramePtr) as HolographicFrame;
#else
                return null;
#endif
            }
        }

        private static SpatialCoordinateSystem spatialCoordinateSystem = null;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        public static UnityEngine.Vector3 SystemVector3ToUnity(System.Numerics.Vector3 vector)
        {
            return new UnityEngine.Vector3(vector.X, vector.Y, -vector.Z);
        }

        public static UnityEngine.Quaternion SystemQuaternionToUnity(System.Numerics.Quaternion quaternion)
        {
            return new UnityEngine.Quaternion(-quaternion.X, -quaternion.Y, quaternion.Z, quaternion.W);
        }
    }
}
