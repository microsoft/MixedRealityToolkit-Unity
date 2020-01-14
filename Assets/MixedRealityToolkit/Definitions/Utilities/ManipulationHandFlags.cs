// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Flags used to represent the number of hands that can be used in manipulation
    /// </summary>
    [System.Flags]
    public enum ManipulationHandFlags
    {
        OneHanded = 1 << 0,
        TwoHanded = 1 << 1,
    }
}