// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A class that functions as the value of one item in a collection. It provides the
    /// fully resolved keypath and the item index position in the collection.
    /// </summary>
    public class CollectionItemIdentifier
    {
        /// <summary>
        /// Fully resolved keypath of this item in the collection
        /// </summary>
        public string FullyResolvedKeypath { get; }

        /// <summary>
        /// The index position of the item in the collection, starting with
        /// 0 for the first position.
        /// </summary>
        public int IndexPosition { get; }

        /// <summary>
        /// Constructor that establishes the keypath and the index position of the item in the collection
        /// </summary>
        /// <param name="keyPath">They fully resolved keypath of the item</param>
        /// <param name="position">The zero-based index position of the item in the collection.</param>
        public CollectionItemIdentifier(string keyPath, int position)
        {
            FullyResolvedKeypath = keyPath;
            IndexPosition = position;
        }
    }
}
