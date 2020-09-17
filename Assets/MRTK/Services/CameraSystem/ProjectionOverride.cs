// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using UnityEngine;

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
        public bool EnableOverride { get; set; } = false;

#if WINDOWS_UWP
        private struct InternalHoloLensFrameStructure
        {
#pragma warning disable 0649
            public uint VersionNumber;
            public uint MaxNumberOfCameras;
            public IntPtr ISpatialCoordinateSystemPtr;
            public IntPtr IHolographicFramePtr;
            public IntPtr IHolographicCameraPtr;
#pragma warning restore 0649
        }
#endif

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
            if (!EnableOverride)
                return;

            StartCoroutine(ResetViewMatricesOnFrameEnd());

#if WINDOWS_UWP
            IntPtr nativeStruct = UnityEngine.XR.XRDevice.GetNativePtr();
            if (nativeStruct != IntPtr.Zero)
            {
                InternalHoloLensFrameStructure s = System.Runtime.InteropServices.Marshal.PtrToStructure<InternalHoloLensFrameStructure>(nativeStruct);

                if (s.IHolographicFramePtr != IntPtr.Zero)
                {
                    var holographicCamera = (HolographicCamera)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(s.IHolographicCameraPtr);
                    var holographicFrame = (HolographicFrame)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(s.IHolographicFramePtr);
                    HolographicFramePrediction prediction = holographicFrame.CurrentPrediction;
                    for (int i = 0; i < prediction.CameraPoses.Count; ++i)
                    {
                        HolographicCameraPose cameraPose = prediction.CameraPoses[i];

                        // Default spatial locator is the HMD
                        SpatialLocator defaultSpatialLocator = SpatialLocator.GetDefault();

                        SpatialLocatorAttachedFrameOfReference attachedFrameOfReference = defaultSpatialLocator.CreateAttachedFrameOfReferenceAtCurrentHeading();
                        SpatialCoordinateSystem attachedCoordinateFrame = attachedFrameOfReference.GetStationaryCoordinateSystemAtTimestamp(prediction.Timestamp);

                        HolographicStereoTransform stereoProjection = cameraPose.ProjectionTransform;
                        if (holographicCamera.CanOverrideViewport)
                        {
                            float ResolutionScale = 45.0f / 33.0f;


                            Matrix4x4 leftProj = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                            Matrix4x4 rightProj = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                            leftProj.m00 *= ResolutionScale;
                            leftProj.m11 *= ResolutionScale;
                            rightProj.m00 *= ResolutionScale;
                            rightProj.m11 *= ResolutionScale;
                            Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, leftProj);
                            Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rightProj);


                            stereoProjection.Left.M11 *= ResolutionScale;
                            stereoProjection.Left.M22 *= ResolutionScale;
                            stereoProjection.Right.M11 *= ResolutionScale;
                            stereoProjection.Right.M22 *= ResolutionScale;

                            cameraPose.OverrideProjectionTransform(stereoProjection);
                        }
                    }
                }
            }
#endif
        }
    }
}