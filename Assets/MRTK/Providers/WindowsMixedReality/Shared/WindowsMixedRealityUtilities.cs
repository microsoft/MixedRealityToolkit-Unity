// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
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
        /// <summary>
        /// The provider that should be used for the corresponding utilities.
        /// </summary>
        /// <remarks>
        /// This is intended to be used to support both XR SDK and Unity's legacy XR pipeline, which provide
        /// different APIs to access these native objects.
        /// </remarks>
        public static IWindowsMixedRealityUtilitiesProvider UtilitiesProvider { get; set; } = null;

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
#endif // ENABLE_DOTNET

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
                if (UtilitiesProvider != null)
                {
                    IntPtr newSpatialCoordinateSystemPtr = UtilitiesProvider.ISpatialCoordinateSystemPtr;
                    if (newSpatialCoordinateSystemPtr != currentSpatialCoordinateSystemPtr && newSpatialCoordinateSystemPtr != IntPtr.Zero)
                    {
#if ENABLE_DOTNET
                        spatialCoordinateSystem = GetSpatialCoordinateSystem(newSpatialCoordinateSystemPtr);
#elif WINDOWS_UWP
                        spatialCoordinateSystem = Marshal.GetObjectForIUnknown(newSpatialCoordinateSystemPtr) as SpatialCoordinateSystem;
#elif DOTNETWINRT_PRESENT
                        spatialCoordinateSystem = SpatialCoordinateSystem.FromNativePtr(newSpatialCoordinateSystemPtr);
#endif
                        currentSpatialCoordinateSystemPtr = newSpatialCoordinateSystemPtr;
                    }
                }
                return spatialCoordinateSystem;
            }
        }

        private static IntPtr currentSpatialCoordinateSystemPtr = IntPtr.Zero;

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
                if (UtilitiesProvider == null || UtilitiesProvider.IHolographicFramePtr == IntPtr.Zero)
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

        [Obsolete("Use the System.Numerics.Vector3 extension method ToUnityVector3 instead.")]
        public static UnityEngine.Vector3 SystemVector3ToUnity(System.Numerics.Vector3 vector)
        {
            return vector.ToUnityVector3();
        }

        [Obsolete("Use the System.Numerics.Quaternion extension method ToUnityQuaternion instead.")]
        public static UnityEngine.Quaternion SystemQuaternionToUnity(System.Numerics.Quaternion quaternion)
        {
            return quaternion.ToUnityQuaternion();
        }
    }
}
