using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Logic for the ReadingModeScene
/// </summary>
public class ReadingModeSceneBehavior : MonoBehaviour
{
    public PinchSlider RenderViewportScaleSlider;

    private void Update()
    {
        if (this.RenderViewportScaleSlider != null)
        {
            XRSettings.renderViewportScale = this.RenderViewportScaleSlider.SliderValue;
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
