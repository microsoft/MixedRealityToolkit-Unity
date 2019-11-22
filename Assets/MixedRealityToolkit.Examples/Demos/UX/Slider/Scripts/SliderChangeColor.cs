//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("Scripts/MRTK/Examples/SliderChangeColor")]
    public class SliderChangeColor : MonoBehaviour
    {
        [SerializeField]
        private Renderer TargetRenderer;

        public void OnSliderUpdatedRed(SliderEventData eventData)
        {
            TargetRenderer = GetComponentInChildren<Renderer>();
            if ((TargetRenderer != null) && (TargetRenderer.material != null))
            {
                TargetRenderer.material.color = new Color(eventData.NewValue, TargetRenderer.sharedMaterial.color.g, TargetRenderer.sharedMaterial.color.b);
            }
        }

        public void OnSliderUpdatedGreen(SliderEventData eventData)
        {
            TargetRenderer = GetComponentInChildren<Renderer>();
            if ((TargetRenderer != null) && (TargetRenderer.material != null))
            {
                TargetRenderer.material.color = new Color(TargetRenderer.sharedMaterial.color.r, eventData.NewValue, TargetRenderer.sharedMaterial.color.b);
            }
        }

        public void OnSliderUpdateBlue(SliderEventData eventData)
        {
            TargetRenderer = GetComponentInChildren<Renderer>();
            if ((TargetRenderer != null) && (TargetRenderer.material != null))
            {
                TargetRenderer.material.color = new Color(TargetRenderer.sharedMaterial.color.r, TargetRenderer.sharedMaterial.color.g, eventData.NewValue);
            }
        }
    }
}
