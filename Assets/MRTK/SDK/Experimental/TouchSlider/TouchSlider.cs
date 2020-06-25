// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This control enables touching a slider with one finger to set its value
    /// </summary>
    public class TouchSlider : MonoBehaviour
    {
        [Experimental]
        [SerializeField]
        private PinchSlider slider;
        [SerializeField]
        private TextMeshPro text;
        public void UpdateSliderText()
        {
            text.text = slider.SliderValue.ToString();
        }
    }
}