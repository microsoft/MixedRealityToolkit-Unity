using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    public class StepSlider : PinchSlider
    {
        [Experimental]
        [SerializeField]
        [Tooltip("Number of subdivisions of slider values.")]
        private int sliderStepDivisions = 0;
        
        public int SliderStepDivisions
        {
            get { return sliderStepDivisions; }
            set
            {
                sliderStepDivisions = value;
            }
        }

        private float sliderStepVal = 0.01f;
        private void updateStepVal()
        {
            var startVal = 0.0f;
            var endVal = 1.0f;
            sliderStepVal = (endVal - startVal) / sliderStepDivisions;
            SliderValue = (startVal + (sliderStepDivisions / 2) * sliderStepVal);
        }

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
