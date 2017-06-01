// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    public class ReadOnlyHashSet<TElement> :
        ICollection<TElement>,
        IEnumerable<TElement>,
        IEnumerable
    {
        private readonly HashSet<TElement> underlyingSet;

        public ReadOnlyHashSet(HashSet<TElement> underlyingSet)
        {
            Assert.IsTrue(DiagnosticContext.Create(), underlyingSet != null, "underlyingSet cannot be null.");

            this.underlyingSet = underlyingSet;
        }

        public int Count
        {
            get { return underlyingSet.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Add(TElement item)
        {
            throw NewWriteDeniedException();
        }

        public void Clear()
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

        public bool Remove(TElement item)
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
