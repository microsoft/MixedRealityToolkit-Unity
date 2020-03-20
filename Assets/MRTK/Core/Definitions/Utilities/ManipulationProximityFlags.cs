// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Flags used to represent whether manipulation can be far, near or both
    /// </summary>
    [System.Flags]
    public enum ManipulationProximityFlags
    {
        Near = 1 << 0,
        Far = 1 << 1,
    }
}