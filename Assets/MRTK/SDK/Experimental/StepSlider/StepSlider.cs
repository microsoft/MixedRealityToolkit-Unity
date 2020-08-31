using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    [AddComponentMenu("MRTK/SDK/Experimental/StepSlider")]
    public class StepSlider : PinchSlider, IMixedRealityPointerHandler
    {

        #region Serialized Fields and Properties
        [Experimental]
        [SerializeField]
        [Tooltip("Number of subdivisions of slider values.")]
        
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
            updateStepVal();
            base.Start();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Private method used to calculate and update the slider step divisions based on the number provided 
        /// </summary>
        private void updateStepVal()
        {
            if (SliderStepDivisions >= 1)
            {
                var startVal = 0.0f;
                var endVal = 1.0f;
                sliderStepVal = (endVal - startVal) / sliderStepDivisions;
                SliderValue = (startVal + (sliderStepDivisions / 2) * sliderStepVal);
            }
        }
        #endregion

        #region IMixedRealityPointerHandler

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
