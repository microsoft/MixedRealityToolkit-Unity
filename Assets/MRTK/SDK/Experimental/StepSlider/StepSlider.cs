using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A slider with a fixed number of step values that can be moved by grabbing / pinching a slider thumb
    /// Number of steps defaults to 0
    /// </summary>
    [AddComponentMenu("MRTK/SDK/Experimental/StepSlider")]
    public class StepSlider : PinchSlider, IMixedRealityPointerHandler
    {

        #region Serialized Fields and Properties
        [Experimental]
        [SerializeField]
        [Tooltip("Number of subdvisions the slider is split into.")]
        
        private int sliderStepDivisions = 0;
        /// <summary>
        /// Property accessor of sliderStepDivisions, it holds the number of subdvisions the slider is split into.
        /// </summary>
        public int SliderStepDivisions
        {
            get 
            { 
                return sliderStepDivisions; 
            }
            set
            {
                sliderStepDivisions = value;
            }
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// Private member used to adjust slider values
        /// </summary>
        protected float sliderStepVal = 0.01f;
        #endregion

        #region Unity methods
        new void Start()
        {
            UpdateStepVal();
            CheckSliderInit();
            base.Start();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Private method used to calculate and update the slider step divisions based on the sliderStepDivisions provided 
        /// Slider value initialized to provided sliderInitStep value
        /// </summary>
        private void UpdateStepVal()
        {
            if (SliderStepDivisions >= 1)
            {
                var startVal = 0.0f;
                var endVal = 1.0f;
                sliderStepVal = (endVal - startVal) / sliderStepDivisions;
            }
        }

        /// <summary>
        /// Private method used to adjust initial slider value to stepwise values
        /// </summary>
        private void CheckSliderInit()
        {
            var percent = SliderValue / sliderStepVal;
            var value = ((sliderStepVal * Mathf.FloorToInt(percent)));
            SliderValue = Mathf.Clamp(value, 0.0f, 1.0f);
        }
        #endregion

        #region IMixedRealityPointerHandler

        /// <summary>
        /// Called every frame a pointer is down. Can be used to implement drag-like behaviors.
        /// Uses member sliderStepVal to move up and down the steps on the slider.
        /// </summary>
        public new void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                var delta = activePointer.Position - startPointerPosition;
                var handDelta = Vector3.Dot(SliderTrackDirection.normalized, delta);
                var stepVal = (handDelta / SliderTrackDirection.magnitude > 0) ? sliderStepVal : (sliderStepVal * -1);
                var stepMag = Mathf.Floor(Mathf.Abs((handDelta / SliderTrackDirection.magnitude)) / sliderStepVal);
                SliderValue = Mathf.Clamp(startSliderValue + (stepVal * stepMag), 0, 1);

                // Mark the pointer data as used to prevent other behaviors from handling input events
                eventData.Use();
            }
        }

        #endregion
    }
}
