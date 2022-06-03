// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Flags used to specify a set of one or more 3D axes.
    /// </summary>
    [System.Flags]
    public enum AxisFlags
    {
        /// <summary>
        /// The horizontal axis.
        /// </summary>
        XAxis = 1 << 0,

        /// <summary>
        /// The vertical axis.
        /// </summary>
        YAxis = 1 << 1,

        /// <summary>
        /// The depth axis.
        /// </summary>
        ZAxis = 1 << 2
    }
}