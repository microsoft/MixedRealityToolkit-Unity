// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Lines
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