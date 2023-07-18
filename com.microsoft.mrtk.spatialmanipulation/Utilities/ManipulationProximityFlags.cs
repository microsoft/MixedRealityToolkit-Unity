// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Flags used to represent whether manipulation can be far, near or both
    /// </summary>
    [System.Flags]
    public enum ManipulationProximityFlags
    {
        /// <summary>
        /// No proximity type has been specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// A manipulation be executed by a near interactor, such as grab or touch.
        /// </summary>
        Near = 1 << 0,
        
        /// <summary>
        /// A manipulation be executed by a far interactor, such as a hand ray or gaze.
        /// </summary>
        Far = 1 << 1,
    }
}