// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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