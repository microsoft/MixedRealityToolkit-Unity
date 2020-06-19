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
    public class ColorPicker : MonoBehaviour, IMixedRealityTouchHandler
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
        public MeshRenderer GradientMesh;
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
                CalculateGradientDraggerPosition();
            }
        }
        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            //Debug.Log("OnTouchStarted: " + Time.unscaledTime);
        }
        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            //Debug.Log("OnTouchCompleted: " + Time.unscaledTime);
        }

        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            GradientDragger.transform.position = new Vector3(eventData.InputData.x, eventData.InputData.y, eventData.InputData.z);
            ConstrainDragging();
            ApplyColor();
            UpdateSliderText();
            ApplySliderValues();
        }
        private void CalculateGradientDraggerPosition()
        {
            float xPosition = ((Saturation + GradientDragMaxDistance) * -1) + 1;
            float yPosition = Brightness - GradientDragMaxDistance;
            GradientDragCurrentPosition.x = Mathf.Clamp(xPosition, -GradientDragMaxDistance, GradientDragMaxDistance);
            GradientDragCurrentPosition.y = Mathf.Clamp(yPosition, -GradientDragMaxDistance, GradientDragMaxDistance);
            GradientDragger.transform.localPosition = GradientDragCurrentPosition;
        }
        public void ClickGradientTexture(MixedRealityPointerEventData eventData)
        {
            GradientDragger.transform.position = eventData.Pointer.Result.Details.Point;
            ConstrainDragging();
            ApplyColor();
            UpdateSliderText();
            ApplySliderValues();
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
            Color.RGBToHSV(CustomColor, out Hue, out Saturation, out Brightness);
            CustomColor.a = Alpha;
            //
            CalculateGradientDraggerPosition();
            UpdateSliderText();
            ApplyColor();
            ApplySliderValues();
        }
        public void StartDrag(GameObject dragger)
        {
            dragger.SetActive(true);
            IsDraggingSliders = true;
        }
        public void StopDrag(GameObject dragger)
        {
            dragger.SetActive(false);
            IsDraggingSliders = false;
            ApplySliderValues();
        }
        private void UpdateSliderText()
        {
            TextRed.text = Mathf.Clamp(Mathf.RoundToInt(CustomColor.r * 255),0,255).ToString();
            TextBlue.text = Mathf.Clamp(Mathf.RoundToInt(CustomColor.b * 255), 0, 255).ToString();
            TextGreen.text = Mathf.Clamp(Mathf.RoundToInt(CustomColor.g * 255), 0, 255).ToString();
            TextAlpha.text = Mathf.Clamp(Mathf.RoundToInt(CustomColor.a * 100), 0, 100) + "%";
            //
            TextHex.text = "#" + ColorUtility.ToHtmlStringRGBA(CustomColor);
            TextHue.text = Mathf.Clamp(Mathf.RoundToInt(Hue * 360), 0, 360).ToString();
            TextSaturation.text = Mathf.Clamp(Mathf.RoundToInt(Saturation * 100), 0, 100) + "%";
            TextBrightness.text = Mathf.Clamp(Mathf.RoundToInt(Brightness * 100), 0, 100) + "%";
        }
        private void ApplyColor()
        {
            GradientMesh.material.color = Color.HSVToRGB(Hue, 1, 1);
            foreach (MeshRenderer rend in TargetObjectMesh)
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
            SliderRed.SliderValue = Mathf.Clamp(CustomColor.r,0,1);
            SliderGreen.SliderValue = Mathf.Clamp(CustomColor.g, 0, 1);
            SliderBlue.SliderValue = Mathf.Clamp(CustomColor.b, 0, 1);
            SliderAlpha.SliderValue = Mathf.Clamp(CustomColor.a, 0, 1);
            SliderHue.SliderValue = Mathf.Clamp(Hue, 0, 1);
            SliderSaturation.SliderValue = Mathf.Clamp(Saturation, 0, 1);
            SliderBrightness.SliderValue = Mathf.Clamp(Brightness, 0, 1);
        }
    }
}
