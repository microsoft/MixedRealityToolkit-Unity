//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class SliderEventData
    {
        public SliderEventData(float o, float n, IMixedRealityPointer pointer, PinchSlider slider)
        {
            OldValue = o;
            NewValue = n;
            Pointer = pointer;
            Slider = slider;
        }

        /// <summary>
        /// The previous value of the slider
        /// </summary>
        public float OldValue { get; private set; }

        /// <summary>
        /// The current value of the slider
        /// </summary>
        public float NewValue { get; private set; }

        /// <summary>
        /// The slider that triggered this event
        /// </summary>
        public PinchSlider Slider { get; private set; }

        /// <summary>
        /// The currently active pointer manipulating / hovering the slider,
        /// or null if no pointer is manipulating the slider.
        /// Note: OnSliderUpdated is called with .Pointer == null
        /// OnStart, so always check if this field is null before using!
        /// </summary>
        public IMixedRealityPointer Pointer { get; set; }
    }
}
