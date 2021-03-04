// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;

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
        private const float MinScale = 0.001f;

        private void Update()
        {
            if (renderViewportScaleSlider == null || renderViewportScaleSlider.SliderValue == previousSliderValue)
            {
                return;
            }

            previousSliderValue = renderViewportScaleSlider.SliderValue;

            if (DeviceUtility.IsPresent)
            {
                XRSettings.renderViewportScale = Mathf.Max(renderViewportScaleSlider.SliderValue, MinScale);
            }
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
