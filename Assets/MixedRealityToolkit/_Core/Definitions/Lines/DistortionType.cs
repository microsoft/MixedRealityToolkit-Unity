// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines
{
    public enum DistortionType
    {
        NormalizedLength,   // Use the normalized length of the line plus its distortion strength curve to determine distortion strength
        Uniform,            // Use a single value to determine distortion strength
    }
}