// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Flags used to represent a combination of different types of transformation
    /// </summary>
    [System.Flags]
    public enum TransformFlags
    {
        Move = 1 << 0,
        Rotate = 1 << 1,
        Scale = 1 << 2
    }
}