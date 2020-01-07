// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using System;
using System.Runtime.InteropServices;
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

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
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
        /// Please see https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/HandTracking.html#net-native for more info.
        /// </remarks>
        private static SpatialCoordinateSystem GetSpatialCoordinateSystem(IntPtr nativePtr)
        {
            try
            {
                SpatialCoordinateSystem coordinateSystem;
                GetSpatialCoordinateSystem(nativePtr, out coordinateSystem);
                return coordinateSystem;
            }
            catch
            {
                UnityEngine.Debug.LogError("Call to the DotNetNativeWorkaround plug-in failed. The plug-in is required for correct behavior when using .NET Native compilation");
                return Marshal.GetObjectForIUnknown(nativePtr) as SpatialCoordinateSystem;
            }
        }
#endif //ENABLE_DOTNET

        public static IWindowsMixedRealityUtilitiesProvider UtilitiesProvider { get; set; } = null;

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
                if (spatialCoordinateSystem == null && UtilitiesProvider != null)
                {
#if ENABLE_DOTNET
                    spatialCoordinateSystem = GetSpatialCoordinateSystem(UtilitiesProvider.ISpatialCoordinateSystemPtr);
#elif WINDOWS_UWP
                    spatialCoordinateSystem = Marshal.GetObjectForIUnknown(UtilitiesProvider.ISpatialCoordinateSystemPtr) as SpatialCoordinateSystem;
#elif DOTNETWINRT_PRESENT
                    spatialCoordinateSystem = SpatialCoordinateSystem.FromNativePtr(UtilitiesProvider.ISpatialCoordinateSystemPtr);
#endif
                }
                return spatialCoordinateSystem;
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
                if (UtilitiesProvider == null)
                {
                    return null;
                }

#if DOTNETWINRT_PRESENT
                return HolographicFrame.FromNativePtr(UtilitiesProvider.IHolographicFramePtr);
#elif WINDOWS_UWP
                return Marshal.GetObjectForIUnknown(UtilitiesProvider.IHolographicFramePtr) as HolographicFrame;
#else
                return null;
#endif
            }
        }

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
