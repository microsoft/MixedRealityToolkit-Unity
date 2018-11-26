// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.Collections
{
    /// <summary>
    /// Sorting type for collections
    /// </summary>
    public enum SortTypeEnum
    {
        None,                   // Don't sort, just display in order received
        Transform,              // Sort by transform order
        Alphabetical,           // Sort by transform name
        TransformReversed,      // Sort by transform order reversed
        AlphabeticalReversed,   // Sort by transform name reversed
    }
}
