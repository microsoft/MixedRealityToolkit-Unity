// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// How to apply the distortion along the line.
    /// </summary>
    public enum DistortionMode
    {
        /// <summary>
        /// Use the normalized length of the line plus its distortion strength curve to determine distortion strength
        /// </summary>
        NormalizedLength = 0,
        /// <summary>
        /// Use a single value to determine distortion strength
        /// </summary>
        Uniform,
    }
}