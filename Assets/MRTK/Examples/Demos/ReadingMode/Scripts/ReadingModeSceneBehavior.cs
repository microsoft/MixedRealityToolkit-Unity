// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
using Microsoft.MixedReality.Toolkit.Utilities;
#endif // UNITY_2019_3_OR_NEWER

#if !UNITY_2020_1_OR_NEWER
using UnityEngine.XR;
#endif // !UNITY_2020_1_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.ReadingMode
{
    /// <summary>
    /// Logic for the ReadingModeScene
    /// </summary>
    public class ReadingModeSceneBehavior : MonoBehaviour
    {
        [SerializeField]
        private PinchSlider renderViewportScaleSlider = null;

        private float previousSliderValue = -1;

        private void Update()
        {
            if (renderViewportScaleSlider == null || renderViewportScaleSlider.SliderValue == previousSliderValue)
            {
                return;
            }

            previousSliderValue = renderViewportScaleSlider.SliderValue;

#if UNITY_2019_3_OR_NEWER
            if (XRSubsystemHelpers.DisplaySubsystem != null)
            {
                XRSubsystemHelpers.DisplaySubsystem.scaleOfAllViewports = renderViewportScaleSlider.SliderValue;
                return;
            }
#endif // UNITY_2019_3_OR_NEWER

#if !UNITY_2020_1_OR_NEWER
            if (XRDevice.isPresent)
            {
                XRSettings.renderViewportScale = renderViewportScaleSlider.SliderValue;
            }
#endif // !UNITY_2020_1_OR_NEWER
        }

        public void EnableReadingMode()
        {
            var projectionOverrideProvider = CoreServices.GetCameraSystemDataProvider<IMixedRealityCameraProjectionOverrideProvider>();
            if (projectionOverrideProvider != null)
            {
                projectionOverrideProvider.IsProjectionOverrideEnabled = true;
            }
        }

        public void DisableReadingMode()
        {
            var projectionOverrideProvider = CoreServices.GetCameraSystemDataProvider<IMixedRealityCameraProjectionOverrideProvider>();
            if (projectionOverrideProvider != null)
            {
                projectionOverrideProvider.IsProjectionOverrideEnabled = false;
            }
        }
    }
}
