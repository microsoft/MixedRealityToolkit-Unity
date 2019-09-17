// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Defines the X, Y, and Z Axes as flag-based preferences.
    /// </summary>
    /// <remarks>
    /// Useful in scenarios where constraints need to be applied to specific
    /// combinations of axes.
    /// Note that the mechanics of whether or not these values are used as
    /// a constraint (the presence of the X flag means X is not immobile)
    /// or a freedom (the presence of the X flag means X is the only free axis)
    /// is up to the caller/user of this enum.
    /// </remarks>
    public enum AxisPreference
    {
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
    }
}
