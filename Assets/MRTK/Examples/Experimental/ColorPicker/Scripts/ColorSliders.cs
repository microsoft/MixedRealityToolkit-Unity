// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.ColorPicker
{
    /// <summary>
    /// Example script to demonstrate adding sliders to control material values at runtime.
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
        public TextMeshPro TextGreen;
        public TextMeshPro TextBlue;
        public TextMeshPro TextAlpha;
        public TextMeshPro TextHue;
        public TextMeshPro TextSaturation;
        public TextMeshPro TextBrightness;
        //
        private Color CustomColor;
        private float Hue, Saturation, Brightness, Alpha = 0.3f;
        //
        bool IsDragging = false;
        //
        private void Start()
        {
            CustomColor = Color.red;
            CustomColor.a = Alpha;
            UpdateSliderText();
            ApplyColor();
        }
        public void UpdateColorHSV()
        {
            if (IsDragging == true)
            {
                Hue = SliderHue.SliderValue;
                Saturation = SliderSaturation.SliderValue;
                Brightness = SliderBrightness.SliderValue;
                CustomColor = Color.HSVToRGB(Hue, Saturation, Brightness);
                CustomColor.a = Alpha;
                //
                UpdateSliderText();
                ApplyColor();
            }
        }
        public void UpdateColorRGB() {
            if (IsDragging == true)
            {
                CustomColor.r = SliderRed.SliderValue;
                CustomColor.g = SliderGreen.SliderValue;
                CustomColor.b = SliderBlue.SliderValue;
                Alpha = SliderAlpha.SliderValue;
                CustomColor.a = Alpha;
                Color.RGBToHSV(CustomColor, out Hue, out Saturation, out Brightness);
                //
                UpdateSliderText();
                ApplyColor();
            }
        }
        public void StartDrag()
        {
            IsDragging = true;
        }
        public void StopDrag()
        {
            IsDragging = false;
            ApplySliderValues();
        }
        private void UpdateSliderText()
        {
            TextRed.text = Mathf.RoundToInt(CustomColor.r * 255).ToString();
            TextBlue.text = Mathf.RoundToInt(CustomColor.b * 255).ToString();
            TextGreen.text = Mathf.RoundToInt(CustomColor.g * 255).ToString();
            TextAlpha.text = Mathf.RoundToInt(CustomColor.a * 100) + "%";
            //
            TextHue.text = Mathf.RoundToInt(Hue * 360).ToString();
            TextSaturation.text = Mathf.RoundToInt(Saturation * 100) + "%";
            TextBrightness.text = Mathf.RoundToInt(Brightness * 100) + "%";
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
            SliderAlpha.SliderValue = CustomColor.a;
            SliderHue.SliderValue = Hue;
            SliderSaturation.SliderValue = Saturation;
            SliderBrightness.SliderValue = Brightness;
        }
    }
}
