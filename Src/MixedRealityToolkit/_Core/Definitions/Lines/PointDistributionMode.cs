// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Lines
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