// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ReaderView : ushort
    {
        /// <summary>
        /// Objects that are in front of the user and visible within the field of view.
        /// </summary>
        FieldOfView = 1 << 0,

        /// <summary>
        /// Objects that are all around the user, in front, behind, above, below, etc.
        /// </summary>
        Surround = 1 << 15,
    }
}
