// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Collation order type used for sorting
    /// </summary>
    public enum CollationOrder
    {
        /// <summary>
        /// Don't sort, just display in order received
        /// </summary>
        None = 0,
        /// <summary>
        /// Sort by child order of parent
        /// </summary>
        ChildOrder,
        /// <summary>
        /// Sort by transform name
        /// </summary>
        Alphabetical,
        /// <summary>
        /// Sort by child order of parent, reversed
        /// </summary>
        ChildOrderReversed,
        /// <summary>
        /// Sort by transform name, reversed
        /// </summary>
        AlphabeticalReversed
    }
}
