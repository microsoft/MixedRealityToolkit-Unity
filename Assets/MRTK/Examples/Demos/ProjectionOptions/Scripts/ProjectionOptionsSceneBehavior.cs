using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;

public class ProjectionOptionsSceneBehavior : MonoBehaviour
{
    public PinchSlider RenderViewportScaleSlider;

    private void Update()
    {
        if(this.RenderViewportScaleSlider != null)
        {
            XRSettings.renderViewportScale = this.RenderViewportScaleSlider.SliderValue;
        }
    }

    public void EnableProjectionOverride()
    {
        CoreServices.CameraSystem.ProjectionOverrideEnabled = true;
    }

    public void DisableProjectionOverride()
    {
        CoreServices.CameraSystem.ProjectionOverrideEnabled = false;
    }
}
