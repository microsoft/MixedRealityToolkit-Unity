// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for .Net Comparer's
    /// </summary>
    public static class ComparerExtensions
    {
        /// <summary>
        /// Gets a comparer that sorts elements in the opposite order of the original comparer.
        /// </summary>
        /// <typeparam name="TElement">The type of element the comparer compares.</typeparam>
        /// <param name="originalComparer">The comparer whose order should be reversed.</param>
        /// <returns>A comparer that sorts elements in the opposite order of <paramref name="originalComparer"/>.</returns>
        public static IComparer<TElement> GetReversed<TElement>(this IComparer<TElement> originalComparer)
        {
            return new ReverseComparer<TElement>(originalComparer);
        }

        private class ReverseComparer<TElement> : IComparer<TElement>
        {
            private readonly IComparer<TElement> originalComparer;

            public ReverseComparer(IComparer<TElement> originalComparer)
            {
                 Debug.Assert(originalComparer != null, "originalComparer cannot be null.");

                this.originalComparer = originalComparer;
            }

            public int Compare(TElement left, TElement right)
            {
                return originalComparer.Compare(right, left);
            }
        }
    }
}
