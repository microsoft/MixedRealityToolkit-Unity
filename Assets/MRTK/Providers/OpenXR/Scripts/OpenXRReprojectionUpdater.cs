// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
using Microsoft.MixedReality.OpenXR;
using System.Linq;
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    public class OpenXRReprojectionUpdater : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the reprojection method used.
        /// </summary>
        public HolographicReprojectionMethod ReprojectionMethod { get; set; }

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private ReprojectionSettings reprojectionSettings = default;

        private void OnPostRender()
        {
            // The reprojection method needs to be set each frame.
            if (ReprojectionMethod != HolographicReprojectionMethod.Depth)
            {
                ReprojectionMode reprojectionMode = MapMRTKReprojectionMethodToOpenXR(ReprojectionMethod);
                reprojectionSettings.ReprojectionMode = reprojectionMode;
                foreach (ViewConfiguration viewConfiguration in ViewConfiguration.EnabledViewConfigurations)
                {
                    if (viewConfiguration.IsActive && viewConfiguration.SupportedReprojectionModes.Contains(reprojectionMode))
                    {
                        viewConfiguration.SetReprojectionSettings(reprojectionSettings);
                    }
                }
            }
        }

        private ReprojectionMode MapMRTKReprojectionMethodToOpenXR(HolographicReprojectionMethod reprojectionMethod)
        {
            switch (reprojectionMethod)
            {
                case HolographicReprojectionMethod.Depth:
                default:
                    return ReprojectionMode.Depth;
                case HolographicReprojectionMethod.PlanarFromDepth:
                    return ReprojectionMode.PlanarFromDepth;
                case HolographicReprojectionMethod.PlanarManual:
                    return ReprojectionMode.PlanarManual;
                case HolographicReprojectionMethod.OrientationOnly:
                    return ReprojectionMode.OrientationOnly;
                case HolographicReprojectionMethod.NoReprojection:
                    return ReprojectionMode.NoReprojection;
            }
        }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
    }
}
