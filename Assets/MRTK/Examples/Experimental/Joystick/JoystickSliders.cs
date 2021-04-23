// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.Joystick
{
    /// <summary>
    /// Example script to demonstrate adding sliders to control the joystick values at runtime.
    /// </summary>
    public class JoystickSliders : MonoBehaviour
    {
        public PinchSlider SliderMove;
        public PinchSlider SliderScale;
        public PinchSlider SliderRotate;
        public PinchSlider SliderRebound;
        public PinchSlider SliderSensitivityHorizontal;
        public PinchSlider SliderSensitivityVertical;
        public TextMeshPro TextMove;
        public TextMeshPro TextScale;
        public TextMeshPro TextRotate;
        public TextMeshPro TextRebound;
        public TextMeshPro TextSensitivityHorizontal;
        public TextMeshPro TextSensitivityVertical;
        public JoystickController[] Joysticks;
        private float MinimumFloatValue = 0.001f;
        private int MinimumIntValue = 1;
        private float DefaultMoveValue = 0.001f;
        private float DefaultScaleValue = 0.0001f;
        private float DefaultRotateValue = 0.3f;
        private float DefaultReboundValue = 5f;
        private float DefaultSensitivityHorizontalValue = 3f;
        private float DefaultSliderSensitivityVerticalValue = 6f;
        private float CurrentMoveValue;
        private float CurrentScaleValue;
        private float CurrentRotateValue;
        private float CurrentReboundValue;
        private float CurrentSensitivityHorizontalValue;
        private float CurrentSliderSensitivityVerticalValue;
        private void Start()
        {
            CalculateValues();
        }
        private void CalculateValues()
        {
            CurrentMoveValue = (DefaultMoveValue * 2) * SliderMove.SliderValue + MinimumFloatValue;
            CurrentScaleValue = (DefaultScaleValue * 2) * SliderScale.SliderValue + MinimumFloatValue;
            CurrentRotateValue = (DefaultRotateValue * 2) * SliderRotate.SliderValue + MinimumFloatValue;
            CurrentReboundValue = (DefaultReboundValue * 2) * SliderRebound.SliderValue + MinimumIntValue;
            CurrentSensitivityHorizontalValue = (DefaultSensitivityHorizontalValue * 2) * SliderSensitivityHorizontal.SliderValue + MinimumIntValue;
            CurrentSliderSensitivityVerticalValue = (DefaultSliderSensitivityVerticalValue * 2) * SliderSensitivityVertical.SliderValue + MinimumIntValue;
        }
        public void UpdateSliderValues()
        {
            CalculateValues();
            TextMove.text = CurrentMoveValue.ToString();
            TextScale.text = CurrentScaleValue.ToString();
            TextRotate.text = CurrentRotateValue.ToString();
            TextRebound.text = CurrentReboundValue.ToString();
            TextSensitivityHorizontal.text = CurrentSensitivityHorizontalValue.ToString();
            TextSensitivityVertical.text = CurrentSliderSensitivityVerticalValue.ToString();
            foreach (JoystickController joystick in Joysticks)
            {
                joystick.MoveSpeed = CurrentMoveValue;
                joystick.ScaleSpeed = CurrentScaleValue;
                joystick.RotationSpeed = CurrentRotateValue;
                joystick.ReboundSpeed = CurrentReboundValue;
                joystick.SensitivityLeftRight = CurrentSensitivityHorizontalValue;
                joystick.SensitivityForwardBack = CurrentSliderSensitivityVerticalValue;
            }
        }
    }
}
