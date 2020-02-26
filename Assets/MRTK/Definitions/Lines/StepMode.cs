// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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