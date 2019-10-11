// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    [System.Flags]
    public enum TransformFlags
    {
        Move = 1 << 0,
        Rotate = 1 << 1,
        Scale = 1 << 2
    }
}