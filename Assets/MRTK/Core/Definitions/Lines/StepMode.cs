// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines how to generate points in a line renderer
    /// </summary>
    public enum StepMode
    {
        /// <summary>
        /// Draw points based on LineStepCount
        /// </summary>
        Interpolated = 0,
        /// <summary>
        /// Draw only the points available in the source - use this for hard edges
        /// </summary>
        FromSource,
    }
}