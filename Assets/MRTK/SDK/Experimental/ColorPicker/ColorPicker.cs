// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.ColorPicker
{
    /// <summary>
    /// Example script to demonstrate adding buttons, sliders and a touchable gradient to control material values at runtime.
    /// </summary>
    public class ColorPicker : MonoBehaviour, IMixedRealityTouchHandler
    {
        private MeshRenderer targetObjectMesh = null;
        public MeshRenderer TargetObjectMesh
        {
            get => targetObjectMesh;
            set => targetObjectMesh = value;
        }
        private SpriteRenderer targetObjectSprite = null;
        public SpriteRenderer TargetObjectSprite
        {
            get => targetObjectSprite;
            set => targetObjectSprite = value;
        }
        [Experimental]
        [SerializeField]
        [Tooltip("Any mesh within the ColorPicker UI that receives color changes")]
        private MeshRenderer[] PickerUIMeshes = null;
        [SerializeField]
        [Tooltip("Any sprite within the ColorPicker UI that receives color changes")]
        private SpriteRenderer[] PickerUISprites = null;
        [SerializeField]
        [Tooltip("The gradient mesh that receives touch input")]
        private MeshRenderer GradientMesh = null;
        [SerializeField]
        [Tooltip("The gradient drag game object that gets constrained while dragging")]
        private GameObject GradientDragger = null;
        [SerializeField]
        [Tooltip("A pinch slider used for the color red")]
        private PinchSlider SliderRed = null;
        [SerializeField]
        [Tooltip("A pinch slider used for the color green")]
        private PinchSlider SliderGreen = null;
        [SerializeField]
        [Tooltip("A pinch slider used for the color blue")]
        private PinchSlider SliderBlue = null;
        [SerializeField]
        [Tooltip("A pinch slider used for the color alpha")]
        private PinchSlider SliderAlpha = null;
        [SerializeField]
        [Tooltip("A pinch slider used for the color hue")]
        private PinchSlider SliderHue = null;
        [SerializeField]
        [Tooltip("A pinch slider used for saturation")]
        private PinchSlider SliderSaturation = null;
        [SerializeField]
        [Tooltip("A pinch slider used for brightness")]
        private PinchSlider SliderBrightness = null;
        //
        [SerializeField]
        [Tooltip("The text value of the color's red property")]
        private TextMeshPro TextRed = null;
        [SerializeField]
        [Tooltip("The text value of the color's green property")]
        private TextMeshPro TextGreen = null;
        [SerializeField]
        [Tooltip("The text value of the color's blue property")]
        private TextMeshPro TextBlue = null;
        [SerializeField]
        [Tooltip("The text value of the color's alpha property")]
        private TextMeshPro TextAlpha = null;
        [SerializeField]
        [Tooltip("The text value of the color's hex property")]
        private TextMeshPro TextHex = null;
        [SerializeField]
        [Tooltip("The text value of the color's hue property")]
        private TextMeshPro TextHue = null;
        [SerializeField]
        [Tooltip("The text value of the color's saturation property")]
        private TextMeshPro TextSaturation = null;
        [SerializeField]
        [Tooltip("The text value of the color's brightness property")]
        private TextMeshPro TextBrightness = null;
        //
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
            this.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (IsDraggingGradient)
            {
                ConstrainGradientDragging();
            }
            if (IsDraggingSliders)
            {
                CalculateGradientDraggerPosition();
            }
        }
        #region Gradient Logic (Private)
        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            GradientDragger.transform.position = new Vector3(eventData.InputData.x, eventData.InputData.y, eventData.InputData.z);
            ConstrainGradientDragging();
            ApplyColor();
            UpdateSliderText();
            ApplySliderValues();
        }
        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
        }
        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
        }
        private void CalculateGradientDraggerPosition()
        {
            float xPosition = ((Saturation + GradientDragMaxDistance) * -1) + 1;
            float yPosition = Brightness - GradientDragMaxDistance;
            GradientDragCurrentPosition.x = Mathf.Clamp(xPosition, -GradientDragMaxDistance, GradientDragMaxDistance);
            GradientDragCurrentPosition.y = Mathf.Clamp(yPosition, -GradientDragMaxDistance, GradientDragMaxDistance);
            GradientDragger.transform.localPosition = GradientDragCurrentPosition;
        }
        private void ConstrainGradientDragging()
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
            Saturation = Mathf.Abs(GradientDragCurrentPosition.x + (GradientDragMaxDistance * -1));
            Brightness = GradientDragCurrentPosition.y + GradientDragMaxDistance;
            CustomColor = Color.HSVToRGB(Hue, Saturation, Brightness);
            CustomColor.a = Alpha;
            //
            UpdateSliderText();
            ApplyColor();
        }
        #endregion

        #region Gradient Logic (Public)
        /// <summary>
        /// Touching the gradient texture will calculate the color and constrain the dragger based on the point in eventData
        /// </summary>
        public void ClickGradientTexture(MixedRealityPointerEventData eventData)
        {
            GradientDragger.transform.position = eventData.Pointer.Result.Details.Point;
            ConstrainGradientDragging();
            ApplyColor();
            UpdateSliderText();
            ApplySliderValues();
        }
        /// <summary>
        /// Tells the update loop that the gradient is being dragged
        /// </summary>
        public void StartDragGradient()
        {
            IsDraggingGradient = true;
        }
        /// <summary>
        /// Tells the update loop that the gradient is not being dragged, and applies the updated color value to the sliders
        /// </summary>
        public void StopDragGradient()
        {
            IsDraggingGradient = false;
            ApplySliderValues();
        }

        #endregion

        #region Public Functions
        /// <summary>
        /// This will set the visibility, scale, and position of the color picker while extracting the color of the touched object's MeshRenderer or SpriteRender
        /// </summary>
        public void SummonColorPicker(GameObject container)
        {
            this.gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            transform.position = GameObject.Find(container.name + "/Anchor").transform.position;
            TargetObjectMesh = GameObject.Find(container.name + "/TargetObject (Mesh)").GetComponent<MeshRenderer>();
            TargetObjectSprite = GameObject.Find(container.name + "/TargetObject (Sprite)").GetComponent<SpriteRenderer>();
            ExtractColorFromMaterial(TargetObjectMesh);
        }
        /// <summary>
        /// Applies Hue, Saturation, Brightness slider values to the Red, Green, Blue sliders
        /// </summary>
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
        /// <summary>
        /// Applies Red, Green, Blue slider values to the Hue, Saturation, Brightness sliders
        /// </summary>
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
        /// <summary>
        /// Extracts a color from a MeshRenderer and applies it to the color picker
        /// </summary>
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
        /// <summary>
        /// Tells the update loop that a slider knob is being dragged
        /// </summary>
        public void StartDrag(GameObject dragger)
        {
            dragger.SetActive(true);
            IsDraggingSliders = true;
        }
        /// <summary>
        /// Tells the update loop that a slider knob is not being dragged and applies the value to all sliders
        /// </summary>
        public void StopDrag(GameObject dragger)
        {
            dragger.SetActive(false);
            IsDraggingSliders = false;
            ApplySliderValues();
        }
        #endregion

        #region UI Logic
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
            if(GradientMesh != null && GradientMesh.material != null)
            {
                GradientMesh.material.color = Color.HSVToRGB(Hue, 1, 1);
            }
            if(TargetObjectMesh != null && TargetObjectMesh.material != null)
            {
                TargetObjectMesh.material.color = CustomColor;
            }
            if(TargetObjectSprite != null)
            {
                TargetObjectSprite.color = CustomColor;
            }
            foreach(MeshRenderer rend in PickerUIMeshes)
            {
                if(rend != null)
                {
                    rend.material.color = CustomColor;
                }
            }
            foreach(SpriteRenderer rend in PickerUISprites)
            {
                if(rend != null)
                {
                    rend.color = CustomColor;
                    if (rend.name == "Dragger")
                    {
                        // don't fade the alpha of the dragger object
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
        #endregion
    }
}
