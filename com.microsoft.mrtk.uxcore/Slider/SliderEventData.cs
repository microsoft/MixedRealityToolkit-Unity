//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Event data for when a slider's value changes
    /// </summary>
    public class SliderEventData
    {
        public SliderEventData(float o, float n)
        {
            OldValue = o;
            NewValue = n;
        }

        /// <summary>
        /// The previous value of the slider
        /// </summary>
        public float OldValue { get; private set; }

        /// <summary>
        /// The current value of the slider
        /// </summary>
        public float NewValue { get; private set; }
    }
}
