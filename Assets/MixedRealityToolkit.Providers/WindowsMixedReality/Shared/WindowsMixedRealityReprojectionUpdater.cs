// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA && DOTNETWINRT_PRESENT
using Microsoft.Windows.Foundation.Metadata;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Script used to update the reprojection method for Windows Mixed Reality devices.
    /// </summary>
    public class WindowsMixedRealityReprojectionUpdater : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the reprojection method used by Windows Mixed Reality.
        /// </summary>
        public HolographicDepthReprojectionMethod ReprojectionMethod { get; set; }

#if UNITY_WSA && DOTNETWINRT_PRESENT
        private readonly Dictionary<uint, bool> cameraIdToSupportsAutoPlanar = new Dictionary<uint, bool>();

        private static readonly bool isDepthReprojectionModeSupported = ApiInformation.IsPropertyPresent("Windows.Graphics.Holographic.HolographicCameraRenderingParameters", "DepthReprojectionMethod");

        private void OnPostRender()
        {
            // The reprojection method needs to be set each frame.
            if (isDepthReprojectionModeSupported &&
                (ReprojectionMethod == HolographicDepthReprojectionMethod.AutoPlanar))
            {
                Microsoft.Windows.Graphics.Holographic.HolographicFrame frame = WindowsMixedRealityUtilities.CurrentHolographicFrame;
                foreach (var cameraPose in frame?.CurrentPrediction.CameraPoses)
                {
                    if (CameraSupportsAutoPlanar(cameraPose.HolographicCamera))
                    {
                        Microsoft.Windows.Graphics.Holographic.HolographicCameraRenderingParameters renderingParams = frame.GetRenderingParameters(cameraPose);
                        renderingParams.DepthReprojectionMethod = Microsoft.Windows.Graphics.Holographic.HolographicDepthReprojectionMethod.AutoPlanar;
                    }
                }
            }
        }

        /// <summary>
        /// Checks the Holographic camera to see if it supports auto-planar reprojection.
        /// </summary>
        /// <param name="camera">The camera to be queried.</param>
        /// <returns>
        /// True if the camera supports auto-planar reprojection, false otherwise.
        /// </returns>
        private bool CameraSupportsAutoPlanar(Microsoft.Windows.Graphics.Holographic.HolographicCamera camera)
        {
            bool supportsAutoPlanar = false;

            if (!cameraIdToSupportsAutoPlanar.TryGetValue(camera.Id, out supportsAutoPlanar))
            {
                foreach (var method in camera.ViewConfiguration.SupportedDepthReprojectionMethods)
                {
                    if (method == Microsoft.Windows.Graphics.Holographic.HolographicDepthReprojectionMethod.AutoPlanar)
                    {
                        supportsAutoPlanar = true;
                        break;
                    }
                }
                cameraIdToSupportsAutoPlanar.Add(camera.Id, supportsAutoPlanar);
            }

            return supportsAutoPlanar;
        }
#endif // UNITY_WSA && DOTNETWINRT_PRESENT
    }
}
