// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    /// <summary>
    /// Sorting type for collections
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// Don't sort, just display in order received
        /// </summary>
        None,
        /// <summary>
        /// Sort by transform order
        /// </summary>
        Transform,
        /// <summary>
        /// Sort by transform name
        /// </summary>
        Alphabetical,
        /// <summary>
        /// Sort by transform order reversed
        /// </summary>
        TransformReversed,
        /// <summary>
        /// Sort by transform name reversed
        /// </summary>
        AlphabeticalReversed
    }
}
