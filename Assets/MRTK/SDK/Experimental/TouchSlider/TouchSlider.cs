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
        private PinchSlider Slider = null;
        [SerializeField]
        private TextMeshPro SliderValue = null;
        [SerializeField]
        private BoxCollider TouchableCollider = null;
        [SerializeField]
        private float SnapValue = 0.05f;
        //
        private float ColliderWidth;
        private float ColliderPosition;
        private float ColliderLeft;
        private float ColliderRight;
        public void UpdateSliderText()
        {
            if(SliderValue != null)
            {
                SliderValue.text = Slider.SliderValue.ToString();
            }
        }
        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            //throw new NotImplementedException();
        }

        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            //throw new NotImplementedException();
        }

        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            CalculateSliderPosition(eventData.InputData.x);
        }
        private void CalculateSliderPosition(float touchPoint) {
            ColliderWidth = TouchableCollider.bounds.size.x;
            ColliderPosition = TouchableCollider.gameObject.transform.position.x;
            ColliderLeft = ColliderPosition - ColliderWidth / 2;
            ColliderRight = ColliderPosition + ColliderWidth / 2;
            //
            float result = (touchPoint - ColliderLeft) / (ColliderRight - ColliderLeft);
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