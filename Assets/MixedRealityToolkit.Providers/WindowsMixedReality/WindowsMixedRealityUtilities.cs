// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if WINDOWS_UWP
#if ENABLE_DOTNET
using System;
#endif // ENABLE_DOTNET
using System.Runtime.InteropServices;
using UnityEngine.XR.WSA;
using Windows.Perception.Spatial;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    public static class WindowsMixedRealityUtilities
    {
#if WINDOWS_UWP
#if ENABLE_DOTNET
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
#else
        public static SpatialCoordinateSystem SpatialCoordinateSystem => spatialCoordinateSystem ?? (spatialCoordinateSystem = Marshal.GetObjectForIUnknown(WorldManager.GetNativeISpatialCoordinateSystemPtr()) as SpatialCoordinateSystem);
#endif
        private static SpatialCoordinateSystem spatialCoordinateSystem = null;
#endif // WINDOWS_UWP

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