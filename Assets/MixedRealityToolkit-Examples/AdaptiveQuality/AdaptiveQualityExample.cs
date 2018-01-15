// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.AdaptiveQuality
{
    public class AdaptiveQualityExample : MonoBehaviour
    {
        public TextMesh Text;
        public MixedRealityToolkit.Utilities.AdaptiveQuality.AdaptiveQuality Quality;

        private void Update()
        {
            Text.text = string.Format("GPUTime:{0:N2}\nQualityLevel:{1}\nViewportScale:{2:N2}",
                MixedRealityToolkit.Utilities.GpuTiming.GpuTiming.GetTime("Frame") * 1000.0f,
                Quality.QualityLevel,
#if UNITY_2017_2_OR_NEWER
            UnityEngine.XR.XRSettings.renderViewportScale);
#else
            UnityEngine.VR.VRSettings.renderViewportScale);
#endif
        }
    }
}