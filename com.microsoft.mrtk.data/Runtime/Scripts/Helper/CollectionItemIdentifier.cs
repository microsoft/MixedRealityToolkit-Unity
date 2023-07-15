// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

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
        /// Initializes a new instance of the <see cref="CollectionItemIdentifier"/> class.
        /// </summary>
        /// <remarks>
        /// This will establish a key path and the index position of the item in the collection.
        /// </remarks>
        /// <param name="keyPath">They fully resolved keypath of the item</param>
        /// <param name="position">The zero-based index position of the item in the collection.</param>
        public CollectionItemIdentifier(string keyPath, int position)
        {
            FullyResolvedKeypath = keyPath;
            IndexPosition = position;
        }
    }
}
#pragma warning restore CS1591