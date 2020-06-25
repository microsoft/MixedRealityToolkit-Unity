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
        private TextMeshPro SliderValue= null;
        //[SerializeField]
        //private BoxCollider TouchableCollider = null;
        [SerializeField]
        private TextMeshPro DebugText = null;
        [SerializeField]
        private float TouchableWidth = 0.135776976f;
        [SerializeField]
        private float Multiplier = 4f;
        public void UpdateSliderText()
        {
            //SliderValue.text = Slider.SliderValue.ToString();
        }
        //void Start()
        //{
        //    DebugText.text = TouchableCollider.bounds.size.x.ToString();
        //}
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
            float position = (eventData.InputData.x + TouchableWidth) * Multiplier;
            float newPosition;
            if(position < 0.1f)
            {
                newPosition = 0;
            } else if(position > 0.95f)
            {
                newPosition = 1;
            } else
            {
                newPosition = position;
            }
            Slider.SliderValue = newPosition;
            SliderValue.text = (Mathf.Round(newPosition*100) / 100).ToString();
            DebugText.text = position.ToString() + " | " + newPosition.ToString();
        }
    }
}