// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Flags used to represent a set of 3D axes
    /// </summary>
    [System.Flags]
    public enum AxisFlags
    {
        XAxis = 1 << 0,
        YAxis = 1 << 1,
        ZAxis = 1 << 2
    }
}