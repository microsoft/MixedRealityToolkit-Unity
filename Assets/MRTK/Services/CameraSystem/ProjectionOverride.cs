// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using Microsoft.MixedReality.Toolkit.Utilities;

#if WINDOWS_UWP
using Windows.Perception.Spatial;
using Windows.Graphics.Holographic;
#endif

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Helper class to override the projection parameters of the HoloLens frame being presented
    /// so that smaller details may appear more sharp.  The FOV of the HoloLens will be smaller
    /// as a tradeoff.
    /// </summary>
    /// <remarks>
    /// Instances of this class are created dynamically by <see cref="MixedRealityCameraSystem"/>.
    /// So there is no need for an AddComponentMenu attribute.
    /// </remarks>
    internal class ProjectionOverride : MonoBehaviour
    {
        /// <summary>
        /// When this is true, projection will be overridden on each frame
        /// </summary>
        public bool ReadingModeEnabled { get; set; } = false;

#if WINDOWS_UWP
        /// <summary>
        /// Internal HoloLens Frame Structure.  Documented <see href="https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unity/unity-xrdevice-advanced">here</see>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct HolographicFrameNativeData
        {
#pragma warning disable 0649
            public uint VersionNumber;
            public uint MaxNumberOfCameras;
            public IntPtr ISpatialCoordinateSystemPtr;
            public IntPtr IHolographicFramePtr;
            public IntPtr IHolographicCameraPtr;
#pragma warning restore 0649
        }

        /// <summary>
        /// Coroutine function to set the camera matrices back to their defaults
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        IEnumerator ResetViewMatricesOnFrameEnd()
        {
            yield return new WaitForEndOfFrame();
            Camera cam = GetComponent<Camera>();
            cam.ResetStereoViewMatrices();
            cam.ResetStereoProjectionMatrices();
        }

        /// <inheritdoc />
        void OnPreCull()
        {
            if (!ReadingModeEnabled)
            {
                return;
            }

            StartCoroutine(ResetViewMatricesOnFrameEnd());

            IntPtr nativeStruct = UnityEngine.XR.XRDevice.GetNativePtr();
            if (nativeStruct != IntPtr.Zero)
            {
                HolographicFrameNativeData s = System.Runtime.InteropServices.Marshal.PtrToStructure<HolographicFrameNativeData>(nativeStruct);

                if (s.IHolographicFramePtr != IntPtr.Zero)
                {
                    var holographicCamera = (HolographicCamera)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(s.IHolographicCameraPtr);
                    var holographicFrame = (HolographicFrame)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(s.IHolographicFramePtr);
                    HolographicFramePrediction prediction = holographicFrame.CurrentPrediction;

                    if (holographicCamera.CanOverrideViewport)
                    {
                        for (int i = 0; i < prediction.CameraPoses.Count; ++i)
                        {
                            HolographicCameraPose cameraPose = prediction.CameraPoses[i];

                            // Default spatial locator is the HMD
                            SpatialLocator defaultSpatialLocator = SpatialLocator.GetDefault();

                            SpatialLocatorAttachedFrameOfReference attachedFrameOfReference = defaultSpatialLocator.CreateAttachedFrameOfReferenceAtCurrentHeading();
                            SpatialCoordinateSystem attachedCoordinateFrame = attachedFrameOfReference.GetStationaryCoordinateSystemAtTimestamp(prediction.Timestamp);

                            HolographicStereoTransform stereoProjection = cameraPose.ProjectionTransform;
                            float ResolutionScale = 45.0f / 33.0f;


                            Matrix4x4 leftProj =  CameraCache.Main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                            Matrix4x4 rightProj =  CameraCache.Main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                            leftProj.m00 *= ResolutionScale;
                            leftProj.m11 *= ResolutionScale;
                            rightProj.m00 *= ResolutionScale;
                            rightProj.m11 *= ResolutionScale;
                            CameraCache.Main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, leftProj);
                            CameraCache.Main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rightProj);


                            stereoProjection.Left.M11 *= ResolutionScale;
                            stereoProjection.Left.M22 *= ResolutionScale;
                            stereoProjection.Right.M11 *= ResolutionScale;
                            stereoProjection.Right.M22 *= ResolutionScale;

                            cameraPose.OverrideProjectionTransform(stereoProjection);
                        }
                    }
                }
            }
        }
#endif
    }
}
