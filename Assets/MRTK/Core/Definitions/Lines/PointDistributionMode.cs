// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines how to get an interpolated point along a line
    /// </summary>
    public enum PointDistributionMode
    {
        /// <summary>
        /// Don't adjust placement
        /// </summary>
        None = 0,
        /// <summary>
        /// Adjust placement automatically (default)
        /// </summary>
        Auto,
        /// <summary>
        /// Place based on distance
        /// </summary>
        DistanceSingleValue,
        /// <summary>
        /// Place based on curve
        /// </summary>
        DistanceCurveValue,
    }
}