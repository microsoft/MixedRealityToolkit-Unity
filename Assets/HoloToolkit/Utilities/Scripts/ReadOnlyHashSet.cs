// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A wrapper for <see cref="HashSet{T}"/> that doesn't allow modification of the set. This is
    /// useful for handing out references to a set that is going to be modified internally, without
    /// giving external consumers the opportunity to accidentally modify the set.
    /// </summary>
    public class ReadOnlyHashSet<TElement> :
        ICollection<TElement>,
        IEnumerable<TElement>,
        IEnumerable
    {
        private readonly HashSet<TElement> underlyingSet;

        public ReadOnlyHashSet(HashSet<TElement> underlyingSet)
        {
            Debug.Assert(underlyingSet != null, "underlyingSet cannot be null.");

            this.underlyingSet = underlyingSet;
        }

        public int Count
        {
            get { return underlyingSet.Count; }
        }

        bool ICollection<TElement>.IsReadOnly
        {
            get { return true; }
        }

        void ICollection<TElement>.Add(TElement item)
        {
            throw NewWriteDeniedException();
        }

        void ICollection<TElement>.Clear()
        {
            throw NewWriteDeniedException();
        }

        public bool Contains(TElement item)
        {
            return underlyingSet.Contains(item);
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            underlyingSet.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return underlyingSet.GetEnumerator();
        }

        bool ICollection<TElement>.Remove(TElement item)
        {
            throw NewWriteDeniedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return underlyingSet.GetEnumerator();
        }

        private NotSupportedException NewWriteDeniedException()
        {
            return new NotSupportedException("ReadOnlyHashSet<TElement> is not directly writeable.");
        }
    }

    public static class ReadOnlyHashSetRelatedExtensions
    {
        public static ReadOnlyHashSet<TElement> AsReadOnly<TElement>(this HashSet<TElement> set)
        {
            return new ReadOnlyHashSet<TElement>(set);
        }
    }
}
