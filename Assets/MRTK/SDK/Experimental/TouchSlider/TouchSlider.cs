// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This control enables touching a slider with one finger to set its value
    /// </summary>
    public class TouchSlider : MonoBehaviour, IMixedRealityTouchHandler
    {
        [Experimental]
        [SerializeField]
        [Tooltip("The pinch slider that will be receiving touch input")]
        private PinchSlider Slider = null;
        [SerializeField]
        [Tooltip("Optional TextMeshPro that displays the slider value to the end user")]
        private TextMeshPro SliderValue = null;
        [SerializeField]
        [Tooltip("The collider object that receives touch input")]
        private BoxCollider TouchableCollider = null;
        [SerializeField]
        [Tooltip("Optional value to clamp the beginning / end of the slider touch area so it snaps to zero or one with ease")]
        private float SnapValue = 0.05f;
        private float ColliderWidth;
        private float ColliderPosition;
        private float ColliderLeft;
        private float ColliderRight;
        /// <summary>
        /// This can get called from a pinch slider's OnValueChanged event to display the text value
        /// </summary>
        public void UpdateSliderText()
        {
            if(SliderValue != null)
            {
                SliderValue.text = Slider.SliderValue.ToString();
            }
        }
        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
        }

        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
        }
        /// <summary>
        /// When the collider is touched, use the touch point to Calculate the Slider value
        /// </summary>
        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            CalculateSliderValueBasedOnTouchPoint(eventData.InputData.x);
        }
        private void CalculateSliderValueBasedOnTouchPoint(float touchPoint) {
            // The collider's anchor is at the centerpoint, so convert the touchpoint to slider value
            ColliderWidth = TouchableCollider.bounds.size.x;
            ColliderPosition = TouchableCollider.gameObject.transform.position.x;
            ColliderLeft = ColliderPosition - ColliderWidth / 2;
            ColliderRight = ColliderPosition + ColliderWidth / 2;
            float result = (touchPoint - ColliderLeft) / (ColliderRight - ColliderLeft);
            // clamp the value between zero and one, and also trim out the SnapValue
            float clampedResult;
            if (result < SnapValue)
            {
                clampedResult = 0;
            }
            else if (result > (1 - SnapValue))
            {
                clampedResult = 1;
            }
            else
            {
                clampedResult = result;
            }
            Slider.SliderValue = clampedResult;
        }
    }
}
