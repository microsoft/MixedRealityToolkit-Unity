// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.ColorPicker
{
    /// <summary>
    /// Example script to demonstrate adding sliders to control the joystick values at runtime.
    /// </summary>
    public class ColorSliders : MonoBehaviour
    {
        public MeshRenderer[] TargetObjectMesh;
        public SpriteRenderer[] TargetObjectSprite;
        //
        public PinchSlider SliderRed;
        public PinchSlider SliderGreen;
        public PinchSlider SliderBlue;
        public PinchSlider SliderAlpha;
        public PinchSlider SliderHue;
        public PinchSlider SliderSaturation;
        public PinchSlider SliderBrightness;
        //
        public TextMeshPro TextRed;
        public TextMeshPro TextBlue;
        public TextMeshPro TextGreen;
        public TextMeshPro TextAlpha;
        public TextMeshPro TextHue;
        public TextMeshPro TextSaturation;
        public TextMeshPro TextBrightness;
        //
        private Color CustomColor = Color.clear;
        private float hue, saturation, brightness;
        //
        public void UpdateColorHSV()
        {
            hue = SliderHue.SliderValue;
            saturation = SliderSaturation.SliderValue;
            brightness = SliderBrightness.SliderValue;
            CustomColor = Color.HSVToRGB(hue, saturation, brightness);
            //
            UpdateSliderText();
            ApplyColor();
        }
        public void UpdateColorRGB() {
            CustomColor.r = SliderRed.SliderValue;
            CustomColor.g = SliderGreen.SliderValue;
            CustomColor.b = SliderBlue.SliderValue;
            CustomColor.a = SliderAlpha.SliderValue;
            //
            UpdateSliderText();
            ApplyColor();
        }
        private void UpdateSliderText()
        {
            TextRed.text = CustomColor.r.ToString();
            TextBlue.text = CustomColor.b.ToString();
            TextGreen.text = CustomColor.g.ToString();
            TextAlpha.text = CustomColor.a.ToString();
            //
            TextHue.text = hue.ToString();
            TextSaturation.text = saturation.ToString();
            TextBrightness.text = brightness.ToString();
        }
        private void ApplyColor()
        {
            foreach(MeshRenderer rend in TargetObjectMesh)
            {
                if(rend != null)
                {
                    rend.material.color = CustomColor;
                }
            }
            foreach(SpriteRenderer rend in TargetObjectSprite)
            {
                if(rend != null)
                {
                    rend.color = CustomColor;
                    if (rend.name == "Dragger")
                    {
                        // dont fade the alpha of the dragger object
                        rend.color = new Color(CustomColor.r, CustomColor.g, CustomColor.b, 1);
                    }
                }
            }
        }
        private void ApplySliderValues() {
            SliderRed.SliderValue = CustomColor.r;
            SliderGreen.SliderValue = CustomColor.g;
            SliderBlue.SliderValue = CustomColor.b;
            SliderHue.SliderValue = hue;
            SliderSaturation.SliderValue = saturation;
            SliderBrightness.SliderValue = brightness;
        }
    }
}
