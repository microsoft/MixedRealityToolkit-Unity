// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.OpenXR;

#if MSFT_OPENXR
using System.Collections.Generic;
#endif // MSFT_OPENXR

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    public class OpenXRReprojectionUpdater : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the reprojection method used.
        /// </summary>
        public HolographicReprojectionMethod ReprojectionMethod { get; set; }

#if MSFT_OPENXR
        private readonly Dictionary<uint, bool> cameraIdToSupportsAutoPlanar = new Dictionary<uint, bool>();

        private static readonly bool IsReprojectionModeSupported = OpenXRRuntime.IsExtensionEnabled("XR_MSFT_composition_layer_reprojection_preview");

        //private void OnPostRender()
        //{
        //    // The reprojection method needs to be set each frame.
        //    if (IsReprojectionModeSupported)
        //    {
        //        Microsoft.Windows.Graphics.Holographic.HolographicFrame frame = WindowsMixedRealityUtilities.CurrentHolographicFrame;
        //        foreach (var cameraPose in frame?.CurrentPrediction.CameraPoses)
        //        {
        //            if (CameraSupportsAutoPlanar(cameraPose.HolographicCamera))
        //            {
        //                Microsoft.Windows.Graphics.Holographic.HolographicCameraRenderingParameters renderingParams = frame.GetRenderingParameters(cameraPose);
        //                renderingParams.DepthReprojectionMethod = Microsoft.Windows.Graphics.Holographic.HolographicDepthReprojectionMethod.AutoPlanar;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Checks the Holographic camera to see if it supports auto-planar reprojection.
        ///// </summary>
        ///// <param name="camera">The camera to be queried.</param>
        ///// <returns>
        ///// True if the camera supports auto-planar reprojection, false otherwise.
        ///// </returns>
        //private bool CameraSupportsAutoPlanar(Microsoft.Windows.Graphics.Holographic.HolographicCamera camera)
        //{
        //    bool supportsAutoPlanar = false;

        //    if (!cameraIdToSupportsAutoPlanar.TryGetValue(camera.Id, out supportsAutoPlanar))
        //    {
        //        foreach (var method in camera.ViewConfiguration.SupportedDepthReprojectionMethods)
        //        {
        //            if (method == Microsoft.Windows.Graphics.Holographic.HolographicDepthReprojectionMethod.AutoPlanar)
        //            {
        //                supportsAutoPlanar = true;
        //                break;
        //            }
        //        }
        //        cameraIdToSupportsAutoPlanar.Add(camera.Id, supportsAutoPlanar);
        //    }

        //    return supportsAutoPlanar;
        //}
#endif // MSFT_OPENXR
    }
}
