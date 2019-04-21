//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
namespace Microsoft.MixedReality.Toolkit.UI
{
    public class SliderEventData
    {
        public SliderEventData(float o, float n, bool isNear, PinchSlider slider)
        {
            OldValue = o;
            NewValue = n;
            IsNear = isNear;
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
        /// Whether the slider is being interacted near or at a distance.
        /// </summary>
        public bool IsNear { get; set; }
    }
}