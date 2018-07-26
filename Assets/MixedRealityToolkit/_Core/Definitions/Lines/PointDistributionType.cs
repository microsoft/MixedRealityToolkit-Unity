// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines
{
    /// <summary>
    /// Default options for getting an interpolated point along a line
    /// </summary>
    public enum PointDistributionType
    {
        None,                       // Don't adjust placement
        Auto,                       // Adjust placement automatically (default)
        DistanceSingleValue,        // Place based on distance
        DistanceCurveValue,         // Place based on curve
    }
}