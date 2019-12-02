// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Values for user-friendly dimensions settings
    /// </summary>
    public enum SelectionModes
    {
        /// <summary>
        /// Not a valid mode
        /// </summary>
        Invalid = -1,
        /// <summary>
        /// Just click, no selection
        /// </summary>
        Button,
        /// <summary>
        /// A selection with two dimensions, selected/unselection.
        /// A two mode switch, checkbox, toggle, radial
        /// </summary>
        Toggle,
        /// <summary>
        /// A selection with more than two dimensions,
        /// like one control for Small, Medium, and Large
        /// </summary>
        MultiDimension
    };
}
