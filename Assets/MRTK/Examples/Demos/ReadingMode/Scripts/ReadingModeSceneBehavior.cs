// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
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

        private void Update()
        {
            if (renderViewportScaleSlider != null)
            {
                XRSettings.renderViewportScale = renderViewportScaleSlider.SliderValue;
            }
        }

        public void EnableReadingMode()
        {
            CoreServices.CameraSystem.ReadingModeEnabled = true;
        }

        public void DisableReadingMode()
        {
            CoreServices.CameraSystem.ReadingModeEnabled = false;
        }
    }
}
