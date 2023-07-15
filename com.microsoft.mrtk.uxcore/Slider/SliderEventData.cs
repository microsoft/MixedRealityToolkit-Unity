// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Event data for when a slider's value changes
    /// </summary>
    public class SliderEventData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderEventData"/> class.
        /// </summary>
        /// <param name="oldValue">The previous value of the slider.</param>
        /// <param name="newValue">The current value of the slider.</param>
        public SliderEventData(float oldValue, float newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// The previous value of the slider.
        /// </summary>
        public float OldValue { get; private set; }

        /// <summary>
        /// The current value of the slider.
        /// </summary>
        public float NewValue { get; private set; }
    }
}
