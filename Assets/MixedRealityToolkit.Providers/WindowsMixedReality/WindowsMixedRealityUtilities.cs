// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
#if NETFX_CORE
using System;
#endif // NETFX_CORE
using UnityEngine.XR.WSA;
#if WINDOWS_UWP
using System.Runtime.InteropServices;
using Windows.Perception.Spatial;
#elif DOTNETWINRT_PRESENT
using Microsoft.Windows.Perception.Spatial;
#endif
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    public static class WindowsMixedRealityUtilities
    {
#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
#if NETFX_CORE
        [DllImport("DotNetNativeWorkaround.dll", EntryPoint = "MarshalIInspectable")]
        private static extern void GetSpatialCoordinateSystem(IntPtr nativePtr, out SpatialCoordinateSystem coordinateSystem);

        public static SpatialCoordinateSystem SpatialCoordinateSystem => spatialCoordinateSystem ?? (spatialCoordinateSystem = GetSpatialCoordinateSystem(WorldManager.GetNativeISpatialCoordinateSystemPtr()));

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
#elif WINDOWS_UWP
        public static SpatialCoordinateSystem SpatialCoordinateSystem => spatialCoordinateSystem ?? (spatialCoordinateSystem = Marshal.GetObjectForIUnknown(WorldManager.GetNativeISpatialCoordinateSystemPtr()) as SpatialCoordinateSystem);
#elif DOTNETWINRT_PRESENT
        public static SpatialCoordinateSystem SpatialCoordinateSystem
        {
            get
            {
                var spatialCoordinateSystemPtr = WorldManager.GetNativeISpatialCoordinateSystemPtr();
                if (spatialCoordinateSystem == null && spatialCoordinateSystemPtr != System.IntPtr.Zero)
                {
                    spatialCoordinateSystem = SpatialCoordinateSystem.FromNativePtr(WorldManager.GetNativeISpatialCoordinateSystemPtr());
                }
                return spatialCoordinateSystem;
            }
        }
#endif
        private static SpatialCoordinateSystem spatialCoordinateSystem = null;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        [System.Obsolete("Use the System.Numerics.Vector3 extension method ToUnityVector3 instead.")]
        public static UnityEngine.Vector3 SystemVector3ToUnity(System.Numerics.Vector3 vector)
        {
            return vector.ToUnityVector3();
        }

        [System.Obsolete("Use the System.Numerics.Quaternion extension method ToUnityQuaternion instead.")]
        public static UnityEngine.Quaternion SystemQuaternionToUnity(System.Numerics.Quaternion quaternion)
        {
            return quaternion.ToUnityQuaternion();
        }
    }
}