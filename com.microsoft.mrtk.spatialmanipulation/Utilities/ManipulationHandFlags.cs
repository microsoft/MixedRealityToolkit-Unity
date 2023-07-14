// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Flags used to represent the number of hands that can be used in manipulation
    /// </summary>
    [System.Flags]
    public enum ManipulationHandFlags
    {
        /// <summary>
        /// No hand count has been specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// A manipulation being executed by a single hand.
        /// </summary>
        OneHanded = 1 << 0,

        /// <summary>
        /// A manipulation being executed by two hands.
        /// </summary>
        TwoHanded = 1 << 1,
    }
}