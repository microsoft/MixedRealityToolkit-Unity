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
    public class ColorPicker : MonoBehaviour
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
        public TextMeshPro TextHex;
        public TextMeshPro TextHue;
        public TextMeshPro TextSaturation;
        public TextMeshPro TextBrightness;
        //
        public GameObject GradientDragger;
        private float GradientDragMaxDistance = 0.5f;
        private Vector3 GradientDragStartPosition;
        private Vector3 GradientDragCurrentPosition;
        //
        private Color CustomColor;
        private float Hue, Saturation, Brightness, Alpha = 0.3f;
        //
        private bool IsDraggingSliders = false;
        private bool IsDraggingGradient = false;
        //
        private void Start()
        {
            GradientDragStartPosition = GradientDragger.transform.localPosition;
            GradientDragCurrentPosition = GradientDragStartPosition;
            CustomColor = Color.red;
            CustomColor.a = Alpha;
            UpdateSliderText();
            ApplyColor();
        }
        private void Update()
        {
            if (IsDraggingGradient)
            {
                ConstrainDragging();
            }
            if (IsDraggingSliders)
            {
                GradientDragCurrentPosition.x = ((Saturation + GradientDragMaxDistance) * -1)+1;
                GradientDragCurrentPosition.y = Brightness - GradientDragMaxDistance;
                GradientDragger.transform.localPosition = GradientDragCurrentPosition;
            }
        }
        public void StartDragGradient()
        {
            IsDraggingGradient = true;
        }
        public void StopDragGradient()
        {
            IsDraggingGradient = false;
            ApplySliderValues();
        }
        private void ConstrainDragging()
        {
            // Horizontal
            if (GradientDragger.transform.localPosition.x >= GradientDragStartPosition.x + GradientDragMaxDistance)
            {
                GradientDragCurrentPosition.x = GradientDragStartPosition.x + GradientDragMaxDistance;
            }
            else if (GradientDragger.transform.localPosition.x <= GradientDragStartPosition.x - GradientDragMaxDistance)
            {
                GradientDragCurrentPosition.x = GradientDragStartPosition.x - GradientDragMaxDistance;
            }
            else
            {
                GradientDragCurrentPosition.x = GradientDragger.transform.localPosition.x;
            }
            // Vertical
            if (GradientDragger.transform.localPosition.y >= GradientDragStartPosition.y + GradientDragMaxDistance)
            {
                GradientDragCurrentPosition.y = GradientDragStartPosition.y + GradientDragMaxDistance;
            }
            else if (GradientDragger.transform.localPosition.y <= GradientDragStartPosition.y - GradientDragMaxDistance)
            {
                GradientDragCurrentPosition.y = GradientDragStartPosition.y - GradientDragMaxDistance;
            }
            else
            {
                GradientDragCurrentPosition.y = GradientDragger.transform.localPosition.y;
            }
            GradientDragger.transform.localPosition = GradientDragCurrentPosition;
            //DebugText.text = GradientDragCurrentPosition.ToString();
            Saturation = Mathf.Abs(GradientDragCurrentPosition.x + (GradientDragMaxDistance * -1));
            Brightness = GradientDragCurrentPosition.y + GradientDragMaxDistance;
            CustomColor = Color.HSVToRGB(Hue, Saturation, Brightness);
            CustomColor.a = Alpha;
            //
            UpdateSliderText();
            ApplyColor();
        }
        public void UpdateColorHSV()
        {
            if (IsDraggingSliders == true)
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
            if (IsDraggingSliders == true)
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
        public void ExtractColorFromMaterial(MeshRenderer meshRenderer)
        {
            CustomColor = meshRenderer.material.color;
            CustomColor.a = Alpha;
            UpdateSliderText();
            ApplyColor();
            ApplySliderValues();
        }
        public void StartDrag()
        {
            IsDraggingSliders = true;
        }
        public void StopDrag()
        {
            IsDraggingSliders = false;
            ApplySliderValues();
        }
        private void UpdateSliderText()
        {
            TextRed.text = Mathf.RoundToInt(CustomColor.r * 255).ToString();
            TextBlue.text = Mathf.RoundToInt(CustomColor.b * 255).ToString();
            TextGreen.text = Mathf.RoundToInt(CustomColor.g * 255).ToString();
            TextAlpha.text = Mathf.RoundToInt(CustomColor.a * 100) + "%";
            //
            TextHex.text = "#" + ColorUtility.ToHtmlStringRGBA(CustomColor);
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
