// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

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
        private PinchSlider slider = null;

        [SerializeField]
        [Tooltip("Optional TextMeshPro that displays the slider value to the end user")]
        private TextMeshPro sliderValue = null;

        [SerializeField]
        [Tooltip("The collider object that receives touch input")]
        private BoxCollider touchableCollider = null;

        [SerializeField]
        [Tooltip("Optional value to clamp the beginning / end of the slider touch area so it snaps to zero or one with ease")]
        [Range(0, 0.5f)]
        private float snapValue = 0.05f;

        private float colliderWidth;
        private float colliderPosition;
        private float colliderLeft;
        private float colliderRight;

        /// <summary>
        /// This can get called from a pinch slider's OnValueChanged event to display the text value
        /// </summary>
        public void UpdateSliderText()
        {
            if (sliderValue != null)
            {
                sliderValue.text = slider.SliderValue.ToString();
            }
        }

        public void OnTouchStarted(HandTrackingInputEventData eventData) { }

        public void OnTouchCompleted(HandTrackingInputEventData eventData) { }

        /// <summary>
        /// When the collider is touched, use the touch point to Calculate the Slider value
        /// </summary>
        public void OnTouchUpdated(HandTrackingInputEventData eventData) => CalculateSliderValueBasedOnTouchPoint(eventData.InputData.x);

        private void CalculateSliderValueBasedOnTouchPoint(float touchPoint)
        {
            // The collider's anchor is at the centerpoint, so convert the touchpoint to slider value
            colliderWidth = touchableCollider.bounds.size.x;
            colliderPosition = touchableCollider.gameObject.transform.position.x;
            colliderLeft = colliderPosition - colliderWidth / 2;
            colliderRight = colliderPosition + colliderWidth / 2;
            float result = (touchPoint - colliderLeft) / (colliderRight - colliderLeft);
            // clamp the value between zero and one, and also trim out the SnapValue
            float clampedResult;
            if (result < snapValue)
            {
                clampedResult = 0;
            }
            else if (result > (1 - snapValue))
            {
                clampedResult = 1;
            }
            else
            {
                clampedResult = result;
            }
            slider.SliderValue = clampedResult;
        }
    }
}
